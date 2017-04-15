using System;
using UnityEngine;

public class TestSkill_Player : MonoBehaviour
{
	private static Hero _target;

	public static Hero target
	{
		get
		{
			return TestSkill_Player._target;
		}
	}

	public void Update()
	{
		if (TestSkill_Player._target == null)
		{
			TestSkill_Player._target = base.gameObject.GetComponentInChildren<Hero>();
		}
	}

	private void Awake()
	{
	}

	public void Start()
	{
		base.gameObject.SetActiveRecursively(true);
		TestSkill_Player._target = base.gameObject.GetComponentInChildren<Hero>();
		TestSkill_Player._target.isPlayer = true;
		if (TestSkill_Player._target != null)
		{
			TestSkill_Player._target.gameObject.active = true;
		}
		if (TestSkill_Player._target != null)
		{
		}
	}
}
