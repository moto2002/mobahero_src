using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ControlEvent
	{
		public EControlType type;

		public int id = -1;

		public float touchDownTimeStamp;

		public Vector2 position;

		public float createTimeStamp;

		public override string ToString()
		{
			return string.Format("figureID={0}  type={1} position=({2},{3})  ", new object[]
			{
				this.id,
				this.type.ToString(),
				this.position.x,
				this.position.y
			});
		}

		public static int CompareTouchEvent(ControlEvent a, ControlEvent b)
		{
			if (a == null && b == null)
			{
				return 0;
			}
			if (a == null)
			{
				return 1;
			}
			if (b == null)
			{
				return -1;
			}
			if (a.type == EControlType.eDown)
			{
				if (b.type != EControlType.eDown)
				{
					return -1;
				}
				if (a.id > b.id)
				{
					return -1;
				}
				return 1;
			}
			else if (a.type == EControlType.ePress)
			{
				if (b.type == EControlType.eDown)
				{
					return 1;
				}
				if (b.type == EControlType.ePress)
				{
					if (a.touchDownTimeStamp > b.touchDownTimeStamp)
					{
						return -1;
					}
					if (a.touchDownTimeStamp < b.touchDownTimeStamp)
					{
						return 1;
					}
					return 0;
				}
				else
				{
					if (b.type == EControlType.eUp)
					{
						return -1;
					}
					return -1;
				}
			}
			else if (a.type == EControlType.eUp)
			{
				if (b.type == EControlType.eDown)
				{
					return 1;
				}
				if (b.type == EControlType.ePress)
				{
					return 1;
				}
				if (b.type != EControlType.eUp)
				{
					return 0;
				}
				if (a.id > b.id)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				if (b.type == EControlType.eNull)
				{
					return 0;
				}
				return 1;
			}
		}
	}
}
