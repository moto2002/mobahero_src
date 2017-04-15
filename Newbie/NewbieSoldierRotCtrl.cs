using System;
using System.Collections.Generic;
using UnityEngine;

namespace Newbie
{
	public class NewbieSoldierRotCtrl : MonoBehaviour
	{
		private bool _isRotateDone;

		private void Update()
		{
			if (this._isRotateDone)
			{
				return;
			}
			this.TryRotate();
		}

		private void TryRotate()
		{
			List<Units> mapUnits = MapManager.Instance.GetMapUnits(TargetTag.Minions);
			if (mapUnits == null || mapUnits.Count < 1)
			{
				return;
			}
			Units units = null;
			for (int i = 0; i < mapUnits.Count; i++)
			{
				if (mapUnits[i] != null && TeamManager.CheckTeamType(mapUnits[i].teamType, 1))
				{
					units = mapUnits[i];
					break;
				}
			}
			if (units == null)
			{
				return;
			}
			units.mTransform.rotation = Quaternion.Euler(0f, -90f, 0f);
			this._isRotateDone = true;
			base.enabled = false;
		}
	}
}
