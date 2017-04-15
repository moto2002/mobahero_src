using System;

namespace Com.Game.Data
{
	[Serializable]
	public class SysHeroProficiencyPointsVo
	{
		public string unikey;

		public int id;

		public float battle_damage;

		public int type;

		public int failure_bonus_point;

		public int victory_bonus_point;

		public string evaluate;
	}
}
