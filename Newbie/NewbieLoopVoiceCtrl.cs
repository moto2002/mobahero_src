using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieLoopVoiceCtrl : MonoBehaviour
	{
		private string _voiceResId = string.Empty;

		private float _loopTime = 1f;

		private float _startTime = 1f;

		private void Update()
		{
			if (Time.time - this._startTime > this._loopTime)
			{
				this._startTime = Time.time;
				this.PlayVoice();
			}
		}

		public void StartLoopVoice(string inResId, float inLoopTime)
		{
			this._voiceResId = inResId;
			this._loopTime = inLoopTime;
			this._startTime = Time.time;
			this.PlayVoice();
		}

		private void PlayVoice()
		{
			NewbieManager.Instance.NewbiePlayHint(this._voiceResId, base.gameObject);
		}

		public void StopLoopVoice()
		{
			base.enabled = false;
		}
	}
}
