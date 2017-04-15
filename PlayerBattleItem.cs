using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleItem : MonoBehaviour
{
	public UISprite BG;

	public UISprite LightLine;

	public UITexture portrait;

	public UISprite Mvp;

	public UILabel Name;

	public Transform Grid;

	public UILabel Evaluate;

	public UILabel KDA;

	public UILabel Money;

	public UIGrid Equipment;

	private GameObject AchiSprite;

	private Dictionary<int, int> achi = new Dictionary<int, int>();

	public void init(int index, List<MemberBattleInfo> dataList, BattleMaxInfo maxInfo)
	{
		this.AchiSprite = Resources.Load<GameObject>("Prefab/UI/Achievement/AchiSprite");
		this.SetInfo(dataList[index], maxInfo);
	}

	private void SetInfo(MemberBattleInfo rankData, BattleMaxInfo maxInfo)
	{
		this.achi.Clear();
		double num = 0.0;
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(rankData.heroId);
		this.portrait.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
		this.Name.text = rankData.userName;
		string b = ModelManager.Instance.Get_userData_filed_X("UserId");
		string text = ModelManager.Instance.Get_userData_filed_X("NickName");
		if (rankData.userId == b)
		{
			this.BG.color = new Color32(0, 39, 53, 255);
			this.Name.text = text;
			this.Name.color = new Color32(224, 178, 39, 255);
		}
		else
		{
			this.BG.color = new Color32(0, 25, 34, 255);
			this.Name.color = Color.white;
		}
		this.Name.gameObject.GetComponent<AllochroicLabelChecker>().StoreLabelStyle(true);
		this.Name.gameObject.GetComponent<AllochroicLabelChecker>().RenderLabel(rankData.CharmRankvalue);
		if (rankData.playercounter != null)
		{
			if (rankData.playercounter.allDamage == maxInfo.maxDamage && rankData.playercounter.allDamage != 0L)
			{
				this.achi.Add(13, 1);
			}
			if (rankData.playercounter.allBeDamage == maxInfo.maxBedamage && rankData.playercounter.allBeDamage != 0L)
			{
				this.achi.Add(12, 1);
			}
			int killMonsterCount = rankData.playercounter.killMonsterCount;
			if (killMonsterCount == maxInfo.maxMonsterKill && killMonsterCount != 0)
			{
				this.achi.Add(10, 1);
			}
			if (rankData.playercounter.killTowerCount == maxInfo.maxTower && rankData.playercounter.killTowerCount != 0)
			{
				this.achi.Add(11, 1);
			}
			if (rankData.playercounter.allmoney == maxInfo.maxMoney)
			{
				double value = (double)rankData.playercounter.allmoney / 1000.0;
				value = Math.Round(value, 1);
				this.Money.text = "[00ff42]" + value.ToString("F1") + "K[-]";
				this.achi.Add(9, 1);
			}
			else
			{
				double value = (double)rankData.playercounter.allmoney / 1000.0;
				value = Math.Round(value, 1);
				this.Money.text = "[0075a6]" + value.ToString("F1") + "K[-]";
			}
			string text2;
			if (rankData.playercounter.killHoreCount == maxInfo.maxK && rankData.playercounter.killHoreCount != 0)
			{
				this.achi.Add(7, 1);
				text2 = "[ffc000]" + rankData.playercounter.killHoreCount.ToString() + "[-]";
			}
			else
			{
				text2 = "[0075a6]" + rankData.playercounter.killHoreCount.ToString() + "[-]";
			}
			string text3;
			if (rankData.playercounter.deadCount == maxInfo.maxD)
			{
				text3 = "[ffc000]" + rankData.playercounter.deadCount.ToString() + "[-]";
			}
			else
			{
				text3 = "[0075a6]" + rankData.playercounter.deadCount.ToString() + "[-]";
			}
			string text4;
			if (rankData.playercounter.helpKillHoreCount == maxInfo.maxA && rankData.playercounter.helpKillHoreCount != 0)
			{
				this.achi.Add(8, 1);
				text4 = "[ffc000]" + rankData.playercounter.helpKillHoreCount.ToString() + "[-]";
			}
			else
			{
				text4 = "[0075a6]" + rankData.playercounter.helpKillHoreCount.ToString() + "[-]";
			}
			this.KDA.text = string.Concat(new string[]
			{
				text2,
				"[0075a6]/[-]",
				text3,
				"[0075a6]/[-]",
				text4
			});
			if (rankData.playercounter.deadCount == 0)
			{
				num = (double)(rankData.playercounter.killHoreCount + rankData.playercounter.helpKillHoreCount) / 1.0 * 3.0;
			}
			else
			{
				num = (double)(rankData.playercounter.killHoreCount + rankData.playercounter.helpKillHoreCount) / (double)rankData.playercounter.deadCount * 3.0;
			}
		}
		else
		{
			UILabel arg_4F6_0 = this.Money;
			string empty = string.Empty;
			this.KDA.text = empty;
			arg_4F6_0.text = empty;
		}
		num = Math.Round(num, 1);
		if (num == maxInfo.maxEvaluate && num != 0.0)
		{
			this.Evaluate.text = "[ffc000]" + num.ToString("F1") + "[-]";
		}
		else
		{
			this.Evaluate.text = "[0075a6]" + num.ToString("F1") + "[-]";
		}
		if (rankData.isWin)
		{
			this.LightLine.spriteName = "Data_statistics_team_green";
			if (rankData == maxInfo.maxMVP)
			{
				this.Mvp.gameObject.SetActive(true);
			}
			else
			{
				this.Mvp.gameObject.SetActive(false);
			}
		}
		else
		{
			this.LightLine.spriteName = "Data_statistics_team_red";
			this.Mvp.gameObject.SetActive(false);
		}
		this.UpdateEquipmentList(rankData);
		if (rankData.playercounter != null)
		{
			this.Achieve(rankData.playercounter);
		}
	}

	private void UpdateEquipmentList(MemberBattleInfo rankData)
	{
		if (rankData.playercounter == null || rankData.playercounter.equiplist == null)
		{
			this.Equipment.gameObject.SetActive(false);
			return;
		}
		this.Equipment.gameObject.SetActive(true);
		for (int i = 0; i < this.Equipment.transform.childCount; i++)
		{
			this.Equipment.transform.GetChild(i).gameObject.SetActive(false);
		}
		rankData.playercounter.equiplist.Remove(string.Empty);
		int num = 0;
		while (num < rankData.playercounter.equiplist.Count && num < 6)
		{
			SysBattleItemsVo dataById = BaseDataMgr.instance.GetDataById<SysBattleItemsVo>(rankData.playercounter.equiplist[num]);
			if (dataById != null)
			{
				this.Equipment.transform.GetChild(num).GetComponent<UITexture>().mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
				this.Equipment.transform.GetChild(num).gameObject.SetActive(true);
			}
			num++;
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
		this.Grid.GetComponent<UIGrid>().repositionNow = true;
		this.achi.Clear();
	}
}
