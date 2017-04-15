using MobaFrame.SkillAction;
using System;

public class SkillUnits_LanmaoBaoZha : SkillUnit
{
	protected override void OnStart()
	{
		base.OnStart();
		UnitVisibilityManager.SetItemVisible(this, true);
	}

	public override void RemoveSelf(float delay = 0f)
	{
		UnitVisibilityManager.SetItemVisible(this, false);
		base.RemoveSelf(3.2f);
		ActionManager.PlayEffect("Perform_723", this, null, null, true, string.Empty, null);
	}
}
