using System;

namespace CLRSharp
{
	public class Delegate_BindTool_Ret<TRet> : IDelegate_BindTool
	{
		private delegate TRet Action();

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool_Ret<TRet>.Action action = delegate
			{
				object obj = _method.Invoke(context, _this, new object[0], true, true);
				bool flag = obj is VBox;
				if (flag)
				{
					obj = (obj as VBox).BoxDefine();
				}
				return (TRet)((object)obj);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool_Ret<TRet, T1> : IDelegate_BindTool
	{
		private delegate TRet Action(T1 p1);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool_Ret<TRet, T1>.Action action = delegate(T1 p1)
			{
				object obj = _method.Invoke(context, _this, new object[]
				{
					p1
				}, true, true);
				bool flag = obj is VBox;
				if (flag)
				{
					obj = (obj as VBox).BoxDefine();
				}
				return (TRet)((object)obj);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool_Ret<TRet, T1, T2> : IDelegate_BindTool
	{
		private delegate TRet Action(T1 p1, T2 p2);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool_Ret<TRet, T1, T2>.Action action = delegate(T1 p1, T2 p2)
			{
				object obj = _method.Invoke(context, _this, new object[]
				{
					p1,
					p2
				}, true, true);
				bool flag = obj is VBox;
				if (flag)
				{
					obj = (obj as VBox).BoxDefine();
				}
				return (TRet)((object)obj);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool_Ret<TRet, T1, T2, T3> : IDelegate_BindTool
	{
		private delegate TRet Action(T1 p1, T2 p2, T3 p3);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool_Ret<TRet, T1, T2, T3>.Action action = delegate(T1 p1, T2 p2, T3 p3)
			{
				object obj = _method.Invoke(context, _this, new object[]
				{
					p1,
					p2,
					p3
				}, true, true);
				bool flag = obj is VBox;
				if (flag)
				{
					obj = (obj as VBox).BoxDefine();
				}
				return (TRet)((object)obj);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
	public class Delegate_BindTool_Ret<TRet, T1, T2, T3, T4> : IDelegate_BindTool
	{
		private delegate TRet Action(T1 p1, T2 p2, T3 p3, T4 p4);

		public Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method)
		{
			Delegate_BindTool_Ret<TRet, T1, T2, T3, T4>.Action action = delegate(T1 p1, T2 p2, T3 p3, T4 p4)
			{
				object obj = _method.Invoke(context, _this, new object[]
				{
					p1,
					p2,
					p3,
					p4
				}, true, true);
				bool flag = obj is VBox;
				if (flag)
				{
					obj = (obj as VBox).BoxDefine();
				}
				return (TRet)((object)obj);
			};
			return Delegate.CreateDelegate(deletype, action.Target, action.Method);
		}
	}
}
