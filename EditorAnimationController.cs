using System;
using UnityEngine;

public class EditorAnimationController : EditorMono, IEditorUnitCompoent
{
	private Animator animator;

	private EditorUnit self;

	protected override void Awake()
	{
		this.animator = base.GetComponentInChildren<Animator>();
	}

	protected override void Update()
	{
		if (this.self.MoveController.IsMoveing)
		{
			this.Move();
		}
		else
		{
			this.Idle();
		}
	}

	public void Move()
	{
		if (this.animator != null)
		{
			this.animator.SetBool("IsMove", true);
		}
	}

	public void Idle()
	{
		if (this.animator != null)
		{
			this.animator.SetBool("IsMove", false);
		}
	}

	public void ForceIdle()
	{
		this.animator.CrossFade("breath", 0f);
	}

	public void Skill(int index)
	{
		if (this.animator != null)
		{
			this.animator.Play("conjure" + index);
		}
	}

	public void Attack(int index)
	{
		if (this.animator != null)
		{
			this.animator.Play("attack" + index);
		}
	}

	public void Init(EditorUnit unit)
	{
		this.self = unit;
	}
}
