using System;

namespace Com.Game.Utils
{
	public interface IKeyAlloc<T>
	{
		void Reset();

		T Get();

		T GetRange(int num);
	}
}
