using Com.Game.Module;
using Com.Game.Utils;
using GameLogin.State;
using MobaHeros.Pvp.State;
using MobaProtocol.Data;
using System;
using System.Linq;
using UnityEngine;

namespace MobaHeros.Pvp
{
	public class PvpObserveMgr
	{
		private static int _userId;

		public static bool IsObserveFree
		{
			get
			{
				return PvpObserveMgr._userId == 0;
			}
		}

		public static Vector3 FreeObservePos
		{
			get;
			set;
		}

		public static void ObserveUser(int userId)
		{
			Units unit = MapManager.Instance.GetUnit(-userId);
			if (!unit)
			{
				ClientLogger.Error("ObserveUser: cannot found #" + userId);
				return;
			}
			PvpObserveMgr._userId = userId;
			PlayerControlMgr.Instance.ChangePlayer(unit, true);
			BattleCameraMgr.Instance.ResetEvents();
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.Center);
			BattleCameraMgr.Instance.SetRoleObj(unit, true);
		}

		public static void ObserveFree()
		{
			PvpObserveMgr._userId = 0;
			BattleCameraMgr.Instance.ResetEvents();
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.AlwaysFree);
		}

		public static Units GetObserserUnit()
		{
			if (PvpObserveMgr.IsObserveFree)
			{
				return null;
			}
			return MapManager.Instance.GetUnit(-PvpObserveMgr._userId);
		}

		public static void BeginObserve()
		{
			int newUid = Singleton<PvpManager>.Instance.GetPlayersByTeam(TeamType.LM).First<ReadyPlayerSampleInfo>().newUid;
			PvpObserveMgr.ObserveUser(newUid);
			if (GlobalSettings.FogMode >= 2)
			{
				FOWSystem.Instance.enableFog(false);
			}
			else if (GlobalSettings.FogMode == 1)
			{
				FogMgr.Instance.enableFog(false);
			}
		}

		public static void QuitObserve()
		{
			NetWorkHelper.Instance.DisconnectPvpServer();
			NetWorkHelper.Instance.DisconnectPvpServer();
			PvpStateManager.Instance.ChangeState(new PvpStateHome());
			if (GlobalSettings.FogMode >= 2)
			{
				FOWSystem.Instance.enableFog(true);
			}
			else if (GlobalSettings.FogMode == 1)
			{
				FogMgr.Instance.enableFog(true);
			}
		}

		public static void ConfirmQuitObserve()
		{
			CtrlManager.ShowMsgBox("退出", "确定要退出观战吗？", new Action(PvpObserveMgr.QuitObserve), PopViewType.PopOneButton, "确定", "取消", null);
		}

		[Obsolete("temp use only")]
		public static void TryRefreshUI()
		{
			if (Singleton<PvpManager>.Instance.IsObserver)
			{
				ObserveView.DoRefresh();
			}
		}
	}
}
