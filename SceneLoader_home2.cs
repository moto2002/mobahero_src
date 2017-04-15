using Assets.Scripts.Model;
using Com.Game.Module;
using MobaHeros.Gate;
using MobaHeros.Pvp;
using MobaMessageData;
using Newbie;
using System;

internal class SceneLoader_home2 : SceneLoader
{
	public SceneLoader_home2(SceneType s, SceneManager m, string strName) : base(s, m, strName)
	{
	}

	public override void Unload()
	{
		base.Unload();
		HomeManager.Instance.Enable(false);
		this.SendTargetProgress(5);
		base.SetGoOn();
	}

	public override void ClearViews()
	{
		base.ClearViews();
		if (NewbieManager.Instance.IsDoNewbieSpecialProcess())
		{
			CtrlManager.DestroyAllWindowsExcept(new WindowID[]
			{
				WindowID.NewbieLoadView
			});
		}
		else
		{
			CtrlManager.DestroyAllWindows();
			ResourceManager.UnLoadAllAssets(false);
		}
		base.SetGoOn();
	}

	public override void BeginLoad()
	{
		base.BeginLoad();
		this.SendTargetProgress(15);
		HomeAtlasPreloader.Load();
		base.SetGoOn();
	}

	public override void LoadScene()
	{
		base.LoadScene();
		base.SetGoOn();
	}

	public override void SetCamera()
	{
		base.SetCamera();
		base.SetGoOn();
	}

	public override void SetFPS()
	{
		base.SetFPS();
		this.SendTargetProgress(85);
		base.SetGoOn();
	}

	public override void SetManager()
	{
		base.SetManager();
		MobaMessageManager.RegistMessage(ClientC2C.SceneManagerReady, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		MobaMessageManager.RegistMessage(ClientC2C.GateConnected, new MobaMessageFunc(this.OnGateAvailable));
		GateReconnection.CanTrigger = true;
		NetWorkHelper.Instance.DisconnectFromMasterServer();
		NetWorkHelper.Instance.GateReconnection.Begin();
	}

	private void OnGateAvailable(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage(ClientC2C.GateConnected, new MobaMessageFunc(this.OnGateAvailable));
		HomeManager.Instance.Enable(true);
	}

	public override void OpenViews()
	{
		MobaMessageManager.RegistMessage(ClientV2C.LoadView2_subComplete, new MobaMessageFunc(this.OnMsg_LoadView_complete));
		this.SendTargetProgress(100);
	}

	public override void LoadSceneEnd()
	{
		base.LoadSceneEnd();
		base.SendSceneLoadComplete();
		this.OpenHomeViews();
		base.SetGoOn();
	}

	private void OnMsg_SceneManagerReady(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage(ClientC2C.SceneManagerReady, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		base.SetGoOn();
	}

	private void OnMsg_LoadView_complete(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage(ClientV2C.LoadView2_subComplete, new MobaMessageFunc(this.OnMsg_LoadView_complete));
		CtrlManager.CloseWindow(WindowID.LoadView2);
		PlayerMng.Instance.Enable(false);
		base.SetGoOn();
	}

	protected override void SendTargetProgress(int targetNum)
	{
		MobaMessageManagerTools.LoadView2_setProgress2(targetNum, -1, -1, null, MsgData_LoadView2_setProgress.SetType.targetNum);
	}

	protected override void SendAddProgress(int addNum)
	{
		MobaMessageManagerTools.LoadView2_setProgress2(addNum, -1, -1, null, MsgData_LoadView2_setProgress.SetType.addNum);
	}

	private void OpenHomeViews()
	{
		HomeGCManager.Instance.DoClearToHome();
		base.OpenViews();
		CtrlManager.CloseWindow(WindowID.BgView);
		CtrlManager.OpenWindow(WindowID.MainBg, null);
		CtrlManager.OpenWindow(WindowID.MenuView, null);
		CtrlManager.OpenWindow(WindowID.MenuBottomBarView, null);
		CtrlManager.OpenWindow(WindowID.MenuTopBarView, null);
		if (LevelManager.CurBattleType != 0)
		{
			if (LevelManager.CurBattleType == 6)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				CtrlManager.OpenWindow(WindowID.CardView, null);
			}
			else if (LevelManager.CurBattleType == 9)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				CtrlManager.OpenWindow(WindowID.XLDView, null);
			}
			else if (LevelManager.Instance.IsZyBattleType || LevelManager.CurBattleType == 2)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				if (LevelManager.Instance.IsServerZyBattleType)
				{
					Singleton<PveManager>.Instance.ResetState();
					PvpManager.ResetState();
				}
			}
			else if (LevelManager.Instance.IsPvpBattleType)
			{
				if (!Singleton<PvpManager>.Instance.IsObserver)
				{
					CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
				}
				PvpManager.ResetState();
			}
			else if (LevelManager.CurBattleType != 11)
			{
				CtrlManager.OpenWindow(WindowID.ArenaModeView, null);
			}
		}
		PvpLevelStorage.CheckFightAgain();
		TimeSpan t = ModelManager.Instance.Get_loginTime_diff_X();
		DateTime d = ModelManager.Instance.Get_loginTime_DataTime();
		DateTime dateTime = d + t;
		bool flag = true;
		if (dateTime.Day == d.Day)
		{
			int isPass = ModelManager.Instance.Get_GetSignDay_X().isPass;
			if (isPass == 1)
			{
				CtrlManager.OpenWindow(WindowID.SignView, null);
				flag = false;
			}
		}
		else
		{
			Singleton<MenuView>.Instance.SetNews(6, "0");
		}
		if (flag)
		{
			this.OpenActivityView();
		}
		else
		{
			MobaMessageManager.RegistMessage(ClientV2C.signView_close, new MobaMessageFunc(this.OnMsg_OnSignViewClose));
		}
		Singleton<MenuView>.Instance.OpenSummonerRegisterView();
	}

	private void OpenActivityView()
	{
		if (NewbieManager.Instance.IsInNewbieGuide())
		{
			return;
		}
		CtrlManager.OpenWindow(WindowID.ActivityView, null);
		MobaMessageManagerTools.SendClientMsg(ClientC2V.ShowActivityNotice, null, false);
	}

	private void OnMsg_OnSignViewClose(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage(ClientV2C.signView_close, new MobaMessageFunc(this.OnMsg_OnSignViewClose));
		this.OpenActivityView();
		Singleton<MenuView>.Instance.RemoveNews(6, "0");
	}
}
