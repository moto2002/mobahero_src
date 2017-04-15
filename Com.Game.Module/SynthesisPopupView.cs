using Assets.Scripts.GUILogic.View.Runes;
using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using MobaMessageData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Com.Game.Module
{
	public class SynthesisPopupView : BaseView<SynthesisPopupView>
	{
		private Transform btnCloseWindow;

		private Transform centerAnchor;

		private Transform transPrimary;

		private Transform transMiddle;

		private Transform transHigh;

		public SynthesisPopupView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Sacrificial/SynthesisPopupView");
		}

		public override void Init()
		{
			base.Init();
			this.centerAnchor = this.transform.Find("CenterAnchor");
			this.btnCloseWindow = this.centerAnchor.Find("Quit");
			this.transPrimary = this.centerAnchor.Find("Primary");
			this.transMiddle = this.centerAnchor.Find("Middle");
			this.transHigh = this.centerAnchor.Find("High");
			UIEventListener.Get(this.btnCloseWindow.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseWindow);
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.RegistMessage((ClientMsg)21087, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.RegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.RegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)25012, new MobaMessageFunc(this.OnTimeOut));
			MobaMessageManager.UnRegistMessage((ClientMsg)21087, new MobaMessageFunc(this.OnOpenView));
			MobaMessageManager.UnRegistMessage((ClientMsg)25062, new MobaMessageFunc(this.OnPeerConnected));
			MobaMessageManager.UnRegistMessage((ClientMsg)25061, new MobaMessageFunc(this.OnPeerDisconnected));
		}

		public override void HandleAfterOpenView()
		{
			this.centerAnchor.gameObject.SetActive(true);
			RunesCtrl.GetInstance().runestate = RunesInlayState.Nothing;
		}

		public override void HandleBeforeCloseView()
		{
		}

		private void OnOpenView(MobaMessage msg)
		{
			if (msg.Param != null)
			{
				string modelid = string.Empty;
				modelid = (string)msg.Param;
				ModelIDs modelIDs = default(ModelIDs);
				this.CheckModelID(modelid, out modelIDs);
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewOpenView, modelIDs, false);
			}
		}

		private void OnTimeOut(MobaMessage msg)
		{
			if (msg != null)
			{
				MobaMessageType mobaMessageType = MobaMessageType.GameCode;
				int num = 204;
				MsgData_WaitServerResponsTimeout msgData_WaitServerResponsTimeout = (MsgData_WaitServerResponsTimeout)msg.Param;
				if (msgData_WaitServerResponsTimeout.MobaMsgType != mobaMessageType || msgData_WaitServerResponsTimeout.MsgID == num)
				{
				}
			}
		}

		private void OnPeerConnected(MobaMessage msg)
		{
		}

		private void OnPeerDisconnected(MobaMessage msg)
		{
		}

		private void CheckModelID(string _modelid, out ModelIDs mds)
		{
			int modelID_ = int.Parse(_modelid);
			Dictionary<string, SysGameItemsVo> typeDicByType = BaseDataMgr.instance.GetTypeDicByType<SysGameItemsVo>();
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(_modelid);
			int quality = dataById.quality;
			if (quality != 0 && quality != 1)
			{
				List<string> list = new List<string>();
				list = typeDicByType.Keys.ToList<string>();
				for (int i = 0; i < list.Count; i++)
				{
					if (typeDicByType[list[i]].synthetic_id.CompareTo(_modelid) == 0)
					{
						modelID_ = int.Parse(list[i]);
						break;
					}
				}
			}
			mds.modelID_1 = modelID_;
			mds.modelID_2 = int.Parse(typeDicByType[mds.modelID_1.ToString()].synthetic_id);
			mds.modelID_3 = int.Parse(typeDicByType[mds.modelID_2.ToString()].synthetic_id);
		}

		private void CloseWindow(GameObject obj)
		{
			if (null != obj)
			{
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewCloseView, null, false);
				CtrlManager.CloseWindow(WindowID.SynthesisPopupView);
			}
		}
	}
}
