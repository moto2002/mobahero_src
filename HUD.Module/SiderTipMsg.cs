using MobaHeros.Pvp;
using System;
using System.Collections.Generic;

namespace HUD.Module
{
	public class SiderTipMsg
	{
		public bool isTeamSign;

		public TeamSignalType signType;

		public bool isAllyTip;

		public string killerSpriteName;

		public string victimSpriteName;

		public List<string> assistSpriteName = new List<string>();

		public TeamType attackerTeam;

		public TeamType victimTeam;

		public SiderTipMsg(string _sender, TeamSignalType _signType)
		{
			this.isTeamSign = true;
			this.signType = _signType;
			this.killerSpriteName = _sender;
		}

		public SiderTipMsg(string _killer, string _victim, List<string> _assist, bool _isAllyTip, TeamType _attackerTeam, TeamType _victimTeam)
		{
			this.isTeamSign = false;
			this.killerSpriteName = _killer;
			this.victimSpriteName = _victim;
			this.assistSpriteName = _assist;
			this.isAllyTip = _isAllyTip;
			this.attackerTeam = _attackerTeam;
			this.victimTeam = _victimTeam;
		}
	}
}
