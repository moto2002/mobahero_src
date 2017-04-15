using Mono.Cecil.Cil;
using System;
using System.IO;
using System.Reflection;

namespace CLRSharp
{
	public interface ICLRSharp_Environment
	{
		string version
		{
			get;
		}

		ICLRSharp_Logger logger
		{
			get;
		}

		void LoadModule(Stream dllStream);

		void LoadModule(Stream dllStream, Stream pdbStream, ISymbolReaderProvider debugInfoLoader);

		void AddSerachAssembly(Assembly assembly);

		string[] GetAllTypes();

		ICLRType GetType(string name);

		string[] GetModuleRefNames();

		ICLRType GetType(Type systemType);

		void RegType(ICLRType type);

		void RegCrossBind(ICrossBind bind);

		ICrossBind GetCrossBind(Type type);
	}
}
