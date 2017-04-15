using Mono.Cecil;
using System;

namespace CLRSharp
{
	public interface ICLRType_Sharp : ICLRType
	{
		CLRSharp_Instance staticInstance
		{
			get;
		}

		bool NeedCCtor
		{
			get;
		}

		TypeDefinition type_CLRSharp
		{
			get;
		}

		bool HasSysBase
		{
			get;
		}

		void ResetStaticInstace();

		void InvokeCCtor(ThreadContext context);

		IMethod GetVMethod(IMethod _base);

		bool ContainBase(Type t);

		string[] GetMethodNames();
	}
}
