using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Module;
using Com.Game.Utils;
using MobaFrame.SkillAction;
using MobaHeros;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class SkillManager : UnitComponent
{
	public Dictionary<int, List<SkillDataKey>> all_skills_index = new Dictionary<int, List<SkillDataKey>>();

	public Dictionary<int, List<SkillDataKey>> all_attacks_index = new Dictionary<int, List<SkillDataKey>>();

	public Dictionary<int, string> skills_index = new Dictionary<int, string>();

	public Dictionary<int, string> attacks_index = new Dictionary<int, string>();

	private Dictionary<string, Skill> skills = new Dictionary<string, Skill>();

	private Dictionary<string, Skill> attacks = new Dictionary<string, Skill>();

	private List<string> _unlockedSkills = new List<string>();

	private List<string> ItemSkills = new List<string>();

	private string personalBackHomeSkilltId = string.Empty;

	private string personalEyeSkillId = string.Empty;

	private List<BornPownerObjSkillData> _bornPownerObjSkillDatas = new List<BornPownerObjSkillData>();

	private int _skillPointsLeft;

	public bool isFirstInited;

	private List<string> m_show_skills;

	private List<string> m_show_attacks;

	private List<string> m_passive_skill;

	private List<SkillDataKey> skillKeys;

	private Skill skill;

	private Skill tempSkill;

	public int SkillPointsLeft
	{
		get
		{
			return this._skillPointsLeft;
		}
		set
		{
			this._skillPointsLeft = value;
		}
	}

	public List<string> ShowSkills
	{
		get
		{
			if (this.skills_index != null)
			{
				from o in this.skills_index
				orderby o.Key
				select o;
				this.m_show_skills = this.skills_index.Values.ToList<string>();
			}
			return this.m_show_skills;
		}
	}

	public List<string> ShowAttacks
	{
		get
		{
			if (this.attacks_index != null && this.m_show_attacks == null)
			{
				from o in this.attacks_index
				orderby o.Key
				select o;
				this.m_show_attacks = this.attacks_index.Values.ToList<string>();
			}
			return this.m_show_attacks;
		}
	}

	public List<string> PassiveSkill
	{
		get
		{
			if (this.ShowSkills != null && this.ShowSkills.Count > 0 && this.m_passive_skill == null)
			{
				this.m_passive_skill = new List<string>();
				for (int i = 0; i < this.ShowSkills.Count; i++)
				{
					string text = this.ShowSkills[i];
					if (!this.m_passive_skill.Contains(text))
					{
						Skill skill = this.skills[text];
						if (skill.IsPassive)
						{
							this.m_passive_skill.Add(text);
						}
					}
				}
			}
			return this.m_passive_skill;
		}
	}

	public SkillManager()
	{
	}

	public SkillManager(Units self) : base(self)
	{
	}

	public override void OnInit()
	{
		if (!this.isFirstInited)
		{
			this.AssignPersonalData();
			this.ClearAll();
			this.InitSkill();
			this.isFirstInited = true;
		}
		if (!this.self.isHero)
		{
			this.donotUpdateByMonster = true;
		}
	}

	private void AssignPersonalData()
	{
		if (this.self is Hero)
		{
			Hero hero = this.self as Hero;
			if (hero.heroData == null)
			{
				return;
			}
			if (hero.heroData.backEffectId != 0)
			{
				SysGameItemsVo dataById = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.backEffectId.ToString());
				if (dataById != null)
				{
					this.personalBackHomeSkilltId = dataById.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制道具没找到 error id=" + hero.heroData.backEffectId);
				}
			}
			if (hero.heroData.eyeUnitSkinId != 0)
			{
				SysGameItemsVo dataById2 = BaseDataMgr.instance.GetDataById<SysGameItemsVo>(hero.heroData.eyeUnitSkinId.ToString());
				if (dataById2 != null)
				{
					this.personalEyeSkillId = dataById2.hero_decorate_param;
				}
				else
				{
					ClientLogger.Error("私人定制道具没找到 error id=" + hero.heroData.eyeUnitSkinId);
				}
			}
		}
	}

	public override void OnStart()
	{
		if (Singleton<PvpManager>.Instance.IsInPvp)
		{
			this.EnableSkills(false);
		}
		else if (this.self.StartCount == 0 && LevelManager.CurBattleType == 6)
		{
			GlobalObject.Instance.StartCoroutine(this.FirstStartPassive());
		}
		else
		{
			this.EnableSkills(false);
		}
	}

	[DebuggerHidden]
	private IEnumerator FirstStartPassive()
	{
		SkillManager.<FirstStartPassive>c__Iterator43 <FirstStartPassive>c__Iterator = new SkillManager.<FirstStartPassive>c__Iterator43();
		<FirstStartPassive>c__Iterator.<>f__this = this;
		return <FirstStartPassive>c__Iterator;
	}

	public override void OnUpdate(float deltaTime)
	{
		Dictionary<string, Skill>.Enumerator enumerator = this.skills.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Skill> current = enumerator.Current;
			current.Value.OnUpdate(deltaTime);
		}
	}

	public override void OnStop()
	{
	}

	public override void OnDeath(Units attacker)
	{
		this.StopSkill();
		this.TryClearBornPowerObjSkillInfoOnDead();
	}

	public override void OnExit()
	{
		this.ClearAll();
	}

	private void InitSkill()
	{
		string[] data = this.self.GetData<string[]>(DataType.Attacks);
		if (data != null)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (StringUtils.CheckValid(data[i]))
				{
					Skill skill;
					if (data[i].StartsWith("Attack_Jinkesi"))
					{
						skill = new Attack_Jinkesi(data[i], this.self);
					}
					if (data[i].StartsWith("Attack_Tulun"))
					{
						skill = new Attack_Tulun(data[i], this.self);
					}
					else
					{
						skill = new Skill(data[i], this.self);
					}
					this.AddAttack(skill);
				}
			}
		}
		string[] data2 = this.self.GetData<string[]>(DataType.Skills);
		if (data2 != null)
		{
			for (int j = 0; j < data2.Length; j++)
			{
				if (StringUtils.CheckValid(data2[j]))
				{
					if (!this.isSkillLock(j))
					{
						if (!this._unlockedSkills.Contains(data2[j]))
						{
							this._unlockedSkills.Add(data2[j]);
						}
						Skill skill2;
						if (data2[j] == "Skill_Lanmao_04")
						{
							skill2 = new Skill_Lanmao_04(data2[j], this.self);
						}
						else if (data2[j] == "Permanent_VisionWard")
						{
							skill2 = new Skill_Forward(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Jiuwei_04")
						{
							skill2 = new Skill_Jiuwei_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tunvlang_01")
						{
							skill2 = new Skill_Tunvlang_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tunvlang_04")
						{
							skill2 = new Skill_Tunvlang_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Timo_04")
						{
							skill2 = new Skill_Timo_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_AKL_04")
						{
							skill2 = new Skill_Akali_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Houzi_02")
						{
							skill2 = new Skill_Houzi_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Yingmo_01")
						{
							skill2 = new Skill_Yingmo_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Yingmo_02")
						{
							skill2 = new Skill_Yingmo_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Yingmo_03")
						{
							skill2 = new Skill_Yingmo_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Xiaohei_01")
						{
							skill2 = new Skill_Xiaohei_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Guangmingshen_03")
						{
							skill2 = new Skill_GuangMingShen_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Xiaolu_03")
						{
							skill2 = new Skill_Xiaolu_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Xiaoxiao_02")
						{
							skill2 = new Skill_Xiaoxiao_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Liaonida_01")
						{
							skill2 = new Skill_Liaonida_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Liaonida_03")
						{
							skill2 = new Skill_Liaonida_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Baoliezuolun_01")
						{
							skill2 = new Skill_Baoliezuolun_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Jinkesi_01")
						{
							skill2 = new Skill_Jinkesi_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Jiansheng_04")
						{
							skill2 = new Skill_Jiansheng_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Aier_01")
						{
							skill2 = new Skill_Yasuo_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Aier_01_2")
						{
							skill2 = new Skill_Yasuo_01_2(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Aier_02")
						{
							skill2 = new Skill_Yasuo_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Aier_03")
						{
							skill2 = new Skill_Yasuo_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Aier_04")
						{
							skill2 = new Skill_Yasuo_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Emowushi_03")
						{
							skill2 = new Skill_Emowushi_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_CreateGold")
						{
							skill2 = new Skill_Guowang_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_CreateTreasureChest")
						{
							skill2 = new Skill_Guowang_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tufu_01")
						{
							skill2 = new Skill_Tufu_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tufu_02")
						{
							skill2 = new Skill_Tufu_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tufu_03")
						{
							skill2 = new Skill_Tufu_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tufu_04")
						{
							skill2 = new Skill_Tufu_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Luna_03")
						{
							skill2 = new Skill_Luna_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tulun_01")
						{
							skill2 = new Skill_Tulun_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tulun_02")
						{
							skill2 = new Skill_Tulun_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tulun_03")
						{
							skill2 = new Skill_Tulun_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tulun_04")
						{
							skill2 = new Skill_Tulun_04(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Tulun_02_1")
						{
							skill2 = new Skill_Tulun_02_1(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Huimiezhe_01")
						{
							skill2 = new Skill_Huimiezhe_01(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Huimiezhe_02")
						{
							skill2 = new Skill_Huimiezhe_02(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Huimiezhe_03")
						{
							skill2 = new Skill_Huimiezhe_03(data2[j], this.self);
						}
						else if (data2[j] == "Skill_Huimiezhe_04")
						{
							skill2 = new Skill_Huimiezhe_04(data2[j], this.self);
						}
						else
						{
							skill2 = new Skill(data2[j], this.self);
						}
						this.AddSkill(skill2);
						skill2.OnCreate();
					}
				}
				else
				{
					ClientLogger.Error(this.self.name + ".Skill has invalid skills: Id[" + data2[0] + "]");
				}
			}
			if (LevelManager.Instance.IsPvpBattleType || LevelManager.Instance.IsZyBattleType || LevelManager.CurBattleType == 2)
			{
				string summonerSkillId = Singleton<PvpManager>.Instance.GetSummonerSkillId(this.self.unique_id);
				if (!string.IsNullOrEmpty(summonerSkillId))
				{
					this.AddExtraSkill(summonerSkillId, -1, false);
				}
			}
			this.AddExtraSkill(string.Empty, -1, false);
		}
	}

	public void SetItemSkill(List<ItemDynData> ItemList)
	{
	}

	private void CleanItemSkill()
	{
		if (this.ItemSkills != null)
		{
			for (int i = 0; i < this.ItemSkills.Count; i++)
			{
				if (this.skills.ContainsKey(this.ItemSkills[i]))
				{
					this.skills[this.ItemSkills[i]].RemoveHighEffAndBuff();
					this.RemoveSkill(this.skills[this.ItemSkills[i]]);
				}
				if (this._unlockedSkills.Contains(this.ItemSkills[i]))
				{
					this._unlockedSkills.Remove(this.ItemSkills[i]);
				}
			}
		}
		this.ItemSkills.Clear();
	}

	private void AddExtraSkill(string skillId = "", int skillLv = -1, bool isTalentSkill = false)
	{
		if (skillId == string.Empty)
		{
			SysBattleSceneVo dataById = BaseDataMgr.instance.GetDataById<SysBattleSceneVo>(LevelManager.CurLevelId);
			string backHomeSkill = dataById.backHomeSkill;
			string permanent_treatment_skill = dataById.permanent_treatment_skill;
			if (backHomeSkill != string.Empty && backHomeSkill != "[]" && this.personalBackHomeSkilltId != string.Empty)
			{
				backHomeSkill = this.personalBackHomeSkilltId;
			}
			if (this.personalEyeSkillId != string.Empty)
			{
				permanent_treatment_skill.Replace("Permanent_VisionWard", this.personalEyeSkillId);
			}
			if (dataById != null && backHomeSkill != string.Empty && backHomeSkill != "[]")
			{
				if (!this._unlockedSkills.Contains(backHomeSkill))
				{
					this._unlockedSkills.Add(backHomeSkill);
				}
				Skill skill = new Skill(backHomeSkill, this.self);
				skill.isTalentSkill = isTalentSkill;
				skill.SetLevel(skillLv);
				this.AddExtraSkill(skill, this.skills_index.Count);
			}
			if (dataById != null && permanent_treatment_skill != string.Empty && permanent_treatment_skill != "[]")
			{
				string[] array = StringUtils.SplitVoString(permanent_treatment_skill, ",");
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!this._unlockedSkills.Contains(array[i]))
						{
							this._unlockedSkills.Add(array[i]);
						}
						Skill skill2 = new Skill(array[i], this.self);
						skill2.isTalentSkill = isTalentSkill;
						skill2.SetLevel(skillLv);
						this.AddExtraSkill(skill2, this.skills_index.Count);
					}
				}
			}
		}
		else
		{
			if (!this._unlockedSkills.Contains(skillId))
			{
				this._unlockedSkills.Add(skillId);
			}
			Skill skill3 = new Skill(skillId, this.self);
			skill3.isTalentSkill = isTalentSkill;
			skill3.SetLevel(skillLv);
			this.AddExtraSkill(skill3, this.skills_index.Count);
		}
	}

	public virtual void EnableSkills(bool MustEnable = false)
	{
		Dictionary<string, Skill>.Enumerator enumerator = this.skills.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Skill> current = enumerator.Current;
			Skill value = current.Value;
			if (this.isSkillEnabled(value))
			{
				if (value.IsSkill)
				{
					value.DoSkillLevelUp();
				}
			}
		}
	}

	protected virtual bool isSkillEnabled(Skill skill)
	{
		return skill != null && BattleAttrManager.Instance.IsCurSkillUnlock(skill.skillIndex, this.self);
	}

	private void StopSkill()
	{
		Dictionary<string, Skill>.Enumerator enumerator = this.attacks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Skill> current = enumerator.Current;
			Skill value = current.Value;
			if (value != null)
			{
				value.End();
			}
		}
		Dictionary<string, Skill>.Enumerator enumerator2 = this.skills.GetEnumerator();
		while (enumerator2.MoveNext())
		{
			KeyValuePair<string, Skill> current2 = enumerator2.Current;
			Skill value2 = current2.Value;
			if (value2 != null)
			{
				value2.End();
			}
		}
		this.DestroyActions();
	}

	private void AddSkill(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		int skillIndex = skill.skillIndex;
		if (skillIndex < 0)
		{
			return;
		}
		string skillMainId = skill.skillMainId;
		if (!this.skills.ContainsKey(skillMainId))
		{
			this.skills.Add(skillMainId, skill);
			if (!this.all_skills_index.ContainsKey(skillIndex))
			{
				this.all_skills_index.Add(skillIndex, new List<SkillDataKey>());
				this.skills_index.Add(skillIndex, skillMainId);
			}
			int level = (!SkillUtility.IsSkillCanLevelUp(skillMainId)) ? 0 : 1;
			this.all_skills_index[skillIndex].Add(new SkillDataKey(skillMainId, level, 0));
		}
	}

	public string toString()
	{
		return null;
	}

	private void RemoveSkill(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		int skillIndex = skill.skillIndex;
		if (skillIndex < 0)
		{
			return;
		}
		string skillMainId = skill.skillMainId;
		if (this.skills.ContainsKey(skillMainId))
		{
			this.skills.Remove(skillMainId);
			if (this.all_skills_index.ContainsKey(skillIndex) && skillIndex > 7)
			{
				this.all_skills_index.Remove(skillIndex);
				this.skills_index.Remove(skillIndex);
			}
		}
	}

	private void AddExtraSkill(Skill skill, int index)
	{
		if (skill == null)
		{
			return;
		}
		if (index < 0)
		{
			return;
		}
		string skillMainId = skill.skillMainId;
		if (!this.skills.ContainsKey(skillMainId))
		{
			this.skills.Add(skillMainId, skill);
			if (!this.all_skills_index.ContainsKey(index))
			{
				this.all_skills_index.Add(index, new List<SkillDataKey>());
				this.skills_index.Add(index, skillMainId);
			}
			int level = (!SkillUtility.IsSkillCanLevelUp(skillMainId)) ? 0 : 1;
			this.all_skills_index[index].Add(new SkillDataKey(skillMainId, level, 0));
		}
	}

	private void AddAttack(Skill skill)
	{
		if (skill == null)
		{
			return;
		}
		int skillIndex = skill.skillIndex;
		if (skillIndex < 0)
		{
			return;
		}
		string skillMainId = skill.skillMainId;
		if (!this.attacks.ContainsKey(skillMainId))
		{
			this.attacks.Add(skillMainId, skill);
			if (!this.all_attacks_index.ContainsKey(skillIndex))
			{
				this.all_attacks_index.Add(skillIndex, new List<SkillDataKey>());
				this.attacks_index.Add(skillIndex, skillMainId);
			}
			int level = (!SkillUtility.IsSkillCanLevelUp(skillMainId)) ? 0 : 1;
			this.all_attacks_index[skillIndex].Add(new SkillDataKey(skillMainId, level, 0));
		}
	}

	private void ClearAll()
	{
		foreach (KeyValuePair<string, Skill> current in this.attacks)
		{
			if (current.Value != null)
			{
				current.Value.OnExit();
			}
		}
		this.attacks.Clear();
		foreach (KeyValuePair<string, Skill> current2 in this.skills)
		{
			if (current2.Value != null)
			{
				current2.Value.OnExit();
			}
		}
		this.skills.Clear();
		this.attacks_index.Clear();
		this.skills_index.Clear();
		this.all_attacks_index.Clear();
		this.all_skills_index.Clear();
		this.m_show_attacks = null;
		this.m_show_skills = null;
		this.m_passive_skill = null;
	}

	public void ReplaceSkill(string sourceId, string targetId)
	{
		if (this.skills.Count == 0)
		{
			return;
		}
		if (!this.skills.ContainsKey(sourceId))
		{
			if (!this.skills.ContainsKey(targetId))
			{
				UnityEngine.Debug.LogError(" ReplaceSkill Error : sourceId = " + sourceId + " Cannot Find!!");
			}
			return;
		}
		if (this.skills.ContainsKey(sourceId))
		{
			int key = this.IndexOfSkill(sourceId);
			if (this.skills_index.ContainsKey(key))
			{
				this.skills_index[key] = targetId;
			}
		}
	}

	public void ReplaceAttack(string sourceId, string targetId)
	{
		if (this.skills.Count == 0)
		{
			return;
		}
		if (!this.attacks.ContainsKey(sourceId))
		{
			if (this.attacks.ContainsKey(targetId))
			{
			}
			return;
		}
		if (this.attacks.ContainsKey(sourceId))
		{
			int key = this.IndexOfAttack(sourceId);
			if (this.attacks_index.ContainsKey(key))
			{
				this.attacks_index[key] = targetId;
			}
		}
	}

	public List<string> GetSkills()
	{
		return this.ShowSkills;
	}

	public List<string> GetAttacks()
	{
		return this.ShowAttacks;
	}

	public List<string> GetUnlockSkills()
	{
		return this._unlockedSkills;
	}

	public List<string> GetPassiveSkills()
	{
		return this.PassiveSkill;
	}

	public bool IsSkillUnlock(string skillID)
	{
		bool result = false;
		if (!string.IsNullOrEmpty(skillID) && this.skills.ContainsKey(skillID))
		{
			Skill skill = this.skills[skillID];
			int skillLevelMax = SkillUtility.GetSkillLevelMax(skillID);
			result = (skillLevelMax <= 0 || (skill != null && skill.skillLevel > 0));
		}
		return result;
	}

	public bool IsSkillUnlockByIndex(int inIndex)
	{
		if (this.skills_index.ContainsKey(inIndex))
		{
			string skillID = this.skills_index[inIndex];
			return this.IsSkillUnlock(skillID);
		}
		return false;
	}

	public void SetSkillLevel(string skillID, int level)
	{
		if (this.skills.ContainsKey(skillID))
		{
			Skill skill = this.skills[skillID];
			if (skill.SetLevel(level) && skill.IsNormalSkill)
			{
				List<SkillDataKey> skillsByIndex = this.getSkillsByIndex(skill.skillIndex);
				if (skillsByIndex != null)
				{
					for (int i = 0; i < skillsByIndex.Count; i++)
					{
						skillsByIndex[i] = new SkillDataKey(skillsByIndex[i].SkillID, level, skillsByIndex[i].Skin);
						Skill skillById = this.getSkillById(skillsByIndex[i].SkillID);
						if (skillById != null)
						{
							skillById.SetLevel(level);
						}
					}
				}
			}
		}
	}

	public void UpgradeSkillLevel(string skillID, int level)
	{
		if (this.skills.ContainsKey(skillID))
		{
			this.skill = this.skills[skillID];
			if (this.skill.SetLevel(level))
			{
				this.skill.DoSkillLevelUp();
				if (this.skill.IsNormalSkill)
				{
					this.skillKeys = this.getSkillsByIndex(this.skill.skillIndex);
					if (this.skillKeys != null)
					{
						for (int i = 0; i < this.skillKeys.Count; i++)
						{
							this.skillKeys[i] = new SkillDataKey(this.skillKeys[i].SkillID, level, this.skillKeys[i].Skin);
							this.tempSkill = this.getSkillById(this.skillKeys[i].SkillID);
							if (this.tempSkill != null)
							{
								this.tempSkill.SetLevel(level);
							}
						}
					}
				}
			}
		}
	}

	public void ResetSkillSubIndex(string skillID, int idx)
	{
		if (this.skills.ContainsKey(skillID))
		{
			this.skill = this.skills[skillID];
			this.skill.skillSubIdx = idx;
			this.skill.RefreshSkillIcon();
		}
	}

	public void UpgradeAllSkillLevel(int level)
	{
		foreach (KeyValuePair<string, Skill> current in this.skills)
		{
			this.UpgradeSkillLevel(current.Key, level);
		}
	}

	public int GetSkillLevel(string skillID)
	{
		int result = -1;
		if (this.skills.ContainsKey(skillID))
		{
			Skill skill = this.skills[skillID];
			result = skill.skillLevel;
		}
		return result;
	}

	public void UpgradeSkillLevel(string skillID)
	{
		int skillLevel = this.GetSkillLevel(skillID);
		this.UpgradeSkillLevel(skillID, skillLevel + 1);
	}

	public string[] GetSkillIDs()
	{
		return this.skills.Keys.ToArray<string>();
	}

	public int GetAttackTotalNum()
	{
		return this.attacks.Count;
	}

	public Skill getSkillById(string skillId)
	{
		Skill result;
		if (skillId != null && this.skills.TryGetValue(skillId, out result))
		{
			return result;
		}
		return null;
	}

	public Skill getAttackById(string attackId)
	{
		Skill result;
		if (attackId != null && this.attacks.TryGetValue(attackId, out result))
		{
			return result;
		}
		return null;
	}

	public Skill getSkillByIndex(int index)
	{
		if (this.skills_index.ContainsKey(index))
		{
			string key = this.skills_index[index];
			Skill result;
			this.skills.TryGetValue(key, out result);
			return result;
		}
		return null;
	}

	public Skill getAttackByIndex(int index)
	{
		if (this.attacks_index.ContainsKey(index))
		{
			string key = this.attacks_index[index];
			return this.attacks[key];
		}
		return null;
	}

	public string getSkillIdByIndex(int index)
	{
		if (this.skills_index.ContainsKey(index))
		{
			return this.skills_index[index];
		}
		return null;
	}

	public int IndexOfSkill(string skill_id)
	{
		if (this.skills.ContainsKey(skill_id))
		{
			return this.skills[skill_id].skillIndex;
		}
		return -1;
	}

	public int IndexOfAttack(string skill_id)
	{
		if (this.attacks.ContainsKey(skill_id))
		{
			return this.attacks[skill_id].skillIndex;
		}
		return -1;
	}

	public List<SkillDataKey> getSkillsByIndex(int index)
	{
		if (this.all_skills_index.ContainsKey(index))
		{
			return this.all_skills_index[index];
		}
		return null;
	}

	public int GetSkillPropority(int index)
	{
		return 0;
	}

	public float GetCDSpeed(string skill_name)
	{
		Skill skillById = this.getSkillById(skill_name);
		if (skillById == null)
		{
			return 10000f;
		}
		if (skillById.CD > 0f)
		{
			return 1f / skillById.CD;
		}
		return 10000f;
	}

	public float GetSkillCDTime(string skill_name)
	{
		Skill skillById = this.getSkillById(skill_name);
		if (skillById != null)
		{
			return skillById.CD;
		}
		return 0f;
	}

	public float GetPublicCDTime(string skill_name)
	{
		Skill skillById = this.getSkillById(skill_name);
		if (skillById != null)
		{
			return skillById.public_cd;
		}
		return 0f;
	}

	public float GetPublicEachMagic(string skill_name)
	{
		Skill skillById = this.getSkillById(skill_name);
		if (skillById != null)
		{
			return skillById.GetCostValue(AttrType.Mp);
		}
		return 0f;
	}

	public bool isSkillLock(int skill_index)
	{
		return false;
	}

	public void DestroyActions()
	{
		foreach (Skill current in this.skills.Values)
		{
			current.DestroyActions(SkillCastPhase.Cast_None);
		}
		foreach (Skill current2 in this.attacks.Values)
		{
			current2.DestroyActions(SkillCastPhase.Cast_None);
		}
	}

	private BaseAction GetActionInSkill(string inSkillId, SkillCastPhase inSkillCastPhase, int inActionId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return null;
		}
		Skill skillById = this.getSkillById(inSkillId);
		if (skillById == null)
		{
			return null;
		}
		return skillById.GetAction(inSkillCastPhase, inActionId);
	}

	public void TryAddBornPowerObjSkillData(string inSkillId, int inActionId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return;
		}
		Skill skillById = this.getSkillById(inSkillId);
		if (skillById == null)
		{
			return;
		}
		if (skillById.Data.config.skill_logic_type != 17)
		{
			return;
		}
		skillById.OnBornPowerObj();
		BornPownerObjSkillData bornPownerObjSkillData = this.GetBornPownerObjSkillData(inSkillId);
		if (bornPownerObjSkillData == null)
		{
			bornPownerObjSkillData = new BornPownerObjSkillData(inSkillId);
			this._bornPownerObjSkillDatas.Add(bornPownerObjSkillData);
		}
		bornPownerObjSkillData.AddDamageActionId(inActionId);
	}

	private bool IsSkillHaveBornPowerObj(string inSkillId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return false;
		}
		Skill skillById = this.getSkillById(inSkillId);
		return skillById != null && skillById.Data.config.skill_logic_type == 17;
	}

	private bool IsCanRemoveBornPowerObjSkillDataOnInterrupt(string inSkillId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return true;
		}
		Skill skillById = this.getSkillById(inSkillId);
		return skillById == null || skillById.Data.config.skill_logic_type == 17;
	}

	public void ClearBornPowerObjSkillData()
	{
		this._bornPownerObjSkillDatas.Clear();
	}

	public void TryRemoveBornPowerObjSkillDataOnInterrupt(string inSkillId)
	{
		if (!this.IsCanRemoveBornPowerObjSkillDataOnInterrupt(inSkillId))
		{
			return;
		}
		this.RemoveBornPowerObjSkillData(inSkillId);
	}

	public void TryClearBornPowerObjSkillInfoOnDead()
	{
		this._bornPownerObjSkillDatas.Clear();
		Dictionary<string, Skill>.Enumerator enumerator = this.skills.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, Skill> current = enumerator.Current;
			Skill value = current.Value;
			if (value != null && value.Data.config.skill_logic_type == 17)
			{
				value.OnBornPowerObjTriggered();
				if (this.self.isPlayer)
				{
					Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(value.skillMainId);
				}
			}
		}
	}

	public void OnSkillSynced(string inSkillId, byte inUseState)
	{
		if (string.IsNullOrEmpty(inSkillId))
		{
			return;
		}
		Skill skillById = this.getSkillById(inSkillId);
		if (skillById != null)
		{
			skillById.OnSynced(inUseState);
		}
	}

	private void RemoveBornPowerObjSkillData(string inSkillId)
	{
		if (this._bornPownerObjSkillDatas.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < this._bornPownerObjSkillDatas.Count; i++)
		{
			if (this._bornPownerObjSkillDatas[i] != null)
			{
				if (this._bornPownerObjSkillDatas[i].SkillId == inSkillId)
				{
					this._bornPownerObjSkillDatas.RemoveAt(i);
					return;
				}
			}
		}
	}

	private BornPownerObjSkillData GetBornPownerObjSkillData(string inSkillId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return null;
		}
		if (this._bornPownerObjSkillDatas.Count <= 0)
		{
			return null;
		}
		for (int i = 0; i < this._bornPownerObjSkillDatas.Count; i++)
		{
			if (this._bornPownerObjSkillDatas[i] != null)
			{
				if (this._bornPownerObjSkillDatas[i].SkillId == inSkillId)
				{
					return this._bornPownerObjSkillDatas[i];
				}
			}
		}
		return null;
	}

	public bool IsSkillCanTriggerBornPowerObj(string inSkillId)
	{
		if (!StringUtils.CheckValid(inSkillId))
		{
			return false;
		}
		Skill skillById = this.getSkillById(inSkillId);
		return skillById != null && skillById.IsCanTriggerBornPowerObj();
	}

	public void TriggerBornPowerObj(string inSkillId, bool inIsMainPlayer)
	{
		BornPownerObjSkillData bornPownerObjSkillData = this.GetBornPownerObjSkillData(inSkillId);
		if (bornPownerObjSkillData != null && bornPownerObjSkillData.DamageActionIds != null && bornPownerObjSkillData.DamageActionIds.Count > 0)
		{
			for (int i = 0; i < bornPownerObjSkillData.DamageActionIds.Count; i++)
			{
				BaseAction actionInSkill = this.GetActionInSkill(bornPownerObjSkillData.SkillId, SkillCastPhase.Cast_In, bornPownerObjSkillData.DamageActionIds[i]);
				if (actionInSkill != null)
				{
					actionInSkill.DoSpecialProcess();
				}
			}
		}
		this.OnBornPownerObjTriggered(inSkillId, inIsMainPlayer);
	}

	private void OnBornPownerObjTriggered(string inSkillId, bool inIsMainPlayer)
	{
		Skill skillById = this.getSkillById(inSkillId);
		if (inIsMainPlayer)
		{
			if (skillById != null)
			{
			}
			Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(inSkillId);
		}
		if (skillById != null)
		{
			skillById.OnBornPowerObjTriggered();
		}
		this.RemoveBornPowerObjSkillData(inSkillId);
		if (skillById != null)
		{
			skillById.RemoveHighEffectAndBuffBySkillPhase(SkillPhrase.Start);
		}
	}

	public void TryShowBornPowerObjHint(string inSkillId, bool inIsMainPlayer)
	{
		if (!inIsMainPlayer)
		{
			return;
		}
		if (!this.IsSkillHaveBornPowerObj(inSkillId))
		{
			return;
		}
		Singleton<SkillView>.Instance.ShowTriggerBornPowerObjHint(inSkillId);
	}

	public void TryRemoveBornPowerObjInfo(string inSkillId, bool inIsMainPlayer)
	{
		if (!this.IsSkillHaveBornPowerObj(inSkillId))
		{
			return;
		}
		this.RemoveBornPowerObjSkillData(inSkillId);
		Skill skillById = this.getSkillById(inSkillId);
		if (skillById != null)
		{
			skillById.RemoveHighEffectAndBuffBySkillPhase(SkillPhrase.Start);
		}
		if (inIsMainPlayer)
		{
			Singleton<SkillView>.Instance.HideTriggerBornPowerObjHint(inSkillId);
		}
	}

	public void UpdateSkillUI()
	{
		Singleton<SkillView>.Instance.CheckIconToGrayByCanUseAll(null);
	}
}
