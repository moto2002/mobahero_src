using System;
using System.Collections;
using System.Diagnostics;

namespace MobaFrame.SkillAction
{
	public class PhysicCritPropAction : BaseHighEffAction
	{
		private int m_nLevel;

		protected override bool doAction()
		{
			base.doAction();
			if (this.data == null)
			{
				return false;
			}
			new Task(this.Coroutine(), true);
			return true;
		}

		[DebuggerHidden]
		public IEnumerator Coroutine()
		{
			PhysicCritPropAction.<Coroutine>c__Iterator83 <Coroutine>c__Iterator = new PhysicCritPropAction.<Coroutine>c__Iterator83();
			<Coroutine>c__Iterator.<>f__this = this;
			return <Coroutine>c__Iterator;
		}
	}
}
