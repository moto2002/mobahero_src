using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class ChaosPlayerInfo_Settlement : MonoBehaviour
{
	public TeamType teamType;

	public int id;

	public UITexture headIcon;

	public UILabel summonerName;

	public GameObject reward;

	public UILabel kdaLabel;

	public UILabel goldLabel;

	public UISprite[] equips;

	public UISprite addFriend;

	public UISprite addGood;

	public UISprite report;

	private VictPlayerData mPlayerData;

	private bool _IsAlly;

	private void Start()
	{
		if (this.teamType == TeamType.LM)
		{
			this.summonerName.color = new Color32(244, 0, 161, 255);
		}
		else if (this.teamType == TeamType.BL)
		{
			this.summonerName.color = new Color32(49, 253, 255, 255);
		}
		else if (this.teamType == TeamType.Team_3)
		{
			this.summonerName.color = new Color32(253, 233, 52, 255);
		}
	}

	private void Update()
	{
	}

	public void Init(HeroData _heroData, bool _win, VictPlayerData _playerData, bool isSelf = false, bool isFriend = false)
	{
		this._IsAlly = Singleton<PvpManager>.Instance.IsOurPlayer(int.Parse(_playerData.UserID));
		this.mPlayerData = _playerData;
		this.reward.gameObject.SetActive(_win);
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(_heroData.HeroID);
		this.headIcon.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
		this.summonerName.text = _playerData.SummonerName;
		this.kdaLabel.text = string.Format("{0}/{1}/{2}", _playerData.KillHero, _playerData.Death, _playerData.KillAssist);
		this.goldLabel.text = this.ThousandFormat(_playerData.Gold);
		if (Singleton<PvpManager>.Instance.IsObserver)
		{
			this.addFriend.gameObject.SetActive(false);
			this.addGood.gameObject.SetActive(false);
			this.report.gameObject.SetActive(false);
		}
		else
		{
			this.addFriend.gameObject.SetActive(!isSelf && !isFriend);
			this.addFriend.name = _playerData.SummonerId;
			this.addGood.gameObject.SetActive(!isSelf);
			this.addGood.name = _playerData.UserID;
			this.report.gameObject.SetActive(!isSelf);
		}
		UIEventListener.Get(this.addFriend.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAddFriend);
		UIEventListener.Get(this.addGood.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAddGood);
		UIEventListener.Get(this.report.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickReport);
		this.SetEquipment(_playerData);
	}

	private void SetEquipment(VictPlayerData playerData)
	{
		if (playerData.HeroEquipmentList == null)
		{
			for (int i = 0; i < this.equips.Length; i++)
			{
				this.equips[i].transform.FindChild("NullMask").gameObject.SetActive(true);
			}
			return;
		}
		for (int j = 0; j < this.equips.Length; j++)
		{
			if (j >= playerData.HeroEquipmentList.Count)
			{
				this.equips[j].enabled = false;
			}
			else if (this.equips[j].name != playerData.HeroEquipmentList[j])
			{
				if (playerData.HeroEquipmentList[j] == string.Empty)
				{
					this.equips[j].transform.FindChild("NullMask").gameObject.SetActive(true);
				}
				else
				{
					SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(playerData.HeroEquipmentList[j]);
					if (dataById != null)
					{
						this.equips[j].spriteName = dataById.icon;
						this.equips[j].transform.FindChild("NullMask").gameObject.SetActive(false);
					}
				}
			}
		}
	}

	private string ThousandFormat(int _num)
	{
		if (_num < 1000)
		{
			return _num.ToString();
		}
		return ((float)_num / 1000f).ToString("F1") + "K";
	}

	private void ClickReport(GameObject _obj = null)
	{
		object[] msgParam = new object[]
		{
			this.mPlayerData,
			this._IsAlly
		};
		MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)21030, msgParam, 0f);
		MobaMessageManager.ExecuteMsg(message);
		MobaMessageManager.RegistMessage((ClientMsg)21031, new MobaMessageFunc(this.ConfirmReport));
	}

	public void ConfirmReport(MobaMessage msg)
	{
		bool flag = (bool)msg.Param;
		if (flag)
		{
			this.report.collider.enabled = false;
			this.report.GetComponent<UIButton>().state = UIButtonColor.State.Disabled;
		}
		MobaMessageManager.UnRegistMessage((ClientMsg)21031, new MobaMessageFunc(this.ConfirmReport));
	}

	private void ClickAddFriend(GameObject _obj = null)
	{
		string friendId = _obj.name;
		if (friendId == string.Empty)
		{
			return;
		}
		long num = ModelManager.Instance.Get_userData_filed_X("SummonerId");
		if (friendId == num.ToString())
		{
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_CanNotAddYouself"), 1f);
			return;
		}
		if (ModelManager.Instance.Get_FriendDataList_X().Find((FriendData obj) => obj.TargetId.ToString() == friendId) != null)
		{
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_HeIsYouFriend"), 1f);
			return;
		}
		if (!SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, null, new object[]
		{
			long.Parse(friendId)
		}))
		{
			Singleton<TipView>.Instance.ShowViewSetText(":(申请发送失败", 1f);
			return;
		}
		this.addFriend.GetComponent<TweenScale>().PlayForward();
		this.addFriend.transform.FindChild("fly").gameObject.SetActive(true);
		this.addFriend.spriteName = "Settlement_add_friends_02";
		this.addFriend.transform.FindChild("label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
		this.addFriend.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
		UIEventListener.Get(this.addFriend.gameObject).onClick = null;
	}

	private void ClickAddGood(GameObject _obj = null)
	{
		if (!SendMsgManager.Instance.SendMsg(MobaGameCode.SayGoodToSomeOne, null, new object[]
		{
			this.addGood.name
		}))
		{
			Singleton<TipView>.Instance.ShowViewSetText(":(发送赞失败", 1f);
			return;
		}
		this.addGood.GetComponent<TweenScale>().PlayForward();
		this.addGood.transform.FindChild("fly").gameObject.SetActive(true);
		this.addGood.spriteName = "Settlement_praise_02";
		this.addGood.transform.FindChild("label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
		this.addGood.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Like");
		UIEventListener.Get(this.addGood.gameObject).onClick = null;
	}
}
