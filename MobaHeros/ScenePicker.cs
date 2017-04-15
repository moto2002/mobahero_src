using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobaHeros
{
	public class ScenePicker
	{
		private readonly Vector3 _touchPos;

		private float dpi;

		private bool gotDpi;

		public ScenePicker(Vector3 touchPos)
		{
			this._touchPos = touchPos;
		}

		public Vector3? GetGroundPoint()
		{
			Ray ray = Camera.main.ScreenPointToRay(this._touchPos);
			int groundMask = Layer.GroundMask;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, 200f, groundMask);
			if (flag)
			{
				return new Vector3?(raycastHit.point);
			}
			return null;
		}

		private float GetDPI()
		{
			if (!this.gotDpi)
			{
				this.dpi = ((Screen.dpi <= 0f) ? 100f : Screen.dpi);
				this.gotDpi = true;
			}
			return this.dpi;
		}

		public Units PickBestUnit(Predicate<Units> cond)
		{
			float num = 0.3f;
			float num2 = 999f;
			Units result = null;
			Vector3? groundPoint = this.GetGroundPoint();
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			foreach (Units current in allMapUnits.Values)
			{
				if (!(null == current) && !current.isItem && !current.isBuffItem && current.m_SelectRadius > 0f && cond(current))
				{
					float num3 = Vector3.Distance(current.transform.position, groundPoint.Value);
					if (num3 <= current.m_SelectRadius + 2.5f)
					{
						Vector3 b = Camera.main.WorldToScreenPoint(current.ColliderCenter);
						b.z = 0f;
						Vector3 touchPos = this._touchPos;
						touchPos.z = 0f;
						float num4 = Vector3.Distance(touchPos, b);
						float num5 = this.GetDPI() * num;
						if (TagManager.CheckTag(current, TargetTag.Tower))
						{
							num5 *= 1.5f;
						}
						else if (TagManager.CheckTag(current, TargetTag.Home))
						{
							num5 *= 3f;
						}
						if (num4 < num5 + this.GetDPI() * num * 0.2f && num4 < num2)
						{
							num2 = num4;
							result = current;
						}
					}
				}
			}
			return result;
		}
	}
}
