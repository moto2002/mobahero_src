using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using GUIFramework;
using HUD.Module;
using MobaHeros;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class MiniMapView : BaseView<MiniMapView>
	{
		private class SignalOnUnits
		{
			public int uid;

			public GameObject signalGo;

			public int senderId;
		}

		private class SignalOnPos
		{
			public Vector3 pos;

			public GameObject signalGo;

			public int senderId;
		}

		private class SearchMiniGroupItem
		{
			public Vector3 pos;

			public int BattleMonsterTeamId;
		}

		private class MapItemInfo
		{
			public Transform sample;

			public Transform mapItem;

			public bool isShowDeath;

			public bool needTarget = true;

			public Units targetUnits;
		}

		private const int SigMiss = 0;

		private const int SigAttack = 1;

		private const int SigGoto = 2;

		private const int SigDanger = 3;

		private const int SigDefense = 4;

		private const int SigMax = 5;

		private const float SIGNAL_DUR = 3f;

		private Transform Map;

		private Transform _dynamicMark;

		private Transform _staticMark;

		private Transform Map_S;

		private Transform ChangeMapBtn;

		private Transform BigMap;

		private Transform ChangeMapBtnBig;

		private UISprite MapText;

		private UISprite BigMapText;

		private BoxCollider BigMapCollider;

		private BoxCollider MapCollider;

		private Transform Kuang;

		private GameObject HomeMark1;

		private GameObject HomeMark2;

		private GameObject Hero1Mark;

		private GameObject Hero2Mark;

		private GameObject Monster1Mark;

		private GameObject Monster2Mark;

		private GameObject ChestFlyMark;

		private GameObject ChestMark;

		private GameObject ChestAlertMark;

		private GameObject MiniCreepMark1;

		private GameObject MiniCreepMark2;

		private GameObject BossCreepMark1;

		private GameObject BossCreepMark2;

		private GameObject BuffMark;

		private GameObject BuffMark1;

		private GameObject BuffMark2;

		private GameObject ShopMark;

		private GameObject PlayerMark;

		private GameObject TowerMark1;

		private GameObject TowerMark2;

		private GameObject BaseMark1;

		private GameObject BaseMark2;

		private GameObject EyeMark1;

		private GameObject EyeMark2;

		private GameObject DeathMark1;

		private GameObject DeathMark2;

		private GameObject RuneMark;

		private GameObject WarningMark;

		private TeamSignalPart _teamSignalMark;

		private GameObject HomeMark1Big;

		private GameObject HomeMark2Big;

		private GameObject Hero1MarkBig;

		private GameObject Hero2MarkBig;

		private GameObject Monster1MarkBig;

		private GameObject Monster2MarkBig;

		private GameObject MiniCreepMark1Big;

		private GameObject MiniCreepMark2Big;

		private GameObject BossCreepMark1Big;

		private GameObject BossCreepMark2Big;

		private GameObject BuffMarkBig;

		private GameObject BuffMark1Big;

		private GameObject BuffMark2Big;

		private GameObject ShopMarkBig;

		private GameObject PlayerMarkBig;

		private GameObject TowerMark1Big;

		private GameObject TowerMark2Big;

		private bool isCreate;

		private float minX;

		private float maxX;

		private float minY;

		private float maxY;

		private float offX;

		private float offY;

		private float fixY = 9999f;

		private int type;

		private int mapTag = 20000;

		private float mapAngle;

		private Dictionary<int, MiniMapGroup> MiniMapGroupDic = new Dictionary<int, MiniMapGroup>();

		private UIWidget MapTexture;

		private MinimapHeroMark heroMark;

		private UpdateController m_update;

		private CoroutineManager corManager;

		private readonly Dictionary<int, MiniMapView.MapItemInfo> _mapItemList = new Dictionary<int, MiniMapView.MapItemInfo>();

		private readonly Dictionary<GameObject, ObjectRecycler<Transform>> _mapItemPools = new Dictionary<GameObject, ObjectRecycler<Transform>>();

		private List<MiniMapView.SearchMiniGroupItem> searchMiniGroupList = new List<MiniMapView.SearchMiniGroupItem>();

		public float MapTextureWidth;

		public float MapTextureHeight;

		public float MapWidth = 33f;

		public float MapHeight = 25f;

		public float anchor_y;

		public float anchor_x;

		private readonly List<MiniMapView.SignalOnUnits> _unitSignals = new List<MiniMapView.SignalOnUnits>();

		private readonly List<MiniMapView.SignalOnPos> _posSignals = new List<MiniMapView.SignalOnPos>();

		private readonly List<MinimapTracer> _tracers = new List<MinimapTracer>();

		public bool isResetMap;

		private float update_time;

		private float fix_update_time;

		private bool canRay;

		private int onMapRayCount;

		private int selfTeamType = -1;

		private UIAtlas _miniMapPicAtlas;

		private CoroutineManager m_CoroutineManager;

		private float per_x;

		private float per_y;

		private float posion_x;

		private float posion_z;

		public MiniMapView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "MiniMapView");
		}

		public override void Init()
		{
			base.Init();
			this._dynamicMark = this.transform.Find("Anchor/Content/Dynamic/Mark");
			this._staticMark = this.transform.Find("Anchor/Content/Static/Mark");
			this.Map = this.transform.Find("Anchor/Content/Static/Map");
			this.ChangeMapBtn = this.transform.Find("Anchor/Content/Static/ChangeMapBtn");
			this.BigMap = this.transform.Find("Anchor/Content/Static/BigMap");
			this.ChangeMapBtnBig = this.transform.Find("Anchor/Content/Static/BigMap/ChangeMapBtn");
			this.Kuang = this._dynamicMark.Find("Kuang");
			this.HomeMark1 = (Resources.Load("Prefab/UI/MiniMap/Home1Mark") as GameObject);
			this.HomeMark2 = (Resources.Load("Prefab/UI/MiniMap/Home2Mark") as GameObject);
			this.Hero1Mark = base.LoadPrefabCache("Hero1Mark");
			this.Hero2Mark = base.LoadPrefabCache("Hero2Mark");
			this.Monster1Mark = base.LoadPrefabCache("Monster1Mark");
			this.Monster2Mark = base.LoadPrefabCache("Monster2Mark");
			this.BuffMark = base.LoadPrefabCache("BuffMark");
			this.BuffMark1 = (Resources.Load("Prefab/UI/MiniMap/Buff1Mark") as GameObject);
			this.BuffMark2 = (Resources.Load("Prefab/UI/MiniMap/Buff2Mark") as GameObject);
			this.PlayerMark = base.LoadPrefabCache("PlayerMark");
			this.TowerMark1 = (Resources.Load("Prefab/UI/MiniMap/Tower1Mark") as GameObject);
			this.TowerMark2 = (Resources.Load("Prefab/UI/MiniMap/Tower2Mark") as GameObject);
			this.WarningMark = base.LoadPrefabCache("WarningMark");
			this.MiniCreepMark1 = (Resources.Load("Prefab/UI/MiniMap/MiniCreep1Mark") as GameObject);
			this.MiniCreepMark2 = (Resources.Load("Prefab/UI/MiniMap/MiniCreep2Mark") as GameObject);
			this.BossCreepMark1 = (Resources.Load("Prefab/UI/MiniMap/BossCreep1Mark") as GameObject);
			this.BossCreepMark2 = (Resources.Load("Prefab/UI/MiniMap/BossCreep2Mark") as GameObject);
			this.BaseMark1 = (Resources.Load("Prefab/UI/MiniMap/Base1Mark") as GameObject);
			this.BaseMark2 = (Resources.Load("Prefab/UI/MiniMap/Base2Mark") as GameObject);
			this.EyeMark1 = (Resources.Load("Prefab/UI/MiniMap/Eye1Mark") as GameObject);
			this.EyeMark2 = (Resources.Load("Prefab/UI/MiniMap/Eye2Mark") as GameObject);
			this.DeathMark1 = (Resources.Load("Prefab/UI/MiniMap/Death1Mark") as GameObject);
			this.DeathMark2 = (Resources.Load("Prefab/UI/MiniMap/Death2Mark") as GameObject);
			this.RuneMark = (Resources.Load("Prefab/UI/MiniMap/RuneMark") as GameObject);
			this.ShopMark = (Resources.Load("Prefab/UI/MiniMap/ShopMark") as GameObject);
			this.HomeMark1Big = (Resources.Load("Prefab/UI/MiniMap/Home1MarkBig") as GameObject);
			this.HomeMark2Big = (Resources.Load("Prefab/UI/MiniMap/Home2MarkBig") as GameObject);
			this.Hero1MarkBig = (Resources.Load("Prefab/UI/MiniMap/Hero1MarkBig") as GameObject);
			this.Hero2MarkBig = (Resources.Load("Prefab/UI/MiniMap/Hero2MarkBig") as GameObject);
			this.Monster1MarkBig = (Resources.Load("Prefab/UI/MiniMap/Monster1MarkBig") as GameObject);
			this.Monster2MarkBig = (Resources.Load("Prefab/UI/MiniMap/Monster2MarkBig") as GameObject);
			this.BuffMarkBig = (Resources.Load("Prefab/UI/MiniMap/BuffMarkBig") as GameObject);
			this.BuffMark1Big = (Resources.Load("Prefab/UI/MiniMap/Buff1MarkBig") as GameObject);
			this.BuffMark2Big = (Resources.Load("Prefab/UI/MiniMap/Buff2MarkBig") as GameObject);
			this.PlayerMarkBig = (Resources.Load("Prefab/UI/MiniMap/PlayerMarkBig") as GameObject);
			this.TowerMark1Big = (Resources.Load("Prefab/UI/MiniMap/Tower1MarkBig") as GameObject);
			this.TowerMark2Big = (Resources.Load("Prefab/UI/MiniMap/Tower2MarkBig") as GameObject);
			this.MiniCreepMark1Big = (Resources.Load("Prefab/UI/MiniMap/MiniCreep1MarkBig") as GameObject);
			this.MiniCreepMark2Big = (Resources.Load("Prefab/UI/MiniMap/MiniCreep2MarkBig") as GameObject);
			this.BossCreepMark1Big = (Resources.Load("Prefab/UI/MiniMap/BossCreep1MarkBig") as GameObject);
			this.BossCreepMark2Big = (Resources.Load("Prefab/UI/MiniMap/BossCreep2MarkBig") as GameObject);
			this.ChestFlyMark = (Resources.Load("Prefab/UI/MiniMap/ChestFlyMark") as GameObject);
			this.ChestMark = (Resources.Load("Prefab/UI/MiniMap/ChestMark") as GameObject);
			this.ChestAlertMark = (Resources.Load("Prefab/UI/MiniMap/ChestAlertMark") as GameObject);
			this.ShopMarkBig = (Resources.Load("Prefab/UI/MiniMap/ShopMarkBig") as GameObject);
			this.MapText = this.Map.GetComponent<UISprite>();
			this.BigMapText = this.BigMap.GetComponent<UISprite>();
			this.BigMapCollider = this.BigMap.GetComponent<BoxCollider>();
			this.MapCollider = this.Map.GetComponent<BoxCollider>();
			GameObject gameObject = base.LoadPrefabCache("SignalMark");
			this._teamSignalMark = gameObject.GetComponent<TeamSignalPart>();
			this.mapTag = 20000;
			UIEventListener.Get(this.Map.gameObject).onPress = new UIEventListener.BoolDelegate(this.MapOnPress);
			UIEventListener.Get(this.BigMap.gameObject).onPress = new UIEventListener.BoolDelegate(this.MapOnPress);
			UIEventListener.Get(this.ChangeMapBtnBig.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnChangeMapBtn);
			this.MapTexture = this.Map.GetComponent<UIWidget>();
		}

		public override void HandleAfterOpenView()
		{
			this.InitSetMiniMapPic();
			this.SetPosByLevelID();
		}

		public override void HandleBeforeCloseView()
		{
			this.ClearAllItems();
			this.ClearMiniMapPicResources();
			this.searchMiniGroupList.Clear();
			foreach (KeyValuePair<int, MiniMapGroup> current in this.MiniMapGroupDic)
			{
				if (current.Value != null)
				{
					current.Value.DestroyGroup();
				}
			}
			this.MiniMapGroupDic.Clear();
			foreach (KeyValuePair<GameObject, ObjectRecycler<Transform>> current2 in this._mapItemPools)
			{
				current2.Value.DestroyPool();
			}
			if (this._posSignals != null)
			{
				for (int i = 0; i < this._posSignals.Count; i++)
				{
					if (this._posSignals[i].signalGo != null)
					{
						UnityEngine.Object.Destroy(this._posSignals[i].signalGo);
					}
				}
			}
			this._posSignals.Clear();
			this._mapItemPools.Clear();
		}

		private Transform CreateMapItem(Transform sample, bool @static)
		{
			Transform transform = (!@static) ? this._dynamicMark : this._staticMark;
			if (transform == null)
			{
				ClientLogger.Error(sample.name);
			}
			if (!this._mapItemPools.ContainsKey(sample.gameObject))
			{
				this._mapItemPools[sample.gameObject] = new ObjectRecycler<Transform>(delegate
				{
					UnityEngine.Object @object = UnityEngine.Object.Instantiate(sample);
					return (Transform)@object;
				}, null);
			}
			return this._mapItemPools[sample.gameObject].Create(transform);
		}

		private void ReleaseMapItem(Transform sample, Transform prefab)
		{
			if (!this._mapItemPools.ContainsKey(sample.gameObject))
			{
				UnityEngine.Debug.LogError("cannot found pool");
				return;
			}
			this._mapItemPools[sample.gameObject].Release(prefab);
		}

		private TeamSignalType GetSignalTypeFromIndex(int index)
		{
			switch (index)
			{
			case 0:
				return TeamSignalType.Miss;
			case 1:
				return TeamSignalType.Fire;
			case 2:
				return TeamSignalType.Goto;
			case 3:
				return TeamSignalType.Danger;
			case 4:
				return TeamSignalType.Defense;
			default:
				return TeamSignalType.Goto;
			}
		}

		public override void RegisterUpdateHandler()
		{
			this.m_CoroutineManager = new CoroutineManager();
			this.RefreshUI();
			MobaMessageManager.RegistMessage((ClientMsg)25060, new MobaMessageFunc(this.OnChangeMapSize));
			MinimapTracer.OnAdd += new Action<MinimapTracer>(this.MinimapTracer_OnAdd);
			MinimapTracer.OnRemove += new Action<MinimapTracer>(this.MinimapTracer_OnRemove);
			MapManager.Instance.OnAddUnit += new Action<Units>(this.MapManager_OnAddUnit);
			MapManager.Instance.OnRemoveUnit += new Action<Units>(this.MapManager_OnRemoveUnit);
		}

		private void MinimapTracer_OnRemove(MinimapTracer minimapTracer)
		{
			if (minimapTracer == null)
			{
				return;
			}
			if (this._tracers.Contains(minimapTracer))
			{
				UnityEngine.Object.Destroy(minimapTracer.texture);
				this._tracers.Remove(minimapTracer);
			}
		}

		private void MinimapTracer_OnAdd(MinimapTracer minimapTracer)
		{
			if (!this._tracers.Contains(minimapTracer))
			{
				minimapTracer.texture.transform.parent = this._dynamicMark;
				minimapTracer.texture.transform.rotation = Quaternion.identity;
				this.UpdateTracerPos(minimapTracer);
				this._tracers.Add(minimapTracer);
			}
		}

		private void MapManager_OnAddUnit(Units unit)
		{
		}

		private void MapManager_OnRemoveUnit(Units unit)
		{
			if (this._mapItemList.ContainsKey(unit.unique_id + this.mapTag) && this._mapItemList[unit.unique_id + this.mapTag] != null && this._mapItemList[unit.unique_id + this.mapTag].mapItem != null)
			{
				UnityEngine.Object.Destroy(this._mapItemList[unit.unique_id + this.mapTag].mapItem.gameObject);
				this._mapItemList.Remove(unit.unique_id + this.mapTag);
			}
		}

		private void UpdateTracerPos(MinimapTracer minimapTracer)
		{
			if (minimapTracer != null && minimapTracer.texture)
			{
				minimapTracer.texture.transform.localPosition = this.ChangePostion(minimapTracer.position);
			}
		}

		private void UpdateAllTracers()
		{
			try
			{
				List<MinimapTracer>.Enumerator enumerator = this._tracers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					this.UpdateTracerPos(enumerator.Current);
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		public override void CancelUpdateHandler()
		{
			this.m_CoroutineManager.StopAllCoroutine();
			this.ClearAllItems();
			MobaMessageManager.UnRegistMessage((ClientMsg)25060, new MobaMessageFunc(this.OnChangeMapSize));
			this.selfTeamType = -1;
			MinimapTracer.OnAdd -= new Action<MinimapTracer>(this.MinimapTracer_OnAdd);
			MinimapTracer.OnRemove -= new Action<MinimapTracer>(this.MinimapTracer_OnRemove);
			MapManager.Instance.OnAddUnit -= new Action<Units>(this.MapManager_OnAddUnit);
			MapManager.Instance.OnRemoveUnit -= new Action<Units>(this.MapManager_OnRemoveUnit);
		}

		public override void RefreshUI()
		{
			this.m_CoroutineManager.StartCoroutine(this.UpdateMiniMapView_Coroutine(), true);
		}

		public override void Destroy()
		{
			this.MapTextureWidth = 0f;
			this.MapTextureHeight = 0f;
			this.MapWidth = 25f;
			this.MapHeight = 25f;
			this.Hero1Mark = null;
			this.Hero2Mark = null;
			this.Monster1Mark = null;
			this.Monster2Mark = null;
			this.BuffMark = null;
			this.PlayerMark = null;
			this.WarningMark = null;
			this.ClearAllItems();
			this.Map_S = null;
			this.HomeMark1 = null;
			this.HomeMark2 = null;
			this.Hero1Mark = null;
			this.Hero2Mark = null;
			this.Monster1Mark = null;
			this.Monster2Mark = null;
			this.BuffMark = null;
			this.PlayerMark = null;
			this.TowerMark1 = null;
			this.TowerMark2 = null;
			this.WarningMark = null;
			this.MiniCreepMark1 = null;
			this.MiniCreepMark2 = null;
			this.BossCreepMark1 = null;
			this.BossCreepMark2 = null;
			this.BaseMark1 = null;
			this.BaseMark2 = null;
			this.selfTeamType = -1;
			base.Destroy();
		}

		private void SetPosByLevelID()
		{
			string level_id = LevelManager.m_CurLevel.level_id;
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(level_id);
			if (dataById != null && (dataById.scene_map_id.Equals("Map21") || dataById.scene_map_id.Equals("Map22")))
			{
				TweenPosition component = this.transform.Find("Anchor/Content").GetComponent<TweenPosition>();
				component.transform.localPosition = new Vector3(0f, 90f, 0f);
				component.from = new Vector3(0f, 90f, 0f);
				component.to = new Vector3(1000f, 90f, 0f);
			}
		}

		private void ReAddShop()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("SceneUnit");
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].layer == Layer.ShopLayer && array[i].GetComponent<BattleEquipmentShopModel>() != null)
					{
						this.AddShopItem(array[i].transform, 90000 + i);
					}
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator UpdateMiniMapView_Coroutine()
		{
			MiniMapView.<UpdateMiniMapView_Coroutine>c__IteratorE1 <UpdateMiniMapView_Coroutine>c__IteratorE = new MiniMapView.<UpdateMiniMapView_Coroutine>c__IteratorE1();
			<UpdateMiniMapView_Coroutine>c__IteratorE.<>f__this = this;
			return <UpdateMiniMapView_Coroutine>c__IteratorE;
		}

		private void UpdateSignals()
		{
			this._unitSignals.RemoveAll((MiniMapView.SignalOnUnits x) => x.signalGo == null);
			List<MiniMapView.SignalOnUnits>.Enumerator enumerator = this._unitSignals.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.PlaceUnitsSignal(enumerator.Current);
			}
		}

		private void UpdateMiniMapView()
		{
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			Dictionary<int, Units>.Enumerator enumerator = allMapUnits.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Units> current = enumerator.Current;
				Units value = current.Value;
				if (!(value == null))
				{
					if (value.gameObject.activeInHierarchy)
					{
						this.UpdateMapItem(value);
						this.SearchHideMiniGroup();
					}
				}
			}
		}

		private void SearchHideMiniGroup()
		{
			for (int i = 0; i < this.searchMiniGroupList.Count; i++)
			{
				if (FOWSystem.instance.IsVisible(this.searchMiniGroupList[i].pos) && this.MiniMapGroupDic.ContainsKey(this.searchMiniGroupList[i].BattleMonsterTeamId))
				{
					this.MiniMapGroupDic[this.searchMiniGroupList[i].BattleMonsterTeamId].ShowSp(false);
				}
			}
		}

		private void AddShopItem(Transform tra, int id)
		{
			GameObject gameObject;
			if (this.CheckTagIsBig())
			{
				gameObject = this.ShopMarkBig;
				id += 100;
			}
			else
			{
				gameObject = this.ShopMark;
			}
			if (gameObject != null)
			{
				if (this._mapItemList.ContainsKey(id))
				{
					Transform mapItem = this._mapItemList[id].mapItem;
					this.SetIconVisible(mapItem.gameObject, true);
					mapItem.localPosition = this.ChangePostion(tra.position.x, tra.position.z);
				}
				else
				{
					Transform transform = this.CreateMapItem(gameObject.transform, true);
					transform.name = "Shop";
					this._mapItemList.Add(id, new MiniMapView.MapItemInfo
					{
						mapItem = transform,
						sample = gameObject.transform
					});
					transform.localPosition = this.ChangePostion(tra.position.x, tra.position.z);
				}
			}
		}

		public GameObject AddMiniMapIcon(Units target)
		{
			return null;
		}

		public void AddMiniMapBossFreshTune(int groupId, string time)
		{
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			this.corManager.StartCoroutine(this.DelayAddMiniMapBossFreshTune(groupId, time), true);
		}

		[DebuggerHidden]
		private IEnumerator DelayAddMiniMapBossFreshTune(int groupId, string time)
		{
			MiniMapView.<DelayAddMiniMapBossFreshTune>c__IteratorE2 <DelayAddMiniMapBossFreshTune>c__IteratorE = new MiniMapView.<DelayAddMiniMapBossFreshTune>c__IteratorE2();
			<DelayAddMiniMapBossFreshTune>c__IteratorE.groupId = groupId;
			<DelayAddMiniMapBossFreshTune>c__IteratorE.time = time;
			<DelayAddMiniMapBossFreshTune>c__IteratorE.<$>groupId = groupId;
			<DelayAddMiniMapBossFreshTune>c__IteratorE.<$>time = time;
			<DelayAddMiniMapBossFreshTune>c__IteratorE.<>f__this = this;
			return <DelayAddMiniMapBossFreshTune>c__IteratorE;
		}

		public void RemoveMiniMapIcon(GameObject mark)
		{
		}

		private void UpdateMapItem(Units target)
		{
			if (target == null)
			{
				return;
			}
			Transform transform = null;
			if (target.isLive && target.gameObject.activeInHierarchy)
			{
				if (!this._mapItemList.ContainsKey(target.unique_id + this.mapTag))
				{
					if (target.IsMonsterCreep() && this.MiniMapGroupDic.ContainsKey(target.GetBattleMonsterTeamId()))
					{
						if (this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetTeamType() != target.teamType)
						{
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].SetTeamType(target.teamType);
							string spriteName = this.GetMapItemPrefab(target).GetComponent<UISprite>().spriteName;
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].SetUISprite(spriteName);
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].RestorePos();
						}
						if ((this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetTeamType() == this.selfTeamType || BaseDataMgr.instance.GetMonsterMainData(target.npc_id).item_type == 9) && this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetItem() != null)
						{
							if (this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetItem().parent != this._dynamicMark)
							{
								this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetItem().parent = this._dynamicMark;
								this._dynamicMark.parent.gameObject.SetActive(false);
								this._dynamicMark.parent.gameObject.SetActive(true);
							}
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].GetItem().localPosition = this.ChangePostion(target.transform.position.x, target.transform.position.z);
						}
						this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].UpdateMember(target);
						this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].ShowSp(true);
						this.searchMiniGroupList.Remove(this.searchMiniGroupList.Find((MiniMapView.SearchMiniGroupItem obj) => obj.BattleMonsterTeamId == target.GetBattleMonsterTeamId()));
						return;
					}
					GameObject mapItemPrefab = this.GetMapItemPrefab(target);
					if (mapItemPrefab != null)
					{
						Transform transform2 = this.CreateMapItem(mapItemPrefab.transform, MiniMapView.IsStatic(target));
						if (target.tag == "Hero" || target.tag == "Player")
						{
							SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(target.npc_id);
							transform2.GetComponent<UISprite>().spriteName = heroMainData.avatar_icon;
						}
						this.isCreate = true;
						if (!target.IsMonsterCreep())
						{
							this._mapItemList.Add(target.unique_id + this.mapTag, new MiniMapView.MapItemInfo
							{
								mapItem = transform2,
								sample = mapItemPrefab.transform,
								targetUnits = target
							});
							if (target.isBuffItem && !target.npc_id.Contains("jia"))
							{
								if (this.corManager == null)
								{
									this.corManager = new CoroutineManager();
								}
								this.corManager.StartCoroutine(this.DelayHideChild(transform2, 2f), true);
							}
						}
						else
						{
							this.MiniMapGroupDic.Add(target.GetBattleMonsterTeamId(), new MiniMapGroup(target.GetBattleMonsterTeamId(), transform2));
							transform2.name = "Group" + target.GetBattleMonsterTeamId();
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].SetTeamType(target.teamType);
							this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].UpdateMember(target);
							SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(target.npc_id);
							int replace_id = BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(target.GetBattleMonsterCreepId()).replace_id;
							if (replace_id != 0)
							{
								this.DestoryGloupByReplaceId(replace_id);
							}
							if (monsterMainData.item_type == 7 || monsterMainData.item_type == 9)
							{
								if (this.corManager == null)
								{
									this.corManager = new CoroutineManager();
								}
								this.corManager.StartCoroutine(this.DelayHideChild(transform2, 2f), true);
							}
						}
						transform = transform2;
					}
				}
				else
				{
					transform = this._mapItemList[target.unique_id + this.mapTag].mapItem;
					if (this._mapItemList[target.unique_id + this.mapTag].isShowDeath)
					{
						this.ShowDeathIcon(transform, false);
						this._mapItemList[target.unique_id + this.mapTag].isShowDeath = false;
					}
					this.isCreate = false;
				}
				if (transform != null && !target.IsMonsterCreep())
				{
					this.SetIconVisible(transform.gameObject, this.CheckUnitIsShow(target));
				}
				if (transform != null && transform.gameObject.activeInHierarchy)
				{
					if (this.isCreate && target.IsMonsterCreep())
					{
						this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].SaveBirthPosition(target.transform.position);
					}
					if (target.IsMonsterCreep() && BaseDataMgr.instance.GetMonsterMainData(target.npc_id).item_type == 9)
					{
						transform.localPosition = this.ChangePostion(target.transform.position.x, target.transform.position.z);
						return;
					}
					if (!this.isCreate && target.IsMonsterCreep() && target.teamType != this.selfTeamType)
					{
						return;
					}
					transform.localPosition = this.ChangePostion(target.transform.position.x, target.transform.position.z);
				}
			}
			else
			{
				if (target.IsMonsterCreep())
				{
					SysMonsterMainVo monsterMainData2 = BaseDataMgr.instance.GetMonsterMainData(target.npc_id);
					if (!FOWSystem.instance.IsVisible(target.transform.position) && monsterMainData2.item_type != 7 && monsterMainData2.item_type != 9)
					{
						int battleMonsterTeamId = target.GetBattleMonsterTeamId();
						if (this.MiniMapGroupDic.ContainsKey(battleMonsterTeamId) && this.MiniMapGroupDic[battleMonsterTeamId].CheckMemberDeath(target))
						{
							this.searchMiniGroupList.Add(new MiniMapView.SearchMiniGroupItem
							{
								BattleMonsterTeamId = target.GetBattleMonsterTeamId(),
								pos = target.transform.position
							});
						}
						return;
					}
					if (this.MiniMapGroupDic.ContainsKey(target.GetBattleMonsterTeamId()) && this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].CheckMemberDeath(target))
					{
						this.MiniMapGroupDic[target.GetBattleMonsterTeamId()].StartTimer(BaseDataMgr.instance.GetDataById<SysBattleMonsterCreepVo>(target.GetBattleMonsterCreepId()).refresh_time - 3);
					}
				}
				else if (target.isHero)
				{
					if (!this._mapItemList.ContainsKey(target.unique_id + this.mapTag))
					{
						return;
					}
					if (!target.MirrorState)
					{
						transform = this._mapItemList[target.unique_id + this.mapTag].mapItem;
						this._mapItemList[target.unique_id + this.mapTag].isShowDeath = true;
						this.ShowDeathIcon(transform, true);
						return;
					}
				}
				if (this._mapItemList.ContainsKey(target.unique_id + this.mapTag))
				{
					MiniMapView.MapItemInfo mapItemInfo = this._mapItemList[target.unique_id + this.mapTag];
					this.ReleaseMapItem(mapItemInfo.sample, mapItemInfo.mapItem);
					this._mapItemList.Remove(target.unique_id + this.mapTag);
				}
			}
		}

		private void DestoryGloupByReplaceId(int replaceId)
		{
			if (this.MiniMapGroupDic.ContainsKey(replaceId))
			{
				this.MiniMapGroupDic[replaceId].DestroyGroup();
				this.MiniMapGroupDic.Remove(replaceId);
			}
		}

		private void ShowDeathIcon(Transform item, bool isDeath)
		{
			this.heroMark = item.gameObject.GetComponent<MinimapHeroMark>();
			if (this.heroMark == null)
			{
				return;
			}
			if (isDeath)
			{
				if (!this.heroMark.IsDeathState())
				{
					this.heroMark.ShowDeath();
				}
			}
			else if (this.heroMark.IsDeathState())
			{
				this.heroMark.ShowNomal();
			}
		}

		public void ShowTowerMapWarn(Units tower)
		{
			MiniMapView.MapItemInfo mapItemInfo = null;
			if (!this._mapItemList.TryGetValue(tower.unique_id + this.mapTag, out mapItemInfo))
			{
				return;
			}
			if (mapItemInfo.mapItem == null)
			{
				return;
			}
			Transform mapItem = mapItemInfo.mapItem;
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			this.corManager.StartCoroutine(this.DelayHideChild(mapItem, 2f), true);
		}

		[DebuggerHidden]
		private IEnumerator DelayHideChild(Transform tra, float time)
		{
			MiniMapView.<DelayHideChild>c__IteratorE3 <DelayHideChild>c__IteratorE = new MiniMapView.<DelayHideChild>c__IteratorE3();
			<DelayHideChild>c__IteratorE.tra = tra;
			<DelayHideChild>c__IteratorE.time = time;
			<DelayHideChild>c__IteratorE.<$>tra = tra;
			<DelayHideChild>c__IteratorE.<$>time = time;
			return <DelayHideChild>c__IteratorE;
		}

		private void SetIconVisible(GameObject icon, bool visible)
		{
			if (!icon)
			{
				return;
			}
			PerfTools.SetVisible(icon.gameObject, visible);
		}

		private static bool IsStatic(Units target)
		{
			return target.tag == "Building" || target.tag == "Home" || target.IsMonsterCreep();
		}

		public void ForceUpdateMapItemByUnits(Units target, bool needShow = true)
		{
			if (!this._mapItemList.ContainsKey(target.unique_id + this.mapTag))
			{
				return;
			}
			MiniMapView.MapItemInfo mapItemInfo = this._mapItemList[target.unique_id + this.mapTag];
			this.ReleaseMapItem(mapItemInfo.sample, mapItemInfo.mapItem);
			this._mapItemList.Remove(target.unique_id + this.mapTag);
			if (!target.isLive || !target.gameObject.activeInHierarchy || !needShow)
			{
				return;
			}
			GameObject mapItemPrefab = this.GetMapItemPrefab(target);
			if (mapItemPrefab != null)
			{
				Transform transform = this.CreateMapItem(mapItemPrefab.transform, MiniMapView.IsStatic(target));
				if (target.tag == "Hero" || target.tag == "Player")
				{
					SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(target.npc_id);
					transform.GetComponent<UISprite>().spriteName = heroMainData.avatar_icon;
				}
				this._mapItemList.Add(target.unique_id + this.mapTag, new MiniMapView.MapItemInfo
				{
					mapItem = transform,
					sample = mapItemPrefab.transform,
					targetUnits = target
				});
			}
		}

		private bool CheckUnitIsShow(Units unit)
		{
			int nVisibleState = unit.m_nVisibleState;
			if (unit.IsMonsterCreep())
			{
				return unit.isLive;
			}
			if (unit.tag == "Home" || unit.tag == "Building")
			{
				return unit.isLive;
			}
			return nVisibleState < 2 && nVisibleState >= 0;
		}

		private void ClearAllItems()
		{
			Dictionary<int, MiniMapView.MapItemInfo>.Enumerator enumerator = this._mapItemList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, MiniMapView.MapItemInfo> current = enumerator.Current;
				MiniMapView.MapItemInfo value = current.Value;
				if (value != null && value.mapItem)
				{
					UnityEngine.Object.Destroy(value.mapItem.gameObject);
					this.ReleaseMapItem(value.sample, value.mapItem);
				}
			}
			this._mapItemList.Clear();
		}

		private void HideAllItems()
		{
			Dictionary<int, MiniMapView.MapItemInfo>.Enumerator enumerator = this._mapItemList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, MiniMapView.MapItemInfo> current = enumerator.Current;
				MiniMapView.MapItemInfo value = current.Value;
				if (value != null && value.mapItem)
				{
					this.SetIconVisible(value.mapItem.gameObject, false);
				}
			}
		}

		public GameObject GetMapItemPrefab(Units unit)
		{
			if ("TreasureChestTest" == unit.npc_id)
			{
				return this.ChestMark;
			}
			if (unit.isMonster)
			{
				if (unit.npc_id == "Boxxinshi")
				{
					return this.ChestFlyMark;
				}
				SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(unit.npc_id);
				if (monsterMainData != null)
				{
					if (TagManager.CheckTag(unit, TargetTag.EyeUnit))
					{
						if (unit.teamType == this.selfTeamType)
						{
							if (this.CheckTagIsBig())
							{
								return this.EyeMark1;
							}
							return this.EyeMark1;
						}
						else
						{
							if (this.CheckTagIsBig())
							{
								return this.EyeMark2;
							}
							return this.EyeMark2;
						}
					}
					else if (unit.IsMonsterCreep())
					{
						if (monsterMainData.item_type == 7 || monsterMainData.item_type == 9)
						{
							if (unit.teamType != this.selfTeamType)
							{
								if (this.CheckTagIsBig())
								{
									return this.BossCreepMark1Big;
								}
								return this.BossCreepMark1;
							}
							else
							{
								if (this.CheckTagIsBig())
								{
									return this.BossCreepMark2Big;
								}
								return this.BossCreepMark2;
							}
						}
						else if (unit.teamType != this.selfTeamType)
						{
							if (this.CheckTagIsBig())
							{
								return this.MiniCreepMark1Big;
							}
							return this.MiniCreepMark1;
						}
						else
						{
							if (this.CheckTagIsBig())
							{
								return this.MiniCreepMark2Big;
							}
							return this.MiniCreepMark2;
						}
					}
				}
				if (unit.teamType == this.selfTeamType)
				{
					if (monsterMainData.item_type == 7 || monsterMainData.item_type == 9)
					{
						if (this.CheckTagIsBig())
						{
							return this.BossCreepMark2Big;
						}
						return this.BossCreepMark2;
					}
					else if (unit.IsMonsterCreep())
					{
						if (this.CheckTagIsBig())
						{
							return this.MiniCreepMark2Big;
						}
						return this.MiniCreepMark2;
					}
					else
					{
						if (this.CheckTagIsBig())
						{
							return this.Monster1MarkBig;
						}
						return this.Monster1Mark;
					}
				}
				else if (monsterMainData.item_type == 7 || monsterMainData.item_type == 9)
				{
					if (this.CheckTagIsBig())
					{
						return this.BossCreepMark1Big;
					}
					return this.BossCreepMark1;
				}
				else if (unit.IsMonsterCreep())
				{
					if (this.CheckTagIsBig())
					{
						return this.MiniCreepMark1Big;
					}
					return this.MiniCreepMark1;
				}
				else
				{
					if (this.CheckTagIsBig())
					{
						return this.Monster2MarkBig;
					}
					return this.Monster2Mark;
				}
			}
			else if (unit.isHero)
			{
				if (unit.teamType == this.selfTeamType)
				{
					if (unit.isPlayer)
					{
						if (this.CheckTagIsBig())
						{
							return this.PlayerMarkBig;
						}
						return this.PlayerMark;
					}
					else
					{
						if (this.CheckTagIsBig())
						{
							return this.Hero1MarkBig;
						}
						return this.Hero1Mark;
					}
				}
				else
				{
					if (this.CheckTagIsBig())
					{
						return this.Hero2MarkBig;
					}
					return this.Hero2Mark;
				}
			}
			else if (unit.isHome)
			{
				if (unit.teamType == this.selfTeamType)
				{
					if (this.CheckTagIsBig())
					{
						return this.HomeMark1Big;
					}
					return this.HomeMark1;
				}
				else
				{
					if (this.CheckTagIsBig())
					{
						return this.HomeMark2Big;
					}
					return this.HomeMark2;
				}
			}
			else if (unit.isTower)
			{
				if (unit.name.Contains("Home"))
				{
					return null;
				}
				if (unit.teamType == this.selfTeamType)
				{
					if (this.CheckTagIsBig())
					{
						return this.TowerMark1Big;
					}
					return this.TowerMark1;
				}
				else
				{
					if (this.CheckTagIsBig())
					{
						return this.TowerMark2Big;
					}
					return this.TowerMark2;
				}
			}
			else
			{
				if (!unit.isBuffItem)
				{
					return null;
				}
				if (unit.teamType == this.selfTeamType)
				{
					if (unit.npc_id.Contains("jia"))
					{
						return null;
					}
					if (this.CheckTagIsBig())
					{
						return this.BuffMarkBig;
					}
					return this.BuffMark;
				}
				else
				{
					if (unit.npc_id.Contains("jia"))
					{
						return null;
					}
					if (this.CheckTagIsBig())
					{
						return this.BuffMarkBig;
					}
					return this.BuffMark;
				}
			}
		}

		private bool CheckTagIsBig()
		{
			return this.mapTag != 20000 && this.mapTag == 40000;
		}

		public Vector3 ChangePostion(float real_x, float real_y)
		{
			if (this.MapWidth == 0f)
			{
				this.MapWidth = 25f;
			}
			if (this.MapHeight == 0f)
			{
				this.MapHeight = 25f;
			}
			float x = (real_x / this.MapWidth - 0.5f) * this.MapTextureWidth + this.offX;
			float y = (real_y / this.MapHeight - 0.5f) * this.MapTextureHeight + this.offY;
			return new Vector3(x, y);
		}

		private Vector3 ChangePostion(Vector3 pos)
		{
			return this.ChangePostion(pos.x, pos.z);
		}

		private void MapOnPress(GameObject go, bool state)
		{
			if (state)
			{
				if (!TeamSignalManager.IsBegin)
				{
					this.canRay = true;
					this.TryToRay();
				}
			}
			else
			{
				if (TeamSignalManager.IsBegin)
				{
					this.canRay = true;
					this.TryToRay();
				}
				this.canRay = false;
				if (Singleton<TriggerManager>.Instance != null)
				{
					Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapUp);
				}
				if (this.Kuang.gameObject.activeSelf)
				{
					this.Kuang.gameObject.SetActive(false);
				}
			}
		}

		private void OnChangeMapSize(MobaMessage msg)
		{
			this.OnChangeMapBtn(null);
		}

		public void OnChangeMapBtn(GameObject obj)
		{
			this.HideAllItems();
			this.HideAllUnitSignals();
			if (this.Map.gameObject.activeInHierarchy)
			{
				this.Map.gameObject.SetActive(false);
				this.BigMap.gameObject.SetActive(true);
				this.MapTexture = this.BigMap.GetComponent<UIWidget>();
				this.MapTextureWidth = (float)this.MapTexture.width;
				this.MapTextureHeight = (float)this.MapTexture.height;
				this.SetBigMapValue();
				this.BigMapCollider.center = new Vector3((float)(-(float)this.BigMapText.width / 2), (float)(-(float)this.BigMapText.height / 2), 0f);
				this.BigMapCollider.size = new Vector3((float)this.BigMapText.width, (float)this.BigMapText.height, 0f);
			}
			else
			{
				this.Map.gameObject.SetActive(true);
				this.BigMap.gameObject.SetActive(false);
				this.MapTexture = this.Map.GetComponent<UIWidget>();
				this.MapTextureWidth = (float)this.MapTexture.width;
				this.MapTextureHeight = (float)this.MapTexture.height;
				this.SetSmallMapValue();
			}
			Transform arg_16D_0 = this._staticMark;
			Vector3 localPosition = new Vector3(this.anchor_x, this.anchor_y, 0f);
			this._dynamicMark.localPosition = localPosition;
			arg_16D_0.localPosition = localPosition;
			this.MapCollider.center = new Vector3((float)(-(float)this.MapText.width / 2), (float)(-(float)this.MapText.height / 2), 0f);
			this.MapCollider.size = new Vector3((float)this.MapText.width, (float)this.MapText.height, 0f);
			UIAtlas atlas = this.Monster1MarkBig.GetComponent<UISprite>().atlas;
			UIAtlas atlas2 = this.TowerMark1.GetComponent<UISprite>().atlas;
			foreach (KeyValuePair<int, MiniMapGroup> current in this.MiniMapGroupDic)
			{
				if (this.mapTag == 40000)
				{
					current.Value.GetUISprite().atlas = atlas;
				}
				else
				{
					current.Value.GetUISprite().atlas = atlas2;
				}
				current.Value.GetUISprite().MakePixelPerfect();
				if (current.Value.GetItem() != null)
				{
					current.Value.GetItem().localPosition = this.ChangePostion(current.Value.GetBirthPosition().x, current.Value.GetBirthPosition().z);
				}
			}
		}

		private void TryToRay()
		{
			if (!this.canRay)
			{
				return;
			}
			if (this.Map_S == null && MapRuler.map != null)
			{
				this.Map_S = MapRuler.map.transform.Find("Map_s");
				if (this.Map_S == null)
				{
					ClientLogger.Error("===> MapRuler.map.name:" + MapRuler.map.name + "找不到Map_s");
					return;
				}
			}
			if (this.fixY == 9999f)
			{
				this.fixY = (this.transform.Find("Anchor").localPosition.y - 541.6f) * 0.9029999f / 180.236938f;
			}
			this.onMapRayCount = 0;
			Vector3 zero = Vector3.zero;
			if (Input.touchCount > 0)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < Input.touchCount; i++)
				{
					zero = new Vector3(touches[i].position.x, touches[i].position.y, 0f);
					this.DoRay(zero);
				}
				if (this.onMapRayCount == 0)
				{
					this.canRay = false;
					Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapUp);
					if (this.Kuang.gameObject.activeSelf)
					{
						this.Kuang.gameObject.SetActive(false);
					}
				}
			}
		}

		private void DoRay(Vector3 tapVector)
		{
			Ray ray = CameraRoot.Instance.UICamera.ScreenPointToRay(tapVector);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))
			{
				this.onMapRayCount++;
				GameObject gameObject = raycastHit.collider.gameObject;
				if ((gameObject.name == "Map" || gameObject.name == "BigMap") && this.Map_S != null)
				{
					this.per_x = (raycastHit.point.x - this.maxX) / (this.minX - this.maxX);
					this.per_y = (raycastHit.point.y - this.maxY - this.fixY) / (this.minY - this.maxY);
					this.posion_x = -(this.Map_S.transform.localScale.x / 2f) * (this.per_x - 0.5f);
					this.posion_z = -(this.Map_S.transform.localScale.z / 2f) * (this.per_y - 0.5f);
					if (TeamSignalManager.IsBegin)
					{
						TeamSignalManager.TrySendTeamPosNotify(TeamSignalManager.CurMode, new Vector3(this.posion_x, 0f, this.posion_z));
						this.canRay = false;
						TeamSignalManager.End();
					}
					else
					{
						Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapDown);
						BattleCameraMgr.Instance.SetPostion(new Vector3(this.posion_x, 0f, this.posion_z));
						this.ShowTipSign(tapVector, gameObject.name);
					}
				}
				else if (Input.touchCount < 2)
				{
					this.canRay = false;
					Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapUp);
					if (this.Kuang.gameObject.activeSelf)
					{
						this.Kuang.gameObject.SetActive(false);
					}
				}
			}
			else if (Input.touchCount < 2)
			{
				this.canRay = false;
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapUp);
				if (this.Kuang.gameObject.activeSelf)
				{
					this.Kuang.gameObject.SetActive(false);
				}
			}
		}

		public void CancelTapMiniMap()
		{
			if (BattleCameraMgr.Instance._currenCameraControllerType == CameraControllerType.MoveByTap)
			{
				Singleton<TriggerManager>.Instance.SendGameStateEvent(GameEvent.TapMiniMapUp);
				if (this.Kuang.gameObject.activeSelf)
				{
					this.Kuang.gameObject.SetActive(false);
				}
			}
		}

		private void ShowTipSign(Vector3 tapVector, string p)
		{
			if (!this.Kuang.gameObject.activeSelf)
			{
				this.Kuang.gameObject.SetActive(true);
			}
			this.Kuang.position = CameraRoot.Instance.UICamera.ScreenToWorldPoint(tapVector + new Vector3(18f, 10f, 0f));
		}

		public void SetMap(int width, int height, int angle, int _x = 0, int _y = 0, int _type = 0)
		{
			this.MapWidth = (float)width;
			this.MapHeight = (float)height;
			this.mapAngle = (float)angle;
			this.offX = (float)_x;
			this.offY = (float)_y;
			this.type = _type;
		}

		public void ShowTeamSignal(int sender, TeamSignalType notifyType, Vector3 pos)
		{
			try
			{
				if (this.IsOpened)
				{
					GameObject gameObject = NGUITools.AddChild(this._dynamicMark.gameObject, this._teamSignalMark.gameObject);
					if (!(gameObject == null))
					{
						TeamSignalPart component = gameObject.GetComponent<TeamSignalPart>();
						if (component)
						{
							component.Show(notifyType);
						}
						gameObject.transform.localPosition = this.ChangePostion(pos.x, pos.z);
						UnityEngine.Object.Destroy(gameObject, 3f);
						this._posSignals.Add(new MiniMapView.SignalOnPos
						{
							pos = pos,
							senderId = sender,
							signalGo = gameObject
						});
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		public void ShowTeamSignal(int senderId, TeamSignalType notifyType, int targetId)
		{
			try
			{
				if (this.IsOpened)
				{
					GameObject gameObject = NGUITools.AddChild(this._dynamicMark.gameObject, this._teamSignalMark.gameObject);
					if (!(gameObject == null))
					{
						TeamSignalPart component = gameObject.GetComponent<TeamSignalPart>();
						if (component)
						{
							component.Show(notifyType);
						}
						UnityEngine.Object.Destroy(gameObject, 3f);
						MiniMapView.SignalOnUnits signalOnUnits = new MiniMapView.SignalOnUnits
						{
							uid = targetId,
							senderId = senderId,
							signalGo = gameObject
						};
						this._unitSignals.Add(signalOnUnits);
						this.PlaceUnitsSignal(signalOnUnits);
					}
				}
			}
			catch (Exception e)
			{
				ClientLogger.LogException(e);
			}
		}

		private void HideAllUnitSignals()
		{
			if (this._posSignals == null)
			{
				return;
			}
			for (int i = 0; i < this._posSignals.Count; i++)
			{
				if (this._posSignals[i].signalGo != null)
				{
					this._posSignals[i].signalGo.gameObject.SetActive(false);
				}
			}
		}

		private void PlaceUnitsSignal(MiniMapView.SignalOnUnits units)
		{
			if (!units.signalGo)
			{
				return;
			}
			Units unit = MapManager.Instance.GetUnit(units.uid);
			if (!unit)
			{
				units.signalGo.SetActive(false);
				return;
			}
			bool flag = this.CheckUnitIsShow(unit);
			units.signalGo.SetActive(flag);
			if (flag)
			{
				units.signalGo.transform.localPosition = this.ChangePostion(unit.transform.position.x, unit.transform.position.z);
			}
		}

		public void FlyOut()
		{
			this._staticMark.parent.GetComponent<UIPanel>().widgetsAreStatic = false;
			this.transform.Find("Anchor/Content").GetComponent<TweenPosition>().onFinished.Clear();
			this.transform.Find("Anchor/Content").GetComponent<TweenPosition>().PlayForward();
		}

		public void FlyIn()
		{
			this.transform.Find("Anchor/Content").GetComponent<TweenPosition>().onFinished.Add(new EventDelegate(new EventDelegate.Callback(this.SetStaticMaskStatic)));
			this.transform.Find("Anchor/Content").GetComponent<TweenPosition>().PlayReverse();
		}

		private void SetStaticMaskStatic()
		{
			this._staticMark.parent.GetComponent<UIPanel>().widgetsAreStatic = true;
		}

		public float GetMapHeight()
		{
			if (this.Map != null && this.Map.GetComponent<UISprite>() != null)
			{
				return (float)this.Map.GetComponent<UISprite>().height;
			}
			return 0f;
		}

		public void GetMapTexture()
		{
			this.SetSmallMapValue();
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				this.Map.GetComponent<BoxCollider>().enabled = true;
			}
			else if (this.type == 0)
			{
				this.Map.GetComponent<BoxCollider>().enabled = false;
			}
			else if (this.type == 1)
			{
				this.Map.GetComponent<BoxCollider>().enabled = true;
			}
			this.BigMap.GetComponent<UISprite>().MakePixelPerfect();
			UISprite component = this.Map.GetComponent<UISprite>();
			component.MakePixelPerfect();
			this.MapTextureWidth = (float)this.MapTexture.width;
			this.MapTextureHeight = (float)this.MapTexture.height;
			Transform arg_D9_0 = this._staticMark;
			Vector3 localPosition = new Vector3(this.anchor_x, this.anchor_y, 0f);
			this._dynamicMark.localPosition = localPosition;
			arg_D9_0.localPosition = localPosition;
			this.MapCollider.center = new Vector3((float)(-(float)this.MapText.width / 2), (float)(-(float)this.MapText.height / 2), 0f);
			this.MapCollider.size = new Vector3((float)this.MapText.width, (float)this.MapText.height, 0f);
			ActionIndicator module = Singleton<HUDModuleManager>.Instance.GetModule<ActionIndicator>(EHUDModule.ActionIndicator);
			if (module != null)
			{
				module.ResetBottomTrans((float)component.height + 30f);
			}
			SiderTipsModule module2 = Singleton<HUDModuleManager>.Instance.GetModule<SiderTipsModule>(EHUDModule.SiderTips);
			if (module2 != null)
			{
				module2.ResetHeight((float)component.height + 30f);
			}
		}

		private void SetSmallMapValue()
		{
			if (this.type == 0)
			{
				this.minX = 3f;
				this.maxX = 4.2f;
				this.minY = -98.4f;
				this.maxY = -97.7f;
			}
			else
			{
				this.minX = 3.6f;
				this.maxX = 4.45f;
				this.minY = -98.4f;
				this.maxY = -97.7f;
			}
			this.mapTag = 20000;
			this.anchor_x = 73.5f;
			this.anchor_y = 0f;
			this.gameObject.GetComponent<FixMapValue>().SetValue(this.minX, this.maxX, this.minY, this.maxY);
		}

		private void SetBigMapValue()
		{
			this.anchor_x = 0f;
			this.anchor_y = 0f;
			this.minX = 0.5f;
			this.maxX = 3.3f;
			this.minY = -98.92f;
			this.maxY = -97.86f;
			this.mapTag = 40000;
			this.gameObject.GetComponent<FixMapValue>().SetValue(this.minX, this.maxX, this.minY, this.maxY);
		}

		public void SetMapValue(float _minX, float _maxX, float _minY, float _maxY)
		{
			this.minX = _minX;
			this.maxX = _maxX;
			this.minY = _minY;
			this.maxY = _maxY;
		}

		public void BeginTeamSignal()
		{
			this.MapCollider.enabled = true;
			this.BigMapCollider.enabled = true;
		}

		public void EndTeamSignal()
		{
			if (this.mapTag == 40000)
			{
				this.BigMapCollider.enabled = true;
			}
			else if (this.type == 0)
			{
				this.MapCollider.enabled = false;
			}
		}

		public int[] GetMapVector3(string battleId)
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
			if (dataById == null)
			{
				Reminder.ReminderStr("未找到当前的关卡，关卡ID===>" + battleId);
				return null;
			}
			string mini_map = dataById.mini_map;
			if (mini_map == "[]" || mini_map == string.Empty)
			{
				Reminder.ReminderStr("当前关卡对应的小地图ID为空，关卡ID===>" + battleId + "小地图ID===>" + mini_map);
				return null;
			}
			SysMiniMapVo dataById2 = BaseDataMgr.instance.GetDataById<SysMiniMapVo>(mini_map);
			if (dataById2 == null)
			{
				Reminder.ReminderStr(string.Concat(new object[]
				{
					"当前关卡对应的小地图为空，关卡ID===>",
					battleId,
					"小地图ID===>",
					mini_map,
					"minimap表中ID===>",
					dataById2
				}));
				return null;
			}
			string[] array = dataById2.realMapSize.Split(new char[]
			{
				'|'
			});
			return new int[]
			{
				int.Parse(array[0]),
				int.Parse(array[1]),
				int.Parse(array[2]),
				int.Parse(array[3]),
				int.Parse(array[4]),
				int.Parse(array[5])
			};
		}

		private void InitSetMiniMapPic()
		{
			string miniMapPicAtlasPath = this.GetMiniMapPicAtlasPath(LevelManager.CurLevelId);
			if (string.IsNullOrEmpty(miniMapPicAtlasPath))
			{
				return;
			}
			int num = miniMapPicAtlasPath.LastIndexOf('/');
			if (num == -1)
			{
				return;
			}
			string text = miniMapPicAtlasPath.Substring(num + 1);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			this._miniMapPicAtlas = Resources.Load<UIAtlas>(miniMapPicAtlasPath);
			if (this._miniMapPicAtlas == null)
			{
				return;
			}
			if (this.Map != null)
			{
				UISprite component = this.Map.GetComponent<UISprite>();
				if (component != null)
				{
					component.atlas = this._miniMapPicAtlas;
					component.spriteName = text;
				}
			}
			if (this.BigMap != null)
			{
				UISprite component = this.BigMap.GetComponent<UISprite>();
				if (component != null)
				{
					component.atlas = this._miniMapPicAtlas;
					component.spriteName = text + "_Big";
				}
			}
		}

		private void ClearMiniMapPicResources()
		{
			if (this.Map != null)
			{
				UISprite component = this.Map.GetComponent<UISprite>();
				if (component != null)
				{
					component.atlas = null;
				}
			}
			if (this.BigMap != null)
			{
				UISprite component = this.BigMap.GetComponent<UISprite>();
				if (component != null)
				{
					component.atlas = null;
				}
			}
			if (this._miniMapPicAtlas != null)
			{
				this._miniMapPicAtlas = null;
			}
		}

		private string GetMiniMapPicAtlasPath(string battleId)
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(battleId);
			if (dataById == null)
			{
				return string.Empty;
			}
			string mini_map = dataById.mini_map;
			if (mini_map == "[]" || mini_map == string.Empty)
			{
				return string.Empty;
			}
			SysMiniMapVo dataById2 = BaseDataMgr.instance.GetDataById<SysMiniMapVo>(mini_map);
			if (dataById2 == null)
			{
				return string.Empty;
			}
			SysGameResVo gameResData = BaseDataMgr.instance.GetGameResData(dataById2.miniMap);
			if (gameResData != null)
			{
				return gameResData.path;
			}
			return string.Empty;
		}

		public bool IsShowMiNiMap(string battleId)
		{
			return true;
		}

		private List<Units> GetAllSightEnemy()
		{
			TeamType myTeam = TeamManager.MyTeam;
			IList<TeamType> enemyTeams = TeamManager.EnemyTeams;
			List<Units> list = new List<Units>();
			List<Units> units = this.GetUnits(new TeamType[]
			{
				myTeam
			});
			List<Units> units2 = this.GetUnits(enemyTeams);
			List<Units> collection = MapManager.Instance.EnumMapUnits(enemyTeams, TargetTag.Tower).ToList<Units>();
			List<Units> collection2 = MapManager.Instance.EnumMapUnits(enemyTeams, TargetTag.Home).ToList<Units>();
			if (units != null && units.Count != 0)
			{
				for (int i = 0; i < units.Count; i++)
				{
					if (units2 == null || units2.Count == 0)
					{
						break;
					}
					for (int j = 0; j < units2.Count; j++)
					{
						if (!this.JudgeSight(units[i], units2[j]) && !list.Contains(units2[j]))
						{
							list.Add(units2[j]);
						}
					}
				}
				list.AddRange(collection);
				list.AddRange(collection2);
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (units2.Contains(list[k]))
				{
					units2.Remove(list[k]);
				}
			}
			return units2;
		}

		private List<Units> GetUnits(IList<TeamType> team)
		{
			List<Units> list = new List<Units>();
			List<Units> list2 = MapManager.Instance.EnumMapUnits(team, TargetTag.All).ToList<Units>();
			if (list2.Count == 0)
			{
				return list;
			}
			foreach (Units current in list2)
			{
				if (current.isLive)
				{
					list.Add(current);
				}
			}
			return list;
		}

		private bool JudgeSight(Units self, Units target)
		{
			return !Singleton<PvpManager>.Instance.IsGlobalObserver && (null == self || target == null || self.GetAttr(AttrType.WarningRange) < (self.transform.position - target.transform.position).sqrMagnitude);
		}

		public void ShowChestAlert(float x, float z)
		{
			Transform mark = this.CreateMapItem(this.ChestAlertMark.transform, false);
			mark.localPosition = this.ChangePostion(x, z);
			ObjectRecycler<Transform> pool = this._mapItemPools[this.ChestAlertMark];
			new Task(delegate
			{
				if (pool != null)
				{
					pool.Release(mark);
				}
			}, 3f);
		}

		public void UpdateAfterReConect()
		{
			if (this._mapItemList == null)
			{
				return;
			}
			Dictionary<int, MiniMapView.MapItemInfo>.Enumerator enumerator = this._mapItemList.GetEnumerator();
			List<int> list = new List<int>();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, MiniMapView.MapItemInfo> current = enumerator.Current;
				if (current.Value.needTarget)
				{
					KeyValuePair<int, MiniMapView.MapItemInfo> current2 = enumerator.Current;
					if (current2.Value.targetUnits == null)
					{
						List<int> arg_6E_0 = list;
						KeyValuePair<int, MiniMapView.MapItemInfo> current3 = enumerator.Current;
						arg_6E_0.Add(current3.Key);
					}
				}
			}
			List<int>.Enumerator enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				this._mapItemList.Remove(enumerator2.Current);
			}
			list.Clear();
			if (this.MiniMapGroupDic == null)
			{
				return;
			}
			Dictionary<int, MiniMapGroup>.Enumerator enumerator3 = this.MiniMapGroupDic.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				KeyValuePair<int, MiniMapGroup> current4 = enumerator3.Current;
				current4.Value.m_group.RemoveAll((Units obj) => obj == null);
				KeyValuePair<int, MiniMapGroup> current5 = enumerator3.Current;
				if (current5.Value.m_group.Count == 0)
				{
					KeyValuePair<int, MiniMapGroup> current6 = enumerator3.Current;
					current6.Value.ShowSp(false);
				}
			}
			this.OnChangeMapBtn(null);
			this.OnChangeMapBtn(null);
		}
	}
}
