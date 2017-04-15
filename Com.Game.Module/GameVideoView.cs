using Assets.Scripts.GUILogic.View.GameVideo;
using GUIFramework;
using System;

namespace Com.Game.Module
{
	public class GameVideoView : BaseView<GameVideoView>
	{
		private GameVideo_LeftAnchor leftAnchor;

		private GameVideo_RightAnchor rightAnchor;

		public GameVideoView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Home/GameVideoView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("LiveorPlayback_Theme002", "直播与视频");
		}

		public override void Init()
		{
			this.leftAnchor = this.transform.FindChild("Left").GetComponent<GameVideo_LeftAnchor>();
			this.rightAnchor = this.transform.FindChild("Right").GetComponent<GameVideo_RightAnchor>();
		}

		public override void HandleAfterOpenView()
		{
			this.leftAnchor.InitToggles();
		}

		public override void RegisterUpdateHandler()
		{
		}

		public override void CancelUpdateHandler()
		{
		}
	}
}
