using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class BottleBookView : BaseView<BottleBookView>
	{
		private Transform BackBtn;

		private Transform BookList;

		private UIPanel m_scrollView;

		private BottleBookItem _bottleBookItem;

		private List<ItemBoookData> bookItems = new List<ItemBoookData>();

		private List<BottleBookItem> _bookItemLst = new List<BottleBookItem>();

		private CoroutineManager cMgr = new CoroutineManager();

		private Task corTask;

		private float itemOffY = 342f;

		private float itemOffX = 280f;

		private int oldIndex = -1;

		private int itemSize;

		private BetterList<Transform> mChildren = new BetterList<Transform>();

		public BottleBookView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Bottle/BottleBook");
		}

		public override void Init()
		{
			this.BackBtn = this.transform.Find("TopAnchor/BackBtn");
			this.m_scrollView = this.transform.Find("Scroll View").GetComponent<UIPanel>();
			this.BookList = this.transform.Find("Scroll View/Grid");
			this._bottleBookItem = Resources.Load<BottleBookItem>("Prefab/UI/Bottle/BookItems");
			UIEventListener.Get(this.BackBtn.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseBottleBookView);
			this.m_scrollView.onClipMove = new UIPanel.OnClippingMoved(this.OnClipMove);
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			MobaMessageManagerTools.EndWaiting_manual("WaittingForBottleBook");
			if (this.bookItems == null || this.bookItems.Count == 0)
			{
				this.CreateBookList();
			}
			else
			{
				this.FillTheBookList(this.bookItems, 4);
			}
			base.HandleAfterOpenView();
		}

		public override void HandleBeforeCloseView()
		{
			this.cMgr.StopCoroutine(this.corTask);
			this.oldIndex = -1;
			this._bookItemLst.Clear();
			this.mChildren.Clear();
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void CloseBottleBookView(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.BottleBookView);
		}

		private void OnClipMove(UIPanel panel)
		{
			this.WrapContent();
			if (this.bookItems == null || this.bookItems.Count == 0 || this._bookItemLst.Count == this.bookItems.Count)
			{
				return;
			}
			this.FillTheBookList(this.bookItems, this.CheckIndex(panel.clipOffset.y));
		}

		private int CheckIndex(float offY)
		{
			if (offY > 1f)
			{
				return 0;
			}
			int num = -(int)((offY + 134f - 6f * this.itemOffY) / this.itemOffY);
			if (num < 0)
			{
				num = 0;
			}
			if (this.bookItems != null && num > (this.bookItems.Count - 18) / 6)
			{
				num = (this.bookItems.Count - 18) / 6;
			}
			return num;
		}

		private void CreateBookList()
		{
			Dictionary<string, SysSummonersHeadportraitVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysSummonersHeadportraitVo>();
			foreach (SysSummonersHeadportraitVo current in typeDicByType.Values)
			{
				if (current.headportrait_output == 5)
				{
					ItemBoookData itemBoookData = new ItemBoookData();
					itemBoookData._icon = current.headportrait_icon;
					itemBoookData._name = current.headportrait_name;
					itemBoookData._quality = current.headportrait_quality;
					itemBoookData._bottleBookType = BookItemType.HeadPortrait;
					itemBoookData._itemType = ItemType.HeadPortrait;
					this.bookItems.Add(itemBoookData);
				}
			}
			Dictionary<string, SysGameItemsVo> typeDicByType2 = BaseDataMgr.instance.GetTypeDicByType<SysGameItemsVo>();
			foreach (SysGameItemsVo current2 in typeDicByType2.Values)
			{
				if (current2.hero_decorate_type >= 1 && current2.hero_decorate_type <= 6)
				{
					SysCustomizationVo dataById = BaseDataMgr.instance.GetDataById<SysCustomizationVo>(current2.items_id);
					if (dataById.customization_source.Equals("2"))
					{
						ItemBoookData itemBoookData2 = new ItemBoookData();
						itemBoookData2._icon = current2.icon;
						itemBoookData2._name = current2.name;
						itemBoookData2._quality = current2.quality;
						itemBoookData2._itemType = ItemType.NormalGameItem;
						itemBoookData2._bottleBookType = this.Hero_Decorate_Type(current2.hero_decorate_type);
						this.bookItems.Add(itemBoookData2);
					}
				}
			}
			Dictionary<string, SysHeroSkinVo> typeDicByType3 = BaseDataMgr.instance.GetTypeDicByType<SysHeroSkinVo>();
			foreach (SysHeroSkinVo current3 in typeDicByType3.Values)
			{
				if (current3.source == "2")
				{
					ItemBoookData itemBoookData3 = new ItemBoookData();
					itemBoookData3._icon = current3.avatar_icon;
					itemBoookData3._name = current3.name;
					itemBoookData3._quality = current3.quality;
					itemBoookData3._itemType = ItemType.HeroSkin;
					itemBoookData3._bottleBookType = BookItemType.HeroSkin;
					this.bookItems.Add(itemBoookData3);
				}
			}
			this.bookItems = (from item in this.bookItems
			orderby item._quality descending, item._bottleBookType
			select item).ToList<ItemBoookData>();
			this.itemSize = this.bookItems.Count;
			this.FillTheBookList(this.bookItems, 4);
			this.corTask = this.cMgr.StartCoroutine(this.SetPanelState(), true);
		}

		private BookItemType Hero_Decorate_Type(int type)
		{
			switch (type)
			{
			case 1:
				return BookItemType.Pets;
			case 2:
				return BookItemType.Trailing;
			case 3:
				return BookItemType.LevelUp;
			case 4:
				return BookItemType.TownPortal;
			case 5:
				return BookItemType.Birth;
			case 6:
				return BookItemType.Death;
			default:
				return BookItemType.None;
			}
		}

		private void FillTheBookList(List<ItemBoookData> Lst, int col)
		{
			int num = col * 6;
			if (this.oldIndex == num)
			{
				return;
			}
			for (int i = this._bookItemLst.Count; i < num; i++)
			{
				if (Lst != null && i >= Lst.Count)
				{
					break;
				}
				if (i >= this._bookItemLst.Count)
				{
					GameObject gameObject = NGUITools.AddChild(this.BookList.gameObject, this._bottleBookItem.gameObject);
					gameObject.name = i.ToString();
					BottleBookItem component = gameObject.GetComponent<BottleBookItem>();
					this._bookItemLst.Add(this._bottleBookItem);
					gameObject.transform.localPosition = new Vector3((float)(i % 6) * this.itemOffX, (float)(-(float)i / 6) * this.itemOffY, 0f);
					component.ShowPic(Lst[i]);
					component.ShowName(Lst[i]._name, Lst[i]._bottleBookType);
					this.mChildren.Add(gameObject.transform);
				}
			}
			this.oldIndex = num;
		}

		[DebuggerHidden]
		private IEnumerator SetPanelState()
		{
			BottleBookView.<SetPanelState>c__Iterator114 <SetPanelState>c__Iterator = new BottleBookView.<SetPanelState>c__Iterator114();
			<SetPanelState>c__Iterator.<>f__this = this;
			return <SetPanelState>c__Iterator;
		}

		public void WrapContent()
		{
			Vector3[] worldCorners = this.m_scrollView.worldCorners;
			for (int i = 0; i < 4; i++)
			{
				Vector3 vector = worldCorners[i];
				vector = this.BookList.InverseTransformPoint(vector);
				worldCorners[i] = vector;
			}
			Vector3 vector2 = Vector3.Lerp(worldCorners[0], worldCorners[2], 0.5f);
			float num = worldCorners[0].y - (float)this.itemSize + this.itemOffY;
			float num2 = worldCorners[2].y + (float)this.itemSize + this.itemOffY;
			for (int j = 0; j < this.mChildren.size; j++)
			{
				Transform transform = this.mChildren[j];
				float num3 = transform.localPosition.y - vector2.y;
				num3 += this.m_scrollView.clipOffset.y - this.BookList.localPosition.y;
				if (!UICamera.IsPressed(transform.gameObject))
				{
					NGUITools.SetActive(transform.gameObject, num3 > num && num3 < num2, false);
				}
			}
		}
	}
}
