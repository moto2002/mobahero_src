using System;
using System.Collections;
using System.Diagnostics;

public class SkillUnits_TimoMoGu : SkillUnit
{
	protected override void OnStart()
	{
		base.OnStart();
	}

	public override void RemoveSelf(float delay = 0f)
	{
		base.ClearAllCharaState();
		GlobalObject.Instance.StartCoroutine(this.PlayAnim());
	}

	[DebuggerHidden]
	private IEnumerator PlayAnim()
	{
		SkillUnits_TimoMoGu.<PlayAnim>c__Iterator22 <PlayAnim>c__Iterator = new SkillUnits_TimoMoGu.<PlayAnim>c__Iterator22();
		<PlayAnim>c__Iterator.<>f__this = this;
		return <PlayAnim>c__Iterator;
	}
}
