using System;

namespace CLRSharp
{
	public class Delegate_BindTool : IDelegate_BindTool
	{
		private delegate void Action();

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool.Action action = delegate
			{
				_method.Invoke(context, _this, new object[0], true, true);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool<T1> : IDelegate_BindTool
	{
		private delegate void Action(T1 p1);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool<T1>.Action action = delegate(T1 p1)
			{
				_method.Invoke(context, _this, new object[]
				{
					p1
				}, true, true);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool<T1, T2> : IDelegate_BindTool
	{
		private delegate void Action(T1 p1, T2 p2);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool<T1, T2>.Action action = delegate(T1 p1, T2 p2)
			{
				_method.Invoke(context, _this, new object[]
				{
					p1,
					p2
				}, true, true);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool<T1, T2, T3> : IDelegate_BindTool
	{
		private delegate void Action(T1 p1, T2 p2, T3 p3);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool<T1, T2, T3>.Action action = delegate(T1 p1, T2 p2, T3 p3)
			{
				_method.Invoke(context, _this, new object[]
				{
					p1,
					p2,
					p3
				}, true, true);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool<T1, T2, T3, T4> : IDelegate_BindTool
	{
		private delegate void Action(T1 p1, T2 p2, T3 p3, T4 p4);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool<T1, T2, T3, T4>.Action action = delegate(T1 p1, T2 p2, T3 p3, T4 p4)
			{
				_method.Invoke(context, _this, new object[]
				{
					p1,
					p2,
					p3,
					p4
				}, true, true);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
}
