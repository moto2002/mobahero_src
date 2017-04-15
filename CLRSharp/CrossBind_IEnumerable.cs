using System;
using System.Collections;

namespace CLRSharp
{
	public class CrossBind_IEnumerable : ICrossBind
	{
		private class Base_IEnumerable : IEnumerable
		{
			private CLRSharp_Instance inst;

			public Base_IEnumerable(CLRSharp_Instance inst)
			{
				this.inst = inst;
			}

			public IEnumerator GetEnumerator()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				ICLRType type = activeContext.environment.GetType(typeof(IEnumerable));
				IMethod method = this.inst.type.GetMethod(type.FullName + ".GetEnumerator", MethodParamList.constEmpty());
				object obj = method.Invoke(activeContext, this.inst, null);
				return obj as IEnumerator;
			}
		}

		public Type Type
		{
			get
			{
				return typeof(IEnumerable);
			}
		}

		public object CreateBind(CLRSharp_Instance inst)
		{
			return new CrossBind_IEnumerable.Base_IEnumerable(inst);
		}
	}
}
