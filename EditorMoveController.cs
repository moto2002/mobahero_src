using System;
using UnityEngine;

public class EditorMoveController : EditorMono, IEditorUnitCompoent
{
	private Vector3 targetPos;

	private float stopDistance = 0.2f;

	private Quaternion newQuaternion;

	public bool IsMoveing
	{
		get;
		private set;
	}

	protected override void Start()
	{
	}

	protected override void Update()
	{
		if (Vector3.Distance(base.Trans.localPosition, this.targetPos) <= this.stopDistance)
		{
			this.IsMoveing = false;
		}
		else
		{
			base.Trans.localPosition += base.Trans.forward * 2f * Time.deltaTime;
			this.IsMoveing = true;
		}
	}

	public void Move(Vector3 pos, float stopDistance = 0.2f)
	{
		this.stopDistance = stopDistance;
		base.transform.LookAt(pos);
		this.targetPos = pos;
	}

	public void LookAt(Vector3 pos)
	{
		base.transform.LookAt(pos);
	}

	public void Stop()
	{
		this.targetPos = base.Trans.position;
	}

	public void Init(EditorUnit hero)
	{
		this.targetPos = base.Trans.localPosition;
	}
}
