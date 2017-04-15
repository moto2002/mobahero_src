using System;

namespace MobaClientCom
{
	public enum JsonToken
	{
		None,
		ObjectStart,
		PropertyName,
		ObjectEnd,
		ArrayStart,
		ArrayEnd,
		Int,
		Long,
		Double,
		String,
		Boolean,
		Null
	}
}
