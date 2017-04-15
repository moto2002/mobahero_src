using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Model
{
	public class RecommendItem_possess : ITreeTraversCallback
	{
		private List<string> _hasItems;

		private string curTarget;

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

		public RecommendItem_possess(List<string> hasItems)
		{
			this._hasItems = new List<string>(hasItems);
			this.curTarget = string.Empty;
		}

		public void Reset(List<string> hasItems)
		{
			this.curTarget = string.Empty;
			this._hasItems.Clear();
			this._hasItems.AddRange(hasItems);
		}

		private bool OnCallback(BEquip_node node, int level)
		{
			bool result = false;
			RItemData data = node.Data;
			if (data != null)
			{
				this.curTarget = data.ID;
				data.PossessedNum = this._hasItems.Count(new Func<string, bool>(this.Selector));
				result = !data.Possessed;
			}
			return result;
		}

		private bool Selector(string target)
		{
			return this.curTarget.Equals(target);
		}
	}
}
