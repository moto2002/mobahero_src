using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneSpawnFourSoldier : NewbieStepBase
	{
		private string _voiceResId = "2019";

		public NewbieStepEleBatOneOneSpawnFourSoldier()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SpawnFourSoldier;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			this.ReqSpawnEnemySoldiers();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(4.2f);
		}

		private void ReqSpawnEnemySoldiers()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 23
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
