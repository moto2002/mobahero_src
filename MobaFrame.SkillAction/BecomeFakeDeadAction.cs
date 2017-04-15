using System;

namespace MobaFrame.SkillAction
{
	public class BecomeFakeDeadAction : BecomeDeadAction
	{
		protected override bool doAction()
		{
			return true;
		}
	}
}
