using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class BattleEquip_ShopItem : MonoBehaviour
{
	[SerializeField]
	private UISprite B_Mask;

	[SerializeField]
	private UITexture B_Texture;

	[SerializeField]
	private UILabel B_Name;

	[SerializeField]
	private UILabel B_Number;

	[SerializeField]
	private Transform B_Frame;

	[SerializeField]
	private UISprite B_Sign;

	[SerializeField]
	private UISprite B_Right;

	[SerializeField]
	private UISprite B_Left;

	[SerializeField]
	private Animation[] animations;

	private Callback<SItemData> onClick;

	private Callback<SItemData> onDoubleClick;

	private CoroutineManager coroutineManager;

	private SItemData sItemData;

	public SItemData ItemData
	{
		get
		{
			return this.sItemData;
		}
		set
		{
			this.sItemData = value;
			this.RefreshUI_item();
			this.RefreshUI_chooseState();
			this.RefreshUI_possessState();
			this.RefreshUI_affordState();
			this.RefreshUI_price();
			this.RefreshUI_connectRoute();
		}
	}

	public float PosY
	{
		get
		{
			return base.transform.localPosition.y;
		}
	}

	public Callback<SItemData> OnClick
	{
		get;
		set;
	}

	public Callback<SItemData> OnDoubleClick
	{
		get;
		set;
	}

	private void Awake()
	{
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickItem);
		UIEventListener.Get(base.gameObject).onDoubleClick = new UIEventListener.BoolDelegate(this.OnDoubleClickItem);
	}

	private void RefreshUI_item()
	{
		if (this.ItemData != null)
		{
			this.B_Name.text = LanguageManager.Instance.GetStringById(this.ItemData.Config.name);
			this.B_Texture.mainTexture = ResourceManager.Load<Texture>(this.ItemData.Config.icon, true, true, null, 0, false);
			this.B_Name.color = new Color32(255, 255, 255, 255);
			base.name = this.ItemData.ID;
		}
	}

	private void RefreshUI_chooseState()
	{
		this.B_Frame.gameObject.SetActive(this.ItemData != null && this.ItemData.ChooseState);
		this.B_Name.color = ((this.ItemData == null || !this.ItemData.ChooseState) ? new Color32(255, 255, 255, 255) : new Color32(0, 255, 204, 255));
	}

	private void RefreshUI_possessState()
	{
		this.B_Sign.gameObject.SetActive(this.ItemData != null && this.ItemData.Possessed);
	}

	private void RefreshUI_price()
	{
		this.B_Number.text = ((this.ItemData == null) ? string.Empty : this.ItemData.RealPrice.ToString());
	}

	private void RefreshUI_affordState()
	{
		bool flag = this.ItemData != null && this.ItemData.Afford;
		this.B_Mask.spriteName = ((!flag) ? "HUD_shop_frame_gray" : "HUD_shop_frame_yellow");
		this.B_Texture.material = CharacterDataMgr.instance.ReturnMaterialType((!flag) ? 9 : 1);
	}

	private void RefreshUI_connectRoute()
	{
		this.B_Left.gameObject.SetActive(this.ItemData != null && this.ItemData.LConnect);
		this.B_Right.gameObject.SetActive(this.ItemData != null && this.ItemData.HConnect);
	}

	private void OnClickItem(GameObject obj = null)
	{
		AudioMgr.PlayUI("Play_Shop_Select", null, false, false);
		if (this.OnClick != null)
		{
			this.OnClick(this.ItemData);
		}
	}

	private void OnDoubleClickItem(GameObject obj = null, bool state = false)
	{
		if (this.OnDoubleClick != null)
		{
			this.OnDoubleClick(this.ItemData);
			for (int i = 0; i < this.animations.Length; i++)
			{
				this.animations[i].Play();
				if (this.coroutineManager == null)
				{
					this.coroutineManager = new CoroutineManager();
				}
				this.coroutineManager.StartCoroutine(this.ReSetScale(this.animations[i].transform), true);
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator ReSetScale(Transform tra)
	{
		BattleEquip_ShopItem.<ReSetScale>c__IteratorFA <ReSetScale>c__IteratorFA = new BattleEquip_ShopItem.<ReSetScale>c__IteratorFA();
		<ReSetScale>c__IteratorFA.tra = tra;
		<ReSetScale>c__IteratorFA.<$>tra = tra;
		return <ReSetScale>c__IteratorFA;
	}
}
