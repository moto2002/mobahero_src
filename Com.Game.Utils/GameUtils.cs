using System;
using UnityEngine;

namespace Com.Game.Utils
{
	public static class GameUtils
	{
		public static Transform FindObjByAllChilds(Transform genTrans, string targetObjName)
		{
			while (genTrans.childCount > 0)
			{
				foreach (Transform transform in genTrans)
				{
					if (transform.gameObject.name == targetObjName)
					{
						Transform result = transform;
						return result;
					}
					Transform transform2 = GameUtils.FindObjByAllChilds(transform, targetObjName);
					if (transform2 != null)
					{
						Transform result = transform2;
						return result;
					}
				}
			}
			return null;
		}

		public static bool Assert(bool expr, string msg = null)
		{
			if (!expr)
			{
				ClientLogger.Warn("Assert failed: " + msg);
			}
			return expr;
		}
	}
}
