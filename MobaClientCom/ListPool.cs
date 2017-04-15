using System;
using System.Collections.Generic;

namespace MobaClientCom
{
	public class ListPool
	{
		public static List<T> get<T>()
		{
			return ObjPool<List<T>>.get();
		}

		public static void release<T>(List<T> list)
		{
			list.Clear();
			ObjPool<List<T>>.release(list);
		}

		public static void clear<T>()
		{
			ObjPool<List<T>>.clear();
		}
	}
}
