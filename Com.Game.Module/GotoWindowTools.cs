using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using System;

namespace Com.Game.Module
{
	public static class GotoWindowTools
	{
		public static void GotoWindow(int i)
		{
			switch (i)
			{
			case 1:
				if (Singleton<UIPvpEntranceCtrl>.Instance.IsOpen)
				{
					CtrlManager.CloseWindow(WindowID.PvpEntranceView);
				}
				if (!Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuView, null);
				}
				CtrlManager.CloseWindow(WindowID.TaskView);
				CtrlManager.CloseWindow(WindowID.DailyView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				break;
			case 2:
				if (!Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuView, null);
				}
				CtrlManager.CloseWindow(WindowID.TaskView);
				CtrlManager.CloseWindow(WindowID.DailyView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				Singleton<ArenaModeView>.Instance.TravalClick(2);
				break;
			case 3:
			{
				long exp = ModelManager.Instance.Get_userData_X().Exp;
				int userLevel = CharacterDataMgr.instance.GetUserLevel(exp);
				int scene_limit_level = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80006").scene_limit_level;
				bool flag = userLevel >= scene_limit_level;
				if (flag)
				{
					if (Singleton<UIPvpEntranceCtrl>.Instance.IsOpen)
					{
						CtrlManager.CloseWindow(WindowID.PvpEntranceView);
					}
					if (!Singleton<ArenaModeView>.Instance.IsOpen)
					{
						CtrlManager.OpenWindow(WindowID.MenuView, null);
					}
					CtrlManager.CloseWindow(WindowID.TaskView);
					CtrlManager.CloseWindow(WindowID.DailyView);
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
					Singleton<ArenaModeView>.Instance.TravalClick(3);
				}
				else
				{
					CtrlManager.ShowMsgBox("提示", string.Format("等级不足，{0}级才能前往", scene_limit_level), delegate
					{
					}, PopViewType.PopOneButton, "确定", "取消", null);
				}
				break;
			}
			case 4:
			{
				long exp2 = ModelManager.Instance.Get_userData_X().Exp;
				int userLevel2 = CharacterDataMgr.instance.GetUserLevel(exp2);
				int scene_limit_level2 = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80021").scene_limit_level;
				bool flag2 = userLevel2 >= scene_limit_level2;
				if (flag2)
				{
					if (!Singleton<ArenaModeView>.Instance.IsOpen)
					{
						CtrlManager.OpenWindow(WindowID.MenuView, null);
					}
					CtrlManager.CloseWindow(WindowID.TaskView);
					CtrlManager.CloseWindow(WindowID.DailyView);
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
					Singleton<ArenaModeView>.Instance.TravalClick(4);
				}
				else
				{
					CtrlManager.ShowMsgBox("提示", string.Format("等级不足，{0}级才能前往", scene_limit_level2), delegate
					{
					}, PopViewType.PopOneButton, "确定", "取消", null);
				}
				break;
			}
			case 5:
				if (!Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuView, null);
				}
				CtrlManager.CloseWindow(WindowID.TaskView);
				CtrlManager.CloseWindow(WindowID.DailyView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				Singleton<ArenaModeView>.Instance.TravalClick(5);
				break;
			case 6:
				if (!Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuView, null);
				}
				CtrlManager.CloseWindow(WindowID.TaskView);
				CtrlManager.CloseWindow(WindowID.DailyView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				Singleton<ArenaModeView>.Instance.TravalClick(6);
				break;
			case 7:
				if (!Singleton<ArenaModeView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.MenuView, null);
				}
				CtrlManager.CloseWindow(WindowID.TaskView);
				CtrlManager.CloseWindow(WindowID.DailyView);
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				Singleton<ArenaModeView>.Instance.TravalClick(7);
				break;
			case 8:
				MobaMessageManagerTools.SendClientMsg(ClientV2C.bottleSystemOpenView, null, false);
				break;
			case 9:
				CtrlManager.OpenWindow(WindowID.GoldHandView, null);
				break;
			case 10:
				if (CharacterDataMgr.instance.OwenHeros.Count > 0)
				{
					CtrlManager.OpenWindow(WindowID.PropertyView, null);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.sacriviewChangeHero, CharacterDataMgr.instance.OwenHeros[0], false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Rune, false);
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText("当前没有已拥有的英雄，无法完成此任务", 1f);
				}
				break;
			case 11:
				CtrlManager.OpenWindow(WindowID.SignView, null);
				break;
			}
		}
	}
}
