using GUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Com.Game.Module
{
	public class RegularView : BaseView<RegularView>
	{
		private Transform BG;

		private UIScrollView Panel;

		private UISlider ProgressBar;

		private GameObject Btn;

		private UITable Table;

		private GameObject ruleItemCache;

		private Dictionary<string, string> rules = new Dictionary<string, string>();

		private CoroutineManager cMgr;

		public RegularView()
		{
			this.WinResCfg = new WinResurceCfg(false, false, "Prefab/UI/Regular/RegularView");
		}

		public override void Init()
		{
			base.Init();
			this.BG = this.transform.Find("BG");
			this.Panel = this.transform.Find("Panel").GetComponent<UIScrollView>();
			this.ProgressBar = this.transform.Find("Progress Bar").GetComponent<UISlider>();
			this.Btn = this.transform.Find("Btn").gameObject;
			this.Table = this.transform.Find("Panel/Table").GetComponent<UITable>();
			this.ruleItemCache = ResourceManager.LoadPath<GameObject>("Prefab/UI/Regular/RuleItem", null, 0);
			UIEventListener.Get(this.BG.gameObject).onClick = new UIEventListener.VoidDelegate(this.BackBtn);
			UIEventListener.Get(this.Btn).onClick = new UIEventListener.VoidDelegate(this.BackBtn);
			this.AnimationRoot = this.gameObject;
		}

		public override void RegisterUpdateHandler()
		{
			this.RefreshUI();
		}

		public override void CancelUpdateHandler()
		{
		}

		public override void HandleAfterOpenView()
		{
			if (this.cMgr == null)
			{
				this.cMgr = new CoroutineManager();
			}
			this.rules.Clear();
		}

		public override void HandleBeforeCloseView()
		{
			this.cMgr.StopAllCoroutine();
			this.cMgr = null;
		}

		public override void RefreshUI()
		{
		}

		public override void Destroy()
		{
			base.Destroy();
		}

		public void AddRule(string _title, string _context)
		{
			this.rules.Add(_title, _context);
		}

		public void ShowRule()
		{
			int num = 0;
			foreach (KeyValuePair<string, string> current in this.rules)
			{
				RuleItem component = NGUITools.AddChild(this.Table.gameObject, this.ruleItemCache).GetComponent<RuleItem>();
				component.SetText(current.Key, current.Value);
				component.gameObject.name = num.ToString("D2");
				num++;
			}
			this.cMgr.StartCoroutine(this.WaitUntilNextFrame(), true);
		}

		[DebuggerHidden]
		private IEnumerator WaitUntilNextFrame()
		{
			RegularView.<WaitUntilNextFrame>c__Iterator16C <WaitUntilNextFrame>c__Iterator16C = new RegularView.<WaitUntilNextFrame>c__Iterator16C();
			<WaitUntilNextFrame>c__Iterator16C.<>f__this = this;
			return <WaitUntilNextFrame>c__Iterator16C;
		}

		private void BackBtn(GameObject obj)
		{
			CtrlManager.CloseWindow(WindowID.RegularView);
		}
	}
}
