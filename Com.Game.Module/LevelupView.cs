using Assets.Scripts.Model;
using Com.Game.Data;
using Com.Game.Manager;
using GUIFramework;
using MobaProtocol.Data;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class LevelupView : BaseView<LevelupView>
	{
		private Transform Mask;

		private UITexture SummonerIcon;

		private UISprite SummonerFrame;

		private UILabel LeftLevel;

		private UILabel RightLevel;

		private UILabel LeftHeroMaxLevel;

		private UILabel RightHeroMaxLevel;

		private int m_curLevel;

		private int m_nextLevel;

		public LevelupView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Common/LevelupView");
		}

		public override void Init()
		{
			base.Init();
			this.Mask = this.transform.Find("Mask");
			this.SummonerFrame = this.transform.Find("BG/Summoner/SummonerFrame").GetComponent<UISprite>();
			this.SummonerIcon = this.transform.Find("BG/Summoner").GetComponent<UITexture>();
			this.LeftLevel = this.transform.Find("BG/Nature/1/Number1").GetComponent<UILabel>();
			this.RightLevel = this.transform.Find("BG/Nature/1/Number2").GetComponent<UILabel>();
			this.LeftHeroMaxLevel = this.transform.Find("BG/Nature/4/Number1").GetComponent<UILabel>();
			this.RightHeroMaxLevel = this.transform.Find("BG/Nature/4/Number2").GetComponent<UILabel>();
			UIEventListener.Get(this.Mask.gameObject).onClick = new UIEventListener.VoidDelegate(this.CloseTheView);
			this.AnimationRoot = this.transform.Find("BG").gameObject;
		}

		public override void HandleAfterOpenView()
		{
		}

		public override void HandleBeforeCloseView()
		{
		}

		public override void RegisterUpdateHandler()
		{
			this.UpdateView();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		private void UpdateView()
		{
			UserData userData = ModelManager.Instance.Get_userData_X();
			SysSummonersPictureframeVo dataById = BaseDataMgr.instance.GetDataById<SysSummonersPictureframeVo>(userData.PictureFrame.ToString());
			this.SummonerFrame.spriteName = dataById.pictureframe_icon;
			SysSummonersHeadportraitVo dataById2 = BaseDataMgr.instance.GetDataById<SysSummonersHeadportraitVo>(userData.Avatar.ToString());
			this.SummonerIcon.mainTexture = ResourceManager.Load<Texture>(dataById2.headportrait_icon, true, true, null, 0, false);
			this.LeftLevel.text = this.m_curLevel.ToString();
			this.RightLevel.text = this.m_nextLevel.ToString();
			this.LeftHeroMaxLevel.text = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>(this.m_curLevel.ToString()).hero_main.ToString();
			this.RightHeroMaxLevel.text = BaseDataMgr.instance.GetDataById<SysSummonersLevelVo>(this.m_nextLevel.ToString()).hero_main.ToString();
		}

		public void SetLevelInfo(int curLevel, int nextLevel)
		{
			this.m_curLevel = curLevel;
			this.m_nextLevel = nextLevel;
		}

		private void CloseTheView(GameObject obj = null)
		{
			CtrlManager.CloseWindow(WindowID.LevelupView);
		}
	}
}
