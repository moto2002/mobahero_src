using System;

namespace CLRSharp
{
	public interface IDelegate_BindTool
	{
		Delegate CreateDele(Type deletype, ThreadContext context, CLRSharp_Instance _this, IMethod _method);
	}
}
