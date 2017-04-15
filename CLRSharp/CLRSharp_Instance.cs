using System;
using System.Collections.Generic;

namespace CLRSharp
{
	public class CLRSharp_Instance
	{
		public object system_base;

		public Dictionary<string, object> Fields = new Dictionary<string, object>();

		public Dictionary<IMethod, Delegate> Delegates = new Dictionary<IMethod, Delegate>();

		public ICLRType_Sharp type
		{
			get;
			private set;
		}

		public CLRSharp_Instance(ICLRType_Sharp type)
		{
			this.type = type;
		}

		public Delegate GetDelegate(ThreadContext context, Type deleType, IMethod method)
		{
			Delegate @delegate = null;
			bool flag = !this.Delegates.TryGetValue(method, out @delegate);
			if (flag)
			{
				@delegate = Delegate_Binder.MakeDelegate(deleType, this, method);
				this.Delegates[method] = @delegate;
			}
			return @delegate;
		}
	}
}
