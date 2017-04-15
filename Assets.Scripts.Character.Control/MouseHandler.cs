using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class MouseHandler : IControlHandler
	{
		private ControlEventManager tEventManager;

		private int mouseID = -1;

		private bool hasPressed;

		private float downTimeStamp;

		private Vector2 mousePos;

		public int ControlID
		{
			get
			{
				return this.mouseID;
			}
		}

		public MouseHandler(int mouseIndex, ControlEventManager teManager)
		{
			this.tEventManager = teManager;
			this.mouseID = mouseIndex;
			this.Init();
		}

		private void Init()
		{
			this.hasPressed = false;
			this.downTimeStamp = 0f;
		}

		public override void updateControl(float deltaTime)
		{
		}

		private void AnalysisState(float deltaTime)
		{
			this.mousePos = Input.mousePosition;
			if (Input.GetMouseButtonDown(this.ControlID))
			{
				this.Down(deltaTime);
			}
			else if (Input.GetMouseButtonUp(this.ControlID))
			{
				this.Up(deltaTime);
			}
			else if (Input.GetMouseButton(this.ControlID) && base.IsDrag(Input.mousePosition))
			{
				this.Press(deltaTime);
			}
		}

		private void Down(float deltaTime)
		{
			this.hasPressed = false;
			base.InitPressPos(Input.mousePosition);
			this.downTimeStamp = Time.realtimeSinceStartup;
			this.CreateTouchEvent(EControlType.eDown);
		}

		private void Up(float deltaTime)
		{
			if (this.hasPressed)
			{
				this.hasPressed = false;
				this.CreateTouchEvent(EControlType.eMoveEnd);
			}
			else
			{
				this.CreateTouchEvent(EControlType.eUp);
			}
			this.downTimeStamp = 0f;
		}

		private void Press(float deltaTime)
		{
			this.hasPressed = true;
			this.CreateTouchEvent(EControlType.ePress);
		}

		private void CreateTouchEvent(EControlType tType)
		{
			this.tEventManager.AddEvent(new ControlEvent
			{
				type = tType,
				id = this.ControlID,
				touchDownTimeStamp = this.downTimeStamp,
				position = this.mousePos,
				createTimeStamp = Time.realtimeSinceStartup
			});
		}
	}
}
