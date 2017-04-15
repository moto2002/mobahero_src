using Assets.Scripts.GUILogic.View.BattleEquipment;
using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaHeros;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class HeroDataManager : DataManager
{
	private bool isInited;

	public SysHeroMainVo configAttr;

	public HeroDataManager()
	{
	}

	public HeroDataManager(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		if (this.isInited)
		{
			return;
		}
		this.isInited = true;
		if (this.self.npc_id != "[]")
		{
			this.configAttr = BaseDataMgr.instance.GetHeroMainData(this.self.npc_id);
			if (this.configAttr == null)
			{
				ClientLogger.Error("hero SetConfigData Data Error : " + this.self.npc_id);
				return;
			}
			this.MainAttrType = this.GetMainAttr(this.configAttr.character_type);
			base.ClearDataPool();
			this.SetConfigData();
			this.CollectGameItemAttr();
			this.CollectTalent();
			this.SetL1AttrGrowth(1);
			base.CalcAllAttrs();
			this.InitDynamicData();
		}
	}

	private AttrType GetMainAttr(int character_type)
	{
		switch (character_type)
		{
		case 1:
			return AttrType.Power;
		case 2:
			return AttrType.Agile;
		case 3:
			return AttrType.Intelligence;
		default:
			return AttrType.None;
		}
	}

	private void SetConfigData()
	{
		base.SetData(DataType.DisplayName, this.configAttr.name);
		base.SetData(DataType.Description, this.configAttr.description);
		base.SetData(DataType.CharacterType, this.configAttr.character_type);
		base.SetData(DataType.AttackType, this.configAttr.atk_type);
		base.SetData(DataType.Attacks, StringUtils.GetStringValue(this.configAttr.attack_id, ','));
		base.SetData(DataType.Skills, StringUtils.GetStringValue(this.configAttr.skill_id, ','));
		base.SetData(DataType.AvatarIcon, this.configAttr.avatar_icon);
		base.SetData(DataType.ModelId, this.configAttr.model_id);
		base.SetData(DataType.MusicId, this.configAttr.music_id);
		base.SetData(DataType.DefaultAttackSpeed, this.configAttr.atk_speed);
		base.SetData(DataType.DefaultMoveSpeed, this.configAttr.move_speed);
		if (base.GetData<string>(DataType.EffectId) == "Default")
		{
			base.SetData(DataType.EffectId, this.configAttr.effect_id);
		}
		base.SetData(DataType.ResistanceParam, this.configAttr.resistance_param);
		base.SetData(DataType.HpRestoreParam, this.configAttr.hp_restore_param);
		base.SetData(DataType.MpRestoreParam, this.configAttr.mp_restore_param);
		base.InitProperty(AttrType.Hp, this.configAttr.hp);
		base.InitProperty(AttrType.Mp, this.configAttr.mp);
		base.InitProperty(AttrType.Power, this.configAttr.power);
		base.InitProperty(AttrType.Agile, this.configAttr.agile);
		base.InitProperty(AttrType.Intelligence, this.configAttr.intelligence);
		base.InitProperty(AttrType.Attack, this.configAttr.attack);
		base.InitProperty(AttrType.Armor, this.configAttr.armor);
		base.InitProperty(AttrType.MagicResist, this.configAttr.resistance);
		base.InitProperty(AttrType.HpMax, this.configAttr.hp);
		base.InitProperty(AttrType.MpMax, this.configAttr.mp);
		base.InitProperty(AttrType.HpRestore, this.configAttr.hp_restore);
		base.InitProperty(AttrType.MpRestore, this.configAttr.mp_restore);
		base.InitProperty(AttrType.PhysicPower, this.configAttr.pysic_power);
		base.InitProperty(AttrType.MagicPower, this.configAttr.magic_power);
		base.InitProperty(AttrType.HitProp, this.configAttr.hit_prop);
		base.InitProperty(AttrType.DodgeProp, this.configAttr.dodge_prop);
		base.InitProperty(AttrType.PhysicCritProp, this.configAttr.physic_crit_prop);
		base.InitProperty(AttrType.MagicCritProp, this.configAttr.magic_crit_prop);
		base.InitProperty(AttrType.AttackSpeed, this.configAttr.atk_speed);
		base.InitProperty(AttrType.MoveSpeed, this.configAttr.move_speed);
		base.InitProperty(AttrType.PhysicCritMag, this.configAttr.physic_crit_mag);
		base.InitProperty(AttrType.MagicCritMag, this.configAttr.magic_crit_mag);
		base.InitProperty(AttrType.ArmorCut, this.configAttr.armor_cut);
		base.InitProperty(AttrType.ArmorCut_Percentage, this.configAttr.armor_cut_percent);
		base.InitProperty(AttrType.MagicResistanceCut, this.configAttr.resistance_cut);
		base.InitProperty(AttrType.MagicResistanceCut_Percentage, this.configAttr.resistance_cut_percent);
		base.InitProperty(AttrType.AttackRange, this.configAttr.atk_range);
		base.InitProperty(AttrType.WarningRange, this.configAttr.warning_range);
		base.InitProperty(AttrType.FogRange, this.configAttr.player_range);
		base.InitProperty(AttrType.RealSightRange, 0f);
		base.InitProperty(AttrType.NormalSkillCooling, 0f);
		base.InitProperty(AttrType.SummonSkillCooling, 0f);
		base.InitProperty(AttrType.ItemSkillCooling, 0f);
		base.InitProperty(AttrType.DamageMultiple, 1f);
		base.InitProperty(AttrType.LifeSteal, 0f);
		this.self.sourceAnimSpeed = this.configAttr.atk_speed;
		this.self.sourceMoveSpeed = this.configAttr.move_speed;
	}

	private void SetL1AttrGrowth(int levelBeforeBattle)
	{
		base.InitProperty(AttrType.PowerGroth, this.configAttr.power_groth);
		base.InitProperty(AttrType.AgileGroth, this.configAttr.agile_groth);
		base.InitProperty(AttrType.IntelligenceGroth, this.configAttr.intelligence_groth);
	}

	public override void CollectGameItemAttr()
	{
		HeroInfoData heroInfoData = ModelManager.Instance.Get_heroInfo_item_byModelID_X(this.self.npc_id);
		if (heroInfoData == null)
		{
			return;
		}
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		Dictionary<AttrType, float> dictionary2 = new Dictionary<AttrType, float>();
		if (heroInfoData.Ep_1 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_1.ToString(), ref dictionary, ref dictionary2);
		}
		if (heroInfoData.Ep_2 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_2.ToString(), ref dictionary, ref dictionary2);
		}
		if (heroInfoData.Ep_3 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_3.ToString(), ref dictionary, ref dictionary2);
		}
		if (heroInfoData.Ep_4 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_4.ToString(), ref dictionary, ref dictionary2);
		}
		if (heroInfoData.Ep_5 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_5.ToString(), ref dictionary, ref dictionary2);
		}
		if (heroInfoData.Ep_6 > 0)
		{
			this.GetGameItemAttr(heroInfoData.Ep_6.ToString(), ref dictionary, ref dictionary2);
		}
		foreach (KeyValuePair<AttrType, float> current in dictionary)
		{
			base.SetAttr(DataManager.DataPoolType.AddDataBefore, current.Key, current.Value);
		}
		foreach (KeyValuePair<AttrType, float> current2 in dictionary2)
		{
			base.SetAttr(DataManager.DataPoolType.MulDataBefore, current2.Key, current2.Value);
		}
	}

	private void GetGameItemAttr(string equips, ref Dictionary<AttrType, float> addDict, ref Dictionary<AttrType, float> mulDict)
	{
		SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(equips);
		if (dataById == null)
		{
			return;
		}
		if (addDict == null)
		{
			addDict = new Dictionary<AttrType, float>();
		}
		if (mulDict == null)
		{
			mulDict = new Dictionary<AttrType, float>();
		}
		string[] stringValue = StringUtils.GetStringValue(dataById.attribute, ',');
		for (int i = 0; i < stringValue.Length; i++)
		{
			string[] array = stringValue[i].Split(new char[]
			{
				'|'
			});
			AttrType type = (AttrType)int.Parse(array[0]);
			if (!array[1].Contains("%"))
			{
				if (dataById.rune_type == 2)
				{
					HeroDataManager.AddToDict(ref addDict, type, float.Parse(array[1]) * (float)this.level);
				}
				else
				{
					HeroDataManager.AddToDict(ref addDict, type, float.Parse(array[1]));
				}
			}
			else
			{
				string text = array[1].Trim();
				float num = float.Parse(text.Substring(0, text.Length - 1)) / 100f;
				if (dataById.rune_type == 2)
				{
					num *= (float)this.level;
				}
				HeroDataManager.AddToDict(ref mulDict, type, num);
			}
		}
	}

	private void CollectEquip()
	{
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		Dictionary<AttrType, float> dictionary2 = new Dictionary<AttrType, float>();
		this.CollectEquipAttrs(this.self.npc_id, out dictionary, out dictionary2);
		foreach (KeyValuePair<AttrType, float> current in dictionary)
		{
			base.ChangeAttr(current.Key, OpType.Add, current.Value);
		}
		foreach (KeyValuePair<AttrType, float> current2 in dictionary2)
		{
			base.ChangeAttr(current2.Key, OpType.Mul, current2.Value);
		}
	}

	private void CollectTalent()
	{
		Dictionary<AttrType, float> dictionary = new Dictionary<AttrType, float>();
		Dictionary<AttrType, float> dictionary2 = new Dictionary<AttrType, float>();
		HeroDataManager.CellectTalentAttrs(out dictionary, out dictionary2);
		foreach (KeyValuePair<AttrType, float> current in dictionary)
		{
			base.ChangeAttr(current.Key, OpType.Add, current.Value);
		}
		foreach (KeyValuePair<AttrType, float> current2 in dictionary2)
		{
			base.ChangeAttr(current2.Key, OpType.Mul, current2.Value);
		}
	}

	public void addEquipInBattle()
	{
	}

	public override void OnStart()
	{
	}

	public override void OnUpdate(float deltaTime)
	{
		base.RestoreData(deltaTime);
	}

	public override void OnStop()
	{
	}

	public override void OnExit()
	{
	}

	public override void OnWound(Units attacker, float damage)
	{
	}

	private void InitDynamicData()
	{
		this.self.ChangeAttr(AttrType.Hp, OpType.Add, this.self.hp_max * this.self.hp_init_p);
		this.self.ChangeAttr(AttrType.Mp, OpType.Add, this.self.mp_max * this.self.mp_init_p);
		this.self.attackExtraDamage = 0f;
		this.self.attackMultipleDamage = 0f;
		this.self.beheadedCoefficient = 0f;
		this.self.attackReboundCoefficient = 0f;
		this.self.a_growthFactor = 1f;
		this.self.attackForTargetBuff = string.Empty;
		List<string> heroItemsString = BattleEquipTools_op.GetHeroItemsString(this.self);
		Dictionary<AttrType, float> dictionary;
		Dictionary<AttrType, float> dictionary2;
		BattleEquipTools_config.GetItemsAttri(heroItemsString, out dictionary, out dictionary2);
		foreach (KeyValuePair<AttrType, float> current in dictionary)
		{
			base.ChangeAttr(current.Key, OpType.Add, current.Value);
		}
		foreach (KeyValuePair<AttrType, float> current2 in dictionary2)
		{
			base.ChangeAttr(current2.Key, OpType.Mul, current2.Value);
		}
	}

	public void CollectEquipAttrs(string heroId, out Dictionary<AttrType, float> equipAdd, out Dictionary<AttrType, float> equipMul)
	{
		equipAdd = new Dictionary<AttrType, float>();
		equipMul = new Dictionary<AttrType, float>();
		if (NetWorkHelper.Instance == null)
		{
			return;
		}
		if (ModelManager.Instance.Get_heroInfo_item_byModelID_X(heroId) == null)
		{
			return;
		}
	}

	private void CollectEquipAttr(string equips, int magicStar, ref Dictionary<AttrType, float> addDict, ref Dictionary<AttrType, float> mulDict)
	{
		if (addDict == null || addDict.Keys.Count == 0)
		{
			addDict = new Dictionary<AttrType, float>();
		}
		if (mulDict == null || mulDict.Keys.Count == 0)
		{
			mulDict = new Dictionary<AttrType, float>();
		}
		string[] stringValue = StringUtils.GetStringValue(equips, ',');
		for (int i = 0; i < stringValue.Length; i++)
		{
			SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(stringValue[i]);
			if (dataById == null)
			{
				Debug.LogError("HeroAttr.CollectEquipAttr , equips [" + equips + "] has null id: " + stringValue[i]);
			}
			else
			{
				string[] stringValue2 = StringUtils.GetStringValue(dataById.attribute, ',');
				for (int j = 0; j < stringValue2.Length; j++)
				{
					string[] array = stringValue2[j].Split(new char[]
					{
						'|'
					});
					AttrType type = (AttrType)int.Parse(array[0]);
					if (!array[1].Contains("%"))
					{
						HeroDataManager.AddToDict(ref addDict, type, float.Parse(array[1]));
					}
					else
					{
						string text = array[1].Trim();
						float value = float.Parse(text.Substring(0, text.Length - 1)) / 100f;
						HeroDataManager.AddToDict(ref mulDict, type, value);
					}
				}
				if (magicStar > 0)
				{
					string[] stringValue3 = StringUtils.GetStringValue(dataById.enchant_attribute, ',');
					for (int k = 0; k < stringValue3.Length; k++)
					{
						string[] array2 = stringValue3[k].Split(new char[]
						{
							'|'
						});
						AttrType type2 = (AttrType)int.Parse(array2[0]);
						if (!array2[1].Contains("%"))
						{
							HeroDataManager.AddToDict(ref addDict, type2, float.Parse(array2[1]) * (float)magicStar);
						}
						else
						{
							string text2 = array2[1].Trim();
							float num = float.Parse(text2.Substring(0, text2.Length - 1)) / 100f;
							HeroDataManager.AddToDict(ref mulDict, type2, num * (float)magicStar);
						}
					}
				}
			}
		}
	}

	public static void CellectTalentAttrs(out Dictionary<AttrType, float> talentAdd, out Dictionary<AttrType, float> talentMul)
	{
		talentAdd = new Dictionary<AttrType, float>();
		talentMul = new Dictionary<AttrType, float>();
		if (NetWorkHelper.Instance == null)
		{
			return;
		}
		TalentDataParse.Parse(ref talentAdd, ref talentMul);
	}

	private static void AddToDict(ref Dictionary<AttrType, float> dict, AttrType type, float value)
	{
		if (dict == null)
		{
			return;
		}
		if (dict.ContainsKey(type))
		{
			dict[type] += value;
		}
		else
		{
			dict.Add(type, value);
		}
	}

	public override void OnLevelUpTo(int _level)
	{
		base.OnLevelUpTo(_level);
	}
}
