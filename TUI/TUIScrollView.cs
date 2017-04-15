using System;
using System.Collections;
using UnityEngine;

namespace TUI
{
	public class TUIScrollView : TUIWidget
	{
		public UIScrollView scrollView;

		public UIScrollBar scorllBar;

		public UIGrid grid;

		[Tooltip("item的预制体")]
		public GameObject itemPrefab;

		[Tooltip("默认listitem个数")]
		public int Count;

		private ArrayList listItem = new ArrayList();
	}
}
