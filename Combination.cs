using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class Combination : MonoBehaviour
{
	public int count = 5;

	public float width;

	public GameObject Prefab;

	private List<CombinationUnit> CombinationUnitList = new List<CombinationUnit>();

	private bool isStop;

	private void Awake()
	{
	}

	private void OnSpawned()
	{
	}

	private void OnEnable()
	{
		if (Application.isPlaying)
		{
			return;
		}
		this.isStop = false;
		this.ClearAll();
		for (int i = 0; i < this.count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(this.Prefab, Vector3.zero, base.transform.rotation) as GameObject;
			base.gameObject.AddComponent(gameObject.GetType());
			if (gameObject)
			{
				gameObject.name += i;
			}
			gameObject.layer = LayerMask.NameToLayer("SkillObstacle");
			gameObject.transform.parent = base.transform;
			CombinationUnit combinationUnit = gameObject.AddComponent<CombinationUnit>();
			combinationUnit.Init(this, i);
			gameObject.transform.localPosition = new Vector3(0f, 0f, this.width * (float)i);
			this.CombinationUnitList.Add(combinationUnit);
		}
	}

	[DebuggerHidden]
	private IEnumerator Creator()
	{
		Combination.<Creator>c__Iterator91 <Creator>c__Iterator = new Combination.<Creator>c__Iterator91();
		<Creator>c__Iterator.<>f__this = this;
		return <Creator>c__Iterator;
	}

	public void Stop(int index)
	{
	}

	private void ClearAll()
	{
	}
}
