using System;
using System.Collections;
using System.Diagnostics;

namespace Com.Game.Module
{
	public class Activity_content_time : Activity_contentBase
	{
		public UILabel lb_content;

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eTime;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak = null)
		{
			Activity_content_time.<RefreshUI>c__IteratorBD <RefreshUI>c__IteratorBD = new Activity_content_time.<RefreshUI>c__IteratorBD();
			<RefreshUI>c__IteratorBD.ieBreak = ieBreak;
			<RefreshUI>c__IteratorBD.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorBD.<>f__this = this;
			return <RefreshUI>c__IteratorBD;
		}
	}
}
