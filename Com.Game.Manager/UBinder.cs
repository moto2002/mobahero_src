using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Com.Game.Manager
{
	public class UBinder : SerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			Assembly assembly = Assembly.Load(assemblyName);
			return assembly.GetType(typeName);
		}
	}
}
