using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class BaseStateAction : BaseHighEffAction
	{
		public Units targetUnit;

		protected override void StartHighEff()
		{
			base.mCoroutineManager.StartCoroutine(this.Coroutine(), true);
		}

		protected override void StopHighEff()
		{
			base.mCoroutineManager.StopAllCoroutine();
		}

		[DebuggerHidden]
		protected virtual IEnumerator Coroutine()
		{
			BaseStateAction.<Coroutine>c__Iterator6C <Coroutine>c__Iterator6C = new BaseStateAction.<Coroutine>c__Iterator6C();
			<Coroutine>c__Iterator6C.<>f__this = this;
			return <Coroutine>c__Iterator6C;
		}
	}
}
