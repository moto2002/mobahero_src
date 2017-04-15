using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class TouchHandler : IControlHandler
	{
		private ControlEventManager tEventManager;

		private int fingerID;

		private bool hasPressed;

		private float downTimeStamp;

		private Touch touchInfo;

		public int ControlID
		{
			get
			{
				return this.fingerID;
			}
		}

		public TouchHandler(int fingerIndex, ControlEventManager teManager)
		{
			this.tEventManager = teManager;
			this.fingerID = fingerIndex;
			this.Init();
		}

		private void Init()
		{
			this.hasPressed = false;
			this.downTimeStamp = 0f;
		}

		public override void updateControl(float deltaTime)
		{
			this.AnalysisState(deltaTime);
		}

		private void AnalysisState(float deltaTime)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.fingerId == this.ControlID)
				{
					this.touchInfo = touch;
					this.DidpatchEvent(this.touchInfo.phase, deltaTime);
				}
			}
		}

		private void DidpatchEvent(TouchPhase phase, float deltaTime)
		{
			switch (phase)
			{
			case TouchPhase.Began:
				this.Down(deltaTime);
				break;
			case TouchPhase.Moved:
				if (base.IsDrag(this.touchInfo.position))
				{
					this.Press(deltaTime);
				}
				break;
			case TouchPhase.Stationary:
				if (base.IsDrag(this.touchInfo.position))
				{
					this.Press(deltaTime);
				}
				break;
			case TouchPhase.Ended:
				this.Up(deltaTime);
				break;
			}
		}

		private void Down(float deltaTime)
		{
			this.hasPressed = false;
			base.InitPressPos(this.touchInfo.position);
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
				position = this.touchInfo.position,
				createTimeStamp = Time.realtimeSinceStartup
			});
		}
	}
}
