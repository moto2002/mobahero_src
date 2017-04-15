using System;

namespace CLRSharp
{
	public class CrossBind_IDisposable : ICrossBind
	{
		private class Base_IDisposable : IDisposable
		{
			private CLRSharp_Instance inst;

			public Base_IDisposable(CLRSharp_Instance inst)
			{
				this.inst = inst;
			}

			public void Dispose()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				ICLRType type = activeContext.environment.GetType(typeof(IDisposable));
				IMethod method = this.inst.type.GetMethod(type.FullName + ".Dispose", MethodParamList.constEmpty());
				object obj = method.Invoke(activeContext, this.inst, null);
			}
		}

		public Type Type
		{
			get
			{
				return typeof(IDisposable);
			}
		}

		public object CreateBind(CLRSharp_Instance inst)
		{
			return new CrossBind_IDisposable.Base_IDisposable(inst);
		}
	}
}
