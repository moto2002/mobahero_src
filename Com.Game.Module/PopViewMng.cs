using Assets.Scripts.Server;
using System;
using System.Collections.Generic;

namespace Com.Game.Module
{
	public class PopViewMng : IGlobalComServer
	{
		private static PopViewMng instance;

		private bool enable;

		private Queue<PopViewParam> QPop;

		private object[] msgs;

		public static PopViewMng Instance
		{
			get
			{
				if (PopViewMng.instance == null)
				{
					throw new Exception("PopViewMng 初始化顺序有问题");
				}
				return PopViewMng.instance;
			}
		}

		public void OnAwake()
		{
			PopViewMng.instance = this;
			this.QPop = new Queue<PopViewParam>();
			this.msgs = new object[]
			{
				ClientC2V.PopView_enqueue
			};
			this.Regist();
		}

		public void OnStart()
		{
		}

		public void OnUpdate()
		{
			if (this.enable && this.QPop.Count > 0 && !this.HasPop())
			{
				this.DoPopView(this.QPop.Dequeue());
			}
		}

		public void OnDestroy()
		{
			this.Unregist();
			this.QPop.Clear();
			this.QPop = null;
			PopViewMng.instance = null;
		}

		public void Enable(bool b)
		{
			this.enable = b;
		}

		public void OnRestart()
		{
			if (Singleton<NewPopView>.Instance != null)
			{
				Singleton<NewPopView>.Instance.IsOpened = false;
			}
			this.Enable(false);
		}

		public void OnApplicationQuit()
		{
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		private void Regist()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void Unregist()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		private void OnMsg_PopView_enqueue(MobaMessage msg)
		{
			PopViewParam popViewParam = msg.Param as PopViewParam;
			if (popViewParam != null)
			{
				if (this.QPop.Count == 0 && !this.HasPop())
				{
					this.DoPopView(popViewParam);
				}
				else
				{
					this.QPop.Enqueue(popViewParam);
				}
			}
		}

		private void DoPopView(PopViewParam param)
		{
			CtrlManager.OpenWindow(WindowID.NewPopView, null);
			NewPopView ctrl = CtrlManager.GetCtrl<NewPopView>(WindowID.NewPopView);
			if (ctrl != null)
			{
				ctrl.SetParam(param);
			}
		}

		private bool HasPop()
		{
			return Singleton<NewPopView>.Instance != null && Singleton<NewPopView>.Instance.IsOpen;
		}
	}
}
