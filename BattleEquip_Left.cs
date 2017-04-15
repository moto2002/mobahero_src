using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleEquip_Left : MonoBehaviour
{
	private UIGrid grid_menu;

	private BattleEquip_MenuItem template_menuItem;

	private Dictionary<BattleEquipType, BattleEquip_MenuItem> dict;

	private BattleEquipType curMenuType;

	private void Awake()
	{
		this.Init();
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		this.Init_data();
		this.Reigister();
		this.RefreshUI_iniItems();
	}

	private void OnDisable()
	{
		this.Unregister();
	}

	private void OnDestroy()
	{
	}

	private void Reigister()
	{
		MobaMessageManager.RegistMessage((ClientMsg)23014, new MobaMessageFunc(this.OnMsg_MenuChange));
	}

	private void Unregister()
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)23014, new MobaMessageFunc(this.OnMsg_MenuChange));
	}

	private void Init()
	{
		this.grid_menu = base.transform.FindChild("Grid").GetComponent<UIGrid>();
		this.template_menuItem = Resources.Load<BattleEquip_MenuItem>("Prefab/UI/BattleEquipment/LeftButtonItem");
		this.dict = new Dictionary<BattleEquipType, BattleEquip_MenuItem>();
	}

	private void Init_data()
	{
		this.curMenuType = ModelManager.Instance.Get_BattleShop_curMenu();
	}

	private void RefreshUI_iniItems()
	{
		int equipMenuCount = BattleEquipTools_config.GetEquipMenuCount();
		GridHelper.FillGrid<BattleEquip_MenuItem>(this.grid_menu, this.template_menuItem, equipMenuCount, delegate(int idx, BattleEquip_MenuItem comp)
		{
			BattleEquipType equipMenuTypeByIndex = BattleEquipTools_config.GetEquipMenuTypeByIndex(idx);
			comp.gameObject.SetActive(true);
			comp.MenuItemType = equipMenuTypeByIndex;
			comp.OnClick = new Action<BattleEquip_MenuItem>(this.OnClick_menuItem);
			this.dict[equipMenuTypeByIndex] = comp;
		});
		this.RefreshUI_state();
	}

	private void RefreshUI_state()
	{
		foreach (KeyValuePair<BattleEquipType, BattleEquip_MenuItem> current in this.dict)
		{
			current.Value.ChooseState = (current.Key == this.curMenuType);
		}
	}

	private void OnMsg_MenuChange(MobaMessage msg)
	{
		this.curMenuType = (BattleEquipType)((int)msg.Param);
		this.RefreshUI_state();
	}

	private void OnClick_menuItem(BattleEquip_MenuItem com)
	{
		if (null != com)
		{
			this.curMenuType = com.MenuItemType;
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickMenu, this.curMenuType, false);
		}
	}
}
