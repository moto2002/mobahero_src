using Com.Game.Module;
using System;

namespace Assets.Scripts.GUILogic.View.Runes
{
	public class RunesCtrl
	{
		private static RunesCtrl instance;

		private static object obj_lock = new object();

		private object[] mgs;

		private string heroNpc;

		public int modelID;

		public RunesInlayState state;

		public string HeroNpc
		{
			get
			{
				return this.heroNpc;
			}
		}

		public RunesInlayState runestate
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		private RunesCtrl()
		{
		}

		public static RunesCtrl GetInstance()
		{
			if (RunesCtrl.instance == null)
			{
				object obj = RunesCtrl.obj_lock;
				lock (obj)
				{
					if (RunesCtrl.instance == null)
					{
						RunesCtrl.instance = new RunesCtrl();
						return RunesCtrl.instance;
					}
				}
			}
			return RunesCtrl.instance;
		}

		public void Init()
		{
			this.mgs = new object[]
			{
				ClientV2C.runesviewOpenView,
				ClientV2C.coalesceviewAfterCoalesce,
				ClientC2V.RunesSynthesize,
				ClientC2V.RunesErrorRefreshMoney
			};
			this.Register();
			this.state = RunesInlayState.Nothing;
		}

		public void UnInit()
		{
			this.UnRegister();
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		private void UnRegister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		private void OnMsg_runesviewOpenView(MobaMessage msg)
		{
			string param = string.Empty;
			if (msg.Param != null)
			{
				param = (string)msg.Param;
				this.heroNpc = param;
			}
			CtrlManager.OpenWindow(WindowID.RunesOverView, null);
			MobaMessageManagerTools.SendClientMsg(ClientV2C.runesviewInitToggle, param, false);
		}

		private void OnMsg_RunesSynthesize(MobaMessage msg)
		{
			if (msg != null)
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
				MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewAfterCoalesce, null, false);
			}
		}

		private void OnMsg_RunesErrorRefreshMoney(MobaMessage msg)
		{
			if (msg != null && Singleton<MenuTopBarView>.Instance != null)
			{
				Singleton<MenuTopBarView>.Instance.RefreshUI();
			}
		}

		private void OnMsg_coalesceviewAfterCoalesce(MobaMessage msg)
		{
			if (msg != null && this.modelID != 0)
			{
				CtrlManager.OpenWindow(WindowID.GetItemsView, null);
				MobaMessageManagerTools.GetItems_Rune(this.modelID);
				GetItemsView expr_29 = Singleton<GetItemsView>.Instance;
				expr_29.onFinish = (Callback)Delegate.Combine(expr_29.onFinish, new Callback(this.RefreshDataUI));
				Singleton<GetItemsView>.Instance.Play();
			}
		}

		private void RefreshDataUI()
		{
			MobaMessageManagerTools.SendClientMsg(ClientV2C.coalesceviewChangeBtnState, null, false);
		}
	}
}
