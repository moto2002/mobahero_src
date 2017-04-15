using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class DelegateContainer<Tkey>
	{
		private Dictionary<Tkey, Delegate> m_funcs = new Dictionary<Tkey, Delegate>();

		public bool IsIgoreUnKnowCmd = false;

		public bool Regist(Tkey key, Delegate func)
		{
			Delegate @delegate = null;
			bool result;
			if (this.m_funcs.TryGetValue(key, out @delegate))
			{
				result = false;
			}
			else
			{
				this.m_funcs[key] = func;
				result = true;
			}
			return result;
		}

		public bool ExecMethod(Tkey key, object msg)
		{
			bool result;
			if (!this.m_funcs.ContainsKey(key))
			{
				if (!this.IsIgoreUnKnowCmd)
				{
					throw new ArgumentException("ExecMethod Exception,unknow cmd :" + key);
				}
				result = false;
			}
			else
			{
				Delegate @delegate = this.m_funcs[key];
				try
				{
					@delegate.DynamicInvoke(new object[]
					{
						msg
					});
				}
				catch (Exception ex)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"ExecMethod Exception:",
						key,
						"  ->",
						ex.ToString()
					}), ex);
				}
				result = true;
			}
			return result;
		}

		public bool ExecMethod(Tkey key, object[] list, string debugInfo)
		{
			bool result;
			if (!this.m_funcs.ContainsKey(key))
			{
				if (!this.IsIgoreUnKnowCmd)
				{
					throw new ArgumentException(string.Concat(new object[]
					{
						"ExecMethod Exception,unknow cmd :",
						key,
						", debugInfo:",
						debugInfo
					}));
				}
				result = false;
			}
			else
			{
				Delegate @delegate = this.m_funcs[key];
				int num = list.Length;
				int num2 = 0;
				try
				{
					switch (num)
					{
					case 0:
						@delegate.DynamicInvoke(new object[0]);
						break;
					case 1:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++]
						});
						break;
					case 2:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++]
						});
						break;
					case 3:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					case 4:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					case 5:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					case 6:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					case 7:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					case 8:
						@delegate.DynamicInvoke(new object[]
						{
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++],
							list[num2++]
						});
						break;
					}
				}
				catch (Exception ex)
				{
					string text = "";
					for (int i = 0; i < list.Length; i++)
					{
						object obj = list[i];
						text = text + obj.ToString() + ",";
					}
					throw new ArgumentException(string.Concat(new object[]
					{
						"ExecMethod Exception:",
						key,
						"  ",
						text,
						"->",
						ex.ToString()
					}), ex);
				}
				result = true;
			}
			return result;
		}

		public new string ToString()
		{
			string text = base.ToString() + "\n";
			foreach (KeyValuePair<Tkey, Delegate> current in this.m_funcs)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					current.Key,
					"  ",
					current.Value.ToString(),
					"\n"
				});
			}
			return text;
		}
	}
}
