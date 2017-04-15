using System;
using UnityEngine;

public class TestSkill_Target : MonoBehaviour
{
	public static string targetskillid = string.Empty;

	public static int targetattackidx = 1;

	private static Units _target;

	public static Units target
	{
		get
		{
			return TestSkill_Target._target;
		}
	}

	public void Update()
	{
		if (TestSkill_Target._target == null)
		{
			TestSkill_Target._target = base.gameObject.GetComponentInChildren<Units>();
		}
	}

	private void Awake()
	{
	}

	public void Start()
	{
		base.gameObject.SetActiveRecursively(true);
		TestSkill_Target._target = base.gameObject.GetComponentInChildren<Units>();
		if (!(TestSkill_Target._target != null) || TestSkill_Target._target.GetType() == typeof(Hero))
		{
		}
	}
}
