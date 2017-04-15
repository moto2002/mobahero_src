using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class ViewerInfo
	{
		[ProtoMember(1)]
		public int count
		{
			get;
			set;
		}
	}
}
