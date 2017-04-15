using System;
using UnityEngine;

namespace Com.Game.Module
{
	public static class PerfTools
	{
		public static void SetDebugName(this UnityEngine.Object c, string name)
		{
			if (c)
			{
				c.name = name;
			}
		}

		public static void SetVisible(Transform trans, bool visible)
		{
			trans.gameObject.SetActive(visible);
		}

		public static void SetVisible(GameObject obj, bool visible)
		{
			obj.SetActive(visible);
		}

		public static bool IsVisible(Transform transform)
		{
			return transform && transform.gameObject.activeSelf;
		}

		public static void SetVisibleByTranslate(Transform trans, bool visible)
		{
			if (visible)
			{
				trans.localPosition = Vector3.zero;
			}
			else
			{
				trans.localPosition = Vector3.up * 500f;
			}
		}
	}
}
