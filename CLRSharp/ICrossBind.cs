using System;

namespace CLRSharp
{
	public interface ICrossBind
	{
		Type Type
		{
			get;
		}

		object CreateBind(CLRSharp_Instance inst);
	}
}
