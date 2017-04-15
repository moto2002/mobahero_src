using GUIFramework;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class SettlementMaskView : BaseView<SettlementMaskView>
	{
		private const string EscapeText = "[u]跳过等待，直接返回";

		private const string WaitText = "[888888]请等待... (*)";

		private CoroutineManager cMgr;

		private bool isConnectedGameServer;

		private UILabel _escapeBtn;

		public SettlementMaskView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Victory/SettlementMaskView");
		}

		public override void Init()
		{
			base.Init();
			this.cMgr = new CoroutineManager();
			this._escapeBtn = this.transform.FindChild("BG/BtnLabel").GetComponent<UILabel>();
			UIEventListener.Get(this._escapeBtn.gameObject).onClick = null;
		}

		public override void HandleAfterOpenView()
		{
			if (Singleton<BattleSettlementView>.Instance.IsReplay)
			{
				Time.timeScale = 1f;
			}
			this.isConnectedGameServer = false;
			this.cMgr.StartCoroutine(this.Process(), true);
			this.cMgr.StartCoroutine(this.TimerToEscape(), true);
			this._escapeBtn.gameObject.SetActive(false);
		}

		public override void HandleBeforeCloseView()
		{
			if (this.cMgr != null)
			{
				this.cMgr.StopAllCoroutine();
				this.cMgr = null;
			}
		}

		[DebuggerHidden]
		private IEnumerator Process()
		{
			return new SettlementMaskView.<Process>c__Iterator100();
		}

		[DebuggerHidden]
		private IEnumerator TimerToEscape()
		{
			SettlementMaskView.<TimerToEscape>c__Iterator101 <TimerToEscape>c__Iterator = new SettlementMaskView.<TimerToEscape>c__Iterator101();
			<TimerToEscape>c__Iterator.<>f__this = this;
			return <TimerToEscape>c__Iterator;
		}

		private void OnClickEscape(GameObject obj = null)
		{
			Singleton<BattleSettlementView>.Instance.BackToLobby();
		}
	}
}
