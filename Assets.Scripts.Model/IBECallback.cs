using System;

namespace Assets.Scripts.Model
{
	public interface IBECallback
	{
		Func<object, int, bool> Callback
		{
			get;
		}

		object Ret
		{
			get;
		}
	}
}
