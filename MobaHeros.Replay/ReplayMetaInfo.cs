using MobaProtocol;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Replay
{
	[Serializable]
	public class ReplayMetaInfo
	{
		[Serializable]
		public class ReplayMetaEntry
		{
			public int ReplayId;

			public string ReplayFile;

			public int BattleId;

			public PvpJoinType JoinType;

			public PvpGameType GameType;

			public DateTime Time;

			public TimeSpan GameDuration;

			public string Extra;

			public int RoomId;

			public int MyNewId;

			public byte[] AllPlayersBytes;

			[NonSerialized]
			public ReadyPlayerSampleInfo[] AllPlayers;

			public void Encode()
			{
				this.AllPlayersBytes = SerializeHelper.Serialize<ReadyPlayerSampleInfo[]>(this.AllPlayers);
			}

			public void Decode()
			{
				if (this.AllPlayersBytes != null)
				{
					this.AllPlayers = SerializeHelper.Deserialize<ReadyPlayerSampleInfo[]>(this.AllPlayersBytes);
				}
			}
		}

		public List<ReplayMetaInfo.ReplayMetaEntry> _replayEntryList = new List<ReplayMetaInfo.ReplayMetaEntry>();

		public List<ReplayMetaInfo.ReplayMetaEntry> ReplayEntryList
		{
			get
			{
				return this._replayEntryList;
			}
		}
	}
}
