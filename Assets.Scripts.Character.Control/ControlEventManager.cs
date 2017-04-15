using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Character.Control
{
	public class ControlEventManager
	{
		private List<ControlEvent> rowEventList;

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

		private void ClearEvent()
		{
			this.rowEventList.Clear();
		}

		public void AddEvent(ControlEvent newEvent)
		{
			if (newEvent != null)
			{
				this.rowEventList.Add(newEvent);
			}
		}

		public ControlEvent UpdateControl(float deltaTime)
		{
			this.ClearEvent();
			for (int i = 0; i < this.listTouchHandler.Count; i++)
			{
				this.listTouchHandler[i].updateControl(deltaTime);
			}
			return this.HandleEvent();
		}

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

		private ControlEvent GetFirstPirorityEvent()
		{
			this.rowEventList.Sort((ControlEvent x, ControlEvent y) => ControlEvent.CompareTouchEvent(x, y));
			if (this.rowEventList.Count > 0)
			{
				return this.rowEventList[0];
			}
			return null;
		}

		private bool IsEventChanged(ControlEvent cEvent)
		{
			bool result = true;
			if (cEvent == null)
			{
				result = false;
			}
			else if (this.lastTriggerEvent != null)
			{
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
