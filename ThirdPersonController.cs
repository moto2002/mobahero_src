using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[Serializable]
public class ThirdPersonController : MonoBehaviour
{
	public AnimationClip idleAnimation;

	public AnimationClip walkAnimation;

	public AnimationClip runAnimation;

	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed;

	public float trotMaxAnimationSpeed;

	public float runMaxAnimationSpeed;

	public float jumpAnimationSpeed;

	public float landAnimationSpeed;

	private Animation _animation;

	private CharacterState _characterState;

	public float walkSpeed;

	public float trotSpeed;

	public float runSpeed;

	public float inAirControlAcceleration;

	public float jumpHeight;

	public float gravity;

	public float speedSmoothing;

	public float rotateSpeed;

	public float trotAfterSeconds;

	public bool canJump;

	private float jumpRepeatTime;

	private float jumpTimeout;

	private float groundedTimeout;

	private float lockCameraTimer;

	private Vector3 moveDirection;

	private float verticalSpeed;

	private float moveSpeed;

	private CollisionFlags collisionFlags;

	private bool jumping;

	private bool jumpingReachedApex;

	private bool movingBack;

	private bool isMoving;

	private float walkTimeStart;

	private float lastJumpButtonTime;

	private float lastJumpTime;

	private float lastJumpStartHeight;

	private Vector3 inAirVelocity;

	private float lastGroundedTime;

	private bool isControllable;

	public ThirdPersonController()
	{
		this.walkMaxAnimationSpeed = 0.75f;
		this.trotMaxAnimationSpeed = 1f;
		this.runMaxAnimationSpeed = 1f;
		this.jumpAnimationSpeed = 1.15f;
		this.landAnimationSpeed = 1f;
		this.walkSpeed = 2f;
		this.trotSpeed = 4f;
		this.runSpeed = 6f;
		this.inAirControlAcceleration = 3f;
		this.jumpHeight = 0.5f;
		this.gravity = 20f;
		this.speedSmoothing = 10f;
		this.rotateSpeed = 500f;
		this.trotAfterSeconds = 3f;
		this.canJump = true;
		this.jumpRepeatTime = 0.05f;
		this.jumpTimeout = 0.15f;
		this.groundedTimeout = 0.25f;
		this.moveDirection = Vector3.zero;
		this.lastJumpButtonTime = -10f;
		this.lastJumpTime = -1f;
		this.inAirVelocity = Vector3.zero;
		this.isControllable = true;
	}

	public override void Awake()
	{
		this.moveDirection = this.transform.TransformDirection(Vector3.forward);
		this._animation = (Animation)this.GetComponent(typeof(Animation));
		if (!this._animation)
		{
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		}
		if (!this.idleAnimation)
		{
			this._animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if (!this.walkAnimation)
		{
			this._animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if (!this.runAnimation)
		{
			this._animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!this.jumpPoseAnimation && this.canJump)
		{
			this._animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	public override void UpdateSmoothedMovementDirection()
	{
		Transform transform = Camera.main.transform;
		bool flag = this.IsGrounded();
		Vector3 a = transform.TransformDirection(Vector3.forward);
		a.y = (float)0;
		a = a.normalized;
		Vector3 a2 = new Vector3(a.z, (float)0, -a.x);
		float axisRaw = Input.GetAxisRaw("Vertical");
		float axisRaw2 = Input.GetAxisRaw("Horizontal");
		if (axisRaw < -0.2f)
		{
			this.movingBack = true;
		}
		else
		{
			this.movingBack = false;
		}
		bool flag2 = this.isMoving;
		this.isMoving = ((Mathf.Abs(axisRaw2) > 0.1f) ?? (Mathf.Abs(axisRaw) > 0.1f));
		Vector3 vector = axisRaw2 * a2 + axisRaw * a;
		if (flag)
		{
			this.lockCameraTimer += Time.deltaTime;
			if (this.isMoving != flag2)
			{
				this.lockCameraTimer = (float)0;
			}
			if (vector != Vector3.zero)
			{
				if (this.moveSpeed < this.walkSpeed * 0.9f && flag)
				{
					this.moveDirection = vector.normalized;
				}
				else
				{
					this.moveDirection = Vector3.RotateTowards(this.moveDirection, vector, this.rotateSpeed * 0.0174532924f * Time.deltaTime, (float)1000);
					this.moveDirection = this.moveDirection.normalized;
				}
			}
			float t = this.speedSmoothing * Time.deltaTime;
			float num = Mathf.Min(vector.magnitude, 1f);
			this._characterState = CharacterState.Idle;
			if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
			{
				num *= this.runSpeed;
				this._characterState = CharacterState.Running;
			}
			else if (Time.time - this.trotAfterSeconds > this.walkTimeStart)
			{
				num *= this.trotSpeed;
				this._characterState = CharacterState.Trotting;
			}
			else
			{
				num *= this.walkSpeed;
				this._characterState = CharacterState.Walking;
			}
			this.moveSpeed = Mathf.Lerp(this.moveSpeed, num, t);
			if (this.moveSpeed < this.walkSpeed * 0.3f)
			{
				this.walkTimeStart = Time.time;
			}
		}
		else
		{
			if (this.jumping)
			{
				this.lockCameraTimer = (float)0;
			}
			if (this.isMoving)
			{
				this.inAirVelocity += vector.normalized * Time.deltaTime * this.inAirControlAcceleration;
			}
		}
	}

	public override void ApplyJumping()
	{
		if (this.lastJumpTime + this.jumpRepeatTime <= Time.time)
		{
			if (this.IsGrounded() && this.canJump && Time.time < this.lastJumpButtonTime + this.jumpTimeout)
			{
				this.verticalSpeed = this.CalculateJumpVerticalSpeed(this.jumpHeight);
				this.SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public override void ApplyGravity()
	{
		if (this.isControllable)
		{
			bool button = Input.GetButton("Jump");
			if (this.jumping && !this.jumpingReachedApex && this.verticalSpeed <= (float)0)
			{
				this.jumpingReachedApex = true;
				this.SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			if (this.IsGrounded())
			{
				this.verticalSpeed = (float)0;
			}
			else
			{
				this.verticalSpeed -= this.gravity * Time.deltaTime;
			}
		}
	}

	public override float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt((float)2 * targetJumpHeight * this.gravity);
	}

	public override void DidJump()
	{
		this.jumping = true;
		this.jumpingReachedApex = false;
		this.lastJumpTime = Time.time;
		this.lastJumpStartHeight = this.transform.position.y;
		this.lastJumpButtonTime = (float)-10;
		this._characterState = CharacterState.Jumping;
	}

	public override void Update()
	{
		if (!this.isControllable)
		{
			Input.ResetInputAxes();
		}
		if (Input.GetButtonDown("Jump"))
		{
			this.lastJumpButtonTime = Time.time;
		}
		this.UpdateSmoothedMovementDirection();
		this.ApplyGravity();
		this.ApplyJumping();
		Vector3 vector = this.moveDirection * this.moveSpeed + new Vector3((float)0, this.verticalSpeed, (float)0) + this.inAirVelocity;
		vector *= Time.deltaTime;
		CharacterController characterController = (CharacterController)this.GetComponent(typeof(CharacterController));
		this.collisionFlags = characterController.Move(vector);
		if (this._animation)
		{
			if (this._characterState == CharacterState.Jumping)
			{
				if (!this.jumpingReachedApex)
				{
					this._animation[this.jumpPoseAnimation.name].speed = this.jumpAnimationSpeed;
					this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					this._animation.CrossFade(this.jumpPoseAnimation.name);
				}
				else
				{
					this._animation[this.jumpPoseAnimation.name].speed = -this.landAnimationSpeed;
					this._animation[this.jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
					this._animation.CrossFade(this.jumpPoseAnimation.name);
				}
			}
			else if (characterController.velocity.sqrMagnitude < 0.1f)
			{
				this._animation.CrossFade(this.idleAnimation.name);
			}
			else if (this._characterState == CharacterState.Running)
			{
				this._animation[this.runAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, (float)0, this.runMaxAnimationSpeed);
				this._animation.CrossFade(this.runAnimation.name);
			}
			else if (this._characterState == CharacterState.Trotting)
			{
				this._animation[this.walkAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, (float)0, this.trotMaxAnimationSpeed);
				this._animation.CrossFade(this.walkAnimation.name);
			}
			else if (this._characterState == CharacterState.Walking)
			{
				this._animation[this.walkAnimation.name].speed = Mathf.Clamp(characterController.velocity.magnitude, (float)0, this.walkMaxAnimationSpeed);
				this._animation.CrossFade(this.walkAnimation.name);
			}
		}
		if (this.IsGrounded())
		{
			this.transform.rotation = Quaternion.LookRotation(this.moveDirection);
		}
		else
		{
			Vector3 forward = vector;
			forward.y = (float)0;
			if (forward.sqrMagnitude > 0.001f)
			{
				this.transform.rotation = Quaternion.LookRotation(forward);
			}
		}
		if (this.IsGrounded())
		{
			this.lastGroundedTime = Time.time;
			this.inAirVelocity = Vector3.zero;
			if (this.jumping)
			{
				this.jumping = false;
				this.SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public override void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.moveDirection.y > 0.01f)
		{
		}
	}

	public override float GetSpeed()
	{
		return this.moveSpeed;
	}

	public override bool IsJumping()
	{
		return this.jumping;
	}

	public override bool IsGrounded()
	{
		return (this.collisionFlags & CollisionFlags.Below) != CollisionFlags.None;
	}

	public override Vector3 GetDirection()
	{
		return this.moveDirection;
	}

	public override bool IsMovingBackwards()
	{
		return this.movingBack;
	}

	public override float GetLockCameraTimer()
	{
		return this.lockCameraTimer;
	}

	public override bool IsMoving()
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5f;
	}

	public override bool HasJumpReachedApex()
	{
		return this.jumpingReachedApex;
	}

	public override bool IsGroundedWithTimeout()
	{
		return this.lastGroundedTime + this.groundedTimeout > Time.time;
	}

	public override void Reset()
	{
		this.gameObject.tag = "Player";
	}

	public override void Main()
	{
	}
}
