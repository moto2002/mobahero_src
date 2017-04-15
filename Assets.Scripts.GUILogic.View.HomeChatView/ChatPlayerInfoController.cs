using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.HomeChatView
{
	public class ChatPlayerInfoController : MonoBehaviour
	{
		public GameObject mGameObject;

		public UISprite mHeadIcon;

		public UISprite mHeadFrame;

		public UILabel mSummonerLevelLabel;

		public UILabel mSummonerName;

		public UIToggle mStateToggle;

		public UILabel mStageLabel;

		public UILabel mBottleLabel;

		public UILabel mTeamLabel;

		public GameObject mAddFriend;

		public GameObject mShowInfo;

		public GameObject mBlockPlayer;

		private static string mStatePrefix = "排位赛积分: [EFB32F]";

		private static string mBottlePrefix = "小魔瓶等级: [EFB32F]";

		private static string mTeamPrefix = "战队: [EFB32F]";

		private bool isFriend;

		private AgentBaseInfo targetData;

		private long targetSummId;

		private void Awake()
		{
			MobaMessageManager.RegistMessage(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnMsg_GetPlayerData));
			UIEventListener.Get(this.mGameObject).onClick = new UIEventListener.VoidDelegate(this.Close);
			UIEventListener.Get(this.mAddFriend).onClick = new UIEventListener.VoidDelegate(this.OnClickAddFriend);
			UIEventListener.Get(this.mShowInfo).onClick = new UIEventListener.VoidDelegate(this.OnClickShowInfo);
			UIEventListener.Get(this.mBlockPlayer).onClick = new UIEventListener.VoidDelegate(this.OnClickBlock);
		}

		private void OnDestroy()
		{
			MobaMessageManager.UnRegistMessage(MobaGameCode.GetPlayerData, new MobaMessageFunc(this.OnMsg_GetPlayerData));
		}

		private void Close(GameObject obj = null)
		{
			this.mGameObject.SetActive(false);
		}

		private void OnClickAddFriend(GameObject obj = null)
		{
			if (this.targetData == null)
			{
				return;
			}
			if (!SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, null, new object[]
			{
				this.targetSummId
			}))
			{
				Singleton<TipView>.Instance.ShowViewSetText(":(申请发送失败", 1f);
				return;
			}
			Singleton<TipView>.Instance.ShowViewSetText("请求已发送", 1f);
		}

		private void OnClickShowInfo(GameObject obj = null)
		{
			SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
			SendMsgManager.Instance.SendMsg(MobaGameCode.GetPlayerData, param, new object[]
			{
				this.targetData.UserId.ToString()
			});
		}

		private void OnClickBlock(GameObject obj1 = null)
		{
			if (ModelManager.Instance.Get_blackList_X() != null && ModelManager.Instance.Get_blackList_X().Find((FriendData obj) => obj.TargetId == this.targetSummId) != null)
			{
				Singleton<TipView>.Instance.ShowViewSetText("已在黑名单中", 1f);
				return;
			}
			CtrlManager.ShowMsgBox(LanguageManager.Instance.GetStringById("FriendsUI_DialogBox_Title_AddBlackList"), LanguageManager.Instance.GetStringById("FriendsUI_DialogBox_Content_AddBlackList"), new Action<bool>(this.BoolMoveBlack), PopViewType.PopTwoButton, "确定", "取消", null);
		}

		private void BoolMoveBlack(bool bIsTrigger)
		{
			if (bIsTrigger && this.targetData != null)
			{
				SendMsgManager.SendMsgParam param = new SendMsgManager.SendMsgParam(true, LanguageManager.Instance.GetStringById("FriendsUI_Tips_AddBlackList"), true, 15f);
				SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ModifyFriendStatus, param, new object[]
				{
					this.targetSummId,
					5
				});
				if (!this.mAddFriend.activeInHierarchy)
				{
					this.mAddFriend.SetActive(true);
				}
				if (Singleton<FriendView>.Instance.newMessageList.Contains(ToolsFacade.Instance.GetSummIdByUserid(this.targetSummId)))
				{
					Singleton<FriendView>.Instance.newMessageList.Remove(ToolsFacade.Instance.GetSummIdByUserid(this.targetSummId));
					Singleton<MenuView>.Instance.UpdateFriendNew();
				}
				Singleton<HomeChatview>.Instance.UnFriended();
			}
		}

		private void OnMsg_GetPlayerData(MobaMessage msg)
		{
			if (msg.Param != null && !Singleton<RankView>.Instance.IsOpen)
			{
				CtrlManager.OpenWindow(WindowID.DetailsInfo, null);
				Singleton<DetailsInfo>.Instance.ShowDetailsInfo_withoutBottleRank(this.targetData.UserId.ToString());
			}
		}

		public void Hide()
		{
			this.mGameObject.SetActive(false);
		}

		public void ShowPlayer(AgentBaseInfo _data)
		{
			this.mGameObject.SetActive(true);
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(_data.head.ToString());
			SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(_data.headFrame.ToString());
			this.mHeadIcon.spriteName = dataById.headportrait_icon;
			this.mHeadFrame.spriteName = dataById2.pictureframe_icon;
			this.targetData = _data;
			this.targetSummId = ((_data.UserId <= 100000L) ? _data.UserId : (_data.UserId - 100000L));
			this.mSummonerLevelLabel.text = _data.Level.ToString();
			this.mSummonerName.text = _data.NickName.ToString();
			this.mSummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(_data.CharmRankvalue);
			this.mStageLabel.text = ChatPlayerInfoController.mStatePrefix + _data.Ladder.ToString();
			this.mBottleLabel.text = ChatPlayerInfoController.mBottlePrefix + _data.BotLevel.ToString();
			if (string.IsNullOrEmpty(_data.TeamName))
			{
				this.mTeamLabel.gameObject.SetActive(false);
			}
			else
			{
				this.mTeamLabel.gameObject.SetActive(true);
				this.mTeamLabel.text = ChatPlayerInfoController.mTeamPrefix + _data.TeamName.ToString();
			}
			this.isFriend = (ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId == this.targetSummId) != null);
			this.mAddFriend.SetActive(!this.isFriend);
		}
	}
}
