using System;
using System.Collections;

namespace Com.Game.Utils
{
	public class XMLNode : Hashtable
	{
		public XMLNodeList GetNodeList(string path)
		{
			return this.GetObject(path) as XMLNodeList;
		}

		public XMLNode GetNode(string path)
		{
			return this.GetObject(path) as XMLNode;
		}

		public string GetValue(string path)
		{
			return this.GetObject(path) as string;
		}

		private object GetObject(string path)
		{
			XMLNode xMLNode = this;
			XMLNodeList xMLNodeList = null;
			bool flag = false;
			string[] array = path.Split(new char[]
			{
				'>'
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (flag)
				{
					xMLNode = (xMLNodeList[int.Parse(array[i])] as XMLNode);
					flag = false;
				}
				else
				{
					object obj = xMLNode[array[i]];
					if (!(obj is XMLNodeList))
					{
						if (i != array.Length - 1)
						{
							string str = string.Empty;
							for (int j = 0; j <= i; j++)
							{
								str = str + ">" + array[j];
							}
						}
						return obj;
					}
					xMLNodeList = (obj as XMLNodeList);
					flag = true;
				}
			}
			if (flag)
			{
				return xMLNodeList;
			}
			return xMLNode;
		}
	}
}
