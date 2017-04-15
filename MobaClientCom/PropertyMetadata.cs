using System;
using System.Reflection;

namespace MobaClientCom
{
	internal struct PropertyMetadata
	{
		public MemberInfo Info;

		public bool IsField;

		public Type Type;
	}
}
