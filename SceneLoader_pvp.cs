using Com.Game.Module;
using MobaHeros.Pvp;
using Newbie;
using System;
using UnityEngine;

internal class SceneLoader_pvp : SceneLoader
{
	private bool allPlayerReady;

	private bool selfReady;

	private bool loadingFinish;

	public SceneLoader_pvp(SceneType s, SceneManager m, string strName) : base(s, m, strName)
	{
	}

	public override void Unload()
	{
		AudioMgr.loadSoundBank_UI();
		AudioMgr.unloadSoundBank_INGAME();
		if (LevelManager.m_CurLevel.Is3V3V3())
		{
			AudioMgr.unloadSoundBank_3V3V3();
		}
		base.Unload();
		GlobalObject.Instance.EnableGameManager(false);
		this.SendAddProgress(5);
		if (NewbieManager.Instance.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_BeginLoad, false, ENewbieStepType.None);
		}
		base.SetGoOn();
	}

	public override void ClearViews()
	{
		base.ClearViews();
		CtrlManager.DestroyAllWindows();
		ResourceManager.UnLoadAllAssets(false);
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void BeginLoad()
	{
		base.BeginLoad();
		if (ChaosFightMgr.IsChaosFight)
		{
			CtrlManager.OpenWindow(WindowID.PVPV3LoadView, null);
		}
		else
		{
			CtrlManager.OpenWindow(WindowID.PVPLoadingView, null);
		}
		this.SendTargetProgress(10);
		base.SetGoOn();
	}

	public override void LoadScene()
	{
		HomeGCManager.Instance.DoClearToBattleLoading();
		AudioMgr.unloadSoundBank_UI();
		AudioMgr.loadSoundBank_INGAME();
		if (LevelManager.m_CurLevel.Is3V3V3())
		{
			AudioMgr.loadSoundBank3V3V3();
		}
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
		if (GlobalSettings.Instance.isHighFPS)
		{
			CameraRoot.SetTargetFPS(60);
		}
		else
		{
			CameraRoot.SetTargetFPS(30);
		}
		this.SendAddProgress(70);
		base.SetGoOn();
	}

	public override void SetManager()
	{
		base.SetManager();
		MobaMessageManager.RegistMessage(ClientC2C.SceneManagerReady, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		MobaMessageManager.RegistMessage(ClientC2C.SceneManagerLoadComplete, new MobaMessageFunc(this.OnMsg_SceneManagerLoadComplete));
		MobaMessageManager.RegistMessage(ClientV2C.PVPLoadView_complete, new MobaMessageFunc(this.OnMsg_PVPLoadView_complete));
		GlobalObject.Instance.EnableGameManager(true);
		GameManager.Instance.StartGame();
		this.SendAddProgress(5);
	}

	public override void OpenViews()
	{
		base.OpenViews();
		base.SetGoOn();
	}

	public override void LoadSceneEnd()
	{
		if (ChaosFightMgr.IsChaosFight)
		{
			CtrlManager.CloseWindow(WindowID.PVPV3LoadView);
		}
		else
		{
			CtrlManager.CloseWindow(WindowID.PVPLoadingView);
		}
		base.LoadSceneEnd();
		base.SendSceneLoadComplete();
		base.SetGoOn();
		if (NewbieManager.Instance.IsDoNewbieSpecialProcess())
		{
			NewbieManager.Instance.MoveNextStep();
		}
		else if (NewbieManager.Instance.IsInNewbiePhase(ENewbiePhaseType.FakeMatchFive))
		{
			NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_TriggerChecker, false, ENewbieStepType.None);
		}
		else if (NewbieManager.Instance.IsGuideNewbieNormalCastSkill())
		{
			NewbieManager.Instance.EnterGuideNewbieNormalCastSkill();
		}
	}

	private void OnMsg_SceneManagerReady(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage(ClientC2C.SceneManagerReady, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		this.selfReady = true;
		Debug.Log("*********ClientC2C.SceneManagerReady");
		this.SendTargetProgress(100);
		if (this.selfReady && this.allPlayerReady && this.loadingFinish)
		{
			base.SetGoOn();
		}
	}

	private void OnMsg_SceneManagerLoadComplete(MobaMessage msg)
	{
		Debug.Log("*********ClientC2C.SceneManagerLoadComplete");
		MobaMessageManager.UnRegistMessage(ClientC2C.SceneManagerLoadComplete, new MobaMessageFunc(this.OnMsg_SceneManagerLoadComplete));
		this.allPlayerReady = true;
		if (this.selfReady && this.allPlayerReady && this.loadingFinish)
		{
			base.SetGoOn();
		}
	}

	private void OnMsg_PVPLoadView_complete(MobaMessage msg)
	{
		Debug.Log("*********ClientC2C.PVPLoadView_complete");
		MobaMessageManager.UnRegistMessage(ClientV2C.PVPLoadView_complete, new MobaMessageFunc(this.OnMsg_PVPLoadView_complete));
		Singleton<SkillView>.Instance.SetGameObjectActive(true);
		this.loadingFinish = true;
		if (this.selfReady && this.allPlayerReady && this.loadingFinish)
		{
			base.SetGoOn();
		}
	}
}
