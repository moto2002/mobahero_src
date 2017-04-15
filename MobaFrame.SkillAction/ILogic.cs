using System;

namespace MobaFrame.SkillAction
{
	public interface ILogic<T>
	{
		void OnInit();

		void OnAction();

		void OnInterrupt();

		void OnEnd();

		void Destroy();
	}
}
