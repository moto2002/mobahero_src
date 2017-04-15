using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
	public event CallbackDelegateBool OnTrigger
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		add
		{
			this.OnTrigger = (CallbackDelegateBool)Delegate.Combine(this.OnTrigger, value);
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		remove
		{
			this.OnTrigger = (CallbackDelegateBool)Delegate.Remove(this.OnTrigger, value);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("SkillPointer") && this.OnTrigger != null)
		{
			this.OnTrigger(true);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.CompareTag("SkillPointer") && this.OnTrigger != null)
		{
			this.OnTrigger(false);
		}
	}
}
