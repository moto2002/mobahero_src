using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogin.State
{
	public class LoginTask
	{
		public class TaskInfo
		{
			public List<ELoginAction> dep;

			public Action func;

			public TaskInfo(List<ELoginAction> dep, Action func)
			{
				this.dep = dep;
				this.func = func;
			}
		}

		private Queue<LoginTask.TaskInfo> qTask;

		public LoginTask()
		{
			this.qTask = new Queue<LoginTask.TaskInfo>();
		}

		public void AddTask(List<ELoginAction> dep, Action func)
		{
			this.qTask.Enqueue(new LoginTask.TaskInfo(dep, func));
		}

		public void OnAction(ELoginAction e)
		{
			if (this.qTask.Count > 0)
			{
				for (int i = 0; i < this.qTask.Count; i++)
				{
					LoginTask.TaskInfo taskInfo = this.qTask.ElementAt(i);
					if (taskInfo.dep != null && taskInfo.dep.Count > 0)
					{
						int num = taskInfo.dep.IndexOf(e);
						if (num != -1)
						{
							taskInfo.dep.RemoveAt(num);
						}
					}
				}
			}
		}

		public Action GetAction()
		{
			Action result = null;
			if (this.qTask.Count > 0)
			{
				LoginTask.TaskInfo taskInfo = this.qTask.Peek();
				if (taskInfo.dep == null || taskInfo.dep.Count == 0)
				{
					result = taskInfo.func;
					this.qTask.Dequeue();
				}
			}
			return result;
		}

		public bool HasAction()
		{
			return this.qTask.Count > 0;
		}
	}
}
