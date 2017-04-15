using System;

namespace MobaClientCom
{
	internal enum ParserToken
	{
		None = 65536,
		Number,
		True,
		False,
		Null,
		CharSeq,
		Char,
		Text,
		Object,
		ObjectPrime,
		Pair,
		PairRest,
		Array,
		ArrayPrime,
		Value,
		ValueRest,
		String,
		End,
		Epsilon
	}
}
