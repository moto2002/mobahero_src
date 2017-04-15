using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleRecord : MonoBehaviour
{
	public Transform bg1;

	public Transform LightFrame;

	public Transform bg;

	public UILabel battleName;

	public Transform vectory;

	public Transform defeat;

	public UITexture HeroAvatar;

	public UISprite mvp;

	public Transform info;

	public UILabel killCount;

	public UILabel deathCount;

	public UILabel assistCount;

	public Transform achive;

	public GameObject Friend;

	public UILabel date;

	private GameObject AchiSprite;

	private KDAData recordData;

	public static GameObject TempObj;

	private void Awake()
	{
		UIEventListener expr_10 = UIEventListener.Get(this.bg.gameObject);
		expr_10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_10.onClick, new UIEventListener.VoidDelegate(this.OnClickItemInMain));
		UIEventListener expr_41 = UIEventListener.Get(this.bg1.gameObject);
		expr_41.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_41.onClick, new UIEventListener.VoidDelegate(this.OnClickItem));
	}

	private void Update()
	{
	}

	public GameObject Bg1()
	{
		return this.bg1.gameObject;
	}

	public void init(bool isMain, int idx, heroRecordInfo infos)
	{
		this.AchiSprite = Resources.Load<GameObject>("Prefab/UI/Achievement/AchiSprite");
		if (isMain)
		{
			this.bg1.gameObject.SetActive(false);
			this.bg.gameObject.SetActive(true);
			this.info.gameObject.SetActive(true);
		}
		else
		{
			this.bg1.gameObject.SetActive(true);
			this.bg.gameObject.SetActive(false);
			this.info.gameObject.SetActive(false);
			this.LightFrame.gameObject.SetActive(false);
		}
		if (infos.win)
		{
			if (infos.mvp)
			{
				this.mvp.gameObject.SetActive(true);
			}
			else
			{
				this.mvp.gameObject.SetActive(false);
			}
			this.vectory.gameObject.SetActive(true);
			this.defeat.gameObject.SetActive(false);
		}
		else
		{
			this.mvp.gameObject.SetActive(false);
			this.vectory.gameObject.SetActive(false);
			this.defeat.gameObject.SetActive(true);
		}
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(infos.heroid);
		this.HeroAvatar.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
		if (infos.playercounter != null)
		{
			this.killCount.text = infos.playercounter.killHoreCount.ToString();
			this.deathCount.text = infos.playercounter.deadCount.ToString();
			this.assistCount.text = infos.playercounter.helpKillHoreCount.ToString();
			this.Achieve(infos, infos.playercounter, isMain);
		}
		int month = infos.gametime.Month;
		int day = infos.gametime.Day;
		if (isMain)
		{
			this.date.text = string.Concat(new string[]
			{
				month.ToString(),
				"月",
				day.ToString(),
				"日",
				DateTime.Parse(infos.gametime.ToString()).ToString("HH:mm")
			});
		}
		else
		{
			this.date.text = month.ToString() + "月" + day.ToString() + "日";
		}
		this.battleName.text = LanguageManager.Instance.GetStringById("Combatgains_Describe_" + infos.battleid);
	}

	private void OnClickItem(GameObject obj = null)
	{
		if (BattleRecord.TempObj != obj.transform.parent.gameObject)
		{
			if (BattleRecord.TempObj != null)
			{
				BattleRecord.TempObj.transform.GetChild(0).transform.GetChild(0).gameObject.SetActive(false);
			}
			Singleton<AchievementView>.Instance.UpdateFrameLight(obj.transform.parent.gameObject.name);
			long logID = long.Parse(obj.transform.parent.name);
			Singleton<AchievementView>.Instance.SendMsgToGetHistoryRec(logID);
		}
		BattleRecord.TempObj = obj.transform.parent.gameObject;
	}

	private void OnClickItemInMain(GameObject obj = null)
	{
		SendMsgManager.SendMsgParam sendMsgParam = new SendMsgManager.SendMsgParam(true, "正在获得数据", true, 15f);
		Singleton<AchievementView>.Instance.battleID = Singleton<AchievementView>.Instance.chooseBattleName;
		Singleton<AchievementView>.Instance.AllCount = Singleton<AchievementView>.Instance.AllCountInMain;
		Singleton<AchievementView>.Instance.PageNum = Singleton<AchievementView>.Instance.PageNumInMain;
		Singleton<AchievementView>.Instance.vBtn = Singleton<AchievementView>.Instance.PageNumInMain;
		Singleton<AchievementView>.Instance.heroRecordDic = Singleton<AchievementView>.Instance.heroRecordListInMain;
		Singleton<AchievementView>.Instance.oobj = obj.transform.parent.gameObject;
		Singleton<AchievementView>.Instance.isInMainPanel = false;
		long logID = long.Parse(obj.transform.parent.name);
		Singleton<AchievementView>.Instance.SendMsgToGetHistoryRec(logID);
		Singleton<AchievementView>.Instance.OpenHistoryPanel(false);
		Singleton<AchievementView>.Instance.CreatBattleList(Singleton<AchievementView>.Instance.heroRecordListInMain);
		Singleton<AchievementView>.Instance.UpdateFrameLight(obj.transform.parent.name);
		Singleton<AchievementView>.Instance.historyBtn.GetComponent<UIToggle>().value = true;
		string str = Singleton<AchievementView>.Instance.battleID;
		if (string.Empty == Singleton<AchievementView>.Instance.battleID)
		{
			str = "All";
		}
		else if (Singleton<AchievementView>.Instance.battleID != "80003" && Singleton<AchievementView>.Instance.battleID != "80006" && Singleton<AchievementView>.Instance.battleID != "800055")
		{
			str = "Other";
		}
		BattleTypeList component = Singleton<AchievementView>.Instance.VBtnInHistory.GetComponent<BattleTypeList>();
		component.chooseStr = LanguageManager.Instance.GetStringById("Combatgains_Describe_" + str);
	}

	public void Achieve(heroRecordInfo recordInfo, PlayerCounter playercounter, bool isMain)
	{
		if (!isMain)
		{
			return;
		}
		while (this.achive.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(this.achive.GetChild(0).gameObject);
		}
		bool flag = false;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		dictionary = playercounter.extKillCount;
		if (dictionary == null)
		{
			return;
		}
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(dictionary);
		if (dictionary.Count > 0)
		{
			list.Sort((KeyValuePair<int, int> s1, KeyValuePair<int, int> s2) => s2.Key.CompareTo(s1.Key));
		}
		for (int i = 0; i < list.Count; i++)
		{
			int num = list[i].Key;
			switch (num)
			{
			case 3:
				if (!flag)
				{
					if (this.achive.childCount < i + 1)
					{
						NGUITools.AddChild(this.achive.gameObject, this.AchiSprite);
					}
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_3";
					UIWidget arg_3B4_0 = this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>();
					int num2 = 36;
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().height = num2;
					arg_3B4_0.width = num2;
					this.achive.GetChild(this.achive.childCount - 1).gameObject.SetActive(true);
				}
				break;
			case 4:
				if (!flag)
				{
					if (this.achive.childCount < i + 1)
					{
						NGUITools.AddChild(this.achive.gameObject, this.AchiSprite);
					}
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_4";
					UIWidget arg_2E7_0 = this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>();
					int num2 = 36;
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().height = num2;
					arg_2E7_0.width = num2;
					this.achive.GetChild(this.achive.childCount - 1).gameObject.SetActive(true);
					flag = true;
				}
				break;
			case 5:
			{
				if (this.achive.childCount < i + 1)
				{
					NGUITools.AddChild(this.achive.gameObject, this.AchiSprite);
				}
				this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_5";
				UIWidget arg_21A_0 = this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>();
				int num2 = 36;
				this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().height = num2;
				arg_21A_0.width = num2;
				this.achive.GetChild(this.achive.childCount - 1).gameObject.SetActive(true);
				flag = true;
				break;
			}
			default:
				if (num == 104)
				{
					if (this.achive.childCount < i + 1)
					{
						NGUITools.AddChild(this.achive.gameObject, this.AchiSprite);
					}
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_shen";
					UIWidget arg_155_0 = this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>();
					int num2 = 36;
					this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().height = num2;
					arg_155_0.width = num2;
					this.achive.GetChild(this.achive.childCount - 1).gameObject.SetActive(true);
				}
				break;
			}
		}
		if (playercounter.isMostMoney)
		{
			GameObject gameObject = NGUITools.AddChild(this.achive.gameObject, this.AchiSprite);
			gameObject.transform.GetComponent<UISprite>().spriteName = "Data_statistics_images_money";
			UIWidget arg_476_0 = this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>();
			int num = 36;
			this.achive.GetChild(this.achive.childCount - 1).GetComponent<UISprite>().height = num;
			arg_476_0.width = num;
			gameObject.gameObject.SetActive(true);
		}
		if (recordInfo.isTeamFight)
		{
			this.Friend.SetActive(true);
		}
		else
		{
			this.Friend.SetActive(false);
		}
		this.achive.GetComponent<UIGrid>().Reposition();
	}
}
