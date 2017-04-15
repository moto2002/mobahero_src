using System;

namespace CLRSharp
{
	public interface ICLRType
	{
		ICLRSharp_Environment env
		{
			get;
		}

		string Name
		{
			get;
		}

		string FullName
		{
			get;
		}

		string FullNameWithAssembly
		{
			get;
		}

		Type TypeForSystem
		{
			get;
		}

		ICLRType[] SubTypes
		{
			get;
		}

		IMethod GetMethod(string funcname, MethodParamList types);

		IMethod[] GetMethods(string funcname);

		IMethod[] GetAllMethods();

		object InitObj();

		IMethod GetMethodT(string funcname, MethodParamList TTypes, MethodParamList types);

		IField GetField(string name);

		string[] GetFieldNames();

		bool IsInst(object obj);

		ICLRType GetNestType(ICLRSharp_Environment env, string fullname);

		bool IsEnum();
	}
}
