using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class UdpPoolBase<T> where T : class, new()
	{
		private static List<T> poolList = new List<T>();

		protected virtual void Reset()
		{
		}

		public static T Alloc()
		{
			T t = default(T);
			if (UdpPoolBase<T>.poolList.Count > 0)
			{
				t = UdpPoolBase<T>.poolList[0];
				UdpPoolBase<T>.poolList.RemoveAt(0);
				(t as UdpPoolBase<T>).Reset();
			}
			else
			{
				t = Activator.CreateInstance<T>();
			}
			return t;
		}

		public static void Free(T ins)
		{
			UdpPoolBase<T>.poolList.Add(ins);
		}
	}
}
