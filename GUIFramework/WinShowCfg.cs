using System;
using UnityEngine;

namespace GUIFramework
{
	[Serializable]
	public class WinShowCfg
	{
		[Tooltip("窗口打开模式")]
		public WindowShowMode ShowMode;

		[Tooltip("是否清除返回窗口序列")]
		public bool IsClearReturnSeq;

		[Tooltip("是否加入到返回队列")]
		public bool IsAddToReturnSeq;
	}
}
