using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class BEquip_node
	{
		private int _level;

		private RItemData data;

		private List<BEquip_node> children;

		public RItemData Data
		{
			get
			{
				return this.data;
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
		}

		public BEquip_node(RItemData d, int level)
		{
			this.data = d;
			this._level = level;
		}

		public int GetChildrenNum()
		{
			return (this.children == null) ? 0 : this.children.Count;
		}

		public BEquip_node GetChildByIndex(int n)
		{
			BEquip_node result = null;
			if (this.children != null && n >= 0 && n < this.children.Count)
			{
				result = this.children[n];
			}
			return result;
		}

		public void AddChild(BEquip_node newChild)
		{
			if (newChild != null)
			{
				if (this.children == null)
				{
					this.children = new List<BEquip_node>();
				}
				this.children.Add(newChild);
			}
		}

		public void GenerateChilren(int depth)
		{
			if (depth > 3)
			{
				return;
			}
			List<RItemData> list = this.data.GenerateChilren(this._level + 1);
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] != null)
					{
						BEquip_node bEquip_node = new BEquip_node(list[i], this._level + 1);
						this.AddChild(bEquip_node);
						bEquip_node.GenerateChilren(depth + 1);
					}
				}
			}
		}

		public void DestroyChildren()
		{
			if (this.children != null)
			{
				foreach (BEquip_node current in this.children)
				{
					if (current != null)
					{
						current.DestroyChildren();
					}
				}
				this.children.Clear();
				this.children = null;
			}
		}

		public void Travers_first(ITreeTraversCallback iCallbck)
		{
			if (iCallbck == null || iCallbck.TraversCallback(this, this._level))
			{
				int num = (this.children == null) ? 0 : this.children.Count;
				for (int i = 0; i < num; i++)
				{
					if (this.children[i] != null)
					{
						this.children[i].Travers_first(iCallbck);
					}
				}
			}
		}
	}
}
