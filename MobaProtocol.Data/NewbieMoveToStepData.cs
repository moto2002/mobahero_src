using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class NewbieMoveToStepData
	{
		[ProtoMember(1)]
		public int stepType
		{
			get;
			set;
		}
	}
}
