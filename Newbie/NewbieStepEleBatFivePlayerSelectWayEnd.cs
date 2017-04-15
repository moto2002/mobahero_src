using Assets.Scripts.Model;
using MobaProtocol;
using MobaProtocol.Data;
using System;

namespace Newbie
{
	public class NewbieStepEleBatFivePlayerSelectWayEnd : NewbieStepBase
	{
		public NewbieStepEleBatFivePlayerSelectWayEnd()
		{
			this._stepType = ENewbieStepType.EleBatFive_PlayerSelectWayEnd;
		}

		public override void OnLeave()
		{
		}

		public override void HandleAction()
		{
			this.ReqFriendsSelectWay();
			NewbieManager.Instance.EleBatFiveStartTriggerChecker();
		}

		private void ReqFriendsSelectWay()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 69
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}
	}
}
