using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaHeros;
using MobaProtocol;
using MobaProtocol.Data;
using System;
using UnityEngine;

public class StatisticItem : MonoBehaviour
{
	[SerializeField]
	private GameObject P_HeroItem;

	[SerializeField]
	private UILabel P_KillHero;

	[SerializeField]
	private UILabel P_KillMonster;

	[SerializeField]
	private UILabel P_KillAssist;

	[SerializeField]
	private UILabel P_Kill;

	[SerializeField]
	private UISprite P_Frame;

	[SerializeField]
	private Transform P_AddFriend;

	[SerializeField]
	private UISprite P_BG;

	[SerializeField]
	private UISprite P_BG1;

	[SerializeField]
	private UISprite P_BG2;

	[SerializeField]
	private UILabel P_SummonerName;

	[SerializeField]
	private UITexture P_SummonerSkill;

	[SerializeField]
	private UISprite[] P_Equips;

	[SerializeField]
	private UISprite P_HeroPic;

	[SerializeField]
	private UILabel P_HeroLevel;

	[SerializeField]
	private UILabel LeftDeathTimeLabel;

	[SerializeField]
	private Material HeroMask1;

	[SerializeField]
	private Material HeroMask2;

	[SerializeField]
	private Transform EquipInfo;

	[SerializeField]
	private UILabel EquipName;

	[SerializeField]
	private UILabel Sell;

	[SerializeField]
	private UILabel Atrib;

	[SerializeField]
	private UILabel Describ;

	[SerializeField]
	private Transform EquipMask;

	[SerializeField]
	private UISprite SilencedIcon;

	private string silencedIcon_on = "Silenced_on";

	private string silencedIcon_off = "Silenced_off";

	private Callback<GameObject> AddFriend;

	public void Init(VictPlayerType side, HeroData data, VictPlayerType type, VictPlayerData playerData, bool isSelf = false, bool isFriend = false)
	{
		if (side == type)
		{
			this.P_Frame.color = new Color32(7, 154, 0, 255);
			this.P_BG2.color = new Color32(0, 154, 91, 171);
		}
		else
		{
			this.P_Frame.color = new Color32(222, 0, 0, 255);
			this.P_BG2.color = new Color32(132, 5, 5, 171);
		}
		isSelf = playerData.IsControlPlayer;
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(data.HeroID);
		this.P_HeroPic.spriteName = heroMainData.avatar_icon;
		if (LevelManager.Instance.IsPvpBattleType)
		{
			this.P_SummonerName.text = playerData.SummonerName;
			this.P_SummonerSkill.gameObject.SetActive(true);
			SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(playerData.SummonerSkillID);
			if (skillMainData != null)
			{
				this.P_SummonerSkill.mainTexture = ResourceManager.Load<Texture>(skillMainData.skill_icon, true, true, null, 0, false);
			}
			else
			{
				this.P_SummonerSkill.gameObject.SetActive(false);
			}
		}
		else
		{
			this.P_SummonerName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
			this.P_SummonerSkill.gameObject.SetActive(false);
		}
		this.SetIsHighestKillHero(playerData.isHighestKillHero);
		this.SetIsHighestKillMonster(playerData.isHighestKillMonster);
		this.P_HeroLevel.text = data.LV.ToString();
		this.P_HeroPic.parent.name = data.HeroID;
		this.P_KillHero.text = playerData.KillHero.ToString();
		this.P_KillMonster.text = playerData.KillMonster.ToString();
		this.P_Kill.text = "/" + playerData.Death.ToString();
		this.P_KillAssist.text = "/" + playerData.KillAssist.ToString();
		this.P_BG1.gameObject.SetActive(isSelf);
		if (isSelf)
		{
			this.P_SummonerName.color = new Color(0.9647059f, 0.9019608f, 0.360784322f);
		}
		else
		{
			this.P_SummonerName.color = Color.white;
		}
		if (playerData.SelfIsDeath && !GameManager.IsGameOver())
		{
			this.P_BG2.gameObject.SetActive(true);
			this.LeftDeathTimeLabel.text = playerData.LeftDeathTime.ToString();
		}
		else
		{
			this.P_BG2.gameObject.SetActive(false);
		}
		this.P_AddFriend.gameObject.SetActive(!isSelf && !isFriend);
		this.P_AddFriend.name = playerData.SummonerId;
		UIEventListener.Get(this.P_AddFriend.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickAddFriend);
		this.CheckEquipment(playerData);
		if (this.SilencedIcon != null)
		{
			UIEventListener.Get(this.SilencedIcon.gameObject).onClick = new UIEventListener.VoidDelegate(this.ClickBlock);
			if (Singleton<StatisticView>.Instance.blockedSummonerList.Contains(playerData.SummonerId))
			{
				this.SilencedOn(true);
			}
			else
			{
				this.SilencedOn(false);
			}
			if (!isSelf && side == type)
			{
				this.SilencedIcon.gameObject.SetActive(true);
			}
			else
			{
				this.SilencedIcon.gameObject.SetActive(false);
			}
		}
		if (playerData.LastChamRank <= 50)
		{
			this.P_SummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(playerData.LastChamRank);
		}
	}

	private void CheckEquipment(VictPlayerData playerData)
	{
		if (playerData.HeroEquipmentList == null)
		{
			return;
		}
		for (int i = 0; i < this.P_Equips.Length; i++)
		{
			if (!(this.P_Equips[i] == null))
			{
				if (i >= playerData.HeroEquipmentList.Count)
				{
					this.P_Equips[i].spriteName = null;
				}
				else if (this.P_Equips[i].name != playerData.HeroEquipmentList[i])
				{
					this.P_Equips[i].name = playerData.HeroEquipmentList[i];
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
						this.P_Equips[i].name = playerData.HeroEquipmentList[i];
						UIEventListener expr_114 = UIEventListener.Get(this.P_Equips[i].gameObject);
						expr_114.onMobileHover = (UIEventListener.BoolDelegate)Delegate.Combine(expr_114.onMobileHover, new UIEventListener.BoolDelegate(this.HoverEquip));
					}
				}
			}
		}
	}

	private void HoverEquip(GameObject go, bool state)
	{
		SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(go.name);
		this.EquipName.text = LanguageManager.Instance.GetStringById(dataById.name);
		this.Sell.text = dataById.sell.ToString();
		this.Atrib.text = this.GetAttriDes(go.name, dataById, "\n", 6);
		this.Describ.text = LanguageManager.Instance.GetStringById(dataById.describe);
		if (go.transform.parent.name == "EquipIconLM")
		{
			this.EquipInfo.localPosition = new Vector3(go.transform.localPosition.x + 314f, go.transform.localPosition.y + 75f, 0f);
		}
		else
		{
			this.EquipInfo.localPosition = new Vector3(go.transform.localPosition.x - 314f, go.transform.localPosition.y + 75f, 0f);
		}
		this.EquipInfo.gameObject.SetActive(state);
		this.EquipMask.gameObject.SetActive(state);
		this.EquipMask.localPosition = go.transform.localPosition;
	}

	private string GetAttriDes(string equipID, SysBattleItemsVo battleItem, string seporator = "\n", int maxNum = 6)
	{
		string text = string.Empty;
		if (!string.IsNullOrEmpty(battleItem.attribute))
		{
			string[] array = battleItem.attribute.Split(new char[]
			{
				','
			});
			int num = 0;
			while (num < array.Length && num < maxNum)
			{
				if (!string.IsNullOrEmpty(array[num]) && !("[]" == array[num]))
				{
					string[] array2 = array[num].Split(new char[]
					{
						'|'
					});
					if (array2.Length < 3)
					{
						Reminder.ReminderStr(equipID + " in battle_items.attrbuite.length < 3");
					}
					else
					{
						string format = array2[2];
						AttrType type = (AttrType)int.Parse(array2[0]);
						float num2;
						if (array2[1].Contains("%"))
						{
							string text2 = array2[1].Trim();
							num2 = float.Parse(text2.Substring(0, text2.Length - 1)) / 100f;
						}
						else
						{
							num2 = float.Parse(array2[1]);
						}
						string text3 = text;
						text = string.Concat(new string[]
						{
							text3,
							"+",
							num2.ToString(format),
							" ",
							CharacterDataMgr.instance.GetChinaName((int)type),
							seporator
						});
					}
				}
				num++;
			}
		}
		return text;
	}

	public void SetIsHighestKillMonster(bool isHighest)
	{
		if (isHighest)
		{
			this.P_KillMonster.color = new Color(0.9529412f, 0.745098054f, 0.1882353f);
		}
		else
		{
			this.P_KillMonster.color = new Color(0.980392158f, 0.8666667f, 0.5568628f);
		}
	}

	public void SetIsHighestKillHero(bool isHighest)
	{
		if (isHighest)
		{
			this.P_KillHero.color = new Color(0.9529412f, 0.745098054f, 0.1882353f);
		}
		else
		{
			this.P_KillHero.color = Color.white;
		}
	}

	public void SetSummonerName(string _name)
	{
	}

	public void SetAddFriendShow(bool _bool)
	{
		if (this.P_AddFriend != null)
		{
			this.P_AddFriend.gameObject.SetActive(_bool);
		}
	}

	public void UpdateInfo(HeroData data, VictPlayerType type, VictPlayerData playerData, bool isSelf = false)
	{
		isSelf = playerData.IsControlPlayer;
		this.P_HeroLevel.text = data.LV.ToString();
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(data.HeroID);
		if (LevelManager.Instance.IsPvpBattleType)
		{
			this.P_SummonerName.text = playerData.SummonerName;
		}
		else
		{
			this.P_SummonerName.text = LanguageManager.Instance.GetStringById(heroMainData.name);
		}
		this.SetIsHighestKillHero(playerData.isHighestKillHero);
		this.SetIsHighestKillMonster(playerData.isHighestKillMonster);
		this.P_KillHero.text = playerData.KillHero.ToString();
		this.P_KillMonster.text = playerData.KillMonster.ToString();
		this.P_Kill.text = "/" + playerData.Death.ToString();
		this.P_KillAssist.text = "/" + playerData.KillAssist.ToString();
		if (isSelf)
		{
			this.P_SummonerName.color = new Color(0.9647059f, 0.9019608f, 0.360784322f);
		}
		else
		{
			this.P_SummonerName.color = Color.white;
		}
		if (playerData.LastChamRank <= 50)
		{
			this.P_SummonerName.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(playerData.LastChamRank);
		}
		if (playerData.SelfIsDeath)
		{
			if (this.P_BG2 != null)
			{
				this.P_BG2.gameObject.SetActive(true);
			}
			if (this.LeftDeathTimeLabel != null)
			{
				this.LeftDeathTimeLabel.text = playerData.LeftDeathTime.ToString();
			}
		}
		else if (this.P_BG2 != null)
		{
			this.P_BG2.gameObject.SetActive(false);
		}
		this.CheckEquipment(playerData);
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

	private void SilencedOn(bool isSilenced)
	{
		if (isSilenced)
		{
			this.SilencedIcon.spriteName = this.silencedIcon_on;
		}
		else
		{
			this.SilencedIcon.spriteName = this.silencedIcon_off;
		}
	}

	private void ClickBlock(GameObject _obj = null)
	{
		if (this.SilencedIcon.spriteName == this.silencedIcon_off)
		{
			this.SilencedOn(true);
			if (!Singleton<StatisticView>.Instance.blockedSummonerList.Contains(this.P_AddFriend.name))
			{
				Singleton<StatisticView>.Instance.blockedSummonerList.Add(this.P_AddFriend.name);
			}
		}
		else
		{
			this.SilencedOn(false);
			if (Singleton<StatisticView>.Instance.blockedSummonerList.Contains(this.P_AddFriend.name))
			{
				Singleton<StatisticView>.Instance.blockedSummonerList.Remove(this.P_AddFriend.name);
			}
		}
	}

	private void ClickAddFriend(GameObject _obj = null)
	{
		string friendId = _obj.name;
		if (friendId == string.Empty)
		{
			Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("FriendsUI_Tips_PleaseInputSummonerID"), 1f);
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
		Singleton<TipView>.Instance.ShowViewSetText(LanguageManager.Instance.GetStringById("GangUpUI_Tips_MessageHasBeenSent"), 1f);
		SendMsgManager.Instance.SendMsg(MobaFriendCode.Friend_ApplyAddFriend, null, new object[]
		{
			long.Parse(friendId)
		});
	}

	public void AddDelegate(Callback<GameObject> addObj)
	{
		this.AddFriend = addObj;
	}
}
