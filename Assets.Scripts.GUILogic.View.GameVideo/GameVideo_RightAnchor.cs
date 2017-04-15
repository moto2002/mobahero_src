using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_RightAnchor : MonoBehaviour
	{
		public GameVideo_LocalRecordPage LocalRecordPage;

		public GameVideo_CSTV CstvPage;

		private void OnEnable()
		{
			MobaMessageManager.RegistMessage(ClientV2V.GameVideo_ShowLocalRecordPage, new MobaMessageFunc(this.OnMsg_ShowRecordPage));
			MobaMessageManager.RegistMessage(ClientV2V.GameVideo_ShowCSTV, new MobaMessageFunc(this.OnMsg_ShowCSTV));
		}

		private void OnDisable()
		{
			MobaMessageManager.UnRegistMessage(ClientV2V.GameVideo_ShowLocalRecordPage, new MobaMessageFunc(this.OnMsg_ShowRecordPage));
			MobaMessageManager.UnRegistMessage(ClientV2V.GameVideo_ShowCSTV, new MobaMessageFunc(this.OnMsg_ShowCSTV));
		}

		private void OnMsg_ShowRecordPage(MobaMessage msg)
		{
			if (this.LocalRecordPage.IsActive)
			{
				return;
			}
			this.SetPageActive(this.LocalRecordPage);
			this.LocalRecordPage.ResetPage();
		}

		private void OnMsg_ShowCSTV(MobaMessage msg)
		{
			if (this.CstvPage.IsActive)
			{
				return;
			}
			this.SetPageActive(this.CstvPage);
			this.CstvPage.ResetPage();
		}

		private void SetPageActive(object _targetObj)
		{
			this.LocalRecordPage.SetActive(_targetObj.GetType() == typeof(GameVideo_LocalRecordPage));
			this.CstvPage.SetActive(_targetObj.GetType() == typeof(GameVideo_CSTV));
		}
	}
}
