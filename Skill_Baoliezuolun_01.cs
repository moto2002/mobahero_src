using MobaProtocol.Data;
using System;

public class Skill_Baoliezuolun_01 : Skill
{
	public Skill_Baoliezuolun_01(string skill_id, Units self) : base(skill_id, self)
	{
	}

	public override void SynInfo(SynSkillInfo info)
	{
		if (info.extraInt1 == 0 && this.unit.isPlayer)
		{
			UIMessageBox.ShowMessage("现在不能释放", 1.5f, 0);
		}
	}
}
