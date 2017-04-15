using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Game.Utils
{
	public static class CommonExtension
	{
		public static TDefault GetDictValue<TKey, TValue, TDefault>(this Dictionary<TKey, TValue> dict, TKey key, TDefault def) where TDefault : TValue
		{
			TValue tValue;
			if (dict.TryGetValue(key, out tValue))
			{
				return (TDefault)((object)tValue);
			}
			return def;
		}

		public static Vector3 ToVector3(this SVector3 v)
		{
			return new Vector3(v.x, v.y, v.z);
		}

		public static SVector3 ToSVector3(this Vector3 v)
		{
			return new SVector3
			{
				x = v.x,
				y = v.y,
				z = v.z
			};
		}

		public static float Magnitude2D(this Vector3 v)
		{
			return Mathf.Sqrt(v.x * v.x + v.z * v.z);
		}

		public static float SqrMagnitude2D(this Vector3 v)
		{
			return v.x * v.x + v.z * v.z;
		}

		public static T TryGetComp<T>(this Transform trans, string childPath = null) where T : Component
		{
			if (trans == null)
			{
				return (T)((object)null);
			}
			if (childPath != null)
			{
				trans = trans.TryFindChild(childPath);
			}
			T component = trans.GetComponent<T>();
			if (!component)
			{
				ClientLogger.Error("TryGetComp failed: no component " + typeof(T));
			}
			return component;
		}

		public static Transform TryFindChild(this Transform trans, string childName)
		{
			if (!trans)
			{
				ClientLogger.Error("TryFindChild failed: parent is null");
				return null;
			}
			Transform transform = trans.Find(childName);
			if (!transform)
			{
				ClientLogger.Error(string.Format("TryFindChild failed: parent {0}, child {1}", trans.name, childName));
			}
			return transform;
		}
	}
}
