using System;
using System.Collections;
using System.Diagnostics;

public class Monster_Guowang : Monster
{
	private bool canPlay;

	protected override void OnUpdate(float deltaTime)
	{
		base.OnUpdate(deltaTime);
		if (!this.canPlay)
		{
			this.m_CoroutineManager.StartCoroutine(this.GuowangStandby(), true);
			this.canPlay = true;
		}
	}

	[DebuggerHidden]
	private IEnumerator GuowangStandby()
	{
		Monster_Guowang.<GuowangStandby>c__Iterator1E <GuowangStandby>c__Iterator1E = new Monster_Guowang.<GuowangStandby>c__Iterator1E();
		<GuowangStandby>c__Iterator1E.<>f__this = this;
		return <GuowangStandby>c__Iterator1E;
	}
}
