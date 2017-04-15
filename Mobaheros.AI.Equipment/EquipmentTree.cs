using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mobaheros.AI.Equipment
{
	public class EquipmentTree
	{
		private List<Node<EquipmentData>> _allNodes = new List<Node<EquipmentData>>();

		public static int GlobalIndex;

		public Units Owner;

		public string TreeName
		{
			get;
			private set;
		}

		public int TreeIndex
		{
			get;
			private set;
		}

		public List<Node<EquipmentData>> AllNodes
		{
			get
			{
				return this._allNodes;
			}
		}

		public Node<EquipmentData> RootNode
		{
			get;
			private set;
		}

		private EquipmentTree(string name, int idx)
		{
			this.TreeName = name;
			this.TreeIndex = idx;
			SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(this.TreeName);
			if (dataById == null)
			{
				Debug.LogError("AI.Equipment EquipmentTree TreeName=" + this.TreeName + "  在SysBattleItemsVo中找不到");
				return;
			}
			EquipmentData data = new EquipmentData(dataById.items_id, dataById.sale, dataById.sell);
			Node<EquipmentData> node = new Node<EquipmentData>(data, null, this.TreeName, dataById.describe);
			this.SetRootNode(node);
			this.CreateTreeNode(node);
		}

		public static EquipmentTree CreateTree(string equipmentId, Units owner)
		{
			EquipmentTree.GlobalIndex++;
			return new EquipmentTree(equipmentId, EquipmentTree.GlobalIndex)
			{
				Owner = owner
			};
		}

		private void CreateTreeNode(Node<EquipmentData> parent)
		{
			string nodeIndex = parent.NodeIndex;
			SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(nodeIndex);
			if (StringUtils.CheckValid(dataById.consumption))
			{
				string[] stringValue = StringUtils.GetStringValue(dataById.consumption, ',');
				if (stringValue != null)
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i];
						SysBattleItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(text);
						if (dataById2 != null)
						{
							EquipmentData data = new EquipmentData(dataById2.items_id, dataById2.sale, dataById2.sell);
							Node<EquipmentData> node = new Node<EquipmentData>(data, parent, text, dataById2.describe);
							parent.AddChildNode(node);
							this.AddNode(node);
							this.CreateTreeNode(node);
						}
					}
				}
			}
		}

		protected void SetRootNode(Node<EquipmentData> root)
		{
			this.RootNode = root;
			this.AddNode(root);
		}

		protected void AddNode(Node<EquipmentData> node)
		{
			if (!this._allNodes.Contains(node))
			{
				this._allNodes.Add(node);
			}
		}

		protected void RemoveNode(Node<EquipmentData> node)
		{
			if (!this._allNodes.Contains(node))
			{
				this._allNodes.Remove(node);
			}
		}

		public bool IsTreeFinished()
		{
			foreach (Node<EquipmentData> current in this._allNodes)
			{
				if (!current.Data.HaveBeenBought)
				{
					return false;
				}
			}
			return true;
		}

		public EquipConsumer BuyEquiment()
		{
			if (LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsServerZyBattleType)
			{
				return null;
			}
			this.ProcessBuying(this.RootNode);
			return this.GetConsumer();
		}

		private EquipConsumer GetConsumer()
		{
			List<ItemInfo> infos = new List<ItemInfo>();
			this.TraverseConsumer(this.RootNode, ref infos);
			return new EquipConsumer(infos, this.RootNode.Data.PosseState);
		}

		private void TraverseConsumer(Node<EquipmentData> node, ref List<ItemInfo> infos)
		{
			if (node == null)
			{
				return;
			}
			if (node.Data.PosseState)
			{
				infos.Add(node.Data.GetItemInfo());
			}
			List<Node<EquipmentData>> childrenNodes = node.GetChildrenNodes();
			if (childrenNodes == null || childrenNodes.Count == 0)
			{
				return;
			}
			foreach (Node<EquipmentData> current in childrenNodes)
			{
				this.TraverseConsumer(current, ref infos);
			}
		}

		private void ProcessBuying(Node<EquipmentData> node)
		{
			if (node.Data.HaveBeenBought)
			{
				return;
			}
			int goldById = UtilManager.Instance.GetGoldById(this.Owner.unique_id);
			int num = this.GoldNeeded2Buy(node);
			if (goldById >= num)
			{
				this.DoConsume(node, num);
				return;
			}
			List<Node<EquipmentData>> childrenNodes = node.GetChildrenNodes();
			if (childrenNodes == null || childrenNodes.Count == 0)
			{
				return;
			}
			foreach (Node<EquipmentData> current in childrenNodes)
			{
				this.ProcessBuying(current);
			}
		}

		private void DoConsume(Node<EquipmentData> node, int cost)
		{
			BattleEquipTools_op.ChangeHeroMoney(this.Owner.unique_id, -cost);
			node.Data.ChangeBoughtState(true);
			node.Data.ChangePosseState(true);
			this.MarkBoughtState(node);
		}

		private void MarkBoughtState(Node<EquipmentData> node)
		{
			List<Node<EquipmentData>> childrenNodes = node.GetChildrenNodes();
			if (childrenNodes == null || childrenNodes.Count == 0)
			{
				return;
			}
			foreach (Node<EquipmentData> current in childrenNodes)
			{
				current.Data.ChangeBoughtState(true);
				current.Data.ChangePosseState(false);
				this.MarkBoughtState(current);
			}
		}

		public int GoldNeeded2Buy(Node<EquipmentData> node)
		{
			if (node.Data.HaveBeenBought)
			{
				return 0;
			}
			List<Node<EquipmentData>> childrenNodes = node.GetChildrenNodes();
			if (childrenNodes == null || childrenNodes.Count == 0)
			{
				return node.Data.BuyCost;
			}
			int num = 0;
			foreach (Node<EquipmentData> current in childrenNodes)
			{
				int num2 = this.GoldNeeded2Buy(current);
				num += num2;
			}
			return num + node.Data.ExtraSpent;
		}
	}
}
