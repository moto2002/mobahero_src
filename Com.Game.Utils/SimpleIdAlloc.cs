using System;

namespace Com.Game.Utils
{
	public class SimpleIdAlloc : IKeyAlloc<int>
	{
		private const int InitVal = 1000;

		private int _curVal;

		public SimpleIdAlloc()
		{
			this.Reset();
		}

		public void Reset()
		{
			this._curVal = 1000;
		}

		public int Get()
		{
			return this._curVal++;
		}

		public int GetRange(int num)
		{
			if (num <= 0)
			{
				throw new ArgumentException("num must be positive");
			}
			this._curVal += num;
			return this._curVal;
		}
	}
}
