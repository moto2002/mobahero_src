using GUIFramework;
using MobaHeros.Pvp;
using MobaProtocol.Data;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class SurrenderView : BaseView<SurrenderView>
	{
		private SurrenderComps _comps;

		private Task _timeoutTask;

		private Task _closeTask;

		private ActiveSurrenderInfo info;

		public SurrenderView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Battle/SurrenderView");
		}

		public override void RegisterUpdateHandler()
		{
			MobaMessageManager.RegistMessage((ClientMsg)23064, new MobaMessageFunc(this.OnUpdateSurrenderInfo));
		}

		public override void CancelUpdateHandler()
		{
			MobaMessageManager.UnRegistMessage((ClientMsg)23064, new MobaMessageFunc(this.OnUpdateSurrenderInfo));
		}

		public override void Init()
		{
			base.Init();
			this._comps = this.uiWindow.gameObj.GetComponent<SurrenderComps>();
			this._comps.OnConfirmSurrender = new Action<bool>(this.OnConfirmSurrender);
			this._comps.ShowResultPanel(false);
			this._comps.ShowVotePanel(false);
			this.Refresh();
		}

		private void OnConfirmSurrender(bool ok)
		{
			GameManager.Instance.SurrenderMgr.VoteSurrender(ok);
		}

		public override void HandleAfterOpenView()
		{
			this.SetPosition(Singleton<HUDModuleManager>.Instance.skillPanelPivot == SkillPanelPivot.Right);
		}

		public override void HandleBeforeCloseView()
		{
			this.StopTimer();
		}

		public void SetPosition(bool isRightAnchor)
		{
			if (isRightAnchor)
			{
				this.transform.FindChild("Anchor_BottomRight").GetComponent<UIAnchor>().relativeOffset = new Vector2(-0.07f, 0f);
			}
		}

		private void StopTimer()
		{
			if (this._timeoutTask != null)
			{
				this._timeoutTask.Stop();
			}
		}

		private void Refresh()
		{
			if (Singleton<PvpManager>.Instance.OurPlayers.Count <= 1)
			{
				this._comps.ShowVotePanel(false);
				this._comps.ShowVotePanel(false);
				return;
			}
			SurrenderMgr surrenderMgr = GameManager.Instance.SurrenderMgr;
			this._comps.SetMemberCount(Singleton<PvpManager>.Instance.OurPlayers.Count);
			this._comps.SetCanSurrender(surrenderMgr.CanCurPlayerVote());
			this._comps.SetMemberStates(surrenderMgr.GetTeamMemberStates());
			this._comps.RatioSlider.value = (float)surrenderMgr.AcceptCount / (float)surrenderMgr.OurTeamCount;
			int num = Mathf.CeilToInt((float)surrenderMgr.OurTeamCount * 0.7f);
			int num2 = num - surrenderMgr.AcceptCount;
			if (num2 <= 0)
			{
				this._comps.TipLabel.text = string.Empty;
			}
			else
			{
				this._comps.TipLabel.text = LanguageManager.Instance.FormatString("BattleSurrenderUI_Content_SurrenderNumber", new object[]
				{
					num2
				});
			}
			float leftAliveTime = surrenderMgr.GetLeftAliveTime();
			if (leftAliveTime < 0f || surrenderMgr.HasAllVoted)
			{
				this.StopTimer();
				this.ShowResult();
			}
			else
			{
				this._comps.ShowVotePanel(true);
				this._comps.ShowResultPanel(false);
				if (this._timeoutTask == null)
				{
					this._timeoutTask = new Task(this.TimerEnumerator(leftAliveTime), false);
					this._timeoutTask.Finished += delegate(bool manual)
					{
						this._timeoutTask = null;
						if (!manual)
						{
							this.ShowResult();
						}
					};
					this._timeoutTask.Start();
				}
			}
		}

		private void ShowResult()
		{
			if (GameManager.Instance == null)
			{
				return;
			}
			SurrenderMgr surrenderMgr = GameManager.Instance.SurrenderMgr;
			if (surrenderMgr == null)
			{
				return;
			}
			int num = Mathf.CeilToInt((float)surrenderMgr.ValidVoters * 0.7f);
			if (num <= surrenderMgr.AcceptCount)
			{
				CtrlManager.CloseWindow(WindowID.SurrenderView);
				return;
			}
			this._comps.ShowVotePanel(false);
			this._comps.ShowResultPanel(true);
			string resultText = string.Empty;
			int acceptCount = surrenderMgr.AcceptCount;
			int num2 = surrenderMgr.Votes - surrenderMgr.AcceptCount;
			int num3 = surrenderMgr.OurTeamCount - surrenderMgr.Votes;
			string format = "投票结果{0}票同意 {1}票拒绝 {2}票弃权 投降失败";
			resultText = string.Format(format, acceptCount, num2, num3);
			this._comps.SetResultText(resultText);
			if (this._closeTask == null)
			{
				this._closeTask = new Task(this.TimerEnumerator(4f), false);
				this._closeTask.Finished += delegate(bool manual)
				{
					this._closeTask = null;
					CtrlManager.CloseWindow(WindowID.SurrenderView);
				};
				this._closeTask.Start();
			}
		}

		[DebuggerHidden]
		private IEnumerator TimerEnumerator(float time)
		{
			SurrenderView.<TimerEnumerator>c__Iterator16B <TimerEnumerator>c__Iterator16B = new SurrenderView.<TimerEnumerator>c__Iterator16B();
			<TimerEnumerator>c__Iterator16B.time = time;
			<TimerEnumerator>c__Iterator16B.<$>time = time;
			return <TimerEnumerator>c__Iterator16B;
		}

		private void OnUpdateSurrenderInfo(MobaMessage msg)
		{
			this.info = (msg.Param as ActiveSurrenderInfo);
			this.Refresh();
		}
	}
}
