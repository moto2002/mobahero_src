using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace Mobaheros.AI.Equipment
{
	public class AiEquipmentMgr
	{
		private static AiEquipmentMgr _intance;

		private string[] treeNames = new string[]
		{
			"10203",
			"10203",
			"10203",
			"10203",
			"10203",
			"10203"
		};

		private Dictionary<Units, List<EquipmentTree>> heroEquipmentTreeTable = new Dictionary<Units, List<EquipmentTree>>();

		public static AiEquipmentMgr Instance
		{
			get
			{
				if (AiEquipmentMgr._intance == null)
				{
					AiEquipmentMgr._intance = new AiEquipmentMgr();
				}
				return AiEquipmentMgr._intance;
			}
		}

		private AiEquipmentMgr()
		{
		}

		private List<string> InitRecommendEquip(Units target)
		{
			List<string> list = new List<string>();
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(target.npc_id);
			if (heroMainData == null)
			{
				ClientLogger.Error("hero data is null with id:" + target.npc_id + ",name:" + target.name);
				list.AddRange(this.treeNames);
				return null;
			}
			string[] equipStrings = this.GetEquipStrings(heroMainData);
			list.AddRange(equipStrings);
			return list;
		}

		private string[] GetEquipStrings(SysHeroMainVo heroData)
		{
			if (string.IsNullOrEmpty(heroData.recommend_equip))
			{
				return this.treeNames;
			}
			string[] array = heroData.recommend_equip.Split(new char[]
			{
				','
			});
			if (string.IsNullOrEmpty(array[0]))
			{
				return this.treeNames;
			}
			string[] array2 = array[0].Split(new char[]
			{
				'|'
			});
			if (array2.Length < 2)
			{
				return this.treeNames;
			}
			string text = array2[1];
			if (string.IsNullOrEmpty(text))
			{
				return this.treeNames;
			}
			SysRecommendEquipmentVo dataById = BaseDataMgr.instance.GetDataById<SysRecommendEquipmentVo>(text);
			if (!StringUtils.CheckValid(dataById.equipments))
			{
				return this.treeNames;
			}
			return StringUtils.GetStringValue(dataById.equipments, ',');
		}

		public void InitHeroEquipTree(Units hero)
		{
			if (!this.heroEquipmentTreeTable.ContainsKey(hero))
			{
				List<string> list = this.InitRecommendEquip(hero);
				if (list == null)
				{
					return;
				}
				List<EquipmentTree> list2 = new List<EquipmentTree>();
				this.heroEquipmentTreeTable.Add(hero, list2);
				foreach (string current in list)
				{
					EquipmentTree item = EquipmentTree.CreateTree(current, hero);
					list2.Add(item);
				}
			}
		}

		public List<EquipmentTree> GetHeroEquipTree(Units hero)
		{
			if (this.heroEquipmentTreeTable.ContainsKey(hero))
			{
				return this.heroEquipmentTreeTable[hero];
			}
			return null;
		}

		public void ResetAllData()
		{
			this.heroEquipmentTreeTable.Clear();
		}
	}
}
