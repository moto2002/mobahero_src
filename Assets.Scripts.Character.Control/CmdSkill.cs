using System;

namespace Assets.Scripts.Character.Control
{
	public class CmdSkill : CmdBase
	{
		public string skillMainID;

		public Units targetUnits;

		public Skill skill;

		public CmdSkill()
		{
			this.isSkillCmd = true;
		}
	}
}
