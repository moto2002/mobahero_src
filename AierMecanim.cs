using System;
using UnityEngine;

public class AierMecanim : Mecanim
{
	public static int ID_Anim_Attack_Idle = Animator.StringToHash("attack_idle");

	private bool bIsInAttackLastFrame;

	private int attackSwitchToShoudaoDelay;

	private bool bInSoudao;

	private float soudaoDeltaTime;

	public override void StopMoveAnim()
	{
		base.StopMoveAnim();
	}

	public override void Idle(bool IsIdle)
	{
		if (!this.character.animController.IsInIdleOrMove())
		{
			this.bIsInAttackLastFrame = false;
			this.attackSwitchToShoudaoDelay = 0;
			this.SwitchToSoudao();
			if (this.usingmeshanim)
			{
				if (this.animplayer == null || !this.character.isVisibleInCamera)
				{
					return;
				}
				this.animplayer.PlayAnimate("breath", WrapMode.Loop);
				return;
			}
			else
			{
				if (this.animator == null)
				{
					return;
				}
				this.animator.CrossFade(AierMecanim.ID_Anim_Attack_Idle, 0.01f);
				if (IsIdle)
				{
					this.animator.SetBool(Mecanim.ID_Move, false);
				}
				AnimationInfo[] currentAnimationClipState = this.animator.GetCurrentAnimationClipState(0);
				if (currentAnimationClipState != null && currentAnimationClipState.Length >= 1 && this.roc != null)
				{
					this.roc.setclip(currentAnimationClipState[0].clip.name);
				}
			}
		}
		else
		{
			base.Idle(IsIdle);
		}
	}

	public override void ForceIdle()
	{
		if (!this.character.animController.IsInIdleOrMove())
		{
			this.bIsInAttackLastFrame = false;
			this.attackSwitchToShoudaoDelay = 0;
			this.SwitchToSoudao();
		}
		base.ForceIdle();
	}

	public override void ForceDeath()
	{
		base.ForceDeath();
	}

	public override void Move(bool IsMove)
	{
		if (!this.character.animController.IsInIdleOrMove())
		{
			this.bIsInAttackLastFrame = false;
			this.attackSwitchToShoudaoDelay = 0;
			this.SwitchToSoudao();
		}
		base.Move(IsMove);
	}

	public override void Victory(bool IsVictory)
	{
		this.SwitchUpperLayerToEmpty();
		base.Victory(IsVictory);
	}

	public override void Failure(bool IsFailure)
	{
		this.SwitchUpperLayerToEmpty();
		base.Failure(IsFailure);
	}

	public override void Home(bool IsHome)
	{
		this.SwitchUpperLayerToEmpty();
		base.Home(IsHome);
	}

	public override void Death(bool IsDeath)
	{
		this.SwitchUpperLayerToEmpty();
		base.Death(IsDeath);
	}

	public override void ComboAttack(int index)
	{
		this.bIsInAttackLastFrame = true;
		this.SwitchUpperLayerToEmpty();
		base.ComboAttack(index);
	}

	public override void Conjure(int index)
	{
		this.bIsInAttackLastFrame = true;
		this.SwitchUpperLayerToEmpty();
		base.Conjure(index);
	}

	public override void PlayAnimByName(string name)
	{
		this.bIsInAttackLastFrame = true;
		this.SwitchUpperLayerToEmpty();
		base.PlayAnimByName(name);
	}

	public override void ResetState()
	{
		this.ResetUpperLayer();
		base.ResetState();
	}

	protected override void Update()
	{
		if (this.bInSoudao)
		{
			if (this.soudaoDeltaTime > 0f)
			{
				this.ResetUpperLayer();
			}
			else
			{
				this.soudaoDeltaTime += Time.deltaTime;
			}
		}
		else if (this.bIsInAttackLastFrame && (base.IsInMove() || base.IsInIdle()))
		{
			if (this.attackSwitchToShoudaoDelay > 1)
			{
				this.SwitchToSoudao();
				this.bIsInAttackLastFrame = false;
				this.attackSwitchToShoudaoDelay = 0;
			}
			else
			{
				this.attackSwitchToShoudaoDelay++;
			}
		}
		base.Update();
	}

	public void ResetUpperLayer()
	{
		this.bInSoudao = false;
		this.animator.SetBool("bInShoudaoAier", false);
		this.animator.SetBool("bIsForceBackAier", false);
	}

	private void SwitchUpperLayerToEmpty()
	{
		this.bInSoudao = false;
		this.animator.SetBool("bInShoudaoAier", false);
		this.animator.SetBool("bIsForceBackAier", true);
	}

	private void SwitchToSoudao()
	{
		this.bInSoudao = true;
		this.soudaoDeltaTime = 0f;
		this.animator.SetBool("bInShoudaoAier", true);
		this.animator.SetBool("bIsForceBackAier", false);
	}
}
