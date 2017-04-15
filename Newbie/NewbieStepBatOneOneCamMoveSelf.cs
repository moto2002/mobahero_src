using Assets.Scripts.Model;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Newbie
{
	public class NewbieStepBatOneOneCamMoveSelf : NewbieStepBase
	{
		public NewbieStepBatOneOneCamMoveSelf()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_CamMoveSelf;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			NewbieManager.Instance.StopCheckForbidSelectTower();
			this.ReqCheckTowerKilled();
			this.DoCamMoveSelf();
			base.AutoMoveNextStepWithDelay(0.3f);
		}

		private void ReqCheckTowerKilled()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 41
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}

		private void DoCamMoveSelf()
		{
			BattleCameraMgr.Instance.RestoreCameraController(0.4f);
		}
	}
}
