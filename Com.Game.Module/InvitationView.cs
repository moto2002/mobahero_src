using Assets.Scripts.Model;
using Com.Game.Utils;
using ExitGames.Client.Photon;
using GUIFramework;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class InvitationView : BaseView<InvitationView>
	{
		private UIGrid grid;

		private List<InvitationItem> invitationItemList;

		private CoroutineManager coroutineManager = new CoroutineManager();

		private string saveData = string.Empty;

		public InvitationView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/InvitationView");
		}

		public override void Init()
		{
			this.grid = this.transform.Find("Anchor/Panel").GetComponent<UIGrid>();
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Join, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Levea, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_StartGame, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ChangeTeamType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Destory, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_Kick, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_CurrData, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ChangeRoomType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ComeBack, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_ExchangeTeamType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_OwnerQuit, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_PlayerEscapeCD, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.AddListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Join, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Levea, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_StartGame, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ChangeTeamType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Destory, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_Kick, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_CurrData, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ChangeRoomType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ComeBack, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_ExchangeTeamType, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_OwnerQuit, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_PlayerEscapeCD, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
			MVC_MessageManager.RemoveListener_view(MobaTeamRoomCode.Room_InviteJoinRoom, new MobaMessageFunc(this.OnGetMsg_RoomsManager));
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void OnGetMsg_RoomsManager(MobaMessage msg)
		{
			if (Singleton<PvpRoomView>.Instance.gameObject != null && Singleton<PvpRoomView>.Instance.gameObject.activeSelf)
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
				if (mobaErrorCode != MobaErrorCode.TableNotExist)
				{
					Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_JoinTheRoom"), 1f);
				}
				else
				{
					Singleton<TipView>.Instance.ShowViewSetText("房间已经不存在", 1f);
				}
			}
		}

		public void AddInvitation(string id, string title, string content, string summerId, InvitateType type = InvitateType.KHOrZDY)
		{
			if (SceneManager.Instance.CurSceneType == SceneType.Home && !Singleton<PvpRoomView>.Instance.IsOpen && !Singleton<PvpSelectHeroView>.Instance.IsOpen)
			{
				if (!Singleton<InvitationView>.Instance.IsOpen)
				{
					CtrlManager.OpenWindow(WindowID.InvitationView, null);
				}
				this.coroutineManager.StartCoroutine(this.AddOneNotice(id, title, content, summerId, type), true);
			}
			else
			{
				this.SaveInvitation(id, title, content, summerId, type);
			}
		}

		public void CheackSaveData()
		{
			if (this.saveData != string.Empty)
			{
				string[] array = this.saveData.Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						'&'
					});
					int num;
					if (array2.Length > 4 && int.TryParse(array2[4], out num))
					{
						this.AddInvitation(array2[0], array2[1], array2[2], array2[3], (InvitateType)int.Parse(array2[4]));
					}
					else
					{
						ClientLogger.Error(string.Concat(new object[]
						{
							"something wrong _datas[",
							i,
							"]=",
							array[i]
						}));
					}
				}
			}
			this.saveData = string.Empty;
		}

		private void SaveInvitation(string id, string title, string content, string summerId, InvitateType type = InvitateType.KHOrZDY)
		{
			if (this.saveData != string.Empty)
			{
				this.saveData = string.Concat(new object[]
				{
					this.saveData,
					"|",
					id,
					"&",
					title,
					"&",
					content,
					"&",
					summerId,
					"&",
					(int)type
				});
			}
			else
			{
				this.saveData = string.Concat(new object[]
				{
					id,
					"&",
					title,
					"&",
					content,
					"&",
					summerId,
					"&",
					(int)type
				});
			}
		}

		[DebuggerHidden]
		private IEnumerator AddOneNotice(string id, string title, string content, string summerId, InvitateType type = InvitateType.KHOrZDY)
		{
			InvitationView.<AddOneNotice>c__Iterator13A <AddOneNotice>c__Iterator13A = new InvitationView.<AddOneNotice>c__Iterator13A();
			<AddOneNotice>c__Iterator13A.summerId = summerId;
			<AddOneNotice>c__Iterator13A.id = id;
			<AddOneNotice>c__Iterator13A.type = type;
			<AddOneNotice>c__Iterator13A.title = title;
			<AddOneNotice>c__Iterator13A.content = content;
			<AddOneNotice>c__Iterator13A.<$>summerId = summerId;
			<AddOneNotice>c__Iterator13A.<$>id = id;
			<AddOneNotice>c__Iterator13A.<$>type = type;
			<AddOneNotice>c__Iterator13A.<$>title = title;
			<AddOneNotice>c__Iterator13A.<$>content = content;
			<AddOneNotice>c__Iterator13A.<>f__this = this;
			return <AddOneNotice>c__Iterator13A;
		}

		public void GetResult(string id, bool isAccept, string targetSummerId, InvitateType type = InvitateType.KHOrZDY)
		{
			if (type == InvitateType.KHOrZDY)
			{
				Singleton<PvpRoomView>.Instance.InvitationToRoom(id, isAccept, targetSummerId);
				if (Singleton<FriendView>.Instance.IsOpen && isAccept)
				{
					CtrlManager.CloseWindow(WindowID.FriendView);
				}
			}
			else
			{
				if (isAccept)
				{
					SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, null, new object[]
					{
						long.Parse(id),
						1
					});
				}
				else
				{
					SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, null, new object[]
					{
						long.Parse(id),
						2
					});
				}
				Singleton<MenuView>.Instance.RemoveNews(14, "0");
			}
		}

		public void FixGrid()
		{
			this.grid.Reposition();
		}

		public void AddFriendApplyList(List<FriendData> friendDatList)
		{
			for (int i = 0; i < friendDatList.Count; i++)
			{
				this.AddInvitation(friendDatList[i].TargetId.ToString(), "好友申请", friendDatList[i].TargetName + "申请添加你为好友！", friendDatList[i].SummId.ToString(), InvitateType.Friend);
			}
		}
	}
}
