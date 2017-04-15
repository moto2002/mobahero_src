using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Game.Module
{
	public class TeamDataManagement
	{
		private bool _isDebug = true;

		private readonly List<RoomData> _roomMemberList = new List<RoomData>();

		public List<RoomData> RoomMemberList
		{
			get
			{
				return this._roomMemberList;
			}
		}

		public bool IsEmpty()
		{
			return 0 == this._roomMemberList.Count;
		}

		public void AddRoomData(List<RoomData> roomDataList)
		{
			if (roomDataList == null)
			{
				return;
			}
			roomDataList.RemoveAll((RoomData x) => x == null);
			for (int i = 0; i < roomDataList.Count; i++)
			{
				this._roomMemberList.Add(roomDataList[i]);
			}
			this.Dump();
		}

		public void RemoveRoomData(RoomData roomData)
		{
			if (roomData == null)
			{
				return;
			}
			for (int i = 0; i < this._roomMemberList.Count; i++)
			{
				if (this._roomMemberList[i].UserId == roomData.UserId)
				{
					this._roomMemberList.Remove(this._roomMemberList[i]);
					break;
				}
			}
			this.Dump();
		}

		public void RemoveRoomData(List<RoomData> roomDataList)
		{
			if (roomDataList == null)
			{
				return;
			}
			for (int i = 0; i < roomDataList.Count; i++)
			{
				this.RemoveRoomData(roomDataList[i]);
			}
		}

		public void ClearRoomData()
		{
			this._roomMemberList.Clear();
			this.Dump();
		}

		public void ReplaceRoomData(RoomData roomData)
		{
			if (roomData == null)
			{
				return;
			}
			if (this.IsEmpty())
			{
				return;
			}
			for (int i = 0; i < this._roomMemberList.Count; i++)
			{
				if (this._roomMemberList[i].UserId == roomData.UserId)
				{
					this._roomMemberList[i] = roomData;
					break;
				}
			}
			this.Dump();
		}

		public void ReplaceRoomData(List<RoomData> roomDataList)
		{
			if (roomDataList == null)
			{
				return;
			}
			for (int i = 0; i < roomDataList.Count; i++)
			{
				this.ReplaceRoomData(roomDataList[i]);
			}
			this.Dump();
		}

		public void SetRoomDataList(List<RoomData> roomDataList)
		{
			this._roomMemberList.Clear();
			if (roomDataList != null)
			{
				roomDataList.RemoveAll((RoomData x) => x == null);
				this._roomMemberList.AddRange(roomDataList);
			}
			this.Dump();
		}

		private void Dump()
		{
			if (this._isDebug)
			{
				IEnumerable<string> source = from x in this._roomMemberList
				select x.NickName + "@" + x.TeamType;
				string text = source.Aggregate("result is: ", (string x, string y) => x + "  " + y);
			}
		}

		public void Clear()
		{
			this._roomMemberList.Clear();
			this.Dump();
		}

		public void Remove(RoomData item)
		{
			this._roomMemberList.Remove(item);
			this.Dump();
		}

		public void Insert(int i, RoomData item)
		{
			this._roomMemberList.Insert(i, item);
			this.Dump();
		}
	}
}
