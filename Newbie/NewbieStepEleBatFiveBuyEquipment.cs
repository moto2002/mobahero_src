using Assets.Scripts.Model;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatFiveBuyEquipment : NewbieStepBase
	{
		private string _voiceResId = "2104";

		private float _loopTime = 10f;

		public NewbieStepEleBatFiveBuyEquipment()
		{
			this._stepType = ENewbieStepType.EleBatFive_BuyEquipment;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			Singleton<NewbieView>.Instance.HideFastBuyEquipHint();
			Singleton<NewbieView>.Instance.HideMask();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			this.ReqForbidFastDoor();
			this.CreateForbidFastDoorEffect();
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.SetMaskInfo(true, new Color(0f, 0f, 0f, 0.01f));
			Singleton<NewbieView>.Instance.ShowFastBuyEquipHint();
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}

		private void ReqForbidFastDoor()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 67
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}

		private void CreateForbidFastDoorEffect()
		{
			NewbieManager.Instance.EleBatFiveCreateForbidDoorEffect();
		}
	}
}
