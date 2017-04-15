using Assets.Scripts.Model;
using Com.Game.Module;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneMoveToHome : NewbieStepBase
	{
		private string _mainText = "点击地面即可移动";

		private GameObject _checkObjInst;

		private Vector3 _centerPos = new Vector3(-14.64f, 0f, -3.95f);

		private string _hintEffResId = "Fx_introduction_movehere";

		private ResourceHandle _hintEffResHandle;

		private string _voiceResId = "2011";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneMoveToHome()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_MoveToHome;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
			NewbieManager.Instance.HideMoveGuideLine();
			this.StopCheckPlayerReachPosition();
			this.DestroyEffectHint();
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			this.SendMsgOnStepBatOneOneMoveToHome();
			this.DestoryObstacleEffect();
			this.StartCheckPlayerReachPosition();
			this.DispEffectHint();
			NewbieManager.Instance.ShowMoveGuideLine(this._centerPos);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
		}

		private void SendMsgOnStepBatOneOneMoveToHome()
		{
			byte[] msgBody = SerializeHelper.Serialize<NewbieMoveToStepData>(new NewbieMoveToStepData
			{
				stepType = 13
			});
			byte[] args = SerializeHelper.Serialize<NewbieInBattleData>(new NewbieInBattleData
			{
				msgType = 5,
				msgBody = msgBody
			});
			SendMsgManager.Instance.SendPvpMsg(PvpCode.C2P_NewbieInBattle, args);
		}

		private void DestoryObstacleEffect()
		{
			NewbieManager.Instance.DestroyEffectObstacleByIdx(0);
		}

		private void StartCheckPlayerReachPosition()
		{
			this._checkObjInst = new GameObject();
			this._checkObjInst.name = "NewbieCheckPlayerReachPos";
			NewbieCheckPlayerReachPos newbieCheckPlayerReachPos = this._checkObjInst.AddComponent<NewbieCheckPlayerReachPos>();
			if (newbieCheckPlayerReachPos != null)
			{
				newbieCheckPlayerReachPos.InitInfo(this._centerPos, 1.5f);
			}
		}

		private void StopCheckPlayerReachPosition()
		{
			if (this._checkObjInst != null)
			{
				UnityEngine.Object.Destroy(this._checkObjInst);
				this._checkObjInst = null;
			}
		}

		private void DestroyEffectHint()
		{
			if (this._hintEffResHandle != null)
			{
				this._hintEffResHandle.Release();
				this._hintEffResHandle = null;
			}
		}

		private void DispEffectHint()
		{
			this._hintEffResHandle = MapManager.Instance.SpawnResourceHandle(this._hintEffResId, null, 0);
			Transform raw = this._hintEffResHandle.Raw;
			if (raw != null)
			{
				raw.position = this._centerPos;
			}
		}
	}
}
