using Assets.Scripts.Model;
using Com.Game.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class BattleEquip_Bottom : MonoBehaviour
	{
		private Transform btn_backLeft;

		private Transform btn_backRight;

		private BattleEquip_PItemInShop buyEquipmentItem;

		private UIGrid gridItems;

		private List<BattleEquip_PItemInShop> listCom;

		private List<string> oldItemStrs = new List<string>();

		private ItemInfo curItem;

		private List<ItemInfo> possessItems;

		private ItemInfo CurItem
		{
			get
			{
				return this.curItem;
			}
			set
			{
				if (this.curItem != value)
				{
					this.curItem = value;
					this.RefreshUI_chooseState();
				}
			}
		}

		private List<ItemInfo> PossessItems
		{
			get
			{
				if (this.possessItems == null)
				{
					this.possessItems = new List<ItemInfo>();
				}
				return this.possessItems;
			}
			set
			{
				this.possessItems = value;
				this.RefreshUI_pItems();
			}
		}

		private void Awake()
		{
			this.listCom = new List<BattleEquip_PItemInShop>();
			this.InitUI();
		}

		private void OnDestroy()
		{
		}

		private void OnEnable()
		{
			this.LoadData();
			this.Register();
		}

		private void OnDisable()
		{
			this.UnRegister();
		}

		private void Register()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23018, new MobaMessageFunc(this.OnMsg_curPossessItemChanged));
			MobaMessageManager.RegistMessage((ClientMsg)23019, new MobaMessageFunc(this.OnMsg_possessItemsChanged));
		}

		private void UnRegister()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23018, new MobaMessageFunc(this.OnMsg_curPossessItemChanged));
			MobaMessageManager.UnRegistMessage((ClientMsg)23019, new MobaMessageFunc(this.OnMsg_possessItemsChanged));
		}

		private void InitUI()
		{
			this.btn_backLeft = base.transform.Find("Left");
			this.btn_backRight = base.transform.Find("Right");
			this.buyEquipmentItem = Resources.Load<BattleEquip_PItemInShop>("Prefab/UI/BattleEquipment/BuyEquipmentItem");
			this.gridItems = base.transform.FindChild("Grid2").GetComponent<UIGrid>();
			UIEventListener.Get(this.btn_backLeft.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBack);
		}

		private void LoadData()
		{
			this.possessItems = null;
			this.curItem = null;
			this.PossessItems = ModelManager.Instance.Get_BattleShop_pItems();
			this.CurItem = ModelManager.Instance.Get_BattleShop_curPItem();
		}

		private void OnMsg_curPossessItemChanged(MobaMessage msg)
		{
			this.CurItem = (msg.Param as ItemInfo);
		}

		private void OnMsg_possessItemsChanged(MobaMessage msg)
		{
			this.PossessItems = (msg.Param as List<ItemInfo>);
		}

		private void OnClickBack(GameObject obj)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickBack, null, false);
		}

		private void OnClickItem(BattleEquip_PItemInShop com)
		{
			AudioMgr.PlayUI("Play_Shop_Select", null, false, false);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickPItem, com.ItemData, false);
		}

		private void RefreshUI_pItems()
		{
			List<ItemInfo> list = this.PossessItems;
			this.listCom.Clear();
			GridHelper.FillGrid<BattleEquip_PItemInShop>(this.gridItems, this.buyEquipmentItem, list.Count, delegate(int idx, BattleEquip_PItemInShop comp)
			{
				comp.ItemData = list[idx];
				comp.ChooseState = false;
				comp.OnClickItem = new Callback<BattleEquip_PItemInShop>(this.OnClickItem);
				comp.name = list[idx].OID.ToString();
				comp.gameObject.name = comp.name;
				comp.gameObject.SetActive(true);
				this.listCom.Add(comp);
			});
			for (int i = 0; i < this.listCom.Count; i++)
			{
				if (this.oldItemStrs.Count > i)
				{
					if (this.oldItemStrs[i] != this.listCom[i].name)
					{
						this.oldItemStrs[i] = this.listCom[i].name;
						this.listCom[i].ShowAni();
					}
				}
				else
				{
					this.oldItemStrs.Add(this.listCom[i].name);
					this.listCom[i].ShowAni();
				}
			}
			if (this.oldItemStrs.Count > this.listCom.Count)
			{
				this.oldItemStrs.RemoveRange(this.listCom.Count, this.oldItemStrs.Count - this.listCom.Count);
			}
			this.gridItems.Reposition();
		}

		private void RefreshUI_chooseState()
		{
			ItemInfo itemInfo = this.CurItem;
			foreach (BattleEquip_PItemInShop current in this.listCom)
			{
				current.ChooseState = (current.ItemData == itemInfo);
			}
		}
	}
}
