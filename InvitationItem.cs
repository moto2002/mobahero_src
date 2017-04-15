using Com.Game.Module;
using MobaHeros.Pvp;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class InvitationItem : MonoBehaviour
{
	public UILabel title;

	public UILabel timeLabel;

	public UILabel ContentLabel;

	public GameObject cancelBtn;

	public GameObject okBtn;

	public string targetSummerId;

	public string roomId;

	public float timeValue;

	public InvitateType type;

	private CoroutineManager coroutineManager = new CoroutineManager();

	private Task task;

	private void Start()
	{
		UIEventListener expr_10 = UIEventListener.Get(this.okBtn.gameObject);
		expr_10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_10.onClick, new UIEventListener.VoidDelegate(this.OnClickOk));
		UIEventListener expr_41 = UIEventListener.Get(this.cancelBtn.gameObject);
		expr_41.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_41.onClick, new UIEventListener.VoidDelegate(this.OnClickCancel));
		this.task = this.coroutineManager.StartCoroutine(this.Timer(20f), true);
		AudioMgr.PlayUI("Play_Menu_Invite", null, false, false);
	}

	private void OnClickCancel(GameObject go)
	{
		Singleton<InvitationView>.Instance.GetResult(this.roomId, false, this.targetSummerId, this.type);
		this.task.Stop();
		UnityEngine.Object.DestroyImmediate(base.gameObject);
		Singleton<InvitationView>.Instance.FixGrid();
	}

	private void OnClickOk(GameObject go)
	{
		if (this.type == InvitateType.KHOrZDY && Singleton<PvpRoomView>.Instance.IsOpen)
		{
			Singleton<TipView>.Instance.ShowViewSetText("您需要离开此房间，才能加入他的游戏", 1f);
			return;
		}
		this.task.Stop();
		Singleton<InvitationView>.Instance.FixGrid();
		if (this.type == InvitateType.KHOrZDY)
		{
			if (PvpMatchMgr.State == PvpMatchState.Matching || PvpMatchMgr.State == PvpMatchState.Matched)
			{
				Singleton<InvitationView>.Instance.GetResult(this.roomId, false, this.targetSummerId, this.type);
				Singleton<TipView>.Instance.ShowViewSetText("您正在匹配，无法接受好友开黑邀请", 1f);
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			CtrlManager.DestroyAllWindowsExcept(new WindowID[]
			{
				WindowID.MenuView,
				WindowID.MainBg,
				WindowID.MenuTopBarView,
				WindowID.MenuBottomBarView,
				WindowID.ArenaModeView
			});
		}
		Singleton<InvitationView>.Instance.GetResult(this.roomId, true, this.targetSummerId, this.type);
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	[DebuggerHidden]
	private IEnumerator Timer(float value)
	{
		InvitationItem.<Timer>c__Iterator139 <Timer>c__Iterator = new InvitationItem.<Timer>c__Iterator139();
		<Timer>c__Iterator.value = value;
		<Timer>c__Iterator.<$>value = value;
		<Timer>c__Iterator.<>f__this = this;
		return <Timer>c__Iterator;
	}

	private void Update()
	{
	}
}
