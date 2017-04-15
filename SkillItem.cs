using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using Newbie;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class SkillItem : MonoBehaviour
{
	[Serializable]
	public class CdItems
	{
		public Transform cdRoot;

		[NonSerialized]
		public GameObject mask;

		[NonSerialized]
		public GameObject mask1;

		[NonSerialized]
		public GameObject mask2;

		[NonSerialized]
		public TweenFill tweener;

		[NonSerialized]
		public TweenFill tweener1;

		[NonSerialized]
		public TweenFill tweener2;

		[NonSerialized]
		public bool isVisible = true;

		public void FindAll()
		{
			this.mask = this.cdRoot.Find("Mask1").gameObject;
			this.mask1 = this.cdRoot.Find("Mask2").gameObject;
			this.mask2 = this.cdRoot.Find("Mask3").gameObject;
			this.tweener = this.mask.GetComponent<TweenFill>();
			this.tweener1 = this.mask1.GetComponent<TweenFill>();
			this.tweener2 = this.mask2.GetComponent<TweenFill>();
		}

		public void Show(bool shown)
		{
			if (this.isVisible != shown)
			{
				this.isVisible = shown;
				this.cdRoot.gameObject.SetActive(shown);
			}
		}
	}

	[SerializeField]
	private SkillItem.CdItems m_cdItems;

	[SerializeField]
	private SkillIcon m_skillIconScript;

	[SerializeField]
	private GameObject m_state1;

	[SerializeField]
	private GameObject m_state2;

	[SerializeField]
	private GameObject m_lock;

	[SerializeField]
	public UISprite m_bgSprite;

	[SerializeField]
	public UISprite m_bgSprite2;

	[SerializeField]
	private UITexture m_textureIcon1;

	[SerializeField]
	private UISprite m_spriteIcon1;

	[SerializeField]
	private UITexture m_textureIcon2;

	[SerializeField]
	private UISprite m_spriteIcon2;

	[SerializeField]
	private UILabel numText;

	[SerializeField]
	private UILabel m_countLabel;

	public UISprite m_selectSprite;

	public UILabel m_forbidLabel;

	public GameObject m_unlockAnim;

	public int SkillItemIndex;

	private string saveName = string.Empty;

	[SerializeField]
	private GameObject SkillStateFX;

	[SerializeField]
	private GameObject SkillBornPowerObjTriggerHintFX;

	[SerializeField]
	public GameObject levelUp;

	public GameObject levelUpBtn;

	[SerializeField]
	public GameObject levelUpBtn_b;

	[SerializeField]
	public GameObject levelUpBtn_l;

	[SerializeField]
	public GameObject levelUpBtn_r;

	[SerializeField]
	private UISprite[] points;

	[SerializeField]
	private UIGrid pointsGrid;

	private TaskState task;

	[SerializeField]
	private UISprite NeedMagicIcon;

	[SerializeField]
	private UILabel MagicLabel;

	[SerializeField]
	private GameObject levelUpEffect;

	[SerializeField]
	private GameObject levelUpEffectL;

	[SerializeField]
	private GameObject levelUpEffectR;

	[SerializeField]
	private GameObject skillRecommandGlow;

	private float m_coolDownTime;

	private bool isPublic;

	private bool isIconToGray = true;

	public bool canPress;

	public SkillItemType skillItemType;

	public bool isLock = true;

	public bool isQueLan;

	public bool isSkillUIForbid;

	public float timeNum;

	private Units m_Player;

	public bool canLevelUp;

	public int level;

	private int maxLevel = -1;

	private int skillGetPowerCount = 1;

	private float maxSkillGetPowerTime;

	private int maxSkillGetPowerCount;

	private bool isInCharge = true;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private Task taskCoroutineManager;

	private Task TaskCheckIconToGray;

	public bool isSummerSkill;

	private Vector3 _lastPos;

	private float timerSum;

	public SkillItem.CdItems CdItems1
	{
		get
		{
			return this.m_cdItems;
		}
		set
		{
			this.m_cdItems = value;
		}
	}

	private void Awake()
	{
		this.m_cdItems.FindAll();
		this.SetSkillPivot(Singleton<SkillView>.Instance.GetSkillPanelPivot());
	}

	public void SetSkillPivot(SkillPanelPivot skillPanelPivot)
	{
		if (skillPanelPivot == SkillPanelPivot.Bottom)
		{
			this.levelUpBtn = this.levelUpBtn_b;
			this.levelUpBtn_r.gameObject.SetActive(false);
			this.levelUpBtn_l.gameObject.SetActive(false);
			this.pointsGrid.transform.localPosition = Vector3.zero;
			this.MagicLabel.transform.parent.localPosition = new Vector3(39f, -55f, 0f);
			this.MagicLabel.transform.parent.GetComponent<UISprite>().flip = UISprite.Flip.Nothing;
		}
		else if (skillPanelPivot == SkillPanelPivot.Left)
		{
			this.levelUpBtn = this.levelUpBtn_l;
			this.levelUpBtn_r.gameObject.SetActive(false);
			this.levelUpBtn_b.gameObject.SetActive(false);
			this.pointsGrid.transform.localPosition = new Vector3(0f, 21f, 0f);
			this.MagicLabel.transform.parent.localPosition = new Vector3(39f, 52f, 0f);
			this.MagicLabel.transform.parent.GetComponent<UISprite>().flip = UISprite.Flip.Horizontally;
		}
		else if (skillPanelPivot == SkillPanelPivot.Right)
		{
			this.levelUpBtn = this.levelUpBtn_r;
			this.levelUpBtn_l.gameObject.SetActive(false);
			this.levelUpBtn_b.gameObject.SetActive(false);
			this.pointsGrid.transform.localPosition = new Vector3(0f, 21f, 0f);
			this.MagicLabel.transform.parent.localPosition = new Vector3(39f, 52f, 0f);
			this.MagicLabel.transform.parent.GetComponent<UISprite>().flip = UISprite.Flip.Horizontally;
		}
		UIEventListener.Get(this.levelUpBtn).onClick = new UIEventListener.VoidDelegate(this.OnClickLevelUpBtn);
		if (NewbieManager.Instance.IsForbidShowSkillLevelUp())
		{
			if (this.levelUpBtn_b != null)
			{
				this.levelUpBtn_b.SetActive(false);
			}
			if (this.levelUpBtn_l != null)
			{
				this.levelUpBtn_l.SetActive(false);
			}
			if (this.levelUpBtn_r != null)
			{
				this.levelUpBtn_r.SetActive(false);
			}
		}
	}

	public void SetCdParent(UIPanel panel)
	{
		this.m_cdItems.cdRoot.parent = panel.transform;
		this.m_cdItems.cdRoot.position = base.transform.position;
	}

	public void SetLevelUpToCdParent(UIPanel panel)
	{
		this.levelUp.transform.parent = panel.transform;
		this.levelUp.transform.position = base.transform.position;
		this.levelUp.transform.localPosition += Vector3.up * 140f;
	}

	public void SetSkillType(SkillItemType type)
	{
		this.skillItemType = type;
		if (type == SkillItemType.passive)
		{
			this.ChangeIconToGrayByCanUse(true);
		}
	}

	public void SetLeveupBtn(bool isShow)
	{
		this.levelUp.gameObject.SetActive(isShow);
	}

	public SkillItemType GetSkillType()
	{
		return this.skillItemType;
	}

	public void ChangeIconToGrayByCanUse(bool canUse)
	{
		if (this.isLock)
		{
			return;
		}
		if (this.m_Player == null)
		{
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
		}
		if (this.isIconToGray == canUse)
		{
			return;
		}
		if (!canUse && this.timeNum <= 0f && this.m_Player.isLive)
		{
			if (this.TaskCheckIconToGray != null)
			{
				this.TaskCheckIconToGray.Stop();
				this.TaskCheckIconToGray = null;
			}
			this.isIconToGray = canUse;
			this.TaskCheckIconToGray = this.coroutineManager.StartCoroutine(this.CheckIconToGray(), true);
		}
		if (this.m_Player != null)
		{
			this.isIconToGray = canUse;
			if (canUse && this.m_Player.isLive)
			{
				if (this.skillItemType == SkillItemType.passive)
				{
					this.m_textureIcon1.color = new Color(0.7f, 0.7f, 0.7f, 1f);
					this.m_spriteIcon1.color = new Color(0.7f, 0.7f, 0.7f, 1f);
					this.SkillStateFX.gameObject.SetActive(true);
				}
				else
				{
					this.m_textureIcon1.color = new Color(1f, 1f, 1f, 1f);
					this.m_spriteIcon1.color = new Color(1f, 1f, 1f, 1f);
					this.SkillStateFX.gameObject.SetActive(false);
				}
			}
			else
			{
				this.m_textureIcon1.color = new Color(0.392f, 0.392f, 0.392f, 1f);
				this.m_spriteIcon1.color = new Color(0.392f, 0.392f, 0.392f, 1f);
				this.SkillStateFX.gameObject.SetActive(false);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator CheckIconToGray()
	{
		SkillItem.<CheckIconToGray>c__IteratorEA <CheckIconToGray>c__IteratorEA = new SkillItem.<CheckIconToGray>c__IteratorEA();
		<CheckIconToGray>c__IteratorEA.<>f__this = this;
		return <CheckIconToGray>c__IteratorEA;
	}

	public void ExternalCall()
	{
		this.OnClickLevelUpBtn(base.gameObject);
	}

	public void UpdateSkillStateFX()
	{
		if (this.skillItemType == SkillItemType.passive && !this.SkillStateFX.gameObject.activeInHierarchy)
		{
			this.SkillStateFX.gameObject.SetActive(true);
		}
	}

	private void OnClickLevelUpBtn(GameObject go)
	{
		if (Singleton<SkillView>.Instance.CanAddLevel())
		{
			this.AddLevel();
		}
		NewbieManager.Instance.TryHideEleBatFiveLearnSkillHint();
	}

	public void AddLevel()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			PvpEvent.SendSkillLevelUp(this.saveName);
		}
	}

	public void AddLevelGetCallBack(int level)
	{
		this.level = level;
		this.UpdateLevelIcon(true);
		Singleton<SkillView>.Instance.UseSkillPoint(1, this.saveName, this.level);
		if (this.skillItemType == SkillItemType.passive)
		{
			Singleton<SkillView>.Instance.UpdateSkillStateFX();
		}
		PerfTools.SetVisible(this.m_unlockAnim.transform, true);
		this.coroutineManager.StartCoroutine(this.DelayHideUnlock(), true);
		AudioMgr.PlayUI("Play_UI_Skill_active", null, false, false);
	}

	[DebuggerHidden]
	private IEnumerator DelayHideUnlock()
	{
		SkillItem.<DelayHideUnlock>c__IteratorEB <DelayHideUnlock>c__IteratorEB = new SkillItem.<DelayHideUnlock>c__IteratorEB();
		<DelayHideUnlock>c__IteratorEB.<>f__this = this;
		return <DelayHideUnlock>c__IteratorEB;
	}

	public void SetMagicLabel(int costValue, bool isSummerSkill = false)
	{
		if (costValue > 0)
		{
			PerfTools.SetVisible(this.MagicLabel.parent.transform, true);
			if (isSummerSkill)
			{
				this.MagicLabel.parent.GetComponent<UISprite>().enabled = false;
				this.MagicLabel.transform.parent.localPosition = new Vector3(37f, -50f, 0f);
				this.MagicLabel.pivot = UIWidget.Pivot.BottomRight;
				this.MagicLabel.overflowMethod = UILabel.Overflow.ClampContent;
				this.MagicLabel.fontSize = 26;
				this.MagicLabel.width = 53;
				this.MagicLabel.height = 26;
			}
			else
			{
				this.MagicLabel.parent.GetComponent<UISprite>().enabled = true;
			}
			this.MagicLabel.text = costValue.ToString();
		}
		else
		{
			this.MagicLabel.text = string.Empty;
		}
	}

	public void SetLevel(int value)
	{
		this.level = value;
		this.UpdateLevelIcon(false);
		Singleton<SkillView>.Instance.UseSkillPoint(0, this.saveName, this.level);
		if (this.skillItemType == SkillItemType.passive)
		{
			Singleton<SkillView>.Instance.UpdateSkillStateFX();
		}
	}

	public int GetLevel()
	{
		return this.level;
	}

	public int GetMaxLevel()
	{
		if (this.maxLevel == -1)
		{
			this.maxLevel = BaseDataMgr.instance.GetSkillMainData(this.saveName).skill_levelmax;
		}
		return this.maxLevel;
	}

	public void ShowLevelUpEffect()
	{
		SkillPanelPivot skillPanelPivot = Singleton<SkillView>.Instance.GetSkillPanelPivot();
		if (skillPanelPivot == SkillPanelPivot.Bottom)
		{
			this.levelUpEffect.gameObject.SetActive(true);
			this.levelUpEffectL.gameObject.SetActive(false);
			this.levelUpEffectR.gameObject.SetActive(false);
		}
		else if (skillPanelPivot == SkillPanelPivot.Left)
		{
			this.levelUpEffect.gameObject.SetActive(false);
			this.levelUpEffectL.gameObject.SetActive(true);
			this.levelUpEffectR.gameObject.SetActive(false);
		}
		else if (skillPanelPivot == SkillPanelPivot.Right)
		{
			this.levelUpEffect.gameObject.SetActive(false);
			this.levelUpEffectL.gameObject.SetActive(false);
			this.levelUpEffectR.gameObject.SetActive(true);
		}
	}

	[DebuggerHidden]
	private IEnumerator HideLevelUpEffect()
	{
		SkillItem.<HideLevelUpEffect>c__IteratorEC <HideLevelUpEffect>c__IteratorEC = new SkillItem.<HideLevelUpEffect>c__IteratorEC();
		<HideLevelUpEffect>c__IteratorEC.<>f__this = this;
		return <HideLevelUpEffect>c__IteratorEC;
	}

	public void UpdateLevelIcon(bool isAdd = false)
	{
		if (isAdd)
		{
			this.ShowLevelUpEffect();
			if (this.taskCoroutineManager != null)
			{
				this.coroutineManager.StopCoroutine(this.taskCoroutineManager);
			}
			this.taskCoroutineManager = this.coroutineManager.StartCoroutine(this.HideLevelUpEffect(), true);
		}
		for (int i = 0; i < this.points.Length; i++)
		{
			if (i < this.level)
			{
				this.points[i].spriteName = "HUD_skill_dot_green";
				if (i == this.level - 1)
				{
					PerfTools.SetVisible(this.points[i].transform.GetChild(0), true);
					this.coroutineManager.StartCoroutine(this.HidePoints(i.ToString()), true);
				}
			}
			else
			{
				this.points[i].spriteName = "HUD_skill_dot_gray";
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator HidePoints(string index)
	{
		SkillItem.<HidePoints>c__IteratorED <HidePoints>c__IteratorED = new SkillItem.<HidePoints>c__IteratorED();
		<HidePoints>c__IteratorED.index = index;
		<HidePoints>c__IteratorED.<$>index = index;
		<HidePoints>c__IteratorED.<>f__this = this;
		return <HidePoints>c__IteratorED;
	}

	public void Rename(string name)
	{
		if (this.saveName == name)
		{
			return;
		}
		if (this.m_Player == null)
		{
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
		}
		this.SetActivPeMask3(false);
		base.gameObject.name = name;
		this.saveName = name;
		this.m_skillIconScript.Init();
		if (this.skillItemType == SkillItemType.passive)
		{
			this.m_skillIconScript.SetLock(true);
		}
		else
		{
			this.m_skillIconScript.SetLock(false);
		}
	}

	public void SetCanLevelUp(bool canLevel)
	{
		if (NewbieManager.Instance.IsForbidShowSkillLevelUp())
		{
			return;
		}
		this.canLevelUp = canLevel;
		this.UpdateCanLevelUp(canLevel);
	}

	private void OnApplicationFocus(bool isFocus)
	{
		if (isFocus && this.canLevelUp)
		{
			this.SetCanLevelUp(false);
			this.SetCanLevelUp(true);
		}
	}

	private void UpdateCanLevelUp(bool _canLevelUp)
	{
		if (GameManager.Instance.ReplayController.IsReplayStart)
		{
			return;
		}
		if (_canLevelUp)
		{
			this.m_state1.GetComponent<UISprite>().spriteName = "HUD_skill_select";
			this.m_state2.GetComponent<UISprite>().spriteName = "HUD_skill_select";
			if (Singleton<SkillView>.Instance.CheckSkillHelp(this.SkillItemIndex))
			{
				this.skillRecommandGlow.gameObject.SetActive(true);
			}
			else
			{
				this.skillRecommandGlow.gameObject.SetActive(false);
			}
			this.levelUpBtn.SetActive(true);
		}
		else if (this.skillItemType == SkillItemType.active)
		{
			this.m_state1.GetComponent<UISprite>().spriteName = "HUD_skill_frame";
			this.m_state2.GetComponent<UISprite>().spriteName = "HUD_skill_frame";
			this.levelUpBtn.SetActive(false);
			this.skillRecommandGlow.gameObject.SetActive(false);
		}
		else if (this.skillItemType == SkillItemType.passive)
		{
			this.m_state1.GetComponent<UISprite>().spriteName = "HUD_skill_frame_beid";
			this.m_state2.GetComponent<UISprite>().spriteName = "HUD_skill_frame_beid";
			this.levelUpBtn.SetActive(false);
			this.skillRecommandGlow.gameObject.SetActive(false);
		}
		else
		{
			this.m_state1.GetComponent<UISprite>().spriteName = "HUD_skill_frame";
			this.levelUpBtn.SetActive(false);
			this.skillRecommandGlow.gameObject.SetActive(false);
		}
	}

	public bool NewbieCheckIsCanLevelUp()
	{
		return this.canLevelUp;
	}

	public void IsShowNeedMagic(bool isShow)
	{
		if (isShow)
		{
			PerfTools.SetVisible(this.NeedMagicIcon.transform, true);
		}
		else
		{
			PerfTools.SetVisible(this.NeedMagicIcon.transform, false);
		}
	}

	public GameObject GetObjectState1()
	{
		return this.m_state1;
	}

	public GameObject GetObjectState2()
	{
		return this.m_state2;
	}

	public void SetActiveState1(bool active)
	{
		if (active)
		{
			this.isLock = false;
		}
		else
		{
			this.isLock = true;
		}
		PerfTools.SetVisible(this.m_state1.transform, active);
	}

	public void SetActiveState2(bool active)
	{
		PerfTools.SetVisible(this.m_state2.transform, active);
	}

	public void SetActiveLock(bool active)
	{
		PerfTools.SetVisible(this.m_lock.transform, active);
	}

	public void SetActiveMask(bool active)
	{
		PerfTools.SetVisible(this.m_cdItems.mask.transform, active);
	}

	public bool GetActiveMask1()
	{
		return this.m_cdItems.mask1.activeInHierarchy;
	}

	public void SetActiveMask1(bool active)
	{
		if (active && this.NeedMagicIcon.gameObject.activeInHierarchy)
		{
			return;
		}
		PerfTools.SetVisible(this.m_cdItems.mask1.transform, active);
	}

	public void SetTweenCD1(float duration, float fromValue = 1f)
	{
		if (this.NeedMagicIcon.gameObject.activeInHierarchy)
		{
			return;
		}
		this.m_cdItems.tweener1.ignoreTimeScale = false;
		this.m_cdItems.tweener1.duration = duration;
		this.m_cdItems.tweener1.from = fromValue;
		this.m_cdItems.tweener1.Begin();
	}

	public float GetTweenCD1()
	{
		return this.m_cdItems.tweener1.value;
	}

	public void SetSkillIcon(Texture texture)
	{
		this.m_textureIcon1.mainTexture = texture;
		this.m_textureIcon2.mainTexture = texture;
		PerfTools.SetVisible(this.m_spriteIcon1.transform, false);
		PerfTools.SetVisible(this.m_spriteIcon2.transform, false);
	}

	public void SetSkillSpriteIcon(string skillname, bool isSummerSkill = false)
	{
		this.isSummerSkill = isSummerSkill;
		if (isSummerSkill)
		{
			UIAtlas atlas = Resources.Load("Texture/Skiller/SummerSkill", typeof(UIAtlas)) as UIAtlas;
			this.m_spriteIcon1.atlas = atlas;
			this.m_spriteIcon2.atlas = this.m_spriteIcon1.atlas;
			this.SetSummerSkillStyle(skillname);
		}
		else
		{
			string[] array = skillname.Split(new char[]
			{
				'_'
			});
			string text = array[0] + "_" + array[1];
			UIAtlas uIAtlas = Resources.Load("Texture/Skiller/" + text, typeof(UIAtlas)) as UIAtlas;
			if (uIAtlas == null)
			{
				Texture skillIcon = ResourceManager.Load<Texture>(skillname, true, true, null, 0, false);
				this.SetSkillIcon(skillIcon);
				return;
			}
			this.m_spriteIcon1.atlas = uIAtlas;
			this.m_spriteIcon2.atlas = uIAtlas;
			HomeGCManager.Instance.SetBattlingHeroSkillIconAtlasName(text);
		}
		this.m_spriteIcon1.spriteName = skillname;
		this.m_spriteIcon2.spriteName = skillname;
		PerfTools.SetVisible(this.m_textureIcon1.transform, false);
		PerfTools.SetVisible(this.m_textureIcon2.transform, false);
	}

	public void SetSummerSkillStyle(string skillname)
	{
		if (skillname != "Summoner_Ignite" && skillname != "Summoner_Barrier" && skillname != "Summoner_Smite" && skillname != "Summoner_Heal" && skillname != "Summoner_Clarity")
		{
			return;
		}
		SysSkillLevelupVo dataById = BaseDataMgr.instance.GetDataById<SysSkillLevelupVo>(skillname);
		if (dataById == null || this.m_Player == null)
		{
			return;
		}
		float num = 0f;
		if (dataById.damage_id != "[]")
		{
			num = (float)this.GetDamageValue(dataById.damage_id);
		}
		else if (dataById.start_buff_ids != "[]")
		{
			num = (float)this.GetDamageValue(BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(dataById.start_buff_ids).damage_id);
		}
		else if (dataById.hit_buff_ids != "[]")
		{
			SysSkillBuffVo dataById2 = BaseDataMgr.instance.GetDataById<SysSkillBuffVo>(dataById.hit_buff_ids);
			float num2;
			if (dataById2.effective_time == 0f)
			{
				num2 = 0f;
			}
			else
			{
				num2 = dataById2.buff_time / dataById2.effective_time;
			}
			if (num2 < 1f)
			{
				num2 = 1f;
			}
			num = num2 * (float)this.GetDamageValue(dataById2.damage_id);
		}
		this.SetMagicLabel(Math.Abs((int)num), true);
	}

	private int GetDamageValue(string id)
	{
		float num = 0f;
		DamageData vo = Singleton<DamageDataManager>.Instance.GetVo(int.Parse(id));
		if (vo.property_percent)
		{
			AttrType property_key = vo.property_key;
			if (property_key != AttrType.Hp)
			{
				if (property_key == AttrType.Mp)
				{
					num = -vo.property_value * this.m_Player.mp_max;
				}
			}
			else
			{
				num = -vo.property_value * this.m_Player.hp_max;
			}
		}
		else
		{
			num = FormulaTool.GetFormualValue((int)vo.damageParam2, this.m_Player);
		}
		return (int)num;
	}

	public void SetFillAmount(float amount)
	{
		this.m_bgSprite.fillAmount = amount;
	}

	public void SetTweenCD(float duration, float fromValue = 1f)
	{
		this.m_cdItems.Show(true);
		this.m_cdItems.tweener.ignoreTimeScale = false;
		this.m_cdItems.tweener.onFinished.Clear();
		this.m_cdItems.tweener.onFinished.Add(new EventDelegate(delegate
		{
			this.m_cdItems.Show(false);
		}));
		this.m_cdItems.tweener.duration = duration;
		this.m_cdItems.tweener.from = fromValue;
		this.m_cdItems.tweener.Begin();
	}

	public void StartTextTimer(Units _units, float duration)
	{
		this.timeNum = duration;
		this.TextTimer();
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		if (position != this._lastPos)
		{
			this._lastPos = position;
			this.m_cdItems.cdRoot.position = position;
		}
		this.OnUpdate(Time.deltaTime);
	}

	public void OnUpdate(float deltaTime)
	{
		if (GameManager.IsPausing())
		{
			return;
		}
		if (this.timeNum > 0f)
		{
			if (!this.numText.gameObject.activeInHierarchy)
			{
				PerfTools.SetVisible(this.numText.transform, true);
			}
			this.timeNum -= deltaTime;
			this.timerSum += deltaTime;
			if (this.timeNum > 1f)
			{
				if (this.timerSum >= 0.5f)
				{
					this.timerSum = 0f;
					this.TextTimer();
				}
			}
			else if (this.timerSum >= 0.1f)
			{
				this.timerSum = 0f;
				this.TextTimer();
			}
		}
		else if (this.numText.gameObject.activeInHierarchy)
		{
			this.TextTimer();
			PerfTools.SetVisible(this.numText.transform, false);
		}
	}

	public void SetPointNum(int value)
	{
		if (value < 1)
		{
			value = 1;
		}
		else if (value > 5)
		{
			value = 5;
		}
		for (int i = 0; i < this.pointsGrid.transform.childCount; i++)
		{
			if (i < value)
			{
				PerfTools.SetVisible(this.points[i].transform, true);
			}
			else
			{
				PerfTools.SetVisible(this.points[i].transform, false);
			}
		}
		this.pointsGrid.enabled = true;
	}

	public void TextTimer()
	{
		if (this.timeNum > 0f)
		{
			if (this.timeNum < 1f)
			{
				this.numText.text = this.timeNum.ToString("0.0");
			}
			else
			{
				this.numText.text = this.timeNum.ToString("0");
			}
			return;
		}
		if (this.numText)
		{
			this.numText.text = string.Empty;
		}
		if (this.m_Player == null)
		{
			return;
		}
		this.m_Player.SetCDTime(this.saveName, 0f);
		Singleton<SkillView>.Instance.UpdateSkillActive(this.SkillItemIndex);
		if (this.m_Player.getSkillByIndex(this.SkillItemIndex) != null && Math.Abs(this.m_Player.getSkillByIndex(this.SkillItemIndex).GetCostValue(AttrType.Mp)) > this.m_Player.mp)
		{
			PerfTools.SetVisible(this.NeedMagicIcon.transform, true);
		}
		Singleton<SkillView>.Instance.CheckIconToGrayByCanUse(null, this.SkillItemIndex);
	}

	public void SetCanPress(bool canPress, string str = "")
	{
		if (this.canPress == canPress)
		{
			return;
		}
		this.canPress = canPress;
		if (this.timeNum > 0f && canPress)
		{
			return;
		}
		this.CheckCanPress();
	}

	public void CheckCanPress()
	{
		if (this.m_Player == null)
		{
			this.m_Player = PlayerControlMgr.Instance.GetPlayer();
		}
		string skillID = this.saveName;
		if (this.timeNum <= 0f && this.m_Player.CanClickSkillUI(skillID) && !this.m_Player.LockInputState && this.m_Player.isLive && !this.isQueLan && this.isInCharge)
		{
			this.canPress = true;
		}
		else
		{
			this.canPress = false;
		}
		this.m_skillIconScript.SetCanPress(this.canPress, true);
	}

	public void SetIconCanPress(bool enbale)
	{
		this.isInCharge = enbale;
		this.m_skillIconScript.SetCanPress(enbale, true);
	}

	public void SetPanelDepth(int number)
	{
	}

	public void Pause()
	{
		this.m_cdItems.tweener.Pause();
		this.m_cdItems.tweener2.Pause();
	}

	public void Resume()
	{
		this.m_cdItems.tweener.Resume();
		this.m_cdItems.tweener2.Resume();
	}

	public void SetActivPeMask3(bool active)
	{
		PerfTools.SetVisible(this.m_cdItems.mask2.transform, active);
	}

	public void UpdateChargeCD(float percent, float chargeCD)
	{
		if (!this.m_cdItems.mask2.activeInHierarchy)
		{
			PerfTools.SetVisible(this.m_cdItems.mask2.transform, true);
		}
		this.m_bgSprite2.fillAmount = percent;
		if (this.timeNum > 0f)
		{
			return;
		}
		if (chargeCD > 0f)
		{
			if (chargeCD >= 1f)
			{
				this.numText.text = Mathf.RoundToInt(chargeCD).ToString();
			}
			else
			{
				this.numText.text = Math.Round((double)chargeCD, 1).ToString();
			}
		}
		else
		{
			this.numText.text = string.Empty;
		}
	}

	public void SetChargeCount(int count)
	{
		if (count > 0)
		{
			PerfTools.SetVisible(this.m_countLabel.transform, true);
			this.m_countLabel.text = count.ToString();
		}
		else
		{
			PerfTools.SetVisible(this.m_countLabel.transform, false);
		}
	}

	public void SetChargeAlpha(float alpha)
	{
		if (!this.m_cdItems.mask2.activeInHierarchy)
		{
			PerfTools.SetVisible(this.m_cdItems.mask2.transform, true);
		}
		Color color = this.m_bgSprite2.color;
		color.a = alpha;
		this.m_bgSprite2.color = color;
	}

	public void AddSkillKey(int num)
	{
		switch (num)
		{
		case 0:
			this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.Q;
			break;
		case 1:
			this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.W;
			break;
		case 2:
			this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.E;
			break;
		case 3:
			this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.R;
			break;
		default:
			if (base.gameObject.name == "Skill_GoTown")
			{
				this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.B;
			}
			else if (base.gameObject.name == "Permanent_VisionWard")
			{
				this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.Alpha1;
			}
			else if (base.gameObject.name == "Permanent_TreatmentSkill")
			{
				this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.D;
			}
			else
			{
				this.m_state1.GetComponent<UIKeyBinding>().keyCode = KeyCode.F;
			}
			break;
		}
	}

	public void ShowTriggerBornPowerObjHint()
	{
		if (this.SkillBornPowerObjTriggerHintFX != null)
		{
			this.SkillBornPowerObjTriggerHintFX.SetActive(true);
		}
	}

	public void HideTriggerBornPowerObjHint()
	{
		if (this.SkillBornPowerObjTriggerHintFX != null)
		{
			this.SkillBornPowerObjTriggerHintFX.SetActive(false);
		}
	}
}
