using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendItemInHomeChat : MonoBehaviour
{
	public UISprite Avatar;

	public UISprite Frame;

	public UISprite Statu;

	public UILabel Name;

	public UILabel StatuLabel;

	public Transform MessageSign;

	private FriendData mFriendData;

	public void ShowMessageSign()
	{
		this.MessageSign.gameObject.SetActive(true);
	}

	public void HideMessageSign()
	{
		this.MessageSign.gameObject.SetActive(false);
	}

	public void SetGameState(GameStatus type)
	{
		if ((int)type == 0)
		{
			this.Statu.spriteName = "Home_chatting_friend_state_offline";
			this.StatuLabel.text = "离线";
		}
		else
		{
			this.Statu.spriteName = "Home_chatting_friend_state_online";
			this.StatuLabel.text = "在线";
		}
	}

	public void Initialize(FriendData data)
	{
		SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(data.Icon.ToString());
		if (dataById != null)
		{
			this.Avatar.spriteName = dataById.headportrait_icon.ToString();
		}
		else
		{
			this.Avatar.spriteName = "headportrait_0001";
		}
		SysSummonersPictureframeVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(data.PictureFrame.ToString());
		if (dataById2 != null)
		{
			this.Frame.spriteName = dataById2.pictureframe_icon;
		}
		else
		{
			this.Frame.spriteName = "pictureframe_0000";
		}
		this.Name.text = data.TargetName;
		this.Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(data.CharmRankValue);
		this.SetGameState((GameStatus)data.GameStatus);
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickToChat);
	}

	private void ClickToChat(GameObject go)
	{
		Singleton<HomeChatview>.Instance.FriendLst.gameObject.SetActive(false);
		Singleton<HomeChatview>.Instance.FriendChat.gameObject.SetActive(true);
		Singleton<HomeChatview>.Instance._friendChat.selectFriendItem = go.name;
		Singleton<HomeChatview>.Instance._friendChat.ClearChatBox();
		if (Singleton<FriendView>.Instance.newMessageList.Contains(long.Parse(go.name)))
		{
			Singleton<FriendView>.Instance.newMessageList.Remove(long.Parse(go.name));
		}
		this.HideMessageSign();
		Singleton<HomeChatview>.Instance.UpdateNewsPoint();
		Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
		dictionary.Add(102, go.name);
		NetWorkHelper.Instance.client.SendSessionChannelMessage(7, MobaChannel.Chat, dictionary);
	}
}
