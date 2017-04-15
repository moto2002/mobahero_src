using System;
using UnityEngine;

namespace MobaHeros.AI
{
	public class MemoryRecord
	{
		public double dTimeBecameVisible;

		public double dTimeLastSensed;

		public double dTimeLastVisible;

		public double dTimeLastOutOfView;

		public double dTimeSpawned;

		public Vector3 vLastSensedPosition;

		public bool bWithingFOV;

		public bool bShootable;

		public int fHatredValue;
	}
}
