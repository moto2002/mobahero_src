using Com.Game.Data;
using Com.Game.Manager;
using Com.Game.Utils;
using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace MobaHeros.Pvp
{
	public class RoomInfo
	{
		public const int ObserverUserId = -2147483648;

		private readonly Dictionary<int, HeroExtraInRoom> _heroExtras = new Dictionary<int, HeroExtraInRoom>();

		private readonly List<ReadyPlayerSampleInfo> _pvpPlayers = new List<ReadyPlayerSampleInfo>();

		public double OldLadderScore;

		public readonly HashSet<int> CtrlUniqueIds = new HashSet<int>();

		public string SummIdObserved
		{
			get;
			private set;
		}

		public IEnumerable<ReadyPlayerSampleInfo> PvpPlayers
		{
			get
			{
				return this._pvpPlayers;
			}
		}

		public int RoomId
		{
			get;
			private set;
		}

		public bool IsRoomReady
		{
			get;
			private set;
		}

		public int MyUserId
		{
			get;
			private set;
		}

		public TeamType SelfTeam
		{
			get;
			private set;
		}

		public PvpTeamInfo BattleResult
		{
			get;
			set;
		}

		public TeamType? WinTeam
		{
			get;
			set;
		}

		public bool IsOtherCancelConfirm
		{
			get;
			set;
		}

		public long MySummId
		{
			get
			{
				ReadyPlayerSampleInfo myPlayerInfo = this.GetMyPlayerInfo();
				return (myPlayerInfo != null) ? myPlayerInfo.SummerId : 0L;
			}
		}

		public RoomInfo()
		{
			this.BattleResult = null;
			this.MyUserId = 0;
			this.SelfTeam = TeamType.None;
			this.IsRoomReady = false;
			this.WinTeam = null;
			this.IsOtherCancelConfirm = false;
		}

		public void SetPlayers(int roomId, int myUserId, ReadyPlayerSampleInfo[] players, string summonerIdObserverd)
		{
			this.RoomId = roomId;
			this.MyUserId = myUserId;
			this.SummIdObserved = summonerIdObserverd;
			this.WinTeam = null;
			this.IsRoomReady = true;
			this._heroExtras.Clear();
			for (int i = 0; i < players.Length; i++)
			{
				this._heroExtras[players[i].newUid] = new HeroExtraInRoom();
			}
			this._pvpPlayers.Clear();
			this._pvpPlayers.AddRange(players);
			ReadyPlayerSampleInfo readyPlayerSampleInfo;
			if (myUserId == -2147483648)
			{
				readyPlayerSampleInfo = this._pvpPlayers.Find((ReadyPlayerSampleInfo x) => x.SummerId.ToString() == summonerIdObserverd);
			}
			else
			{
				readyPlayerSampleInfo = this._pvpPlayers.Find((ReadyPlayerSampleInfo x) => x.newUid == myUserId);
			}
			if (readyPlayerSampleInfo != null)
			{
				TeamType team = readyPlayerSampleInfo.GetTeam();
				if (team == TeamType.Neutral)
				{
					this.MyUserId = -2147483648;
					this.SelfTeam = TeamType.LM;
				}
				else
				{
					this.SelfTeam = team;
				}
			}
			else
			{
				ClientLogger.Error("cannot found related playerinfo");
			}
		}

		public void NewbieSetInfo(int inMyUserId, ReadyPlayerSampleInfo[] inPlayerInfos)
		{
			this.IsRoomReady = true;
			this.MyUserId = inMyUserId;
			this._pvpPlayers.Clear();
			this._pvpPlayers.AddRange(inPlayerInfos);
			for (int i = 0; i < this._pvpPlayers.Count; i++)
			{
				ReadyPlayerSampleInfo readyPlayerSampleInfo = this._pvpPlayers[i];
				if (readyPlayerSampleInfo != null)
				{
					readyPlayerSampleInfo.selfDefSkillId = "1";
				}
			}
			this.NewbieSetUserNameForRobot();
			ReadyPlayerSampleInfo readyPlayerSampleInfo2 = this._pvpPlayers.Find((ReadyPlayerSampleInfo x) => x.newUid == inMyUserId);
			if (readyPlayerSampleInfo2 != null)
			{
				this.SelfTeam = readyPlayerSampleInfo2.GetTeam();
			}
			else
			{
				ClientLogger.Error("NewbieSetInfo cannot found related playerinfo");
			}
		}

		private void NewbieSetUserNameForRobot()
		{
			string text = string.Empty;
			string text2 = string.Empty;
			for (int i = 0; i < this._pvpPlayers.Count; i++)
			{
				ReadyPlayerSampleInfo readyPlayerSampleInfo = this._pvpPlayers[i];
				if (readyPlayerSampleInfo != null)
				{
					if (string.IsNullOrEmpty(readyPlayerSampleInfo.userName))
					{
						if (readyPlayerSampleInfo.heroInfo != null)
						{
							text = readyPlayerSampleInfo.heroInfo.heroId;
							if (!string.IsNullOrEmpty(text))
							{
								SysHeroMainVo heroMainData = BaseDataMgr.instance.GetHeroMainData(text);
								if (heroMainData != null)
								{
									text2 = heroMainData.name;
									if (!string.IsNullOrEmpty(text2))
									{
										SysLanguageVo languageData = BaseDataMgr.instance.GetLanguageData(text2);
										if (languageData != null)
										{
											readyPlayerSampleInfo.userName = languageData.content + "(电脑)";
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void SetRoomReady(bool inIsReady)
		{
			this.IsRoomReady = inIsReady;
		}

		public void SetMyUserId(int inUserId)
		{
			this.MyUserId = inUserId;
		}

		public void SetSelfTeam(TeamType team)
		{
			this.SelfTeam = team;
		}

		public ReadyPlayerSampleInfo GetPlayerInfoByUserId(int userId)
		{
			return this._pvpPlayers.Find((ReadyPlayerSampleInfo x) => x != null && x.newUid == userId);
		}

		public ReadyPlayerSampleInfo GetMyPlayerInfo()
		{
			return this.GetPlayerInfoByUserId(this.MyUserId);
		}

		public HeroExtraInRoom GetHeroExtraByUserId(int userId)
		{
			HeroExtraInRoom result;
			this._heroExtras.TryGetValue(userId, out result);
			return result;
		}

		public void UpdateAllLoadOk()
		{
			foreach (KeyValuePair<int, HeroExtraInRoom> current in this._heroExtras)
			{
				if (current.Key != this.MyUserId)
				{
					current.Value.LoadProgress = 101;
				}
			}
		}

		public void UpdateAllLoadProgress(Dictionary<int, byte> dict)
		{
			if (dict == null)
			{
				ClientLogger.Error("UpdateAllLoadProgress: argument is null");
				return;
			}
			foreach (KeyValuePair<int, byte> current in dict)
			{
				if (this._heroExtras.ContainsKey(current.Key))
				{
					this._heroExtras[current.Key].LoadProgress = (int)current.Value;
				}
				else
				{
					ClientLogger.Error("UpdateAllLoadProgress: cannot found " + current.Key);
				}
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"my team is lm: ",
				this.SelfTeam,
				" SummonerIdObserverd: ",
				this.SummIdObserved
			});
		}
	}
}
