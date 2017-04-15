using GUIFramework;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class ReplayControllerView : BaseView<ReplayControllerView>
	{
		private const string PLAY_ICON_SPRITENAME = "Live_hud_icons_03";

		private const string PAUSE_ICON_SPRITENAME = "Live_hud_icons_04";

		public GameObject BackGround;

		public Transform ParentAnchorNode;

		public GameObject PlayBtn;

		public UISprite PlayBtnIcon;

		public GameObject SpeedDownBtn;

		public GameObject SpeedUpBtn;

		public GameObject ExitBtn;

		public UILabel SpeedLabel;

		private bool activeFlag;

		private bool playStateFlag;

		private static readonly float[] SPEED_ARRAY = new float[]
		{
			0.2f,
			0.5f,
			1f,
			2f,
			4f,
			8f,
			16f
		};

		private int arrayPointer = 2;

		private float timeScaleRecord = 1f;

		private static readonly int ARRAY_POINTER_START = 2;

		public float PlaySpeed
		{
			get
			{
				return (this.arrayPointer < 0 || this.arrayPointer >= ReplayControllerView.SPEED_ARRAY.Length) ? 1f : ReplayControllerView.SPEED_ARRAY[this.arrayPointer];
			}
		}

		public ReplayControllerView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/ReplayControllerView");
		}

		public override void Init()
		{
			this.arrayPointer = 2;
			this.BackGround = this.transform.FindChild("bg").gameObject;
			this.ParentAnchorNode = this.transform.FindChild("ParentAnchor");
			this.PlayBtn = this.ParentAnchorNode.FindChild("playBtn").gameObject;
			this.PlayBtnIcon = this.PlayBtn.transform.FindChild("icon").GetComponent<UISprite>();
			this.SpeedDownBtn = this.ParentAnchorNode.FindChild("minus").gameObject;
			this.SpeedUpBtn = this.ParentAnchorNode.FindChild("add").gameObject;
			this.ExitBtn = this.ParentAnchorNode.FindChild("exit").gameObject;
			this.SpeedLabel = this.ParentAnchorNode.FindChild("speedPower/Label").GetComponent<UILabel>();
			UIEventListener.Get(this.BackGround).onClick = new UIEventListener.VoidDelegate(this.OnClick_BG);
			UIEventListener.Get(this.PlayBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_PlayOrPause);
			UIEventListener.Get(this.SpeedDownBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_SpeedDown);
			UIEventListener.Get(this.SpeedUpBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_SpeedUp);
			UIEventListener.Get(this.ExitBtn).onClick = new UIEventListener.VoidDelegate(this.OnClick_Exit);
		}

		public override void HandleAfterOpenView()
		{
			this.playStateFlag = true;
			this.GetPointerAvaliable();
			this.activeFlag = true;
			this.ParentAnchorNode.gameObject.SetActive(true);
			this.ApplyPlayPauseState();
			this.ApplySpeed(true);
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)25064, new MobaMessageFunc(this.OnMsg_ApplicationPause));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25064, new MobaMessageFunc(this.OnMsg_ApplicationPause));
		}

		private void OnClick_BG(GameObject obj = null)
		{
			this.activeFlag = !this.activeFlag;
			this.ParentAnchorNode.gameObject.SetActive(this.activeFlag);
		}

		private void OnClick_PlayOrPause(GameObject obj = null)
		{
			this.playStateFlag = !this.playStateFlag;
			this.ApplyPlayPauseState();
		}

		private void OnClick_SpeedDown(GameObject obj = null)
		{
			this.arrayPointer--;
			this.GetPointerAvaliable();
			this.ApplySpeed(false);
		}

		private void OnClick_SpeedUp(GameObject obj = null)
		{
			this.arrayPointer++;
			this.GetPointerAvaliable();
			this.ApplySpeed(false);
		}

		private void OnClick_Exit(GameObject obj = null)
		{
			CtrlManager.ShowMsgBox("确认退出", "是否要退出战斗回放？", delegate(bool _isConfirm)
			{
				if (_isConfirm)
				{
					GameManager.Instance.ReplayController.EndReplay();
				}
			}, PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void OnMsg_ApplicationFocus(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			this.playStateFlag = flag;
			this.ApplyPlayPauseState();
		}

		private void OnMsg_ApplicationPause(MobaMessage msg)
		{
			bool flag = (bool)msg.Param;
			this.playStateFlag = !flag;
			this.ApplyPlayPauseState();
		}

		private void ApplySpeed(bool isForced = false)
		{
			float playSpeed = this.PlaySpeed;
			this.SpeedLabel.text = "x" + playSpeed.ToString((playSpeed >= 1f) ? "F0" : "F1");
			this.timeScaleRecord = playSpeed;
			if (this.playStateFlag || isForced)
			{
				Time.timeScale = playSpeed;
				this.ReplaySpeedChangeMsg(playSpeed);
			}
			if (isForced && !this.playStateFlag)
			{
				this.playStateFlag = true;
				this.ApplyPlayPauseState();
			}
		}

		private void GetPointerAvaliable()
		{
			this.arrayPointer = ((this.arrayPointer >= 0) ? ((this.arrayPointer < ReplayControllerView.SPEED_ARRAY.Length) ? this.arrayPointer : (ReplayControllerView.SPEED_ARRAY.Length - 1)) : 0);
		}

		private void ApplyPlayPauseState()
		{
			if (this.playStateFlag)
			{
				this.Play();
			}
			else
			{
				this.Pause();
			}
		}

		private void Play()
		{
			Time.timeScale = this.timeScaleRecord;
			this.ReplaySpeedChangeMsg(this.timeScaleRecord);
			this.PlayBtnIcon.spriteName = "Live_hud_icons_04";
			this.PlayBtnIcon.MakePixelPerfect();
		}

		private void Pause()
		{
			Time.timeScale = 0f;
			this.ReplaySpeedChangeMsg(0f);
			this.PlayBtnIcon.spriteName = "Live_hud_icons_03";
			this.PlayBtnIcon.MakePixelPerfect();
		}

		private void ReplaySpeedChangeMsg(float _timescale)
		{
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26034, _timescale, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}
	}
}
