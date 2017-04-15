using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleDetailsItem : MonoBehaviour
{
	public UISprite Bg;

	public UISprite LightLine;

	public Transform Frame;

	public Transform MVP;

	public UILabel Name;

	public UITexture Portrait;

	public Transform Grid;

	public UILabel Evaluate;

	public UILabel Grade;

	public UILabel KDA;

	public UILabel Money;

	public UILabel MonstersKill;

	public UILabel Damage;

	public UILabel DamagePer;

	public UILabel BeDamage;

	public UILabel BeDamagePer;

	public UISprite Bar;

	public UILabel Number;

	private GameObject AchiSprite;

	private Dictionary<int, int> achi = new Dictionary<int, int>();

	public void init(int index, List<MemberBattleInfo> dataList, BattleMaxInfo maxInfo, double co_op, long teamDam, long teamBeDam, double teamKill)
	{
		this.AchiSprite = Resources.Load<GameObject>("Prefab/UI/Achievement/AchiSprite");
		if (index % 2 == 0)
		{
			this.Bg.color = new Color32(0, 32, 23, 255);
		}
		else
		{
			this.Bg.color = new Color32(0, 38, 27, 255);
		}
		this.SetInfo(dataList[index], maxInfo, co_op, teamDam, teamBeDam, teamKill);
		switch (index)
		{
		case 0:
		{
			UIWidget arg_C0_0 = this.Number;
			Color color = new Color32(0, 183, 168, 255);
			this.Bar.color = color;
			arg_C0_0.color = color;
			break;
		}
		case 1:
		{
			UIWidget arg_F8_0 = this.Number;
			Color color = new Color32(201, 133, 0, 255);
			this.Bar.color = color;
			arg_F8_0.color = color;
			break;
		}
		case 2:
		{
			UIWidget arg_12D_0 = this.Number;
			Color color = new Color32(6, 121, 218, 255);
			this.Bar.color = color;
			arg_12D_0.color = color;
			break;
		}
		case 3:
		{
			UIWidget arg_163_0 = this.Number;
			Color color = new Color32(179, 56, 32, 255);
			this.Bar.color = color;
			arg_163_0.color = color;
			break;
		}
		case 4:
		{
			UIWidget arg_199_0 = this.Number;
			Color color = new Color32(175, 11, 119, 255);
			this.Bar.color = color;
			arg_199_0.color = color;
			break;
		}
		}
	}

	private void SetInfo(MemberBattleInfo rankData, BattleMaxInfo maxInfo, double co_op, long teamDam, long teamBeDam, double teamKill)
	{
		this.achi.Clear();
		this.MVP.gameObject.SetActive(false);
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(rankData.heroId);
		this.Portrait.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
		if (rankData.playercounter != null)
		{
			if (rankData.playercounter.killTowerCount == maxInfo.maxTower && maxInfo.maxTower != 0)
			{
				this.achi.Add(11, 1);
			}
			if (rankData.playercounter.allmoney == maxInfo.maxMoney)
			{
				double value = (double)rankData.playercounter.allmoney / 1000.0;
				value = Math.Round(value, 1);
				this.Money.text = "[ffe400]" + value.ToString("F1") + "K[-]";
				this.achi.Add(9, 1);
			}
			else
			{
				double value = (double)rankData.playercounter.allmoney / 1000.0;
				value = Math.Round(value, 1);
				this.Money.text = "[bba85b]" + value.ToString("F1") + "K[-]";
			}
			string text;
			if (rankData.playercounter.killHoreCount == maxInfo.maxK && rankData.playercounter.killHoreCount != 0)
			{
				this.achi.Add(7, 1);
				text = "[ffe400]" + rankData.playercounter.killHoreCount.ToString() + "[-]";
			}
			else
			{
				text = "[bba85b]" + rankData.playercounter.killHoreCount.ToString() + "[-]";
			}
			string text2;
			if (rankData.playercounter.deadCount == maxInfo.maxD)
			{
				text2 = "[ffe400]" + rankData.playercounter.deadCount.ToString() + "[-]";
			}
			else
			{
				text2 = "[bba85b]" + rankData.playercounter.deadCount.ToString() + "[-]";
			}
			string text3;
			if (rankData.playercounter.helpKillHoreCount == maxInfo.maxA && rankData.playercounter.helpKillHoreCount != 0)
			{
				this.achi.Add(8, 1);
				text3 = "[ffe400]" + rankData.playercounter.helpKillHoreCount.ToString() + "[-]";
			}
			else
			{
				text3 = "[bba85b]" + rankData.playercounter.helpKillHoreCount.ToString() + "[-]";
			}
			this.KDA.text = string.Concat(new string[]
			{
				text,
				"[bba85b]/[-]",
				text2,
				"[bba85b]/[-]",
				text3
			});
			double num;
			if (rankData.playercounter.deadCount == 0)
			{
				num = (double)(rankData.playercounter.killHoreCount + rankData.playercounter.helpKillHoreCount) / 1.0 * 3.0;
			}
			else
			{
				num = (double)(rankData.playercounter.killHoreCount + rankData.playercounter.helpKillHoreCount) / (double)rankData.playercounter.deadCount * 3.0;
			}
			num = Math.Round(num, 1);
			if (num == maxInfo.maxEvaluate && num != 0.0)
			{
				this.Evaluate.text = "[ffe400]" + num.ToString("F1") + "[-]";
			}
			else
			{
				this.Evaluate.text = "[bba85b]" + num.ToString("F1") + "[-]";
			}
			float num2 = 0f;
			if (teamDam != 0L)
			{
				num2 = (float)rankData.playercounter.allDamage / (float)teamDam * 100f;
			}
			if (rankData.playercounter.allDamage == maxInfo.maxDamage && maxInfo.maxDamage != 0L)
			{
				this.achi.Add(13, 1);
				this.Damage.text = "[ffe400]" + rankData.playercounter.allDamage.ToString();
			}
			else
			{
				this.Damage.text = "[bba85b]" + rankData.playercounter.allDamage.ToString();
			}
			this.DamagePer.text = "(" + num2.ToString("F0") + "%)";
			float num3 = 0f;
			if (teamBeDam != 0L)
			{
				num3 = (float)rankData.playercounter.allBeDamage / (float)teamBeDam * 100f;
			}
			if (rankData.playercounter.allBeDamage == maxInfo.maxBedamage && maxInfo.maxBedamage != 0L)
			{
				this.achi.Add(12, 1);
				this.BeDamage.text = "[ffe400]" + rankData.playercounter.allBeDamage.ToString() + "[-]";
				this.BeDamagePer.text = "(" + num3.ToString("F0") + "%)";
			}
			else
			{
				this.BeDamage.text = "[bba85b]" + rankData.playercounter.allBeDamage.ToString();
				this.BeDamagePer.text = "(" + num3.ToString("F0") + "%)";
			}
			if (rankData.playercounter.killMonsterCount == maxInfo.maxMonsterKill && maxInfo.maxMonsterKill != 0)
			{
				this.achi.Add(10, 1);
				this.MonstersKill.text = "[ffe400]" + rankData.playercounter.killMonsterCount.ToString() + "[-]";
			}
			else
			{
				this.MonstersKill.text = "[bba85b]" + rankData.playercounter.killMonsterCount.ToString() + "[-]";
			}
			double num4 = (double)(rankData.playercounter.killHoreCount + rankData.playercounter.helpKillHoreCount) / teamKill * 100.0;
			this.Number.text = num4.ToString("F0") + "%";
			this.Number.text = ((!(this.Number.text == "NaN%")) ? this.Number.text : "0%");
			this.Bar.width = (int)(num4 / co_op * 226.0);
		}
		else
		{
			UILabel arg_73A_0 = this.Money;
			string text4 = string.Empty;
			this.Number.text = text4;
			text4 = text4;
			this.MonstersKill.text = text4;
			text4 = text4;
			this.BeDamagePer.text = text4;
			text4 = text4;
			this.BeDamage.text = text4;
			text4 = text4;
			this.Damage.text = text4;
			text4 = text4;
			this.Evaluate.text = text4;
			text4 = text4;
			this.KDA.text = text4;
			arg_73A_0.text = text4;
		}
		if (rankData.isWin)
		{
			this.LightLine.spriteName = "Data_statistics_team_green";
			this.Name.text = rankData.userName;
			this.Name.color = new Color32(0, 198, 154, 255);
		}
		else
		{
			this.LightLine.spriteName = "Data_statistics_team_red";
			this.Name.text = rankData.userName;
			this.Name.color = new Color32(246, 62, 62, 255);
		}
		string b = ModelManager.Instance.Get_userData_filed_X("UserId");
		string text5 = ModelManager.Instance.Get_userData_filed_X("NickName");
		if (rankData.userId == b)
		{
			this.Frame.gameObject.SetActive(true);
			this.Name.text = text5;
			if (rankData.isWin)
			{
				Singleton<AchievementView>.Instance.dtl_vectory.spriteName = "Settlement_images_win";
			}
			else
			{
				Singleton<AchievementView>.Instance.dtl_vectory.spriteName = "Settlement_images_lose";
			}
		}
		else
		{
			this.Frame.gameObject.SetActive(false);
		}
		if (rankData.isWin && rankData == maxInfo.maxMVP)
		{
			this.MVP.gameObject.SetActive(true);
		}
		this.Name.gameObject.GetComponent<AllochroicLabelChecker>().StoreLabelStyle(true);
		this.Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(rankData.CharmRankvalue);
		string stringById = LanguageManager.Instance.GetStringById(Singleton<FriendView>.Instance.GetState((int)rankData.ladderScore));
		this.Grade.text = ((stringById.Length <= 4) ? stringById.Substring(2, 2) : stringById.Substring(2, 3));
		if (rankData.playercounter != null)
		{
			this.Achieve(rankData.playercounter);
		}
	}

	public void Achieve(PlayerCounter playercounter)
	{
		while (this.Grid.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(this.Grid.GetChild(0).gameObject);
		}
		bool flag = false;
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		dictionary = playercounter.extKillCount;
		if (dictionary != null)
		{
			List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(dictionary);
			if (dictionary.Count > 0)
			{
				list.Sort((KeyValuePair<int, int> s1, KeyValuePair<int, int> s2) => s2.Key.CompareTo(s1.Key));
			}
			for (int i = 0; i < list.Count; i++)
			{
				int key = list[i].Key;
				switch (key)
				{
				case 3:
					if (!flag)
					{
						if (this.Grid.childCount < i + 1)
						{
							NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
						}
						this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_3";
						this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
					}
					break;
				case 4:
					if (!flag)
					{
						if (this.Grid.childCount < i + 1)
						{
							NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
						}
						this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_4";
						this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
						flag = true;
					}
					break;
				case 5:
					if (this.Grid.childCount < i + 1)
					{
						NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
					}
					this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_5";
					this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
					flag = true;
					break;
				default:
					if (key == 104)
					{
						if (this.Grid.childCount < i + 1)
						{
							NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
						}
						this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_shen";
						this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
					}
					break;
				}
			}
		}
		List<KeyValuePair<int, int>> list2 = new List<KeyValuePair<int, int>>(this.achi);
		if (this.achi.Count > 0)
		{
			list2.Sort((KeyValuePair<int, int> s1, KeyValuePair<int, int> s2) => s1.Key.CompareTo(s2.Key));
		}
		for (int j = 0; j < this.achi.Count; j++)
		{
			switch (list2[j].Key)
			{
			case 7:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_kills";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 8:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_fist";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 9:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_images_money";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 10:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_batman";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 11:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_tower";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 12:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_tank";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			case 13:
				if (this.Grid.childCount < this.Grid.childCount + j + 1)
				{
					NGUITools.AddChild(this.Grid.gameObject, this.AchiSprite);
				}
				this.Grid.GetChild(this.Grid.childCount - 1).GetComponent<UISprite>().spriteName = "Data_statistics_icons_injure";
				this.Grid.GetChild(this.Grid.childCount - 1).gameObject.SetActive(true);
				break;
			}
		}
		this.Grid.GetComponent<UIGrid>().Reposition();
		this.achi.Clear();
	}
}
