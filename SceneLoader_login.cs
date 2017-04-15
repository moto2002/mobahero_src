using Com.Game.Module;
using System;

internal class SceneLoader_login : SceneLoader
{
	public SceneLoader_login(SceneType s, SceneManager m, string strName) : base(s, m, strName)
	{
	}

	public override void Unload()
	{
		base.Unload();
		LoginStateManager.Instance.Enable(false);
		base.SetGoOn();
	}

	public override void ClearViews()
	{
		base.ClearViews();
		CtrlManager.CloseWindow(WindowID.AreaView_New);
		base.SetGoOn();
	}

	public override void BeginLoad()
	{
		base.BeginLoad();
		base.SetGoOn();
	}

	public override void LoadScene()
	{
		base.LoadScene();
		base.SetGoOn();
	}

	public new virtual void SetCamera()
	{
		base.SetCamera();
		base.SetGoOn();
	}

	public new virtual void SetFPS()
	{
		base.SetFPS();
		base.SetGoOn();
	}

	public override void SetManager()
	{
		base.SetManager();
		MobaMessageManager.RegistMessage((ClientMsg)25011, new MobaMessageFunc(this.ManagerReady));
		LoginStateManager.Instance.Enable(true);
	}

	public override void OpenViews()
	{
		base.OpenViews();
		base.SetGoOn();
	}

	public override void LoadSceneEnd()
	{
		base.LoadSceneEnd();
		base.SendSceneLoadComplete();
		base.SetGoOn();
	}

	private void ManagerReady(MobaMessage msg)
	{
		MobaMessageManager.UnRegistMessage((ClientMsg)25011, new MobaMessageFunc(this.ManagerReady));
		base.SetGoOn();
	}
}
