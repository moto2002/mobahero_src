using System;

namespace MobaClientCom
{
	internal delegate void ExporterFunc(object obj, JsonWriter writer);
	public delegate void ExporterFunc<T>(T obj, JsonWriter writer);
}
