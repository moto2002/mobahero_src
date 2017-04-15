using System;

namespace CLRSharp
{
	public interface ICLRType_System : ICLRType
	{
		Delegate CreateDelegate(Type deletype, object _this, IMethod_System _method);
	}
}
