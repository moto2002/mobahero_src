using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class BlackListView : BaseView<BlackListView>
	{
		private Transform BackButton;

		private Transform BlackListPanel;

		private TweenAlpha m_AlphaController;

		public BlackListView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/BlackListView");
		}

		public override void Init()
		{
			base.Init();
			this.BackButton = this.transform.Find("Anchor/BackButton");
			this.BlackListPanel = this.transform.Find("Anchor/BlackListPanel/Grid");
			UIEventListener expr_42 = UIEventListener.Get(this.BackButton.gameObject);
			expr_42.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_42.onClick, new UIEventListener.VoidDelegate(this.OnBackBtn));
			this.m_AlphaController = this.transform.GetComponent<TweenAlpha>();
		}

		public override void HandleAfterOpenView()
		{
			this.m_AlphaController.Begin();
			this.RefreshUI();
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_FriendList));
			MVC_MessageManager.AddListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg_ModifyFriendStatus));
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得黑名单信息", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
		}

		private void OnGetMsg_ModifyFriendStatus(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.TryModifyFriendStatusEventCallback(num, operationResponse.DebugMessage, 0, 0L);
			}
			else
			{
				byte arg = (byte)operationResponse.Parameters[10];
				long arg2 = (long)operationResponse.Parameters[102];
				this.TryModifyFriendStatusEventCallback(num, operationResponse.DebugMessage, arg, arg2);
			}
		}

		private void TryModifyFriendStatusEventCallback(int arg1, string arg2, byte arg3, long arg4)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "等待服务器响应...", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_GetFriendList, param, new object[0]);
		}

		public override void RefreshUI()
		{
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_GetFriendList, new MobaMessageFunc(this.OnGetMsg_FriendList));
			MVC_MessageManager.RemoveListener_view(MobaFriendCode.Friend_ModifyFriendStatus, new MobaMessageFunc(this.OnGetMsg_ModifyFriendStatus));
		}

		private void OnGetMsg_FriendList(MobaMessage msg)
		{
			if (msg == null)
			{
				return;
			}
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse == null)
			{
				return;
			}
			int num = (int)operationResponse.Parameters[1];
			MobaErrorCode mobaErrorCode = (MobaErrorCode)num;
			if (mobaErrorCode != MobaErrorCode.Ok)
			{
				this.GetFriendList(num, operationResponse.DebugMessage, null);
			}
			else
			{
				List<FriendData> friendDataList = ModelManager.Instance.Get_blackList_X();
				this.GetFriendList(num, operationResponse.DebugMessage, friendDataList);
			}
		}

		private void GetFriendList(int arg1, string arg2, List<FriendData> _friendDataList)
		{
			this.StartGetFriendList();
		}

		private void StartGetFriendList()
		{
			GameObject friendItemPre = Resources.Load("Prefab/UI/Home/BlackListItem") as GameObject;
			this.CreateList(friendItemPre, this.GetBlackList(), 0);
			this.BlackListPanel.GetComponent<UIGrid>().Reposition();
		}

		private void CreateList(GameObject friendItemPre, List<FriendData> FriendDataList, int index = 0)
		{
			for (int i = 0; i < FriendDataList.Count; i++)
			{
				GameObject gameObject;
				if (i + index < this.BlackListPanel.childCount)
				{
					gameObject = this.BlackListPanel.GetChild(i + index).gameObject;
					gameObject.gameObject.SetActive(true);
				}
				else
				{
					gameObject = NGUITools.AddChild(this.BlackListPanel.gameObject, friendItemPre);
				}
				FriendItem component = gameObject.GetComponent<FriendItem>();
				component.apply.gameObject.name = FriendDataList[i].TargetId.ToString();
				component.refuse.gameObject.name = FriendDataList[i].TargetId.ToString();
				gameObject.name = FriendDataList[i].TargetId.ToString();
				SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(FriendDataList[i].PictureFrame.ToString());
				component.pictureFrame.spriteName = dataById.pictureframe_icon;
				SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(FriendDataList[i].Icon.ToString());
				component.texture.spriteName = dataById2.headportrait_icon;
				component.Name = FriendDataList[i].TargetName;
				component.name.text = FriendDataList[i].TargetName;
				component.id = FriendDataList[i].TargetId;
				component.level.text = CharacterDataMgr.instance.GetUserLevel(FriendDataList[i].Exp).ToString();
				if (UIEventListener.Get(component.transform.Find("RemoveBlackBtn").gameObject).onClick == null)
				{
					UIEventListener expr_1C4 = UIEventListener.Get(component.transform.Find("RemoveBlackBtn").gameObject);
					expr_1C4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_1C4.onClick, new UIEventListener.VoidDelegate(this.OnRemoveBlackBtn));
				}
			}
			int count = this.GetBlackList().Count;
			if (this.BlackListPanel.childCount > count)
			{
				for (int j = 0; j < this.BlackListPanel.childCount - count; j++)
				{
					this.BlackListPanel.GetChild(count + j).gameObject.SetActive(false);
				}
			}
		}

		private List<FriendData> GetBlackList()
		{
			return ModelManager.Instance.Get_blackList_X();
		}

		private void OnBackBtn(GameObject go)
		{
			CtrlManager.CloseWindow(WindowID.BlackListView);
			CtrlManager.OpenWindow(WindowID.SystemSettingView, null);
		}

		private void OnRemoveBlackBtn(GameObject go)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "黑名单移除中", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
			{
				long.Parse(go.transform.parent.name),
				4
			});
		}
	}
}
