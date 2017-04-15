using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class Activity_content_interval : Activity_contentBase
	{
		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eNone;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak = null)
		{
			Activity_content_interval.<RefreshUI>c__IteratorB4 <RefreshUI>c__IteratorB = new Activity_content_interval.<RefreshUI>c__IteratorB4();
			<RefreshUI>c__IteratorB.ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<>f__this = this;
			return <RefreshUI>c__IteratorB;
		}
	}
}
