using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class Activity_content_intro : Activity_contentBase
	{
		public UILabel lb_content;

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eIntro;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak)
		{
			Activity_content_intro.<RefreshUI>c__IteratorB5 <RefreshUI>c__IteratorB = new Activity_content_intro.<RefreshUI>c__IteratorB5();
			<RefreshUI>c__IteratorB.ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<>f__this = this;
			return <RefreshUI>c__IteratorB;
		}
	}
}
