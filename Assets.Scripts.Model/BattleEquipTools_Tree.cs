using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class BattleEquipTools_Tree
	{
		private BEquip_node root;

		private Queue<BEquip_node> queue;

		public BEquip_node GetRoot()
		{
			return this.root;
		}

		public void GenerateTree(RItemData data)
		{
			if (data != null)
			{
				this.root = new BEquip_node(data, 0);
				this.root.GenerateChilren(0);
			}
		}

		public void DestroyTree()
		{
			if (this.root != null)
			{
				this.root.DestroyChildren();
				this.root = null;
			}
			if (this.queue != null)
			{
				this.queue.Clear();
				this.queue = null;
			}
		}

		public void Travers_first(ITreeTraversCallback icallback)
		{
			if (this.root != null)
			{
				this.root.Travers_first(icallback);
			}
		}

		public void Travers_level(ITreeTraversCallback iCallback)
		{
			if (this.queue == null)
			{
				this.queue = new Queue<BEquip_node>();
			}
			if (this.root != null)
			{
				this.queue.Enqueue(this.root);
			}
			while (this.queue.Count > 0)
			{
				BEquip_node bEquip_node = this.queue.Dequeue();
				if (iCallback == null || iCallback.TraversCallback(bEquip_node, bEquip_node.Level))
				{
					int childrenNum = bEquip_node.GetChildrenNum();
					for (int i = 0; i < childrenNum; i++)
					{
						BEquip_node childByIndex = bEquip_node.GetChildByIndex(i);
						if (childByIndex != null)
						{
							this.queue.Enqueue(childByIndex);
						}
					}
				}
			}
		}
	}
}
