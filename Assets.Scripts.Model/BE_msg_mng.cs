using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class BE_msg_mng
	{
		private Dictionary<int, BE_msg> dic;

		public BE_msg_mng()
		{
			this.dic = new Dictionary<int, BE_msg>();
		}

		public void Add(object code, object param, bool dispatch)
		{
			int key = (int)code;
			BE_msg value = new BE_msg(code, param, dispatch);
			if (this.dic.ContainsKey(key))
			{
				this.dic[key] = value;
			}
			else
			{
				this.dic.Add(key, value);
			}
		}

		public void Dispatch()
		{
			if (this.dic != null)
			{
				Dictionary<int, BE_msg>.Enumerator enumerator = this.dic.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, BE_msg> current = enumerator.Current;
					BE_msg value = current.Value;
					MobaMessageManagerTools.SendClientMsg(value.code, value.param, value.dispatch);
				}
				this.dic.Clear();
			}
		}
	}
}
