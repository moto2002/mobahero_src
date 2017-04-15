using System;
using UnityEngine;

public class EffectListener : MonoBehaviour
{
	[SerializeField]
	private eUIEffectType m_effectType;

	[SerializeField]
	private float m_effectScale;

	private void Start()
	{
	}

	private void OnDestroy()
	{
	}
}
