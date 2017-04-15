using Com.Game.Module;
using MobaHeros;
using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieStepEleBatOneOneMoveToTower : NewbieStepBase
	{
		private string _mainText = "移动到我方防御塔前";

		private GameObject _checkObjInst;

		private Vector3 _centerPos = new Vector3(-5.22f, 0f, -0.44f);

		private string _hintEffResId = "Fx_introduction_movehere";

		private ResourceHandle _hintEffResHandle;

		private string _voiceResId = "2013";

		private float _loopTime = 10f;

		public NewbieStepEleBatOneOneMoveToTower()
		{
			this._stepType = ENewbieStepType.ElementaryBatOneOne_MoveToTower;
		}

		public override void OnLeave()
		{
			NewbieManager.Instance.StopVoiceHintLoop();
			NewbieManager.Instance.HideMoveGuideLine();
			this.StopCheckPlayerReachPosition();
			this.DestroyEffectHint();
			Singleton<NewbieView>.Instance.HideTitle();
			CtrlManager.CloseWindow(WindowID.NewbieView);
		}

		public override void HandleAction()
		{
			CtrlManager.OpenWindow(WindowID.NewbieView, null);
			Singleton<NewbieView>.Instance.ShowTitleWithText(this._mainText);
			this.StartCheckPlayerReachPosition();
			this.DispEffectHint();
			NewbieManager.Instance.ShowMoveGuideLine(this._centerPos);
			NewbieManager.Instance.PlayVoiceHintLoop(this._voiceResId, this._loopTime);
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
