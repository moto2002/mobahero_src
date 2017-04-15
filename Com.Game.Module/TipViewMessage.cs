using System;

namespace Com.Game.Module
{
	public class TipViewMessage
	{
		private string text;

		private float delayTime = 2f;

		private Callback<object> callBack;

		public string Text
		{
			get
			{
				return this.text;
			}
			private set
			{
				this.text = value;
			}
		}

		public float DelayTime
		{
			get
			{
				return this.delayTime;
			}
			private set
			{
				this.delayTime = value;
			}
		}

		public Callback<object> CallBack
		{
			get
			{
				return this.callBack;
			}
			private set
			{
				this.callBack = value;
			}
		}

		public TipViewMessage(string str = "", float delay = 2f, Callback<object> objCallBack = null)
		{
			this.Text = str;
			this.DelayTime = delay;
			this.CallBack = objCallBack;
		}
	}
}
