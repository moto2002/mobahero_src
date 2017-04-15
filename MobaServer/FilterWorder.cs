using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace MobaServer
{
	public class FilterWorder
	{
		private static readonly FilterWorder instance = new FilterWorder();

		private static IList<string> list = null;

		private CoroutineManager corManager;

		public static FilterWorder Instance
		{
			get
			{
				return FilterWorder.instance;
			}
		}

		private FilterWorder()
		{
		}

		[DebuggerHidden]
		private IEnumerator<WWW> Init()
		{
			return new FilterWorder.<Init>c__Iterator44();
		}

		public void TryInit()
		{
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			if (FilterWorder.list == null || FilterWorder.list.Count < 1)
			{
				this.corManager.StartCoroutine(this.Init(), true);
			}
		}

		public string ReplaceKeyword(string strText)
		{
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			if (FilterWorder.list == null || FilterWorder.list.Count < 1)
			{
				this.corManager.StartCoroutine(this.Init(), true);
			}
			if (FilterWorder.list == null || FilterWorder.list.Count < 1)
			{
				return strText;
			}
			string text = strText;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c < ' ' || c == '\u007f')
				{
					strText = strText.Replace(c, '*');
				}
			}
			foreach (string current in FilterWorder.list)
			{
				if (strText.Contains(current))
				{
					int length = current.Length;
					string text2 = string.Empty;
					for (int j = 0; j < length; j++)
					{
						text2 += "*";
					}
					strText = strText.Replace(current, text2);
				}
			}
			return strText;
		}

		public bool CheckKeyword(string strText)
		{
			if (this.corManager == null)
			{
				this.corManager = new CoroutineManager();
			}
			if (FilterWorder.list == null || FilterWorder.list.Count < 1)
			{
				this.corManager.StartCoroutine(this.Init(), true);
			}
			if (FilterWorder.list == null || FilterWorder.list.Count < 1)
			{
				return false;
			}
			foreach (string current in FilterWorder.list)
			{
				if (strText.Contains(current))
				{
					return true;
				}
			}
			return false;
		}
	}
}
