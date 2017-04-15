using System;

namespace MobaMessageData
{
	public class MsgData_LoadView_setProgress
	{
		public enum SetType
		{
			addNum,
			targetNum
		}

		private MsgData_LoadView_setProgress.SetType _e;

		private int _num;

		public MsgData_LoadView_setProgress.SetType AddType
		{
			get
			{
				return this._e;
			}
		}

		public int Num
		{
			get
			{
				return this._num;
			}
		}

		public MsgData_LoadView_setProgress(MsgData_LoadView_setProgress.SetType e, int num)
		{
			this._e = e;
			this._num = num;
		}

		public override string ToString()
		{
			return this._e + ":" + this._num;
		}
	}
}
