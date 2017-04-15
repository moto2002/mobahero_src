using System;
using System.Collections;

namespace CLRSharp
{
	public class CrossBind_IEnumerator : ICrossBind
	{
		private class Base_IEnumerator : IEnumerator
		{
			private CLRSharp_Instance inst;

			private IMethod _MoveNext;

			private IMethod _get_Current;

			private IMethod _Reset;

			public object Current
			{
				get
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					return this._get_Current.Invoke(activeContext, this.inst, null);
				}
			}

			public Base_IEnumerator(CLRSharp_Instance inst)
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				this.inst = inst;
				string[] methodNames = this.inst.type.GetMethodNames();
				string[] array = methodNames;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					bool flag = text.Contains("MoveNext");
					if (flag)
					{
						this._MoveNext = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					bool flag2 = text.Contains(".get_Current");
					if (flag2)
					{
						this._get_Current = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					bool flag3 = text.Contains(".Reset");
					if (flag3)
					{
						this._Reset = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
				}
			}

			public bool MoveNext()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				VBox vBox = this._MoveNext.Invoke(activeContext, this.inst, null) as VBox;
				return vBox.ToBool();
			}

			public void Reset()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._Reset.Invoke(activeContext, this.inst, null);
			}
		}

		public Type Type
		{
			get
			{
				return typeof(IEnumerator);
			}
		}

		public object CreateBind(CLRSharp_Instance inst)
		{
			return new CrossBind_IEnumerator.Base_IEnumerator(inst);
		}
	}
}
