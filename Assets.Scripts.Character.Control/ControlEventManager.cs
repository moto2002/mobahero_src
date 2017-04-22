using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ControlEventManager
	{
        /// <summary>
        /// 控制事件列表
        /// </summary>
		private List<ControlEvent> rowEventList;
        /// <summary>
        /// 事件处理器列表
        /// </summary>
		private List<IControlHandler> listTouchHandler;

		private ControlEvent lastTriggerEvent;

		private bool isDebug;

		private TouchDebug tDebug;

		private int UIlayerMask = 1 << LayerMask.NameToLayer("PlayerUI") | 1 << LayerMask.NameToLayer("NGUI") | 1 << LayerMask.NameToLayer("UIEffect");

		private Camera mUICamera
		{
			get
			{
				return UICamera.mainCamera;
			}
		}

		public void OnInit()
		{
			this.rowEventList = new List<ControlEvent>();
			this.listTouchHandler = new List<IControlHandler>();
			this.listTouchHandler.Add(new TouchHandler(0, this));
			this.listTouchHandler.Add(new TouchHandler(1, this));
			if (this.isDebug)
			{
				this.tDebug = new TouchDebug();
				this.tDebug.OnInit();
			}
		}

		public void OnExit()
		{
			if (this.isDebug && this.tDebug != null)
			{
				this.tDebug.OnExit();
			}
		}
        /// <summary>
        /// 开启或禁用多点触摸
        /// </summary>
        /// <param name="b"></param>
		public void OpenMultiTouch(bool b)
		{
			Input.multiTouchEnabled = b;
			if (null != this.mUICamera)
			{
				UICamera component = this.mUICamera.GetComponent<UICamera>();
				if (component != null)
				{
					component.allowMultiTouch = b;
				}
			}
		}
        /// <summary>
        /// 清空事件列表
        /// </summary>
		private void ClearEvent()
		{
			this.rowEventList.Clear();
		}
        /// <summary>
        /// 添加新事件到事件列表
        /// </summary>
        /// <param name="newEvent"></param>
		public void AddEvent(ControlEvent newEvent)
		{
			if (newEvent != null)
			{
				this.rowEventList.Add(newEvent);
			}
		}
        /// <summary>
        /// 更新检测控制事件
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
		public ControlEvent UpdateControl(float deltaTime)
		{
			this.ClearEvent();
			for (int i = 0; i < this.listTouchHandler.Count; i++)
			{
				this.listTouchHandler[i].updateControl(deltaTime);
			}
			return this.HandleEvent();
		}
        /// <summary>
        /// 根据优先级获取一个待处理事件
        /// </summary>
        /// <returns></returns>
		private ControlEvent HandleEvent()
		{
			ControlEvent firstPirorityEvent = this.GetFirstPirorityEvent();
			if (this.isDebug)
			{
				this.tDebug.UpdateList(this.rowEventList);
			}
			if (firstPirorityEvent != null && this.IsEventChanged(firstPirorityEvent) && !this.IsTouchUI(firstPirorityEvent.position))
			{
				this.lastTriggerEvent = firstPirorityEvent;
				return firstPirorityEvent;
			}
			return null;
		}
        /// <summary>
        /// 事件列表排序，取出第一优先事件
        /// </summary>
        /// <returns></returns>
		private ControlEvent GetFirstPirorityEvent()
		{
			this.rowEventList.Sort((ControlEvent x, ControlEvent y) => ControlEvent.CompareTouchEvent(x, y));
			if (this.rowEventList.Count > 0)
			{
				return this.rowEventList[0];
			}
			return null;
		}
        /// <summary>
        /// 触发事件是否发生变化
        /// </summary>
        /// <param name="cEvent"></param>
        /// <returns></returns>
		private bool IsEventChanged(ControlEvent cEvent)
		{
			bool result = true;
			if (cEvent == null)
			{
				result = false;
			}
			else if (this.lastTriggerEvent != null)
			{
                //主要是针对按住保持的特殊处理
				if (cEvent.type == EControlType.ePress && this.lastTriggerEvent.type == EControlType.ePress)
				{
					if (cEvent.id == this.lastTriggerEvent.id)
					{
						if (Mathf.Abs(cEvent.position.x - this.lastTriggerEvent.position.x) < 2f && Mathf.Abs(cEvent.position.y - this.lastTriggerEvent.position.y) < 2f && cEvent.createTimeStamp - this.lastTriggerEvent.createTimeStamp < 0.2f)
						{
							result = false;
						}
					}
				}
			}
			return result;
		}
        /// <summary>
        /// 是否触摸在UI上面
        /// </summary>
        /// <param name="targetPoint"></param>
        /// <returns></returns>
		public bool IsTouchUI(Vector3 targetPoint)
		{
			if (this.mUICamera == null)
			{
				return false;
			}
			Ray ray = UICamera.mainCamera.ScreenPointToRay(targetPoint);
			RaycastHit raycastHit;
			return Physics.Raycast(ray, out raycastHit, 100f, this.UIlayerMask);
		}
	}
}
