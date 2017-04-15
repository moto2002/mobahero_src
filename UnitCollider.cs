using MobaTools.Prefab;
using System;
using UnityEngine;

public class UnitCollider : StaticUnitComponent
{
	private int sourceTeamType;

	private int _originalTeamType;

	protected int sourceLayer;

	protected Vector3 moveDirection = Vector3.zero;

	protected float targetlockdirection = 0.5f;

	public int SourceTeamType
	{
		get
		{
			return this.sourceTeamType;
		}
		set
		{
			this.sourceTeamType = value;
		}
	}

	public int OriginalTeamType
	{
		get
		{
			return this._originalTeamType;
		}
		set
		{
			this._originalTeamType = value;
		}
	}

	public override void OnInit()
	{
		this.InitTeam();
	}

	public override void OnStart()
	{
		this.EnableCollider(true);
	}

	public override void OnUpdate(float deltaTime)
	{
	}

	public override void OnStop()
	{
		this.EnableCollider(false);
		this.RevertTeam();
	}

	public override void OnExit()
	{
	}

	public void EnableCollider(bool enabled)
	{
		if (base.collider != null)
		{
			base.collider.enabled = enabled;
		}
	}

	public void GetBone(int bone_type, out Transform bone, out Vector3 offset)
	{
		bone = base.transform;
		offset = new Vector3(0f, 0f, 0f);
		switch (bone_type)
		{
		case 0:
			bone = base.transform;
			offset = new Vector3(0f, 0f, 0f);
			break;
		case 1:
			bone = base.transform;
			offset = new Vector3(0f, this.GetHeight() / 2f, 0f);
			break;
		case 2:
			bone = base.transform;
			offset = new Vector3(0f, this.GetHeight(), 0f);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
			if (this.self.bones != null && bone_type < this.self.bones.Length && this.self.bones[bone_type] != null)
			{
				if (this.self.isMonster)
				{
					bone = base.transform;
					offset = new Vector3(0f, this.GetHeight() / 2f, 0f);
				}
				else
				{
					bone = this.self.bones[bone_type];
					offset = new Vector3(0f, 0f, 0f);
				}
			}
			break;
		}
	}

	public Vector3 GetBone(BoneAnchorType bone_type)
	{
		switch (bone_type)
		{
		case BoneAnchorType.None:
			return this.self.mTransform.position;
		case BoneAnchorType.Center:
			return this.self.mTransform.position + new Vector3(0f, this.GetHeight() / 2f, 0f);
		case BoneAnchorType.Top:
			return this.self.mTransform.position + new Vector3(0f, this.GetHeight(), 0f);
		case BoneAnchorType.FootStep:
		case BoneAnchorType.Chest:
		case BoneAnchorType.Head:
		case BoneAnchorType.LeftHand:
		case BoneAnchorType.RightHand:
		case BoneAnchorType.Weapon:
		case BoneAnchorType.LeftFoot:
		case BoneAnchorType.RightFoot:
		case BoneAnchorType.LeftForearm:
		case BoneAnchorType.RightForearm:
		case BoneAnchorType.Spine:
		case BoneAnchorType.Neck:
		case BoneAnchorType.Weapon2:
		case BoneAnchorType.Duang:
			if (this.self.bones != null && bone_type < (BoneAnchorType)this.self.bones.Length && this.self.bones[(int)bone_type] != null)
			{
				return this.self.bones[(int)bone_type].position;
			}
			break;
		}
		return Vector3.zero;
	}

	public float GetHeight()
	{
		return this.self.m_ColliderHeight;
	}

	public Vector3 GetCenter()
	{
		float num = this.GetHeight() / 2f;
		Vector3 position = base.transform.position;
		position.y = base.transform.position.y + num;
		return position;
	}

	public Vector3 GetTop()
	{
		float height = this.GetHeight();
		Vector3 position = base.transform.position;
		position.y = base.transform.position.y + height;
		return position;
	}

	public Vector3 GetFeet()
	{
		return base.transform.position;
	}

	public void InitLayer()
	{
		this.sourceLayer = base.gameObject.layer;
	}

	public void ChangeLayer(string newLayer)
	{
		int num = LayerMask.NameToLayer(newLayer);
		if (base.gameObject.layer == num)
		{
			return;
		}
		this.sourceLayer = base.gameObject.layer;
		base.gameObject.layer = num;
	}

	public void RevertLayer()
	{
		if (base.gameObject == null)
		{
			return;
		}
		base.gameObject.layer = this.sourceLayer;
	}

	public TeamType InitTeam()
	{
		this.sourceTeamType = this.self.teamType;
		this._originalTeamType = this.self.teamType;
		return (TeamType)this.sourceTeamType;
	}

	public void SetTeam(TeamType teamType)
	{
		base.gameObject.name = base.gameObject.name.Replace(TeamManager.GetTeamName((TeamType)this.self.teamType), TeamManager.GetTeamName(teamType));
		this.self.teamType = (int)teamType;
	}

	public void RevertTeam()
	{
		base.gameObject.name = base.gameObject.name.Replace(TeamManager.GetTeamName((TeamType)this.self.teamType), TeamManager.GetTeamName((TeamType)this.sourceTeamType));
		this.self.teamType = this.sourceTeamType;
	}

	public void TurnToTarget(Transform target)
	{
		if (!this.self.CanRoatate)
		{
			return;
		}
		if (target != null)
		{
			Vector3 lhs = target.position - base.transform.position;
			if (Vector3.Dot(lhs.normalized, base.transform.forward.normalized) > 0.9f)
			{
				return;
			}
			if (lhs != Vector3.zero)
			{
				this.moveDirection = lhs.normalized;
				this.moveDirection.y = 0f;
				this.doRotation(this.moveDirection);
			}
		}
	}

	public void TurnToTarget(Vector3 target)
	{
		if (!this.self.CanRoatate)
		{
			return;
		}
		Vector3 lhs = target - base.transform.position;
		if (Vector3.Dot(lhs.normalized, base.transform.forward.normalized) > 0.9f)
		{
			return;
		}
		if (lhs != Vector3.zero)
		{
			this.moveDirection = lhs.normalized;
			this.moveDirection.y = 0f;
			this.doRotation(this.moveDirection);
		}
	}

	private void doRotation(Vector3 moveDirection)
	{
		if (!this.self.CanRoatate)
		{
			return;
		}
		Quaternion quaternion = Quaternion.LookRotation(moveDirection.normalized);
		if (quaternion != Quaternion.identity)
		{
			base.transform.rotation = quaternion;
		}
		this.setDirection(moveDirection);
	}

	public void StopRotation()
	{
		TweenRotation component = base.gameObject.GetComponent<TweenRotation>();
		if (component)
		{
			component.enabled = false;
		}
	}

	public Vector3 getDirection()
	{
		return this.moveDirection;
	}

	public void setDirection(Vector3 direction)
	{
		this.moveDirection = direction;
	}

	private void SetPosition(Vector3 pos, Quaternion rotation)
	{
		base.transform.position = pos;
		base.transform.rotation = rotation;
	}
}
