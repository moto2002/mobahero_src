using MobaHeros;
using MobaProtocol.Data;
using ProtoBuf.Meta;
using System;
using System.Reflection;

internal static class ProtocolFactory
{
	private static readonly ObjectPoolWithCollectiveReset<UnitSnapInfo> _pool = new ObjectPoolWithCollectiveReset<UnitSnapInfo>(10, null, null);

	public static void Clear()
	{
		ProtocolFactory._pool.ReleaseAll();
	}

	private static UnitSnapInfo CreateSnapInfo()
	{
		return ProtocolFactory._pool.Get();
	}

	public static void Init()
	{
		MethodInfo method = typeof(ProtocolFactory).GetMethod("CreateSnapInfo", BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		RuntimeTypeModel.Default[typeof(UnitSnapInfo)].SetFactory(method);
	}
}
