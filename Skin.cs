using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using System;
using UnityEngine;

public class Skin : MobaMono
{
	[SerializeField]
	private UITexture _uiTex;

	[SerializeField]
	private UISprite _frame;

	[SerializeField]
	private UISprite _diamondSprite;

	[SerializeField]
	private UILabel _costLabel;

	[SerializeField]
	private UISprite _moneySprite;

	[SerializeField]
	private UILabel _costLabelmoney;

	[SerializeField]
	private UILabel _useLabel;

	[SerializeField]
	private Transform costLabel;

	[SerializeField]
	private Transform Single;

	[SerializeField]
	private Transform Both;

	[SerializeField]
	private Transform Discount;

	private UIPanel _uiPanel;

	private SkinInfo _skinInfo;

	private string strOwned = string.Empty;

	private string strPlzPurchase = string.Empty;

	private string strNotOwned = string.Empty;

	private int _posIdx;

	private Vector3 _toPos;

	private Vector3 _fromPos;

	private float _fromScale;

	private float _toScale;

	private float _toAlpha;

	private float _fromAlpha;

	private TweenAlpha _ta;

	private Vector3 targetSteadyPos;

	private float targetSteadyAlpha;

	private static GameObject _prefab;

	public Vector3 toPos
	{
		get
		{
			return this._toPos;
		}
		set
		{
			this._toPos = value;
		}
	}

	public Vector3 fromPos
	{
		get
		{
			return this._fromPos;
		}
		set
		{
			this._fromPos = value;
		}
	}

	public float fromScale
	{
		get
		{
			return this._fromScale;
		}
		set
		{
			this._fromScale = value;
		}
	}

	public float toScale
	{
		get
		{
			return this._toScale;
		}
		set
		{
			this._toScale = value;
		}
	}

	public float toAlpha
	{
		get
		{
			return this._toAlpha;
		}
		set
		{
			this._toAlpha = value;
		}
	}

	public float fromAlpha
	{
		get
		{
			return this._fromAlpha;
		}
		set
		{
			this._fromAlpha = value;
		}
	}

	public int depth
	{
		get
		{
			return this._uiPanel.depth;
		}
		set
		{
			this._uiPanel.depth = value;
		}
	}

	public bool animLerping
	{
		get
		{
			return base.lsAnim.anim.isPlaying;
		}
	}

	public static GameObject prefab
	{
		get
		{
			if (Skin._prefab == null)
			{
				Skin._prefab = CachedRes.getUITemplate("skin");
			}
			return Skin._prefab;
		}
	}

	public void ClearResources()
	{
		if (this._uiTex != null && this._uiTex.mainTexture != null)
		{
			this._uiTex.mainTexture = null;
		}
		if (this._skinInfo != null)
		{
			this._skinInfo = null;
		}
	}

	public void setSkinInfo(SkinInfo skinInfo, bool isPossess, bool iswear = false)
	{
		if (!CharacterDataMgr.instance.OwenHeros.Contains(Singleton<PropertyView>.Instance.HeroNpc))
		{
			this._useLabel.text = this.strPlzPurchase;
			if (isPossess)
			{
				if (skinInfo.skinId != 0)
				{
					this._useLabel.text = this.strOwned;
				}
				else
				{
					this._useLabel.text = this.strPlzPurchase;
				}
			}
			else
			{
				this._useLabel.text = this.strNotOwned;
			}
		}
		else
		{
			this._useLabel.text = ((!isPossess) ? this.strNotOwned : this.strOwned);
		}
		this._useLabel.color = ((!(this._useLabel.text == this.strOwned)) ? Color.white : Color.green);
		float num = ((float)this._costLabel.width + (float)this._diamondSprite.width + 14f) / 2f;
		this._skinInfo = skinInfo;
		this._uiTex.mainTexture = skinInfo.tex;
		if (skinInfo.IsWear || isPossess || iswear)
		{
			return;
		}
		if (skinInfo.skinId != 0)
		{
			SysHeroSkinVo dataById = BaseDataMgr.instance.GetDataById<SysHeroSkinVo>(skinInfo.skinId.ToString());
			string[] array = dataById.source.Split(new char[]
			{
				'|'
			});
			if (0 <= array.Length && array[0].CompareTo("1") != 0)
			{
				this.Single.gameObject.SetActive(false);
				this.Both.gameObject.SetActive(false);
				this.Discount.gameObject.SetActive(false);
				return;
			}
		}
		switch (skinInfo.Container)
		{
		case 1:
		{
			this.Single.gameObject.SetActive(true);
			this.Both.gameObject.SetActive(false);
			this.Discount.gameObject.SetActive(false);
			int num2 = skinInfo.CostType;
			if (num2 != 1)
			{
				if (num2 != 2)
				{
					if (num2 == 9)
					{
						this.Single.GetChild(0).GetComponent<UISprite>().spriteName = "Home_page_icons_cap";
						this.Single.GetChild(0).GetComponent<UISprite>().SetDimensions(40, 37);
						this.Single.GetChild(1).GetComponent<UILabel>().text = skinInfo.CostBottle.ToString();
					}
				}
				else
				{
					this.Single.GetChild(0).GetComponent<UISprite>().spriteName = "icon_zuanshi";
					this.Single.GetChild(0).GetComponent<UISprite>().SetDimensions(40, 37);
					this.Single.GetChild(1).GetComponent<UILabel>().text = skinInfo.Cost.ToString();
				}
			}
			else
			{
				this.Single.GetChild(0).GetComponent<UISprite>().spriteName = "icon_gold";
				this.Single.GetChild(0).GetComponent<UISprite>().SetDimensions(35, 35);
				this.Single.GetChild(1).GetComponent<UILabel>().text = skinInfo.CostMoney.ToString();
			}
			break;
		}
		case 2:
		{
			this.Single.gameObject.SetActive(false);
			this.Both.gameObject.SetActive(true);
			this.Discount.gameObject.SetActive(false);
			int num2 = skinInfo.CostType;
			if (num2 != 1)
			{
				if (num2 != 2)
				{
					if (num2 != 9)
					{
					}
				}
				else
				{
					this.Both.GetChild(1).GetComponent<UILabel>().text = skinInfo.Cost.ToString();
				}
			}
			else
			{
				this.Both.GetChild(3).GetComponent<UILabel>().text = skinInfo.CostMoney.ToString();
			}
			num2 = skinInfo.CostTypeanother;
			if (num2 != 1)
			{
				if (num2 != 2)
				{
					if (num2 != 9)
					{
					}
				}
				else
				{
					this.Both.GetChild(1).GetComponent<UILabel>().text = skinInfo.Cost.ToString();
				}
			}
			else
			{
				this.Both.GetChild(3).GetComponent<UILabel>().text = skinInfo.CostMoney.ToString();
			}
			break;
		}
		case 3:
		{
			this.Single.gameObject.SetActive(false);
			this.Both.gameObject.SetActive(false);
			this.Discount.gameObject.SetActive(true);
			int num2 = skinInfo.CostType;
			if (num2 != 1)
			{
				if (num2 != 2)
				{
					if (num2 == 9)
					{
						this.Discount.GetChild(2).GetComponent<UISprite>().spriteName = "Home_page_icons_cap";
						this.Discount.GetChild(2).GetComponent<UISprite>().SetDimensions(40, 37);
						this.Discount.GetChild(3).GetComponent<UILabel>().text = skinInfo.CostBottle.ToString();
						this.Discount.GetChild(1).GetComponent<UILabel>().text = "原价" + skinInfo.CostBottle / skinInfo.Discount;
					}
				}
				else
				{
					this.Discount.GetChild(2).GetComponent<UISprite>().spriteName = "icon_zuanshi";
					this.Discount.GetChild(2).GetComponent<UISprite>().SetDimensions(40, 37);
					this.Discount.GetChild(3).GetComponent<UILabel>().text = skinInfo.Cost.ToString();
					this.Discount.GetChild(1).GetComponent<UILabel>().text = "原价" + skinInfo.Cost / skinInfo.Discount;
				}
			}
			else
			{
				this.Discount.GetChild(2).GetComponent<UISprite>().spriteName = "icon_gold";
				this.Discount.GetChild(2).GetComponent<UISprite>().SetDimensions(35, 35);
				this.Discount.GetChild(3).GetComponent<UILabel>().text = skinInfo.CostMoney.ToString();
				this.Discount.GetChild(1).GetComponent<UILabel>().text = "原价" + skinInfo.CostMoney / skinInfo.Discount;
			}
			break;
		}
		}
	}

	public void SetPriceActice(bool isactive)
	{
		if (isactive)
		{
			return;
		}
		NGUITools.SetActive(this.Single.gameObject, isactive);
		NGUITools.SetActive(this.Both.gameObject, isactive);
		NGUITools.SetActive(this.Discount.gameObject, isactive);
	}

	public SkinInfo getSkinInfo()
	{
		return this._skinInfo;
	}

	private void Awake()
	{
		this._uiPanel = base.GetComponent<UIPanel>();
		this.strOwned = LanguageManager.Instance.GetStringById("SummonerUI_Passport_BaseAvatar");
		this.strPlzPurchase = LanguageManager.Instance.GetStringById("SummonerUI_Passport_BaseHero");
		this.strNotOwned = LanguageManager.Instance.GetStringById("SummonerUI_Passport_HeroAvatar");
	}

	public void snap()
	{
		this._uiTex.MakePixelPerfect();
	}

	public void animToTarget(float animLength)
	{
		this.tweenPosAndScale(this._toPos, this._toScale, animLength);
		this.tweenAlpha(this._toAlpha);
	}

	public void animToOrigin(float animLength)
	{
		this.tweenPosAndScale(this._fromPos, this._fromScale, animLength);
		this.tweenAlpha(this._fromAlpha);
	}

	private void tweenAlpha(float alpha)
	{
		this._ta = base.gameObject.GetComponent<TweenAlpha>();
		this._ta.from = alpha;
		this._ta.to = alpha;
		this._ta.ResetToBeginning();
		this._ta.PlayForward();
	}

	private void tweenPosAndScale(Vector3 pos, float scale, float animLength)
	{
		if (base.lsAnim)
		{
			base.lsAnim.easyInOutPosAndScaleTo(pos, new Vector3(scale, scale, 1f), animLength);
		}
	}

	public void applyLerp(float lerp)
	{
		float num = Mathf.Abs(lerp);
		if (num > 1f || lerp == 0f)
		{
			return;
		}
		base.trans.localPosition = Vector3.Lerp(this._fromPos, this._toPos, num);
		float num2 = Mathf.Lerp(this.fromScale, this.toScale, num);
		base.trans.localScale = new Vector3(num2, num2, 1f);
		this._uiPanel.alpha = Mathf.Lerp(this._fromAlpha, this._toAlpha, num);
	}

	public void SetAlpha(float tarAlpha)
	{
		this._uiPanel.alpha = tarAlpha;
	}
}
