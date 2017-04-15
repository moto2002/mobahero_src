using System;

namespace CLRSharp
{
	public interface IMethod_Sharp : IMethod
	{
		CodeBody body
		{
			get;
		}
	}
}
