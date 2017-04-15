using Com.Game.Module;
using System;

internal class SceneLoader_battle : SceneLoader
{
	public SceneLoader_battle(SceneType s, SceneManager m, string strName) : base(s, m, strName)
	{
	}

	public override void Unload()
	{
		AudioMgr.loadSoundBank_UI();
		AudioMgr.unloadSoundBank_INGAME();
		base.Unload();
		GlobalObject.Instance.EnableGameManager(false);
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void ClearViews()
	{
		base.ClearViews();
		CtrlManager.DestroyAllWindowsExcept(new WindowID[]
		{
			WindowID.VictoryView
		});
		ResourceManager.UnLoadAllAssets(false);
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void BeginLoad()
	{
		base.BeginLoad();
		CtrlManager.OpenWindow(WindowID.LoadView, null);
		this.SendTargetProgress(10);
		base.SetGoOn();
	}

	public override void LoadScene()
	{
		AudioMgr.unloadSoundBank_UI();
		AudioMgr.loadSoundBank_INGAME();
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
		this.SendAddProgress(5);
		base.SetGoOn();
	}

	public override void SetManager()
	{
		base.SetManager();
		MobaMessageManager.RegistMessage((ClientMsg)25011, new MobaMessageFunc(this.OnMsg_SceneManagerReady));
		GlobalObject.Instance.EnableGameManager(true);
		GameManager.Instance.StartGame();
		this.SendAddProgress(35);
	}

	public override void OpenViews()
	{
		base.OpenViews();
		this.SendTargetProgress(100);
		MobaMessageManager.RegistMessage((ClientMsg)21009, new MobaMessageFunc(this.OnMsg_LoadView_complete));
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
		CtrlManager.CloseWindow(WindowID.LoadView);
		Singleton<SkillView>.Instance.SetGameObjectActive(true);
		base.SetGoOn();
	}
}
