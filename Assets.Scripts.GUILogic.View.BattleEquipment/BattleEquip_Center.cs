using Assets.Scripts.Model;
using Com.Game.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.BattleEquipment
{
	public class BattleEquip_Center : MonoBehaviour
	{
		private BattleEquip_ShopItem2 template_sItem;

		private UIScrollView scrollView;

		private UISprite[] template_verLine = new UISprite[2];

		private Dictionary<ColumnType, UIGrid> dicGrid;

		private Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>> dicSItemCom;

		private Dictionary<ColumnType, Dictionary<string, SItemData>> sItems;

		private object[] msgs;

		private Dictionary<ColumnType, Dictionary<string, SItemData>> SItems
		{
			get
			{
				return this.sItems;
			}
			set
			{
				this.sItems = value;
				BattleEquipTools_config.RegularSItems(ref this.sItems, false);
			}
		}

		private SItemData CurSItem
		{
			get;
			set;
		}

		private SItemData CurSRItem
		{
			get;
			set;
		}

		private void Awake()
		{
			this.template_sItem = Resources.Load<BattleEquip_ShopItem2>("Prefab/UI/BattleEquipment/EquipmentItem2");
			this.scrollView = base.transform.FindChild("DragPanel").GetComponent<UIScrollView>();
			this.scrollView.considerInactive = false;
			this.template_verLine[0] = base.transform.Find("DragPanel/Line1/Line").GetComponent<UISprite>();
			this.template_verLine[1] = base.transform.Find("DragPanel/Line2/Line").GetComponent<UISprite>();
			this.InitMem();
			this.msgs = new object[]
			{
				ClientC2V.BattleShop_shopItemsChanged,
				ClientC2V.BattleShop_curShopItemChanged,
				ClientC2V.BattleShop_curShopItemRouteChanged,
				ClientC2V.BattleShop_possessItemsChanged,
				ClientC2V.BattleShop_walletChanged
			};
		}

		private void Start()
		{
		}

		private void OnEnable()
		{
			this.Register();
			this.InitData();
			this.RefreshUI_shopItems();
			this.RefreshUI_itemCom();
		}

		private void OnDisable()
		{
			this.Unregister();
		}

		private void OnDestroy()
		{
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void OnMsg_BattleShop_shopItemsChanged(MobaMessage msg)
		{
			this.SItems = (msg.Param as Dictionary<ColumnType, Dictionary<string, SItemData>>);
			this.RefreshUI_shopItems();
			this.RefreshUI_itemCom();
		}

		private void OnMsg_BattleShop_curShopItemChanged(MobaMessage msg)
		{
			this.CurSItem = (msg.Param as SItemData);
			this.RefreshUI_itemCom();
		}

		private void OnMsg_BattleShop_curShopItemRouteChanged(MobaMessage msg)
		{
			this.CurSRItem = (msg.Param as SItemData);
			this.RefreshUI_itemCom();
			this.RefreshUI_route();
		}

		private void OnMsg_BattleShop_possessItemsChanged(MobaMessage msg)
		{
			this.RefreshUI_itemCom();
		}

		private void OnMsg_BattleShop_walletChanged(MobaMessage msg)
		{
			this.RefreshUI_itemCom();
		}

		private void OnClickItem(SItemData item)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_clickSItem, item, false);
		}

		private void OnDoubleClickItem(SItemData item)
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.BattleShop_DClickSItem, item, false);
		}

		private void RefreshUI()
		{
		}

		private void RefreshUI_shopItems()
		{
			this.ClearCenterEquipmentDict();
			Dictionary<ColumnType, Dictionary<string, SItemData>> dictionary = this.SItems;
			Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
				ColumnType arg_53_1 = current.Key;
				KeyValuePair<ColumnType, Dictionary<string, SItemData>> current2 = enumerator.Current;
				Dictionary<string, SItemData> arg_53_2 = current2.Value;
				Dictionary<ColumnType, UIGrid> arg_4E_0 = this.dicGrid;
				KeyValuePair<ColumnType, Dictionary<string, SItemData>> current3 = enumerator.Current;
				this.FillGrid(arg_53_1, arg_53_2, arg_4E_0[current3.Key]);
			}
			this.scrollView.ResetPosition();
		}

		private void RefreshUI_itemCom()
		{
			Dictionary<ColumnType, Dictionary<string, SItemData>>.Enumerator enumerator = this.SItems.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ColumnType, Dictionary<string, SItemData>> current = enumerator.Current;
				Dictionary<string, SItemData>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>> arg_40_0 = this.dicSItemCom;
					KeyValuePair<ColumnType, Dictionary<string, SItemData>> current2 = enumerator.Current;
					if (arg_40_0.ContainsKey(current2.Key))
					{
						Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>> arg_60_0 = this.dicSItemCom;
						KeyValuePair<ColumnType, Dictionary<string, SItemData>> current3 = enumerator.Current;
						Dictionary<string, BattleEquip_ShopItem2> arg_75_0 = arg_60_0[current3.Key];
						KeyValuePair<string, SItemData> current4 = enumerator2.Current;
						if (arg_75_0.ContainsKey(current4.Key))
						{
							Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>> arg_95_0 = this.dicSItemCom;
							KeyValuePair<ColumnType, Dictionary<string, SItemData>> current5 = enumerator.Current;
							Dictionary<string, BattleEquip_ShopItem2> arg_AA_0 = arg_95_0[current5.Key];
							KeyValuePair<string, SItemData> current6 = enumerator2.Current;
							BattleEquip_ShopItem2 arg_BF_0 = arg_AA_0[current6.Key];
							KeyValuePair<string, SItemData> current7 = enumerator2.Current;
							arg_BF_0.ItemData = current7.Value;
						}
					}
				}
			}
		}

		private void RefreshUI_route()
		{
			List<List<SItemData>>[] array = ModelManager.Instance.Get_BattleShop_sections();
			this.RefreshUI_hideVerLine();
			this.RefreshUI_drawLine(array[0], this.template_verLine[0]);
			this.RefreshUI_drawLine(array[1], this.template_verLine[1]);
		}

		private void RefreshUI_drawLine(List<List<SItemData>> s, UISprite sp)
		{
			if (s == null)
			{
				return;
			}
			for (int i = 0; i < s.Count; i++)
			{
				if (s[i] != null && s[i].Count != 0)
				{
					float num = -999999f;
					float num2 = 999999f;
					for (int j = 0; j < s[i].Count; j++)
					{
						SItemData sItemData = s[i][j];
						if (sItemData != null)
						{
							ColumnType level = sItemData.Level;
							string iD = sItemData.ID;
							if (this.dicSItemCom.ContainsKey(level))
							{
								if (this.dicSItemCom[level].ContainsKey(iD))
								{
									BattleEquip_ShopItem2 battleEquip_ShopItem = this.dicSItemCom[level][iD];
									num = Mathf.Max(battleEquip_ShopItem.PosY, num);
									num2 = Mathf.Min(battleEquip_ShopItem.PosY, num2);
								}
							}
						}
					}
					this.RefreshUI_drawLine(sp, num, num2, i);
				}
			}
		}

		private void RefreshUI_drawLine(UISprite template, float max, float min, int n)
		{
			Transform parent = template.transform.parent;
			if (parent.childCount < n + 2)
			{
				NGUITools.AddChild(parent.gameObject, template.gameObject);
			}
			UISprite component = parent.GetChild(n + 1).GetComponent<UISprite>();
			component.gameObject.SetActive(true);
			float y = (max + min) / 2f;
			float num = max - min;
			component.transform.localPosition = new Vector3(component.transform.localPosition.x, y, component.transform.localPosition.z);
			component.height = (int)num;
		}

		private void RefreshUI_hideVerLine()
		{
			for (int i = 0; i < this.template_verLine.Length; i++)
			{
				Transform parent = this.template_verLine[i].transform.parent;
				for (int j = 0; j < parent.childCount; j++)
				{
					Transform child = parent.GetChild(j);
					child.gameObject.SetActive(false);
				}
			}
		}

		private void FillGrid(ColumnType type, Dictionary<string, SItemData> dic, UIGrid grid)
		{
			List<SItemData> list = dic.Values.ToList<SItemData>();
			GridHelper.FillGrid<BattleEquip_ShopItem2>(grid, this.template_sItem, (list != null) ? list.Count : 0, delegate(int idx, BattleEquip_ShopItem2 comp)
			{
				comp.ItemData = list[idx];
				comp.OnClick = new Callback<SItemData>(this.OnClickItem);
				comp.OnDoubleClick = new Callback<SItemData>(this.OnDoubleClickItem);
				comp.gameObject.SetActive(true);
				this.dicSItemCom[type][list[idx].ID] = comp;
			});
			grid.Reposition();
		}

		private void InitData()
		{
			this.SItems = ModelManager.Instance.Get_BattleShop_shopitems();
			this.CurSItem = ModelManager.Instance.Get_BattleShop_curSItem();
		}

		private void InitMem()
		{
			this.dicSItemCom = new Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>>();
			this.dicGrid = new Dictionary<ColumnType, UIGrid>();
			ColumnType[] array = (ColumnType[])Enum.GetValues(typeof(ColumnType));
			ColumnType[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ColumnType columnType = array2[i];
				this.dicSItemCom.Add(columnType, new Dictionary<string, BattleEquip_ShopItem2>());
				UIGrid component = base.transform.Find("DragPanel/Grid" + (int)columnType).GetComponent<UIGrid>();
				this.dicGrid.Add(columnType, component);
			}
		}

		private void ClearCenterEquipmentDict()
		{
			Dictionary<ColumnType, Dictionary<string, BattleEquip_ShopItem2>>.Enumerator enumerator = this.dicSItemCom.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ColumnType, Dictionary<string, BattleEquip_ShopItem2>> current = enumerator.Current;
				current.Value.Clear();
			}
		}
	}
}
