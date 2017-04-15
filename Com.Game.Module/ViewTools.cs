using System;
using UnityEngine;

namespace Com.Game.Module
{
	public static class ViewTools
	{
		public static bool MyGetCompoent<T>(this Transform trans, string path, out T com) where T : Component
		{
			bool result = false;
			com = (T)((object)null);
			if (!(null == trans))
			{
				Transform transform;
				if (!string.IsNullOrEmpty(path))
				{
					transform = trans.FindChild(path);
				}
				else
				{
					transform = trans;
				}
				if (null != transform)
				{
					if (typeof(T) == typeof(GameObject))
					{
						com = (transform.gameObject as T);
					}
					else if (typeof(T) == typeof(Transform))
					{
						com = (transform as T);
					}
					else
					{
						com = transform.GetComponent<T>();
					}
				}
				result = (null != com);
			}
			return result;
		}
	}
}
