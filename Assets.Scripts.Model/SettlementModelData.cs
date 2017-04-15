using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class SettlementModelData
	{
		public List<EquipmentInfoData> equipRtnData = new List<EquipmentInfoData>();

		public List<HeroInfoData> heroRtnData = new List<HeroInfoData>();

		public List<DropItemData> dropRtnData = new List<DropItemData>();

		public List<DropItemData> repeatRtnData = new List<DropItemData>();

		public int summonerExpRecord_level;

		public int summonerExpRecord_curExp;

		public int summonerExp_Delta;

		public int summonerExp_curLevelExpRequired;

		public int bottleExpRecord_level;

		public int bottleExpRecord_curExp;

		public int bottleExp_Delta;

		public byte levelScore;

		public int proficiencyExpRecord_curExp;

		public int proficiencyExpRecord_Delta;

		public int coin_Delta;

		public string battleUseTime = "N/A";

		public PvpTeamInfo pvp_teamInfo;

		public List<BattlesModel> pve_battlesInfo = new List<BattlesModel>();

		public bool isGetMsg;
	}
}
