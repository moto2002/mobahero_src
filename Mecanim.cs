using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class Mecanim : MonoBehaviour
{
	public int[] ComboList;

	public int[] AttackList;

	public float animatSpeed = 1f;

	public Animator animator;

	public Units character;

	public AnimatorStateInfo animatorStateInfo;

	public Transform rootBone;

	public static int ID_Move = Animator.StringToHash("IsMove");

	public static int ID_ComboAttack = Animator.StringToHash("ComboAttack");

	public static int ID_Conjure = Animator.StringToHash("Conjure");

	public static int ID_Death = Animator.StringToHash("IsDeath");

	public static int ID_HitUp = Animator.StringToHash("IsHitUp");

	public static int ID_HitStun = Animator.StringToHash("IsHitStun");

	public static int ID_Victory = Animator.StringToHash("IsVictory");

	public static int ID_Failure = Animator.StringToHash("IsFailure");

	public static int ID_StandUp = Animator.StringToHash("IsStandUp");

	public static int ID_Home = Animator.StringToHash("IsHome");

	public static int ID_Sleep = Animator.StringToHash("SleepStatus");

	public static int ID_IsInBattle = Animator.StringToHash("IsInBattle");

	public static int ID_IsDoNormalIdleShow = Animator.StringToHash("IsDoNormalIdleShow");

	public static int ID_ConjureStateIndex = Animator.StringToHash("ConjureState");

	public static int[] ID_Anim_ConjureList = Mecanim.GetAnimArray("conjure", 10);

	public static int[] ID_Anim_AttackList = Mecanim.GetAnimArray("attack", 6);

	public static int ID_Anim_Idle = Animator.StringToHash("breath");

	public static int ID_Anim_Run = Animator.StringToHash("run");

	public static int ID_Anim_Death = Animator.StringToHash("death");

	public static int ID_Anim_Show = Animator.StringToHash("NormalIdleShow");

	public static int ID_Anim_Roar = Animator.StringToHash("BattleRoar");

	public static int ID_Anim_AttackFir = Animator.StringToHash("attack1");

	public static int ID_Anim_AttackSec = Animator.StringToHash("attack2");

	public static int ID_Anim_AttackThird = Animator.StringToHash("attack3");

	public static int ID_Anim_IdleType = Animator.StringToHash("IdleType");

	private CoroutineManager m_CoroutineManager = new CoroutineManager();

	private Vector3 m_privotPoisition;

	private Quaternion m_privotRotation;

	public bool usingmeshanim;

	protected AnimPlayer animplayer;

	protected RecieverObjCtrl roc;

	private static int[] GetAnimArray(string name, int num)
	{
		int[] array = new int[num + 1];
		for (int i = 0; i <= num; i++)
		{
			array[i] = Animator.StringToHash(name + i);
		}
		return array;
	}

	public static Mecanim AddAnimationBase(Transform character)
	{
		Mecanim mecanim = null;
		Animator componentInChildren = character.GetComponentInChildren<Animator>();
		if (componentInChildren != null)
		{
			mecanim = character.gameObject.GetComponent<Mecanim>();
			if (mecanim == null)
			{
				mecanim = character.gameObject.AddComponent<Mecanim>();
			}
		}
		return mecanim;
	}

	protected virtual void Update()
	{
		if (this.animplayer != null && this.usingmeshanim != GlobalSettings.meshanim)
		{
			this.usingmeshanim = GlobalSettings.meshanim;
			if (this.usingmeshanim)
			{
				this.animplayer.disableAnimator();
			}
			else
			{
				this.animplayer.showAnimator();
			}
		}
	}

	private void Awake()
	{
		if (this.character == null)
		{
			this.character = base.GetComponent<Units>();
		}
		if (this.animator == null)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		if (this.animator != null)
		{
			this.animator.logWarnings = false;
		}
		this.animplayer = base.GetComponent<AnimPlayer>();
		if (GlobalSettings.meshanim)
		{
			if (this.animplayer != null)
			{
				this.usingmeshanim = true;
			}
			else
			{
				this.usingmeshanim = false;
			}
		}
		else
		{
			this.usingmeshanim = false;
		}
		this.roc = this.character.GetComponentInChildren<RecieverObjCtrl>();
	}

	private void OnDisable()
	{
		this.m_CoroutineManager.StopAllCoroutine();
	}

	public bool IsInTranslation()
	{
		return !(this.animator == null) && this.animator.IsInTransition(0);
	}

	public bool IsInComboAttack()
	{
		if (this.usingmeshanim)
		{
			return !(this.animplayer == null) && this.character.isVisibleInCamera && this.animplayer.IsPlayingAnimationContain("attack");
		}
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[1] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[2] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[3] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[4] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[5];
	}

	public bool IsInAttackIndex(int index)
	{
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackList[index];
	}

	public bool IsInSkill()
	{
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_ConjureList[1] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_ConjureList[2] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_ConjureList[3] || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_ConjureList[4];
	}

	public bool IsInIdle()
	{
		if (this.usingmeshanim)
		{
			return !(this.animplayer == null) && this.character.isVisibleInCamera && this.animplayer.IsPlayingAnimationContain("breath");
		}
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_Idle;
	}

	public bool IsInMove()
	{
		if (this.usingmeshanim)
		{
			return !(this.animplayer == null) && this.character.isVisibleInCamera && this.animplayer.IsPlayingAnimationContain("run");
		}
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_Run;
	}

	public bool IsCanPlayMoveAnim()
	{
		if (this.usingmeshanim)
		{
			return true;
		}
		if (this.animator == null)
		{
			return false;
		}
		this.animatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
		return this.animatorStateInfo.tagHash == Mecanim.ID_Anim_Idle || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_Show || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_Roar || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackFir || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackSec || this.animatorStateInfo.tagHash == Mecanim.ID_Anim_AttackThird;
	}

	public void RandomIdle()
	{
		this.animator.SetInteger(Mecanim.ID_Anim_IdleType, UnityEngine.Random.Range(1, 3));
	}

	public virtual void StopMoveAnim()
	{
		if (!this.usingmeshanim)
		{
			if (this.roc != null)
			{
				this.roc.setclip("breath");
			}
			if (this.animator != null)
			{
				this.animator.SetBool(Mecanim.ID_Move, false);
			}
			return;
		}
		if (this.animplayer == null || !this.character.isVisibleInCamera)
		{
			return;
		}
		if (this.animplayer.IsPlayingAnimationContain("run"))
		{
			this.animplayer.StopAnim("run");
		}
	}

	public void ResetControlInfos()
	{
		if (this.IsMecanimValid() && Mecanim.ID_Move != 0)
		{
			this.animator.SetBool(Mecanim.ID_Move, false);
		}
	}

	public void StopAction(bool IsStopAct)
	{
		if (this.animator == null)
		{
			return;
		}
	}

	public virtual void Idle(bool IsIdle)
	{
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
			this.animator.CrossFade(Mecanim.ID_Anim_Idle, 0.01f);
			if (IsIdle)
			{
				this.animator.SetBool(Mecanim.ID_Move, false);
			}
			AnimationInfo[] currentAnimationClipState = this.animator.GetCurrentAnimationClipState(0);
			if (currentAnimationClipState != null && currentAnimationClipState.Length >= 1 && this.roc != null)
			{
				this.roc.setclip(currentAnimationClipState[0].clip.name);
			}
			return;
		}
	}

	public virtual void ForceIdle()
	{
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
			this.animator.Play(Mecanim.ID_Anim_Idle);
			AnimationInfo[] currentAnimationClipState = this.animator.GetCurrentAnimationClipState(0);
			if (currentAnimationClipState != null && currentAnimationClipState.Length >= 1 && this.roc != null)
			{
				this.roc.setclip(currentAnimationClipState[0].clip.name);
			}
			return;
		}
	}

	public virtual void ForceDeath()
	{
		if (this.usingmeshanim)
		{
			if (this.animplayer == null || !this.character.isVisibleInCamera)
			{
				return;
			}
			this.animplayer.PlayAnimate("death", WrapMode.Once);
			return;
		}
		else
		{
			if (this.roc != null)
			{
				this.roc.setclip("death");
			}
			if (this.animator == null)
			{
				return;
			}
			this.animator.CrossFade(Mecanim.ID_Anim_Death, 0.1f);
			return;
		}
	}

	public virtual void Move(bool IsMove)
	{
		if (this.usingmeshanim)
		{
			if (this.animplayer == null || !this.character.isVisibleInCamera)
			{
				return;
			}
			this.animplayer.PlayAnimate("run", WrapMode.Loop);
			return;
		}
		else
		{
			if (IsMove)
			{
				if (this.roc != null)
				{
					this.roc.setclip("run");
				}
			}
			if (this.animator == null)
			{
				return;
			}
			this.animator.SetBool(Mecanim.ID_Move, IsMove);
			if (IsMove)
			{
				this.animator.CrossFade(Mecanim.ID_Anim_Run, 0f);
			}
			return;
		}
	}

	public virtual void Victory(bool IsVictory)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_Victory, IsVictory);
		this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_Victory, 5f, false), true);
	}

	public virtual void Failure(bool IsFailure)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_Failure, IsFailure);
		this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_Failure, 5f, false), true);
	}

	public virtual void Home(bool IsHome)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_Home, IsHome);
	}

	public virtual void Death(bool IsDeath)
	{
		if (this.usingmeshanim)
		{
			if (this.animplayer == null || !this.character.isVisibleInCamera)
			{
				return;
			}
			this.animplayer.PlayAnimate("death", WrapMode.Once);
			return;
		}
		else
		{
			if (this.roc != null)
			{
				this.roc.setclip("death");
			}
			if (this.animator == null)
			{
				return;
			}
			this.animator.SetBool(Mecanim.ID_Death, IsDeath);
			this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_Death, 0.5f, false), true);
			return;
		}
	}

	public virtual void ComboAttack(int index)
	{
		if (this.usingmeshanim)
		{
			if (this.animplayer == null || !this.character.isVisibleInCamera)
			{
				return;
			}
			this.animplayer.PlayAnimate("attack" + index.ToString(), WrapMode.Once);
			return;
		}
		else
		{
			if (this.roc != null)
			{
				this.roc.setclip("attack" + index.ToString());
			}
			if (this.animator == null)
			{
				return;
			}
			if (index <= 0)
			{
				return;
			}
			if (!this.IsInAttackIndex(index))
			{
				this.animator.CrossFade(Mecanim.ID_Anim_AttackList[index], 0.01f);
			}
			return;
		}
	}

	public virtual void Conjure(int index)
	{
		if (this.animator == null)
		{
			return;
		}
		if (index < 0)
		{
			return;
		}
		if (index > 0)
		{
			this.animator.CrossFade(Mecanim.ID_Anim_ConjureList[index], 0f);
		}
		AnimationInfo[] currentAnimationClipState = this.animator.GetCurrentAnimationClipState(0);
		if (currentAnimationClipState != null && currentAnimationClipState.Length >= 1 && this.roc != null)
		{
			this.roc.setclip(currentAnimationClipState[0].clip.name);
		}
	}

	public void ConjureState(int index)
	{
		if (this.animator == null)
		{
			return;
		}
	}

	public void HitUp(bool IsHitUp)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_HitUp, IsHitUp);
		this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_HitUp, 0f, false), true);
	}

	public void HitStun(bool IsHitStun)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_HitStun, IsHitStun);
	}

	public void StandUp(bool IsStandUp)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetBool(Mecanim.ID_StandUp, IsStandUp);
		this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_StandUp, 0f, false), true);
	}

	public void Sleep(int inSleepStatus)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetInteger(Mecanim.ID_Sleep, inSleepStatus);
	}

	public void BattleIdle(bool inIsInBattle)
	{
		if (this.animator != null)
		{
			this.animator.SetBool(Mecanim.ID_IsInBattle, inIsInBattle);
		}
	}

	public void NormalIdleShow(bool inIsNormalIdleShow)
	{
		if (this.animator != null)
		{
			this.animator.SetBool(Mecanim.ID_IsDoNormalIdleShow, inIsNormalIdleShow);
			this.m_CoroutineManager.StartCoroutine(this.ReboundState(Mecanim.ID_IsDoNormalIdleShow, 0.5f, false), true);
		}
	}

	public virtual void PlayAnimByName(string name)
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.Play(name);
	}

	private bool IsMecanimValid()
	{
		return !(this.animator == null) && !(this.animator.avatar == null) && this.animator.avatar.isValid;
	}

	public virtual void ResetState()
	{
		if (!this.IsMecanimValid())
		{
			return;
		}
		this.animator.SetInteger(Mecanim.ID_ComboAttack, 0);
		this.animator.SetInteger(Mecanim.ID_Conjure, 0);
		this.animator.SetBool(Mecanim.ID_Move, false);
		this.animator.SetBool(Mecanim.ID_HitStun, false);
		this.animator.SetBool(Mecanim.ID_Death, false);
		this.animator.SetBool(Mecanim.ID_Home, false);
	}

	public void ResetAction()
	{
		if (this.animator == null)
		{
			return;
		}
		this.animator.SetInteger(Mecanim.ID_ComboAttack, 0);
		this.animator.SetInteger(Mecanim.ID_Conjure, 0);
	}

	public bool GetBoolIDState(int id)
	{
		return this.animator.GetBool(id);
	}

	public int GetIndexIDState(int id)
	{
		return this.animator.GetInteger(id);
	}

	[DebuggerHidden]
	private IEnumerator ReboundState(int ID, float time, bool deathFlag = false)
	{
		Mecanim.<ReboundState>c__Iterator29 <ReboundState>c__Iterator = new Mecanim.<ReboundState>c__Iterator29();
		<ReboundState>c__Iterator.time = time;
		<ReboundState>c__Iterator.ID = ID;
		<ReboundState>c__Iterator.<$>time = time;
		<ReboundState>c__Iterator.<$>ID = ID;
		<ReboundState>c__Iterator.<>f__this = this;
		return <ReboundState>c__Iterator;
	}

	[DebuggerHidden]
	private IEnumerator ReboundState(int ID, bool deathFlag = false)
	{
		Mecanim.<ReboundState>c__Iterator2A <ReboundState>c__Iterator2A = new Mecanim.<ReboundState>c__Iterator2A();
		<ReboundState>c__Iterator2A.ID = ID;
		<ReboundState>c__Iterator2A.deathFlag = deathFlag;
		<ReboundState>c__Iterator2A.<$>ID = ID;
		<ReboundState>c__Iterator2A.<$>deathFlag = deathFlag;
		<ReboundState>c__Iterator2A.<>f__this = this;
		return <ReboundState>c__Iterator2A;
	}

	[DebuggerHidden]
	private IEnumerator ReboundState(int ID, int value, float time)
	{
		Mecanim.<ReboundState>c__Iterator2B <ReboundState>c__Iterator2B = new Mecanim.<ReboundState>c__Iterator2B();
		<ReboundState>c__Iterator2B.time = time;
		<ReboundState>c__Iterator2B.ID = ID;
		<ReboundState>c__Iterator2B.value = value;
		<ReboundState>c__Iterator2B.<$>time = time;
		<ReboundState>c__Iterator2B.<$>ID = ID;
		<ReboundState>c__Iterator2B.<$>value = value;
		<ReboundState>c__Iterator2B.<>f__this = this;
		return <ReboundState>c__Iterator2B;
	}

	[DebuggerHidden]
	private IEnumerator ReboundStateFrame(int ID, int value)
	{
		Mecanim.<ReboundStateFrame>c__Iterator2C <ReboundStateFrame>c__Iterator2C = new Mecanim.<ReboundStateFrame>c__Iterator2C();
		<ReboundStateFrame>c__Iterator2C.ID = ID;
		<ReboundStateFrame>c__Iterator2C.value = value;
		<ReboundStateFrame>c__Iterator2C.<$>ID = ID;
		<ReboundStateFrame>c__Iterator2C.<$>value = value;
		<ReboundStateFrame>c__Iterator2C.<>f__this = this;
		return <ReboundStateFrame>c__Iterator2C;
	}
}
