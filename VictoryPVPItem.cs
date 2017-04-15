using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros.Pvp;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class VictoryPVPItem : MonoBehaviour
{
	[SerializeField]
	private GameObject P_HeroItem;

	[SerializeField]
	private GameObject P_MvpIcon;

	[SerializeField]
	private UILabel P_KillHero;

	[SerializeField]
	private UILabel P_KillMonster;

	[SerializeField]
	private UILabel P_KillAssist;

	[SerializeField]
	private UILabel P_Kill;

	[SerializeField]
	private Transform P_Frame;

	[SerializeField]
	private UISprite P_AddFriend;

	[SerializeField]
	private UISprite P_AddGood;

	[SerializeField]
	private UISprite P_BG;

	[SerializeField]
	private UISprite P_BG1;

	[SerializeField]
	private UILabel P_SummonerName;

	[SerializeField]
	private GameObject Sign;

	[SerializeField]
	private UISprite[] P_Equips;

	[SerializeField]
	private UISprite P_Report;

	public VictPlayerData mPlayerData;

	private VictPlayerType playerType;

	private bool _IsAlly;

	private Callback<GameObject> AddFriend;

	public void RegisteListener()
	{
		UIEventListener.Get(this.P_AddFriend.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAddFriend);
		UIEventListener.Get(this.P_AddGood.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAddGood);
		UIEventListener.Get(this.P_Report.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickReport);
		this.P_AddFriend.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_AddFriends");
		this.P_AddGood.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_ClickALike");
		this.P_AddFriend.transform.FindChild("fly").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
	}

	private string ThousandFormat(int _num)
	{
		if (_num < 1000)
		{
			return _num.ToString();
		}
		return ((float)_num / 1000f).ToString("F1") + "K";
	}

	public void Init(HeroData data, VictPlayerType type, VictPlayerData playerData, bool isSelf = false, bool isFriend = false)
	{
		this.mPlayerData = playerData;
		this._IsAlly = Singleton<PvpManager>.Instance.IsOurPlayer(int.Parse(playerData.UserID));
		this.playerType = type;
		NGUITools.SetActiveChildren(this.P_HeroItem, false);
		GameObject gameObject = NGUITools.AddChild(this.P_HeroItem, Resources.Load<GameObject>("Prefab/NewUI/SelectHero/SelectHeroItem"));
		NewHeroItem component = gameObject.GetComponent<NewHeroItem>();
		component.Init(data, NewHeroItem.CardType.HeroAvator, true, true);
		gameObject.name = data.HeroID;
		if (isSelf)
		{
			this.P_SummonerName.color = new Color32(253, 173, 41, 255);
		}
		this.P_SummonerName.text = playerData.SummonerName;
		if (playerData.LastChamRank <= 50)
		{
			this.P_SummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(playerData.LastChamRank);
		}
		this.P_KillHero.text = playerData.KillHero.ToString();
		this.P_KillMonster.text = this.ThousandFormat(playerData.Gold);
		this.P_Kill.text = playerData.Death.ToString();
		this.P_KillAssist.text = playerData.KillAssist.ToString();
		if (Singleton<PvpManager>.Instance.IsObserver || ((LevelManager.m_CurLevel.IsFightWithRobot() || LevelManager.m_CurLevel.IsBattleNewbieGuide()) && type == VictPlayerType.BL))
		{
			this.P_AddFriend.gameObject.SetActive(false);
			this.P_AddGood.gameObject.SetActive(false);
		}
		else
		{
			this.P_AddFriend.gameObject.SetActive(!isSelf && !isFriend);
			this.P_AddFriend.name = playerData.SummonerId;
			this.P_AddGood.gameObject.SetActive(!isSelf);
			this.P_AddGood.name = playerData.UserID;
		}
		this.RegisteListener();
		this.SetEquipment(playerData);
		if (playerData.FirstKill)
		{
			this.Sign.gameObject.SetActive(true);
		}
		this.P_MvpIcon.SetActive(ModelManager.Instance.Get_Settle_MvpUserId() == playerData.UserID);
	}

	public void SetEquipment(VictPlayerData playerData)
	{
		if (playerData.HeroEquipmentList == null)
		{
			return;
		}
		for (int i = 0; i < this.P_Equips.Length; i++)
		{
			if (i >= playerData.HeroEquipmentList.Count)
			{
				this.P_Equips[i].enabled = false;
			}
			else if (this.P_Equips[i].name != playerData.HeroEquipmentList[i])
			{
				if (playerData.HeroEquipmentList[i] == string.Empty)
				{
					this.P_Equips[i].spriteName = null;
				}
				else
				{
					SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(playerData.HeroEquipmentList[i]);
					if (dataById != null)
					{
						this.P_Equips[i].spriteName = dataById.icon;
					}
				}
			}
		}
	}

	public void SetSummonerName(string _name)
	{
		this.P_SummonerName.text = _name;
	}

	public void SetAddFriendShow(bool _bool)
	{
		this.P_AddFriend.gameObject.SetActive(_bool);
	}

	public void UpdateInfo(HeroData data, VictPlayerType type, VictPlayerData playerData, bool isSelf = false)
	{
		this.P_SummonerName.text = playerData.SummonerName;
		this.P_KillHero.text = playerData.KillHero.ToString();
		this.P_KillMonster.text = this.ThousandFormat(playerData.Gold);
		this.P_Kill.text = playerData.Death.ToString();
		this.P_KillAssist.text = playerData.KillAssist.ToString();
		this.P_Frame.gameObject.SetActive(isSelf);
		if (playerData.FirstKill)
		{
			this.Sign.gameObject.SetActive(true);
		}
	}

	private void IsLM(VictPlayerType isLM)
	{
		if (isLM == VictPlayerType.LM)
		{
			return;
		}
		UIWidget[] componentsInChildren = base.transform.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren == null)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].pivot == UIWidget.Pivot.Left)
			{
				componentsInChildren[i].pivot = UIWidget.Pivot.Center;
				componentsInChildren[i].transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				componentsInChildren[i].pivot = UIWidget.Pivot.Right;
			}
			else
			{
				componentsInChildren[i].transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			}
		}
	}

	public void ShowReport()
	{
		if (this.P_AddGood.gameObject.activeInHierarchy)
		{
			if (this.playerType == VictPlayerType.LM)
			{
				TweenPosition.Begin(this.P_Report.gameObject, 0.2f, new Vector3(-338f, 0f, 0f));
				TweenPosition.Begin(this.P_AddGood.gameObject, 0.2f, new Vector3(-587f, 8f, 0f));
				TweenPosition.Begin(this.P_AddFriend.gameObject, 0.2f, new Vector3(-700f, 8f, 0f));
			}
			else
			{
				TweenPosition.Begin(this.P_Report.gameObject, 0.2f, new Vector3(338f, 0f, 0f));
				TweenPosition.Begin(this.P_AddGood.gameObject, 0.2f, new Vector3(587f, 8f, 0f));
				TweenPosition.Begin(this.P_AddFriend.gameObject, 0.2f, new Vector3(700f, 8f, 0f));
			}
		}
	}

	public void HideReport()
	{
		if (this.P_AddGood.gameObject.activeInHierarchy)
		{
			if (this.playerType == VictPlayerType.LM)
			{
				TweenPosition.Begin(this.P_Report.gameObject, 0.2f, new Vector3(-548f, 0f, 0f));
				TweenPosition.Begin(this.P_AddGood.gameObject, 0.2f, new Vector3(-287f, 8f, 0f));
				TweenPosition.Begin(this.P_AddFriend.gameObject, 0.2f, new Vector3(-400f, 8f, 0f));
			}
			else
			{
				TweenPosition.Begin(this.P_Report.gameObject, 0.2f, new Vector3(549f, 0f, 0f));
				TweenPosition.Begin(this.P_AddGood.gameObject, 0.2f, new Vector3(287f, 8f, 0f));
				TweenPosition.Begin(this.P_AddFriend.gameObject, 0.2f, new Vector3(396f, 8f, 0f));
			}
		}
	}

	public void ConfirmReport(MobaMessage msg)
	{
		bool flag = (bool)msg.Param;
		if (flag)
		{
			this.P_Report.collider.enabled = false;
			this.P_Report.GetComponent<UIButton>().state = UIButtonColor.State.Disabled;
		}
		MobaMessageManager.UnRegistMessage((ClientMsg)21031, new MobaMessageFunc(this.ConfirmReport));
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
		long num2 = long.Parse(friendId);
		if (num2 > 0L && !SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, null, new object[]
		{
			num2
		}))
		{
			Singleton<TipView>.Instance.ShowViewSetText(":(申请发送失败", 1f);
			return;
		}
		this.P_AddFriend.GetComponent<TweenScale>().PlayForward();
		this.P_AddFriend.transform.FindChild("fly").gameObject.SetActive(true);
		this.P_AddFriend.spriteName = "Settlement_add_friends_02";
		this.P_AddFriend.transform.FindChild("label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
		this.P_AddFriend.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_HasBeenSend");
		UIEventListener.Get(this.P_AddFriend.gameObject).onClick = null;
	}

	private void ClickAddGood(GameObject _obj = null)
	{
		long num = long.Parse(this.P_AddGood.name);
		if (num > 0L && !SendMsgManager.Instance.SendMsg(MobaGameCode.SayGoodToSomeOne, null, new object[]
		{
			this.P_AddGood.name
		}))
		{
			Singleton<TipView>.Instance.ShowViewSetText(":(发送赞失败", 1f);
			return;
		}
		this.P_AddGood.GetComponent<TweenScale>().PlayForward();
		this.P_AddGood.transform.FindChild("fly").gameObject.SetActive(true);
		this.P_AddGood.spriteName = "Settlement_praise_02";
		this.P_AddGood.transform.FindChild("label").GetComponent<UILabel>().color = new Color32(31, 249, 116, 255);
		this.P_AddGood.transform.FindChild("label").GetComponent<UILabel>().text = LanguageManager.Instance.GetStringById("BattleSettlement_Like");
		UIEventListener.Get(this.P_AddGood.gameObject).onClick = null;
	}

	public void AddDelegate(Callback<GameObject> addObj)
	{
		this.AddFriend = addObj;
	}
}
