using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneSpawnEnemyHero : NewbieStepBase
	{
		private string _voiceResId = "2024";

		public NewbieStepEleBatOneOneSpawnEnemyHero()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_SpawnEnemyHero;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintOnce();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			this.ReqSpawnEnemyHero();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0.5f, 0.5f, 0.5f, 0.01f));
			NewbieManager.Instance.PlayVoiceHintOnce(this._voiceResId);
			base.AutoMoveNextStepWithDelay(7f);
		}

		private void ReqSpawnEnemyHero()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 29
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
