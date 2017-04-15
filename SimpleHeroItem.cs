using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class SimpleHeroItem : MonoBehaviour
{
	[SerializeField]
	private UISprite m_Frame;

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

	protected bool m_Selected;

	public Callback<SimpleHeroItem> OnChangeHeroCallback;

	public HeroData heroData = new HeroData();

	private bool useShowFrame = true;

	private bool m_changeStarDepth = true;

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

	public void Init(HeroData herosData)
	{
		this.useShowFrame = this.useShowFrame;
		this.m_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
		this.m_Frame.gameObject.SetActive(true);
		this.heroData = herosData;
		this.InitHeroItem();
		if (herosData != null)
		{
			this.ShowQuality(herosData.Quality);
			this.ShowStars(herosData.Stars);
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(herosData.HeroID);
			this.m_HeroTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
			this.m_LV.text = herosData.LV.ToString();
			base.transform.name = herosData.HeroID;
		}
		else
		{
			base.transform.name = string.Empty;
		}
		this.m_Center.gameObject.SetActive(true);
		this.m_BottomRight.gameObject.SetActive(false);
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnSelectHero);
	}

	private void InitHeroItem()
	{
		base.GetComponent<UIWidget>().alpha = 1f;
		this.ShowCardLeftOrRight(true);
		this.ShowItem(true);
		UIWidget arg_47_0 = this.m_TopRightFrame;
		Color color = new Color32(65, 65, 65, 255);
		this.m_Frame.color = color;
		arg_47_0.color = color;
		this.m_None.gameObject.SetActive(false);
	}

	private void OnSelectHero(GameObject objct_1 = null)
	{
		if (this.OnChangeHeroCallback != null)
		{
			this.OnChangeHeroCallback(this);
		}
	}

	private void ShowItem(bool type = false)
	{
		this.m_TopLeft.gameObject.SetActive(type);
		this.m_TopRight.gameObject.SetActive(type);
		this.m_Bottom.gameObject.SetActive(type);
		this.m_BottomRight.gameObject.SetActive(type);
		this.m_HeroTexture.gameObject.SetActive(type);
	}

	private void ShowCardLeftOrRight(bool obj = true)
	{
		if (!obj)
		{
			base.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.m_LV.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		else
		{
			Transform arg_6C_0 = base.transform;
			Vector3 zero = Vector3.zero;
			this.m_LV.transform.localEulerAngles = zero;
			arg_6C_0.localEulerAngles = zero;
		}
	}

	private void ShowQuality(int quality = 1)
	{
		UIWidget arg_29_0 = this.m_TopRightFrame;
		Color color = new Color32(65, 65, 65, 255);
		this.m_Frame.color = color;
		arg_29_0.color = color;
		switch (quality)
		{
		case 1:
		{
			UIWidget arg_9F_0 = this.m_TopRightFrame;
			color = new Color32(160, 160, 160, 255);
			this.m_Frame.color = color;
			arg_9F_0.color = color;
			break;
		}
		case 2:
		case 3:
		{
			UIWidget arg_D4_0 = this.m_TopRightFrame;
			color = new Color32(0, 136, 26, 255);
			this.m_Frame.color = color;
			arg_D4_0.color = color;
			this.m_TopRightBg.color = new Color32(8, 66, 0, 255);
			break;
		}
		case 4:
		case 5:
		case 6:
		{
			UIWidget arg_127_0 = this.m_TopRightFrame;
			color = new Color32(0, 107, 197, 255);
			this.m_Frame.color = color;
			arg_127_0.color = color;
			this.m_TopRightBg.color = new Color32(2, 49, 126, 255);
			break;
		}
		case 7:
		case 8:
		case 9:
		case 10:
		{
			UIWidget arg_17E_0 = this.m_TopRightFrame;
			color = new Color32(137, 0, 188, 255);
			this.m_Frame.color = color;
			arg_17E_0.color = color;
			this.m_TopRightBg.color = new Color32(84, 0, 115, 255);
			break;
		}
		case 11:
		case 12:
		{
			UIWidget arg_1D2_0 = this.m_TopRightFrame;
			color = new Color32(178, 121, 0, 255);
			this.m_Frame.color = color;
			arg_1D2_0.color = color;
			this.m_TopRightBg.color = new Color32(92, 37, 0, 255);
			break;
		}
		}
		if (quality == 1 || quality == 2 || quality == 4 || quality == 7 || quality == 11)
		{
			this.m_TopRight.transform.gameObject.SetActive(false);
		}
		else
		{
			this.m_TopRight.transform.gameObject.SetActive(true);
		}
		if (quality == 3 || quality == 5 || quality == 8 || quality == 12)
		{
			this.ShowRank(1);
		}
		else if (quality == 6 || quality == 9)
		{
			this.ShowRank(2);
		}
		else if (quality == 10)
		{
			this.ShowRank(3);
		}
	}

	private void ShowRank(int index)
	{
		int childCount = this.m_Rank.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (i < index)
			{
				this.m_Rank.transform.GetChild(i).gameObject.SetActive(true);
			}
			else
			{
				this.m_Rank.transform.GetChild(i).gameObject.SetActive(false);
			}
		}
		this.m_Rank.transform.GetChild(0).GetComponent<UISprite>().color = Color.white;
		this.m_Rank.transform.GetChild(1).GetComponent<UISprite>().color = new Color32(255, 246, 107, 255);
		this.m_Rank.transform.GetChild(2).GetComponent<UISprite>().color = new Color32(255, 215, 29, 255);
		this.m_Rank.Reposition();
	}

	private void ShowStars(int starNumber)
	{
		this.m_BottomStars.gameObject.SetActive(false);
		this.m_BottomRightStars.gameObject.SetActive(false);
	}

	private void Stars(int starNumber = 0)
	{
		int childCount = this.m_BottomStars.transform.childCount;
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
		}
		this.m_BottomStars.Reposition();
		this.m_BottomRightStars.Reposition();
	}
}
