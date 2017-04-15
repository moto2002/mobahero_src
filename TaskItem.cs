using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class TaskItem : MonoBehaviour
{
	public UISprite m_sp;

	public UITexture m_texture;

	public UILabel title;

	public UILabel numLeft;

	public UILabel numRight;

	public UILabel content;

	public GameObject bgRed;

	public GameObject bgBlue;

	public Transform doingMark;

	public Transform finishMark;

	public Transform getRewardBtn;

	public UIEffectSort getRewardEffect;

	public Transform rewardRoot;

	public Transform Travel;

	private SysDropItemsVo dropItemsVo;

	private SysDropRewardsVo dropRewardsVo;

	private SysAchievementTaskVo achievementTaskVo;

	private SysAchievementDailyVo achievementDailyVo;

	private string taskid = string.Empty;

	private new string name = string.Empty;

	private string describe = string.Empty;

	private string parameter = string.Empty;

	private string icon = string.Empty;

	private string reward = string.Empty;

	private string achievement_point = string.Empty;

	private string drop_items = string.Empty;

	private string value = string.Empty;

	private string[] rewards;

	private float offX;

	private bool havaShowItem;

	private GameObject rewarItem;

	public void InitData(DetailAchieveData achieveData = null, DailyTaskData dailyData = null)
	{
		if (achieveData != null)
		{
			if (Singleton<TaskView>.Instance.IsOpen)
			{
				this.getRewardEffect.panel = Singleton<TaskView>.Instance.transform.GetComponent<UIPanel>();
			}
			this.taskid = achieveData.taskid.ToString();
			this.achievementTaskVo = BaseDataMgr.instance.GetDataById<SysAchievementTaskVo>(this.taskid);
			if (this.achievementTaskVo == null)
			{
				ClientLogger.Error("taskid=" + this.taskid + " !!在表里找不到");
				return;
			}
			this.name = this.achievementTaskVo.name;
			this.describe = this.achievementTaskVo.describe;
			this.parameter = this.achievementTaskVo.parameter.ToString();
			this.icon = this.achievementTaskVo.icon;
			this.reward = this.achievementTaskVo.reward;
			this.achievement_point = this.achievementTaskVo.achievement_point.ToString();
			this.value = achieveData.value.ToString();
		}
		else
		{
			if (Singleton<DailyView>.Instance.IsOpen)
			{
				this.getRewardEffect.panel = Singleton<DailyView>.Instance.transform.GetComponent<UIPanel>();
			}
			this.taskid = dailyData.taskid.ToString();
			this.achievementDailyVo = BaseDataMgr.instance.GetDataById<SysAchievementDailyVo>(this.taskid);
			if (this.achievementDailyVo == null)
			{
				ClientLogger.Warn("daily_id=" + this.taskid + "     在SysAchievementDailyVo中找不到");
			}
			this.name = this.achievementDailyVo.name;
			this.describe = this.achievementDailyVo.describe;
			this.parameter = this.achievementDailyVo.parameter.ToString();
			this.icon = this.achievementDailyVo.icon;
			this.reward = this.achievementDailyVo.reward;
			this.value = dailyData.value.ToString();
		}
		this.drop_items = null;
		this.title.text = BaseDataMgr.instance.GetLanguageData(this.name).content;
		this.content.text = BaseDataMgr.instance.GetLanguageData(this.describe).content;
		if (Singleton<TaskView>.Instance.coroutineManager == null)
		{
			Singleton<TaskView>.Instance.coroutineManager = new CoroutineManager();
		}
		Singleton<TaskView>.Instance.coroutineManager.StartCoroutine(this.GetDisPlayTexture(BaseDataMgr.instance.GetGameResData(this.icon.Split(new char[]
		{
			'|'
		})[1]).path), true);
		this.numLeft.text = this.value;
		this.numRight.text = "/" + this.parameter;
		this.rewards = this.reward.Split(new char[]
		{
			','
		});
		if (achieveData != null)
		{
			this.drop_items = this.achievement_point + ",";
		}
		for (int i = 0; i < this.rewards.Length; i++)
		{
			if (i != 0)
			{
				this.drop_items += ",";
			}
			if (this.rewards[i] != "[]")
			{
				this.dropRewardsVo = BaseDataMgr.instance.GetDataById<SysDropRewardsVo>(this.rewards[i]);
				this.drop_items += this.dropRewardsVo.drop_items;
			}
		}
		if (this.drop_items == null)
		{
			this.rewards = new string[0];
		}
		else
		{
			this.rewards = this.drop_items.Split(new char[]
			{
				','
			});
		}
		this.offX = 0f;
		this.havaShowItem = false;
		for (int j = 0; j < this.rewardRoot.childCount; j++)
		{
			this.rewardRoot.GetChild(j).gameObject.SetActive(false);
		}
		int k = 0;
		while (k < this.rewards.Length)
		{
			if (Singleton<TaskView>.Instance.taskRewardItem == null)
			{
				Singleton<TaskView>.Instance.taskRewardItem = (Resources.Load("Prefab/UI/Home/TaskRewardItem") as GameObject);
			}
			string[] array = null;
			if (k == 0 && dailyData == null)
			{
				goto IL_4A1;
			}
			this.dropItemsVo = BaseDataMgr.instance.GetDataById<SysDropItemsVo>(this.rewards[k]);
			array = this.dropItemsVo.rewards.Split(new char[]
			{
				'|'
			});
			if (!this.havaShowItem || (!(array[0] == "2") && !(array[0] == "3")))
			{
				goto IL_4A1;
			}
			IL_77B:
			k++;
			continue;
			IL_4A1:
			if (k < this.rewardRoot.childCount)
			{
				this.rewarItem = this.rewardRoot.GetChild(k).gameObject;
				this.rewarItem.gameObject.SetActive(true);
			}
			else
			{
				this.rewarItem = NGUITools.AddChild(this.rewardRoot.gameObject, Singleton<TaskView>.Instance.taskRewardItem);
			}
			UISprite component = this.rewarItem.transform.Find("Icon").GetComponent<UISprite>();
			UILabel component2 = this.rewarItem.transform.Find("Label").GetComponent<UILabel>();
			this.rewarItem.transform.localPosition = new Vector3(this.offX, 0f, 0f);
			if (k == 0 && achieveData != null)
			{
				component.spriteName = "Achievement_icons_stars";
				component2.text = "x" + this.rewards[0];
				this.offX += (float)(component.width + component2.width + 20);
				goto IL_77B;
			}
			if (array == null)
			{
				goto IL_77B;
			}
			if (array[0] == "1")
			{
				if (array[1] == "1")
				{
					component.spriteName = "Achievement_icons_gold";
				}
				else if (array[1] == "2")
				{
					component.spriteName = "Achievement_icons_diamond";
				}
				else if (array[1] == "9")
				{
					component.spriteName = "Achievement_icons_cap";
				}
			}
			else if (array[0] == "2")
			{
				this.havaShowItem = true;
				component.spriteName = "Achievement_icons_others";
			}
			else if (array[0] == "3")
			{
				this.havaShowItem = true;
				component.spriteName = "Achievement_icons_others";
			}
			else if (array[0] == "4")
			{
				if (array[1] == "1")
				{
					component.spriteName = "Achievement_icons_exp_summoner";
				}
				else if (array[1] == "2")
				{
					component.spriteName = "Achievement_icons_exp_bottle";
				}
			}
			else if (array[0] == "6")
			{
				this.havaShowItem = true;
				component.spriteName = "Achievement_icons_others";
			}
			if (array.Length >= 3 && array[0] != "2" && array[0] != "3")
			{
				component2.text = "x" + array[2];
			}
			else
			{
				component2.text = "x1";
			}
			this.offX += (float)(component.width + component2.width + 20);
			goto IL_77B;
		}
		bool flag = (achieveData == null) ? dailyData.isGetAward : achieveData.isGetAward;
		bool flag2 = (achieveData == null) ? dailyData.isComplete : achieveData.isComplete;
		if (flag)
		{
			this.getRewardBtn.gameObject.SetActive(false);
			if (this.achievementDailyVo != null)
			{
				if (this.achievementDailyVo.travel_to == 11)
				{
					this.finishMark.gameObject.SetActive(false);
					this.Travel.gameObject.SetActive(true);
					this.doingMark.gameObject.SetActive(false);
					this.Travel.name = this.achievementDailyVo.travel_to.ToString();
					UIEventListener.Get(this.Travel.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTravel);
				}
				else
				{
					this.finishMark.gameObject.SetActive(true);
				}
			}
			else if (this.achievementTaskVo.travel_to == 11)
			{
				this.finishMark.gameObject.SetActive(false);
				this.Travel.gameObject.SetActive(true);
				this.doingMark.gameObject.SetActive(false);
				this.Travel.name = this.achievementTaskVo.travel_to.ToString();
				UIEventListener.Get(this.Travel.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTravel);
			}
			else
			{
				this.finishMark.gameObject.SetActive(true);
			}
			this.bgRed.SetActive(true);
			this.bgBlue.SetActive(false);
			this.SetColor(true);
		}
		else if (flag2)
		{
			this.getRewardBtn.gameObject.SetActive(true);
			this.finishMark.gameObject.SetActive(false);
			this.bgRed.SetActive(true);
			this.bgBlue.SetActive(false);
			this.SetColor(true);
		}
		else
		{
			this.bgRed.SetActive(false);
			this.bgBlue.SetActive(true);
			if (achieveData != null)
			{
				if (this.achievementTaskVo.travel_to != 0)
				{
					this.Travel.gameObject.SetActive(true);
					this.doingMark.gameObject.SetActive(false);
					this.Travel.name = this.achievementTaskVo.travel_to.ToString();
					UIEventListener.Get(this.Travel.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTravel);
				}
				else
				{
					this.doingMark.gameObject.SetActive(true);
					this.Travel.gameObject.SetActive(false);
				}
			}
			else if (this.achievementDailyVo.travel_to != 0)
			{
				this.Travel.name = this.achievementDailyVo.travel_to.ToString();
				UIEventListener.Get(this.Travel.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickTravel);
				this.Travel.gameObject.SetActive(true);
				this.doingMark.gameObject.SetActive(false);
			}
			else
			{
				this.Travel.gameObject.SetActive(false);
				this.doingMark.gameObject.SetActive(true);
			}
			this.SetColor(false);
		}
	}

	private void OnClickTravel(GameObject go)
	{
		Singleton<TaskView>.Instance.TravelView(go.name);
	}

	private void SetColor(bool isRed)
	{
		if (isRed)
		{
			this.title.gradientTop = new Color(0.992156863f, 0.9019608f, 0.5764706f);
			this.title.gradientBottom = new Color(0.996078432f, 0.968627453f, 0.858823538f);
			this.numRight.gradientTop = new Color(0.9411765f, 0.78039217f, 0.4f);
			this.numRight.gradientBottom = new Color(0.996078432f, 0.921568632f, 0.721568644f);
			this.content.color = new Color(0.992156863f, 0.996078432f, 0.831372559f);
		}
		else
		{
			this.title.gradientTop = new Color(0.4862745f, 0.996078432f, 0.929411769f);
			this.title.gradientBottom = new Color(0.4862745f, 0.996078432f, 0.929411769f);
			this.numRight.gradientTop = new Color(0f, 0.4745098f, 0.647058845f);
			this.numRight.gradientBottom = new Color(0f, 0.4745098f, 0.647058845f);
			this.content.color = new Color(0.8352941f, 0.996078432f, 0.9764706f);
		}
	}

	[DebuggerHidden]
	private IEnumerator GetDisPlayTexture(string path)
	{
		TaskItem.<GetDisPlayTexture>c__Iterator187 <GetDisPlayTexture>c__Iterator = new TaskItem.<GetDisPlayTexture>c__Iterator187();
		<GetDisPlayTexture>c__Iterator.path = path;
		<GetDisPlayTexture>c__Iterator.<$>path = path;
		<GetDisPlayTexture>c__Iterator.<>f__this = this;
		return <GetDisPlayTexture>c__Iterator;
	}
}
