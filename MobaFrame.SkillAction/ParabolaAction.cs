using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class ParabolaAction : BaseHighEffAction
	{
		private float fspeed = 0.2f;

		private float fTime = 0.5f;

		private Units target;

		protected override void StartHighEff()
		{
			if (this.targetUnits == null || this.targetUnits.Count < 1 || this.targetUnits[0] == null)
			{
				this.Destroy();
				return;
			}
			this.target = this.targetUnits[0];
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
		}

		[DebuggerHidden]
		protected IEnumerator Coroutine()
		{
			ParabolaAction.<Coroutine>c__Iterator82 <Coroutine>c__Iterator = new ParabolaAction.<Coroutine>c__Iterator82();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
