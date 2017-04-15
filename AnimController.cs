using System;
using UnityEngine;

public class AnimController : UnitComponent
{
	protected Mecanim mecanim;

	public static float DEFAULT_ANIM_SPEED = 1f;

	public static float DEFAULT_MOVE_SPEED = 1f;

	private float CThreatenRoarTimeLength = 3f;

	private float CLeaveInBattleStatusTimeLength = 3f;

	private float CNormalIdleShowTimeSpanLength = 10f;

	private bool _isPlayerInView;

	private int _checkPlayerIsInViewIndex;

	private bool _isInBattle;

	private float _startInBattleStatusTime;

	private float _checkMonsterThreatenRoarTime;

	private bool _isDoNormalIdleShow;

	private float _lastNormalIdleShowTime;

	private AnimControllerState curAnimState;

	private bool firstTime = true;

	private float creepSpecialdelaTime;

	public AnimController()
	{
	}

	public AnimController(Units self) : base(self)
	{
	}

	public override void OnCreate()
	{
		base.OnCreate();
		this.mecanim = this.self.mMecanim;
		if (this.mecanim != null && this.mecanim.animator != null)
		{
			this.mecanim.animator.applyRootMotion = false;
		}
	}

	public override void OnInit()
	{
		this._isPlayerInView = false;
		this._checkPlayerIsInViewIndex = 0;
		this._isInBattle = false;
		this._startInBattleStatusTime = 0f;
		this._isDoNormalIdleShow = false;
		this._lastNormalIdleShowTime = 0f;
	}

	public override void OnStart()
	{
		this.ResetControlInfos();
	}

	public override void OnUpdate(float deltaTime)
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.self.IsFrozenAnimation)
		{
			this.FrozenAnimSpeed();
			return;
		}
		if (this.mecanim.IsInComboAttack())
		{
			if (this.self.currentAttack != null)
			{
				this.SetAttackAnimSpeed(this.self.currentAttack);
			}
		}
		else if (this.mecanim.IsInMove())
		{
			this.SetMoveAnimSpeed(this.self.move_speed);
		}
		else
		{
			this.RevertAnimSpeed();
		}
		this.TryDoMonsterCreepSpecial();
	}

	public override void OnWound(Units attacker, float damage)
	{
		if (this.self.IsMonsterCreep())
		{
			if (!this._isInBattle)
			{
				this._isInBattle = true;
				if (this.mecanim != null)
				{
					this.mecanim.BattleIdle(this._isInBattle || this._isPlayerInView);
				}
			}
			if (this._isInBattle)
			{
				this._startInBattleStatusTime = Time.time;
			}
		}
	}

	public override void OnStop()
	{
		if (this.mecanim != null && this.mecanim.animator != null)
		{
			this.mecanim.animator.Play("breath");
		}
	}

	public override void OnDeath(Units attacker)
	{
		this.PlayAnim(AnimationType.Death, true, 0, false, false);
	}

	public override void OnExit()
	{
		if (this.mecanim != null)
		{
			UnityEngine.Object.Destroy(this.mecanim);
			this.mecanim = null;
		}
	}

	public Mecanim GetMecanim()
	{
		return this.mecanim;
	}

	public void SetAttackAnimSpeed(Skill skill)
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.mecanim.animator == null)
		{
			return;
		}
		float num = 0f;
		if (skill.Data.FullAnimTime > 0f)
		{
			num = skill.Data.FullAnimTime * this.self.attack_speed;
		}
		Mathf.Clamp(num, 0.1f, 2.14748365E+09f);
		if (this.self.isMonster)
		{
			this.mecanim.animator.speed = AnimController.DEFAULT_ANIM_SPEED * num * 1.2f * this.self.GetTimeScale();
		}
		else
		{
			this.mecanim.animator.speed = AnimController.DEFAULT_ANIM_SPEED * num * this.self.GetTimeScale();
		}
	}

	public void SetRawAnimSpeed(float speed, bool isLock = true)
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.mecanim.animator == null)
		{
			return;
		}
		this.mecanim.animator.speed = 1f / speed;
		AnimController.DEFAULT_ANIM_SPEED = speed;
	}

	public void SetMoveAnimSpeed(float speed)
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.mecanim.animator == null)
		{
			return;
		}
		float sourceMoveSpeed = this.self.sourceMoveSpeed;
		if (speed > sourceMoveSpeed)
		{
			float num = (speed - sourceMoveSpeed) / sourceMoveSpeed;
			this.mecanim.animator.speed = AnimController.DEFAULT_MOVE_SPEED + num;
		}
		else
		{
			this.mecanim.animator.speed = AnimController.DEFAULT_MOVE_SPEED;
		}
	}

	public void RevertAnimSpeed()
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.mecanim.animator == null)
		{
			return;
		}
		this.mecanim.animator.speed = 1f / AnimController.DEFAULT_ANIM_SPEED;
	}

	public void FrozenAnimSpeed()
	{
		if (this.mecanim == null)
		{
			return;
		}
		if (this.mecanim.animator == null)
		{
			return;
		}
		this.mecanim.animator.speed = 0f;
	}

	public void ForceIdle()
	{
		if (this.mecanim != null)
		{
			this.mecanim.ForceIdle();
		}
	}

	public void ForceDeath()
	{
		if (this.mecanim != null)
		{
			this.mecanim.ForceDeath();
		}
	}

	public void RandomIdle()
	{
		if (this.mecanim != null)
		{
			this.mecanim.RandomIdle();
		}
	}

	public void PlayAnim(string animname, int idx = 0)
	{
		this.mecanim.PlayAnimByName(animname);
	}

	public void PlayAnim(AnimationType type, bool state, int index = 0, bool mustLive = true, bool force = false)
	{
		if (mustLive && !this.self.isLive)
		{
			return;
		}
		if (!this.self.isHero && !this.self.isVisibleInCamera)
		{
			return;
		}
		if (this.self.IsLockAnimState && !force)
		{
			return;
		}
		if (this.mecanim != null)
		{
			switch (type)
			{
			case AnimationType.Breath:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.Idle(state);
				break;
			case AnimationType.Move:
				if (!this.self.CanMoveAnim && state && this.self.isHero)
				{
					return;
				}
				this.curAnimState = AnimControllerState.Move;
				if (this.self.isMonster && this.self.teamType == 2 && this.firstTime && !state)
				{
					this.firstTime = false;
				}
				else
				{
					this.mecanim.Move(state);
				}
				break;
			case AnimationType.Damage:
				this.mecanim.HitUp(state);
				break;
			case AnimationType.Death:
				if (!this.self.isBuilding)
				{
					this.curAnimState = AnimControllerState.Death;
					this.ResetAnimState();
					this.mecanim.Death(state);
				}
				break;
			case AnimationType.HitStun:
				this.mecanim.HitStun(state);
				break;
			case AnimationType.ComboAttack:
				this.curAnimState = AnimControllerState.Attack;
				if (state)
				{
					this.mecanim.ComboAttack(index);
				}
				else
				{
					this.mecanim.ComboAttack(0);
				}
				break;
			case AnimationType.Conjure:
				this.curAnimState = AnimControllerState.Skill;
				if (state)
				{
					this.mecanim.Conjure(index);
				}
				else
				{
					this.mecanim.Conjure(0);
				}
				break;
			case AnimationType.StandUp:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.StandUp(state);
				break;
			case AnimationType.Victory:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.Victory(state);
				break;
			case AnimationType.Failure:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.Failure(state);
				break;
			case AnimationType.BackHome:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.Home(state);
				break;
			case AnimationType.Sleep:
				this.curAnimState = AnimControllerState.Other;
				this.mecanim.Sleep(index);
				break;
			}
		}
	}

	public void ResetAnimState()
	{
		if (this.mecanim != null)
		{
			this.mecanim.ResetState();
		}
	}

	public void ResetActionState()
	{
		if (this.mecanim != null)
		{
			this.mecanim.ResetAction();
		}
	}

	public bool IsInTransition()
	{
		return this.mecanim != null && this.mecanim.IsInTranslation();
	}

	public void ForceNormalized(float normalized = 1f)
	{
		if (this.mecanim != null && this.mecanim.animator != null)
		{
			this.mecanim.animator.Play(0, 0, normalized);
		}
	}

	public void ToAnimStart()
	{
		if (this.mecanim != null)
		{
			this.mecanim.animator.Play(Mecanim.ID_Anim_Death, 0, 1f);
			this.mecanim.animator.Update(0f);
		}
	}

	public bool IsInIdle()
	{
		return !(this.mecanim == null) && this.mecanim.IsInIdle();
	}

	public bool IsInIdleEx()
	{
		return this.curAnimState == AnimControllerState.Idle;
	}

	public bool IsInMove()
	{
		return !(this.mecanim == null) && this.mecanim.IsInMove();
	}

	public bool IsInMoveEx()
	{
		return this.curAnimState == AnimControllerState.Move;
	}

	public bool IsInComboAttack()
	{
		return !(this.mecanim == null) && this.mecanim.IsInComboAttack();
	}

	public bool IsInSkill()
	{
		return !(this.mecanim == null) && this.mecanim.IsInSkill();
	}

	public bool IsInIdleOrMove()
	{
		return !(this.mecanim == null) && (this.mecanim.IsInMove() || this.mecanim.IsInIdle());
	}

	public bool IsCanPlayMoveAnim()
	{
		return !(this.mecanim == null) && this.mecanim.IsCanPlayMoveAnim();
	}

	public void StopMoveAnim()
	{
		if (this.mecanim != null)
		{
			this.mecanim.StopMoveAnim();
		}
	}

	private void ResetControlInfos()
	{
		if (this.mecanim != null)
		{
			this.mecanim.ResetControlInfos();
		}
	}

	private void TryDoMonsterCreepSpecial()
	{
		if (!this.self.IsMonsterCreep())
		{
			return;
		}
		this.creepSpecialdelaTime += Time.deltaTime;
		if (this.creepSpecialdelaTime < 0.3f)
		{
			return;
		}
		this.creepSpecialdelaTime = 0f;
		if (Time.time - this._checkMonsterThreatenRoarTime > 1f)
		{
			this._checkMonsterThreatenRoarTime = Time.time;
			bool flag = MapManager.Instance.IsPlayerInView(this.self.transform.position, this.self.warning_range, false, this._checkPlayerIsInViewIndex, out this._checkPlayerIsInViewIndex);
			if (this._isPlayerInView != flag)
			{
				this._isPlayerInView = flag;
				if (this.mecanim != null)
				{
					this.mecanim.BattleIdle(this._isPlayerInView || this._isInBattle);
				}
			}
		}
		if (this._isInBattle && Time.time - this._startInBattleStatusTime > this.CLeaveInBattleStatusTimeLength)
		{
			this._isInBattle = false;
			if (this.mecanim != null)
			{
				this.mecanim.BattleIdle(this._isInBattle || this._isPlayerInView);
			}
		}
		if (Time.time - this._lastNormalIdleShowTime > this.CNormalIdleShowTimeSpanLength)
		{
			this._lastNormalIdleShowTime = Time.time;
			if (this.mecanim != null)
			{
				this.mecanim.NormalIdleShow(true);
			}
		}
	}
}
