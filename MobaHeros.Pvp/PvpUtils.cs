using Com.Game.Module;
using System;

namespace MobaHeros.Pvp
{
	internal static class PvpUtils
	{
		public static void GoHome()
		{
			if (!GameManager.IsGameNone())
			{
				GameManager.SetGameState(GameState.Game_Exit);
			}
		}

		public static void ShowNetworkError(string msg)
		{
			CtrlManager.ShowMsgBox("网络错误", msg, null, PopViewType.PopOneButton, "确定", "取消", null);
		}

		public static void ShowNetworkError(int err)
		{
			PvpUtils.ShowNetworkError("服务器错误，code=" + err);
		}
	}
}
