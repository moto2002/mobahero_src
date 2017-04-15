using System;
using System.Collections.Generic;

namespace MobaClientCom
{
	public class ObjPool<T> where T : new()
	{
		private static List<T> poolList = new List<T>();

		public static T get()
		{
			T result;
			if (ObjPool<T>.poolList.Count > 0)
			{
				int index = ObjPool<T>.poolList.Count - 1;
				T t = ObjPool<T>.poolList[index];
				ObjPool<T>.poolList.RemoveAt(index);
				result = t;
			}
			else
			{
				result = ((default(T) == null) ? Activator.CreateInstance<T>() : default(T));
			}
			return result;
		}

		public static void release(T obj)
		{
			ObjPool<T>.poolList.Add(obj);
		}

		public static void clear()
		{
			ObjPool<T>.poolList.Clear();
		}
	}
	public class ObjPool
	{
		public static T get<T>() where T : new()
		{
			return ObjPool<T>.get();
		}

		public static void release<T>(T obj) where T : new()
		{
			ObjPool<T>.release(obj);
		}
	}
}
