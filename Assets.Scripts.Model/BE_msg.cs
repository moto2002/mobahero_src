using System;

namespace Assets.Scripts.Model
{
	internal class BE_msg
	{
		public object code;

		public object param;

		public bool dispatch;

		public BE_msg(object c, object p, bool d)
		{
			this.code = c;
			this.param = p;
			this.dispatch = d;
		}
	}
}
