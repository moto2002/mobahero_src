using Com.Game.Module;
using System;

namespace MobaFrame.SkillAction
{
	public class SwitchAction : BaseHighEffAction
	{
		private SwitchSkillType switchType;

		private SwitchOffType switchOffType;

		private string sourceId;

		private string targetId;

		public void OnAttackOver()
		{
			this.StopHighEff();
		}

		private void DoSwitchOffType()
		{
			SwitchOffType switchOffType = this.switchOffType;
			if (switchOffType != SwitchOffType.AttackOver)
			{
				if (switchOffType != SwitchOffType.SkillOver)
				{
				}
			}
		}

		protected override void StartHighEff()
		{
			this.switchType = (SwitchSkillType)this.data.param1;
			if (this.data.strParam1 == "Attack")
			{
				this.sourceId = base.unit.currentAttack.skillMainId;
			}
			else
			{
				this.sourceId = this.data.strParam1;
			}
			if (this.data.strParam1 == "Skill")
			{
				this.sourceId = base.unit.currentSkill.skillMainId;
			}
			else
			{
				this.targetId = this.data.strParam2;
			}
			this.SwitchOn();
			this.DoSwitchOffType();
		}

		protected override void StopHighEff()
		{
			this.SwitchOff();
		}

		private void SwitchOn()
		{
			SwitchSkillType switchSkillType = this.switchType;
			if (switchSkillType != SwitchSkillType.Attack)
			{
				if (switchSkillType == SwitchSkillType.Skill)
				{
					base.unit.ReplaceSkill(this.sourceId, this.targetId);
					if (base.unit.isPlayer)
					{
						Singleton<SkillView>.Instance.ChangePlayer();
					}
				}
			}
			else
			{
				base.unit.ReplaceAttack(this.sourceId, this.targetId);
			}
		}

		private void SwitchOff()
		{
			SwitchSkillType switchSkillType = this.switchType;
			if (switchSkillType != SwitchSkillType.Attack)
			{
				if (switchSkillType == SwitchSkillType.Skill)
				{
					base.unit.ReplaceSkill(this.targetId, this.sourceId);
					if (base.unit.isPlayer)
					{
						Singleton<SkillView>.Instance.ChangePlayer();
					}
				}
			}
			else
			{
				base.unit.ReplaceAttack(this.targetId, this.sourceId);
			}
		}
	}
}
