using Assets.MobaTools.TriggerPlugin.Scripts;
using Assets.Scripts.Character.Control;
using Assets.Scripts.Model;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using MobaFrame.Move;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Controller;
using MobaHeros.Pvp;
using MobaHeros.Spawners;
using MobaProtocol;
using MobaProtocol.Data;
using MobaTools.Prefab;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Units : BaseEntity
{
	public static class CompType<T> where T : UnitComponent, new()
	{
		public static bool isStatic = Activator.CreateInstance<T>() is StaticUnitComponent;
	}

	public bool debuging;

	public bool IsSyncPosition = true;

	[SerializeField]
	private string m_tag;

	public float m_Radius = 0.5f;

	public float m_SelectRadius = 0.5f;

	public float m_ColliderHeight = 0.5f;

	public float m_fLiveTime = -1f;

	public float m_fLeftTime;

	[HideInInspector]
	public bool DebugDLog;

	[NonSerialized]
	public float sourceAnimSpeed;

	[NonSerialized]
	public float sourceMoveSpeed;

	private float m_hp_init_p = 1f;

	private float m_mp_init_p = 1f;

	private UnitsLifeTime m_unitsLifeTime;

	private UnitControlType _unitControlType;

	private Vector3 moveDirection;

	[NonSerialized]
	public float attackExtraDamage;

	[NonSerialized]
	public float attackMultipleDamage;

	[NonSerialized]
	public float beheadedCoefficient;

	[NonSerialized]
	public float attackReboundCoefficient;

	[NonSerialized]
	public float a_growthFactor = 1f;

	[NonSerialized]
	public string attackForTargetBuff = string.Empty;

	[NonSerialized]
	private int startCount;

	[NonSerialized]
	public int m_nGrassState;

	[NonSerialized]
	public int m_nVisibleState;

	public int m_nServerVisibleState = 2;

	public bool m_bUpdateVisible = true;

	[NonSerialized]
	public float m_fVisibleTimer;

	[NonSerialized]
	public bool m_bIgnoreVisible;

	[NonSerialized]
	public FOWRevealer m_fowrevealer;

	[NonSerialized]
	public HeroVoicePlayer m_hv;

	private float queryTime;

	private float _nextCheckSyncTime;

	private float _lastSyncHp;

	private Units parentUnit;

	[NonSerialized]
	public int killNum;

	[NonSerialized]
	public int rewardNum;

	[NonSerialized]
	public int hitNum;

	[NonSerialized]
	public int exp_cur;

	[NonSerialized]
	public int assistantNum;

	private float hurtTime;

	private Units m_lastHurtHero;

	public Dictionary<Units, float> m_assistantDict = new Dictionary<Units, float>();

	private List<string> m_assistantList = new List<string>();

	protected CrickTime crickTime;

	protected UnitsTimeSyncSystem timeSyncSystem;

	public SurfaceManager surface;

	protected StatisticsManager statistics;

	public AIManager aiManager;

	private Renderer unitRenderer;

	public DataManager data;

	public DataChangeManager dataChange;

	public AnimController animController;

	public SkillManager skillManager;

	public BuffManager buffManager;

	public HighEffManager highEffManager;

	public EffectManager effectManager;

	protected UnitCollider unitCollider;

	[HideInInspector]
	public bool isVisibleInCamera = true;

	public MoveController moveController;

	protected AttackController atkController;

	public CmdCacheController mCmdCacheController;

	public CmdRunningController mCmdRunningController;

	public ShowSkillPointerCrazy mShowSkillPointerCrazy;

	public ShowSkillPointerNormal mShowSkillPointerNormal;

	public ManualController mManualController;

	public BattleTouchController mBtlTouchController;

	private bool isCrazyMode;

	protected CoroutineManager m_CoroutineManager = new CoroutineManager();

	private GameObject cameraVisibleTool;

	private int updateCameraVisibleIdx;

	[SerializeField]
	protected CapsuleCollider m_collider;

	[SerializeField]
	protected Mecanim m_mecanim;

	[SerializeField]
	protected SkinnedMeshRenderer m_render;

	[SerializeField]
	protected Renderer[] m_renders;

	[SerializeField]
	protected CharacterEffect m_characterEffect;

	[SerializeField]
	protected Transform m_model;

	[SerializeField]
	public Transform[] bones;

	[NonSerialized]
	protected GameObject m_obstacleObj;

	protected CapsuleCollider m_obstacleCollider;

	private GameObject m_selectObj;

	protected GameObject m_memory;

	protected SphereCollider m_vechileCollider;

	protected GameObject m_aiObj;

	private bool m_isStart;

	private bool isUpLevel;

	private bool canRoatate = true;

	private bool isActionEnd = true;

	private int beAttackCountInShortTime;

	private bool isCanMove = true;

	private bool isCanAction = true;

	private bool isCanAttack = true;

	private bool isCanSkill = true;

	private bool isCanAIControl = true;

	private bool isAttackStart;

	private bool isEnableActionHighEff = true;

	private bool m_isTaunted;

	private bool m_isCharm;

	private bool isFrozenAnimtion;

	private bool isLockCharaEffect;

	private bool isLockCharaControl;

	private bool isLockInput;

	private bool isLockAnimState;

	protected bool isRemoved;

	private bool isForceClickGroud;

	private bool isHideEffect;

	private bool isRebirthing;

	private bool m_isLive;

	private bool m_isSelected;

	[NonSerialized]
	public bool isPlayer;

	[NonSerialized]
	public bool isHome;

	[NonSerialized]
	public bool isTower;

	[NonSerialized]
	public bool isShop;

	[NonSerialized]
	public bool isBuilding;

	[NonSerialized]
	public bool isItem;

	[NonSerialized]
	public bool isBuffItem;

	[NonSerialized]
	public bool isHero;

	[NonSerialized]
	public bool isMonster;

	private int m_state;

	public CharaState[] m_states;

	private bool canManualControl = true;

	public Callback GetkillHeroNum;

	public Callback GetHeroDeathNum;

	private Units m_fakeAttackTarget;

	private bool mirrorState;

	private Skill contextSkill;

	private Vector3 contextPos;

	private Transform mMiniMapIcon;

	private static float _size = 0.2f;

	public string AIDebugStr = string.Empty;

	[NonSerialized]
	public int recvPath;

	private bool _showServerPos = true;

	private bool _showServerTarget;

	public static bool ShowServerUnitAI = true;

	[NonSerialized]
	public Vector3 serverPos;

	[NonSerialized]
	public Vector3 serverTargetPos;

	private bool _isMonsterCreep;

	public UnitController UnitController;

	private Task m_GrassTask;

	private GameObject _newbieHintObj;

	private List<MobaMessage> pvpSvrMsgList = new List<MobaMessage>();

	private List<UnitComponent> dynamicComponents = new List<UnitComponent>();

	private List<UnitComponent> staticComponents = new List<UnitComponent>();

	private float unitComponentUpdateDelaTime;

	private float componentUpdatedelayTime = 0.17f;

	public event Callback<Units> OnWoundCallback
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnWoundCallback = (Callback<Units>)Delegate.Combine(this.OnWoundCallback, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnWoundCallback = (Callback<Units>)Delegate.Remove(this.OnWoundCallback, value);
		}
	}

	public event Callback<Units> OnDeathCallback
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnDeathCallback = (Callback<Units>)Delegate.Combine(this.OnDeathCallback, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnDeathCallback = (Callback<Units>)Delegate.Remove(this.OnDeathCallback, value);
		}
	}

	public event Callback<Units> OnRebornCallback
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnRebornCallback = (Callback<Units>)Delegate.Combine(this.OnRebornCallback, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnRebornCallback = (Callback<Units>)Delegate.Remove(this.OnRebornCallback, value);
		}
	}

	public event Callback<Units> OnAttackCallback
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnAttackCallback = (Callback<Units>)Delegate.Combine(this.OnAttackCallback, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnAttackCallback = (Callback<Units>)Delegate.Remove(this.OnAttackCallback, value);
		}
	}

	public event Callback<Units> OnSkillCallback
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnSkillCallback = (Callback<Units>)Delegate.Combine(this.OnSkillCallback, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnSkillCallback = (Callback<Units>)Delegate.Remove(this.OnSkillCallback, value);
		}
	}

	public event Callback<float, float> OnUseMagic
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnUseMagic = (Callback<float, float>)Delegate.Combine(this.OnUseMagic, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnUseMagic = (Callback<float, float>)Delegate.Remove(this.OnUseMagic, value);
		}
	}

	public static event Action<Units, Units> OnUnitsDead
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			Units.OnUnitsDead = (Action<Units, Units>)Delegate.Combine(Units.OnUnitsDead, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			Units.OnUnitsDead = (Action<Units, Units>)Delegate.Remove(Units.OnUnitsDead, value);
		}
	}

	public static event Action<Units> OnUnitsRebirth
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			Units.OnUnitsRebirth = (Action<Units>)Delegate.Combine(Units.OnUnitsRebirth, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			Units.OnUnitsRebirth = (Action<Units>)Delegate.Remove(Units.OnUnitsRebirth, value);
		}
	}

	public static event Action<Units, float> OnUnitsWound
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			Units.OnUnitsWound = (Action<Units, float>)Delegate.Combine(Units.OnUnitsWound, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			Units.OnUnitsWound = (Action<Units, float>)Delegate.Remove(Units.OnUnitsWound, value);
		}
	}

	public UnitType UnitType
	{
		get;
		set;
	}

	public new string tag
	{
		get
		{
			if (this.m_tag == null || this.m_tag.Equals(string.Empty))
			{
				this.m_tag = base.gameObject.tag;
			}
			return this.m_tag;
		}
		set
		{
			if (this.tag == "Player" && Singleton<PvpManager>.Instance.IsObserver)
			{
				ClientLogger.Error("can not set player when in obserser mode");
			}
			if (this.m_tag != value)
			{
				base.gameObject.tag = value;
				this.m_tag = value;
			}
		}
	}

	public Vector3 ColliderCenter
	{
		get
		{
			Vector3 position = base.transform.position;
			position.y += this.m_ColliderHeight * 0.3f;
			return position;
		}
	}

	public string model_id
	{
		get
		{
			return this.GetData<string>(DataType.ModelId);
		}
		set
		{
			this.SetData<string>(DataType.ModelId, value);
		}
	}

	public string effect_id
	{
		get
		{
			return this.GetData<string>(DataType.EffectId);
		}
		set
		{
			this.SetData<string>(DataType.EffectId, value);
		}
	}

	public float hp_max
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.HpMax), 0f, 3.40282347E+38f);
		}
	}

	public float hp
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Hp), 0f, this.hp_max);
		}
	}

	public float mp_max
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.MpMax), 0f, 3.40282347E+38f);
		}
	}

	public float mp
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Mp), 0f, this.mp_max);
		}
	}

	public int level
	{
		get
		{
			return this.GetData<int>(DataType.Level);
		}
		set
		{
			if (this.level == value)
			{
				return;
			}
			this.SetData<int>(DataType.Level, value);
		}
	}

	public int star
	{
		get
		{
			return this.GetData<int>(DataType.Star);
		}
		set
		{
			this.SetData<int>(DataType.Star, value);
		}
	}

	public int quality
	{
		get
		{
			return this.GetData<int>(DataType.Quality);
		}
		set
		{
			this.SetData<int>(DataType.Quality, value);
		}
	}

	public float power
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Power), 0f, 3.40282347E+38f);
		}
	}

	public float agile
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Agile), 0f, 3.40282347E+38f);
		}
	}

	public float intelligence
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Intelligence), 0f, 3.40282347E+38f);
		}
	}

	public float hp_restore
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.HpRestore), 0f, 3.40282347E+38f);
		}
	}

	public float mp_restore
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.MpRestore), 0f, 3.40282347E+38f);
		}
	}

	public float attack
	{
		get
		{
			return this.GetAttr(AttrType.Attack);
		}
	}

	public float armor
	{
		get
		{
			return this.GetAttr(AttrType.Armor);
		}
	}

	public float armor_cut_percentage
	{
		get
		{
			return Mathf.Clamp01(this.GetAttr(AttrType.ArmorCut_Percentage));
		}
	}

	public float magic_resist_cut_percentage
	{
		get
		{
			return Mathf.Clamp01(this.GetAttr(AttrType.MagicResistanceCut_Percentage));
		}
	}

	public float armor_cut
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.ArmorCut), 0f, 3.40282347E+38f);
		}
	}

	public float dodge_prop
	{
		get
		{
			return this.GetAttr(AttrType.DodgeProp);
		}
	}

	public float hit_prop
	{
		get
		{
			return this.GetAttr(AttrType.HitProp);
		}
	}

	public float move_speed
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.MoveSpeed), 0f, 3.40282347E+38f);
		}
	}

	public float attack_speed
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.AttackSpeed), 0f, 3.40282347E+38f);
		}
	}

	public float attack_timelenth
	{
		get
		{
			if (this.attack_speed == 0f)
			{
				return 0f;
			}
			return 1f / this.attack_speed;
		}
	}

	public float physic_crit_prop
	{
		get
		{
			return this.GetAttr(AttrType.PhysicCritProp);
		}
	}

	public float physic_crit_mag
	{
		get
		{
			return this.GetAttr(AttrType.PhysicCritMag);
		}
	}

	public float magic_crit_prop
	{
		get
		{
			return this.GetAttr(AttrType.MagicCritProp);
		}
	}

	public float magic_crit_mag
	{
		get
		{
			return this.GetAttr(AttrType.MagicCritMag);
		}
	}

	public float magic_resist
	{
		get
		{
			return this.GetAttr(AttrType.MagicResist);
		}
	}

	public float magic_resist_cut
	{
		get
		{
			return this.GetAttr(AttrType.MagicResistanceCut);
		}
	}

	public float physic_power
	{
		get
		{
			return this.GetAttr(AttrType.PhysicPower);
		}
	}

	public float magic_power
	{
		get
		{
			return this.GetAttr(AttrType.MagicPower);
		}
	}

	public int SourceTeamType
	{
		get
		{
			return this.unitCollider.SourceTeamType;
		}
	}

	public int OriginalTeamType
	{
		get
		{
			return this.unitCollider.OriginalTeamType;
		}
	}

	public float additive_physic_attack_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_physic_attack_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_magic_attack_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_magic_attack_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_real_attack_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_real_attack_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_treatment_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_treatment_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_physic_critprop_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_physic_critprop_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_magic_critprop_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_magic_critprop_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_hitprop_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_hitprop_relative
	{
		get
		{
			return 0f;
		}
	}

	public float additive_dodgeprop_abs
	{
		get
		{
			return 0f;
		}
	}

	public float additive_dodgeprop_relative
	{
		get
		{
			return 0f;
		}
	}

	public float attack_range
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.AttackRange), 0f, 3.40282347E+38f);
		}
	}

	public float warning_range
	{
		get
		{
			return this.GetAttr(AttrType.WarningRange);
		}
	}

	public float fog_range
	{
		get
		{
			return this.GetAttr(AttrType.FogRange);
		}
	}

	public float realSight_range
	{
		get
		{
			return this.GetAttr(AttrType.RealSightRange);
		}
	}

	public float shield
	{
		get
		{
			return this.Clamp(this.GetAttr(AttrType.Shield), 0f, 3.40282347E+38f);
		}
	}

	public int atk_type
	{
		get
		{
			return this.GetData<int>(DataType.AttackType);
		}
		set
		{
			this.SetData<int>(DataType.AttackType, value);
		}
	}

	public float attr_factor
	{
		get
		{
			float? num = new float?(this.GetData<float>(DataType.AttrFactor));
			return (!num.HasValue) ? 1f : num.Value;
		}
		set
		{
			this.SetData<float>(DataType.AttrFactor, value);
		}
	}

	public string[] attacks
	{
		get
		{
			return this.GetData<string[]>(DataType.Attacks);
		}
		set
		{
			this.SetData<string[]>(DataType.Attacks, value);
		}
	}

	public string[] skills
	{
		get
		{
			return this.GetData<string[]>(DataType.Skills);
		}
		set
		{
			this.SetData<string[]>(DataType.Skills, value);
		}
	}

	public string[] equips
	{
		get
		{
			return this.data.GetData<string[]>(DataType.Equips);
		}
		set
		{
			this.data.SetData(DataType.Equips, value);
		}
	}

	public string[] soulstones
	{
		get
		{
			return this.GetData<string[]>(DataType.Soulstones);
		}
		set
		{
			this.SetData<string[]>(DataType.Soulstones, value);
		}
	}

	public float physicDamageAdd
	{
		get
		{
			return this.GetAttr(AttrType.PhysicDamageAdd);
		}
	}

	public float physicDamagePercent
	{
		get
		{
			return this.GetAttr(AttrType.PhysicDamagePercent);
		}
	}

	public float magicDamageAdd
	{
		get
		{
			return this.GetAttr(AttrType.MagicDamageAdd);
		}
	}

	public float magicDamagePercent
	{
		get
		{
			return this.GetAttr(AttrType.MagicDamagePercent);
		}
	}

	public float attackDamageAdd
	{
		get
		{
			return this.GetAttr(AttrType.AttackDamageAdd);
		}
	}

	public float attackDamagePercent
	{
		get
		{
			return this.GetAttr(AttrType.AttackDamagePercent);
		}
	}

	public float physicDamageCut
	{
		get
		{
			return this.GetAttr(AttrType.PhysicDamageCut);
		}
	}

	public float physicDamagePercentCut
	{
		get
		{
			return this.GetAttr(AttrType.PhysicDamagePercentCut);
		}
	}

	public float magicDamageCut
	{
		get
		{
			return this.GetAttr(AttrType.MagicDamageCut);
		}
	}

	public float magicDamagePercentCut
	{
		get
		{
			return this.GetAttr(AttrType.MagicDamagePercentCut);
		}
	}

	public float attackDamageCut
	{
		get
		{
			return this.GetAttr(AttrType.AttackDamageCut);
		}
	}

	public float attackDamagePercentCut
	{
		get
		{
			return this.GetAttr(AttrType.AttackDamagePercentCut);
		}
	}

	public float moFaHuDunCoverProportion
	{
		get
		{
			return this.GetAttr(AttrType.MoFaHuDunCoverProportion);
		}
		set
		{
			this.data.ChangeAttr(AttrType.MoFaHuDunCoverProportion, OpType.Add, value - this.GetAttr(AttrType.MoFaHuDunCoverProportion));
		}
	}

	public float hp_init_p
	{
		get
		{
			return this.m_hp_init_p;
		}
		set
		{
			this.m_hp_init_p = value;
		}
	}

	public float mp_init_p
	{
		get
		{
			return this.m_mp_init_p;
		}
		set
		{
			this.m_mp_init_p = value;
		}
	}

	public int aiType
	{
		get
		{
			return this.GetData<int>(DataType.AIType);
		}
		set
		{
			this.SetData<int>(DataType.AIType, value);
		}
	}

	public UnitsLifeTime unitsLifeTime
	{
		get
		{
			return this.m_unitsLifeTime;
		}
	}

	public UnitControlType unitControlType
	{
		get
		{
			return this._unitControlType;
		}
		set
		{
			this._unitControlType = value;
			this.StopMove();
			if (this.netController != null)
			{
				bool reportMode = this.unitControlType == UnitControlType.PvpAIControl || this.unitControlType == UnitControlType.PvpMyControl;
				this.netController.SetReportMode(reportMode);
			}
		}
	}

	protected bool isPvpControl
	{
		get
		{
			return this._unitControlType == UnitControlType.PvpAIControl || this._unitControlType == UnitControlType.PvpMyControl || this._unitControlType == UnitControlType.PvpNetControl;
		}
	}

	public bool IsReplayControl
	{
		get
		{
			return this.unitControlType == UnitControlType.Replay;
		}
	}

	public virtual bool IsMaster
	{
		get
		{
			return this._unitControlType != UnitControlType.PvpNetControl;
		}
	}

	public bool IsFree
	{
		get
		{
			return this._unitControlType == UnitControlType.Free || this._unitControlType == UnitControlType.None || this._unitControlType == UnitControlType.Replay;
		}
	}

	public int controllerType
	{
		get
		{
			return this.GetData<int>(DataType.ControllerType);
		}
		set
		{
			int num = this.GetData<int>(DataType.ControllerType);
			if (num == 0 || num != value)
			{
				if (value == 1)
				{
					(this.mBtlTouchController = this.AddUnitComponent<BattleTouchController>()).OnInit();
					(this.mManualController = this.AddUnitComponent<ManualController>()).OnInit();
				}
				this.SetData<int>(DataType.ControllerType, value);
			}
		}
	}

	public Team team
	{
		get
		{
			return TeamManager.teams[this.teamType];
		}
	}

	public TeamType TeamType
	{
		get
		{
			return (TeamType)this.teamType;
		}
	}

	public int teamType
	{
		get
		{
			return this.GetData<int>(DataType.TeamType);
		}
		set
		{
			if (value >= 0 && value < TeamManager.teams.Length)
			{
				this.SetData<int>(DataType.TeamType, value);
			}
			else
			{
				ClientLogger.Error("invalid teamtype: " + value);
				this.SetData<int>(DataType.TeamType, 0);
			}
		}
	}

	public float height
	{
		get
		{
			return this.GetHeight();
		}
	}

	public Vector3 direction
	{
		get
		{
			return this.moveDirection;
		}
		set
		{
			this.moveDirection = value;
		}
	}

	public Vector3 spwan_pos
	{
		get
		{
			return this.GetData<Vector3>(DataType.SpawnPos);
		}
		set
		{
			this.SetData<Vector3>(DataType.SpawnPos, value);
		}
	}

	public Quaternion spwan_rotation
	{
		get
		{
			return this.GetData<Quaternion>(DataType.SpawnRotation);
		}
		set
		{
			this.SetData<Quaternion>(DataType.SpawnRotation, value);
		}
	}

	public bool NeedRecycle
	{
		get;
		set;
	}

	public int StartCount
	{
		get
		{
			return this.startCount;
		}
		set
		{
			this.startCount = value;
		}
	}

	public Units ParentUnit
	{
		get
		{
			return this.parentUnit;
		}
		set
		{
			this.parentUnit = value;
		}
	}

	public Units LastHurtHero
	{
		get
		{
			if (Time.time - this.hurtTime >= 5f)
			{
				this.m_lastHurtHero = null;
			}
			return this.m_lastHurtHero;
		}
		protected set
		{
			this.m_lastHurtHero = value;
			this.hurtTime = Time.time;
		}
	}

	public List<string> AssistantList
	{
		get
		{
			this.m_assistantList.Clear();
			float time = Time.time;
			foreach (Units current in this.m_assistantDict.Keys)
			{
				if (time - this.m_assistantDict[current] < 5f)
				{
					if (current.unique_id != this.LastHurtHero.unique_id && !Units.IsSameTeam(current, this) && current.unique_id != this.unique_id)
					{
						this.m_assistantList.Add(current.npc_id);
						current.assistantNum++;
					}
				}
			}
			return this.m_assistantList;
		}
	}

	public bool IsWildMonster
	{
		get
		{
			if (this.data == null)
			{
				return false;
			}
			int dataInt = this.data.GetDataInt(DataType.ItemType);
			return dataInt == 3 || dataInt == 7 || dataInt == 9;
		}
	}

	public ImmunityManager ImmunityManager
	{
		get;
		set;
	}

	public NetController netController
	{
		get;
		set;
	}

	protected CoroutineManager mCoroutineManager
	{
		get
		{
			return this.m_CoroutineManager;
		}
	}

	public HUDText mText
	{
		get
		{
			if (this.surface == null)
			{
				return null;
			}
			return this.surface.mText;
		}
	}

	public UIBloodBar mHpBar
	{
		get
		{
			if (this.surface == null)
			{
				return null;
			}
			return this.surface.mHpBar;
		}
	}

	public ResourceHandleWrapper<SkillPointer> mSkillPointer
	{
		get
		{
			if (this.surface != null)
			{
				return this.surface.mSkillPointer;
			}
			return null;
		}
		set
		{
			if (this.surface.mSkillPointer != null)
			{
				this.surface.mSkillPointer.Release();
			}
			this.surface.mSkillPointer = value;
		}
	}

	public CapsuleCollider mCollider
	{
		get
		{
			return this.m_collider;
		}
		set
		{
			this.m_collider = value;
		}
	}

	public Mecanim mMecanim
	{
		get
		{
			return this.m_mecanim;
		}
		set
		{
			this.m_mecanim = value;
		}
	}

	public SkinnedMeshRenderer mRender
	{
		get
		{
			return this.m_render;
		}
		set
		{
			this.m_render = value;
		}
	}

	public Renderer[] mRenders
	{
		get
		{
			return this.m_renders;
		}
		set
		{
			this.m_renders = value;
		}
	}

	public CharacterEffect mCharacterEffect
	{
		get
		{
			return this.m_characterEffect;
		}
		set
		{
			this.m_characterEffect = value;
		}
	}

	public Transform mModel
	{
		get
		{
			if (this.m_model == null)
			{
				this.m_model = base.mTransform.Find("Model");
			}
			return this.m_model;
		}
		set
		{
			this.m_model = value;
		}
	}

	public GameObject mObstacleObj
	{
		get
		{
			return this.m_obstacleObj;
		}
		set
		{
			this.m_obstacleObj = value;
		}
	}

	public CapsuleCollider mObstacleCollider
	{
		get
		{
			return this.m_obstacleCollider;
		}
		set
		{
			this.m_obstacleCollider = value;
		}
	}

	public GameObject mSelectObj
	{
		get
		{
			return this.m_selectObj;
		}
		set
		{
			this.m_selectObj = value;
		}
	}

	public GameObject mMemory
	{
		get
		{
			return this.m_memory;
		}
		set
		{
			this.m_memory = value;
		}
	}

	public SphereCollider mVechileCollider
	{
		get
		{
			return this.m_vechileCollider;
		}
		set
		{
			this.m_vechileCollider = value;
		}
	}

	public GameObject mAIObject
	{
		get
		{
			return this.m_aiObj;
		}
		set
		{
			this.m_aiObj = value;
		}
	}

	public bool LockInputState
	{
		get
		{
			return this.isLockInput;
		}
	}

	public bool isMoving
	{
		get
		{
			return this.moveController != null && this.moveController.isMoving;
		}
	}

	public bool isLive
	{
		get
		{
			return this.m_isLive;
		}
		set
		{
			this.m_isLive = value;
			if (!this.m_isLive)
			{
				this.data.SetHp(0f);
				this.data.SetMp(0f);
				this.RemoveMiniMapIcon();
			}
			else if (this.hp <= 0f)
			{
				this.data.SetHp(this.hp_max);
				this.data.SetMp(this.mp_max);
			}
		}
	}

	public new bool isStart
	{
		get
		{
			return this.m_isStart;
		}
		set
		{
			this.m_isStart = value;
		}
	}

	public bool CanMoveAnim
	{
		get
		{
			return this.isLive && this.crickTime != null && this.crickTime.canMoveAnim;
		}
	}

	public bool CanMove
	{
		get
		{
			return this.isLive && (this.highEffManager == null || !this.DingShen.IsInState) && (this.isEnableActionHighEff && this.isCanMove && (this.crickTime == null || this.crickTime.canMove));
		}
	}

	public bool CanRoatate
	{
		get
		{
			return this.isLive && this.canRoatate && (this.crickTime == null || this.crickTime.canRotate) && !this.isBuilding;
		}
	}

	public bool CanAction
	{
		get
		{
			return this.isLive && this.isEnableActionHighEff && this.isCanAction && (this.crickTime == null || this.crickTime.canAction);
		}
	}

	public bool CanSkill
	{
		get
		{
			return this.isCanSkill && !this.Temptation.IsInState && !this.MeiHuo.IsInState && !this.ChaoFeng.IsInState && this.CanAction && (this.crickTime == null || this.crickTime.canSkill);
		}
	}

	public bool CanAttack
	{
		get
		{
			if (this.isPlayer)
			{
			}
			return this.IsInAttackTimeLengh && this.CanAction && !this.Temptation.IsInState && this.isCanAttack && (this.crickTime == null || this.crickTime.canAttack);
		}
	}

	public bool IsInAttackTimeLengh
	{
		get
		{
			return this.crickTime == null || this.crickTime.isInAttackTimeLengh;
		}
	}

	public bool IsAISupported
	{
		get
		{
			return this.unitControlType != UnitControlType.PvpNetControl && this.unitControlType != UnitControlType.Replay && !this.Sleeping.IsInState;
		}
	}

	public bool CanAIControl
	{
		get
		{
			return this.aiManager != null && (this.IsAISupported && this.isLive && this.isEnableActionHighEff && this.isCanAIControl) && (this.crickTime == null || this.crickTime.canAI);
		}
	}

	public bool isSelected
	{
		get
		{
			return this.m_isSelected;
		}
		set
		{
			this.m_isSelected = value;
		}
	}

	public bool isCreep
	{
		get
		{
			return Singleton<CreepSpawner>.Instance.IsCreep(this);
		}
	}

	public bool IsSummonedCreature
	{
		get
		{
			return this.UnitType == UnitType.SummonMonster;
		}
	}

	public bool IsPet
	{
		get
		{
			return this.UnitType == UnitType.Pet;
		}
	}

	public bool isCharacter
	{
		get
		{
			return this.isMonster || this.isHero || this.isBuilding;
		}
	}

	public bool isMovingEntity
	{
		get
		{
			return this.isMonster || this.isPlayer || this.isHero;
		}
	}

	public bool isMyTeam
	{
		get
		{
			return TeamManager.MyTeam == (TeamType)this.teamType;
		}
	}

	public bool isEnemy
	{
		get
		{
			return TeamManager.IsEnemy(this);
		}
	}

	public bool EnableActionHighEff
	{
		get
		{
			return this.isEnableActionHighEff;
		}
		set
		{
			this.isEnableActionHighEff = value;
		}
	}

	public bool IsFrozenAnimation
	{
		get
		{
			return this.isFrozenAnimtion;
		}
	}

	public bool IsLockCharaEffect
	{
		get
		{
			return this.isLockCharaEffect;
		}
	}

	public bool IsLockCharaControl
	{
		get
		{
			return this.isLockCharaControl;
		}
	}

	public bool IsLockAnimState
	{
		get
		{
			return this.isLockAnimState;
		}
	}

	public bool IsForceClickGroud
	{
		get
		{
			return this.isForceClickGroud;
		}
	}

	public bool IsHideEffect
	{
		get
		{
			return this.isHideEffect || this.m_nVisibleState == 2;
		}
	}

	public bool IsRebirthing
	{
		get
		{
			return this.isRebirthing;
		}
		set
		{
			this.isRebirthing = value;
		}
	}

	public bool IsFullHp
	{
		get
		{
			return this.hp >= this.hp_max;
		}
	}

	public bool IsFullMp
	{
		get
		{
			return this.mp >= this.mp_max;
		}
	}

	public int state
	{
		get
		{
			this.m_state = 0;
			for (int i = 0; i < this.states.Length; i++)
			{
				if (this.states[i] != null && this.states[i].IsInState)
				{
					this.m_state &= ~(1 << i);
				}
				else
				{
					this.m_state |= 1 << i;
				}
			}
			return this.m_state;
		}
	}

	public CharaState[] states
	{
		get
		{
			if (this.m_states == null)
			{
				this.m_states = new CharaState[33];
				for (int i = 0; i < this.m_states.Length; i++)
				{
					this.m_states[i] = new CharaState();
				}
			}
			return this.m_states;
		}
	}

	public CharaState YunXuan
	{
		get
		{
			return this.states[1];
		}
	}

	public CharaState ChengMo
	{
		get
		{
			return this.states[2];
		}
	}

	public CharaState MeiHuo
	{
		get
		{
			return this.states[3];
		}
	}

	public CharaState BoZang
	{
		get
		{
			return this.states[4];
		}
	}

	public CharaState ZhiMang
	{
		get
		{
			return this.states[5];
		}
	}

	public CharaState ChaoFeng
	{
		get
		{
			return this.states[6];
		}
	}

	public CharaState KongJu
	{
		get
		{
			return this.states[7];
		}
	}

	public CharaState JiFei
	{
		get
		{
			return this.states[8];
		}
	}

	public CharaState WeiYi
	{
		get
		{
			return this.states[9];
		}
	}

	public CharaState DingShen
	{
		get
		{
			return this.states[10];
		}
	}

	public CharaState GuangHuan
	{
		get
		{
			return this.states[11];
		}
	}

	public CharaState ShiHua
	{
		get
		{
			return this.states[12];
		}
	}

	public CharaState JiTui
	{
		get
		{
			return this.states[13];
		}
	}

	public CharaState FuHuo
	{
		get
		{
			return this.states[15];
		}
	}

	public CharaState BingDong
	{
		get
		{
			return this.states[16];
		}
	}

	public CharaState ZhangDa
	{
		get
		{
			return this.states[17];
		}
	}

	public CharaState HunLuan
	{
		get
		{
			return this.states[18];
		}
	}

	public CharaState BianShen
	{
		get
		{
			return this.states[19];
		}
	}

	public CharaState QianYin
	{
		get
		{
			return this.states[14];
		}
	}

	public CharaState Sleeping
	{
		get
		{
			return this.states[20];
		}
	}

	public CharaState Temptation
	{
		get
		{
			return this.states[21];
		}
	}

	public CharaState Imprisonment
	{
		get
		{
			return this.states[28];
		}
	}

	public CharaState CanRebirth
	{
		get
		{
			return this.states[30];
		}
	}

	public CharaState Growth
	{
		get
		{
			return this.states[31];
		}
	}

	public CharaState WuDi
	{
		get
		{
			return this.states[22];
		}
	}

	public CharaState MoFaHuDun
	{
		get
		{
			return this.states[24];
		}
	}

	public CharaState HuiGuangFanZhao
	{
		get
		{
			return this.states[25];
		}
	}

	public CharaState Invisible
	{
		get
		{
			return this.states[26];
		}
	}

	public CharaState grassInvisible
	{
		get
		{
			return this.states[27];
		}
	}

	public CharaState Sprint
	{
		get
		{
			return this.states[29];
		}
	}

	public string[] StartActions
	{
		get;
		set;
	}

	public string[] HitActions
	{
		get;
		set;
	}

	public string[] ReadyActions
	{
		get;
		set;
	}

	public PlayAnimAction CurAnimationAction
	{
		get;
		set;
	}

	public Skill currentSkillOrAttack
	{
		get
		{
			if (this.currentSkill != null)
			{
				return this.currentSkill;
			}
			return this.currentAttack;
		}
	}

	public Skill currentSkill
	{
		get
		{
			if (this.atkController != null)
			{
				return this.atkController.currSkill;
			}
			return null;
		}
	}

	public Skill currentAttack
	{
		get
		{
			if (this.atkController != null)
			{
				return this.atkController.CurAttack;
			}
			return null;
		}
	}

	public bool IsInAttack
	{
		get
		{
			return this.currentAttack != null && this.currentAttack.IsCastting;
		}
	}

	public bool IsInReadyAttack
	{
		get
		{
			return this.currentAttack != null && this.currentAttack.IsCastting && this.currentAttack.CastPhase == SkillCastPhase.Cast_Before;
		}
	}

	public AIManager mAIManager
	{
		get
		{
			return this.aiManager;
		}
	}

	public Units FakeAttackTarget
	{
		get
		{
			if (this.m_fakeAttackTarget && this.m_fakeAttackTarget.isLive)
			{
				return this.m_fakeAttackTarget;
			}
			return null;
		}
		set
		{
			this.m_fakeAttackTarget = value;
		}
	}

	public bool MirrorState
	{
		get
		{
			return this.mirrorState;
		}
	}

	public bool CanBeSelected
	{
		get
		{
			return this.isPlayer || (!this.Sleeping.IsInState && (this.m_nVisibleState < 2 || this.isBuilding) && this.IsConfigedAttackable() && !this.IsRebirthing);
		}
	}

	public bool CanBeSelectedManually
	{
		get
		{
			return this.isVisible && this.isLive && this.isVisibleInCamera && !this.isItem && !this.isBuffItem && this.m_SelectRadius > 0f && this.UnitType != UnitType.Pet && this.UnitType != UnitType.LabisiUnit;
		}
	}

	public bool CanSkillSelected
	{
		get
		{
			return !this.Sleeping.IsInState;
		}
	}

	public bool isLocalUnit
	{
		get
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			return !(player == null) && TeamManager.CheckTeam(base.gameObject, player.teamType);
		}
	}

	public void SetDebugDLog(bool setOn)
	{
		this.DebugDLog = setOn;
		string cheatMsg = "DebugUnit " + this.unique_id + ((!this.DebugDLog) ? " 0" : " 1");
		if (NetWorkHelper.Instance.client != null)
		{
			NetWorkHelper.Instance.client.SendPvpServerMsg(PvpCode.C2P_GMCheat, new object[]
			{
				SerializeHelper.Serialize<CheatInfo>(new CheatInfo
				{
					cheatMsg = cheatMsg
				})
			});
		}
	}

	protected void SetUnitsLifeTime(UnitsLifeTime v)
	{
		this.m_unitsLifeTime = v;
	}

	public void playVoice(string voice)
	{
		if (this.m_hv != null)
		{
			this.m_hv.PlayVoice(voice);
		}
	}

	public void SetParentUnit(Units parent)
	{
		this.parentUnit = parent;
	}

	public static bool IsSameTeam(Units u, Units v)
	{
		if (u == null || v == null)
		{
			return u == v;
		}
		return u.teamType == v.teamType;
	}

	public void GetHeroKillHelper(Units inKiller, List<Units> outHelpers)
	{
		if (outHelpers == null)
		{
			return;
		}
		outHelpers.Clear();
		if (inKiller == null)
		{
			return;
		}
		Dictionary<Units, float>.KeyCollection keys = this.m_assistantDict.Keys;
		foreach (Units current in keys)
		{
			if (current != null && current.unique_id != inKiller.unique_id)
			{
				outHelpers.Add(current);
			}
		}
	}

	public void ResetUnitRenderer(bool force = false)
	{
	}

	public AttackController getAtkController()
	{
		return this.atkController;
	}

	public void ChangeControllerMode()
	{
		ManualController unitComponent = this.GetUnitComponent<ManualController>();
		if (unitComponent == null)
		{
			return;
		}
		if (this.isCrazyMode)
		{
			unitComponent.SetNormalModeNew(false);
		}
		else
		{
			unitComponent.SetCrazyMode(false);
		}
		this.isCrazyMode = !this.isCrazyMode;
	}

	public void SetCrazyMode()
	{
		ManualController unitComponent = this.GetUnitComponent<ManualController>();
		if (unitComponent == null)
		{
			return;
		}
		this.isCrazyMode = true;
		unitComponent.SetCrazyMode(false);
	}

	public void SetNormalMode()
	{
		ManualController unitComponent = this.GetUnitComponent<ManualController>();
		if (unitComponent == null)
		{
			return;
		}
		this.isCrazyMode = false;
		unitComponent.SetNormalModeNew(false);
	}

	public void AddNormalControllerNew(bool isInit)
	{
		if (this.mCmdRunningController == null)
		{
			this.mCmdRunningController = this.AddUnitComponent<CmdRunningController>();
		}
		if (this.mCmdCacheController == null)
		{
			this.mCmdCacheController = this.AddUnitComponent<CmdCacheController>();
		}
		this.mCmdRunningController.m_cmdCacheController = this.mCmdCacheController;
		if (this.mShowSkillPointerCrazy == null)
		{
			this.mShowSkillPointerNormal = this.AddUnitComponent<ShowSkillPointerNormal>();
		}
		if (!isInit)
		{
			this.mCmdRunningController.OnInit();
			this.mCmdCacheController.OnInit();
			this.mShowSkillPointerNormal.OnInit();
		}
	}

	public void RemoveNormalController()
	{
		this.RemoveUnitComponent<CmdRunningController>();
		this.mCmdRunningController = null;
		this.RemoveUnitComponent<CmdCacheController>();
		this.mCmdCacheController = null;
		this.RemoveUnitComponent<ShowSkillPointerNormal>();
		this.mShowSkillPointerNormal = null;
	}

	public void AddCrazyController(bool isInit)
	{
		if (this.mCmdRunningController == null)
		{
			this.mCmdRunningController = this.AddUnitComponent<CmdRunningController>();
		}
		if (this.mCmdCacheController == null)
		{
			this.mCmdCacheController = this.AddUnitComponent<CmdCacheController>();
		}
		this.mCmdRunningController.m_cmdCacheController = this.mCmdCacheController;
		if (this.mShowSkillPointerCrazy == null)
		{
			this.mShowSkillPointerCrazy = this.AddUnitComponent<ShowSkillPointerCrazy>();
		}
		if (!isInit)
		{
			this.mCmdRunningController.OnInit();
			this.mCmdCacheController.OnInit();
			this.mShowSkillPointerCrazy.OnInit();
		}
	}

	public void RemoveCrazyController()
	{
		this.RemoveUnitComponent<CmdRunningController>();
		this.mCmdRunningController = null;
		this.RemoveUnitComponent<CmdCacheController>();
		this.mCmdCacheController = null;
		this.RemoveUnitComponent<ShowSkillPointerCrazy>();
		this.mShowSkillPointerCrazy = null;
	}

	public void AddToolComponent()
	{
		if (this.cameraVisibleTool != null)
		{
			return;
		}
		GameObject original = Resources.Load("Prefab/Actors/Monsters/CameraVisibleCube") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		gameObject.transform.parent = base.mTransform;
		gameObject.transform.position = new Vector3(base.mTransform.position.z, base.mTransform.position.y + 5f, base.mTransform.position.z);
		this.cameraVisibleTool = gameObject;
	}

	public void RemoveToolComponent()
	{
		if (this.cameraVisibleTool != null)
		{
			UnityEngine.Object.Destroy(this.cameraVisibleTool);
		}
	}

	protected virtual void OnVisibleInCamera()
	{
	}

	protected void UpdateCameraVisible()
	{
		this.updateCameraVisibleIdx++;
		if (this.updateCameraVisibleIdx % 6 != this.unique_id % 6)
		{
			return;
		}
		Vector3 vector = GameManager.Instance.mainCamera.WorldToScreenPoint(base.mTransform.position);
		if (vector.y > (float)(Screen.height * 12 / 10) || vector.x > (float)(Screen.width * 12 / 10) || vector.x < (float)(0 - Screen.width * 2 / 10) || vector.y < (float)(0 - Screen.height * 4 / 10))
		{
			if (this.isVisibleInCamera)
			{
				this.isVisibleInCamera = false;
			}
		}
		else if (!this.isVisibleInCamera)
		{
			this.isVisibleInCamera = true;
			this.OnVisibleInCamera();
		}
	}

	public bool CanClickSkillUI(string skillID)
	{
		return !this.MeiHuo.IsInState && !this.ChaoFeng.IsInState && !this.KongJu.IsInState;
	}

	public bool CanManualControl()
	{
		return this.canManualControl;
	}

	public void SetManualControl(bool flag)
	{
		this.canManualControl = flag;
	}

	public void AddState(StateType state)
	{
		this.m_states[(int)state].Add();
	}

	public void RemoveState(StateType state)
	{
		this.m_states[(int)state].Remove();
	}

	protected void ClearAllCharaState()
	{
		this.SetCanAction(true);
		this.ResetInterrupt(true, SkillInterruptType.None);
		for (int i = 0; i < this.states.Length; i++)
		{
			if (this.states[i] != null)
			{
				this.states[i].Reset();
			}
		}
		this.isEnableActionHighEff = true;
		this.isRebirthing = false;
		this.SetLockCharaEffect(false);
		this.SetLockCharaControl(false);
		this.SetLockAnimState(false);
		this.SetForceClickGroundState(false);
		this.ResetAnimState();
		this.EnableAllRenders(true);
	}

	public T GetData<T>(DataType type)
	{
		if (this.data != null)
		{
			return this.data.GetData<T>(type);
		}
		return default(T);
	}

	public void SetData<T>(DataType type, object value)
	{
		if (this.data != null)
		{
			this.data.SetData(type, (T)((object)value));
		}
	}

	public float GetAttr(AttrType type)
	{
		if (this.data != null)
		{
			return this.data.GetAttr(type);
		}
		return 0f;
	}

	public float ChangeAttr(AttrType attrType, OpType opType, float value)
	{
		if (this.data != null)
		{
			return this.data.ChangeAttr(attrType, opType, value);
		}
		return 0f;
	}

	public void ChangeAttr(List<DataInfo> attrType)
	{
		foreach (DataInfo current in attrType)
		{
			if (current.percent)
			{
				this.ChangeAttr(current.key, OpType.Mul, current.value);
			}
			else
			{
				this.ChangeAttr(current.key, OpType.Add, current.value);
			}
		}
	}

	public void SetHp(float value)
	{
		this.data.SetHp(value);
	}

	public void SetMaxHp(float inVal)
	{
		this.data.SetMaxHp(inVal);
	}

	public void SetMp(float value)
	{
		this.data.SetMp(value);
	}

	public void SetBloodBarStyle()
	{
		if (this.surface != null)
		{
			this.surface.SetBloodBar(this.isMyTeam);
		}
	}

	public float GetHeight()
	{
		if (this.unitCollider != null)
		{
			return this.unitCollider.GetHeight();
		}
		return 0f;
	}

	public Vector3 GetBonePos(BoneAnchorType type)
	{
		if (this.unitCollider != null)
		{
			return this.unitCollider.GetBone(type);
		}
		return Vector3.zero;
	}

	public Vector3 GetCenter()
	{
		if (this.unitCollider != null)
		{
			return this.unitCollider.GetCenter();
		}
		return base.transform.position;
	}

	public Vector3 GetTop()
	{
		if (this.unitCollider != null)
		{
			return this.unitCollider.GetTop();
		}
		return base.transform.position;
	}

	public Vector3 GetFeet()
	{
		if (this.unitCollider != null)
		{
			return this.unitCollider.GetFeet();
		}
		return base.transform.position;
	}

	public void GetBone(int bone_type, out Transform bone, out Vector3 offset)
	{
		if (this.unitCollider != null)
		{
			this.unitCollider.GetBone(bone_type, out bone, out offset);
		}
		else
		{
			bone = null;
			offset = Vector3.zero;
		}
	}

	public void ChangeTeam(TeamType type)
	{
		if (this.unitCollider != null)
		{
			this.unitCollider.SetTeam(type);
		}
	}

	public void ChangeLayer(string newLayer)
	{
		if (this.unitCollider != null)
		{
			this.unitCollider.ChangeLayer(newLayer);
		}
	}

	public void RevertLayer()
	{
		if (this.unitCollider != null)
		{
			this.unitCollider.RevertLayer();
		}
	}

	public void Move(Vector3 pos)
	{
	}

	public void RevertTeam()
	{
		if (this.unitCollider != null)
		{
			this.unitCollider.RevertTeam();
		}
	}

	public float GetAttackRange(Units target = null)
	{
		float num = this.m_Radius * base.transform.localScale.x + this.attack_range;
		if (target != null)
		{
			num += target.m_Radius * target.transform.localScale.x;
		}
		return num;
	}

	public void SetPlayer(bool isplayer)
	{
		if (isplayer)
		{
			this.tag = "Player";
			this.controllerType = 1;
			this.surface.MarkAsPlayer(true);
		}
		else
		{
			this.tag = "Hero";
			this.controllerType = 0;
			this.surface.MarkAsPlayer(false);
		}
	}

	public void OnSkillEnd(string skill_id)
	{
		TriggerParamSkillControl triggerParamSkillControl = new TriggerParamSkillControl();
		triggerParamSkillControl.EventID = 2;
		triggerParamSkillControl.Trigger = this;
		triggerParamSkillControl.SkillID = skill_id;
		TriggerManager2.Instance.Trigger2(triggerParamSkillControl);
	}

	public void OnSkill(string skill_id)
	{
		TriggerParamSkillControl triggerParamSkillControl = new TriggerParamSkillControl();
		triggerParamSkillControl.EventID = 1;
		triggerParamSkillControl.Trigger = this;
		triggerParamSkillControl.SkillID = skill_id;
		TriggerManager2.Instance.Trigger2(triggerParamSkillControl);
	}

	public Task TurnToTarget(Vector3? point, bool isFast = true, bool isForce = false, float limitTime = 0f)
	{
		if (this.moveController != null)
		{
			return this.moveController.TurnToTarget(point, isFast, isForce, limitTime);
		}
		return null;
	}

	public NavgationAgent GetNavgationAgent()
	{
		if (this.moveController != null)
		{
			return this.moveController.navAgent;
		}
		return null;
	}

	public int GetTagsChange()
	{
		if (this.moveController != null)
		{
			return this.moveController.GetTagsChange();
		}
		return 0;
	}

	public void SetTagsChange(int tag)
	{
		if (this.moveController != null)
		{
			this.moveController.SetTagsChange(tag);
		}
	}

	public void ResetTagsChange()
	{
		if (this.moveController != null)
		{
			this.moveController.ResetTagsChange();
		}
	}

	public bool isReachedTarget()
	{
		return this.moveController != null && this.moveController.isReachedTarget();
	}

	public void MoveToTarget(Units target)
	{
		if (this.moveController != null)
		{
			this.moveController.MoveToTarget(target);
		}
	}

	public void MoveToPoint(Vector3? point, bool forceSearch = false)
	{
		if (this.moveController != null)
		{
			this.moveController.MoveToPoint(point, -1f, forceSearch);
		}
	}

	public void MoveToPoint(Vector3? point, float distance)
	{
		if (this.moveController != null)
		{
			this.moveController.MoveToPoint(point, distance, false);
		}
	}

	public bool ContinueMove()
	{
		return this.moveController != null && this.moveController.ContinueMove();
	}

	public void StopMove()
	{
		if (this.moveController != null)
		{
			this.moveController.StopMove();
		}
	}

	public void StopMoveForSkill()
	{
		if (this.moveController != null)
		{
			this.moveController.StopMoveForSkill();
		}
	}

	public void AddSearchingCallback(Callback OnStartSearching, Callback OnStopSearching)
	{
		if (this.moveController != null)
		{
			this.moveController.AddSearchingCallback(OnStartSearching, OnStopSearching);
		}
	}

	public void RemoveSearchingCallback(Callback OnStartSearching, Callback OnStopSearching)
	{
		if (this.moveController != null)
		{
			this.moveController.RemoveSearchingCallback(OnStartSearching, OnStopSearching);
		}
	}

	public void PlayAnim(AnimationType type, bool state, int index = 0, bool mustLive = true, bool force = false)
	{
		if (this.animController != null)
		{
			this.animController.PlayAnim(type, state, index, mustLive, force);
		}
	}

	public void PlayAnim(string name, int idx = 0)
	{
		if (this.animController != null)
		{
			this.animController.PlayAnim(name, idx);
		}
	}

	public void SetAttackAnimSpeed(Skill skill)
	{
		if (this.animController != null)
		{
			this.animController.SetAttackAnimSpeed(skill);
		}
	}

	public void SetRawAnimSpeed(float speed, bool animatLock)
	{
		if (this.animController != null)
		{
			this.animController.SetRawAnimSpeed(speed, animatLock);
		}
	}

	public void RevertAnimSpeed()
	{
		if (this.animController != null)
		{
			this.animController.RevertAnimSpeed();
		}
	}

	public void ResetAnimState()
	{
		if (this.animController != null)
		{
			this.animController.ResetAnimState();
		}
	}

	public void ResetActionState()
	{
		if (this.animController != null)
		{
			this.animController.ResetActionState();
		}
	}

	public bool IsAnimInTransition()
	{
		return this.animController != null && this.animController.IsInTransition();
	}

	public void jumpFont(JumpFontType type, string value = "", Units attacker = null, bool mustShow = false)
	{
		if (!this.isLive)
		{
			return;
		}
		if (type == JumpFontType.None)
		{
			return;
		}
		string text = AttrManager.GetName((int)type);
		if (value != string.Empty)
		{
			text = text + " " + value;
		}
		Color color = AttrManager.GetColor((int)type, value);
		this.jumpFontInternal(text, color, attacker, mustShow);
	}

	public void jumpFontValue(AttrType prop, float propValue, Units attacker = null, bool isCrit = false, bool isMustShow = false, int extraParam = 0, int damageValueType = 0)
	{
		if (!this.isLive)
		{
			return;
		}
		if (!BattleCameraMgr.Instance.CanBattleCameraSee(base.transform.position))
		{
			return;
		}
		if (propValue == 0f)
		{
			return;
		}
		int num = (int)propValue;
		if (prop == AttrType.Hp && this.hp > this.hp_max)
		{
			return;
		}
		if (prop == AttrType.Mp && this.mp > this.mp_max)
		{
			return;
		}
		string str = (num <= 0) ? "- " : "+ ";
		if (prop == AttrType.Shield)
		{
			str = ((num <= 0) ? "吸收- " : "破盾 ");
		}
		JumpFontType jumpFontType = JumpFontType.None;
		if (prop == AttrType.Hp)
		{
			if (isCrit)
			{
				jumpFontType = ((num <= 0) ? JumpFontType.Critical : JumpFontType.RevertHp);
			}
			else
			{
				jumpFontType = ((num <= 0) ? JumpFontType.Hp : JumpFontType.RevertHp);
			}
			if (jumpFontType == JumpFontType.Hp)
			{
				if (damageValueType == 1)
				{
					jumpFontType = JumpFontType.PhysicDamage;
				}
				else if (damageValueType == 2)
				{
					jumpFontType = JumpFontType.MagicDamage;
				}
				else if (damageValueType == 3)
				{
					jumpFontType = JumpFontType.RealDamage;
				}
			}
		}
		else if (prop == AttrType.Mp)
		{
			jumpFontType = ((num <= 0) ? JumpFontType.Mp : JumpFontType.RevertMp);
		}
		else if (prop == AttrType.Shield)
		{
			jumpFontType = JumpFontType.Shield;
		}
		if (num != -2147483648)
		{
			if (extraParam == 0)
			{
				this.jumpFont(jumpFontType, str + Mathf.Abs(Mathf.Clamp(num, -2147483648, 2147483647)).ToString(), attacker, isMustShow);
			}
			else if (extraParam == 1)
			{
				if (this.CanJumpFont(attacker, isMustShow))
				{
					ActionManager.JumpFontSingle(new JumpFontInfo
					{
						type = (int)jumpFontType,
						text = str + Mathf.Abs(Mathf.Clamp(num, -2147483648, 2147483647)).ToString(),
						unitId = this.unique_id
					}, this, true, true);
				}
			}
			else if (extraParam == 2)
			{
				this.jumpFont(jumpFontType, str + Mathf.Abs(Mathf.Clamp(num, -2147483648, 2147483647)).ToString(), attacker, isMustShow);
				if (attacker != null)
				{
					PvpEvent.SendJumFontEvent(new JumpFontInfo
					{
						type = (int)jumpFontType,
						text = str + Mathf.Abs(Mathf.Clamp(num, -2147483648, 2147483647)).ToString(),
						unitId = this.unique_id,
						attackerId = attacker.unique_id
					});
				}
			}
		}
	}

	private void jumpFontInternal(string text, Color color, Units attacker, bool mustShow = false)
	{
		if (this.CanJumpFont(attacker, mustShow))
		{
			this.JumpFont(text, color);
		}
	}

	public void JumpFont(string text, Color color)
	{
		if (this.surface != null)
		{
			this.surface.jumpFont(text, color);
		}
	}

	public void JumpGoldFont(int gold, Units attackerUnit = null)
	{
		if (attackerUnit != null && attackerUnit.isPlayer)
		{
			this.JumpFont("+" + gold + "g", HUDText.goldColor);
		}
	}

	public bool CanJumpFont(Units attacker, bool mustShow = false)
	{
		bool result;
		if (!Singleton<PvpManager>.Instance.IsInPvp)
		{
			result = ((attacker != null && (attacker.isPlayer || (attacker.ParentUnit != null && attacker.ParentUnit.isPlayer))) || this.isPlayer || mustShow);
		}
		else
		{
			result = ((attacker != null && (attacker.isPlayer || (attacker.ParentUnit != null && attacker.ParentUnit.isPlayer))) || this.isPlayer || mustShow);
		}
		return result;
	}

	public void ShowDebuffIcon(bool isShow, int type)
	{
		if (this.isHero)
		{
			if (isShow)
			{
				if (!this.isVisible || this.IsHideEffect || this.m_nVisibleState >= 2)
				{
					return;
				}
				string name = AttrManager.GetName(type);
				if (this.mHpBar != null)
				{
					this.mHpBar.SetDebuffIcon(true, "被" + name);
				}
				if (this.isPlayer && type != 118)
				{
					Singleton<SkillView>.Instance.SetForbidMask(true, name + "中");
				}
			}
			else
			{
				if (this.mHpBar != null)
				{
					this.mHpBar.SetDebuffIcon(false, string.Empty);
				}
				if (this.isPlayer && type != 118)
				{
					Singleton<SkillView>.Instance.SetForbidMask(false, string.Empty);
				}
			}
			if (this.isPlayer)
			{
				Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
			}
		}
	}

	public void ShowDebuffText(bool isShow, int type)
	{
		if (this.isHero && type == 131)
		{
			if (isShow)
			{
				if (!this.isVisible || this.IsHideEffect || this.m_nVisibleState >= 2)
				{
					return;
				}
				string name = AttrManager.GetName(type);
				if (this.mHpBar != null)
				{
					this.mHpBar.SetDebuffIcon(true, "被" + name);
				}
			}
			else if (this.mHpBar != null)
			{
				this.mHpBar.SetDebuffIcon(false, string.Empty);
			}
		}
	}

	public void UpdateHUDBar()
	{
		if (this.surface != null)
		{
			this.surface.UpdateHUDBar();
		}
	}

	public void ChangeShader(string shader)
	{
		if (this.surface != null)
		{
			this.surface.SetShader(shader);
		}
	}

	public void ShowOutFlash()
	{
		this.surface.ShowOutFlash();
	}

	public void MarkAsMainPlayer(bool active)
	{
		if (this.surface != null)
		{
			if (active)
			{
				this.surface.MarkAsMainPlayer();
			}
			else
			{
				this.surface.ClearMainPlayer();
			}
		}
	}

	public void MarkAsTarget(bool active)
	{
		if (this.surface != null)
		{
			if (active && this.isVisible && !this.isHideEffect)
			{
				this.surface.MarkAsTarget();
			}
			else
			{
				this.surface.ClearTarget();
			}
		}
	}

	public void SetShader(string shader)
	{
		if (this.surface != null)
		{
			this.surface.SetShader(shader);
		}
	}

	public void RevertShader()
	{
		if (this.surface != null)
		{
			this.surface.RevertShader();
		}
	}

	public void SetOutline(Color color, float width = 0.08f)
	{
		if (this.surface != null)
		{
			this.surface.SetOutline(color, width);
		}
	}

	public void ClearOutline()
	{
		if (this.surface != null)
		{
			this.surface.ClearOutline();
		}
	}

	public void ShowPetrifaction()
	{
		if (this.surface != null)
		{
			this.surface.ShowPetrifaction();
		}
	}

	public void ShowAlpha(bool playOrStopPS, float endAlpha = 1f, float duration = 0.02f, float delay = 0f)
	{
		if (this.surface != null)
		{
			this.surface.ShowAlpha(playOrStopPS, endAlpha, duration, delay);
		}
	}

	public void StopAlphaCoroutine()
	{
		if (this.surface != null)
		{
			this.surface.StopAlphaCoroutine();
		}
	}

	public float GetAlpha()
	{
		if (this.surface != null)
		{
			return this.surface.GetAlpha();
		}
		return -1f;
	}

	public void SetAlpha(float alpha)
	{
		if (this.surface != null)
		{
			this.surface.SetAlpha(alpha);
		}
	}

	public void SetParticlesVisible(bool b)
	{
		if (this.surface != null)
		{
			this.surface.SetParticlesVisible(b);
		}
	}

	public bool isShowSkillPointer()
	{
		return this.surface != null && this.surface.isShowSkillPointer();
	}

	public void ShowSkillPointer()
	{
		if (this.surface != null)
		{
			this.surface.ShowSkillPointer();
		}
	}

	public void HideSkillPointer()
	{
		if (this.surface != null)
		{
			this.surface.HideSkillPointer();
		}
	}

	public void DestroySkillPointer()
	{
		if (this.surface != null)
		{
			this.surface.DestroySkillPointer();
		}
	}

	public void Dissolve(float time)
	{
		if (this.surface != null)
		{
			this.surface.Dissolve(time);
		}
	}

	public int GetKillHeroNum(Callback addBack)
	{
		if (this.GetkillHeroNum == null)
		{
			this.GetkillHeroNum = addBack;
		}
		return this.statistics.GetHeroKill();
	}

	public int GetKillMonsterNum()
	{
		return this.statistics.GetMonsterKill();
	}

	public int GetDeathNum()
	{
		return this.statistics.GetDeathNum();
	}

	public void AddKillHeroNum()
	{
		if (this.statistics != null)
		{
			this.statistics.AddHeroKill();
		}
	}

	public int GetUnitsDeathNum(Callback addBack)
	{
		if (this.GetHeroDeathNum == null)
		{
			this.GetHeroDeathNum = addBack;
		}
		if (this.statistics.DeathCallBack == null)
		{
			this.statistics.DeathCallBack = new Callback(this.AddUnitDeathNum);
		}
		return this.statistics.GetDeathNum();
	}

	public void AddUnitDeathNum()
	{
		if (this.GetHeroDeathNum != null)
		{
			this.GetHeroDeathNum();
		}
	}

	public int GetMoney(Units dead)
	{
		this.ShowMoney(dead);
		int moneyNum = UnityEngine.Random.Range(0, 100);
		if (this.statistics != null)
		{
			return this.statistics.AddMoney(moneyNum);
		}
		return 0;
	}

	private void ShowMoney(Units targetUnit)
	{
	}

	public float GetExp(Units dead)
	{
		return 0f;
	}

	public void UpLevel()
	{
	}

	private bool IsTargetDistanceValid(Units inAttacker, Units inTarget, float inDistance)
	{
		if (inAttacker == null || inTarget == null)
		{
			return false;
		}
		float sqrMagnitude = (inAttacker.transform.position - inTarget.transform.position).sqrMagnitude;
		return sqrMagnitude < inDistance * inDistance;
	}

	public void TryAttackSelectTargetAfterSkillEnd()
	{
		if (StrategyManager.Instance.IsAuto() || !this.CanBeSelected)
		{
			return;
		}
		Units selectedTarget = PlayerControlMgr.Instance.GetSelectedTarget();
		if (selectedTarget == null)
		{
			return;
		}
		if (!this.IsTargetDistanceValid(this, selectedTarget, this.attack_range + 2f))
		{
			return;
		}
		this.SetSelectTarget(selectedTarget);
	}

	public void ComboAttack(Units target = null)
	{
		if (this.atkController != null)
		{
			this.atkController.ComboAttack(target);
		}
	}

	public void InterrupAttack(SkillInterruptType type)
	{
		if (this.atkController != null)
		{
			this.atkController.InterruptAttack(type);
		}
	}

	public void StopAttack()
	{
		if (this.atkController != null)
		{
			this.atkController.StopAttack();
		}
	}

	public void StopAttack(string attackId)
	{
		if (this.atkController != null)
		{
			this.atkController.StopAttack(attackId);
		}
	}

	public void Conjure(string skillId, Units target = null, Vector3? targetPos = null)
	{
		if (this.atkController != null)
		{
			this.atkController.Conjure(skillId, target, targetPos);
		}
	}

	public void InterruptConjure(SkillInterruptType type)
	{
		if (this.atkController != null)
		{
			this.atkController.interruptSkill(type);
		}
	}

	public void StopConjure()
	{
		if (this.atkController != null)
		{
			this.atkController.StopConjure();
		}
	}

	public void StopConjure(string skillId)
	{
		if (this.atkController != null)
		{
			this.atkController.StopConjure(skillId);
		}
	}

	public void StopAction()
	{
		this.StopAttack();
		this.StopConjure();
	}

	public void InterruptAction(SkillInterruptType type)
	{
		this.InterrupAttack(type);
		this.InterruptConjure(type);
	}

	public bool IsSkillCanTriggerBornPowerObj(string inSkillId)
	{
		return this.skillManager != null && this.skillManager.IsSkillCanTriggerBornPowerObj(inSkillId);
	}

	public void ClearBornPowerObjSkillData()
	{
		if (this.skillManager != null)
		{
			this.skillManager.ClearBornPowerObjSkillData();
		}
	}

	public void ResetState()
	{
		this.StopMove();
		this.StopAction();
		this.ResetAnimState();
	}

	public bool CheckSkillCondition(string skillId)
	{
		return this.atkController != null && this.atkController.CheckSkillCondition(skillId);
	}

	public bool CheckSkillCondition(int skillIndex)
	{
		return this.atkController != null && this.atkController.CheckSkillCondition(skillIndex);
	}

	public void SetCurSkillOrAttack(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		if (this.atkController != null)
		{
			if (skill.IsSkill)
			{
				this.atkController.SetCurSkill(skill);
			}
			if (skill.IsAttack)
			{
				this.atkController.SetCurAttack(skill);
			}
		}
	}

	public void UpdateAI(List<Units> targets)
	{
		if (this.aiManager != null)
		{
			this.aiManager.UpdateBehavior(targets);
		}
	}

	public void GuardTower(Units attacker, Units tower)
	{
		if (this.aiManager != null)
		{
			this.aiManager.DefendTower(attacker, tower);
		}
	}

	public void ReceiveEvent(Units sender, AIManager.UnitEvents unit_event)
	{
		if (this.aiManager != null)
		{
			this.aiManager.ReceiveEvent(sender, unit_event);
		}
	}

	public void BroadcastEventToMap(Relation relation, global::TargetTag tag, AIManager.UnitEvents unit_event)
	{
		if (this.aiManager != null)
		{
			this.aiManager.BroadcastEventToMap(relation, tag, unit_event);
		}
	}

	public void BoradcastEventToArround(Relation relation, global::TargetTag tag, AIManager.UnitEvents unit_event)
	{
		if (this.aiManager != null)
		{
			this.aiManager.BroadcastEventToMap(relation, tag, unit_event);
		}
	}

	public void UpdateBotWithVisibile(Units pBot)
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().UpdateBotWithVisibile(pBot);
		}
	}

	public void UpdateMemory(List<Units> pBots)
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().UpdateMemory(pBots);
		}
	}

	public void UpdateBotWithoutVisibile(Units pBot)
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().UpdateBotWithoutVisibile(pBot);
		}
	}

	public void isOpponentWithinFOV(Units pBot)
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().isOpponentWithinFOV(pBot);
		}
	}

	public void UpdateBotWithDeath(Units pBot)
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().UpdateBotWithDeath(pBot);
		}
	}

	public void UpdateBotsWithSpawned()
	{
		if (this.aiManager != null)
		{
			this.aiManager.GetSensoryMemory().UpdateBotsWithSpawned();
		}
	}

	public void SetSelectTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetSelectTarget(target);
		}
	}

	public void SetAttackTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetAttackTarget(target);
		}
	}

	public void SetTauntTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetTauntTarget(target);
		}
	}

	public void SetAttackYouTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetAttackYouTarget(target);
		}
	}

	public void SetSkillHitedTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetSkillHitedTarget(target);
		}
	}

	public void SetSkillHitedYouTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetSkillHitedYouTarget(target);
		}
	}

	public void SetAttackedYouTarget(Units target)
	{
		if (this.aiManager != null)
		{
			this.aiManager.SetAttackedYouTarget(target);
		}
	}

	public Units GetAttackTarget()
	{
		Units units = null;
		if (this.aiManager != null)
		{
			units = this.aiManager.GetAttackTarget();
		}
		if (!units)
		{
			units = this.FakeAttackTarget;
		}
		return units;
	}

	public Units GetTreeAtkTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.CurTreeAtkTarget;
		}
		return null;
	}

	public Units GetAttackYouTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetAttackYouTarget();
		}
		return null;
	}

	public Units GetSkillHitedTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetSkillHitedTarget();
		}
		return null;
	}

	public Units GetSkillHitedYouTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetSkillHitedYouTarget();
		}
		return null;
	}

	public Units GetLastKilledTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.LastKilledTarget;
		}
		return null;
	}

	public void SetLastKilledTarget(Units target)
	{
		if (this.aiManager != null && target != null)
		{
			this.aiManager.LastKilledTarget = target;
		}
	}

	public int GetTargetHatredValue(Units target)
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetTargetHatredValue(target);
		}
		return 0;
	}

	public int GetTargetPorityValue(Units target)
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetTargetPorityValue(target);
		}
		return 0;
	}

	public Units GetAttackedYouTarget()
	{
		if (this.aiManager != null)
		{
			return this.aiManager.GetAttackedYouTarget();
		}
		return null;
	}

	public void SetMirrorState(bool state)
	{
		this.mirrorState = state;
	}

	public PlayEffectAction StartLevelEffect()
	{
		if (this.effectManager != null)
		{
			return this.effectManager.StartLevelUpEffect();
		}
		return null;
	}

	public PlayEffectAction StartEffect(string perform_id)
	{
		if (this.effectManager != null)
		{
			return this.effectManager.StartEffect(perform_id);
		}
		return null;
	}

	public List<string> GetSkills()
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetSkills();
		}
		return null;
	}

	public List<string> GetUnlockSkills()
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetUnlockSkills();
		}
		return null;
	}

	public List<string> GetPassiveSkills()
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetPassiveSkills();
		}
		return null;
	}

	public Skill getSkillById(string skillName)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getSkillById(skillName);
		}
		return null;
	}

	public Skill getSkillByIndex(int skill_index)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getSkillByIndex(skill_index);
		}
		return null;
	}

	public List<SkillDataKey> getSkillsByIndex(int skill_index)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getSkillsByIndex(skill_index);
		}
		return null;
	}

	public string getSkillIdByIndex(int skill_index)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getSkillIdByIndex(skill_index);
		}
		return null;
	}

	public int IndexOfSkill(string skill_id)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.IndexOfSkill(skill_id);
		}
		return -1;
	}

	public int GetSkillPropority(int skill_index)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetSkillPropority(skill_index);
		}
		return 0;
	}

	public Skill getAttackByIndex(int skill_index)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getAttackByIndex(skill_index);
		}
		return null;
	}

	public Skill getAttackById(string attackName)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.getAttackById(attackName);
		}
		return null;
	}

	public bool isSkillLock(int skill_index)
	{
		return this.skillManager != null && this.skillManager.isSkillLock(skill_index);
	}

	public float GetSkillCDTime(string skill_id)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetSkillCDTime(skill_id);
		}
		return -1f;
	}

	public float GetSkillPublicCDTime(string skill_id)
	{
		if (this.skillManager != null)
		{
			return this.skillManager.GetPublicCDTime(skill_id);
		}
		return -1f;
	}

	public void ReplaceSkill(string sourceId, string targetId)
	{
		if (this.skillManager != null)
		{
			this.skillManager.ReplaceSkill(sourceId, targetId);
		}
	}

	public void ReplaceAttack(string sourceId, string targetId)
	{
		if (this.skillManager != null)
		{
			this.skillManager.ReplaceAttack(sourceId, targetId);
		}
	}

	public Skill getSkillOrAttackById(string skillName)
	{
		if (this.skillManager != null)
		{
			Skill skill = this.skillManager.getSkillById(skillName);
			if (skill != null)
			{
				return skill;
			}
			skill = this.skillManager.getAttackById(skillName);
			if (skill != null)
			{
				return skill;
			}
		}
		return null;
	}

	public void SetSkillContext(Skill inSkill)
	{
		this.contextSkill = inSkill;
	}

	public void SetPosContext(Vector3 pos)
	{
		this.contextPos = pos;
	}

	public void ClearSkillContext()
	{
		this.contextSkill = null;
	}

	public Skill GetSkillContext()
	{
		return this.contextSkill;
	}

	public Vector3 GetPosContext()
	{
		return this.contextPos;
	}

	protected override void OnCreate()
	{
		base.OnCreate();
		if (this.mCollider == null)
		{
			this.mCollider = base.gameObject.GetComponent<CapsuleCollider>();
		}
		if (this.mCollider != null)
		{
			this.m_ColliderHeight = this.mCollider.height;
			this.m_Radius = this.mCollider.radius;
		}
		if (this.mRender == null)
		{
			this.mRender = base.GetComponentInChildren<SkinnedMeshRenderer>();
		}
		UnitRenderer component = base.gameObject.GetComponent<UnitRenderer>();
		if (component != null)
		{
			UnityEngine.Object.Destroy(component);
		}
		if (this.mRenders == null || (this.mRenders != null && this.mRenders.Length == 0))
		{
			this.mRenders = base.GetComponentsInChildren<Renderer>();
		}
		if (!AnimPlayer.useMeshanim)
		{
			for (int i = 0; i < this.mRenders.Length; i++)
			{
				if (this.mRenders[i] != null && this.mRenders[i].gameObject.name.Contains("@"))
				{
					this.mRenders[i] = null;
				}
			}
		}
		if (this.mMecanim == null)
		{
			this.mMecanim = base.gameObject.GetComponent<Mecanim>();
		}
		if (this.mCharacterEffect == null)
		{
			this.mCharacterEffect = base.gameObject.GetComponent<CharacterEffect>();
		}
	}

	public void ClearResources()
	{
	}

	private void InitComponentTime()
	{
		if (!this.isHero)
		{
			this.componentUpdatedelayTime = 0.31f;
		}
	}

	protected override void OnInit(bool isRebirth = false)
	{
		this.isRemoved = false;
		this.isPlayer = this.tag.Equals("Player");
		this.isHero = (this.isPlayer || this.tag.Equals("Hero"));
		this.isHome = this.tag.Equals("Home");
		this.isTower = this.tag.Equals("Building");
		this.isShop = (base.gameObject.layer == Layer.ShopLayer);
		this.isItem = this.tag.Equals("Item");
		this.isBuffItem = this.tag.Equals("BuffItem");
		this.isMonster = this.tag.Equals("Monster");
		this.isBuilding = (this.isHome || this.isTower);
		this.InitComponentTime();
		this.InitAllUnitComponent();
		this.EnableAction(true);
		this.isLive = true;
		this.ResetUnitRenderer(false);
		Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitSpawn, this, null, null);
		this.m_nVisibleState = -1;
		this.m_nGrassState = 0;
		this.m_fVisibleTimer = 0f;
		this.m_bIgnoreVisible = false;
		if (GlobalSettings.FogMode >= 2)
		{
			if (this.isMyTeam)
			{
				if (this.m_fowrevealer == null)
				{
					if (!this.isTower && !this.isHome)
					{
						this.m_fowrevealer = new FOWRevealer();
						this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.EveryUpdate, false);
					}
					else
					{
						this.m_fowrevealer = new FOWRevealer();
						this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.Static, false);
					}
				}
				else if (this.m_fowrevealer.onlygrass && !this.isTower && !this.isHome)
				{
					this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.EveryUpdate, false);
				}
			}
			else if (this.m_fowrevealer == null)
			{
				if (!this.isTower && !this.isHome)
				{
					this.m_fowrevealer = new FOWRevealer();
					this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.EveryUpdate, true);
				}
			}
			else if (!this.m_fowrevealer.onlygrass && !this.isTower && !this.isHome)
			{
				this.m_fowrevealer.DoDestroy();
				this.m_fowrevealer.Create(base.transform, this.fog_range, FOWSystem.LOSChecks.EveryUpdate, true);
			}
		}
		base.OnInit(false);
	}

	protected override void OnStart()
	{
		this.StartAllUnitComponent();
		this.startCount++;
		if (Singleton<SkillView>.Instance != null && this.isPlayer)
		{
			Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
		}
	}

	private void LateUpdate()
	{
		if (GameManager.IsPlaying())
		{
			bool visible = this.isVisible && this.m_nVisibleState < 2 && this.isLive;
			this.surface.UpdateHud(visible);
		}
	}

	private void CheckSync()
	{
		if (this.npc_id == "Kulouwang")
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup >= this._nextCheckSyncTime)
		{
			this._nextCheckSyncTime = realtimeSinceStartup + 10f;
			if ((this.MirrorState || this.hp / this.hp_max < 0.5f) && Math.Abs(this._lastSyncHp - this.hp) < 0.1f)
			{
				C2PQueryUnit c2PQueryUnit = new C2PQueryUnit
				{
					unitId = this.unique_id
				};
				SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_QueryUnit, SerializeHelper.Serialize<C2PQueryUnit>(c2PQueryUnit));
			}
		}
	}

	public void RefreshSyncState()
	{
		this._nextCheckSyncTime = Time.realtimeSinceStartup + 20f;
		this._lastSyncHp = this.hp;
	}

	protected override void OnUpdate(float delta)
	{
		this.CheckSync();
		if (this.timeSyncSystem != null)
		{
			delta *= this.timeSyncSystem.unitsTimeScale;
		}
		this.UpdateAllUnitComponent(delta);
		this.UpdateVisible();
		this.isVisible = (this.m_nVisibleState < 2);
		if (this == PlayerControlMgr.Instance.GetSelectedTargetFast())
		{
			PlayerControlMgr.Instance.TryUpdateTargetIcon();
		}
	}

	private void AddMiniMapIcon()
	{
		GameObject gameObject = Singleton<MiniMapView>.Instance.AddMiniMapIcon(this);
		if (gameObject)
		{
			this.mMiniMapIcon = gameObject.transform;
		}
	}

	private void RemoveMiniMapIcon()
	{
		if (this.mMiniMapIcon)
		{
			Singleton<MiniMapView>.Instance.RemoveMiniMapIcon(this.mMiniMapIcon.gameObject);
			this.mMiniMapIcon = null;
		}
	}

	private void UpdateMiniMap()
	{
	}

	public virtual void UpdateVisible()
	{
		if (!this.m_bUpdateVisible)
		{
			return;
		}
		if (this.m_bIgnoreVisible)
		{
			return;
		}
		if (this.isTower || this.isHome)
		{
			return;
		}
		if (this.UnitType == UnitType.Pet && this.parentUnit != null)
		{
			this.m_nServerVisibleState = this.parentUnit.m_nServerVisibleState;
		}
		if (this.UnitType == UnitType.LabisiUnit)
		{
			this.m_nServerVisibleState = 0;
		}
		if (Singleton<PvpManager>.Instance.IsGlobalObserver)
		{
			if (this.UnitType == UnitType.Monster)
			{
				this.m_nServerVisibleState = 0;
			}
			if (this.m_nServerVisibleState != 0)
			{
				this.m_nServerVisibleState = 1;
			}
			else
			{
				this.m_nServerVisibleState = 0;
			}
		}
		if (this.m_nVisibleState != this.m_nServerVisibleState)
		{
			this.m_nVisibleState = this.m_nServerVisibleState;
			switch (this.m_nVisibleState)
			{
			case 0:
				UnitVisibilityManager.BecomeFullVisible(this);
				UnitVisibilityManager.SetUnitAlpha(this, 1f, false, false, false, true);
				UnitVisibilityManager.SetParticlesVisible(this, true);
				this.effectManager.SetVisible(true);
				this.SetAlpha(1f);
				if (this.mHpBar != null && !GlobalSettings.Instance.UIOpt)
				{
					this.mHpBar.SetDebuffIcon(false, string.Empty);
					this.mHpBar.SetBarAlpha(1f);
				}
				break;
			case 1:
				UnitVisibilityManager.SetUnitAlpha(this, 0.5f, false, false, false, true);
				UnitVisibilityManager.SetParticlesVisible(this, true);
				this.effectManager.SetVisible(true);
				this.SetAlpha(0.5f);
				if (this.mHpBar != null && !GlobalSettings.Instance.UIOpt)
				{
					this.mHpBar.SetDebuffIcon(false, string.Empty);
					this.mHpBar.SetBarAlpha(1f);
				}
				break;
			case 2:
				UnitVisibilityManager.SetUnitAlpha(this, 0f, true, true, true, true);
				UnitVisibilityManager.SetParticlesVisible(this, false);
				this.effectManager.SetVisible(false);
				this.SetAlpha(0f);
				if (this.mHpBar != null && !GlobalSettings.Instance.UIOpt)
				{
					this.mHpBar.SetDebuffIcon(false, string.Empty);
					this.mHpBar.SetBarAlpha(0f);
				}
				break;
			case 3:
				UnitVisibilityManager.SetUnitAlpha(this, 0f, true, true, true, true);
				UnitVisibilityManager.SetParticlesVisible(this, false);
				this.effectManager.SetVisible(false);
				this.SetAlpha(0f);
				if (this.mHpBar != null && !GlobalSettings.Instance.UIOpt)
				{
					this.mHpBar.SetDebuffIcon(false, string.Empty);
					this.mHpBar.SetBarAlpha(0f);
				}
				break;
			}
		}
	}

	public void EnableRender(bool bEnable, bool isHideEffect = true, bool isHideHUDText = true)
	{
		if (bEnable)
		{
			this.m_nVisibleState = -1;
			this.m_bIgnoreVisible = false;
			return;
		}
		this.m_bIgnoreVisible = true;
		UnitVisibilityManager.SetUnitAlpha(this, 0f, true, true, true, isHideHUDText);
		UnitVisibilityManager.SetParticlesVisible(this, false);
		if (isHideEffect)
		{
			this.effectManager.SetVisible(false);
		}
		this.SetAlpha(0f);
		if (this.mHpBar != null && !GlobalSettings.Instance.UIOpt)
		{
			this.mHpBar.SetDebuffIcon(false, string.Empty);
			this.mHpBar.SetBarAlpha(0f);
		}
	}

	public void SetItemSkill(List<ItemDynData> ItemList)
	{
		if (ItemList == null)
		{
			return;
		}
		if (this.skillManager != null)
		{
			this.skillManager.SetItemSkill(ItemList);
		}
	}

	protected override void OnStop()
	{
		this.EnableAction(false);
		if (this.m_CoroutineManager != null)
		{
			this.m_CoroutineManager.StopAllCoroutine();
		}
		this.StopAllUnitComponent();
	}

	protected override void OnExit()
	{
		this.EnableAction(false);
		if (this.m_CoroutineManager != null)
		{
			this.m_CoroutineManager.StopAllCoroutine();
		}
		this.RemoveAllUnitComponent();
		this.startCount = 0;
	}

	protected override void OnEnd()
	{
	}

	public virtual void Wound(Units attacker, float damage)
	{
	}

	public virtual void TryDeath(Units attacker)
	{
	}

	public virtual void Rebirth()
	{
	}

	public virtual void PreDeath(Units attacker)
	{
	}

	public virtual void RealDeath(Units attacker)
	{
	}

	public virtual void PseudoDeath(int inOldGroupType, int inNewGroupType, float inHpVal, float inMpVal, string inNpcId, string inBattleMonsterCreepId, Units inAttacker)
	{
	}

	public virtual void FakeDeath()
	{
	}

	protected bool CheckDeathCondition(Units attacker)
	{
		if (this.hp <= 0f || float.IsNaN(this.hp))
		{
			if (this.IsFree)
			{
				return true;
			}
			if (attacker != null && attacker.IsMaster)
			{
				return true;
			}
			if (attacker == null && this.IsMaster)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(this.ColliderCenter, this.m_SelectRadius);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.ColliderCenter, this.m_Radius);
		Gizmos.color = ((this.teamType != 1) ? Color.cyan : Color.yellow);
		if (GlobalSettings.IsMasterGizmosShown)
		{
		}
		if (this._showServerPos && this.teamType != 2)
		{
			this.serverPos.y = 0f;
			Gizmos.DrawSphere(this.serverPos + new Vector3(0f, this.GetHeight() + 1f, 0f), Units._size);
		}
		if (this._showServerTarget && this.isMonster && this.teamType != 2)
		{
			Vector3 vector = this.serverPos + new Vector3(0f, this.GetHeight() + 1f, 0f);
			Gizmos.DrawLine(base.transform.position, vector);
			Vector3 to = this.serverTargetPos;
			to.y = 2f;
			if (this.serverTargetPos != MoveController.ServerPosInvalide)
			{
				Gizmos.DrawLine(vector, to);
			}
			if (this.recvPath < 10)
			{
				Gizmos.DrawRay(vector, new Vector3(0f, 20f, 0f));
				this.recvPath++;
			}
		}
	}

	public void ShowAiState(string str)
	{
		if (Units.ShowServerUnitAI)
		{
			this.AIDebugStr = str;
		}
	}

	public void ForceIdle()
	{
		if (this.animController != null)
		{
			this.animController.ForceIdle();
		}
	}

	public void ForceDeath()
	{
		if (this.animController != null)
		{
			this.animController.ForceDeath();
		}
	}

	public void ForceNormalized(float normalized = 1f)
	{
		if (this.animController != null)
		{
			this.animController.ForceNormalized(normalized);
		}
	}

	public void ToAnimStart()
	{
		if (this.animController != null)
		{
			this.animController.ToAnimStart();
		}
	}

	public void HideSelf(float delay = 0f)
	{
		if (this.surface != null)
		{
			this.surface.HideModel();
		}
	}

	public virtual void RemoveSelf(float delay = 0f)
	{
		this.doRemoveSelf(delay);
	}

	public bool IsRemoved()
	{
		return this.isRemoved;
	}

	public void doRemoveSelf(float delay = 0f)
	{
		if (!this.isRemoved)
		{
			Singleton<TriggerManager>.Instance.SendUnitStateEvent(UnitEvent.UnitDespawn, this, null, null);
			if (MapManager.Instance != null)
			{
				MapManager.Instance.DespawnUnit(this, delay);
			}
			this.isRemoved = true;
		}
		if (this.m_fowrevealer != null)
		{
			this.m_fowrevealer.DoDestroy();
			this.m_fowrevealer = null;
		}
	}

	public virtual void EnableAllRenders(bool b)
	{
		if (this.surface != null)
		{
			this.surface.EnableAllRenders(b);
		}
	}

	public void SetIsMoving(bool moving)
	{
		if (this.moveController != null)
		{
			this.moveController.SetIsMoving(moving);
		}
	}

	public void SetCanMove(bool enabled)
	{
		this.isCanMove = enabled;
	}

	public void SetWaitCool(float wait, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.waitCool = ((wait <= this.crickTime.waitCool && !isReset) ? this.crickTime.waitCool : wait);
		}
	}

	public void SetPosition(Vector3 newPos, bool adjustHeightWithGround = false)
	{
		if (AstarPath.active != null)
		{
			newPos.y = AstarPath.active.GetPosHeight(newPos);
		}
		if (base.trans != null)
		{
			base.trans.position = newPos;
		}
		if (this.moveController != null)
		{
			this.moveController.LocalPosition = newPos;
		}
	}

	public void SetCanAction(bool enabled)
	{
		this.isCanAction = enabled;
	}

	public void SetActionCool(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.actionCool = ((cool <= this.crickTime.actionCool && !isReset) ? this.crickTime.actionCool : cool);
		}
	}

	public void SetMoveAnimCool(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.moveAnimCool = ((cool <= this.crickTime.moveAnimCool && !isReset) ? this.crickTime.moveAnimCool : cool);
		}
	}

	public void SetSkillCool(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.skillCool = ((cool <= this.crickTime.skillCool && !isReset) ? this.crickTime.skillCool : cool);
		}
	}

	public void SetAttackCool(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.attackCool = ((cool <= this.crickTime.attackCool && !isReset) ? this.crickTime.attackCool : cool);
		}
	}

	public void SetCanAIControl(bool enabled)
	{
		if (base.name == "Xiaolu_LM_TEAM" && enabled)
		{
			int num = 10;
			num++;
		}
		this.isCanAIControl = enabled;
		if (this.aiManager != null)
		{
			this.aiManager.UpdateAIState();
		}
	}

	public void SetAIAutoAttackMove(bool enabled)
	{
	}

	public void SetAICool(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.aiCool = ((cool <= this.crickTime.aiCool && !isReset) ? this.crickTime.aiCool : cool);
		}
		if (this.aiManager != null)
		{
			this.aiManager.UpdateAIState();
		}
	}

	public void SetAttackTimeLengh(float cool, bool isReset = false)
	{
		if (this.crickTime != null)
		{
			this.crickTime.attackTimeLenghCool = ((cool <= this.crickTime.attackTimeLenghCool && !isReset) ? this.crickTime.attackTimeLenghCool : cool);
		}
	}

	public void SetCanAttack(bool b)
	{
		this.isCanAttack = b;
	}

	public void SetCanSkill(bool b)
	{
		this.isCanSkill = b;
	}

	public void SetCanRotate(bool enable)
	{
		this.canRoatate = enable;
	}

	public void SetCDTime(string skill_id, float cd_time)
	{
		if (this.crickTime != null)
		{
			this.crickTime.SetSkillCDTime(skill_id, cd_time);
		}
	}

	public float GetCDTime(string skill_id)
	{
		if (this.crickTime != null)
		{
			return this.crickTime.GetSkillCDtime(skill_id);
		}
		return 0f;
	}

	public void SetChargeTime(string skill_id, float cd_time)
	{
		if (this.crickTime != null)
		{
			this.crickTime.SetChargeTime(skill_id, cd_time);
		}
	}

	public float GetChargeTime(string skill_id)
	{
		if (this.crickTime != null)
		{
			return this.crickTime.GetChargeTime(skill_id);
		}
		return 0f;
	}

	public void OnSkillSynced(string inSkillId, byte inUseState)
	{
		if (this.skillManager != null)
		{
			this.skillManager.OnSkillSynced(inSkillId, inUseState);
		}
	}

	public void SetCanEnableActionHighEff(bool b)
	{
		this.isEnableActionHighEff = b;
	}

	public void EnableAction(bool enabled)
	{
		if (enabled)
		{
			this.SetCanAction(true);
			this.SetCanMove(true);
			this.SetCanAIControl(true);
			this.SetCanRotate(true);
			this.SetCanAttack(true);
			this.SetCanSkill(true);
		}
		else
		{
			this.SetCanAction(false);
			this.SetCanMove(false);
			this.SetCanAIControl(false);
			this.SetCanRotate(false);
			this.SetCanAttack(false);
			this.SetCanSkill(false);
		}
	}

	public void SetFrozenAnimation(bool b)
	{
		this.isFrozenAnimtion = b;
	}

	public void SetLockCharaEffect(bool b)
	{
		this.isLockCharaEffect = b;
	}

	public void SetLockCharaControl(bool b)
	{
		this.isLockCharaControl = b;
		if (b)
		{
			this.SetAIAutoAttackMove(b);
		}
	}

	public void SetLockInputState(bool b)
	{
		this.isLockInput = b;
	}

	public void SetLockAnimState(bool b)
	{
		this.isLockAnimState = b;
	}

	public void ResetInterrupt(bool resetCool = true, SkillInterruptType interrupt = SkillInterruptType.None)
	{
		this.SetActionCool(0f, resetCool);
		this.SetAICool(0f, resetCool);
		this.SetWaitCool(0f, resetCool);
		this.SetSkillCool(0f, resetCool);
	}

	public void ResetAttack()
	{
		this.SetAttackCool(0f, true);
	}

	public void ResetConjure()
	{
		this.SetSkillCool(0f, true);
	}

	public void SetForceClickGroundState(bool b)
	{
		this.isForceClickGroud = b;
	}

	public void SetHideEffectForEnemy(bool b)
	{
		this.isHideEffect = b;
	}

	protected void BeInjured()
	{
		if (this.OnWoundCallback != null)
		{
			this.OnWoundCallback(this);
		}
	}

	protected void BeDeath()
	{
		if (this.OnDeathCallback != null)
		{
			this.OnDeathCallback(this);
		}
	}

	protected void BeReborn()
	{
		if (this.OnRebornCallback != null)
		{
			this.OnRebornCallback(this);
		}
	}

	protected void UseMagic()
	{
		if (this.OnUseMagic != null)
		{
			this.OnUseMagic(this.mp, this.mp_max);
		}
	}

	public void CallAttacked()
	{
		if (this.OnAttackCallback != null)
		{
			this.OnAttackCallback(this);
		}
	}

	public void CallSkilled()
	{
		if (this.OnSkillCallback != null)
		{
			this.OnSkillCallback(this);
		}
	}

	public bool IsInAttackOrSkill()
	{
		return (this.currentAttack != null && this.currentAttack.IsCastting) || (this.currentSkill != null && this.currentSkill.IsCastting);
	}

	public bool IsMonsterCreep()
	{
		return this._isMonsterCreep;
	}

	public void SetIsMonsterCreep(bool inIsMonsterCreep)
	{
		this._isMonsterCreep = inIsMonsterCreep;
	}

	public virtual bool IsMonsterCreepAiStatus(EMonsterCreepAiStatus inMonsterCreepAiStatus)
	{
		return false;
	}

	public virtual void SetMonsterCreepAiStatus(EMonsterCreepAiStatus inMonsterCreepAiStatus)
	{
	}

	private float Clamp(float source, float min = 0f, float max = 3.40282347E+38f)
	{
		return Mathf.Clamp(source, min, max);
	}

	public int ReturnSkillsNumber()
	{
		int result;
		switch (this.quality)
		{
		case 1:
			result = 1;
			break;
		case 2:
		case 3:
			result = 2;
			break;
		case 4:
		case 5:
		case 6:
			result = 3;
			break;
		default:
			result = 4;
			break;
		}
		return result;
	}

	public void StartAllSkillCD(float minTime)
	{
		List<string> skills = this.GetSkills();
		for (int i = 0; i < skills.Count; i++)
		{
			if (this.GetCDTime(skills[i]) < minTime)
			{
				this.SetCDTime(skills[i], minTime);
			}
		}
	}

	public void AttackInGrass()
	{
		UnitVisibilityManager.BecomeHalfVisible(this);
		this.m_CoroutineManager.StopCoroutine(this.m_GrassTask);
	}

	public void EnterGrass(string grassId)
	{
	}

	public void ExitGrass(string grassId)
	{
	}

	protected virtual void DispatchDeadEvent(Units attacker)
	{
		this.BeDeath();
		if (Units.OnUnitsDead != null)
		{
			Units.OnUnitsDead(this, attacker);
		}
	}

	protected virtual void DispatchWoundEvent(float damage)
	{
		this.BeInjured();
		if (Units.OnUnitsWound != null)
		{
			Units.OnUnitsWound(this, damage);
		}
	}

	protected virtual void DispatchRebirthEvent()
	{
		this.BeReborn();
		if (Units.OnUnitsRebirth != null)
		{
			Units.OnUnitsRebirth(this);
		}
		base.broadCastMsg("onBorn", false);
	}

	public void SetNewbieHintObj(GameObject inObj)
	{
		if (inObj != null)
		{
			this._newbieHintObj = inObj;
		}
	}

	protected void TryHideNewbieHintObj()
	{
		if (this._newbieHintObj != null)
		{
			this._newbieHintObj.SetActive(false);
		}
	}

	public void onTargetSelected()
	{
		this.playVoice("onUpdateMoveTarget");
	}

	public void UpdateLimitY()
	{
		if (base.mTransform.position.y < 0f)
		{
			this.SetPosition(new Vector3(base.mTransform.position.x, 0f, base.mTransform.position.z), false);
		}
	}

	public void ClearReplacePerform()
	{
		this.ReadyActions = null;
		this.StartActions = null;
		this.HitActions = null;
	}

	public void setGrassInvisible()
	{
	}

	public void setGrassVisible()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		base.addMsgLs(typeof(HitUnitMsg), new Action<GameMessage>(this.onHitUnit));
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		base.dropMsgs();
	}

	public void CrucherMesh(float percent = 0.5f)
	{
	}

	private void onHitUnit(GameMessage gameMsg)
	{
	}

	private void onGrassInvisible()
	{
	}

	private void onGrassVisible()
	{
	}

	private void onLeaveGrass()
	{
	}

	public virtual void SetOrigin(bool fromNeutralMonster, string id, int mosterTeamId)
	{
	}

	public virtual string GetBattleMonsterCreepId()
	{
		return string.Empty;
	}

	public virtual int GetBattleMonsterTeamId()
	{
		return 0;
	}

	public virtual void TryAddBirthEffect()
	{
	}

	protected virtual bool IsConfigedAttackable()
	{
		return true;
	}

	protected virtual bool IsConfigedSelectable()
	{
		return true;
	}

	public bool IsManualSelectable()
	{
		return this.IsConfigedSelectable();
	}

	public float GetTimeScale()
	{
		if (this.timeSyncSystem != null)
		{
			return this.timeSyncSystem.unitsTimeScale;
		}
		return 1f;
	}

	public bool OnPvpServerMsg(MobaMessage msg)
	{
		if (this.timeSyncSystem == null)
		{
			return false;
		}
		if (this.pvpSvrMsgList.Count > 0)
		{
			this.pvpSvrMsgList.Add(msg);
			return true;
		}
		if (msg.svrTime <= this.timeSyncSystem.unitsClientSvrTime + 29L)
		{
			return false;
		}
		this.pvpSvrMsgList.Add(msg);
		return true;
	}

	public MobaMessage FetchPvpServerMsg()
	{
		if (this.timeSyncSystem == null)
		{
			return null;
		}
		long unitsClientSvrTime = this.timeSyncSystem.unitsClientSvrTime;
		if (this.pvpSvrMsgList.Count > 0 && this.pvpSvrMsgList[0].svrTime <= unitsClientSvrTime + 29L)
		{
			MobaMessage mobaMessage = this.pvpSvrMsgList[0];
			this.pvpSvrMsgList.RemoveAt(0);
			this.timeSyncSystem.ResetClientSvrTime(mobaMessage.svrTime);
			return mobaMessage;
		}
		return null;
	}

	public T GetUnitComponent<T>() where T : UnitComponent, new()
	{
		List<UnitComponent> list = (!Units.CompType<T>.isStatic) ? this.dynamicComponents : this.staticComponents;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] is T)
			{
				return list[i] as T;
			}
		}
		return (T)((object)null);
	}

	public T AddUnitComponent<T>() where T : UnitComponent, new()
	{
		List<UnitComponent> list = (!Units.CompType<T>.isStatic) ? this.dynamicComponents : this.staticComponents;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] is T)
			{
				return list[i] as T;
			}
		}
		T t = Activator.CreateInstance<T>();
		t.Create(this);
		list.Add(t);
		return t;
	}

	public T AddUnitComponent<T>(bool needUpdate) where T : UnitComponent, new()
	{
		List<UnitComponent> list = (!needUpdate) ? this.staticComponents : this.dynamicComponents;
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			UnitComponent unitComponent = list[i];
			if (unitComponent is T)
			{
				return unitComponent as T;
			}
		}
		T t = Activator.CreateInstance<T>();
		t.Create(this);
		list.Add(t);
		return t;
	}

	public void RemoveUnitComponent<T>() where T : UnitComponent, new()
	{
		List<UnitComponent> list = (!Units.CompType<T>.isStatic) ? this.dynamicComponents : this.staticComponents;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] is T)
			{
				if (list[i] != null)
				{
					list[i].OnExit();
				}
				list.RemoveAt(i);
				return;
			}
		}
	}

	public void RemoveAllUnitComponent()
	{
		for (int i = 0; i < this.dynamicComponents.Count; i++)
		{
			if (this.dynamicComponents[i] != null)
			{
				this.dynamicComponents[i].OnExit();
			}
		}
		for (int j = 0; j < this.staticComponents.Count; j++)
		{
			if (this.staticComponents[j] != null)
			{
				this.staticComponents[j].OnExit();
			}
		}
		this.dynamicComponents.Clear();
		this.staticComponents.Clear();
	}

	public void InitAllUnitComponent()
	{
		for (int i = 0; i < this.staticComponents.Count; i++)
		{
			if (this.staticComponents[i] != null)
			{
				this.staticComponents[i].OnInit();
			}
		}
		for (int j = 0; j < this.dynamicComponents.Count; j++)
		{
			if (this.dynamicComponents[j] != null)
			{
				this.dynamicComponents[j].OnInit();
			}
		}
	}

	public void StartAllUnitComponent()
	{
		for (int i = 0; i < this.staticComponents.Count; i++)
		{
			if (this.staticComponents[i] != null)
			{
				this.staticComponents[i].OnStart();
			}
		}
		for (int j = 0; j < this.dynamicComponents.Count; j++)
		{
			if (this.dynamicComponents[j] != null)
			{
				this.dynamicComponents[j].OnStart();
			}
		}
	}

	public void UpdateAllUnitComponent(float deltaTime)
	{
		this.unitComponentUpdateDelaTime += deltaTime;
		if (this.moveController != null)
		{
			this.moveController.OnUpdateByFrame(deltaTime);
		}
		if (this.mBtlTouchController != null)
		{
			this.mBtlTouchController.OnUpdateByFrame(deltaTime);
		}
		if (this.timeSyncSystem != null)
		{
			this.timeSyncSystem.OnUpdate(deltaTime);
		}
		if (!this.isVisibleInCamera && !this.isHero && !this.isTower)
		{
			return;
		}
		if (this.unitComponentUpdateDelaTime > this.componentUpdatedelayTime)
		{
			for (int i = 0; i < this.dynamicComponents.Count; i++)
			{
				UnitComponent unitComponent = this.dynamicComponents[i];
				if (unitComponent != null && !unitComponent.donotUpdateByMonster)
				{
					unitComponent.OnUpdate(this.unitComponentUpdateDelaTime);
				}
			}
			this.unitComponentUpdateDelaTime = 0f;
		}
	}

	public void StopAllUnitComponent()
	{
		for (int i = 0; i < this.dynamicComponents.Count; i++)
		{
			if (this.dynamicComponents[i] != null)
			{
				this.dynamicComponents[i].OnStop();
			}
		}
		for (int j = 0; j < this.staticComponents.Count; j++)
		{
			if (this.staticComponents[j] != null)
			{
				this.staticComponents[j].OnStop();
			}
		}
	}
}
