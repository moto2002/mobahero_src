using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class JiTui : MonoBehaviour
{
	private bool isRepel;

	private float distance;

	private bool usePosition;

	private Vector3 targetPos = Vector3.zero;

	private Vector3 startPos = Vector3.zero;

	private float speed = 20f;

	private float angle = 70f;

	private CharacterController controller;

	private Units target;

	private GameObject tempGo;

	public Callback OnDestroyCallback;

	public void Init(Units theurgist, float distance, float speed = 20f, bool usePosition = false, float angle = 70f)
	{
		this.distance = distance;
		this.usePosition = usePosition;
		this.targetPos = base.transform.position + theurgist.transform.forward * distance;
		this.targetPos.y = 0f;
		this.startPos = base.transform.position;
		this.speed = speed;
		this.angle = angle;
		this.controller = base.gameObject.GetComponent<CharacterController>();
		this.target = base.gameObject.GetComponent<Units>();
	}

	public void Init(Units theurgist, Vector3 targetPos, float speed = 20f, bool usePosition = false, float angle = 70f)
	{
		this.usePosition = usePosition;
		this.targetPos = targetPos;
		this.startPos = base.transform.position;
		this.distance = Vector3.Distance(this.startPos, targetPos);
		this.speed = speed;
		this.angle = angle;
		this.controller = base.gameObject.GetComponent<CharacterController>();
		this.target = base.gameObject.GetComponent<Units>();
	}

	[DebuggerHidden]
	public IEnumerator Repel_Coroutine()
	{
		JiTui.<Repel_Coroutine>c__Iterator94 <Repel_Coroutine>c__Iterator = new JiTui.<Repel_Coroutine>c__Iterator94();
		<Repel_Coroutine>c__Iterator.<>f__this = this;
		return <Repel_Coroutine>c__Iterator;
	}

	private void doDestroy()
	{
		if (this.OnDestroyCallback != null)
		{
			this.OnDestroyCallback();
		}
		if (this.tempGo != null)
		{
			UnityEngine.Object.Destroy(this.tempGo);
		}
		UnityEngine.Object.Destroy(this);
	}

	private void OnDestroy()
	{
		this.doDestroy();
	}

	private void OnDespawned()
	{
		this.doDestroy();
	}
}
