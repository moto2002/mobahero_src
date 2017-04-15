using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using Common;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using Newbie;
using System;
using UnityEngine;

public class NewHeroItem : MonoBehaviour
{
	public enum CardType
	{
		None,
		Lock,
		HeroAvator,
		HeroCardLeft,
		HeroCardRight,
		NullObject
	}

	[SerializeField]
	private UISlider m_HpBar;

	[SerializeField]
	private UISprite m_Frame;

	[SerializeField]
	private UISprite m_Mask;

	[SerializeField]
	private UILabel m_LV;

	[SerializeField]
	private UIGrid m_Rank;

	[SerializeField]
	private UISprite m_TopRightBg;

	[SerializeField]
	private UISprite m_TopRightFrame;

	[SerializeField]
	private UIGrid m_BottomStars;

	[SerializeField]
	private UIGrid m_BottomRightStars;

	[SerializeField]
	private UITexture m_HeroTexture;

	[SerializeField]
	private Transform m_TopLeft;

	[SerializeField]
	private Transform m_TopRight;

	[SerializeField]
	private Transform m_Bottom;

	[SerializeField]
	private Transform m_BottomRight;

	[SerializeField]
	private Transform m_Center;

	[SerializeField]
	private Transform m_None;

	[SerializeField]
	private UILabel m_DeadthTip;

	[SerializeField]
	private Transform m_CenterCanChange;

	[SerializeField]
	private UILabel m_CenterCanChangeLabel;

	[SerializeField]
	private Transform m_BlackBG;

	[SerializeField]
	private UILabel m_CustomName;

	[SerializeField]
	private Transform m_BottomLeft;

	[SerializeField]
	private Transform m_FreeSign;

	[SerializeField]
	private Transform m_UnConnect;

	[SerializeField]
	private Transform m_SwitchHeroBtn;

	[SerializeField]
	private Transform m_SwitchingHeroHintPic;

	[SerializeField]
	private UISprite Forrbide;

	protected bool m_Selected;

	public Callback<NewHeroItem> OnChangeHeroCallback;

	public NewHeroItem.CardType cardTypeRecord = NewHeroItem.CardType.Lock;

	public HeroData heroData = new HeroData();

	private bool useShowFrame = true;

	private bool m_changeStarDepth = true;

	private bool _hideEnemy;

	private int _bindedNewUid;

	public bool CanSelect = true;

	private bool _showAbsent;

	private Transform _absentMark;

	public string HeroLV
	{
		get
		{
			return this.m_LV.text;
		}
		set
		{
			this.m_LV.text = value;
		}
	}

	public UITexture HeroTexture
	{
		get
		{
			return this.m_HeroTexture;
		}
	}

	public Transform BlackBG
	{
		get
		{
			return this.m_BlackBG;
		}
	}

	private void Awake()
	{
		this._absentMark = base.transform.Find("IsPresent");
	}

	private void ShowAbsent(ReadyPlayerSampleInfo data, NewHeroItem.CardType cardType = NewHeroItem.CardType.Lock, bool isSelf = false)
	{
		this._showAbsent = false;
		if (data != null && (cardType == NewHeroItem.CardType.HeroCardLeft || (cardType == NewHeroItem.CardType.HeroCardRight && !this._hideEnemy)))
		{
			this._showAbsent = !data.IsReadySelectHero;
			if (isSelf)
			{
				this._showAbsent = false;
			}
		}
		Transform transform = base.transform.Find("IsPresent");
		if (transform)
		{
			transform.gameObject.SetActive(this._showAbsent);
		}
		else
		{
			ClientLogger.Error("cannot found IsPresent");
		}
	}

	public void InitPVP(ReadyPlayerSampleInfo data, NewHeroItem.CardType cardType = NewHeroItem.CardType.Lock, bool isSelf = false)
	{
		SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
		if (dataById == null)
		{
			return;
		}
		this._hideEnemy = (dataById.show_enemy_squad == 0);
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			this._hideEnemy = false;
		}
		this.ShowAbsent(data, cardType, isSelf);
		HeroData herosData = null;
		if (this.m_UnConnect == null && this.m_CenterCanChange == null)
		{
			return;
		}
		if (data == null || data.heroInfo == null)
		{
			this.m_UnConnect.gameObject.SetActive(true);
			this.m_CenterCanChange.gameObject.SetActive(false);
		}
		if (data != null && data.heroInfo != null && !string.IsNullOrEmpty(data.GetHeroId()))
		{
			herosData = new HeroData
			{
				HeroID = data.GetHeroId(),
				Stars = data.heroInfo.star,
				Quality = data.heroInfo.quality,
				LV = CharacterDataMgr.instance.GetLevel(data.heroInfo.exp)
			};
		}
		this.m_UnConnect.gameObject.SetActive(false);
		this.m_CustomName.text = string.Empty;
		if (cardType == NewHeroItem.CardType.Lock)
		{
			this.Init(null, cardType, true, true);
		}
		else if (cardType == NewHeroItem.CardType.HeroCardLeft)
		{
			this.m_CustomName.text = data.userName;
			if (isSelf)
			{
				this.m_CustomName.color = new Color(0.996078432f, 0.772549033f, 0.003921569f);
			}
			if (data.CharmRankvalue <= 50)
			{
				this.m_CustomName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(data.CharmRankvalue);
			}
			this.Init(herosData, NewHeroItem.CardType.HeroCardLeft, true, true);
			this.m_CenterCanChangeLabel.text = ((!isSelf) ? "?" : LanguageManager.Instance.GetStringById("PVPChooseHeroUI_ChooseHeroHelp"));
			this.m_CenterCanChange.gameObject.SetActive(true);
			this.m_CustomName.GetComponent<UIWidget>().rawPivot = UIWidget.Pivot.Left;
			Transform arg_224_0 = this.m_CustomName.transform;
			Vector3 localEulerAngles = Vector3.one;
			this.m_CenterCanChangeLabel.transform.localEulerAngles = localEulerAngles;
			arg_224_0.localEulerAngles = localEulerAngles;
			this.m_BottomLeft.gameObject.SetActive(true);
		}
		else if (cardType == NewHeroItem.CardType.HeroCardRight)
		{
			if (this._hideEnemy)
			{
				this.m_CustomName.text = LanguageManager.Instance.GetStringById("SummonerUI_Title_Summoner") + (base.transform.GetSiblingIndex() + 1);
				this.Init(herosData, NewHeroItem.CardType.HeroCardRight, true, true);
				if (this.m_CenterCanChangeLabel.text != "?")
				{
					this.m_CenterCanChangeLabel.text = "?";
				}
				this.m_HeroTexture.enabled = false;
				this.m_CenterCanChange.gameObject.SetActive(true);
				this.m_CustomName.GetComponent<UIWidget>().rawPivot = UIWidget.Pivot.Right;
				Transform arg_316_0 = this.m_CustomName.transform;
				Vector3 localEulerAngles = new Vector3(0f, 180f, 0f);
				this.m_CenterCanChangeLabel.transform.localEulerAngles = localEulerAngles;
				arg_316_0.localEulerAngles = localEulerAngles;
				this.m_BottomLeft.gameObject.SetActive(true);
			}
			else
			{
				this.m_CustomName.text = data.userName;
				if (data.CharmRankvalue <= 50)
				{
					this.m_CustomName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(data.CharmRankvalue);
				}
				this.Init(herosData, NewHeroItem.CardType.HeroCardRight, true, true);
				this.m_CenterCanChangeLabel.text = ((!isSelf) ? "?" : LanguageManager.Instance.GetStringById("PVPChooseHeroUI_ChooseHeroHelp"));
				this.m_CenterCanChange.gameObject.SetActive(true);
				this.m_CustomName.GetComponent<UIWidget>().rawPivot = UIWidget.Pivot.Right;
				Transform arg_3F2_0 = this.m_CustomName.transform;
				Vector3 localEulerAngles = new Vector3(0f, 180f, 0f);
				this.m_CenterCanChangeLabel.transform.localEulerAngles = localEulerAngles;
				arg_3F2_0.localEulerAngles = localEulerAngles;
				this.m_BottomLeft.gameObject.SetActive(true);
			}
		}
		if (cardType == NewHeroItem.CardType.HeroCardRight || cardType == NewHeroItem.CardType.HeroCardLeft)
		{
			if (this.heroData == null)
			{
				this.m_TopLeft.gameObject.SetActive(false);
				this.m_TopRight.gameObject.SetActive(false);
			}
			if (this.heroData == null)
			{
				this.GrayHeroItem(this.heroData);
			}
		}
		this._bindedNewUid = ((data == null) ? 0 : data.newUid);
	}

	public void Init(HeroData herosData, NewHeroItem.CardType cardType = NewHeroItem.CardType.Lock, bool useShowFrame = true, bool changeStarDepth = true)
	{
		this.useShowFrame = useShowFrame;
		this.m_changeStarDepth = changeStarDepth;
		this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
		if (cardType == NewHeroItem.CardType.NullObject)
		{
			this.ShowAlphItem();
			return;
		}
		if (this.m_Frame && this.m_Frame.gameObject)
		{
			this.m_Frame.gameObject.SetActive(true);
		}
		if (this.m_CenterCanChange && this.m_CenterCanChange.gameObject)
		{
			this.m_CenterCanChange.gameObject.SetActive(false);
		}
		this.heroData = herosData;
		this.cardTypeRecord = cardType;
		this.InitHeroItem();
		if (herosData != null)
		{
			this.ShowQuality(herosData.Quality);
			this.ShowStars(cardType, herosData.Stars);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(herosData.HeroID);
			if (heroMainData == null)
			{
				ClientLogger.Error("HeroMain中查找" + herosData.HeroID + "结果为空，联系策划");
				return;
			}
			if (this.cardTypeRecord == NewHeroItem.CardType.HeroCardLeft || this.cardTypeRecord == NewHeroItem.CardType.HeroCardRight)
			{
				SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
				if (this.cardTypeRecord == NewHeroItem.CardType.HeroCardRight && dataById != null && dataById.show_enemy_squad == 0)
				{
					if (this.m_CenterCanChange && this.m_CenterCanChange.gameObject)
					{
						this.m_CenterCanChange.gameObject.SetActive(true);
					}
				}
				else
				{
					this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
					this.m_HeroTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.longAvatar_icon, true, true, null, 0, false);
				}
			}
			else
			{
				this.m_HeroTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
			}
			this.m_LV.text = herosData.LV.ToString();
			base.transform.name = herosData.HeroID;
		}
		else
		{
			base.transform.name = string.Empty;
		}
		if (this.m_Center && this.m_Center.gameObject)
		{
			this.m_Center.gameObject.SetActive(true);
		}
		switch (cardType)
		{
		case NewHeroItem.CardType.None:
			this.ShowItem(false);
			if (this.m_CenterCanChange && this.m_CenterCanChange.gameObject)
			{
				this.m_CenterCanChange.gameObject.SetActive(true);
			}
			if (this.m_Center && this.m_Center.gameObject)
			{
				this.m_Center.gameObject.SetActive(false);
			}
			break;
		case NewHeroItem.CardType.Lock:
			this.ShowItem(false);
			this.ShowFrame(0, 0, "PVP_select_hero_03");
			if (this.m_None && this.m_None.gameObject)
			{
				this.m_None.gameObject.SetActive(true);
			}
			break;
		case NewHeroItem.CardType.HeroAvator:
			if (this.m_BottomRight && this.m_BottomRight.gameObject)
			{
				this.m_BottomRight.gameObject.SetActive(false);
			}
			this.ShowFrame(218, 218, "PVP_select_hero_04");
			this.m_LV.SetActive(false);
			break;
		case NewHeroItem.CardType.HeroCardLeft:
			this.ShowFrame(420, 140, "PVP_select_hero_01");
			if (this.m_Bottom && this.m_Bottom.gameObject)
			{
				this.m_Bottom.gameObject.SetActive(false);
			}
			break;
		case NewHeroItem.CardType.HeroCardRight:
			this.ShowCardLeftOrRight(false);
			this.ShowFrame(420, 140, "PVP_select_hero_02");
			if (this.m_Bottom && this.m_Bottom.gameObject)
			{
				this.m_Bottom.gameObject.SetActive(false);
			}
			break;
		}
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSelectHero);
	}

	public void GrayHeroItem(HeroData herosData)
	{
		this.heroData = herosData;
		if (this.cardTypeRecord == NewHeroItem.CardType.HeroCardRight && BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId).show_enemy_squad == 0)
		{
			this.m_HeroTexture.gameObject.SetActive(false);
			this.m_CenterCanChange.gameObject.SetActive(true);
			this.m_CenterCanChangeLabel.text = ((!Singleton<PvpManager>.Instance.IsObserver) ? "?" : string.Empty);
		}
		else
		{
			NGUITools.SetActiveChildren(this.m_Center.gameObject, false);
			NGUITools.SetActiveChildren(base.gameObject, false);
			this.m_HeroTexture.gameObject.SetActive(true);
		}
		if (this._showAbsent && this._absentMark)
		{
			this._absentMark.gameObject.SetActive(this._showAbsent);
		}
		this.m_BottomLeft.gameObject.SetActive(true);
		this.m_Frame.gameObject.SetActive(true);
		this.m_Center.gameObject.SetActive(true);
		this.ShowQuality(1);
		if (this.heroData == null)
		{
			this.m_HeroTexture.mainTexture = null;
			this.m_CenterCanChange.gameObject.SetActive(true);
			return;
		}
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(herosData.HeroID);
		this.m_HeroTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.longAvatar_icon, true, true, null, 0, false);
		this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(9);
		Singleton<PvpSelectHeroView>.Instance.TryRefreshSwitchHeroBtnStatus();
	}

	public void SetSelectState(bool state)
	{
		this.CanSelect = !state;
		if (state)
		{
			this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(10);
			this.m_Frame.spriteName = "PVP_select_hero_03";
		}
		else
		{
			this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
			this.m_Frame.spriteName = "PVP_select_hero_04";
		}
	}

	public void ShowFreeSign(bool state)
	{
		if (state)
		{
			this.m_FreeSign.gameObject.SetActive(true);
		}
		else
		{
			this.m_FreeSign.gameObject.SetActive(false);
		}
	}

	public void ShowAlphItem()
	{
		this.m_TopLeft.gameObject.SetActive(false);
		this.m_TopRight.gameObject.SetActive(false);
		this.m_Bottom.gameObject.SetActive(false);
		this.m_BottomRight.gameObject.SetActive(false);
		this.m_Center.gameObject.SetActive(false);
		this.m_None.gameObject.SetActive(false);
		this.m_Frame.gameObject.SetActive(false);
		base.name = "null";
		base.GetComponent<UIWidget>().alpha = 0.01f;
	}

	public static GameObject GetHeroItemPrefab()
	{
		return Resources.Load("Prefab/NewUI/SelectHero/SelectHeroItem") as GameObject;
	}

	public void ShowLV(int lv)
	{
		this.m_LV.text = lv.ToString();
	}

	public void ShowHpValue(bool isShow, float val)
	{
		this.m_HpBar.gameObject.SetActive(isShow);
		this.m_HpBar.value = val;
	}

	public void ShowDeathTip(bool isShow)
	{
		this.m_DeadthTip.gameObject.SetActive(isShow);
	}

	public void ShowBlackBG(bool isShow)
	{
		this.m_BlackBG.gameObject.SetActive(isShow);
	}

	private void InitHeroItem()
	{
		base.GetComponent<UIWidget>().alpha = 1f;
		this.ShowFrame(420, 140, string.Empty);
		this.ShowCardLeftOrRight(true);
		this.ShowItem(true);
		UIWidget arg_65_0 = this.m_TopRightFrame;
		Color color = new Color32(182, 182, 182, 255);
		this.m_Frame.color = color;
		arg_65_0.color = color;
		this.m_None.gameObject.SetActive(false);
	}

	public void OnSelectHero(GameObject objct_1 = null)
	{
		if (this.OnChangeHeroCallback != null)
		{
			this.OnChangeHeroCallback(this);
			if (LevelManager.Instance.IsPvpBattleType)
			{
				Singleton<PvpSelectHeroView>.Instance.ShowHeroInfo(objct_1.name);
			}
			NewbieManager.Instance.TryHandleSelectHero();
		}
	}

	private void ShowItem(bool type = false)
	{
		this.m_TopLeft.gameObject.SetActive(type);
		this.m_HeroTexture.gameObject.SetActive(type);
	}

	private void ShowCardLeftOrRight(bool obj = true)
	{
		if (!obj)
		{
			base.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		else
		{
			base.transform.localEulerAngles = Vector3.zero;
		}
	}

	private void ShowFrame(int width, int longth, string _spriteName = "")
	{
		if (this.useShowFrame)
		{
			base.GetComponent<UIWidget>().SetDimensions(width, longth);
			if (width > 0 && longth > 0)
			{
				this.m_Frame.SetDimensions(width, longth);
			}
			if (_spriteName != string.Empty)
			{
				this.m_Frame.spriteName = _spriteName;
			}
			this.m_HeroTexture.SetDimensions(width, longth);
			this.m_Mask.SetDimensions(width, longth);
		}
	}

	private void ShowQuality(int quality = 1)
	{
	}

	private void ShowRank(int index)
	{
	}

	private void ShowStars(NewHeroItem.CardType cardType = NewHeroItem.CardType.Lock, int starNumber = 0)
	{
	}

	private void Stars(int starNumber = 0)
	{
		int childCount = this.m_BottomStars.transform.childCount;
		int num = 2;
		for (int i = 0; i < childCount; i++)
		{
			if (i < starNumber)
			{
				this.m_BottomStars.transform.GetChild(i).gameObject.SetActive(true);
				this.m_BottomRightStars.transform.GetChild(i).gameObject.SetActive(true);
			}
			else
			{
				this.m_BottomStars.transform.GetChild(i).gameObject.SetActive(false);
				this.m_BottomRightStars.transform.GetChild(i).gameObject.SetActive(false);
			}
			if (this.m_changeStarDepth)
			{
				this.m_BottomStars.transform.GetChild(i).GetComponent<UISprite>().depth = 5 + i;
				this.m_BottomRightStars.transform.GetChild(i).GetComponent<UISprite>().depth = 5 + i;
			}
		}
		if (starNumber % 2 != 0)
		{
			for (int j = 0; j < childCount; j++)
			{
				if (j > starNumber / 2)
				{
					this.m_BottomStars.transform.GetChild(j).GetComponent<UISprite>().depth = 5 + j - num;
					this.m_BottomRightStars.transform.GetChild(j).GetComponent<UISprite>().depth = 5 + j - num;
					num += 2;
				}
			}
		}
		this.m_BottomStars.Reposition();
		this.m_BottomRightStars.Reposition();
	}

	public void ShowWriteMask(bool sure, Color color)
	{
		if (color == Color.blue)
		{
			this.m_Mask.color = new Color32(168, 255, 255, 90);
		}
		else
		{
			this.m_Mask.color = new Color32(255, 255, 255, 120);
		}
		this.m_Mask.gameObject.SetActive(sure);
	}

	public void HideRankAndLv()
	{
		this.m_TopLeft.gameObject.SetActive(false);
		this.m_TopRight.gameObject.SetActive(false);
	}

	public void SetForrbide(bool isShow)
	{
		this.Forrbide.gameObject.SetActive(isShow);
	}

	private void TryDisplaySwitchHeroBtn(bool inIsSelf)
	{
		if (LevelManager.m_CurLevel.IsRandomSelectHero())
		{
			if (inIsSelf)
			{
				this.HideSwitchHeroBtn();
				this.HideSwitchingHeroHintPic();
			}
			else
			{
				this.DisableSwitchHeroBtn();
			}
		}
		else
		{
			this.HideSwitchHeroBtn();
			this.HideSwitchingHeroHintPic();
		}
	}

	public void EnableSwitchHeroBtn()
	{
		if (this.m_SwitchHeroBtn == null)
		{
			return;
		}
		this.m_SwitchHeroBtn.gameObject.SetActive(true);
		BoxCollider component = this.m_SwitchHeroBtn.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = true;
		}
		UISprite component2 = this.m_SwitchHeroBtn.gameObject.GetComponent<UISprite>();
		if (component2 != null)
		{
			component2.spriteName = "PVP_fighter_btn_02";
		}
	}

	public void DisableSwitchHeroBtn()
	{
		if (this.m_SwitchHeroBtn == null)
		{
			return;
		}
		this.m_SwitchHeroBtn.gameObject.SetActive(true);
		BoxCollider component = this.m_SwitchHeroBtn.gameObject.GetComponent<BoxCollider>();
		if (component != null)
		{
			component.enabled = false;
		}
		UISprite component2 = this.m_SwitchHeroBtn.gameObject.GetComponent<UISprite>();
		if (component2 != null)
		{
			component2.spriteName = "PVP_fighter_btn_03";
		}
	}

	public void HideSwitchHeroBtn()
	{
		if (this.m_SwitchHeroBtn != null)
		{
			this.m_SwitchHeroBtn.gameObject.SetActive(false);
		}
	}

	private void BindSwitchHeroBtnEventHandler()
	{
		if (this.m_SwitchHeroBtn != null)
		{
			UIEventListener.Get(this.m_SwitchHeroBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.SwitchHero);
		}
	}

	private void UnBindSwitchHeroBtnEventHandler()
	{
		if (this.m_SwitchHeroBtn != null && this.m_SwitchHeroBtn.gameObject != null)
		{
			UIEventListener.Get(this.m_SwitchHeroBtn.gameObject).onClick = null;
		}
	}

	private void SwitchHero(GameObject obj)
	{
		Singleton<PvpSelectHeroView>.Instance.SetCurNewUidSwitched(this._bindedNewUid);
		Singleton<PvpManager>.Instance.ReqSwitchHero(this._bindedNewUid);
		MobaMessageManager.DispatchMsg((ClientMsg)23068, new ParamShowSwitchHeroInfo(EShowSwitchHeroInfoType.ReqType, this._bindedNewUid), 0f);
	}

	private void ShowSwitchingHeroHintPic()
	{
		this.m_SwitchingHeroHintPic.gameObject.SetActive(true);
	}

	private void HideSwitchingHeroHintPic()
	{
		this.m_SwitchingHeroHintPic.gameObject.SetActive(false);
	}

	private void Start()
	{
		this.BindSwitchHeroBtnEventHandler();
	}

	private void OnDestroy()
	{
		this.UnBindSwitchHeroBtnEventHandler();
	}
}
