using Com.Game.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class Activity_content_goto : Activity_contentBase
	{
		public UILabel lb_content;

		public UIButton btn_goto;

		private void Awake()
		{
			UIEventListener.Get(this.btn_goto.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClickBtn_goto);
		}

		public override EActivityModuleType GetModuleType()
		{
			return EActivityModuleType.eGoto;
		}

		[DebuggerHidden]
		public override IEnumerator RefreshUI(Func<IEnumerator> ieBreak)
		{
			Activity_content_goto.<RefreshUI>c__IteratorB3 <RefreshUI>c__IteratorB = new Activity_content_goto.<RefreshUI>c__IteratorB3();
			<RefreshUI>c__IteratorB.ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<$>ieBreak = ieBreak;
			<RefreshUI>c__IteratorB.<>f__this = this;
			return <RefreshUI>c__IteratorB;
		}

		private void OnClickBtn_goto(GameObject go)
		{
			int i = 0;
			if (base.AType == EActivityType.eActivity)
			{
				SysActivityModuleVo sysActivityModuleVo = base.Info as SysActivityModuleVo;
				if (sysActivityModuleVo != null)
				{
					i = int.Parse(sysActivityModuleVo.content_first);
				}
			}
			else
			{
				NoticeModuleData noticeModuleData = base.Info as NoticeModuleData;
				if (noticeModuleData != null)
				{
					i = noticeModuleData.data.type;
				}
			}
			GotoWindowTools.GotoWindow(i);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.activity_close, null, false);
		}
	}
}
