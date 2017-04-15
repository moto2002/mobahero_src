using System;
using System.Collections;

namespace Com.Game.Utils
{
	public class XMLNodeList : ArrayList
	{
		public XMLNode Pop()
		{
			XMLNode result = this[this.Count - 1] as XMLNode;
			this.RemoveAt(this.Count - 1);
			return result;
		}

		public void Push(XMLNode node)
		{
			this.Add(node);
		}
	}
}
