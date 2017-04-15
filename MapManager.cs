using Assets.Scripts.GUILogic.View.BattleEquipment;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using MobaProtocol.Data;
using Newbie;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MapManager : BaseGameModule
{
	private readonly Dictionary<int, Units> _mapUnits = new Dictionary<int, Units>();

	private readonly Dictionary<int, Units> _recycledUnits = new Dictionary<int, Units>();

	private readonly List<Transform> _mapObjects = new List<Transform>();

	private readonly Dictionary<TeamType, Dictionary<int, List<Units>>> _tagUnits = new Dictionary<TeamType, Dictionary<int, List<Units>>>
	{
		{
			TeamType.LM,
			new Dictionary<int, List<Units>>()
		},
		{
			TeamType.BL,
			new Dictionary<int, List<Units>>()
		},
		{
			TeamType.Neutral,
			new Dictionary<int, List<Units>>()
		},
		{
			TeamType.Team_3,
			new Dictionary<int, List<Units>>()
		}
	};

	private readonly Dictionary<TeamType, Units> _homes = new Dictionary<TeamType, Units>();

	private readonly Dictionary<TeamType, Dictionary<string, List<int>>> _teamGroupCountDict = new Dictionary<TeamType, Dictionary<string, List<int>>>
	{
		{
			TeamType.LM,
			new Dictionary<string, List<int>>()
		},
		{
			TeamType.BL,
			new Dictionary<string, List<int>>()
		},
		{
			TeamType.Neutral,
			new Dictionary<string, List<int>>()
		},
		{
			TeamType.Team_3,
			new Dictionary<string, List<int>>()
		}
	};

	private readonly List<Units> _allHeroUnits = new List<Units>();

	private GameObject _spawnPool;

	private PoolRoot _poolRoot;

	private bool _isInitMap;

	private readonly CoroutineManager _coroutineManager = new CoroutineManager();

	private readonly SimpleIdAlloc _idAlloc = new SimpleIdAlloc();

	private readonly MapPointContainer _mapPointContainer = new MapPointContainer();

	private ResourceSpawner _resSpawner;

	public event Action<Units> OnAddUnit
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnAddUnit = (Action<Units>)Delegate.Combine(this.OnAddUnit, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnAddUnit = (Action<Units>)Delegate.Remove(this.OnAddUnit, value);
		}
	}

	public event Action<Units> OnRemoveUnit
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnRemoveUnit = (Action<Units>)Delegate.Combine(this.OnRemoveUnit, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnRemoveUnit = (Action<Units>)Delegate.Remove(this.OnRemoveUnit, value);
		}
	}

	public static MapManager Instance
	{
		get
		{
			if (GameManager.Instance == null)
			{
				return null;
			}
			return GameManager.Instance.MapManager;
		}
	}

	public IKeyAlloc<int> KeyAlloc
	{
		get
		{
			return this._idAlloc;
		}
	}

	public PoolRoot PoolRoot
	{
		get
		{
			return this._poolRoot;
		}
		set
		{
			this._poolRoot = value;
		}
	}

	public MapPointContainer MapPointContainer
	{
		get
		{
			return this._mapPointContainer;
		}
	}

	public override void Init()
	{
		if (this._isInitMap)
		{
			return;
		}
		this.CreateSpawnPool();
		this._mapPointContainer.Init();
		this._isInitMap = true;
		this._idAlloc.Reset();
		MobaMessageManager.RegistMessage((ClientMsg)25038, new MobaMessageFunc(this.OnSpawnFinished));
	}

	public override void Uninit()
	{
		if (!this._isInitMap)
		{
			return;
		}
		this._isInitMap = false;
		this._recycledUnits.Clear();
		this._coroutineManager.StopAllCoroutine();
		this.ClearMap();
		this._mapPointContainer.Uninit();
		this.DestroySpawnPool();
		MobaMessageManager.UnRegistMessage((ClientMsg)25038, new MobaMessageFunc(this.OnSpawnFinished));
	}

	public Transform GetSpawnPos(TeamType team, int key)
	{
		return this._mapPointContainer.GetSpawnPos(team, key);
	}

	public Dictionary<string, Transform> GetGuidePoints(TeamType team)
	{
		return this._mapPointContainer.GetGuidePoints(team);
	}

	public Transform GetGuidePoint(TeamType team, int line, int key)
	{
		return this._mapPointContainer.GetGuidePoint(team, line, key);
	}

	private void DestroyRecycleUnits()
	{
		foreach (Units current in this._recycledUnits.Values)
		{
			current.NeedRecycle = false;
			SpawnPool pool = this._poolRoot.GetPool((TeamType)current.teamType);
			if (pool != null)
			{
				pool.Despawn(current.transform);
			}
		}
		this._recycledUnits.Clear();
	}

	private void CreateSpawnPool()
	{
		if (this._spawnPool != null)
		{
			return;
		}
		this._spawnPool = (UnityEngine.Object.Instantiate(Resources.Load("Prefab/Manager/SpawnerPool")) as GameObject);
		this._poolRoot = this._spawnPool.GetComponent<PoolRoot>();
		this._resSpawner = new ResourceSpawner(this._poolRoot);
	}

	private void DestroySpawnPool()
	{
		this._resSpawner.Shutdown();
		if (this._spawnPool != null)
		{
			UnityEngine.Object.Destroy(this._spawnPool);
			this._spawnPool = null;
			this._poolRoot = null;
		}
		RecyclerManager.Exit();
	}

	[DebuggerHidden]
	private IEnumerator DelayDestroyPool()
	{
		return new MapManager.<DelayDestroyPool>c__Iterator1B1();
	}

	private void OnSpawnFinished(MobaMessage msg)
	{
		this._homes.Clear();
		List<Units> mapUnits = this.GetMapUnits(global::TargetTag.Home);
		if (mapUnits != null)
		{
			for (int i = 0; i < mapUnits.Count; i++)
			{
				this.SetHome(mapUnits[i]);
			}
		}
	}

	public IList<Units> GetTower(TeamType teamType = TeamType.None)
	{
		return this.GetMapUnits(teamType, global::TargetTag.Tower);
	}

	public Units GetHome(TeamType teamType = TeamType.None)
	{
		if (this._homes.ContainsKey(teamType))
		{
			return this._homes[teamType];
		}
		return null;
	}

	private void SetHome(Units target)
	{
		if (target == null)
		{
			return;
		}
		this._homes[(TeamType)target.teamType] = target;
		target.tag = "Home";
	}

	public Dictionary<int, Units> GetAllMapUnits()
	{
		return this._mapUnits;
	}

	public Dictionary<int, List<Units>> GetTagUnits(TeamType team)
	{
		if (this._tagUnits.ContainsKey(team))
		{
			return this._tagUnits[team];
		}
		return null;
	}

	public IList<Units> GetMapUnits(TeamType teamType, global::TargetTag targetTag)
	{
		IList<Units> list = null;
		if (teamType != TeamType.None)
		{
			Dictionary<int, List<Units>> tagUnits = this.GetTagUnits(teamType);
			if (tagUnits.ContainsKey((int)targetTag))
			{
				list = new ReadOnlyCollection<Units>(tagUnits[(int)targetTag]);
			}
		}
		else
		{
			list = new List<Units>();
			Dictionary<int, Units>.Enumerator enumerator = this._mapUnits.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Units> current = enumerator.Current;
				Units value = current.Value;
				if (!(value == null) && value.isLive)
				{
					if (TeamManager.CheckTeamType(value.teamType, (int)teamType))
					{
						if (TagManager.CheckTag(value, targetTag))
						{
							list.Add(value);
						}
					}
				}
			}
		}
		return list;
	}

	[DebuggerHidden]
	public IEnumerable<Units> EnumMapUnits(IList<TeamType> teamTypes, global::TargetTag targetTag)
	{
		MapManager.<EnumMapUnits>c__Iterator1B2 <EnumMapUnits>c__Iterator1B = new MapManager.<EnumMapUnits>c__Iterator1B2();
		<EnumMapUnits>c__Iterator1B.teamTypes = teamTypes;
		<EnumMapUnits>c__Iterator1B.targetTag = targetTag;
		<EnumMapUnits>c__Iterator1B.<$>teamTypes = teamTypes;
		<EnumMapUnits>c__Iterator1B.<$>targetTag = targetTag;
		<EnumMapUnits>c__Iterator1B.<>f__this = this;
		MapManager.<EnumMapUnits>c__Iterator1B2 expr_2A = <EnumMapUnits>c__Iterator1B;
		expr_2A.$PC = -2;
		return expr_2A;
	}

	public IEnumerable<Units> EnumEnemyMapUnits(TeamType self, global::TargetTag targetTag)
	{
		return this.EnumMapUnits(TeamManager.GetEnemyTeams(self), targetTag);
	}

	public List<Units> GetMapUnits(global::TargetTag targetTag)
	{
		List<Units> list = new List<Units>();
		Dictionary<int, Units>.Enumerator enumerator = this._mapUnits.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, Units> current = enumerator.Current;
			Units value = current.Value;
			if (!(value == null) && value.isLive)
			{
				if (TagManager.CheckTag(value, targetTag))
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public Units GetUnit(int uniqueId)
	{
		Units result = null;
		this._mapUnits.TryGetValue(uniqueId, out result);
		return result;
	}

	public Units GetPlayer()
	{
		Units units = PlayerControlMgr.Instance.GetPlayer();
		if (!units || !units.isPlayer)
		{
			units = this._mapUnits.Values.FirstOrDefault((Units unit) => unit.isPlayer);
		}
		return units;
	}

	public bool GetPlayerSeeEnemySoldierInfo(Units inPlayerHero, List<int> outEnemySoldierIds, GameObject inHintPrefab)
	{
		if (inPlayerHero == null || outEnemySoldierIds == null)
		{
			return false;
		}
		if (this._tagUnits == null)
		{
			return false;
		}
		Dictionary<int, List<Units>> dictionary = null;
		if (!this._tagUnits.TryGetValue(TeamType.BL, out dictionary))
		{
			return false;
		}
		if (dictionary == null)
		{
			return false;
		}
		List<Units> list = null;
		if (!dictionary.TryGetValue(3, out list))
		{
			return false;
		}
		if (list == null)
		{
			return false;
		}
		Camera main = Camera.main;
		if (main == null)
		{
			return false;
		}
		Units units = null;
		for (int i = 0; i < list.Count; i++)
		{
			Units units2 = list[i];
			if (units2 != null && units2.mTransform != null)
			{
				Vector3 vector = main.WorldToViewportPoint(units2.mTransform.position);
				if (vector.x > 0.1f && vector.x < 0.9f && vector.y > 0.1f && vector.y < 0.9f)
				{
					units = units2;
					break;
				}
			}
		}
		if (units == null)
		{
			return false;
		}
		outEnemySoldierIds.Add(units.unique_id);
		if (inHintPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(inHintPrefab) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.parent = units.gameObject.transform;
				gameObject.transform.localPosition = new Vector3(0f, 0.006f, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				units.SetNewbieHintObj(gameObject);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			Units units2 = list[j];
			if (units2 != null && units2.mTransform != null && units.mTransform != null && (units2.mTransform.position - units.mTransform.position).sqrMagnitude < 56.25f)
			{
				outEnemySoldierIds.Add(units2.unique_id);
				if (inHintPrefab != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(inHintPrefab) as GameObject;
					if (gameObject != null)
					{
						gameObject.transform.parent = units2.gameObject.transform;
						gameObject.transform.localPosition = new Vector3(0f, 0.006f, 0f);
						gameObject.transform.localRotation = Quaternion.identity;
						gameObject.transform.localScale = Vector3.one;
						units2.SetNewbieHintObj(gameObject);
					}
				}
			}
		}
		return true;
	}

	public bool CheckEnemySoldierAllDead(List<int> inSoldierIds)
	{
		if (inSoldierIds == null || inSoldierIds.Count < 1)
		{
			return false;
		}
		for (int i = 0; i < inSoldierIds.Count; i++)
		{
			if (this._mapUnits.ContainsKey(inSoldierIds[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckEnemyTowerDead(int inTowerId)
	{
		Units units = null;
		return !this._mapUnits.TryGetValue(inTowerId, out units) || !(units != null) || !units.isLive;
	}

	public bool GetPlayerSeeEnemyTowerInfo(GameObject inHintPrefab, out int outSeeEnemyTowerId)
	{
		outSeeEnemyTowerId = 0;
		if (this._tagUnits == null)
		{
			return false;
		}
		Dictionary<int, List<Units>> dictionary = null;
		if (!this._tagUnits.TryGetValue(TeamType.BL, out dictionary))
		{
			return false;
		}
		if (dictionary == null)
		{
			return false;
		}
		List<Units> list = null;
		if (!dictionary.TryGetValue(4, out list))
		{
			return false;
		}
		if (list == null)
		{
			return false;
		}
		Camera main = Camera.main;
		if (main == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Units units = list[i];
			if (units != null && units.mTransform != null)
			{
				Vector3 vector = main.WorldToViewportPoint(units.mTransform.position);
				if (vector.x > 0.1f && vector.x < 0.9f && vector.y > 0.1f && vector.y < 0.9f)
				{
					outSeeEnemyTowerId = units.unique_id;
					if (inHintPrefab != null)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(inHintPrefab) as GameObject;
						if (gameObject != null)
						{
							gameObject.transform.parent = units.gameObject.transform;
							gameObject.transform.localPosition = new Vector3(0f, 0.006f, 0f);
							gameObject.transform.localRotation = Quaternion.identity;
							gameObject.transform.localScale = new Vector3(3f, 3f, 3f);
							NewbieManager.Instance.SetEleBatFiveFirSeeTowerHintObj(gameObject);
						}
					}
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckPlayerInTowerAtkRange(Units inPlayerHero)
	{
		if (inPlayerHero == null || inPlayerHero.mTransform == null)
		{
			return false;
		}
		if (this._tagUnits == null)
		{
			return false;
		}
		Dictionary<int, List<Units>> dictionary = null;
		if (!this._tagUnits.TryGetValue(TeamType.BL, out dictionary))
		{
			return false;
		}
		if (dictionary == null)
		{
			return false;
		}
		List<Units> list = null;
		if (!dictionary.TryGetValue(4, out list))
		{
			return false;
		}
		if (list == null)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			Units units = list[i];
			if (units != null && units.isLive && units.mTransform != null)
			{
				float attackRange = units.GetAttackRange(null);
				if ((units.mTransform.position - inPlayerHero.mTransform.position).sqrMagnitude < attackRange * attackRange)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool BatFiveCheckLineTowersDead()
	{
		if (this._tagUnits == null)
		{
			return false;
		}
		Dictionary<int, List<Units>> dictionary = null;
		if (!this._tagUnits.TryGetValue(TeamType.BL, out dictionary))
		{
			return false;
		}
		if (dictionary == null)
		{
			return false;
		}
		List<Units> list = null;
		if (!dictionary.TryGetValue(4, out list))
		{
			return false;
		}
		if (list == null)
		{
			return false;
		}
		Vector3 b = new Vector3(-18.77f, 0f, 48.99f);
		Vector3 b2 = new Vector3(2.34f, 0f, 46.57f);
		Vector3 b3 = new Vector3(25.31f, 0f, 48.35f);
		Vector3 b4 = new Vector3(12.16f, 0f, 9.01f);
		Vector3 b5 = new Vector3(19.96f, 0f, 22.67f);
		Vector3 b6 = new Vector3(32.72f, 0f, 32.5f);
		Vector3 b7 = new Vector3(49.07f, 0f, -18.94f);
		Vector3 b8 = new Vector3(46.65f, 0f, 2.35f);
		Vector3 b9 = new Vector3(48.56f, 0f, 25.22f);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		for (int i = 0; i < list.Count; i++)
		{
			Units units = list[i];
			if (units != null && units.mTransform != null)
			{
				Vector3 position = units.mTransform.position;
				if (!flag)
				{
					if (units.isLive && (position - b).sqrMagnitude < 1f)
					{
						flag = true;
						goto IL_31A;
					}
					if (units.isLive && (position - b2).sqrMagnitude < 1f)
					{
						flag = true;
						goto IL_31A;
					}
					if (units.isLive && (position - b3).sqrMagnitude < 1f)
					{
						flag = true;
						goto IL_31A;
					}
				}
				if (!flag2)
				{
					if (units.isLive && (position - b4).sqrMagnitude < 1f)
					{
						flag2 = true;
						goto IL_31A;
					}
					if (units.isLive && (position - b5).sqrMagnitude < 1f)
					{
						flag2 = true;
						goto IL_31A;
					}
					if (units.isLive && (position - b6).sqrMagnitude < 1f)
					{
						flag2 = true;
						goto IL_31A;
					}
				}
				if (!flag3)
				{
					if (units.isLive && (position - b7).sqrMagnitude < 1f)
					{
						flag3 = true;
					}
					else if (units.isLive && (position - b8).sqrMagnitude < 1f)
					{
						flag3 = true;
					}
					else if (units.isLive && (position - b9).sqrMagnitude < 1f)
					{
						flag3 = true;
					}
				}
			}
			IL_31A:;
		}
		return !flag || !flag2 || !flag3;
	}

	public Hero CreateFenShen(Units mainHero, float LifeTime = 10f, int aiType = 1)
	{
		EntityVo npcinfo = new EntityVo(EntityType.Hero, mainHero.npc_id, 0, 0, string.Empty, "Default", 0);
		SpawnUtility spawnUtility = GameManager.Instance.Spawner.GetSpawnUtility();
		Hero hero = spawnUtility.SpawnInstance(npcinfo, "Hero", (TeamType)mainHero.teamType, 0, "[]", null, UnitControlType.None, UnitType.None) as Hero;
		if (hero != null)
		{
			if (aiType == 1)
			{
				hero.aiManager.ChangeTargetingSystem(mainHero);
			}
			else
			{
				hero.SetAICool(3.40282347E+38f, false);
			}
			hero.SetCanSkill(false);
			this.CopyAttrToFenShen(mainHero, hero);
			hero.trans.position = mainHero.trans.position;
			hero.data.SetDamageMultiple(3f);
			hero.SetMirrorState(true);
			hero.m_fLiveTime = LifeTime;
		}
		return hero;
	}

	private void CopyAttrToFenShen(Units master, Units fenShen)
	{
		if (fenShen == null || master == null)
		{
			return;
		}
		DataUpdateInfo dataUpdateInfo = new DataUpdateInfo();
		dataUpdateInfo.dataKeys = new List<short>();
		dataUpdateInfo.dataValues = new List<float>();
		for (AttrType attrType = AttrType.None; attrType < AttrType.End; attrType++)
		{
			dataUpdateInfo.dataKeys.Add((short)attrType);
			float attr = master.data.GetAttr(DataManager.DataPoolType.CurrentData, attrType);
			dataUpdateInfo.dataValues.Add(attr);
		}
		fenShen.data.SetAttrVal(dataUpdateInfo);
		fenShen.level = master.level;
		fenShen.summonerName = master.summonerName;
		this.CopyBuff(master, fenShen);
		fenShen.data.CopyAllData(master);
	}

	private void CopyBuff(Units master, Units fenShen)
	{
		Dictionary<string, BuffVo> allBuffs = master.buffManager.GetAllBuffs();
		Dictionary<string, BuffVo>.Enumerator enumerator = allBuffs.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BuffManager arg_41_0 = fenShen.buffManager;
			KeyValuePair<string, BuffVo> current = enumerator.Current;
			string arg_41_1 = current.Key;
			KeyValuePair<string, BuffVo> current2 = enumerator.Current;
			arg_41_0.AddBuff(arg_41_1, current2.Value.casterUnit);
			BuffManager arg_71_0 = fenShen.buffManager;
			KeyValuePair<string, BuffVo> current3 = enumerator.Current;
			string arg_71_1 = current3.Key;
			KeyValuePair<string, BuffVo> current4 = enumerator.Current;
			arg_71_0.SetBuffCDTime(arg_71_1, current4.Value.totalTime);
		}
	}

	public Transform SpawnUI(string resId, Transform parent)
	{
		Transform transform = null;
		if (resId != null)
		{
			if (!StringUtils.CheckValid(resId))
			{
				return null;
			}
			GameObject gameObject = ResourceManager.Load<GameObject>(resId, true, true, null, 0, false);
			if (gameObject == null)
			{
				ClientLogger.Error("Error resID=" + resId);
				return null;
			}
			SpawnPool poolByPrefab = this._poolRoot.GetPoolByPrefab(gameObject);
			transform = poolByPrefab.Spawn(gameObject.transform);
			if (parent != null)
			{
				transform.parent = parent;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.Euler(Vector3.zero);
				transform.localScale = Vector3.one;
			}
		}
		return transform;
	}

	public ResourceHandle SpawnResourceHandle(string resId, Vector3 position, Quaternion rotation, int skin = 0)
	{
		return this._resSpawner.SpawnResource(resId, position, rotation, skin);
	}

	public ResourceHandle SpawnResourceHandle(string resId, Action<GameObject> onFirstLoad = null, int skin = 0)
	{
		return this._resSpawner.SpawnResource(resId, onFirstLoad, skin);
	}

	public ResourceHandle SpawnResourceHandleFromPath(string path, Action<GameObject> onFirstLoad = null)
	{
		return this._resSpawner.SpawnResourceFromPath(path, onFirstLoad);
	}

	public Units RespawnUnit(int uniqueId, string tag, Dictionary<DataType, object> unitData, Vector3 position, Quaternion rotation, int skillPointLeft, Dictionary<string, int> skillLevelInfos, UnitControlType unitControlType = UnitControlType.None)
	{
		int num = unitData.GetDictValue(DataType.TeamType, -1);
		string dictValue = unitData.GetDictValue(DataType.NameId, null);
		if (dictValue == null || tag == null)
		{
			ClientLogger.Error("SpawnUnit : 参数错误! 不能生成单位!");
			return null;
		}
		if (num == -1)
		{
			ClientLogger.Error(string.Concat(new object[]
			{
				"SpawnUnit : 未指定阵营!! 默认为lm，",
				uniqueId,
				" #",
				dictValue
			}));
			num = 0;
		}
		int dictValue2 = unitData.GetDictValue(DataType.AIType, 0);
		float dictValue3 = unitData.GetDictValue(DataType.AttrFactor, 1f);
		int dictValue4 = unitData.GetDictValue(DataType.Level, 1);
		int dictValue5 = unitData.GetDictValue(DataType.Quality, 1);
		int dictValue6 = unitData.GetDictValue(DataType.Star, 1);
		float dictValue7 = unitData.GetDictValue((DataType)1, 1f);
		float dictValue8 = unitData.GetDictValue((DataType)2, 1f);
		int dictValue9 = unitData.GetDictValue(DataType.Exp, 0);
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			unitControlType = Singleton<PvpManager>.Instance.GetControlType(uniqueId);
		}
		Units units = null;
		if (tag != null)
		{
			if (MapManager.<>f__switch$map37 == null)
			{
				MapManager.<>f__switch$map37 = new Dictionary<string, int>(7)
				{
					{
						"Monster",
						0
					},
					{
						"Home",
						0
					},
					{
						"Building",
						0
					},
					{
						"Hero",
						0
					},
					{
						"Player",
						0
					},
					{
						"Item",
						1
					},
					{
						"BuffItem",
						1
					}
				};
			}
			int num2;
			if (MapManager.<>f__switch$map37.TryGetValue(tag, out num2))
			{
				if (num2 == 0)
				{
					units = this.ForceRespawnUnit(uniqueId);
					if (units)
					{
						Transform transform = units.transform;
						transform.position = position;
						transform.rotation = rotation;
					}
					goto IL_21D;
				}
				if (num2 == 1)
				{
					units = this.ForceRespawnUnit(uniqueId);
					if (units)
					{
						Transform transform2 = units.transform;
						transform2.position = position;
						transform2.rotation = rotation;
						transform2.name = this.GetName(transform2.name, num);
					}
					goto IL_21D;
				}
			}
		}
		ClientLogger.Error("unkown tag: " + tag);
		IL_21D:
		if (!units)
		{
			return null;
		}
		units.npc_id = dictValue;
		units.tag = tag;
		units.transform.tag = tag;
		units.unique_id = uniqueId;
		units.teamType = num;
		units.spwan_pos = position;
		units.spwan_rotation = rotation;
		units.attr_factor = dictValue3;
		units.aiType = dictValue2;
		units.star = dictValue6;
		units.quality = dictValue5;
		units.level = dictValue4;
		units.exp_cur = dictValue9;
		units.unitControlType = unitControlType;
		units.hp_init_p = dictValue7;
		units.mp_init_p = dictValue8;
		units.UnitInit(false);
		units.skillManager.SkillPointsLeft = skillPointLeft;
		string[] skillIDs = units.skillManager.GetSkillIDs();
		string[] array = skillIDs;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (skillLevelInfos.ContainsKey(text))
			{
				units.skillManager.SetSkillLevel(text, skillLevelInfos[text]);
			}
		}
		this.AddToMap(units);
		units.UnitStart();
		return units;
	}

	public bool ReliveNeutralMonster(Units target, string newNpcId, int rebornTeam)
	{
		if (target == null)
		{
			return false;
		}
		target.UnitStop();
		target.ChangeTeam((TeamType)rebornTeam);
		target.isLive = false;
		Task task = new Task(this.DelayRelive(target, newNpcId, rebornTeam), true);
		return true;
	}

	[DebuggerHidden]
	private IEnumerator DelayRelive(Units target, string newNpcId, int rebornTeam)
	{
		MapManager.<DelayRelive>c__Iterator1B3 <DelayRelive>c__Iterator1B = new MapManager.<DelayRelive>c__Iterator1B3();
		<DelayRelive>c__Iterator1B.target = target;
		<DelayRelive>c__Iterator1B.newNpcId = newNpcId;
		<DelayRelive>c__Iterator1B.<$>target = target;
		<DelayRelive>c__Iterator1B.<$>newNpcId = newNpcId;
		return <DelayRelive>c__Iterator1B;
	}

	public Units SpawnUnit(string tag, Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Vector3 position, Quaternion rotation, UnitControlType unitControlType = UnitControlType.None, bool reuse = false, Units parentUnti = null, UnitType unitType = UnitType.None)
	{
		Units units = this.SpawnUnit_Internal(tag, unitData, unitAttrs, position, rotation, unitControlType, reuse);
		if (units != null)
		{
			units.UnitType = unitType;
			if (units.UnitType == UnitType.FenShenHero)
			{
				units.SetMirrorState(true);
			}
			units.UnitInit(false);
			this.AddToMap(units);
			units.UnitStart();
		}
		return units;
	}

	private GameObject GetSkinnedHero(TeamType teamType, string heroResId)
	{
		SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(heroResId);
		string heroSkinPath = HeroSkins.GetHeroSkinPath(heroResId, HeroSkins.GetHeroSkin(teamType, heroResId));
		return ResourceManager.Load<GameObject>(heroSkinPath, true, true, null, 0, false);
	}

	private Units SpawnUnit_Internal(string tag, Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Vector3 position, Quaternion rotation, UnitControlType unitControlType = UnitControlType.None, bool reuse = false)
	{
		int num = unitData.GetDictValue(DataType.TeamType, -1);
		int dictValue = unitData.GetDictValue(DataType.UniqueId, this._idAlloc.Get());
		string dictValue2 = unitData.GetDictValue(DataType.NameId, null);
		string dictValue3 = unitData.GetDictValue(DataType.summerName, null);
		string empty = string.Empty;
		if (dictValue == 0 || dictValue2 == null || tag == null)
		{
			ClientLogger.Error("SpawnUnit : 参数错误! 不能生成单位!");
			return null;
		}
		if (num == -1)
		{
			ClientLogger.Error(string.Concat(new object[]
			{
				"SpawnUnit : 未指定阵营!! 默认为lm，",
				dictValue,
				" #",
				dictValue2
			}));
			num = 0;
		}
		int dictValue4 = unitData.GetDictValue(DataType.AIType, 0);
		float dictValue5 = unitData.GetDictValue(DataType.AttrFactor, 1f);
		int dictValue6 = unitData.GetDictValue(DataType.Level, 1);
		int dictValue7 = unitData.GetDictValue(DataType.Quality, 1);
		int dictValue8 = unitData.GetDictValue(DataType.Star, 1);
		float hp_init_p = (unitAttrs == null || !unitAttrs.ContainsKey(AttrType.Hp)) ? 1f : unitAttrs[AttrType.Hp];
		float mp_init_p = (unitAttrs == null || !unitAttrs.ContainsKey(AttrType.Mp)) ? 1f : unitAttrs[AttrType.Mp];
		int exp_cur = (!unitData.ContainsKey(DataType.Exp)) ? 0 : ((int)unitData[DataType.Exp]);
		if (unitControlType == UnitControlType.None)
		{
			if (Singleton<PvpManager>.Instance.IsInPvp)
			{
				unitControlType = Singleton<PvpManager>.Instance.GetControlType(dictValue);
			}
			else
			{
				unitControlType = UnitControlType.Free;
			}
		}
		SpawnPool spawnPool = null;
		if (this._poolRoot != null)
		{
			spawnPool = this._poolRoot.GetPool((TeamType)num);
		}
		if (spawnPool != null)
		{
			string text = (!unitData.ContainsKey(DataType.ModelId)) ? null : ((string)unitData[DataType.ModelId]);
			int skin = 0;
			if (unitData.ContainsKey(DataType.Skin))
			{
				skin = (int)unitData[DataType.Skin];
			}
			if (tag != null)
			{
				if (MapManager.<>f__switch$map38 == null)
				{
					MapManager.<>f__switch$map38 = new Dictionary<string, int>(7)
					{
						{
							"Monster",
							0
						},
						{
							"Home",
							1
						},
						{
							"Building",
							1
						},
						{
							"Hero",
							1
						},
						{
							"Player",
							1
						},
						{
							"Item",
							2
						},
						{
							"BuffItem",
							3
						}
					};
				}
				int num2;
				if (MapManager.<>f__switch$map38.TryGetValue(tag, out num2))
				{
					Transform transform;
					switch (num2)
					{
					case 0:
					{
						GameObject gameObject = ResourceManager.Load<GameObject>(text, true, true, null, skin, true);
						if (gameObject == null)
						{
							gameObject = ResourceManager.Load<GameObject>(text, true, true, null, 0, true);
							if (gameObject == null)
							{
								UnityEngine.Debug.LogError(string.Concat(new string[]
								{
									" ++++ Load Unit Error : npc_id = ",
									dictValue2,
									" modelId = ",
									text,
									" 没有对应的美术资源"
								}));
								return null;
							}
						}
						transform = spawnPool.Spawn(gameObject.transform, position, rotation);
						transform.name = this.GetName(gameObject.name, num);
						break;
					}
					case 1:
					{
						GameObject skinnedHero = this.GetSkinnedHero((TeamType)num, text);
						if (skinnedHero == null)
						{
							UnityEngine.Debug.LogError(string.Concat(new string[]
							{
								" ++++ Load Unit Error : npc_id = ",
								dictValue2,
								" modelId = ",
								text,
								" 没有对应的美术资源"
							}));
							return null;
						}
						transform = spawnPool.Spawn(skinnedHero.transform, position, rotation);
						transform.name = this.GetName(skinnedHero.name, num);
						break;
					}
					case 2:
					{
						GameObject gameObject2 = ResourceManager.Load<GameObject>(text, true, true, null, unitData.GetDictValue(DataType.Skin, -1), false);
						if (gameObject2 == null)
						{
							gameObject2 = ResourceManager.Load<GameObject>(text, true, true, null, 0, false);
							if (gameObject2 == null)
							{
								UnityEngine.Debug.LogError(string.Concat(new string[]
								{
									" ++++ Load SkillObject Error : npc_id = ",
									dictValue2,
									" modelId = ",
									text,
									" 没有对应的美术资源"
								}));
								return null;
							}
						}
						transform = spawnPool.Spawn(gameObject2.transform, position, rotation);
						transform.name = this.GetName(transform.name, num);
						break;
					}
					case 3:
					{
						GameObject gameObject3 = ResourceManager.Load<GameObject>(text, true, true, null, 0, false);
						if (gameObject3 == null)
						{
							UnityEngine.Debug.LogError(string.Concat(new string[]
							{
								" ++++ Load BuffObject Error : npc_id = ",
								dictValue2,
								" modelId = ",
								text,
								" 没有对应的美术资源"
							}));
							return null;
						}
						transform = spawnPool.Spawn(gameObject3.transform, position, rotation);
						transform.name = this.GetName(transform.name, num);
						break;
					}
					default:
						goto IL_4BE;
					}
					Units component = transform.gameObject.GetComponent<Units>();
					if (component == null)
					{
						ClientLogger.Error("unit 为空=" + transform.gameObject.name);
						return null;
					}
					component.npc_id = dictValue2;
					component.tag = tag;
					transform.tag = tag;
					component.unique_id = dictValue;
					component.summonerName = dictValue3;
					component.summonerId = empty;
					component.teamType = num;
					component.spwan_pos = position;
					component.spwan_rotation = rotation;
					component.attr_factor = dictValue5;
					component.aiType = dictValue4;
					component.star = dictValue8;
					component.quality = dictValue7;
					component.level = dictValue6;
					component.exp_cur = exp_cur;
					component.unitControlType = unitControlType;
					component.NeedRecycle = reuse;
					component.hp_init_p = hp_init_p;
					component.mp_init_p = mp_init_p;
					component.effect_id = unitData.GetDictValue(DataType.EffectId, "Default");
					return component;
				}
			}
			IL_4BE:
			ClientLogger.Error("SpawnUnit : tag指定错误，不是可生成的类型!!检查表");
			return null;
		}
		return null;
	}

	public Units SpawnEyeItemUnit(string tag, Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Transform newPoint, SVector3 eyeItemInfoInst, string eyeItemPreObjRes, UnitControlType unitControlType = UnitControlType.None, bool reuse = false)
	{
		Units units = this.SpawnUnit_Internal(tag, unitData, unitAttrs, newPoint.position, newPoint.rotation, unitControlType, reuse);
		if (units != null)
		{
			units.UnitInit(false);
			this.AddToMap(units);
			units.UnitStart();
			if (eyeItemInfoInst != null)
			{
				GameObject gameObject = units.gameObject;
				if (gameObject != null)
				{
					float inLifeTime = 2f;
					Vector3 inOriginalPos = new Vector3(eyeItemInfoInst.x, eyeItemInfoInst.y, eyeItemInfoInst.z);
					ActionManager.DelayDisplayEyeItem(eyeItemPreObjRes, inOriginalPos, newPoint.position, inLifeTime, gameObject);
				}
			}
		}
		return units;
	}

	public Units SpawnTengYunTuJiReplicateUnit(Units inSrcUnit)
	{
		if (inSrcUnit == null)
		{
			return null;
		}
		Transform transform = inSrcUnit.transform;
		Vector3 vector = (!(transform != null)) ? Vector3.zero : transform.position;
		Quaternion quaternion = (!(transform != null)) ? Quaternion.identity : transform.rotation;
		int teamType = inSrcUnit.teamType;
		SpawnPool spawnPool = null;
		if (this._poolRoot != null)
		{
			spawnPool = this._poolRoot.GetPool((TeamType)teamType);
		}
		if (spawnPool == null)
		{
			return null;
		}
		GameObject skinnedHero = this.GetSkinnedHero((TeamType)teamType, inSrcUnit.model_id);
		if (skinnedHero == null)
		{
			return null;
		}
		Transform transform2 = spawnPool.Spawn(skinnedHero.transform, vector, quaternion);
		transform2.name = this.GetName(skinnedHero.name, teamType);
		transform2.tag = "Hero";
		Units component = transform2.gameObject.GetComponent<Units>();
		component.npc_id = inSrcUnit.npc_id;
		component.tag = "Hero";
		component.teamType = teamType;
		component.spwan_pos = vector;
		component.spwan_rotation = quaternion;
		component.unitControlType = UnitControlType.None;
		component.NeedRecycle = false;
		AnimController animController = component.animController;
		if (animController != null)
		{
			animController.OnInit();
		}
		return component;
	}

	public void DespawnTengYunTuJiReplicateUnit(Units unit)
	{
		if (unit == null)
		{
			return;
		}
		if (this._poolRoot == null)
		{
			return;
		}
		SpawnPool pool = this._poolRoot.GetPool((TeamType)unit.teamType);
		if (pool != null)
		{
			pool.Despawn(unit.transform);
		}
	}

	public Units SpawnBuffItem(Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttr, Vector3 position, Quaternion rotation, UnitControlType unitControlType = UnitControlType.None)
	{
		string text = (!unitData.ContainsKey(DataType.NameId)) ? null : ((string)unitData[DataType.NameId]);
		if (text == null)
		{
			UnityEngine.Debug.LogError(" SpawnSkillUnit Error : npcId is NULL!!");
		}
		SkillUnitData vo = Singleton<SkillUnitDataMgr>.Instance.GetVo(text);
		unitData.Add(DataType.ModelId, vo.config.model_id);
		return this.SpawnUnit("BuffItem", unitData, unitAttr, position, rotation, unitControlType, false, null, UnitType.None);
	}

	private void PlaySound(string unitid, GameObject gameObject)
	{
		if (AudioMgr.Instance.isEffMute())
		{
			return;
		}
		unitid = "U_" + unitid;
		if (AudioMgr.Instance.isUsingWWise())
		{
			if (AudioGameDataLoader.instance._spellSfx.ContainsKey(unitid))
			{
				List<AudioGameDataLoader.audioBindstruct> list = AudioGameDataLoader.instance._spellSfx[unitid];
				if (list != null && list.Count > 0)
				{
					AudioMgr.Play(list[0].eventstr, gameObject, false, false);
				}
			}
			return;
		}
	}

	public Units SpawnSkillUnit(Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttr, Vector3 position, Quaternion rotation, UnitControlType unitControlType = UnitControlType.None, Units parent = null, string skillId = "")
	{
		string text = (!unitData.ContainsKey(DataType.NameId)) ? null : ((string)unitData[DataType.NameId]);
		int num = 0;
		if (parent != null)
		{
			num = HeroSkins.GetRealHeroSkin(parent.TeamType, parent.model_id);
		}
		if (text == null)
		{
			UnityEngine.Debug.LogError(" SpawnBuffItem Error : npcId is NULL!!");
		}
		SkillUnitData vo = Singleton<SkillUnitDataMgr>.Instance.GetVo(text);
		unitData.Add(DataType.ModelId, vo.config.model_id);
		unitData.Add(DataType.Skin, num);
		Units units = this.SpawnUnit_Internal("Item", unitData, unitAttr, position, rotation, unitControlType, false);
		if (!units)
		{
			ClientLogger.Error("SpawnSkillUnit: SpawnUnit_Internal failed for #" + text);
			return null;
		}
		units.UnitInit(false);
		if (parent != null)
		{
			units.unitControlType = parent.unitControlType;
			units.SetParentUnit(parent);
			string[] skillIDs = units.skillManager.GetSkillIDs();
			string[] array = skillIDs;
			for (int i = 0; i < array.Length; i++)
			{
				string skillID = array[i];
				units.skillManager.SetSkillLevel(skillID, parent.skillManager.GetSkillLevel(skillId));
			}
		}
		this.PlaySound(text, units.gameObject);
		units.UnitStart();
		this.AddToMap(units);
		return units;
	}

	public Units SpawnMonster(Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Vector3 position, Quaternion rotation)
	{
		return this.SpawnUnit("Monster", unitData, unitAttrs, position, rotation, UnitControlType.None, false, null, UnitType.None);
	}

	public Units SpawnTower(Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Vector3 position, Quaternion rotation)
	{
		return this.SpawnUnit("Building", unitData, unitAttrs, position, rotation, UnitControlType.None, false, null, UnitType.None);
	}

	public Units SpawnHome(Dictionary<DataType, object> unitData, Dictionary<AttrType, float> unitAttrs, Vector3 position, Quaternion rotation)
	{
		return this.SpawnUnit("Home", unitData, unitAttrs, position, rotation, UnitControlType.None, false, null, UnitType.None);
	}

	private bool RecycleUnit(Units unit)
	{
		if (unit == null)
		{
			ClientLogger.Error("RecycleUnit: target cannot be null");
			return false;
		}
		if (!unit.NeedRecycle)
		{
			return false;
		}
		if (this._recycledUnits.ContainsKey(unit.unique_id))
		{
			ClientLogger.Error("RecycleUnit: id already exists #" + unit.unique_id);
			return true;
		}
		this._recycledUnits[unit.unique_id] = unit;
		unit.UnitStop();
		PoolManagerUtils.SetActive(unit.gameObject, false);
		return true;
	}

	private bool RecycleUnit(Units unit, float seconds)
	{
		if (unit == null)
		{
			ClientLogger.Error("target cannot be null");
			return false;
		}
		if (!unit.NeedRecycle)
		{
			return false;
		}
		if (seconds == 0f)
		{
			this.RecycleUnit(unit);
		}
		else
		{
			this._coroutineManager.StartCoroutine(this.RecycleUnit_Coroutine(unit, seconds), true);
		}
		return true;
	}

	[DebuggerHidden]
	private IEnumerator RecycleUnit_Coroutine(Units unit, float delay)
	{
		MapManager.<RecycleUnit_Coroutine>c__Iterator1B4 <RecycleUnit_Coroutine>c__Iterator1B = new MapManager.<RecycleUnit_Coroutine>c__Iterator1B4();
		<RecycleUnit_Coroutine>c__Iterator1B.delay = delay;
		<RecycleUnit_Coroutine>c__Iterator1B.unit = unit;
		<RecycleUnit_Coroutine>c__Iterator1B.<$>delay = delay;
		<RecycleUnit_Coroutine>c__Iterator1B.<$>unit = unit;
		<RecycleUnit_Coroutine>c__Iterator1B.<>f__this = this;
		return <RecycleUnit_Coroutine>c__Iterator1B;
	}

	private Units ForceRespawnUnit(int uniqueId)
	{
		Units units = this.RespawnFromRecycledUnits(uniqueId);
		if (units == null)
		{
			units = this.GetUnit(uniqueId);
			if (units == null)
			{
				ClientLogger.Error("ForceRespawnUnit: Units respawn failed #" + uniqueId);
			}
		}
		return units;
	}

	private Units RespawnFromRecycledUnits(int uniqueId)
	{
		if (!this._recycledUnits.ContainsKey(uniqueId))
		{
			return null;
		}
		Units units = this._recycledUnits[uniqueId];
		this._recycledUnits.Remove(uniqueId);
		PoolManagerUtils.SetActive(units.gameObject, true);
		units.UnitCreate();
		return units;
	}

	public Units TryFetchRecycledUnit(int uniqueId)
	{
		if (!this._recycledUnits.ContainsKey(uniqueId))
		{
			return null;
		}
		return this._recycledUnits[uniqueId];
	}

	private void ForceDespawnUnit(Units unit)
	{
		if (!unit)
		{
			return;
		}
		unit.NeedRecycle = false;
		this.DespawnUnit(unit);
	}

	public void DespawnUnit(Units unit)
	{
		if (this.RecycleUnit(unit))
		{
			return;
		}
		if (this._poolRoot == null)
		{
			return;
		}
		SpawnPool pool = this._poolRoot.GetPool((TeamType)unit.teamType);
		if (pool != null)
		{
			pool.Despawn(unit.transform);
		}
		this.RemoveFromMapUnits(unit);
	}

	public void DespawnUnit(Units t, float seconds)
	{
		if (this.RecycleUnit(t, seconds))
		{
			return;
		}
		if (this._poolRoot == null)
		{
			return;
		}
		SpawnPool pool = this._poolRoot.GetPool((TeamType)(t.IsMonsterCreep() ? t.OriginalTeamType : t.SourceTeamType));
		if (pool != null)
		{
			pool.Despawn(t.transform, seconds);
		}
		this.RemoveFromMapUnits(t);
	}

	public void DespawnItem(Units t, float seconds)
	{
		if (this._poolRoot == null || t == null)
		{
			return;
		}
		SpawnPool pool = this._poolRoot.GetPool((TeamType)t.teamType);
		if (pool != null)
		{
			pool.Despawn(t.transform, seconds);
			if (seconds > 0f)
			{
				this._coroutineManager.StartCoroutine(this.RemoveItemFromMap(t, seconds), true);
			}
			else
			{
				this.RemoveFromMapUnits(t);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator RemoveItemFromMap(Units t, float seconds)
	{
		MapManager.<RemoveItemFromMap>c__Iterator1B5 <RemoveItemFromMap>c__Iterator1B = new MapManager.<RemoveItemFromMap>c__Iterator1B5();
		<RemoveItemFromMap>c__Iterator1B.seconds = seconds;
		<RemoveItemFromMap>c__Iterator1B.t = t;
		<RemoveItemFromMap>c__Iterator1B.<$>seconds = seconds;
		<RemoveItemFromMap>c__Iterator1B.<$>t = t;
		<RemoveItemFromMap>c__Iterator1B.<>f__this = this;
		return <RemoveItemFromMap>c__Iterator1B;
	}

	public bool IsHomeDestroyed(TeamType team)
	{
		Units units = this._homes[team];
		return units == null || !units.isLive;
	}

	public bool IsHeroAllDead(TeamType team)
	{
		int aliveTargetCount = this.GetAliveTargetCount(team, global::TargetTag.Hero);
		return aliveTargetCount <= 0;
	}

	public bool IsEnemyAllDead()
	{
		int aliveTargetsCount = this.GetAliveTargetsCount(TeamManager.GetEnemyTeams(TeamManager.MyTeam), global::TargetTag.All);
		return aliveTargetsCount <= 0;
	}

	public bool IsOurAllDead()
	{
		int aliveTargetCount = this.GetAliveTargetCount(TeamManager.MyTeam, global::TargetTag.All);
		return aliveTargetCount <= 0;
	}

	public bool IsEnemyHeroAllDead()
	{
		int aliveTargetsCount = this.GetAliveTargetsCount(TeamManager.GetEnemyTeams(TeamManager.MyTeam), global::TargetTag.Hero);
		return aliveTargetsCount <= 0;
	}

	public bool IsOurHeroAllDead()
	{
		int aliveTargetCount = this.GetAliveTargetCount(TeamManager.MyTeam, global::TargetTag.Hero);
		return aliveTargetCount <= 0;
	}

	public int GetAliveTargetCount(TeamType teamType, global::TargetTag targetTag)
	{
		return this.GetMapUnits(teamType, targetTag).Count((Units x) => x.isLive);
	}

	public int GetAliveTargetsCount(IList<TeamType> teams, global::TargetTag targetTag)
	{
		int num = 0;
		if (teams != null)
		{
			for (int i = 0; i < teams.Count; i++)
			{
				num += this.GetAliveTargetCount(teams[i], targetTag);
			}
		}
		return num;
	}

	public void AddToMap(Units target)
	{
		this.AddToMapUnits(target);
	}

	public void RemoveFromMap(Units target)
	{
		this.RemoveFromMapUnits(target);
	}

	public void ClearMap()
	{
		Units[] array = this._mapUnits.Values.ToArray<Units>();
		List<int> list = new List<int>(this._mapUnits.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			Units unit = this._mapUnits[list[i]];
			this.ForceDespawnUnit(unit);
		}
		this._mapUnits.Clear();
		this.ClearAllTagCount();
		this.ClearAllTagUnits();
		this.ClearAllHeroUnits();
		PlayerControlMgr.Instance.ResetPlayer();
		PlayerControlMgr.Instance.CleanSelectTag();
		if (this.OnRemoveUnit != null)
		{
			Units[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				Units obj = array2[j];
				this.OnRemoveUnit(obj);
			}
		}
	}

	public void SaveHerosEquips()
	{
		if (this._allHeroUnits == null)
		{
			return;
		}
		Dictionary<int, StatisticData> dictionary = new Dictionary<int, StatisticData>();
		for (int i = 0; i < this._allHeroUnits.Count; i++)
		{
			StatisticData value = default(StatisticData);
			value.summerName = this._allHeroUnits[i].summonerName;
			value.EquipItems = BattleEquipTools_op.GetHeroItemsString(this._allHeroUnits[i]);
			value.isPlayer = this._allHeroUnits[i].isPlayer;
			dictionary.Add(this._allHeroUnits[i].unique_id, value);
		}
		Singleton<StatisticView>.Instance.SaveHerosEquip(dictionary);
	}

	public void ClearHeros()
	{
		for (int i = 0; i < 4; i++)
		{
			IList<Units> mapUnits = this.GetMapUnits((TeamType)i, global::TargetTag.Hero);
			if (mapUnits != null)
			{
				Units[] array = mapUnits.ToArray<Units>();
				for (int j = 0; j < array.Length; j++)
				{
					this.DespawnUnit(array[j]);
				}
			}
		}
	}

	public void ClearMonsters()
	{
		for (int i = 0; i < 4; i++)
		{
			IList<Units> mapUnits = this.GetMapUnits((TeamType)i, global::TargetTag.Monster);
			if (mapUnits != null)
			{
				Units[] array = mapUnits.ToArray<Units>();
				for (int j = 0; j < array.Length; j++)
				{
					this.DespawnUnit(array[j]);
				}
			}
		}
	}

	public void ClearTowers()
	{
		for (int i = 0; i < 4; i++)
		{
			IList<Units> mapUnits = this.GetMapUnits((TeamType)i, global::TargetTag.Tower);
			if (mapUnits != null)
			{
				Units[] array = mapUnits.ToArray<Units>();
				for (int j = 0; j < array.Length; j++)
				{
					this.DespawnUnit(array[j]);
				}
			}
		}
	}

	private void AddToMapUnits(Units target)
	{
		if (target != null)
		{
			int unique_id = target.unique_id;
			int teamType = target.teamType;
			string text = target.tag;
			if (text.Equals("Player"))
			{
				text = "Hero";
			}
			this.AddToTagCount(teamType, text, unique_id);
			this.AddToTagUnits(teamType, target.tag, target);
			this.TryAddToHeroUnits(target);
			if (!this._mapUnits.ContainsKey(unique_id))
			{
				this._mapUnits.Add(unique_id, target);
			}
			else
			{
				this._mapUnits[unique_id] = target;
			}
			if (this.OnAddUnit != null)
			{
				this.OnAddUnit(target);
			}
		}
	}

	private void RemoveFromMapUnits(Units target)
	{
		if (target == null)
		{
			return;
		}
		Singleton<CreepSpawner>.Instance.RemoveCreep(target);
		int unique_id = target.unique_id;
		int teamType = target.teamType;
		string text = target.tag;
		if (text.Equals("Player"))
		{
			text = "Hero";
		}
		this.RemoveFromTagCount(teamType, text, unique_id);
		this.RemoveFromTagUnits(teamType, target.tag, target);
		if (this._mapUnits.ContainsKey(unique_id))
		{
			this._mapUnits.Remove(unique_id);
		}
		if (this.OnRemoveUnit != null)
		{
			this.OnRemoveUnit(target);
		}
	}

	public void OnMapUnitSwitchCamp(Units inUnit, int inOldType, int inNewType)
	{
		if (inUnit == null)
		{
			return;
		}
		string text = inUnit.tag;
		if (text.Equals("Player"))
		{
			text = "Hero";
		}
		this.RemoveFromTagCount(inOldType, text, inUnit.unique_id);
		this.AddToTagCount(inNewType, text, inUnit.unique_id);
		this.RemoveFromTagUnits(inOldType, inUnit.tag, inUnit);
		this.AddToTagUnits(inNewType, inUnit.tag, inUnit);
	}

	private void AddToTagCount(int teamType, string tag, int uniqueId)
	{
		Dictionary<string, List<int>> dictionary = this._teamGroupCountDict[(TeamType)teamType];
		if (!dictionary.ContainsKey(tag))
		{
			dictionary.Add(tag, new List<int>());
		}
		if (!dictionary[tag].Contains(uniqueId))
		{
			dictionary[tag].Add(uniqueId);
		}
	}

	private void RemoveFromTagCount(int teamType, string tag, int uniqueId)
	{
		Dictionary<string, List<int>> dictionary = this._teamGroupCountDict[(TeamType)teamType];
		if (dictionary.ContainsKey(tag))
		{
			dictionary[tag].Remove(uniqueId);
		}
	}

	private void ClearAllTagCount()
	{
		foreach (Dictionary<string, List<int>> current in this._teamGroupCountDict.Values)
		{
			current.Clear();
		}
	}

	private void AddToTagUnits(int teamType, string tag, Units target)
	{
		if (target == null)
		{
			return;
		}
		List<int> tagType = TagManager.GetTagType(tag);
		Dictionary<int, List<Units>> tagUnits = this.GetTagUnits((TeamType)teamType);
		for (int i = 0; i < tagType.Count; i++)
		{
			if (!tagUnits.ContainsKey(tagType[i]))
			{
				tagUnits.Add(tagType[i], new List<Units>());
			}
			if (!tagUnits[tagType[i]].Contains(target))
			{
				tagUnits[tagType[i]].Add(target);
			}
		}
	}

	private void RemoveFromTagUnits(int teamType, string tag, Units target)
	{
		if (target == null)
		{
			return;
		}
		List<int> tagType = TagManager.GetTagType(tag);
		Dictionary<int, List<Units>> tagUnits = this.GetTagUnits((TeamType)teamType);
		for (int i = 0; i < tagType.Count; i++)
		{
			if (tagUnits.ContainsKey(tagType[i]))
			{
				tagUnits[tagType[i]].Remove(target);
			}
		}
	}

	private void ClearAllTagUnits()
	{
		foreach (Dictionary<int, List<Units>> current in this._tagUnits.Values)
		{
			current.Clear();
		}
	}

	public IList<Units> GetAllHeroes()
	{
		return this._allHeroUnits;
	}

	public int GetFriendHeroesCount(int teamType)
	{
		int num = 0;
		foreach (Units current in this._allHeroUnits)
		{
			if (current.teamType == teamType)
			{
				num++;
			}
		}
		return num;
	}

	private void TryAddToHeroUnits(Units inTarget)
	{
		if (inTarget == null)
		{
			return;
		}
		if ((inTarget.tag == "Player" || inTarget.tag == "Hero") && !this._allHeroUnits.Contains(inTarget))
		{
			this._allHeroUnits.Add(inTarget);
		}
	}

	private void ClearAllHeroUnits()
	{
		this._allHeroUnits.Clear();
	}

	public bool IsPlayerInView(Vector3 inCenterPos, float inWarnRange, bool inIsSearchPart, int inStartIndex, out int outNextStartIndex)
	{
		if (inIsSearchPart)
		{
			int num = (this._allHeroUnits.Count <= 4) ? this._allHeroUnits.Count : 4;
			int num2 = (inStartIndex <= 0) ? 0 : inStartIndex;
			for (int i = 0; i < num; i++)
			{
				if (num2 >= this._allHeroUnits.Count)
				{
					num2 = 0;
				}
				if (UnitFeature.DistanceToPointSqr(inCenterPos, this._allHeroUnits[num2].transform.position) < inWarnRange * inWarnRange)
				{
					outNextStartIndex = num2 + 1;
					return true;
				}
				num2++;
			}
			outNextStartIndex = num2;
			return false;
		}
		outNextStartIndex = inStartIndex;
		for (int j = 0; j < this._allHeroUnits.Count; j++)
		{
			if (UnitFeature.DistanceToPointSqr(inCenterPos, this._allHeroUnits[j].transform.position) < inWarnRange * inWarnRange)
			{
				return true;
			}
		}
		return false;
	}

	private void RemoveFromMapObjects(Transform gameObj)
	{
		this._mapObjects.Remove(gameObj);
	}

	public void CreatePrefabPoolByTeam(TeamType teamType, Transform prefab, int count = 1, int cullAbove = 100, int cullDelay = 20, int cullMaxPerPass = 5)
	{
		SpawnPool spawnPool = (teamType == TeamType.None) ? this._poolRoot.GetPoolByPrefab(prefab.gameObject) : this._poolRoot.GetPool(teamType);
		PrefabPool prefabPool = spawnPool._perPrefabPoolOptions.Find((PrefabPool obj) => obj.prefab == prefab);
		if (prefabPool == null)
		{
			prefabPool = new PrefabPool(prefab)
			{
				preloadAmount = count,
				cullDespawned = true,
				cullAbove = cullAbove,
				cullDelay = cullDelay,
				cullMaxPerPass = cullMaxPerPass
			};
			spawnPool._perPrefabPoolOptions.Add(prefabPool);
		}
		else
		{
			prefabPool.preloadAmount += count;
		}
		spawnPool.CreatePrefabPool(prefabPool);
	}

	private string GetName(string prefabName, int teamtype)
	{
		prefabName = prefabName.Split(new char[]
		{
			'+'
		})[0];
		if (TeamManager.teams != null)
		{
			return prefabName + "+" + TeamManager.teams[teamtype].name;
		}
		return prefabName;
	}

	public void Dump()
	{
		this._resSpawner.Dump();
	}
}
