using System;

namespace MobaFrame.SkillAction
{
	public interface IAction
	{
		void Play();

		void Stop();

		void Destroy();
	}
}
