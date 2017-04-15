using System;

namespace Com.Game.Module
{
	public class Singleton<T> where T : new()
	{
		private static T instance = (default(T) != null) ? default(T) : Activator.CreateInstance<T>();

		public static T Instance
		{
			get
			{
				return Singleton<T>.instance;
			}
		}

		protected Singleton()
		{
		}
	}
}
