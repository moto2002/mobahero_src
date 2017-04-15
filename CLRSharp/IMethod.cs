using System;

namespace CLRSharp
{
	public interface IMethod
	{
		bool isStatic
		{
			get;
		}

		string Name
		{
			get;
		}

		ICLRType DeclaringType
		{
			get;
		}

		ICLRType ReturnType
		{
			get;
		}

		MethodParamList ParamList
		{
			get;
		}

		object Invoke(ThreadContext context, object _this, object[] _params);

		object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual);

		object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump);
	}
}
