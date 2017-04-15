using System;

namespace Com.Game.Module
{
	public class MapInfoData
	{
		private string mTime;

		private string mName;

		private string mPlayersNum;

		private string mRewardPoint;

		private int mBattleId;

		public string time
		{
			get
			{
				return this.mTime;
			}
			set
			{
				this.mTime = value;
			}
		}

		public string name
		{
			get
			{
				return this.mName;
			}
			set
			{
				this.mName = value;
			}
		}

		public string playersNum
		{
			get
			{
				return this.mPlayersNum;
			}
			set
			{
				this.mPlayersNum = value;
			}
		}

		public string rewardPoint
		{
			get
			{
				return this.mRewardPoint;
			}
			set
			{
				this.mRewardPoint = value;
			}
		}

		public int battleId
		{
			get
			{
				return this.mBattleId;
			}
			set
			{
				this.mBattleId = value;
			}
		}

		public MapInfoData(int _id, string _time = "?-?", string _name = "MapName", string _playersNum = "?V?", string _rewardPoint = "?")
		{
			this.mBattleId = _id;
			this.mTime = _time;
			this.mName = _name;
			this.mPlayersNum = _playersNum;
			this.mRewardPoint = _rewardPoint;
		}
	}
}
