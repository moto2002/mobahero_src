using GUIFramework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Module
{
	public class SummonerView : BaseView<SummonerView>
	{
		private Transform ButtonPanel;

		private UIToggle PassportButton;

		private UIToggle VIPButton;

		private UIToggle SummonerSkillButton;

		private Transform Anchor;

		private string chooseName = string.Empty;

		private Transform passportTrans;

		private Transform summonerSkillTrans;

		public PassportView passportView;

		private SummonerSkillView summonerSkillView;

		private GameObject passportObj;

		private GameObject summonerSkillObj;

		private int type = 1;

		private SummonerType currentSummonerType;

		private Dictionary<SummonerType, UIToggle> summonerDictToggle = new Dictionary<SummonerType, UIToggle>();

		public SummonerView()
		{
			this.WinResCfg = new WinResurceCfg(true, true, "SummonerView");
			this.WindowTitle = LanguageManager.Instance.GetStringById("SummonerUI_Title_Summoner");
		}

		public override void Init()
		{
			base.Init();
			this.Anchor = this.transform.Find("Anchor");
			this.passportObj = ResourceManager.Load<GameObject>("PassportView", true, true, null, 0, false);
			this.summonerSkillObj = (Resources.Load("Prefab/UI/Summoner/SummonerSkillView") as GameObject);
			this.PassportButton = this.transform.Find("Left/Grid/PassportBtn").GetComponent<UIToggle>();
			this.VIPButton = this.transform.Find("Left/Grid/VIPBtn").GetComponent<UIToggle>();
			this.SummonerSkillButton = this.transform.Find("Left/Grid/SummonerSkillBtn").GetComponent<UIToggle>();
			EventDelegate.Add(this.PassportButton.onChange, new EventDelegate.Callback(this.ClcikToggle));
			EventDelegate.Add(this.VIPButton.onChange, new EventDelegate.Callback(this.ClcikToggle));
			EventDelegate.Add(this.SummonerSkillButton.onChange, new EventDelegate.Callback(this.ClcikToggle));
			UIEventListener expr_FB = UIEventListener.Get(this.VIPButton.gameObject);
			expr_FB.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(expr_FB.onClick, new UIEventListener.VoidDelegate(this.noOpen));
			this.passportTrans = NGUITools.AddChild(this.Anchor.gameObject, this.passportObj).transform;
			this.summonerSkillTrans = NGUITools.AddChild(this.Anchor.gameObject, this.summonerSkillObj).transform;
			this.passportView = new PassportView(this.passportTrans);
			this.passportView.Init();
			this.summonerSkillView = new SummonerSkillView(this.summonerSkillTrans);
			this.summonerSkillView.Init();
			this.AdjustDepth();
		}

		public override void HandleAfterOpenView()
		{
			this.passportView.HandleAfterOpenView();
			this.summonerSkillView.HandleAfterOpenView();
		}

		public override void HandleBeforeCloseView()
		{
			this.passportView.HandleBeforeCloseView();
			this.summonerSkillView.HandleBeforeCloseView();
		}

		public override void RegisterUpdateHandler()
		{
			this.passportView.RegisterUpdateHandler();
			this.summonerSkillView.RegisterUpdateHandler();
			this.RefreshUI();
			this.ShowBtn();
		}

		public override void CancelUpdateHandler()
		{
			this.passportView.CancelUpdateHandler();
			this.summonerSkillView.CancelUpdateHandler();
			this.currentSummonerType = SummonerType.Passport;
		}

		public override void RefreshUI()
		{
			this.chooseName = string.Empty;
			this.ShowPanel(this.currentSummonerType);
		}

		public override void Destroy()
		{
			base.Destroy();
			this.passportView.Destroy();
			this.summonerSkillView.Destroy();
		}

		private void AdjustDepth()
		{
			this.SetDepth(this.passportTrans.GetComponentsInChildren<UIPanel>(true));
			this.SetDepth(this.summonerSkillTrans.GetComponentsInChildren<UIPanel>(true));
		}

		private void SetDepth(UIPanel[] panels)
		{
			for (int i = 0; i < panels.Length; i++)
			{
				UIPanel uIPanel = panels[i];
				uIPanel.depth = this.gameObject.GetComponent<UIPanel>().depth + 1;
			}
		}

		private void ClcikToggle()
		{
			if (UIToggle.current.value)
			{
				foreach (KeyValuePair<SummonerType, UIToggle> current in this.summonerDictToggle)
				{
					if (UIToggle.current == current.Value)
					{
						this.currentSummonerType = current.Key;
					}
				}
				this.ShowPanel(this.currentSummonerType);
			}
		}

		private void noOpen(GameObject go)
		{
			Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
		}

		private void ShowPanel(SummonerType type)
		{
			this.passportTrans.gameObject.SetActive(false);
			this.summonerSkillTrans.gameObject.SetActive(false);
			switch (type)
			{
			case SummonerType.Passport:
				this.passportTrans.gameObject.SetActive(true);
				this.passportView.RefreshUI();
				break;
			case SummonerType.DreamVIP:
				Singleton<TipView>.Instance.ShowViewSetText("暂未开放", 1f);
				break;
			case SummonerType.SummonerSkill:
				this.summonerSkillTrans.gameObject.SetActive(true);
				this.summonerSkillView.RefreshUI();
				break;
			}
		}

		public void ChangeShow(int newType, bool isShow = true)
		{
			if (newType != 11)
			{
			}
		}

		private void ShowBtn()
		{
			this.summonerDictToggle[SummonerType.Passport] = this.PassportButton;
			this.summonerDictToggle[SummonerType.SummonerSkill] = this.SummonerSkillButton;
			this.summonerDictToggle[this.currentSummonerType].value = true;
		}
	}
}
