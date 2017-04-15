using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieSubtitleCtrl : MonoBehaviour
	{
		private bool _isUpdateSubtitle;

		private int _curSubtitleIndex;

		private float _curSubtitleTimeLen = 1f;

		private string _curSubtitleContentId = string.Empty;

		private int _curSubtitleProcessIdx;

		private float _subtitleStartTime;

		private NewbieSubtitleData _savedSubtitleData;

		private void Update()
		{
			if (!this._isUpdateSubtitle)
			{
				return;
			}
			if (Time.time - this._subtitleStartTime > this._curSubtitleTimeLen && !this.TryDisplayNextSubtitle())
			{
				this.StopUpdateSubtitle();
			}
		}

		public void StartUpdateSubtitle(NewbieSubtitleData inSubtitleData)
		{
			this._curSubtitleIndex = 0;
			this._curSubtitleTimeLen = 1f;
			this._curSubtitleContentId = string.Empty;
			this._curSubtitleProcessIdx = 0;
			this._subtitleStartTime = 0f;
			this._savedSubtitleData.validNum = 0;
			this._savedSubtitleData.processLength = 0;
			this._savedSubtitleData.firSubtitleId = string.Empty;
			this._savedSubtitleData.firSubtitleTimeLen = 1f;
			this._savedSubtitleData.firProcessIdx = 0;
			this._savedSubtitleData.secSubtitleId = string.Empty;
			this._savedSubtitleData.secSubtitleTimeLen = 1f;
			this._savedSubtitleData.secProcessIdx = 0;
			if (inSubtitleData.validNum > 0)
			{
				this._isUpdateSubtitle = true;
				this._savedSubtitleData = inSubtitleData;
				this._curSubtitleIndex = 0;
				this._curSubtitleContentId = inSubtitleData.firSubtitleId;
				this._curSubtitleTimeLen = inSubtitleData.firSubtitleTimeLen;
				this._curSubtitleProcessIdx = inSubtitleData.firProcessIdx;
				this._subtitleStartTime = Time.time;
				this.DoDisplaySubtitle();
			}
			else
			{
				this._isUpdateSubtitle = false;
			}
		}

		public void StopUpdateSubtitle()
		{
			this._isUpdateSubtitle = false;
		}

		private bool TryDisplayNextSubtitle()
		{
			if (this._curSubtitleIndex + 1 < this._savedSubtitleData.validNum)
			{
				this._curSubtitleIndex++;
				if (this._curSubtitleIndex == 1)
				{
					this._curSubtitleContentId = this._savedSubtitleData.secSubtitleId;
					this._curSubtitleTimeLen = this._savedSubtitleData.secSubtitleTimeLen;
					this._curSubtitleProcessIdx = this._savedSubtitleData.secProcessIdx;
				}
				this._subtitleStartTime = Time.time;
				this.DoDisplaySubtitle();
				return true;
			}
			return false;
		}

		private void DoDisplaySubtitle()
		{
			NewbieManager.Instance.DoDisplaySubtitle(this._curSubtitleContentId, this._curSubtitleProcessIdx, this._savedSubtitleData.processLength);
		}
	}
}
