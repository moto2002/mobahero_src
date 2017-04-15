using System;
using UnityEngine;

namespace GUIFramework
{
	[Serializable]
	public class WinDataCfg
	{
		[Tooltip("是否为常驻UI（即使是全屏，也不清除）")]
		public bool IsDestroy = true;

		[Tooltip("关闭是否需要延时")]
		public bool IsDelayClose;

		[Tooltip("打开的标题是否需要延时")]
		public bool IsDelayShowBar;

		[Tooltip("是否起始窗口")]
		public bool IsOnset;

		[Tooltip("窗口类型")]
		public WindowType WinType = WindowType.Normal;
	}
}
