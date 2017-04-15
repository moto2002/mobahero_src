using System;
using UnityEngine;

namespace MobaHeros.Controller
{
	public class UnitController : UnitComponent
	{
		[SerializeField]
		protected bool isActive;

		public UnitController()
		{
		}

		public UnitController(Units self) : base(self)
		{
		}

		public override void OnInit()
		{
		}

		public override void OnStart()
		{
			this.isActive = true;
		}

		public override void OnUpdate(float deltaTime)
		{
			if (this.isActive)
			{
				this.UpdateMouseControl(deltaTime);
			}
		}

		public override void OnStop()
		{
			this.isActive = false;
		}

		public override void OnExit()
		{
			this.isActive = false;
		}

		protected void UpdateMouseControl(float deltaTime)
		{
			if (Input.GetMouseButtonDown(0))
			{
				this.OnTouchDown(Input.mousePosition, 0);
			}
			else if (Input.GetMouseButtonUp(0))
			{
				this.OnTouchUp(Input.mousePosition, 0);
			}
			else if (Input.GetMouseButton(0))
			{
				this.OnTouchMove(Input.mousePosition, 0, deltaTime);
			}
		}

		protected void UpdateTouchControl(float deltaTime)
		{
			if (Input.touchCount > 0)
			{
				int index = 0;
				if (Input.GetTouch(index).phase == TouchPhase.Began)
				{
					this.OnTouchDown(Input.GetTouch(index).position, index);
				}
				else if (Input.GetTouch(index).phase == TouchPhase.Ended)
				{
					this.OnTouchUp(Input.GetTouch(index).position, index);
				}
				else if (Input.GetTouch(index).phase == TouchPhase.Stationary)
				{
					this.OnTouchMove(Input.GetTouch(index).position, index, deltaTime);
				}
			}
		}

		public virtual void OnTouchDown(Vector3 point, int index)
		{
		}

		public virtual void OnTouchMove(Vector3 point, int index, float deltaTime)
		{
		}

		public virtual void OnTouchUp(Vector3 point, int index)
		{
		}

		public virtual void OnSkill(string skill_id)
		{
		}

		public virtual void OnStopSkill()
		{
		}

		public virtual void OnSkillCastBeforeEnd()
		{
		}

		public virtual void OnSkillNotFindTargets()
		{
		}

		public virtual void OnSkillInterrupt()
		{
		}
	}
}
