using Assets.Scripts.GUILogic.View.PropertyView;
using Assets.Scripts.GUILogic.View.Runes;
using GUIFramework;
using MobaMessageData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class RunesOverView : BaseView<RunesOverView>
	{
		public enum RuneToggleState
		{
			DisableInlay,
			EnableInlay
		}

		private Transform leftAnchor;

		private Transform upperAnchor;

		private Transform bottomAnchor;

		private Transform btnClose;

		private UIToggle runesInlayView;

		private UIToggle runesStorageView;

		private RunesFunctionType currType;

		private RunesOverView.RuneToggleState toggleType;

		private Dictionary<RunesFunctionType, UIToggle> dicToggle;

		private object[] mgs;

		private string heroNpc = string.Empty;

		public string HeroNpc
		{
			get
			{
				return this.heroNpc;
			}
		}

		private RunesFunctionType CurrType
		{
			get
			{
				return this.currType;
			}
			set
			{
				this.currType = value;
				this.bottomAnchor.gameObject.SetActive(this.currType == RunesFunctionType.Storage);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewChangeToggle, this.currType, false);
			}
		}

		public RunesOverView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Sacrificial/RunesOverView");
		}

		public override void Init()
		{
			this.leftAnchor = this.transform.Find("LeftAnchor");
			this.upperAnchor = this.transform.Find("UpperAnchor");
			this.bottomAnchor = this.transform.Find("BottomAnchor");
			this.btnClose = this.upperAnchor.Find("CloseFrame");
			this.runesInlayView = this.leftAnchor.Find("RunesInlay").GetComponent<UIToggle>();
			this.runesStorageView = this.leftAnchor.Find("RunesStorage").GetComponent<UIToggle>();
			this.dicToggle = new Dictionary<RunesFunctionType, UIToggle>();
			this.dicToggle[RunesFunctionType.Inlay] = this.runesInlayView;
			this.dicToggle[RunesFunctionType.Storage] = this.runesStorageView;
			EventDelegate.Add(this.runesInlayView.onChange, new EventDelegate.Callback(this.ClickBtn));
			EventDelegate.Add(this.runesStorageView.onChange, new EventDelegate.Callback(this.ClickBtn));
			UIEventListener.Get(this.btnClose.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickCloseRuneView);
			this.toggleType = RunesOverView.RuneToggleState.EnableInlay;
			this.mgs = new object[]
			{
				ClientV2C.runesviewInitToggle,
				ClientC2C.GateDisconnected,
				ClientC2C.GateConnected,
				ClientC2C.WaitServerResponseTimeOut
			};
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		public override void HandleAfterOpenView()
		{
			RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
		}

		public override void HandleBeforeCloseView()
		{
		}

		private void ClickBtn()
		{
			if (UIToggle.current.value)
			{
				if (this.toggleType == RunesOverView.RuneToggleState.DisableInlay && UIToggle.current == this.runesInlayView)
				{
					Singleton<TipView>.Instance.ShowViewSetText("未选择英雄，无法跳转！！！", 1f);
					this.dicToggle[RunesFunctionType.Storage].value = true;
					return;
				}
				foreach (KeyValuePair<RunesFunctionType, UIToggle> current in this.dicToggle)
				{
					if (current.Value == UIToggle.current)
					{
						this.CurrType = current.Key;
						break;
					}
				}
			}
		}

		private void ClickCloseRuneView(GameObject obj)
		{
			if (null != obj)
			{
				if (null != Singleton<PropertyView>.Instance.transform && Singleton<PropertyView>.Instance.gameObject.activeInHierarchy)
				{
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewInitToggle, PropertyType.Info, false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.propviewChangeToggle, PropertyType.Info, false);
					MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewCloseView, null, false);
				}
				CtrlManager.CloseWindow(WindowID.RunesOverView);
			}
		}

		private void OnMsg_runesviewInitToggle(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string value = string.Empty;
				value = (string)msg.Param;
				this.heroNpc = value;
				RunesFunctionType runesFunctionType;
				if (string.IsNullOrEmpty(value))
				{
					runesFunctionType = RunesFunctionType.Storage;
				}
				else
				{
					runesFunctionType = RunesFunctionType.Inlay;
				}
				if (this.dicToggle != null && this.dicToggle.ContainsKey(runesFunctionType))
				{
					this.currType = runesFunctionType;
					this.dicToggle[runesFunctionType].value = true;
					if (this.currType == RunesFunctionType.Storage)
					{
						this.toggleType = RunesOverView.RuneToggleState.DisableInlay;
					}
					else
					{
						this.toggleType = RunesOverView.RuneToggleState.EnableInlay;
					}
				}
				this.bottomAnchor.gameObject.SetActive(this.currType == RunesFunctionType.Storage);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewChangeToggle, runesFunctionType, false);
			}
		}

		private void OnMsg_GateConnected(MobaMessage msg)
		{
			if (msg != null)
			{
				RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
			}
		}

		private void OnMsg_GateDisconnected(MobaMessage msg)
		{
			if (msg != null)
			{
				RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
			}
		}

		private void OnMsg_WaitServerResponseTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 72;
				int num2 = 204;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType == mobaMessageType && (msgData_WaitServerResponsTimeout.MsgID == num || msgData_WaitServerResponsTimeout.MsgID == num2))
				{
					RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
				}
			}
		}
	}
}
