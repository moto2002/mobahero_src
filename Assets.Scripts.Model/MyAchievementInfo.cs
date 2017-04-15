using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class MyAchievementInfo
	{
		public KDAData kdaData = new KDAData();

		public HistoryBattleData historyBattle = new HistoryBattleData();

		public List<KdaMyHeroData> myHero = new List<KdaMyHeroData>();

		public KdaUserTopData myTopData = new KdaUserTopData();

		public List<KdaUserHonorData> myHonorData = new List<KdaUserHonorData>();

		public HomeKDAData myHomeKDA = new HomeKDAData();

		public List<heroRecordInfo> myHeroRecordList = new List<heroRecordInfo>();

		public Dictionary<long, heroRecordInfo> myHeroRecord = new Dictionary<long, heroRecordInfo>();

		public KdaAbilityGraph abilityGraph = new KdaAbilityGraph();

		public bool HaveFight = true;

		public long totalpvpLogId;

		public long abilitypvpLogId;

		public long heropvpLogId;

		public long battleLogId;

		public Dictionary<long, HistoryBattleData> HistoryBattleDataDic = new Dictionary<long, HistoryBattleData>();
	}
}
