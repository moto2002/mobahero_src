using System;

namespace MobaFrame.SkillAction
{
	public abstract class BaseLogic<T> : ILogic<T> where T : new()
	{
		protected T owner;

		protected object data;

		public abstract void OnInit();

		public abstract void OnAction();

		public abstract void OnInterrupt();

		public abstract void OnEnd();

		public abstract void Destroy();
	}
}
