using System;

namespace CLRSharp
{
	public interface ICLRSharp_Logger
	{
		void Log(string str);

		void Log_Warning(string str);

		void Log_Error(string str);
	}
}
