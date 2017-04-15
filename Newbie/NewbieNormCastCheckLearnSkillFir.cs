using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieNormCastCheckLearnSkillFir : MonoBehaviour
	{
		private Units _playerHero;

		private float _startCheckTime;

		private Units CachedPlayerHero
		{
			get
			{
				if (this._playerHero == null && PlayerControlMgr.Instance != null)
				{
					this._playerHero = PlayerControlMgr.Instance.GetPlayer();
				}
				return this._playerHero;
			}
		}

		private void Update()
		{
			if (Time.time - this._startCheckTime > 1f)
			{
				this._startCheckTime = Time.time;
				if (this.CachedPlayerHero != null && this.CachedPlayerHero.skillManager != null && this.CachedPlayerHero.skillManager.IsSkillUnlockByIndex(0))
				{
					this.StopCheckLearnSkillFir();
					NewbieManager.Instance.MoveCertainStep(ENewbieStepType.NormCast_OpenSysSetting, false, ENewbieStepType.None);
				}
			}
		}

		public void StartCheckLearnSkillFir()
		{
			this._startCheckTime = Time.time;
		}

		public void StopCheckLearnSkillFir()
		{
			base.enabled = false;
		}
	}
}
