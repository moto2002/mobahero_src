using System;

namespace MobaMessageData
{
	public class MsgData_LoadView2_setProgress
	{
		public enum SetType
		{
			addNum,
			targetNum
		}

		public MsgData_LoadView2_setProgress.SetType setType;

		public int subProgress;

		public int totalNum;

		public int curNum;

		public string notice;

		public MsgData_LoadView2_setProgress(MsgData_LoadView2_setProgress.SetType e, int sub, int total, int cur, string n = null)
		{
			this.setType = e;
			this.subProgress = sub;
			this.totalNum = total;
			this.curNum = cur;
			this.notice = n;
		}
	}
}
