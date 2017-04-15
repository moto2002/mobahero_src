using CLRSharp;
using System;
using UnityEngine;

namespace ScriptShell
{
	public class LSharpLogger : ICLRSharp_Logger
	{
		public void Log(string str)
		{
			Debug.Log(str);
		}

		public void Log_Error(string str)
		{
			Debug.LogError(str);
		}

		public void Log_Warning(string str)
		{
			Debug.LogWarning(str);
		}
	}
}
