using ProtoBuf;
using System;
using System.Collections.Generic;

namespace MobaProtocol.Data
{
	[ProtoContract]
	public class TeamRoomData
	{
		[ProtoMember(1)]
		public string battleid;

		[ProtoMember(2)]
		public string roomid;

		[ProtoMember(3)]
		public int peoplemax;

		[ProtoMember(4)]
		public int viewerMax;

		[ProtoMember(5)]
		public string ownerid;

		[ProtoMember(6)]
		public Dictionary<TeamRoomType, List<RoomData>> teamMateList;

		[ProtoMember(7)]
		public RoomType roomtype;

		[ProtoMember(8)]
		public TeamPvpData teamPvpData;

		public TeamRoomState roomstate;

		public string lobbykey;

		public List<RoomData> GetUsrs()
		{
			List<RoomData> list = new List<RoomData>();
			foreach (List<RoomData> current in this.teamMateList.Values)
			{
				if (current != null && current.Count > 0)
				{
					list.AddRange(current);
				}
			}
			return list;
		}
	}
}
