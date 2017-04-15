using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SacrificialItem : MonoBehaviour
{
	public enum HeroCardType
	{
		Non_Owned,
		Free,
		Owned
	}

	[SerializeField]
	private UILabel S_Name;

	[SerializeField]
	private Transform S_Mask;

	[SerializeField]
	private UISprite S_Frame;

	[SerializeField]
	private UISprite S_Frame_Shade;

	[SerializeField]
	private UISprite S_Leftはがねおり;

	[SerializeField]
	private UISprite S_Rightはがねおり;

	[SerializeField]
	private UILabel S_SkinOwnedCount;

	[SerializeField]
	private UILabel S_SkinTotalCount;

	[SerializeField]
	private UITexture S_HeroTexture;

	[SerializeField]
	private Transform S_TopLeft;

	[SerializeField]
	private Transform S_Center;

	private Coroutine coroutine;

	public SacrificialItem.HeroCardType heroCardType;

	public Callback<GameObject> ClickCallBack;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public void Init(string heroID, int type = 0)
	{
		switch (type)
		{
		case 0:
			this.heroCardType = SacrificialItem.HeroCardType.Non_Owned;
			break;
		case 1:
			this.heroCardType = SacrificialItem.HeroCardType.Free;
			break;
		case 2:
			this.heroCardType = SacrificialItem.HeroCardType.Owned;
			break;
		default:
			this.heroCardType = SacrificialItem.HeroCardType.Non_Owned;
			break;
		}
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(heroID);
		this.ShowCommon(heroMainData);
		if (type == 0 || type == 1)
		{
			if (type == 1)
			{
				this.S_TopLeft.gameObject.SetActive(true);
				this.S_HeroTexture.material = null;
				this.S_Frame.GetComponent<UISprite>().spriteName = "Hero_list_frame_blue";
				this.S_Name.applyGradient = true;
				this.S_Name.gradientTop = new Color(0f, 0.733333349f, 0.996078432f);
				this.S_Name.gradientBottom = new Color(0f, 0.996078432f, 0.9764706f);
				this.S_Leftはがねおり.spriteName = "Hero_list_name_triangle";
				this.S_Rightはがねおり.spriteName = "Hero_list_name_triangle";
			}
			else
			{
				this.S_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(10);
				this.S_Frame.GetComponent<UISprite>().spriteName = "Hero_list_frame_gray";
				this.S_Name.applyGradient = false;
				this.S_Name.color = new Color(0.870588243f, 0.870588243f, 0.870588243f);
				this.S_TopLeft.gameObject.SetActive(false);
				this.S_Leftはがねおり.spriteName = "Hero_list_name_triangle_gray";
				this.S_Rightはがねおり.spriteName = "Hero_list_name_triangle_gray";
			}
		}
		else if (type == 2)
		{
			this.S_TopLeft.gameObject.SetActive(false);
			this.S_HeroTexture.material = null;
			this.S_Frame.GetComponent<UISprite>().spriteName = "Hero_list_frame_blue";
			this.S_Name.applyGradient = true;
			this.S_Leftはがねおり.spriteName = "Hero_list_name_triangle";
			this.S_Rightはがねおり.spriteName = "Hero_list_name_triangle";
		}
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
		this.SetSkinCount(heroID, heroMainData.skin_id);
	}

	public void SetAlpha(float value)
	{
		if (base.GetComponent<UIWidget>())
		{
			base.GetComponent<UIWidget>().alpha = value;
		}
	}

	private void ShowCommon(SysHeroMainVo heroMain)
	{
		this.S_HeroTexture.mainTexture = ResourceManager.Load<Texture>(heroMain.Loading_icon, true, true, null, 0, false);
		this.S_HeroTexture.material = CharacterDataMgr.instance.ReturnMaterialType(1);
		this.S_Name.text = LanguageManager.Instance.GetStringById(heroMain.name);
		this.S_Name.gameObject.SetActive(true);
		this.S_Frame.gameObject.SetActive(true);
		this.S_Center.gameObject.SetActive(true);
	}

	private void ClickItem(GameObject obj = null)
	{
		if (this.ClickCallBack != null)
		{
			this.ClickCallBack(obj);
		}
	}

	public void SetSkinCount(string heroNpc, string skinStr)
	{
		int num = 0;
		int num2 = 0;
		if ("[]" != skinStr)
		{
			string[] array = skinStr.Split(new char[]
			{
				','
			});
			num = array.Length;
			if (array.Length != 0)
			{
				List<int> heroSkinList = ModelManager.Instance.GetHeroSkinList(heroNpc);
				num2 = ((heroSkinList != null) ? heroSkinList.Count : 0);
			}
		}
		this.S_SkinOwnedCount.text = ((num2 != 0) ? ("[10e100]" + num2 + "[-]") : "[009eba]0[-]");
		this.S_SkinTotalCount.text = ((num2 != num || num == 0) ? ("[009eba]/" + num + "[-]") : ("[10e100]/" + num + "[-]"));
	}

	public void SetTweenData(float delayTime)
	{
		if (this.coroutine != null)
		{
			base.StopCoroutine("PlayAnimation");
		}
		this.coroutine = base.StartCoroutine("PlayAnimation", delayTime);
	}

	public void ClearResources()
	{
		if (this.S_HeroTexture != null && this.S_HeroTexture.mainTexture != null)
		{
			this.S_HeroTexture.mainTexture = null;
		}
	}
}
