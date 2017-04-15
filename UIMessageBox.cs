using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class UIMessageBox : MonoBehaviour
{
	private struct sKillMsg
	{
		public string msg;

		public float duration;

		public string npcId1;

		public string npcId2;

		public bool KillEnmeyOrDeath;

		public EntityType attackerType;

		public string voiceName;

		public EntityType deathType;

		public string summerName1;

		public string sunmerName2;

		public sKillMsg(string msg, float duration, string npcId1, string npcId2, bool KillEnmeyOrDeath, EntityType attackerType, string voiceName, EntityType deathType, string summerName1 = "", string sunmerName2 = "")
		{
			this.msg = msg;
			this.duration = duration;
			this.npcId1 = npcId1;
			this.npcId2 = npcId2;
			this.KillEnmeyOrDeath = KillEnmeyOrDeath;
			this.attackerType = attackerType;
			this.voiceName = voiceName;
			this.summerName1 = summerName1;
			this.sunmerName2 = sunmerName2;
			this.deathType = deathType;
		}
	}

	public bool Lock = true;

	private static UIMessageBox m_Instance;

	[SerializeField]
	private Transform BulletinBox2;

	[SerializeField]
	private UILabel BulletinLabel;

	[SerializeField]
	private UISprite BulletinBg;

	[SerializeField]
	private Transform SkillBox3;

	[SerializeField]
	private UISprite Attacker;

	[SerializeField]
	private UISprite Deather;

	[SerializeField]
	private UISprite Attacker_Sprite;

	[SerializeField]
	private UISprite Deather_Sprite;

	[SerializeField]
	private GameObject LightBg;

	[SerializeField]
	private UILabel PromptLbl;

	[SerializeField]
	private Transform EffectMsg;

	[SerializeField]
	private UISprite AttackerFrame_eff;

	[SerializeField]
	private UISprite AttackerIcon_eff;

	[SerializeField]
	private UISprite DeatherFrame_eff;

	[SerializeField]
	private UISprite DeatherIcon_eff;

	[SerializeField]
	private GameObject doubleKillEff;

	[SerializeField]
	private GameObject doubleKillEff_self;

	[SerializeField]
	private GameObject tripleKillEff;

	[SerializeField]
	private GameObject tripleKillEff_self;

	[SerializeField]
	private GameObject quatartyKillEff;

	[SerializeField]
	private GameObject quatartyKillEff_self;

	[SerializeField]
	private GameObject pentaKillEff;

	[SerializeField]
	private GameObject pentaKillEff_self;

	[SerializeField]
	private GameObject hexaKillEff;

	[SerializeField]
	private GameObject hexaKillEff_self;

	[SerializeField]
	private Transform singlePlayerMsg;

	[SerializeField]
	private EffectPlayTool lightEff_sp;

	[SerializeField]
	private UISprite frame_sp;

	[SerializeField]
	private UISprite icon_sp;

	[SerializeField]
	private UILabel label_sp;

	[SerializeField]
	private Transform gainEquipMsg;

	[SerializeField]
	private GameObject gainEquip_EffBg;

	[SerializeField]
	private UISprite gainEquip_SummonerIconFrame;

	[SerializeField]
	private UISprite gainEquip_SummonerIcon;

	[SerializeField]
	private UITexture gainEquip_Texture;

	[SerializeField]
	private UILabel gainEquip_Label;

	private Color32 blue = new Color32(0, 204, 255, 255);

	private Color32 white = new Color32(255, 255, 255, 255);

	private static CoroutineManager m_CoroutineManager = new CoroutineManager();

	private Task _taskTiming;

	private GameObject RightKillItem;

	private GameObject RightSignalItem;

	public static int CUITextIdSpawnSuperSoldierSelfCamp = 1123;

	public static int CUITextIdSpawnSuperSoldierEnemyCamp = 1122;

	private static Color32 Green = new Color32(24, 154, 25, 255);

	private static Color32 Red = new Color32(220, 0, 20, 255);

	private static Color32 Yellow = new Color32(220, 220, 0, 255);

	private static Color32 Magenta = new Color32(219, 0, 155, 255);

	private static Color32 Cyan = new Color32(38, 230, 231, 255);

	private static Color32 Yellow_T3 = new Color32(244, 224, 49, 255);

	private List<UIMessageBox.sKillMsg> killMsgList = new List<UIMessageBox.sKillMsg>();

	private bool showingKillMsg;

	private bool mIsInReconnectedPromptCatchCD;

	public List<PromptInfo> mPromptInfos = new List<PromptInfo>();

	private readonly string[] eggStrArr = new string[]
	{
		"商人<我就说我这儿的装备特厉害",
		"巨兽<厉害了",
		"赤蛇<66666",
		"商人<有金币记得来我这光顾！",
		"金矿<啊！金币的味道！",
		"红棕狼<好慌啊我的蓝棕狼",
		"蓝棕狼<不要怕我的红棕狼",
		"巨兽<(っ °Д °;)っ",
		"商人<你看，趁手的兵器多好用",
		"防御塔<盯……",
		"红棕狼<我擦！     蓝棕狼*#002*害怕.jpg",
		"蓝魔<我觉得我还可以抢救一下……",
		"红魔<其实我觉得蓝魔脑子不好使",
		"小兵<下次能不能温柔一点！",
		"超级兵<仿佛听到有人说我帅",
		"绿龙<我还能怎么办，我也很绝望啊",
		"峡谷<能不能不要在我头上搞事情！",
		"小兵<666666",
		"巨兽<行家啊~",
		"绿龙仆从<吓得我脸都绿了",
		"商人<瞬间爆炸！",
		"小兵<有人心疼一下死于不明伤害的我吗？",
		"金矿<？？？",
		"符文石块<嗡~嗡~嗡~",
		"小树精<有人看见了阿弗拉姐姐吗？",
		"炮兵<胜利！！",
		"小兵<老大！打谁您说话！",
		"商人<哇！金币！",
		"金矿<我有时候也很疑惑，为什么你们死了也会掉金币？",
		"商人<我以前也是个召唤师",
		"符文石块<您身上也有符文的力量",
		"增益球<我好圆啊……",
		"小兵<您的特效在哪领的？",
		"绿龙<我就不信你还能开大招来打我！",
		"巨兽<臣服于我的力量！！",
		"小兵<喵喵喵？？"
	};

	public PromptInfo current
	{
		get;
		set;
	}

	public static UIMessageBox Instance
	{
		get
		{
			return UIMessageBox.m_Instance;
		}
	}

	public static void HideAllBox()
	{
		Singleton<AttachedPropertyView>.Instance.gameObject.SetActive(false);
	}

	private void Awake()
	{
		this.RightKillItem = (Resources.Load("Prefab/UI/Battle/KillHeroItem") as GameObject);
		this.RightSignalItem = (Resources.Load("Prefab/UI/Battle/SignalHeroItem") as GameObject);
		UIMessageBox.m_Instance = this;
		int depth = Singleton<HUDModuleManager>.Instance.GetDepth(255);
		this.BulletinBox2.GetComponent<UIPanel>().depth = depth + 1;
		this.singlePlayerMsg.GetComponent<UIPanel>().depth = depth + 3;
		this.SkillBox3.GetComponent<UIPanel>().depth = depth + 2;
		this.gainEquipMsg.GetComponent<UIPanel>().depth = depth + 1;
	}

	private void OnDisable()
	{
		UIMessageBox.StopTimeUp();
		UIMessageBox.m_CoroutineManager.StopAllCoroutine();
		base.CancelInvoke();
		Units.OnUnitsDead -= new Action<Units, Units>(this.OnTowerDeath);
	}

	private void OnEnable()
	{
		Units.OnUnitsDead += new Action<Units, Units>(this.OnTowerDeath);
		MobaMessageManager.RegistMessage((ClientMsg)20007, new MobaMessageFunc(this.ClearPromptList));
	}

	private void OnDestroy()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)20007, new MobaMessageFunc(this.ClearPromptList));
		UIMessageBox.m_CoroutineManager.StopAllCoroutine();
		base.CancelInvoke();
		UIMessageBox.m_Instance = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (this.current == null && this.mPromptInfos.Count > 0 && !this.mIsInReconnectedPromptCatchCD)
		{
			this.current = this.mPromptInfos[0];
			this.mPromptInfos.RemoveAt(0);
			switch (this.current.promptType)
			{
			case PromptType.SimpleText:
				UIMessageBox.m_CoroutineManager.StartCoroutine(this.ShowSimpleText(), true);
				break;
			case PromptType.MultiPlayer:
				UIMessageBox.m_CoroutineManager.StartCoroutine(this.ShowKillMsg(), true);
				break;
			case PromptType.SinglePlayer:
				UIMessageBox.m_CoroutineManager.StartCoroutine(this.ShowSinglePlayerMsg(), true);
				break;
			case PromptType.Effect:
				UIMessageBox.m_CoroutineManager.StartCoroutine(this.ShowEffectMsg(), true);
				break;
			case PromptType.GainEquip:
				UIMessageBox.m_CoroutineManager.StartCoroutine(this.ShowGainEquipMsg(), true);
				break;
			}
		}
	}

	public static void DeathBox(float spawn_interval)
	{
		if (UIMessageBox.Instance == null)
		{
			return;
		}
		UIMessageBox.Instance.DeathBox_Internal(spawn_interval);
	}

	public void DeathBox_Internal(float spawn_interval)
	{
		if (UIMessageBox.Instance == null)
		{
			return;
		}
		base.InvokeRepeating("DeathMessage", 0f, 1f);
	}

	public static void StopTimeUp()
	{
		if (!UIMessageBox.Instance)
		{
			return;
		}
		if (UIMessageBox.Instance._taskTiming != null)
		{
			UIMessageBox.Instance._taskTiming.Stop();
			UIMessageBox.Instance._taskTiming = null;
		}
	}

	private bool IsHero(string id)
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysHeroMainVo>();
		return dicByType != null && dicByType.ContainsKey(id);
	}

	[DebuggerHidden]
	private IEnumerator displayMsg()
	{
		UIMessageBox.<displayMsg>c__IteratorF1 <displayMsg>c__IteratorF = new UIMessageBox.<displayMsg>c__IteratorF1();
		<displayMsg>c__IteratorF.<>f__this = this;
		return <displayMsg>c__IteratorF;
	}

	public static void ShowTowerWoundWarn(Units unit)
	{
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			return;
		}
		if (!unit.isMyTeam)
		{
			return;
		}
		if (unit.isTower)
		{
			UIMessageBox.ShowTextPrompt("1104");
		}
		else if (unit.isHome)
		{
			UIMessageBox.ShowTextPrompt("1130");
		}
	}

	private void OnTowerDeath(Units unit, Units attacker)
	{
		if (unit == null || attacker == null)
		{
			return;
		}
		if (unit.isTower)
		{
			string towerDestroyId = PromptHelper.GetTowerDestroyId(unit);
			UIMessageBox.ShowKillPrompt(towerDestroyId, attacker.npc_id, unit.npc_id, EntityType.Hero, EntityType.Tower, string.Empty, string.Empty, attacker.TeamType, unit.TeamType);
		}
		else if (unit.isHome)
		{
			if (!unit.isMyTeam)
			{
				return;
			}
			SysMonsterMainVo monsterMainData = BaseDataMgr.instance.GetMonsterMainData(unit.npc_id);
			if (monsterMainData != null)
			{
				UIMessageBox.ShowMessage(LanguageManager.Instance.GetStringById(monsterMainData.name) + "已阵亡", 3f, 629);
			}
			else
			{
				UIMessageBox.ShowMessage("指挥官已阵亡", 3f, 629);
			}
		}
	}

	public static void ShowSpawnSuperSoldierMsg(TeamType inSpawnSoldierTeam, TeamType inSelfTeam)
	{
		int num = UIMessageBox.CUITextIdSpawnSuperSoldierSelfCamp;
		if (inSpawnSoldierTeam != inSelfTeam)
		{
			num = UIMessageBox.CUITextIdSpawnSuperSoldierEnemyCamp;
		}
		UIMessageBox.ShowTextPrompt(num.ToString());
	}

	public static void ShowTextPrompt(string _promptId)
	{
		if (UIMessageBox.Instance == null || LevelManager.CurBattleType == 6 || string.IsNullOrEmpty(_promptId))
		{
			return;
		}
		PromptInfo promptInfo = UIMessageBox.Instance.mPromptInfos.Find((PromptInfo _info) => _info.promptId == _promptId);
		if (promptInfo != null)
		{
			return;
		}
		promptInfo = new PromptInfo
		{
			promptId = _promptId,
			promptType = PromptType.SimpleText
		};
		UIMessageBox.Instance.mPromptInfos.Add(promptInfo);
	}

	public static void ShowMessage(string mess, float time = 1.5f, int BgLong = 0)
	{
		if (UIMessageBox.Instance == null || LevelManager.CurBattleType == 6)
		{
			return;
		}
		PromptInfo promptInfo = UIMessageBox.Instance.mPromptInfos.Find((PromptInfo _info) => _info.promptText == mess);
		if (promptInfo != null)
		{
			return;
		}
		promptInfo = new PromptInfo
		{
			promptText = mess,
			showTime = time,
			promptType = PromptType.SimpleText,
			bgWidth = BgLong
		};
		UIMessageBox.Instance.mPromptInfos.Add(promptInfo);
	}

	[DebuggerHidden]
	private IEnumerator ShowSimpleText()
	{
		UIMessageBox.<ShowSimpleText>c__IteratorF2 <ShowSimpleText>c__IteratorF = new UIMessageBox.<ShowSimpleText>c__IteratorF2();
		<ShowSimpleText>c__IteratorF.<>f__this = this;
		return <ShowSimpleText>c__IteratorF;
	}

	public static void ShowKillPrompt(string _promptId, string npcId1, string npcId2, EntityType attackerType, EntityType deathType = EntityType.None, string summerName1 = "", string sunmerName2 = "", TeamType _attackerTeam = TeamType.None, TeamType _victimTeam = TeamType.None)
	{
		if (UIMessageBox.Instance == null)
		{
			return;
		}
		if (!UIMessageBox.Instance.Lock)
		{
			return;
		}
		if (Singleton<PvpManager>.Instance.IsObserver && attackerType != EntityType.Hero && deathType != EntityType.Hero && deathType != EntityType.Creep)
		{
			return;
		}
		UIMessageBox.Instance.FilterAcedPrompt(_promptId);
		SysPvpPromptVo dataById = BaseDataMgr.instance.GetDataById<SysPvpPromptVo>(_promptId);
		if (dataById != null)
		{
			PromptInfo item = new PromptInfo
			{
				promptId = _promptId,
				promptType = (PromptType)dataById.text_type,
				npcId1 = npcId1,
				npcId2 = npcId2,
				attackerType = attackerType,
				summerName1 = summerName1,
				sunmerName2 = sunmerName2,
				deathType = deathType,
				attackerTeam = _attackerTeam,
				deathTeam = _victimTeam
			};
			UIMessageBox.Instance.mPromptInfos.Add(item);
		}
	}

	[DebuggerHidden]
	private IEnumerator ShowKillMsg()
	{
		UIMessageBox.<ShowKillMsg>c__IteratorF3 <ShowKillMsg>c__IteratorF = new UIMessageBox.<ShowKillMsg>c__IteratorF3();
		<ShowKillMsg>c__IteratorF.<>f__this = this;
		return <ShowKillMsg>c__IteratorF;
	}

	[DebuggerHidden]
	private IEnumerator ShowSinglePlayerMsg()
	{
		UIMessageBox.<ShowSinglePlayerMsg>c__IteratorF4 <ShowSinglePlayerMsg>c__IteratorF = new UIMessageBox.<ShowSinglePlayerMsg>c__IteratorF4();
		<ShowSinglePlayerMsg>c__IteratorF.<>f__this = this;
		return <ShowSinglePlayerMsg>c__IteratorF;
	}

	[DebuggerHidden]
	private IEnumerator ShowEffectMsg()
	{
		UIMessageBox.<ShowEffectMsg>c__IteratorF5 <ShowEffectMsg>c__IteratorF = new UIMessageBox.<ShowEffectMsg>c__IteratorF5();
		<ShowEffectMsg>c__IteratorF.<>f__this = this;
		return <ShowEffectMsg>c__IteratorF;
	}

	public static void ShowEquipMsg(string _unit_id, string _equip_id)
	{
		if (UIMessageBox.Instance == null)
		{
			return;
		}
		if (!UIMessageBox.Instance.Lock)
		{
			return;
		}
		int uniqueId = 0;
		Units units;
		if (!int.TryParse(_unit_id, out uniqueId))
		{
			units = MapManager.Instance.GetPlayer();
		}
		else
		{
			units = MapManager.Instance.GetUnit(uniqueId);
			if (units == null)
			{
				units = MapManager.Instance.GetPlayer();
			}
		}
		PromptInfo item = new PromptInfo
		{
			promptId = "1177",
			promptType = PromptType.GainEquip,
			npcId1 = units.npc_id,
			npcId2 = _equip_id,
			attackerType = EntityType.Hero,
			summerName1 = units.teamType.ToString(),
			sunmerName2 = string.Empty
		};
		UIMessageBox.Instance.mPromptInfos.Add(item);
	}

	[DebuggerHidden]
	private IEnumerator ShowGainEquipMsg()
	{
		UIMessageBox.<ShowGainEquipMsg>c__IteratorF6 <ShowGainEquipMsg>c__IteratorF = new UIMessageBox.<ShowGainEquipMsg>c__IteratorF6();
		<ShowGainEquipMsg>c__IteratorF.<>f__this = this;
		return <ShowGainEquipMsg>c__IteratorF;
	}

	private void FilterAcedPrompt(string _newPrompt)
	{
		if (PromptHelper.IsTuanmie(_newPrompt))
		{
			PromptInfo promptInfo = UIMessageBox.Instance.mPromptInfos.Find((PromptInfo obj) => obj.promptId == _newPrompt);
			if (promptInfo != null)
			{
				UIMessageBox.Instance.mPromptInfos.Remove(promptInfo);
			}
		}
	}

	private void ClearPromptList(MobaMessage msg)
	{
		UIMessageBox.Instance.mPromptInfos.Clear();
		this.mIsInReconnectedPromptCatchCD = true;
		base.StartCoroutine(this.ReconnectedPromptCatchCD());
	}

	[DebuggerHidden]
	private IEnumerator ReconnectedPromptCatchCD()
	{
		UIMessageBox.<ReconnectedPromptCatchCD>c__IteratorF7 <ReconnectedPromptCatchCD>c__IteratorF = new UIMessageBox.<ReconnectedPromptCatchCD>c__IteratorF7();
		<ReconnectedPromptCatchCD>c__IteratorF.<>f__this = this;
		return <ReconnectedPromptCatchCD>c__IteratorF;
	}
}
