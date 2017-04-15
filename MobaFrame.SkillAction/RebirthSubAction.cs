using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class RebirthSubAction : BaseStateAction
	{
		private VTrigger rebirthTrigger;

		private bool isRebirth;

		private string skillId_kulouwang_04 = "Skill_Kulouwang_04";

		private int skillPointLeft;

		private Dictionary<string, int> skillLevelInfos = new Dictionary<string, int>();

		protected override bool doAction()
		{
			return this.skill != null && !(this.skill.skillMainId != this.skillId_kulouwang_04) && base.doAction();
		}

		protected override void OnDestroy()
		{
		}

		protected override void OnStop()
		{
			base.OnStop();
		}

		[DebuggerHidden]
		protected override IEnumerator Coroutine()
		{
			return new RebirthSubAction.<Coroutine>c__Iterator8C();
		}

		protected override void StartHighEff()
		{
			this.isRebirth = false;
			this.rebirthTrigger = TriggerManager.CreateUnitEventTrigger(UnitEvent.UnitRebirth, null, new TriggerAction(this.doReBirth), this.targetUnit.unique_id);
			this.targetUnit.CanRebirth.Add();
			if (this.skill != null && !this.skill.IsCDTimeOver)
			{
				this.skill.StartCDTime(-1f, false);
			}
		}

		protected virtual void doReBirth()
		{
			if (this.targetUnit == null)
			{
				return;
			}
			if (this.skill == null)
			{
				return;
			}
			if (base.mCoroutineManager == null)
			{
				return;
			}
			this.targetUnit.CanRebirth.Remove();
			if (this.skill.IsCDTimeOver)
			{
				base.mCoroutineManager.StartCoroutine(this.Rebirth_Coroutinue(), true);
			}
			else
			{
				this.Destroy();
			}
		}

		[DebuggerHidden]
		protected IEnumerator Rebirth_Coroutinue()
		{
			RebirthSubAction.<Rebirth_Coroutinue>c__Iterator8D <Rebirth_Coroutinue>c__Iterator8D = new RebirthSubAction.<Rebirth_Coroutinue>c__Iterator8D();
			<Rebirth_Coroutinue>c__Iterator8D.<>f__this = this;
			return <Rebirth_Coroutinue>c__Iterator8D;
		}

		private void SetHPAndMP(float hp, float mp, float preHPMax, float preMPMax)
		{
			if (this.targetUnit == null)
			{
				return;
			}
			if (this.targetUnit.data == null)
			{
				return;
			}
			this.targetUnit.data.SetMaxHp(preHPMax);
			this.targetUnit.data.SetMaxMp(preMPMax);
			this.targetUnit.SetHp(hp);
			this.targetUnit.SetMp(mp);
		}

		[DebuggerHidden]
		private IEnumerator RebirthCD_Coroutinue()
		{
			RebirthSubAction.<RebirthCD_Coroutinue>c__Iterator8E <RebirthCD_Coroutinue>c__Iterator8E = new RebirthSubAction.<RebirthCD_Coroutinue>c__Iterator8E();
			<RebirthCD_Coroutinue>c__Iterator8E.<>f__this = this;
			return <RebirthCD_Coroutinue>c__Iterator8E;
		}

		private void CacheSkillPoint(Units u)
		{
			if (u.skillManager == null)
			{
				return;
			}
			this.skillPointLeft = u.skillManager.SkillPointsLeft;
			this.skillLevelInfos.Clear();
			string[] skillIDs = u.skillManager.GetSkillIDs();
			if (skillIDs == null)
			{
				return;
			}
			string[] array = skillIDs;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				this.skillLevelInfos.Add(text, u.skillManager.GetSkillLevel(text));
			}
		}

		private void ReSetSkillPoint(Units u)
		{
			if (u == null)
			{
				return;
			}
			if (u.skillManager == null)
			{
				return;
			}
			u.skillManager.SkillPointsLeft = this.skillPointLeft;
			string[] skillIDs = u.skillManager.GetSkillIDs();
			string[] array = skillIDs;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (this.skillLevelInfos.ContainsKey(text))
				{
					u.skillManager.SetSkillLevel(text, this.skillLevelInfos[text]);
				}
			}
		}
	}
}
