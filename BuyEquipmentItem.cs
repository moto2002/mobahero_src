using Com.Game.Data;
using Com.Game.Manager;
using System;
using UnityEngine;

public class BuyEquipmentItem : MonoBehaviour
{
	[SerializeField]
	private UITexture B_Texture;

	[SerializeField]
	private UISprite B_Frame;

	[SerializeField]
	private UISprite B_Mask;

	[SerializeField]
	private UILabel B_Label;

	private string equipmentID;

	private Callback<GameObject> clickCallBack;

	public Callback<GameObject> ClickCallBack
	{
		get
		{
			return this.clickCallBack;
		}
		set
		{
			this.clickCallBack = value;
		}
	}

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

	public void Init(string id)
	{
		if (id != null)
		{
			this.EquipmentID = id;
			SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(id);
			if (dataById == null)
			{
				Reminder.ReminderStr("battleItem为空id===>" + id);
				return;
			}
			this.B_Texture.mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
			this.B_Frame.gameObject.SetActive(false);
			UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickItem);
		}
	}

	private void ClickItem(GameObject obj)
	{
		if (this.ClickCallBack != null)
		{
			this.ClickCallBack(obj);
		}
	}

	public void ClickItem(bool type)
	{
		this.B_Frame.gameObject.SetActive(type);
	}
}
