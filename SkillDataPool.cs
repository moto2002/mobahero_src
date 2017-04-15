using Com.Game.Data;
using Com.Game.Manager;
using MobaFrame.SkillAction;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataPool : BaseGameModule
{
	private readonly Dictionary<SkillDataKey, SkillData> mDataPool = new Dictionary<SkillDataKey, SkillData>(new SkillDataKey.EqualityComparer());

	public override void Init()
	{
		this.mDataPool.Clear();
	}

	public override void Uninit()
	{
		this.mDataPool.Clear();
	}

	public void InsertData(string skillID, int skin)
	{
		SysSkillMainVo skillMainData = BaseDataMgr.instance.GetSkillMainData(skillID);
		if (skillMainData != null)
		{
			int skill_levelmax = skillMainData.skill_levelmax;
			if (skill_levelmax <= 0)
			{
				this.DoInsert(skillID, 0, skin);
			}
			else
			{
				for (int i = 1; i <= skill_levelmax; i++)
				{
					this.DoInsert(skillID, i, skin);
				}
			}
		}
		else
		{
			Debug.LogError("没有找到这个技能 error id=" + skillID + " 请检查技能升级表");
		}
	}

	private void DoInsert(string skillID, int level, int skin)
	{
		SkillDataKey key = new SkillDataKey(skillID, level, skin);
		if (!this.mDataPool.ContainsKey(key))
		{
			this.mDataPool.Add(key, new SkillData(skillID, level, skin));
		}
	}

	public SkillData GetData(string skillID, int level = 0, int skin = 0)
	{
		SkillDataKey skillKey = new SkillDataKey(skillID, level, skin);
		return this.GetData(skillKey);
	}

	public SkillData GetData(SkillDataKey skillKey)
	{
		SkillData result;
		if (this.mDataPool.TryGetValue(skillKey, out result))
		{
			return result;
		}
		this.InsertData(skillKey.SkillID, skillKey.Skin);
		if (this.mDataPool.TryGetValue(skillKey, out result))
		{
			return result;
		}
		return null;
	}

	public void InsertMonsterSkillData(ref SysMonsterMainVo monsterMain)
	{
		if (monsterMain == null)
		{
			Debug.LogError("InsertMonsterSkillData Error!!");
			return;
		}
		string[] stringValue = StringUtils.GetStringValue(monsterMain.attack_id, ',');
		if (!ArrayTool.isNullOrEmpty(stringValue))
		{
			string[] array = stringValue;
			for (int i = 0; i < array.Length; i++)
			{
				string skillID = array[i];
				this.InsertData(skillID, 0);
			}
		}
		string[] stringValue2 = StringUtils.GetStringValue(monsterMain.skill_id, ',');
		if (!ArrayTool.isNullOrEmpty(stringValue2))
		{
			string[] array2 = stringValue2;
			for (int j = 0; j < array2.Length; j++)
			{
				string skillID2 = array2[j];
				this.InsertData(skillID2, 0);
			}
		}
	}

	public void InsertSkillUnitSkillData(ref SysSkillUnitVo skillUnitVo)
	{
		if (skillUnitVo == null)
		{
			Debug.LogError("InsertSkillUnitSkillData Error!!!");
			return;
		}
		string[] stringValue = StringUtils.GetStringValue(skillUnitVo.skill_id, ',');
		if (!ArrayTool.isNullOrEmpty(stringValue))
		{
			string[] array = stringValue;
			for (int i = 0; i < array.Length; i++)
			{
				string skillID = array[i];
				this.InsertData(skillID, 0);
			}
		}
	}

	public void InsertEntitySkillData(EntityVo entityVo)
	{
		EntityType entity_type = entityVo.entity_type;
		if (entity_type == EntityType.Hero)
		{
			int skin = (entityVo.skin <= 0) ? 0 : entityVo.skin;
			SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(entityVo.npc_id);
			if (heroMainData != null)
			{
				string[] stringValue = StringUtils.GetStringValue(heroMainData.attack_id, ',');
				if (!ArrayTool.isNullOrEmpty(stringValue))
				{
					string[] array = stringValue;
					for (int i = 0; i < array.Length; i++)
					{
						string skillID = array[i];
						this.InsertData(skillID, skin);
					}
				}
				string[] stringValue2 = StringUtils.GetStringValue(heroMainData.skill_id, ',');
				if (!ArrayTool.isNullOrEmpty(stringValue2))
				{
					string[] array2 = stringValue2;
					for (int j = 0; j < array2.Length; j++)
					{
						string skillID2 = array2[j];
						this.InsertData(skillID2, skin);
					}
				}
			}
			else
			{
				Debug.LogError("Hero [" + entityVo.npc_id + "] is wrong!!");
			}
		}
	}
}
