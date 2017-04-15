using System;

public class SkillUnits_FengBaoZhiYan : SkillUnit
{
	protected override void OnUpdate(float delta)
	{
		base.OnUpdate(delta);
		if (this.isDestroy)
		{
			return;
		}
		if (base.ParentUnit == null || !base.ParentUnit.isLive)
		{
			this.RemoveSelf(0f);
		}
	}
}
