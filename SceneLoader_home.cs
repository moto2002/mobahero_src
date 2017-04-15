using Assets.Scripts.Model;
using Com.Game.Module;
using MobaHeros.Pvp;
using Newbie;
using System;

internal class SceneLoader_home : SceneLoader
{
	public SceneLoader_home(SceneType s, SceneManager m, string strName) : base(s, m, strName)
	{
	}

	public override void Unload()
	{
		base.Unload();
		HomeManager.Instance.Enable(false);
		this.SendAddProgress(5);
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
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void BeginLoad()
	{
		base.BeginLoad();
		CtrlManager.OpenWindow(WindowID.LoadView, null);
		this.SendTargetProgress(10);
		HomeAtlasPreloader.Load();
		base.SetGoOn();
	}

	public override void LoadScene()
	{
		base.LoadScene();
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void SetCamera()
	{
		base.SetCamera();
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void SetFPS()
	{
		base.SetFPS();
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void SetManager()
	{
		base.SetManager();
		MobaMessageManager.RegistMessage((ClientMsg)25011, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		HomeManager.Instance.Enable(true);
		this.SendAddProgress(35);
	}

	public override void OpenViews()
	{
		this.OpenHomeViews();
		this.SendAddProgress(10);
		this.SendTargetProgress(100);
		MobaMessageManager.RegistMessage((ClientMsg)21009, new MobaMessageFunc(this.OnMsg_LoadView_complete));
	}

	private void OpenHomeViews()
	{
		HomeGCManager.Instance.DoClearToHome();
		base.OpenViews();
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
	}

	public override void LoadSceneEnd()
	{
		base.LoadSceneEnd();
		base.SendSceneLoadComplete();
		base.SetGoOn();
	}

	private void OnMsg_SceneManagerReady(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25011, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		base.SetGoOn();
	}

	private void OnMsg_LoadView_complete(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)21009, new MobaMessageFunc(this.OnMsg_LoadView_complete));
		TimeSpan t = ModelManager.Instance.Get_loginTime_diff_X();
		DateTime d = ModelManager.Instance.Get_loginTime_DataTime();
		if ((d + t).Day == d.Day)
		{
			int isPass = ModelManager.Instance.Get_GetSignDay_X().isPass;
			if (isPass == 1)
			{
				CtrlManager.OpenWindow(WindowID.SignView, null);
			}
		}
		else
		{
			Singleton<MenuView>.Instance.SetNews(6, "0");
		}
		Singleton<MenuView>.Instance.OpenSummonerRegisterView();
		CtrlManager.CloseWindow(WindowID.LoadView);
		base.SetGoOn();
	}
}
