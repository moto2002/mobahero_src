using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FriendItem : MonoBehaviour
{
	public UILabel level;

	public new UILabel name;

	public Transform applyState;

	public Transform friendState;

	public UISprite texture;

	public UISprite pictureFrame;

	public UISprite sign;

	public UISprite refuse;

	public UISprite apply;

	public UISprite go;

	public UISprite select;

	public int friendType;

	public string Name = string.Empty;

	public string State = string.Empty;

	public int RankValue;

	public int WinValue;

	public long id;

	public int gameState;

	public List<Messages> messages;

	public UILabel gameStateLabel;

	public UISprite MessageSign;

	public bool isInitMessage;

	public UIButton btnObserve;

	public FriendData data;

	public bool isSelect;

	public int lastCharmRank;

	private void Awake()
	{
		UIEventListener expr_0B = UIEventListener.Get(base.gameObject);
		expr_0B.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_0B.onClick, new UIEventListener.VoidDelegate(this.OnClickItem));
	}

	private void OnClickItem(GameObject obj = null)
	{
		Singleton<FriendView>.Instance.selectid = base.gameObject.name;
		this.select.gameObject.SetActive(true);
		if (Singleton<FriendView>.Instance.TempObj != obj)
		{
			Singleton<FriendView>.Instance.CheckselectObj(this);
			if (Singleton<FriendView>.Instance.TempObj != null)
			{
				Singleton<FriendView>.Instance.TempObj.GetComponent<FriendItem>().isSelect = false;
				Singleton<FriendView>.Instance.TempObj.GetComponent<FriendItem>().select.gameObject.SetActive(false);
			}
		}
		Singleton<FriendView>.Instance.TempObj = obj;
	}

	public void applyStatus()
	{
		this.applyState.gameObject.SetActive(true);
		this.friendState.gameObject.SetActive(false);
		this.refuse.gameObject.SetActive(true);
		this.apply.gameObject.SetActive(true);
		this.sign.gameObject.SetActive(false);
		this.go.gameObject.SetActive(false);
		this.btnObserve.gameObject.SetActive(false);
	}

	public void friendStatus()
	{
		this.applyState.gameObject.SetActive(false);
		this.friendState.gameObject.SetActive(true);
		this.refuse.gameObject.SetActive(false);
		this.apply.gameObject.SetActive(false);
		this.sign.gameObject.SetActive(true);
		this.go.gameObject.SetActive(true);
	}

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
		this.btnObserve.gameObject.SetActive(false);
		if ((int)type == 1)
		{
			this.sign.spriteName = "Friend_icon_online";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Online");
		}
		else if ((int)type == 0)
		{
			this.sign.spriteName = "Friend_icon_offline";
			this.gameStateLabel.color = new Color(0.5058824f, 0.5058824f, 0.5058824f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Offline");
		}
		else if ((int)type == 2)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Busyness");
		}
		else if ((int)type == 3)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Leave");
		}
		else if ((int)type == 51)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Ready");
		}
		else if ((int)type == 50)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Queue");
		}
		else if ((int)type == 52)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_PVP");
			this.btnObserve.gameObject.SetActive(true);
		}
		else if ((int)type == 6)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = "在纸牌屋中战斗";
		}
		else if ((int)type == 5)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = "在小乱斗中战斗";
		}
		else if ((int)type == 4)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Battle");
		}
		else if ((int)type == 7)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Room");
		}
		else if ((int)type == 53)
		{
			this.sign.spriteName = "Friend_icon_offline";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Reconnect");
		}
		else if ((int)type == 54)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(0.07058824f, 0.870588243f, 0.34117648f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_TeamMatch");
		}
		else if ((int)type == 55)
		{
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = "观战中";
		}
		else
		{
			ClientLogger.Error("unknown status " + type);
			this.sign.spriteName = "Friend_icon_gaming";
			this.gameStateLabel.color = new Color(1f, 0.870588243f, 0.152941182f);
			this.gameStateLabel.text = LanguageManager.Instance.GetStringById("FriendsUI_FriendState_Hide");
		}
	}
}
