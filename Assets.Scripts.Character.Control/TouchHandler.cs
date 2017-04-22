using System;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
    /// <summary>
    /// 触摸事件处理类
    /// </summary>
	public class TouchHandler : IControlHandler
	{
		private ControlEventManager tEventManager;
        /// <summary>
        /// 手指索引
        /// </summary>
		private int fingerID;
        /// <summary>
        /// 是否按下
        /// </summary>
		private bool hasPressed;
        /// <summary>
        /// 按下时间戳
        /// </summary>
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
        /// <summary>
        /// 更新控制
        /// </summary>
        /// <param name="deltaTime"></param>
		public override void updateControl(float deltaTime)
		{
			this.AnalysisState(deltaTime);
		}
        /// <summary>
        /// 更新分析事件触发逻辑
        /// </summary>
        /// <param name="deltaTime"></param>
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
        /// <summary>
        /// 派发指定属性的事件
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="deltaTime"></param>
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
        /// <summary>
        /// 创建指定类型的事件,加入控制事件管理器
        /// </summary>
        /// <param name="tType"></param>
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
