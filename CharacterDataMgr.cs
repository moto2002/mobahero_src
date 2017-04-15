using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;

public class CharacterDataMgr
{
	public enum Portrait
	{
		CommonPortrait = 1,
		HeroPortrait,
		SpecialPortrait,
		SociatyPortrait
	}

	public enum PropsItem
	{
		HeroProps = 1,
		EquipmentProps,
		EnchantingView
	}

	public List<HeroInfoData> OwenHerosInfo;

	public List<SysHeroMainVo> AllHerosInfo;

	public List<string> OwenPowerHeros;

	public List<string> OwenAgileHeros;

	public List<string> OwenIQHeros;

	public List<string> OwenHeros;

	public List<string> AllNoHaveHeros;

	public List<string> NoHaveHeros;

	public List<string> AllCanCallHeros;

	public List<string> AllPowerHeros;

	public List<string> AllAgileHeros;

	public List<string> AllIQHeros;

	public List<string> AllHeros;

	private AttrNumberData[] m_AttrNums = new AttrNumberData[99];

	public static CharacterDataMgr instance = new CharacterDataMgr();

	private bool isInitConfigHeros;

	private int accountLevel;

	private string userId = string.Empty;

	public AttrNumberData GetAttrNumberData(AttrType attrType)
	{
		AttrNumberData attrNumberData = this.m_AttrNums[(int)attrType];
		if (attrNumberData == null)
		{
			int num = (int)attrType;
			SysAttrNumberVo attrNumberData2 = BaseDataMgr.instance.GetAttrNumberData(num.ToString());
			if (attrNumberData2 == null)
			{
				return null;
			}
			string[] array = attrNumberData2.attrRange.Split(new char[]
			{
				','
			});
			if (array.Length != 2)
			{
				attrNumberData = new AttrNumberData
				{
					m_attrType = attrType,
					m_fMaxValue = -999999f,
					m_fMinValue = 999999f
				};
				this.m_AttrNums[(int)attrType] = attrNumberData;
				Debug.LogError(num.ToString() + "attrRange error! ");
			}
			else
			{
				attrNumberData = new AttrNumberData
				{
					m_attrType = attrType,
					m_fMaxValue = float.Parse(array[1]),
					m_fMinValue = float.Parse(array[0])
				};
				this.m_AttrNums[(int)attrType] = attrNumberData;
			}
		}
		return attrNumberData;
	}

	public void UpdateHerosData()
	{
		this.GetConfigHerosInfo();
		this.GetOwenHerosInfo();
	}

	public List<string> GetOwenHerosInfo()
	{
		List<HeroInfoData> list = ModelManager.Instance.Get_heroInfo_list_X();
		list = this.GetHideHeroIDList(list);
		list = (from obj in list
		orderby this.GetLevel(obj.Exp) descending
		select obj).ToList<HeroInfoData>();
		this.OwenHerosInfo = list;
		this.UpdateOwenHeros();
		return this.OwenHeros;
	}

	private void UpdateOwenHeros()
	{
		if (this.OwenHerosInfo != null)
		{
			this.OwenHeros = new List<string>();
			for (int i = 0; i < this.OwenHerosInfo.Count; i++)
			{
				this.OwenHeros.Add(this.OwenHerosInfo[i].ModelId);
			}
			this.NoHaveHeros = this.AllHeros.Where(delegate(string hero_id)
			{
				bool flag = this.OwenHerosInfo.Exists((HeroInfoData obj) => obj.ModelId == hero_id);
				return !flag;
			}).ToList<string>();
			this.AllCanCallHeros = new List<string>();
			for (int j = 0; j < this.NoHaveHeros.Count; j++)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(this.NoHaveHeros[j]);
			}
			this.NoHaveHeros = this.NoHaveHeros.Where(delegate(string hero_id)
			{
				bool flag = this.AllCanCallHeros.Exists((string obj) => obj == hero_id);
				return !flag;
			}).ToList<string>();
			this.AllNoHaveHeros = new List<string>();
			if (this.AllCanCallHeros != null)
			{
				this.AllNoHaveHeros.AddRange(this.AllCanCallHeros);
			}
			if (this.AllNoHaveHeros != null)
			{
				this.AllNoHaveHeros.AddRange(this.NoHaveHeros);
			}
			this.AllHeros.Clear();
			this.AllCanCallHeros = this.HeroArrayList(this.NoHaveHeros);
			this.OwenHeros = this.HeroArrayList(this.OwenHeros);
			if (this.OwenHeros != null)
			{
				this.AllHeros.AddRange(this.OwenHeros);
			}
			if (this.AllCanCallHeros != null)
			{
				List<string> list = new List<string>();
				foreach (string current in Singleton<PvpManager>.Instance.freeHeros)
				{
					try
					{
						list.Add(current.Split(new char[]
						{
							','
						})[0]);
					}
					catch (Exception ex)
					{
						CtrlManager.ShowMsgBox(ex.ToString(), this.ToString() + "解析限免英雄数组时报错，检查代码", delegate
						{
						}, PopViewType.PopOneButton, "确定", "取消", null);
					}
				}
				this.AllHeros.AddRange(list.Except(this.OwenHeros).ToList<string>());
				this.AllHeros.AddRange(this.NoHaveHeros.Except(list).ToList<string>());
			}
			this.AllPowerHeros = this.GetHeros(this.AllHeros, 1);
			this.AllAgileHeros = this.GetHeros(this.AllHeros, 2);
			this.AllIQHeros = this.GetHeros(this.AllHeros, 3);
			this.OwenPowerHeros = this.GetHeros(this.OwenHeros, 1);
			this.OwenAgileHeros = this.GetHeros(this.OwenHeros, 2);
			this.OwenIQHeros = this.GetHeros(this.OwenHeros, 3);
			this.AllPowerHeros = this.GetHideHeroIDList(this.AllPowerHeros);
			this.AllAgileHeros = this.GetHideHeroIDList(this.AllAgileHeros);
			this.AllIQHeros = this.GetHideHeroIDList(this.AllIQHeros);
			this.OwenPowerHeros = this.GetHideHeroIDList(this.OwenPowerHeros);
			this.OwenAgileHeros = this.GetHideHeroIDList(this.OwenAgileHeros);
			this.OwenIQHeros = this.GetHideHeroIDList(this.OwenIQHeros);
			this.AllCanCallHeros = this.GetHideHeroIDList(this.AllCanCallHeros);
			this.OwenHeros = this.GetHideHeroIDList(this.OwenHeros);
			this.AllHeros = this.GetHideHeroIDList(this.AllHeros);
		}
	}

	public HeroInfoData GetHeroData(string hero_id)
	{
		return ModelManager.Instance.Get_heroInfo_item_byModelID_X(hero_id);
	}

	private List<SysHeroMainVo> GetConfigHerosInfo()
	{
		if (this.isInitConfigHeros)
		{
			return this.AllHerosInfo;
		}
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysHeroMainVo>();
		if (dicByType == null)
		{
			return null;
		}
		if (dicByType.Values.OfType<SysHeroMainVo>() == null)
		{
			return null;
		}
		this.AllHerosInfo = dicByType.Values.OfType<SysHeroMainVo>().ToList<SysHeroMainVo>();
		this.AllHeros = dicByType.Keys.ToList<string>();
		this.AllHeros = this.GetHideHeroIDList(this.AllHeros);
		this.AllPowerHeros = this.GetHeros(this.AllHeros, 1);
		this.AllAgileHeros = this.GetHeros(this.AllHeros, 2);
		this.AllIQHeros = this.GetHeros(this.AllHeros, 3);
		this.isInitConfigHeros = true;
		return this.AllHerosInfo;
	}

	public List<string> GetHideHeroIDList(List<string> list)
	{
		List<string> list2 = new List<string>();
		if (list == null || list.Count == 0)
		{
			return list;
		}
		for (int i = 0; i < list.Count; i++)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(list[i]);
			if (heroMainData != null && heroMainData.hidden == 1)
			{
				list2.Add(list[i]);
			}
		}
		return list2;
	}

	public List<HeroInfoData> GetHideHeroIDList(List<HeroInfoData> list)
	{
		if (list != null && list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(list[i].ModelId);
				if (heroMainData.hidden == 2)
				{
					list.Remove(list[i]);
				}
			}
		}
		return list;
	}

	public bool IsHideHeroItem(string heroId)
	{
		if (heroId == null)
		{
			return false;
		}
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(heroId);
		return heroMainData != null && heroMainData.hidden != 2;
	}

	private List<string> HeroArrayList(List<string> heroList)
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysHeroMainVo>();
		List<HeroInfoData> list = ModelManager.Instance.Get_heroInfo_list_X();
		for (int i = 0; i < heroList.Count; i++)
		{
			bool flag = false;
			int number = i;
			if (number != 0)
			{
				while (!flag)
				{
					HeroInfoData heroInfoData = list.Find((HeroInfoData obj) => obj.ModelId == heroList[number]);
					HeroInfoData heroInfoData2 = list.Find((HeroInfoData obj) => obj.ModelId == heroList[number - 1]);
					if (heroInfoData == null)
					{
						int num = dicByType.Keys.ToList<string>().IndexOf(heroList[number]);
						int num2 = dicByType.Keys.ToList<string>().IndexOf(heroList[number - 1]);
						if (num < num2)
						{
							string value = heroList[number - 1];
							heroList[number - 1] = heroList[number];
							heroList[number] = value;
							number--;
						}
						else
						{
							flag = true;
						}
					}
					else if (this.GetLevel(heroInfoData.Exp) > this.GetLevel(heroInfoData2.Exp))
					{
						string value = heroList[number - 1];
						heroList[number - 1] = heroList[number];
						heroList[number] = value;
						number--;
					}
					else if (this.GetLevel(heroInfoData.Exp) == this.GetLevel(heroInfoData2.Exp))
					{
						if (heroInfoData.Level > heroInfoData2.Level)
						{
							string value = heroList[number - 1];
							heroList[number - 1] = heroList[number];
							heroList[number] = value;
							number--;
						}
						else if (heroInfoData.Level == heroInfoData2.Level)
						{
							if (heroInfoData.Grade > heroInfoData2.Grade)
							{
								string value = heroList[number - 1];
								heroList[number - 1] = heroList[number];
								heroList[number] = value;
								number--;
							}
							else if (heroInfoData.Grade == heroInfoData2.Grade)
							{
								int num3 = dicByType.Keys.ToList<string>().IndexOf(heroList[number]);
								int num4 = dicByType.Keys.ToList<string>().IndexOf(heroList[number - 1]);
								if (num3 < num4)
								{
									string value = heroList[number - 1];
									heroList[number - 1] = heroList[number];
									heroList[number] = value;
									number--;
								}
								else
								{
									flag = true;
								}
							}
							else
							{
								flag = true;
							}
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (number == 0)
					{
						flag = true;
					}
				}
			}
		}
		return heroList;
	}

	public List<string> GetHeros(List<string> heroList, int character_type)
	{
		return heroList.Where(delegate(string hero_id)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
			if (heroMainData == null)
			{
				ClientLogger.Error("heroVo is NullheroId===>" + hero_id);
			}
			return heroMainData.character_type == character_type;
		}).ToList<string>();
	}

	public string ChangeStrUserKey(string str)
	{
		string str2 = ModelManager.Instance.Get_userData_filed_X("UserId");
		return str2 + str;
	}

	public bool CheckHeroOwned(string npc_id)
	{
		if (npc_id != "[]" && !ModelManager.Instance.ValidData(EModelType.Model_heroInfoList))
		{
			ClientLogger.Error("该英雄还未拥有 npc_id = " + npc_id);
			return false;
		}
		return true;
	}

	public Texture GetHeroAvatarTexture(string hero_id)
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
		return ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
	}

	public string GetHeroModelName(string hero_id)
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(hero_id);
		return heroMainData.model_id;
	}

	public void SaveSelectedHeros(string key, Dictionary<int, string> selectheros)
	{
		if (selectheros != null && selectheros.Count > 0)
		{
			string text = selectheros[0];
			for (int i = 1; i < selectheros.Count; i++)
			{
				text = text + "|" + selectheros[i];
			}
			PlayerPrefs.SetString(key, text);
		}
	}

	public List<string> GetSelectedHeros(int battle_type)
	{
		string @string = PlayerPrefs.GetString(battle_type.ToString());
		if (@string != null && @string != string.Empty)
		{
			string[] stringValue = StringUtils.GetStringValue(@string, '|');
			return new List<string>(stringValue);
		}
		return null;
	}

	public List<string> GetSelectedHeros(string battle_type)
	{
		string @string = PlayerPrefs.GetString(battle_type);
		if (@string != null && @string != string.Empty)
		{
			string[] stringValue = StringUtils.GetStringValue(@string, '|');
			List<string> list = new List<string>(stringValue);
			if (list != null && list.Count != 0)
			{
				list = this.GetHideHeroIDList(list);
			}
			return list;
		}
		return null;
	}

	public int CheckHeroSelected(int battle_type, string hero_id)
	{
		List<string> selectedHeros = this.GetSelectedHeros(battle_type);
		if (selectedHeros != null && selectedHeros.Contains(hero_id))
		{
			return selectedHeros.IndexOf(hero_id);
		}
		return -1;
	}

	public int CheckHeroSelected(string battle_type, string hero_id)
	{
		List<string> selectedHeros = this.GetSelectedHeros(battle_type);
		if (selectedHeros != null && selectedHeros.Contains(hero_id))
		{
			return selectedHeros.IndexOf(hero_id);
		}
		return -1;
	}

	public int CheckHeroSelected(List<string> selected_heros, string hero_id)
	{
		if (selected_heros != null && selected_heros.Contains(hero_id))
		{
			return selected_heros.IndexOf(hero_id);
		}
		return -1;
	}

	public string GetFrame_ByType(int type, int number, bool small)
	{
		string result = null;
		switch ((type != 0) ? type : 1)
		{
		case 1:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		case 2:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		case 3:
			result = this.GetFrame_Piece(number);
			break;
		case 4:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		case 5:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		case 6:
			result = this.GetFrame_Piece(number);
			break;
		case 7:
			result = this.GetFrame_Piece(number);
			break;
		case 8:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		case 9:
			if (small)
			{
				result = this.GetFrame_EquipIconSmall(number);
			}
			else
			{
				result = this.GetFrame_EquipIcon(number);
			}
			break;
		}
		return result;
	}

	public string GetFrame_HeroIcon(int number)
	{
		string result = null;
		switch ((number != 0) ? number : 1)
		{
		case 1:
			result = "kang-W";
			break;
		case 2:
			result = "kang-g";
			break;
		case 3:
			result = "kang-g1";
			break;
		case 4:
			result = "kang-b";
			break;
		case 5:
			result = "kang-b1";
			break;
		case 6:
			result = "kang-b2";
			break;
		case 7:
			result = "kang-v";
			break;
		case 8:
			result = "kang-v1";
			break;
		case 9:
			result = "kang-v2";
			break;
		case 10:
			result = "kang-v3";
			break;
		case 11:
			result = "kang-v4";
			break;
		case 12:
			result = "kang-o";
			break;
		}
		return result;
	}

	public string GetFrame_HeroName(int number)
	{
		return string.Empty;
	}

	public string GetFrame_HeroDetail(int number)
	{
		string result;
		switch (number)
		{
		case 1:
			result = "yx-tx_w72";
			break;
		case 2:
			result = "yx-tx_g72";
			break;
		case 3:
			result = "yx-tx_g1-72";
			break;
		case 4:
			result = "yx-tx_b72";
			break;
		case 5:
			result = "yx-tx_b172";
			break;
		case 6:
			result = "yx-tx_b2-72";
			break;
		case 7:
			result = "yx-tx_p72";
			break;
		case 8:
			result = "yx-tx_p1-72";
			break;
		case 9:
			result = "yx-tx_p2-72";
			break;
		case 10:
			result = "yx-tx_p3-72";
			break;
		case 11:
			result = "yx-tx_p4-72";
			break;
		case 12:
			result = "yx-tx_O-72";
			break;
		default:
			result = "yx-tx_w72";
			break;
		}
		return result;
	}

	public string GetFrame_EquipIcon(int number)
	{
		string result = null;
		switch ((number != 0) ? number : 1)
		{
		case 1:
			result = "xiaokuang_w_01";
			break;
		case 2:
			result = "xiaokuang_G_01";
			break;
		case 3:
			result = "xiaokuang_B_01";
			break;
		case 4:
			result = "xiaokuang_v_01";
			break;
		case 5:
			result = "xiaokuang_O_01";
			break;
		}
		return result;
	}

	public string GetFrame_EquipIconSmall(int number)
	{
		string result;
		switch (number)
		{
		case 1:
			result = "yx-tx_w72";
			break;
		case 2:
			result = "yx-tx_g72";
			break;
		case 3:
			result = "yx-tx_b72";
			break;
		case 4:
			result = "yx-tx_p72";
			break;
		case 5:
			result = "yx-tx_O-72";
			break;
		default:
			result = "yx-tx_w72";
			break;
		}
		return result;
	}

	public string GetFrame_Piece(int quality)
	{
		string result;
		switch (quality)
		{
		case 1:
			result = "zbsp-W";
			break;
		case 2:
			result = "zbsp-G";
			break;
		case 3:
			result = "zbsp-B";
			break;
		case 4:
			result = "zbsp-V";
			break;
		case 5:
			result = "zbsp-O";
			break;
		default:
			result = "zbsp-W";
			break;
		}
		return result;
	}

	public string GetFrameStyle(bool isSoulStone)
	{
		string result;
		if (isSoulStone)
		{
			result = "public_kuang_04";
		}
		else
		{
			result = "public_kuang_07";
		}
		return result;
	}

	public Color32 GetFrameColor(int quality, bool isEquipment = true)
	{
		Color32 result = default(Color32);
		int num = quality;
		if (!isEquipment)
		{
			if (quality == 2 || quality == 3)
			{
				num = 2;
			}
			else if (quality == 4 || quality == 5 || quality == 6)
			{
				num = 3;
			}
			else if (quality == 7 || quality == 8 || quality == 9 || quality == 10)
			{
				num = 4;
			}
			else if (quality == 11 || quality == 12)
			{
				num = 5;
			}
		}
		switch (num)
		{
		case 1:
			result = new Color32(160, 160, 160, 255);
			break;
		case 2:
			result = new Color32(0, 136, 26, 255);
			break;
		case 3:
			result = new Color32(0, 107, 197, 255);
			break;
		case 4:
			result = new Color32(137, 0, 188, 255);
			break;
		case 5:
			result = new Color32(178, 121, 0, 255);
			break;
		}
		return result;
	}

	public Color32 NewGetFrameColor(int quality, bool isEquipment = true)
	{
		Color32 result = default(Color32);
		switch (quality)
		{
		case 1:
			result = new Color32(160, 160, 160, 255);
			break;
		case 2:
			result = new Color32(0, 136, 26, 255);
			break;
		case 3:
			result = new Color32(0, 107, 197, 255);
			break;
		case 4:
			result = new Color32(137, 0, 188, 255);
			break;
		case 5:
			result = new Color32(178, 121, 0, 255);
			break;
		}
		return result;
	}

	public Dictionary<string, int> GetEnchantingEquip()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysGameItemsVo>();
		for (int i = 0; i < dicByType.Keys.Count; i++)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(dicByType.ElementAt(i).Key);
			if (dataById.type == 1 || dataById.type == 2 || dataById.type == 6 || dataById.type == 7 || dataById.type == 8)
			{
				int equipmentNumber = this.GetEquipmentNumber(dataById.items_id);
				if (equipmentNumber > 0)
				{
					dictionary.Add(dataById.items_id, equipmentNumber);
				}
			}
		}
		List<string> list = new List<string>();
		for (int j = 0; j < dictionary.Count; j++)
		{
			list.Add(dictionary.ElementAt(j).Key);
		}
		list = (from obj in list
		orderby BaseDataMgr.instance.GetDataById<SysGameItemsVo>(obj).enchant_points
		select obj).ToList<string>();
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		for (int k = 0; k < list.Count; k++)
		{
			SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(list[k]);
			if (dataById2.type == 8)
			{
				dictionary2.Add(list[k], dictionary[list[k]]);
			}
		}
		for (int l = 0; l < list.Count; l++)
		{
			SysGameItemsVo dataById3 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(list[l]);
			if (dataById3.type != 8)
			{
				dictionary2.Add(list[l], dictionary[list[l]]);
			}
		}
		return dictionary2;
	}

	public int GetEquipmentNumber(string EquipmentID)
	{
		EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId.ToString() == EquipmentID);
		return (equipmentInfoData != null) ? equipmentInfoData.Count : 0;
	}

	private int IsOrNotEnchant(int num, int index, HeroInfoData heroItem, List<string> allEquip)
	{
		if (num > 0)
		{
			int number = int.Parse(heroItem.EpMagic.Split(new char[]
			{
				'|'
			})[index]);
			List<string> list = this.MagicNumber(allEquip[index]);
			int num2 = this.MagicStarLevel(number, list);
			if (num2 < list.Count)
			{
				return 0;
			}
		}
		return 2;
	}

	public int MagicStarLevel(int number, List<string> magicNumber)
	{
		int result = 0;
		if (magicNumber == null || magicNumber.Count == 0 || number == 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < magicNumber.Count; i++)
		{
			num += int.Parse(magicNumber[i]);
			if (number < num)
			{
				result = i;
				break;
			}
			if (i == magicNumber.Count - 1 && number >= num)
			{
				result = magicNumber.Count;
			}
		}
		return result;
	}

	public List<string> MagicNumber(string equipmentID)
	{
		List<string> result = new List<string>();
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(equipmentID);
		if (dataById == null)
		{
			return null;
		}
		if (dataById.quality == 1)
		{
			return result;
		}
		return result;
	}

	public string GetChinaName(int type)
	{
		SysAttrNumberVo attrNumberData = BaseDataMgr.instance.GetAttrNumberData(type.ToString());
		string result;
		if (attrNumberData == null)
		{
			result = "未知属性 " + type + "，策划问题";
		}
		else
		{
			result = LanguageManager.Instance.GetStringById(attrNumberData.attrName);
		}
		return result;
	}

	public string GetEquipmentNature(string EquipmentID, int type = 1)
	{
		string text = null;
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmentID);
		if (dataById.type == 1)
		{
			Dictionary<string, float> natureDict = this.GetNatureDict(dataById.attribute);
			if (type == 1 && natureDict != null && natureDict.Keys.Contains("3") && natureDict.Keys.Contains("4") && natureDict.Keys.Contains("5") && natureDict["3"] == natureDict["4"] && natureDict["4"] == natureDict["5"])
			{
				text = "力量、敏捷、智力 +" + natureDict["3"] + "\n";
				natureDict.Remove("3");
				natureDict.Remove("4");
				natureDict.Remove("5");
				foreach (string current in natureDict.Keys)
				{
					text = string.Concat(new object[]
					{
						text,
						this.GetChinaName(int.Parse(current)),
						" +",
						natureDict[current],
						"\n"
					});
				}
			}
			else if (natureDict != null)
			{
				foreach (string current2 in natureDict.Keys)
				{
					text = string.Concat(new object[]
					{
						text,
						this.GetChinaName(int.Parse(current2)),
						" +",
						natureDict[current2],
						"\n"
					});
				}
			}
		}
		else if (dataById.type == 6 || dataById.type == 7)
		{
			EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId.ToString() == EquipmentID);
			SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(dataById.synthetic_id);
			string text2 = "集齐" + dataById.consumption + "个碎片可以合成" + dataById2.name;
			text = string.Concat(new object[]
			{
				text2,
				"\n\n合成需要碎片： ",
				(equipmentInfoData != null) ? equipmentInfoData.Count : 0,
				"/",
				dataById.consumption
			});
		}
		else
		{
			text = dataById.role + "\n";
		}
		return StringUtils.GetTailNoEnterStr(text);
	}

	public string GetPropertyEquipmentNature(string EquipmentID, int index)
	{
		string text = null;
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmentID);
		if (index == 0)
		{
			return this.GetEquipmentNature(EquipmentID, 1);
		}
		Dictionary<string, float> natureDict = this.GetNatureDict(dataById.enchant_attribute);
		if (dataById.type == 1)
		{
			Dictionary<string, float> natureDict2 = this.GetNatureDict(dataById.attribute);
			if (natureDict2 != null && natureDict2.Keys.Contains("3") && natureDict2.Keys.Contains("4") && natureDict2.Keys.Contains("5") && natureDict2["3"] == natureDict2["4"] && natureDict2["4"] == natureDict2["5"])
			{
				text = string.Concat(new object[]
				{
					"力量、敏捷、智力 +",
					natureDict2["3"],
					(index != 0) ? ("[00ff00] +" + natureDict["3"] * (float)index) : string.Empty,
					"[-]\n"
				});
				natureDict2.Remove("3");
				natureDict2.Remove("4");
				natureDict2.Remove("5");
				for (int i = 0; i < natureDict2.Keys.Count; i++)
				{
					text = string.Concat(new object[]
					{
						text,
						this.GetChinaName(int.Parse(natureDict2.ElementAt(i).Key)),
						" +",
						natureDict2[natureDict2.ElementAt(i).Key],
						(index != 0) ? ("[00ff00] +" + natureDict[natureDict2.ElementAt(i).Key] * (float)index) : string.Empty,
						"[-]\n"
					});
				}
			}
			else if (natureDict2 != null)
			{
				for (int j = 0; j < natureDict2.Keys.Count; j++)
				{
					text = string.Concat(new object[]
					{
						text,
						this.GetChinaName(int.Parse(natureDict2.ElementAt(j).Key)),
						" +",
						natureDict2[natureDict2.ElementAt(j).Key],
						(index != 0) ? ("[00ff00] +" + natureDict[natureDict2.ElementAt(j).Key] * (float)index) : string.Empty,
						"[-]\n"
					});
				}
			}
		}
		return StringUtils.GetTailNoEnterStr(text);
	}

	public List<string> GetHeroEquipmentNature(string EquipmentID, int index, int type = 1)
	{
		List<string> list = new List<string>();
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmentID);
		Dictionary<string, float> natureDict = this.GetNatureDict(dataById.attribute);
		Dictionary<string, float> natureDict2 = this.GetNatureDict(dataById.enchant_attribute);
		if (type == 1 && natureDict != null && natureDict.Keys.Contains("3") && natureDict.Keys.Contains("4") && natureDict.Keys.Contains("5") && natureDict["3"] == natureDict["4"] && natureDict["4"] == natureDict["5"])
		{
			list.Add("[2dabcf]力量、敏捷、智力:[ff2222]" + natureDict["3"] + ((index != 0) ? ("[00ff00]+" + natureDict2["3"] * (float)index) : string.Empty));
			natureDict.Remove("3");
			natureDict.Remove("4");
			natureDict.Remove("5");
			for (int i = 0; i < natureDict.Keys.Count; i++)
			{
				list.Add(string.Concat(new object[]
				{
					"[2dabcf]",
					this.GetChinaName(int.Parse(natureDict.ElementAt(i).Key)),
					":[ff2222]",
					natureDict[natureDict.ElementAt(i).Key],
					(index != 0) ? ("[00ff00]+" + natureDict2[natureDict.ElementAt(i).Key] * (float)index) : string.Empty
				}));
			}
		}
		else if (natureDict != null)
		{
			for (int j = 0; j < natureDict.Keys.Count; j++)
			{
				list.Add(string.Concat(new object[]
				{
					"[2dabcf]",
					this.GetChinaName(int.Parse(natureDict.ElementAt(j).Key)),
					":[ff2222]",
					natureDict[natureDict.ElementAt(j).Key],
					(index != 0) ? ("[00ff00]+" + natureDict2[natureDict.ElementAt(j).Key] * (float)index) : string.Empty
				}));
			}
		}
		return list;
	}

	public Dictionary<string, float> GetNatureDict(string natureList)
	{
		Dictionary<string, float> dictionary = new Dictionary<string, float>();
		string[] array = natureList.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Split(new char[]
			{
				'|'
			})[0];
			float value = float.Parse(array[i].Split(new char[]
			{
				'|'
			})[1]);
			if (dictionary == null || !dictionary.Keys.Contains(text))
			{
				dictionary.Add(text, value);
			}
		}
		return dictionary;
	}

	public Dictionary<string, int> EquipmentComposition(string EquipmnetID)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmnetID);
		string consumption = dataById.consumption;
		if (dataById.synthetic_way != 1 && dataById.synthetic_way != 0 && consumption != "[]")
		{
			string[] array = consumption.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				dictionary.Add(array[i].Split(new char[]
				{
					'|'
				})[0], int.Parse(array[i].Split(new char[]
				{
					'|'
				})[1]));
			}
		}
		return dictionary;
	}

	public int EquipmentFragmentComposition(string EquipmnetID)
	{
		int result = 0;
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmnetID);
		string consumption = dataById.consumption;
		if (dataById.synthetic_way == 1)
		{
			result = int.Parse(consumption);
		}
		return result;
	}

	public int EquipmentNumber(string EquipmnetID)
	{
		EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == int.Parse(EquipmnetID));
		if (equipmentInfoData != null)
		{
			return equipmentInfoData.Count;
		}
		return 0;
	}

	public int EquipmentShow(string EquipmentID, HeroInfoData HeroID)
	{
		if (string.IsNullOrEmpty(EquipmentID))
		{
			ClientLogger.Error("===============>某件装备显示状况 EquipmentID报空了。");
			return 0;
		}
		int num = 0;
		if (!StringUtils.CheckValid(EquipmentID))
		{
			return 3;
		}
		EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId == int.Parse(EquipmentID));
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(EquipmentID);
		if (equipmentInfoData != null && equipmentInfoData.Count > 0)
		{
			if (this.GetLevel(HeroID.Exp) >= dataById.level)
			{
				num = 1;
			}
			else
			{
				num = 2;
			}
		}
		else if (dataById.synthetic_way == 0)
		{
			num = 5;
		}
		else if (!this.IsCanComposition(EquipmentID))
		{
			num = 3;
		}
		else if (num != 3 && num != 5)
		{
			if (this.GetLevel(HeroID.Exp) >= dataById.level)
			{
				num = 4;
			}
			else
			{
				num = 6;
			}
		}
		return num;
	}

	public bool IsCanComposition(string EquipmentID)
	{
		CharacterDataMgr.<IsCanComposition>c__AnonStorey30C <IsCanComposition>c__AnonStorey30C = new CharacterDataMgr.<IsCanComposition>c__AnonStorey30C();
		bool result = true;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		<IsCanComposition>c__AnonStorey30C.recordOwn = new Dictionary<string, int>();
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
		dictionary2.Add(EquipmentID, 1);
		while (dictionary2.Count != 0)
		{
			dictionary3 = new Dictionary<string, int>(dictionary2);
			for (int m = 0; m < dictionary3.Count; m++)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(dictionary3.ElementAt(m).Key);
				if (dataById.consumption == "[]" || dataById.synthetic_way == 1)
				{
					if (dictionary.ContainsKey(dictionary3.ElementAt(m).Key))
					{
						Dictionary<string, int> dictionary4;
						Dictionary<string, int> expr_A0 = dictionary4 = dictionary;
						string key;
						string expr_B4 = key = dictionary3.ElementAt(m).Key;
						int num = dictionary4[key];
						expr_A0[expr_B4] = num + dictionary3.ElementAt(m).Value;
					}
					else
					{
						dictionary.Add(dictionary3.ElementAt(m).Key, dictionary3.ElementAt(m).Value);
					}
				}
				else
				{
					string[] array = dataById.consumption.Split(new char[]
					{
						','
					});
					for (int j = 0; j < array.Length; j++)
					{
						string[] str = array[j].ToString().Split(new char[]
						{
							'|'
						});
						EquipmentInfoData equipmentInfoData = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId.ToString() == str[0].ToString());
						int num2 = 0;
						if (equipmentInfoData != null)
						{
							num2 = equipmentInfoData.Count;
						}
						if (int.Parse(str[1]) * dictionary3.ElementAt(m).Value <= num2)
						{
							if (<IsCanComposition>c__AnonStorey30C.recordOwn.ContainsKey(str[0]))
							{
								Dictionary<string, int> recordOwn;
								Dictionary<string, int> expr_1CF = recordOwn = <IsCanComposition>c__AnonStorey30C.recordOwn;
								string key;
								string expr_1DB = key = str[0];
								int num = recordOwn[key];
								expr_1CF[expr_1DB] = num + int.Parse(str[1]) * dictionary3.ElementAt(m).Value;
							}
							else
							{
								<IsCanComposition>c__AnonStorey30C.recordOwn.Add(str[0], int.Parse(str[1]) * dictionary3.ElementAt(m).Value);
							}
						}
						else
						{
							if (<IsCanComposition>c__AnonStorey30C.recordOwn.ContainsKey(str[0]))
							{
								Dictionary<string, int> recordOwn2;
								Dictionary<string, int> expr_271 = recordOwn2 = <IsCanComposition>c__AnonStorey30C.recordOwn;
								string key;
								string expr_27D = key = str[0];
								int num = recordOwn2[key];
								expr_271[expr_27D] = num + num2;
							}
							else if (num2 != 0)
							{
								<IsCanComposition>c__AnonStorey30C.recordOwn.Add(str[0], num2);
							}
							if (dictionary2.ContainsKey(str[0]))
							{
								Dictionary<string, int> dictionary5;
								Dictionary<string, int> expr_2CD = dictionary5 = dictionary2;
								string key;
								string expr_2D9 = key = str[0];
								int num = dictionary5[key];
								expr_2CD[expr_2D9] = num + (int.Parse(str[1]) * dictionary3.ElementAt(m).Value - num2);
							}
							else
							{
								dictionary2.Add(str[0], int.Parse(str[1]) * dictionary3.ElementAt(m).Value - num2);
							}
						}
					}
				}
			}
			for (int k = 0; k < dictionary3.Count; k++)
			{
				dictionary2.Remove(dictionary3.ElementAt(k).Key);
			}
		}
		for (int l = 0; l < dictionary.Count; l++)
		{
			if (<IsCanComposition>c__AnonStorey30C.recordOwn.ContainsKey(dictionary.ElementAt(l).Key))
			{
				Dictionary<string, int> recordOwn3;
				Dictionary<string, int> expr_3DC = recordOwn3 = <IsCanComposition>c__AnonStorey30C.recordOwn;
				string key;
				string expr_3F0 = key = dictionary.ElementAt(l).Key;
				int num = recordOwn3[key];
				expr_3DC[expr_3F0] = num + dictionary.ElementAt(l).Value;
			}
			else
			{
				<IsCanComposition>c__AnonStorey30C.recordOwn.Add(dictionary.ElementAt(l).Key, dictionary.ElementAt(l).Value);
			}
		}
		int i;
		for (i = 0; i < <IsCanComposition>c__AnonStorey30C.recordOwn.Count; i++)
		{
			EquipmentInfoData equipmentInfoData2 = ModelManager.Instance.Get_equipmentList_X().Find((EquipmentInfoData obj) => obj.ModelId.ToString() == <IsCanComposition>c__AnonStorey30C.recordOwn.ElementAt(i).Key);
			int num3 = 0;
			if (equipmentInfoData2 != null)
			{
				num3 = equipmentInfoData2.Count;
			}
			if (<IsCanComposition>c__AnonStorey30C.recordOwn.ElementAt(i).Value > num3)
			{
				result = false;
				break;
			}
		}
		return result;
	}

	public List<string> GetPortrait(CharacterDataMgr.Portrait type)
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersHeadportraitVo>();
		if (dicByType == null)
		{
			Singleton<TipView>.Instance.ShowViewSetText("SysSummonersHeadportraitVo配置表缺少", 1f);
			return null;
		}
		switch (type)
		{
		case CharacterDataMgr.Portrait.CommonPortrait:
		{
			string text = ModelManager.Instance.Get_userData_filed_X("OwnIconStr");
			if (!string.IsNullOrEmpty(text.Trim()))
			{
				list = text.Split(new char[]
				{
					'|'
				}).ToList<string>();
			}
			break;
		}
		case CharacterDataMgr.Portrait.SpecialPortrait:
			for (int i = 0; i < dicByType.Keys.Count; i++)
			{
				SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(dicByType.ElementAt(i).Key);
				if (!list2.Contains(dicByType.ElementAt(i).Key) && dataById.portrait_type == 3 && dataById.is_hidden != 1)
				{
					list.Add(dataById.headportrait_id.ToString());
				}
			}
			break;
		case CharacterDataMgr.Portrait.SociatyPortrait:
			for (int j = 0; j < dicByType.Keys.Count; j++)
			{
				SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(dicByType.ElementAt(j).Key);
				if (dataById.portrait_type == 2 && dataById.portrait_type == 1)
				{
					list.Add(dataById.headportrait_id.ToString());
				}
			}
			break;
		}
		return list;
	}

	public List<string> GetSocietyPortrait()
	{
		List<string> list = new List<string>();
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersHeadportraitVo>();
		if (dicByType == null)
		{
			Singleton<TipView>.Instance.ShowViewSetText("SysSummonersHeadportraitVo配置表缺少", 1f);
			return null;
		}
		foreach (string current in dicByType.Keys)
		{
			SysSummonersHeadportraitVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(current);
			if (dataById.portrait_type == 2)
			{
				list.Add(current);
			}
		}
		return list;
	}

	public List<string> GetSummonerFrame()
	{
		List<string> list = new List<string>();
		string text = ModelManager.Instance.Get_userData_filed_X("OwnPictureFrame");
		if (!string.IsNullOrEmpty(text.Trim()))
		{
			for (int i = 0; i < text.Split(new char[]
			{
				'|'
			}).Length; i++)
			{
				list.Add(text.Split(new char[]
				{
					'|'
				})[i]);
			}
		}
		return list;
	}

	public List<string> GetNoHaveFrame()
	{
		List<string> list = new List<string>();
		List<string> summonerFrame = this.GetSummonerFrame();
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersPictureframeVo>();
		if (dicByType == null)
		{
			Singleton<TipView>.Instance.ShowViewSetText("SysSummonersPictureframeVo配置表缺少", 1f);
			return null;
		}
		foreach (string current in dicByType.Keys)
		{
			SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(current);
			if (!summonerFrame.Contains(current) && dataById.pictureframe_type == 1 && dataById.is_hidden != 1)
			{
				list.Add(current);
			}
		}
		return list;
	}

	public string GetCoinSpriteName(int i, bool has)
	{
		string result = string.Empty;
		switch (i)
		{
		case 1:
			if (has)
			{
				result = "meiri_0012_jinbi";
			}
			else
			{
				result = "bb_jb";
			}
			break;
		case 2:
			if (has)
			{
				result = "meiri_0007_diamond";
			}
			else
			{
				result = "jjc_zhuanshi_01";
			}
			break;
		case 3:
			if (has)
			{
				result = "hb03";
			}
			else
			{
				result = "hb03";
			}
			break;
		case 4:
			if (has)
			{
				result = "hb04";
			}
			else
			{
				result = "hb04";
			}
			break;
		case 5:
			if (has)
			{
				result = "hb05";
			}
			else
			{
				result = "hb05";
			}
			break;
		case 6:
			break;
		default:
			if (has)
			{
				result = "hb06";
			}
			else
			{
				result = "hb06";
			}
			break;
		}
		return result;
	}

	public string ShowHeroName(int HeroGradeNumber, SysHeroMainVo item)
	{
		string result = item.name;
		if (HeroGradeNumber > 2 && HeroGradeNumber <= 3)
		{
			result = item.name + "[e59c00]+" + (HeroGradeNumber - 2);
		}
		else if (HeroGradeNumber > 4 && HeroGradeNumber <= 6)
		{
			result = item.name + "[e59c00]+" + (HeroGradeNumber - 4);
		}
		else if (HeroGradeNumber > 7 && HeroGradeNumber <= 10)
		{
			result = item.name + "[e59c00]+" + (HeroGradeNumber - 7);
		}
		else if (HeroGradeNumber > 11 && HeroGradeNumber <= 12)
		{
			result = item.name + "[e59c00]+" + (HeroGradeNumber - 11);
		}
		return result;
	}

	public void ShowItem(GameObject object_1, string indexID, CharacterDataMgr.PropsItem type = CharacterDataMgr.PropsItem.EquipmentProps, int number = 1, int materialType = 1)
	{
		GeneralItem component = object_1.GetComponent<GeneralItem>();
		if (type == CharacterDataMgr.PropsItem.HeroProps)
		{
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(indexID);
			component.ItemTexture.mainTexture = ResourceManager.Load<Texture>(heroMainData.avatar_icon, true, true, null, 0, false);
			component.ItemTexture.material = this.ReturnMaterialType(1);
			component.textureFrame.spriteName = this.GetFrameStyle(false);
			component.textureFrame.color = this.GetFrameColor(1, false);
			for (int i = 0; i < component.Star.childCount; i++)
			{
				if (i < 1)
				{
					component.Star.GetChild(i).gameObject.SetActive(true);
				}
				else
				{
					component.Star.GetChild(i).gameObject.SetActive(false);
				}
			}
			component.starGrid.enabled = true;
			component.starGrid.Reposition();
			component.Number.gameObject.SetActive(false);
			component.Number2.gameObject.SetActive(false);
			component.Type.gameObject.SetActive(false);
			component.Can.gameObject.SetActive(false);
			component.Star.gameObject.SetActive(true);
		}
		else if (type == CharacterDataMgr.PropsItem.EquipmentProps)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(indexID);
			component.ItemTexture.mainTexture = ResourceManager.Load<Texture>(dataById.icon, true, true, null, 0, false);
			if (dataById.type == 6 || dataById.type == 7 || dataById.type == 3)
			{
				component.textureFrame.spriteName = this.GetFrameStyle(true);
				component.textureFrame.color = this.GetFrameColor(dataById.quality, true);
				component.Type.gameObject.SetActive(true);
				if (dataById.type == 3)
				{
					component.Type.spriteName = "public_ui_09";
				}
				else
				{
					component.Type.spriteName = "public_ui_08";
				}
				if (materialType == 1)
				{
					component.ItemTexture.material = this.ReturnMaterialType(2);
				}
				else
				{
					component.ItemTexture.material = this.ReturnMaterialType(3);
				}
			}
			else
			{
				component.textureFrame.spriteName = this.GetFrameStyle(false);
				component.textureFrame.color = this.GetFrameColor(dataById.quality, true);
				component.Type.gameObject.SetActive(false);
				component.ItemTexture.material = this.ReturnMaterialType(1);
			}
			if (number > 99)
			{
				component.Number.text = "99+";
			}
			else
			{
				component.Number.text = ((number != 1) ? number.ToString() : string.Empty);
			}
			component.Number2.gameObject.SetActive(false);
			component.Number.gameObject.SetActive(true);
			component.Can.gameObject.SetActive(false);
			component.Star.gameObject.SetActive(false);
		}
		else if (type == CharacterDataMgr.PropsItem.EnchantingView)
		{
			SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(indexID);
			component.ItemTexture.mainTexture = ResourceManager.Load<Texture>(dataById2.icon, true, true, null, 0, false);
			if (dataById2.type == 6 || dataById2.type == 7 || dataById2.type == 3)
			{
				component.textureFrame.spriteName = this.GetFrameStyle(true);
				component.textureFrame.color = this.GetFrameColor(dataById2.quality, true);
				component.Type.gameObject.SetActive(true);
				if (dataById2.type == 3)
				{
					component.Type.spriteName = "public_ui_09";
				}
				else
				{
					component.Type.spriteName = "public_ui_08";
				}
				component.Number2Sprite.spriteName = "sp_dengji_20150129";
				if (materialType == 1)
				{
					component.ItemTexture.material = this.ReturnMaterialType(2);
				}
				else
				{
					component.ItemTexture.material = this.ReturnMaterialType(3);
				}
			}
			else
			{
				component.textureFrame.spriteName = this.GetFrameStyle(false);
				component.textureFrame.color = this.GetFrameColor(dataById2.quality, true);
				component.Type.gameObject.SetActive(false);
				component.Number2Sprite.spriteName = "bb_001";
				component.ItemTexture.material = this.ReturnMaterialType(1);
			}
			component.Number2.text = number.ToString();
			component.ItemTexture.gameObject.SetActive(true);
			component.Number.gameObject.SetActive(false);
			component.Can.gameObject.SetActive(false);
			component.Star.gameObject.SetActive(false);
			component.Number2.gameObject.SetActive(true);
			component.Sign.gameObject.SetActive(false);
		}
	}

	public Material ReturnMaterialType(int type)
	{
		Material result = null;
		switch (type)
		{
		case 2:
			result = (Resources.Load("Material/DiamondNoneClipMask") as Material);
			break;
		case 3:
			result = (Resources.Load("Material/DiamondSoftClipMask") as Material);
			break;
		case 4:
			result = (Resources.Load("Material/CircularNoneClipMask") as Material);
			break;
		case 5:
			result = (Resources.Load("Material/CircularSoftClipMask") as Material);
			break;
		case 6:
			result = (Resources.Load("Material/CircularNoneClipGrayMask") as Material);
			break;
		case 7:
			result = (Resources.Load("Material/CircularGraySoftClipMask") as Material);
			break;
		case 8:
			result = (Resources.Load("Material/SquareNoneClipGrayMask") as Material);
			break;
		case 9:
			result = (Resources.Load("Material/NoneClipGrayMask") as Material);
			break;
		case 10:
			result = (Resources.Load("Material/SoftClipGrayMask") as Material);
			break;
		}
		return result;
	}

	public Dictionary<AttrType, float> GetHeroEquipAllNatures(Dictionary<AttrType, float> dict)
	{
		float power = 0f;
		float agile = 0f;
		float intelligence = 0f;
		if (dict.Keys.Contains(AttrType.Power))
		{
			power = dict[AttrType.Power];
		}
		if (dict.Keys.Contains(AttrType.Intelligence))
		{
			agile = dict[AttrType.Intelligence];
		}
		if (dict.Keys.Contains(AttrType.Agile))
		{
			intelligence = dict[AttrType.Agile];
		}
		Dictionary<AttrType, float> heroSecondProperty = CharacterDataMgr.instance.GetHeroSecondProperty(null, power, agile, intelligence);
		for (int i = 0; i < heroSecondProperty.Keys.Count; i++)
		{
			if (dict.ContainsKey(heroSecondProperty.ElementAt(i).Key))
			{
				dict[heroSecondProperty.ElementAt(i).Key] = dict[heroSecondProperty.ElementAt(i).Key] + heroSecondProperty.ElementAt(i).Value;
			}
			else
			{
				dict.Add(heroSecondProperty.ElementAt(i).Key, heroSecondProperty.ElementAt(i).Value);
			}
		}
		return dict;
	}

	public Dictionary<AttrType, float> GetHeroProperty(string npc_id, float attr_factor = 1f)
	{
		SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(npc_id);
		float num = 0f;
		float value = heroMainData.power_groth + num;
		float power = heroMainData.power;
		float value2 = heroMainData.agile_groth + num;
		float agile = heroMainData.agile;
		float value3 = heroMainData.intelligence_groth + num;
		float intelligence = heroMainData.intelligence;
		return new Dictionary<AttrType, float>
		{
			{
				AttrType.PowerGroth,
				value
			},
			{
				AttrType.Power,
				power * attr_factor
			},
			{
				AttrType.AgileGroth,
				value2
			},
			{
				AttrType.Agile,
				agile * attr_factor
			},
			{
				AttrType.IntelligenceGroth,
				value3
			},
			{
				AttrType.Intelligence,
				intelligence * attr_factor
			}
		};
	}

	public Dictionary<AttrType, float> GetHeroSecondProperty(string npc_id, float power, float agile, float intelligence)
	{
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		SysHeroMainVo sysHeroMainVo = null;
		if (npc_id != null)
		{
			sysHeroMainVo = BaseDataMgr.instance.GetHeroMainData(npc_id);
		}
		float value = ((sysHeroMainVo != null) ? sysHeroMainVo.hp : 0f) + power * 19f;
		dictionary.Add(AttrType.Hp, value);
		dictionary.Add(AttrType.HpMax, value);
		float value2 = (sysHeroMainVo != null) ? sysHeroMainVo.mp : 0f;
		dictionary.Add(AttrType.Mp, value2);
		dictionary.Add(AttrType.MpMax, value2);
		float value3 = ((sysHeroMainVo != null) ? sysHeroMainVo.mp_restore : 0f) + intelligence * 0.04f;
		dictionary.Add(AttrType.MpRestore, value3);
		float value4 = ((sysHeroMainVo != null) ? sysHeroMainVo.hp_restore : 0f) + power * 0.03f;
		dictionary.Add(AttrType.HpRestore, value4);
		float value5 = ((sysHeroMainVo != null) ? sysHeroMainVo.atk_speed : 0f) - agile / 250f;
		dictionary.Add(AttrType.AttackSpeed, value5);
		float value6 = ((sysHeroMainVo != null) ? sysHeroMainVo.armor : 0f) + agile / 7f;
		dictionary.Add(AttrType.Armor, value6);
		float value7 = (sysHeroMainVo != null) ? sysHeroMainVo.dodge_prop : 0f;
		dictionary.Add(AttrType.DodgeProp, value7);
		float value8 = (sysHeroMainVo != null) ? sysHeroMainVo.hit_prop : 0f;
		dictionary.Add(AttrType.HitProp, value8);
		float value9 = (sysHeroMainVo != null) ? sysHeroMainVo.move_speed : 0f;
		dictionary.Add(AttrType.MoveSpeed, value9);
		dictionary.Add(AttrType.Attack, power);
		return dictionary;
	}

	public Dictionary<AttrType, float> GetEquipsAdditive(string equips, int magicStar = 0, Dictionary<AttrType, float> dict = null)
	{
		Dictionary<AttrType, float> dictionary;
		if (dict == null || dict.Keys.Count == 0)
		{
			dictionary = new Dictionary<AttrType, float>();
		}
		else
		{
			dictionary = dict;
		}
		string[] stringValue = StringUtils.GetStringValue(equips, ',');
		for (int i = 0; i < stringValue.Length; i++)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(stringValue[i]);
			if (dataById == null)
			{
				Debug.LogError("Character.GetEquipsAdditive , equips [" + equips + "] has null id: " + stringValue[i]);
			}
			else
			{
				string[] array = null;
				if (magicStar > 0)
				{
					array = StringUtils.GetStringValue(dataById.enchant_attribute, ',');
				}
				string[] stringValue2 = StringUtils.GetStringValue(dataById.attribute, ',');
				for (int j = 0; j < stringValue2.Length; j++)
				{
					float[] stringToFloat = StringUtils.GetStringToFloat(stringValue2[j], '|');
					AttrType key = (AttrType)stringToFloat[0];
					float num = stringToFloat[1] + ((magicStar != 0) ? StringUtils.GetStringToFloat(array[j], '|')[1] : 0f) * (float)magicStar;
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, 0f);
					}
					dictionary[key] += num;
				}
			}
		}
		return dictionary;
	}

	public Dictionary<AttrType, float> GetTalentsAdditive()
	{
		Dictionary<AttrType, float> result = new Dictionary<AttrType, float>();
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		TalentDataParse.Parse(ref result, ref dictionary);
		return result;
	}

	public void SaveNowUserLevel(long exp)
	{
		int userLevel = this.GetUserLevel(exp);
		string b = ModelManager.Instance.Get_userData_X().UserId;
		if (this.userId == string.Empty)
		{
			this.userId = b;
		}
		if (this.userId == b && this.accountLevel != 0 && this.accountLevel < userLevel)
		{
			LevelManager.Instance.CheckShowUnlockView(userLevel);
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SetExtData("2");
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("3");
			}
		}
		else
		{
			this.userId = b;
		}
		this.accountLevel = userLevel;
	}

	public bool CheackUserLevelUp(long exp)
	{
		int userLevel = this.GetUserLevel(exp);
		if (this.accountLevel < userLevel)
		{
			LevelManager.Instance.CheckShowUnlockView(userLevel);
			if (GlobalSettings.isLoginByHoolaiSDK)
			{
				InitSDK.instance.SetExtData("2");
			}
			else if (GlobalSettings.isLoginByAnySDK)
			{
				InitSDK.instance.SetAnySDKExtData("3");
			}
			return true;
		}
		return false;
	}

	public bool CheackDiamondChangeBool()
	{
		long num = ModelManager.Instance.Get_userData_filed_X("Diamonds");
		return (long)int.Parse(MenuTopBarView.CrystalNumber.text) != num;
	}

	public int GetUserLevel(long exp)
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersLevelVo>();
		object obj2 = (from obj in dicByType.Values.ToList<object>()
		where (long)((SysSummonersLevelVo)obj).experience > exp
		select obj).FirstOrDefault<object>();
		if (obj2 != null)
		{
			int num = int.Parse(((SysSummonersLevelVo)obj2).level_id);
			return num - 1;
		}
		return int.Parse(dicByType.Keys.ElementAt(dicByType.Count - 1));
	}

	public int GetUserCurrentExp(long exp)
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersLevelVo>();
		if ((from obj in dicByType.Values.ToList<object>()
		where (long)((SysSummonersLevelVo)obj).experience > exp
		select obj).FirstOrDefault<object>() == null)
		{
			return ((SysSummonersLevelVo)dicByType.Values.ElementAt(dicByType.Count - 1)).experience - ((SysSummonersLevelVo)dicByType.Values.ElementAt(dicByType.Count - 2)).experience;
		}
		object obj2 = (from obj in dicByType.Values.ToList<object>()
		where (long)((SysSummonersLevelVo)obj).experience <= exp
		select obj).LastOrDefault<object>();
		int result;
		if (obj2 == null)
		{
			result = (int)exp;
		}
		else
		{
			result = (int)exp - ((SysSummonersLevelVo)obj2).experience;
		}
		return result;
	}

	public int GetUserNextLevelExp(long exp)
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersLevelVo>();
		object obj3 = (from obj in dicByType.Values.ToList<object>()
		where (long)((SysSummonersLevelVo)obj).experience > exp
		select obj).FirstOrDefault<object>();
		object obj2 = (from obj in dicByType.Values.ToList<object>()
		where (long)((SysSummonersLevelVo)obj).experience <= exp
		select obj).LastOrDefault<object>();
		int result;
		if (obj3 == null)
		{
			result = ((SysSummonersLevelVo)obj2).experience - ((SysSummonersLevelVo)dicByType.Values.ElementAt(dicByType.Count - 2)).experience;
		}
		else if (obj2 == null)
		{
			result = ((SysSummonersLevelVo)obj3).experience;
		}
		else
		{
			result = ((SysSummonersLevelVo)obj3).experience - ((SysSummonersLevelVo)obj2).experience;
		}
		return result;
	}

	public int GetLevel(long exp)
	{
		return 1;
	}

	public int GetMaxHeroLevel()
	{
		Dictionary<string, object> dicByType = BaseDataMgr.instance.GetDicByType<SysSummonersLevelVo>();
		if (dicByType != null)
		{
			return ((SysSummonersLevelVo)dicByType.ElementAt(dicByType.Count - 1).Value).hero_main;
		}
		return 1;
	}

	public bool IsFullExp(int userLevel, long exp)
	{
		SysSummonersLevelVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>(userLevel.ToString());
		bool result;
		if (dataById.hero_main == CharacterDataMgr.instance.GetMaxHeroLevel())
		{
			result = (dataById.hero_main <= CharacterDataMgr.instance.GetLevel(exp));
		}
		else
		{
			result = (dataById.hero_main < CharacterDataMgr.instance.GetLevel(exp + 1L));
		}
		return result;
	}

	public int GetVIPGrade(int number)
	{
		return 0;
	}

	public int GetVIPShow(int number, int type = 1)
	{
		return 0;
	}

	public bool JudgeVIPFullLevel(int level)
	{
		return false;
	}

	public int GetPowerMax(int level = -1)
	{
		int num = ModelManager.Instance.Get_userData_filed_X("VIP");
		int result;
		if (level < 0)
		{
			if (ModelManager.Instance.Get_userData_X() == null)
			{
				return 0;
			}
			result = num * 10 + 100;
		}
		else
		{
			result = 30 + 3 * level + num * 6;
		}
		return result;
	}
}
