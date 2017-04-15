using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class DataUpdateInfo
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
		public List<short> dataKeys
		{
			get;
			set;
		}

		[ProtoMember(4)]
		public List<float> dataValues
		{
			get;
			set;
		}
	}
}
