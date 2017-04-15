using System;
using System.Collections.Generic;

namespace GameLogin.State
{
	public abstract class LoginTaskBase
	{
		private object[] msgs;

		private ELoginTask taskType;

		protected LoginTask lTask;

		public bool Valid
		{
			get;
			protected set;
		}

		public LoginTaskBase(ELoginTask task, params object[] msgs)
		{
			this.taskType = task;
			this.msgs = msgs;
			this.lTask = new LoginTask();
			this.Register();
			this.Valid = true;
		}

		public virtual void Destroy()
		{
			this.Unregister();
		}

		public virtual void Update()
		{
			Action action = this.lTask.GetAction();
			if (action != null)
			{
				action();
			}
		}

		private void Register()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, true, "OnMsg_");
		}

		private void Unregister()
		{
			MobaMessageManagerTools.RegistMsg(this, this.msgs, false, "OnMsg_");
		}

		protected void OnMsg_Login_Action(MobaMessage msg)
		{
			this.OnAction((ELoginAction)((int)msg.Param));
		}

		protected void DoAction(ELoginAction e)
		{
			MobaMessageManagerTools.SendClientMsg(ClientC2C.Login_Action, e, true);
		}

		protected void OnAction(ELoginAction e)
		{
			this.lTask.OnAction(e);
		}

		protected void AddTask(List<ELoginAction> dep, Action func)
		{
			this.lTask.AddTask(dep, func);
		}
	}
}
