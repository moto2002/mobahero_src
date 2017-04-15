using System;
using UnityEngine;

public class ShenNiuSkill01MoveUnit : MonoBehaviour
{
	private float mLiveTime;

	private void Awake()
	{
	}

	private void Start()
	{
		this.mLiveTime = 0f;
	}

	private void Update()
	{
		this.mLiveTime += Time.deltaTime;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (this.mLiveTime < 0.5f)
		{
			Transform parent = base.gameObject.transform.parent;
			if (parent != null)
			{
				ShenNiuSkill01MoveCount component = parent.GetComponent<ShenNiuSkill01MoveCount>();
				if (component != null)
				{
					component.AddCollider(col.gameObject, base.gameObject);
				}
			}
		}
	}
}
