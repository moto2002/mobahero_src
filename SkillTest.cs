using System;
using UnityEngine;

public class SkillTest : MonoBehaviour
{
	public static SkillTest ins;

	[SerializeField]
	public bool isTastAttack;

	private void Awake()
	{
		SkillTest.ins = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
