using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class RItemsTool
	{
		private RecommendItem_possess func_possess;

		private RecommendItem_allPrice func_price;

		private RecommendItem_afford func_afford;

		private RecommendItem_recommend func_recommend;

		private BattleEquipTools_Tree tree;

		private BattleShopData data;

		public RItemsTool(BattleShopData data)
		{
			this.data = data;
		}

		public bool ValidTree()
		{
			return this.tree != null && null != this.tree.GetRoot();
		}

		public List<RItemData> Update_rItemsSub()
		{
			List<RItemData> result = null;
			if (this.data.RItems != null && this.data.RItems.Count > 0)
			{
				this.UpdateRItemsSub_Tree(this.data.RItems[0]);
				this.UpdateRItemsSub_possess();
				this.UpdateRItemsSub_allPrice();
				this.UpdateRItemsSub_afford();
				this.UpdateRItemsSub_recommend();
				result = (this.func_recommend.Result as List<RItemData>);
			}
			else if (this.ValidTree())
			{
				this.UpdateRItemsSub_Tree(null);
			}
			return result;
		}

		public List<RItemData> Update_rItemsSub_forMoney()
		{
			List<RItemData> result = null;
			if (this.tree != null)
			{
				this.UpdateRItemsSub_afford();
				this.UpdateRItemsSub_recommend();
				result = (this.func_recommend.Result as List<RItemData>);
			}
			return result;
		}

		private void UpdateRItemsSub_Tree(string rootItem)
		{
			if (string.IsNullOrEmpty(rootItem))
			{
				if (this.tree != null)
				{
					this.tree.DestroyTree();
				}
			}
			else
			{
				if (this.tree == null)
				{
					this.tree = new BattleEquipTools_Tree();
				}
				BEquip_node root = this.tree.GetRoot();
				if (root == null || !root.Data.ID.Equals(rootItem))
				{
					this.tree.DestroyTree();
					RItemData rItemData = new RItemData(rootItem, this.data.DicShops);
					this.tree.GenerateTree(rItemData);
				}
			}
		}

		private void UpdateRItemsSub_possess()
		{
			if (this.tree != null)
			{
				if (this.func_possess == null)
				{
					this.func_possess = new RecommendItem_possess(this.data.PItemsStr);
				}
				else
				{
					this.func_possess.Reset(this.data.PItemsStr);
				}
				this.tree.Travers_first(this.func_possess);
			}
		}

		private void UpdateRItemsSub_allPrice()
		{
			if (this.tree != null)
			{
				if (this.func_price == null)
				{
					this.func_price = new RecommendItem_allPrice(this.data.PItemsStr);
				}
				else
				{
					this.func_price.Reset(this.data.PItemsStr);
				}
				this.tree.Travers_first(this.func_price);
			}
		}

		private void UpdateRItemsSub_afford()
		{
			if (this.tree != null)
			{
				if (this.func_afford == null)
				{
					this.func_afford = new RecommendItem_afford(this.data.Money);
				}
				else
				{
					this.func_afford.Reset(this.data.Money);
				}
				this.tree.Travers_first(this.func_afford);
			}
		}

		private void UpdateRItemsSub_recommend()
		{
			if (this.tree != null)
			{
				if (this.func_recommend == null)
				{
					this.func_recommend = new RecommendItem_recommend(this.data.PItemsStr);
				}
				else
				{
					this.func_recommend.Reset(this.data.PItemsStr);
				}
				this.tree.Travers_first(this.func_recommend);
			}
		}
	}
}
