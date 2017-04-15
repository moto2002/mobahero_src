using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class Activity_content_title : Activity_contentBase
	{
		public UILabel lb_title;

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eTitle;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak)
		{
			Activity_content_title.<RefreshUI>c__IteratorBE <RefreshUI>c__IteratorBE = new Activity_content_title.<RefreshUI>c__IteratorBE();
			<RefreshUI>c__IteratorBE.ieBreak = ieBreak;
			<RefreshUI>c__IteratorBE.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorBE.<>f__this = this;
			return <RefreshUI>c__IteratorBE;
		}
	}
}
