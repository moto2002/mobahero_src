using Com.Game.Module;
using Com.Game.Utils;
using MobaHeros.Pvp;
using MobaHeros.Replay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Assets.Scripts.GUILogic.View.GameVideo
{
	public class GameVideo_LocalRecordPage : MonoBehaviour
	{
		public GameVideo_LocalRecordItem ItemTemplate;

		public UIGrid ItemGrid;

		public GameObject DeleteButton;

		public UILabel ButtonLabel;

		public GameObject NoDataTip;

		public GameObject Loading;

		public UILabel LoadingLabel;

		[HideInInspector]
		public bool IsActive;

		private bool deleteBtnFlag;

		private void OnEnable()
		{
			this.deleteBtnFlag = false;
			this.ResetPage();
			this.Loading.SetActive(false);
			UIEventListener.Get(this.DeleteButton.gameObject).onClick = new UIEventListener.VoidDelegate(this.OnClick_Delete);
			MobaMessageManager.RegistMessage((ClientMsg)26032, new MobaMessageFunc(this.OnMsg_ShowARecord));
			MobaMessageManager.RegistMessage((ClientMsg)26031, new MobaMessageFunc(this.OnMsg_DeleteARecord));
		}

		private void OnDisable()
		{
			base.StopAllCoroutines();
			MobaMessageManager.UnRegistMessage((ClientMsg)26032, new MobaMessageFunc(this.OnMsg_ShowARecord));
			MobaMessageManager.UnRegistMessage((ClientMsg)26031, new MobaMessageFunc(this.OnMsg_DeleteARecord));
		}

		public void ResetPage()
		{
			this.ButtonLabel.text = LanguageManager.Instance.GetStringById("LiveorPlayback_Playback003", "Edit");
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26030, false, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		private void OnClick_Delete(GameObject obj = null)
		{
			this.deleteBtnFlag = !this.deleteBtnFlag;
			this.ButtonLabel.text = ((!this.deleteBtnFlag) ? LanguageManager.Instance.GetStringById("LiveorPlayback_Playback003", "Edit") : LanguageManager.Instance.GetStringById("LiveorPlayback_Playback004", "Finish"));
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)26030, this.deleteBtnFlag, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		[DebuggerHidden]
		private IEnumerator DelayRefreshGrid()
		{
			GameVideo_LocalRecordPage.<DelayRefreshGrid>c__Iterator12C <DelayRefreshGrid>c__Iterator12C = new GameVideo_LocalRecordPage.<DelayRefreshGrid>c__Iterator12C();
			<DelayRefreshGrid>c__Iterator12C.<>f__this = this;
			return <DelayRefreshGrid>c__Iterator12C;
		}

		private void OnMsg_ShowARecord(MobaMessage msg)
		{
			if (PvpMatchMgr.State != PvpMatchState.None)
			{
				Singleton<TipView>.Instance.ShowViewSetText("正在匹配中，无法打开战斗回放", 1f);
				return;
			}
			if (!GameManager.Instance.ReplayController.IsInitMetaInfoSuc)
			{
				CtrlManager.ShowMsgBox("加载失败", "本地文件错误，尝试重新登录或清除软件缓存", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			int replayId = (int)msg.Param;
			GameManager.Instance.ReplayController.StartReplay(replayId);
			base.StartCoroutine(this.LoadingRing());
		}

		[DebuggerHidden]
		private IEnumerator LoadingRing()
		{
			GameVideo_LocalRecordPage.<LoadingRing>c__Iterator12D <LoadingRing>c__Iterator12D = new GameVideo_LocalRecordPage.<LoadingRing>c__Iterator12D();
			<LoadingRing>c__Iterator12D.<>f__this = this;
			return <LoadingRing>c__Iterator12D;
		}

		private void OnMsg_DeleteARecord(MobaMessage msg)
		{
			GameVideo_LocalRecordItem gameVideo_LocalRecordItem = msg.Param as GameVideo_LocalRecordItem;
			if (gameVideo_LocalRecordItem == null)
			{
				return;
			}
			if (!GameManager.Instance.ReplayController.DeleteFileByReplayId(gameVideo_LocalRecordItem.ReplayId))
			{
				CtrlManager.ShowMsgBox("删除失败", "本地文件错误，尝试重新登录或清除软件缓存", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
				return;
			}
			UnityEngine.Object.Destroy(gameVideo_LocalRecordItem.gameObject);
			base.StartCoroutine(this.DelayRefreshGrid());
		}

		public int ReplayMetaEntryComparator(ReplayMetaInfo.ReplayMetaEntry a, ReplayMetaInfo.ReplayMetaEntry b)
		{
			return b.Time.CompareTo(a.Time);
		}

		private void InitRecordItems()
		{
			if (!GameManager.Instance.ReplayController.IsInitMetaInfoSuc)
			{
				CtrlManager.ShowMsgBox("加载失败", "本地文件错误，尝试重新登录或清除软件缓存", delegate
				{
				}, PopViewType.PopOneButton, "确定", "取消", null);
			}
			else
			{
				ReplayMetaInfo replayMetaInfo = GameManager.Instance.ReplayController.GetReplayMetaInfo();
				if (replayMetaInfo != null)
				{
					List<ReplayMetaInfo.ReplayMetaEntry> _replayList = replayMetaInfo.ReplayEntryList;
					_replayList.Sort(new Comparison<ReplayMetaInfo.ReplayMetaEntry>(this.ReplayMetaEntryComparator));
					GridHelper.FillGrid<GameVideo_LocalRecordItem>(this.ItemGrid, this.ItemTemplate, _replayList.Count, delegate(int index, GameVideo_LocalRecordItem comp)
					{
						comp.SetData(_replayList[index]);
						comp.gameObject.SetActive(true);
					});
				}
			}
			base.StartCoroutine(this.DelayRefreshGrid());
		}

		public void SetActive(bool _isActive)
		{
			this.IsActive = _isActive;
			base.gameObject.SetActive(_isActive);
			if (!_isActive)
			{
				return;
			}
			this.InitRecordItems();
		}
	}
}
