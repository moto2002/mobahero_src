using Com.Game.Module;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_CSTVStartShow : MonoBehaviour
	{
		public GameObject BackGround;

		public UIInput LiveTitleInput;

		public UIToggle SdToggle;

		public UIToggle HdToggle;

		public UIToggle UhdToggle;

		public GameObject LiveBtn;

		private int liveQualityRecord = 1;

		private void OnEnable()
		{
			UIEventListener.Get(this.BackGround).onClick = new UIEventListener.VoidDelegate(this.OnClick_Back);
			UIEventListener.Get(this.SdToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Toggle);
			UIEventListener.Get(this.HdToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Toggle);
			UIEventListener.Get(this.UhdToggle.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Toggle);
			UIEventListener.Get(this.LiveBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_LiveBtn);
		}

		private void OnClick_Back(GameObject obj = null)
		{
			base.gameObject.SetActive(false);
		}

		private void OnClick_Toggle(GameObject obj = null)
		{
			if (obj == this.HdToggle.gameObject)
			{
				this.liveQualityRecord = 1;
			}
			else if (obj == this.UhdToggle.gameObject)
			{
				this.liveQualityRecord = 2;
			}
			else
			{
				this.liveQualityRecord = 0;
			}
		}

		private void OnClick_LiveBtn(GameObject obj = null)
		{
			if (string.IsNullOrEmpty(this.LiveTitleInput.value) || this.LiveTitleInput.value == "请输入标题")
			{
				Singleton<TipView>.Instance.ShowViewSetText("标题不得为空！", 1f);
				return;
			}
			this.SubmitShowSetting();
		}

		public void Show()
		{
			base.gameObject.SetActive(true);
		}

		public void Hide()
		{
			base.gameObject.SetActive(false);
		}

		public void SubmitShowSetting()
		{
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26033, new object[]
			{
				this.LiveTitleInput.value,
				this.liveQualityRecord
			}, 0f);
			MobaMessageManager.ExecuteMsg(message);
			base.gameObject.SetActive(false);
		}
	}
}
