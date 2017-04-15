using System;

namespace MobaHeros.AI
{
	public interface IGoal<T>
	{
		void Destroy();

		void Activate();

		int Process();

		void Terminate();

		void Render();
	}
}
