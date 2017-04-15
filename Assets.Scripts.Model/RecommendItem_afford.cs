using System;

namespace Assets.Scripts.Model
{
	public class RecommendItem_afford : ITreeTraversCallback
	{
		private int _money;

		Func<BEquip_node, int, bool> ITreeTraversCallback.TraversCallback
		{
			get
			{
				return new Func<BEquip_node, int, bool>(this.OnCallback);
			}
		}

		object ITreeTraversCallback.Result
		{
			get
			{
				return null;
			}
		}

		public RecommendItem_afford(int money)
		{
			this._money = money;
		}

		public void Reset(int money)
		{
			this._money = money;
		}

		private bool OnCallback(BEquip_node node, int level)
		{
			RItemData data = node.Data;
			if (data != null)
			{
				data.Afford = (this._money >= data.RealPrice);
			}
			return true;
		}
	}
}
