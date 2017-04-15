using System;
using UnityEngine;

public class DeMaSkill04MoveUnit : MonoBehaviour
{
	public float mLiveTime;

	private void Start()
	{
		this.mLiveTime = 0f;
	}

	private void Update()
	{
		this.mLiveTime += Time.deltaTime;
	}

	private void OnEnable()
	{
		this.mLiveTime = 0f;
	}

	private void OnDisable()
	{
		this.mLiveTime = 0f;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (this.mLiveTime < 0.2f)
		{
			Transform parent = base.gameObject.transform.parent;
			if (parent != null)
			{
				DeMaSkill04MoveCount component = parent.GetComponent<DeMaSkill04MoveCount>();
				if (component != null)
				{
					component.AddCollider(col.gameObject, base.gameObject);
				}
			}
		}
	}
}
