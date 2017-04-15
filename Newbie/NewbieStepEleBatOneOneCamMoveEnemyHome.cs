using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneCamMoveEnemyHome : NewbieStepBase
	{
		public NewbieStepEleBatOneOneCamMoveEnemyHome()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_CamMoveEnemyHome;
		}

		public override void OnLeave()
		{
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			this.ReqSpawnWaveSoldiers();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			this.DoCamMoveEnemyHome();
			base.AutoMoveNextStepWithDelay(0.3f);
		}

		private void ReqSpawnWaveSoldiers()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 37
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}

		private void DoCamMoveEnemyHome()
		{
			Units home = MapManager.Instance.GetHome(TeamType.BL);
			BattleCameraMgr.Instance.ChangeCameraController(CameraControllerType.MoveByTap);
			BattleCameraMgr.Instance.SetPositionAndMoveTime(home.mTransform.position, 0.7f);
		}
	}
}
