using System;
using System.Collections.Generic;
using UnityEngine;

public class FOWRevealer
{
	public Transform mTrans;

	private int previngrass = -1;

	public Vector2 range = new Vector2(2f, 8f);

	public FOWSystem.LOSChecks lineOfSightCheck = FOWSystem.LOSChecks.EveryUpdate;

	public float visiblevalue = 1f;

	public bool isActive = true;

	private Vector3 cumuted = Vector3.zero;

	private FOWSystem.Revealer mRevealer;

	public static List<int> visiblegrasslist = new List<int>();

	private Units unit;

	public bool onlygrass;

	public void Create(Transform transform, float rad, FOWSystem.LOSChecks typ = FOWSystem.LOSChecks.EveryUpdate, bool _onlygrass = false)
	{
		this.mTrans = transform;
		this.onlygrass = _onlygrass;
		if (!_onlygrass)
		{
			this.range.y = rad;
			this.range.x = 2f;
			this.lineOfSightCheck = typ;
			this.mRevealer = FOWSystem.CreateRevealer(typ);
			this.mRevealer.outer = rad;
			this.mRevealer.los = typ;
			this.mRevealer.pos = transform.position;
			this.mRevealer.prvpos = this.mRevealer.pos;
			this.mRevealer.isActive = true;
			this.isActive = true;
		}
		this.unit = transform.gameObject.GetComponent<Units>();
	}

	public void Disable()
	{
		if (this.onlygrass)
		{
			return;
		}
		this.mRevealer.isActive = false;
	}

	public void DoDestroy()
	{
		if (this.onlygrass)
		{
			return;
		}
		FOWSystem.DeleteRevealer(this.mRevealer);
		this.mRevealer = null;
	}

	public void DoUpdate()
	{
		Vector3 position = this.mTrans.position;
		if (!this.onlygrass)
		{
			if (this.isActive)
			{
				if (this.mRevealer != null)
				{
					Vector3 b = this.mRevealer.pos - position;
					this.cumuted += b;
					float sqrMagnitude = this.cumuted.sqrMagnitude;
					if (sqrMagnitude > 0.5f)
					{
						this.mRevealer.moved = true;
						this.cumuted = Vector3.zero;
						this.mRevealer.prvpos = this.mRevealer.pos;
						this.mRevealer.pos = position;
						if (sqrMagnitude >= 2f)
						{
							this.mRevealer.immidate = true;
						}
					}
					if (this.lineOfSightCheck != FOWSystem.LOSChecks.OnlyOnce && this.mRevealer.moved)
					{
						this.mRevealer.cachedBuffer = null;
					}
					this.mRevealer.inner = this.range.x;
					this.mRevealer.outer = this.range.y;
					this.mRevealer.los = this.lineOfSightCheck;
					this.mRevealer.isActive = true;
				}
			}
			else if (this.mRevealer != null)
			{
				this.mRevealer.isActive = false;
				this.mRevealer.cachedBuffer = null;
			}
		}
		if (Time.frameCount % 5 == 0 && this.unit != null)
		{
			int num = FOWSystem.Instance.IsInGrass(position);
			if (num >= 0)
			{
				if (this.unit.isEnemy)
				{
					this.unit.m_nGrassState = 2;
					if (FOWRevealer.visiblegrasslist.Contains(num))
					{
						this.unit.m_nGrassState = 1;
					}
				}
				else
				{
					this.unit.m_nGrassState = 1;
					if (!FOWRevealer.visiblegrasslist.Contains(num))
					{
						FOWRevealer.visiblegrasslist.Add(num);
						this.previngrass = num;
					}
				}
			}
			else
			{
				this.unit.m_nGrassState = 0;
				if (this.previngrass >= 0 && FOWRevealer.visiblegrasslist.Contains(this.previngrass))
				{
					FOWRevealer.visiblegrasslist.Remove(this.previngrass);
				}
			}
		}
	}

	public void Rebuild()
	{
		this.mRevealer.moved = true;
		this.mRevealer.cachedBuffer = null;
	}
}
