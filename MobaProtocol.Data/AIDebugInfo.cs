using ProtoBuf;
using System;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class AIDebugInfo
	{
		[ProtoMember(1)]
		public short _unitId
		{
			get;
			set;
		}

		public int unitId
		{
			get
			{
				return (int)this._unitId;
			}
			set
			{
				this._unitId = (short)value;
			}
		}

		[ProtoMember(2)]
		public string state
		{
			get;
			set;
		}
	}
}
