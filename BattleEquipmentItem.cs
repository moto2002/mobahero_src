using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class BattleEquipmentItem : MonoBehaviour
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

	private string equipmentID;

	private Callback<GameObject> callBack;

	private Callback<GameObject> doubleCallBack;

	public string EquipmentID
	{
		get
		{
			return this.equipmentID;
		}
		set
		{
			this.equipmentID = value;
		}
	}

	public Callback<GameObject> CallBack
	{
		get
		{
			return this.callBack;
		}
		set
		{
			this.callBack = value;
		}
	}

	public Callback<GameObject> DoubleCallBack
	{
		get
		{
			return this.doubleCallBack;
		}
		set
		{
			this.doubleCallBack = value;
		}
	}

	public string SellNumber
	{
		set
		{
			this.B_Number.text = value;
		}
	}

	public void Init(string id, int money, bool isCanBuy)
	{
		if (id != null)
		{
			this.equipmentID = id;
		}
		SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(id);
		this.B_Name.text = dataById.name;
		this.B_Texture.mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
		base.name = id;
		if (this.B_Frame != null)
		{
			this.B_Frame.gameObject.SetActive(false);
		}
		if (this.B_Sign != null)
		{
			this.B_Sign.gameObject.SetActive(false);
		}
		this.B_Name.color = new Color32(255, 255, 255, 255);
		this.ShowPrice(money, isCanBuy);
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
		UIEventListener.Get(base.gameObject).onDoubleClick = new UIEventListener.BoolDelegate(this.DoubleClick);
	}

	public void ShowChoseState(bool isChose)
	{
		if (this.B_Frame != null)
		{
			this.B_Frame.gameObject.SetActive(isChose);
			this.B_Name.color = ((!isChose) ? new Color32(255, 255, 255, 255) : new Color32(0, 255, 204, 255));
		}
	}

	public void ShowBuyState(bool isBuy)
	{
		if (this.B_Sign != null)
		{
			this.B_Sign.gameObject.SetActive(isBuy);
		}
	}

	public void ShowPrice(int money, bool isCanBuy)
	{
		this.B_Number.text = money.ToString();
		this.B_Mask.spriteName = ((!isCanBuy) ? "HUD_shop_frame_gray" : "HUD_shop_frame_yellow");
		this.B_Texture.material = CharacterDataMgr.instance.ReturnMaterialType((!isCanBuy) ? 9 : 1);
	}

	public void ShowOnLine(bool isRight, bool isLeft)
	{
		this.B_Right.gameObject.SetActive(isRight);
		this.B_Left.gameObject.SetActive(isLeft);
	}

	public void HideOnLine()
	{
		this.B_Right.gameObject.SetActive(false);
		this.B_Left.gameObject.SetActive(false);
	}

	private void ClickItem(GameObject obj = null)
	{
		if (this.CallBack != null)
		{
			this.ShowChoseState(true);
			this.CallBack(obj);
		}
	}

	private void DoubleClick(GameObject obj = null, bool ispress = true)
	{
		if (this.DoubleCallBack != null)
		{
			this.DoubleCallBack(obj);
		}
	}
}
