using System;

namespace MobaHeros.Spawners
{
	public interface ISpawnTask
	{
		void Start();

		void Stop();
	}
}
