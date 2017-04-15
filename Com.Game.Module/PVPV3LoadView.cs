using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class PVPV3LoadView : BaseView<PVPV3LoadView>
	{
		private UIGrid grid_team;

		private UILabel lb_noticeLeft;

		private UILabel lb_noticeRight;

		private UILabel lb_noticeRight2;

		private V3Team template_team_bl;

		private V3Team template_team_lm;

		private V3Team template_team_team3;

		private Dictionary<TeamType, V3Team> dicComs;

		private List<TeamType> listTeamType;

		private int teamFinishCounter;

		public PVPV3LoadView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/PVPLoading/PVPLoadingV3");
		}

		public override void Init()
		{
			base.Init();
			this.template_team_bl = Resources.Load<V3Team>("Prefab/UI/PVPLoading/V3Team_bl");
			this.template_team_lm = Resources.Load<V3Team>("Prefab/UI/PVPLoading/V3Team_lm");
			this.template_team_team3 = Resources.Load<V3Team>("Prefab/UI/PVPLoading/V3Team_team3");
			this.grid_team = this.transform.FindChild("center/dragpanel/grid").GetComponent<UIGrid>();
			this.lb_noticeLeft = this.transform.FindChild("top/notice_left").GetComponent<UILabel>();
			this.lb_noticeRight = this.transform.FindChild("top/notice_right").GetComponent<UILabel>();
			this.lb_noticeRight2 = this.transform.FindChild("top/notice_right2").GetComponent<UILabel>();
			this.listTeamType = new List<TeamType>
			{
				TeamType.LM,
				TeamType.BL,
				TeamType.Team_3
			};
			this.dicComs = new Dictionary<TeamType, V3Team>();
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			base.HandleAfterOpenView();
			this.lb_noticeLeft.text = LanguageManager.Instance.GetStringById("Loading_Tips");
			this.lb_noticeRight.text = LanguageManager.Instance.GetStringById("3v3v3_Victory");
			this.lb_noticeRight2.text = LanguageManager.Instance.GetStringById("3v3v3_VictoryTips");
			this.RefreshTeams();
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void RefreshTeams()
		{
			this.teamFinishCounter = 0;
			for (int i = 0; i < this.listTeamType.Count; i++)
			{
				TeamType teamType = this.listTeamType[i];
				V3Team v3Team = null;
				switch (teamType)
				{
				case TeamType.LM:
					v3Team = this.template_team_lm;
					break;
				case TeamType.BL:
					v3Team = this.template_team_bl;
					break;
				case TeamType.Team_3:
					v3Team = this.template_team_team3;
					break;
				}
				GameObject gameObject = NGUITools.AddChild(this.grid_team.gameObject, v3Team.gameObject);
				V3Team component = gameObject.GetComponent<V3Team>();
				component.TeamType_P = teamType;
				component.OnTeamLoadFinish = new Action<V3Team>(this.OnTeamLoadFinish);
				this.dicComs[teamType] = component;
				component.UpdateCom();
				component.gameObject.SetActive(true);
			}
			this.grid_team.Reposition();
		}

		private void OnTeamLoadFinish(V3Team v)
		{
			Debug.Log("Loading finish: team=" + v.TeamType_P);
			if (++this.teamFinishCounter == this.dicComs.Count)
			{
				Debug.Log("Loading finish: all team");
				MobaMessageManagerTools.SendClientMsg(ClientV2C.PVPLoadView_complete, null, true);
			}
		}
	}
}
