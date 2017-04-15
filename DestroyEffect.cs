using System;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
	[SerializeField]
	private eUIEffectType m_effectType;

	[SerializeField]
	private float m_time;

	private void Start()
	{
		base.Invoke("DestroyThis", this.m_time);
	}

	private void DestroyThis()
	{
	}
}
