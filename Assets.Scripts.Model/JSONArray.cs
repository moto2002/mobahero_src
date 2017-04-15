using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Assets.Scripts.Model
{
	public class JSONArray : JSONNode, IEnumerable
	{
		private List<JSONNode> m_List = new List<JSONNode>();

		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return this.m_List[aIndex];
			}
			set
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					this.m_List.Add(value);
				}
				else
				{
					this.m_List[aIndex] = value;
				}
			}
		}

		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				this.m_List.Add(value);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		public override IEnumerable<JSONNode> Childs
		{
			get
			{
				JSONArray.<>c__Iterator192 <>c__Iterator = new JSONArray.<>c__Iterator192();
				<>c__Iterator.<>f__this = this;
				JSONArray.<>c__Iterator192 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public override void Add(string aKey, JSONNode aItem)
		{
			this.m_List.Add(aItem);
		}

		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return null;
			}
			JSONNode result = this.m_List[aIndex];
			this.m_List.RemoveAt(aIndex);
			return result;
		}

		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		[DebuggerHidden]
		public IEnumerator GetEnumerator()
		{
			JSONArray.<GetEnumerator>c__Iterator193 <GetEnumerator>c__Iterator = new JSONArray.<GetEnumerator>c__Iterator193();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}

		public override string ToString()
		{
			string text = "[ ";
			foreach (JSONNode current in this.m_List)
			{
				if (text.Length > 2)
				{
					text += ", ";
				}
				text += current.ToString();
			}
			text += " ]";
			return text;
		}

		public override string ToString(string aPrefix)
		{
			string text = "[ ";
			foreach (JSONNode current in this.m_List)
			{
				if (text.Length > 3)
				{
					text += ", ";
				}
				text = text + "\n" + aPrefix + "   ";
				text += current.ToString(aPrefix + "   ");
			}
			text = text + "\n" + aPrefix + "]";
			return text;
		}

		public override void Serialize(BinaryWriter aWriter)
		{
			aWriter.Write(1);
			aWriter.Write(this.m_List.Count);
			for (int i = 0; i < this.m_List.Count; i++)
			{
				this.m_List[i].Serialize(aWriter);
			}
		}
	}
}
