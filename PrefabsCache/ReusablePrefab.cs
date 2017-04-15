using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PrefabsCache
{
	public abstract class ReusablePrefab : MonoBehaviour
	{
		public event PrefabDeactivatedEventHandler PrefabDeactivated
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.PrefabDeactivated = (PrefabDeactivatedEventHandler)Delegate.Combine(this.PrefabDeactivated, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.PrefabDeactivated = (PrefabDeactivatedEventHandler)Delegate.Remove(this.PrefabDeactivated, value);
			}
		}

		public Transform TransRef
		{
			get
			{
				return base.transform;
			}
		}

		public override int GetHashCode()
		{
			return base.gameObject.name.GetHashCode();
		}

		public virtual void Restart()
		{
			base.gameObject.SetActive(true);
		}

		public virtual void DeactivatePrefab()
		{
			if (this.PrefabDeactivated != null)
			{
				this.PrefabDeactivated(this);
			}
			base.gameObject.SetActive(false);
		}
	}
}
