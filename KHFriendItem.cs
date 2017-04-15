using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using ExitGames.Client.Photon;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class KHFriendItem : MonoBehaviour
{
	[SerializeField]
	private UILabel F_Name;

	[SerializeField]
	private UILabel F_Type;

	[SerializeField]
	private UISprite F_OnlineSign;

	[SerializeField]
	private UISprite F_OfflineSign;

	[SerializeField]
	private UILabel F_Invitation;

	[SerializeField]
	private UISprite F_Refuse;

	[SerializeField]
	private UISprite F_Summoner;

	[SerializeField]
	private UISprite F_SummonerFrame;

	[SerializeField]
	private UILabel F_GradeNumber;

	[SerializeField]
	private UILabel F_RefuseLabel;

	public Callback<GameObject> InviteFriend;

	private bool _beingInvited;

	private FriendData _data;

	private bool _ladderlockFlag;

	public RoomFriendStatus FriendStatus
	{
		get;
		private set;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		MobaMessageManager.UnRegistMessage(MobaTeamRoomCode.Room_RefuseJoinInvite, new MobaMessageFunc(this.OnMsg_RefuseJoinInvite));
	}

	public void Init(FriendData friendData, RoomFriendStatus net)
	{
		if (friendData == null)
		{
			return;
		}
		this.ResetInvitingState();
		this.FriendStatus = net;
		this._data = friendData;
		this.F_Name.text = friendData.TargetName;
		this.F_Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(friendData.CharmRankValue);
		this.F_GradeNumber.text = CharacterDataMgr.instance.GetUserLevel(friendData.Exp).ToString();
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(friendData.Icon.ToString());
		if (dataById == null)
		{
			this.F_Summoner.spriteName = "headportrait_0001";
		}
		else
		{
			this.F_Summoner.spriteName = dataById.headportrait_icon;
		}
		SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(friendData.PictureFrame.ToString());
		if (dataById2 == null)
		{
			this.F_SummonerFrame.spriteName = "pictureframe_0000";
		}
		else
		{
			this.F_SummonerFrame.spriteName = dataById2.pictureframe_icon;
		}
		UIEventListener.Get(this.F_Refuse.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickInvite);
		this.F_Invitation.text = string.Empty;
		this.SetGameState(this.FriendStatus);
		MobaMessageManager.RegistMessage(MobaTeamRoomCode.Room_RefuseJoinInvite, new MobaMessageFunc(this.OnMsg_RefuseJoinInvite));
	}

	public void SetGameState(RoomFriendStatus net)
	{
		this.FriendStatus = net;
		this._ladderlockFlag = false;
		if (net == RoomFriendStatus.Offline)
		{
			Behaviour arg_2D_0 = this.F_OfflineSign;
			bool flag = false;
			this.F_OnlineSign.enabled = flag;
			arg_2D_0.enabled = !flag;
			this.F_Type.color = Color.gray;
			this.SetInvitationBtnState(2);
			this.F_Type.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Offline");
			this.F_Invitation.text = string.Empty;
			base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(false);
		}
		else if (net == RoomFriendStatus.Online)
		{
			Behaviour arg_B1_0 = this.F_OfflineSign;
			bool flag = true;
			this.F_OnlineSign.enabled = flag;
			arg_B1_0.enabled = !flag;
			this.F_Type.color = Color.green;
			this.F_Type.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Online");
			int scene_limit_level = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>("80006").scene_limit_level;
			if (Singleton<PvpRoomView>.Instance.battleId == "80006" && int.Parse(this.F_GradeNumber.text) < scene_limit_level)
			{
				this._ladderlockFlag = true;
				base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(true);
				this.SetInvitationBtnState(2);
			}
			else
			{
				base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(false);
				this.SetInvitationBtnState(1);
			}
		}
		else if (net == RoomFriendStatus.Playing)
		{
			Behaviour arg_198_0 = this.F_OfflineSign;
			bool flag = true;
			this.F_OnlineSign.enabled = flag;
			arg_198_0.enabled = !flag;
			this.SetInvitationBtnState(2);
			this.F_Type.color = Color.yellow;
			this.F_Type.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Busyness");
			this.F_Invitation.text = string.Empty;
			base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(false);
		}
		else if (net == RoomFriendStatus.Refuse)
		{
			Behaviour arg_21D_0 = this.F_OfflineSign;
			bool flag = true;
			this.F_OnlineSign.enabled = flag;
			arg_21D_0.enabled = !flag;
			this.SetInvitationBtnState(2);
			this.F_Invitation.text = LanguageManager.Instance.GetStringById("GangUpUI_Decline");
			this.F_Invitation.color = Color.red;
			base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(false);
		}
		else
		{
			Behaviour arg_28B_0 = this.F_OfflineSign;
			bool flag = true;
			this.F_OnlineSign.enabled = flag;
			arg_28B_0.enabled = !flag;
			this.SetInvitationBtnState(2);
			this.F_Type.color = Color.green;
			this.F_Type.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Room");
			this.F_Invitation.text = string.Empty;
			base.transform.FindChild("RaidLevelLimit").gameObject.SetActive(false);
		}
	}

	public void InviteToRoom()
	{
		this._beingInvited = true;
		this.SetInvitationBtnState(3);
		base.Invoke("ResetInvitingState", 17f);
	}

	private void SetInvitationBtnState(byte state)
	{
		switch (state)
		{
		case 1:
			this.F_Refuse.spriteName = "play_mapchoice_invitation_btn_01";
			this.F_RefuseLabel.color = new Color32(20, 247, 222, 255);
			this.F_RefuseLabel.text = "邀请";
			this.F_Refuse.GetComponent<BoxCollider>().enabled = true;
			break;
		case 2:
			this.F_Refuse.spriteName = "play_mapchoice_invitation_btn_02";
			this.F_RefuseLabel.color = Color.gray;
			this.F_RefuseLabel.text = "邀请";
			this.F_Refuse.GetComponent<BoxCollider>().enabled = false;
			break;
		case 3:
			this.F_Refuse.spriteName = "play_mapchoice_invitation_btn_03";
			this.F_RefuseLabel.color = new Color32(0, 230, 108, 255);
			this.F_RefuseLabel.text = LanguageManager.Instance.GetStringById("GangUpUI_Inviting");
			this.F_Refuse.GetComponent<BoxCollider>().enabled = false;
			break;
		}
	}

	private void ResetInvitingState()
	{
		if (base.gameObject == null)
		{
			return;
		}
		base.CancelInvoke("ResetInvitingState");
		this._beingInvited = false;
		this.SetInvitationBtnState(1);
	}

	private void OnClickInvite(GameObject obj = null)
	{
		if (this._beingInvited)
		{
			return;
		}
		switch (this.FriendStatus)
		{
		case RoomFriendStatus.Online:
			if (this._ladderlockFlag)
			{
				Singleton<TipView>.Instance.ShowViewSetText("好友未达到6级", 1f);
				return;
			}
			if (this.InviteFriend != null)
			{
				this.InviteFriend(obj);
			}
			break;
		case RoomFriendStatus.Offline:
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Offline"), 1f);
			return;
		case RoomFriendStatus.InRoom:
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_HeHasAlreadyInTheRoom"), 1f);
			return;
		case RoomFriendStatus.Refuse:
			if (this.InviteFriend != null)
			{
				this.InviteFriend(obj);
			}
			break;
		}
	}

	private void OnMsg_RefuseJoinInvite(MobaMessage msg)
	{
		if (msg != null)
		{
			OperationResponse operationResponse = msg.Param as OperationResponse;
			if (operationResponse != null && operationResponse.Parameters.ContainsKey(40))
			{
				byte[] buffer = operationResponse.Parameters[40] as byte[];
				NotificationData notificationData = SerializeHelper.Deserialize<NotificationData>(buffer);
				if (notificationData != null && notificationData.Type == 12)
				{
					string[] array = notificationData.Content.Split(new char[]
					{
						'|'
					});
					if (array.Length == 3 && array[2].Equals(this._data.SummId.ToString()))
					{
						this.ResetInvitingState();
					}
				}
			}
		}
	}
}
