using System;
using System.Collections.Generic;

namespace Mobaheros.AI.Equipment
{
	public class Node<T>
	{
		private Dictionary<string, Node<T>> _childrenNodes = new Dictionary<string, Node<T>>();

		public T Data
		{
			get;
			private set;
		}

		public string NodeIndex
		{
			get;
			private set;
		}

		public string NodeDescription
		{
			get;
			private set;
		}

		public Node<T> ParentNode
		{
			get;
			private set;
		}

		public Dictionary<string, Node<T>> ChildrenNodes
		{
			get
			{
				return this._childrenNodes;
			}
		}

		public Node(T data, Node<T> parent, string nodeIndex, string des = "default")
		{
			this.Data = data;
			this.ParentNode = parent;
			this.NodeDescription = des;
			this.NodeIndex = nodeIndex;
		}

		public void AddChildNode(Node<T> child)
		{
			if (!this._childrenNodes.ContainsKey(child.NodeIndex))
			{
				this._childrenNodes.Add(child.NodeIndex, child);
			}
		}

		public void RemoveChildNode(string index)
		{
			if (this._childrenNodes.ContainsKey(index))
			{
				this._childrenNodes.Remove(index);
			}
		}

		public List<Node<T>> GetChildrenNodes()
		{
			return new List<Node<T>>(this._childrenNodes.Values);
		}

		public string GetChildrenNodesDes()
		{
			string text = string.Empty;
			foreach (Node<T> current in this._childrenNodes.Values)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					current.NodeIndex,
					" : ",
					current.NodeDescription,
					" , /n"
				});
			}
			return text;
		}
	}
}
