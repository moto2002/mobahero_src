using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.PropertyView
{
	public class PropViewCollectionUnit : MonoBehaviour
	{
		[SerializeField]
		private Transform shade;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel count;

		[SerializeField]
		private UIGrid grid;

		[SerializeField]
		private UIScrollView horizontalview;

		private List<SysGameItemsVo> gameItems = new List<SysGameItemsVo>();

		private List<EquipmentInfoData> equipList = new List<EquipmentInfoData>();

		private List<int> ItemListCurrHero = new List<int>();

		private List<int> ItemListAllHero = new List<int>();

		private EffectType effectType;

		private EffectItem effectItem;

		private EffectItem currEffectItem;

		private UIScrollView verticalView;

		private bool isShadeNeed;

		private long heroID;

		public bool IsShadeNeed
		{
			get
			{
				return this.isShadeNeed;
			}
		}

		public EffectType Effecttype
		{
			get
			{
				return this.effectType;
			}
		}

		public UIScrollView Horizontalview
		{
			get
			{
				return this.horizontalview;
			}
		}

		public void Init(List<SysGameItemsVo> list, int type, UIScrollView vertical, long heroid)
		{
			this.gameItems = list;
			this.effectType = (EffectType)type;
			this.effectItem = Resources.Load<EffectItem>("Prefab/UI/Sacrificial/EffectItem");
			this.heroID = heroid;
			this.RecatchData();
			this.shade.gameObject.SetActive(this.isShadeNeed = (this.gameItems.Count > 3));
			this.verticalView = vertical;
			this.horizontalview.GetComponent<UIPanel>().depth = this.verticalView.GetComponent<UIPanel>().depth;
		}

		public void RefreshData()
		{
			this.RecatchData();
		}

		public void RefreshUnit(EquipmentInfoData equipmentinfodata, int type)
		{
			GridHelper.FillGrid<EffectItem>(this.grid, this.effectItem, this.gameItems.Count, delegate(int idx, EffectItem comp)
			{
				EquipmentInfoData eid2 = this.equipList.Find((EquipmentInfoData eid) => eid.ModelId == int.Parse(this.gameItems[idx].items_id));
				comp.Refresh(eid2, equipmentinfodata.ModelId, type);
				comp.CollectionIsUsing(this.heroID);
				comp.CheckIsCurrentHero(this.ItemListCurrHero, this.ItemListAllHero);
			});
		}

		public void RefreshClickItemState(EffectItem currEffectItem)
		{
			GridHelper.FillGrid<EffectItem>(this.grid, this.effectItem, this.gameItems.Count, delegate(int idx, EffectItem comp)
			{
				comp.Choose(false);
				comp.CheckClickState();
				comp.CollectionIsUsing(this.heroID);
				comp.CheckIsCurrentHero(this.ItemListCurrHero, this.ItemListAllHero);
			});
			currEffectItem.Choose(true);
			currEffectItem.CheckClickState();
		}

		public void SetData(bool resetPosition = true)
		{
			if (this.gameItems == null || this.gameItems.Count == 0)
			{
				return;
			}
			this.gameItems.Sort(new Comparison<SysGameItemsVo>(this.CustomSort));
			this.horizontalview.GetComponent<UIPanel>().widgetsAreStatic = false;
			GridHelper.FillGrid<EffectItem>(this.grid, this.effectItem, this.gameItems.Count, delegate(int idx, EffectItem comp)
			{
				EquipmentInfoData eid2 = this.equipList.Find((EquipmentInfoData eid) => eid.ModelId == int.Parse(this.gameItems[idx].items_id));
				comp.Init(eid2, this.gameItems[idx], this.effectType, idx, this.heroID);
				comp.GenerateScrollView(this.horizontalview, this.verticalView);
				comp.CheckIsCurrentHero(this.ItemListCurrHero, this.ItemListAllHero);
				comp.name = this.effectType.ToString() + this.gameItems[idx].items_id;
				comp.ClickCallback = new Callback<GameObject>(this.ClickItemState);
			});
			if (resetPosition)
			{
				this.grid.Reposition();
				this.horizontalview.ResetPosition();
			}
			this.horizontalview.GetComponent<UIPanel>().widgetsAreStatic = true;
		}

		private void Allocate()
		{
			PropViewCollectionUnit.<Allocate>c__AnonStorey28C <Allocate>c__AnonStorey28C = new PropViewCollectionUnit.<Allocate>c__AnonStorey28C();
			this.equipList.Clear();
			<Allocate>c__AnonStorey28C.end = ModelManager.Instance.Get_equipmentList_X();
			int i;
			for (i = 0; i < <Allocate>c__AnonStorey28C.end.Count; i++)
			{
				SysGameItemsVo sysGameItemsVo = this.gameItems.Find((SysGameItemsVo sv) => int.Parse(sv.items_id) == <Allocate>c__AnonStorey28C.end[i].ModelId);
				if (sysGameItemsVo != null && sysGameItemsVo.hero_decorate_type == (int)this.effectType)
				{
					this.equipList.Add(<Allocate>c__AnonStorey28C.end[i]);
				}
			}
			this.count.text = this.equipList.Count + "/" + this.gameItems.Count;
		}

		private void RecatchData()
		{
			this.ItemListCurrHero.Clear();
			this.ItemListAllHero.Clear();
			this.Allocate();
			HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byHeroID_X(this.heroID);
			if (heroInfoData == null)
			{
				return;
			}
			List<HeroInfoData> list = ModelManager.Instance.Get_heroInfo_list_X();
			switch (this.effectType)
			{
			case EffectType.Pet:
				if (heroInfoData.petId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.petId);
				}
				foreach (HeroInfoData current in list)
				{
					if (current.petId != 0)
					{
						this.ItemListAllHero.Add(current.petId);
					}
				}
				this.title.text = "宠物";
				break;
			case EffectType.Trail:
				if (heroInfoData.tailEffectId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.tailEffectId);
				}
				foreach (HeroInfoData current2 in list)
				{
					if (current2.tailEffectId != 0)
					{
						this.ItemListAllHero.Add(current2.tailEffectId);
					}
				}
				this.title.text = "拖尾特效";
				break;
			case EffectType.Upgrade:
				if (heroInfoData.levelEffectId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.levelEffectId);
				}
				foreach (HeroInfoData current3 in list)
				{
					if (current3.levelEffectId != 0)
					{
						this.ItemListAllHero.Add(current3.levelEffectId);
					}
				}
				this.title.text = "升级特效";
				break;
			case EffectType.TownPortal:
				if (heroInfoData.backEffectId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.backEffectId);
				}
				foreach (HeroInfoData current4 in list)
				{
					if (current4.backEffectId != 0)
					{
						this.ItemListAllHero.Add(current4.backEffectId);
					}
				}
				this.title.text = "回城特效";
				break;
			case EffectType.Respawn:
				if (heroInfoData.birthEffectId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.birthEffectId);
				}
				foreach (HeroInfoData current5 in list)
				{
					if (current5.birthEffectId != 0)
					{
						this.ItemListAllHero.Add(current5.birthEffectId);
					}
				}
				this.title.text = "出生特效";
				break;
			case EffectType.Dead:
				if (heroInfoData.deathEffectId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.deathEffectId);
				}
				foreach (HeroInfoData current6 in list)
				{
					if (current6.deathEffectId != 0)
					{
						this.ItemListAllHero.Add(current6.deathEffectId);
					}
				}
				this.title.text = "死亡特效";
				break;
			case EffectType.View:
				if (heroInfoData.eyeUnitSkinId != 0)
				{
					this.ItemListCurrHero.Add(heroInfoData.eyeUnitSkinId);
				}
				foreach (HeroInfoData current7 in list)
				{
					if (current7.eyeUnitSkinId != 0)
					{
						this.ItemListAllHero.Add(current7.eyeUnitSkinId);
					}
				}
				this.title.text = "视野单位";
				break;
			}
		}

		private int CustomSort(SysGameItemsVo x, SysGameItemsVo y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			int num = x.quality - y.quality;
			if (num > 0)
			{
				return 1;
			}
			if (num < 0)
			{
				return -1;
			}
			return x.items_id.CompareTo(y.items_id);
		}

		private void ClickItemState(GameObject obj)
		{
			if (null != obj)
			{
				long num = this.heroID;
				this.RecatchData();
				if (null == this.currEffectItem || obj.GetComponent<EffectItem>().ModelID != 0 || num != this.heroID)
				{
					this.currEffectItem = obj.GetComponent<EffectItem>();
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewClickCollectionItem, this.currEffectItem, false);
				}
			}
		}
	}
}
