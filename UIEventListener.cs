using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Event Listener")]
public class UIEventListener : MonoBehaviour
{
	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject draggedObject);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	public object parameter;

	public UIEventListener.VoidDelegate onSubmit;

	public UIEventListener.VoidDelegate onClick;

	public UIEventListener.BoolDelegate onDoubleClick;

	public UIEventListener.BoolDelegate onHover;

	public UIEventListener.BoolDelegate onPress;

	public UIEventListener.BoolDelegate onSelect;

	public UIEventListener.FloatDelegate onScroll;

	public UIEventListener.VectorDelegate onDrag;

	public UIEventListener.ObjectDelegate onDrop;

	public UIEventListener.KeyCodeDelegate onKey;

	public UIEventListener.VoidDelegate onDragStart;

	public UIEventListener.BoolDelegate onMobileHover;

	public UIEventListener.VoidDelegate onDragEnd;

	public UIEventListener.VoidDelegate LongPress;

	private float intialTime = 0.4f;

	private float intervalTime = 0.05f;

	public bool isCanLongPress;

	private bool isPress;

	private void OnSubmit()
	{
		if (this.onSubmit != null)
		{
			this.onSubmit(base.gameObject);
		}
	}

	private void OnClick()
	{
		if (this.onClick != null)
		{
			this.onClick(base.gameObject);
		}
	}

	private void OnDoubleClick()
	{
		if (this.onDoubleClick != null)
		{
			this.onDoubleClick(base.gameObject, true);
		}
	}

	private void OnHover(bool isOver)
	{
		if (this.onHover != null)
		{
			this.onHover(base.gameObject, isOver);
		}
	}

	private void OnPress(bool isPressed)
	{
		if (this.onPress != null)
		{
			this.onPress(base.gameObject, isPressed);
		}
		if (isPressed)
		{
			this.isPress = true;
		}
		else
		{
			this.isPress = false;
		}
	}

	private void OnSelect(bool selected)
	{
		if (this.onSelect != null)
		{
			this.onSelect(base.gameObject, selected);
		}
	}

	private void OnScroll(float delta)
	{
		if (this.onScroll != null)
		{
			this.onScroll(base.gameObject, delta);
		}
	}

	private void OnDrag(Vector2 delta)
	{
		if (this.onDrag != null)
		{
			this.onDrag(base.gameObject, delta);
		}
	}

	private void OnDrop(GameObject go)
	{
		if (this.onDrop != null)
		{
			this.onDrop(base.gameObject, go);
		}
	}

	private void OnKey(KeyCode key)
	{
		if (this.onKey != null)
		{
			this.onKey(base.gameObject, key);
		}
	}

	private void OnDragStart()
	{
	}

	private void OnDragOver(GameObject draggedObject)
	{
		if (this.onMobileHover != null)
		{
			this.onMobileHover(base.gameObject, true);
		}
	}

	private void OnDragOut(GameObject draggedObject)
	{
		if (this.onMobileHover != null)
		{
			this.onMobileHover(base.gameObject, false);
		}
	}

	private void OnDragEnd()
	{
	}

	private void Update()
	{
		if (this.isCanLongPress)
		{
			if (this.isPress)
			{
				if (this.intialTime > 0f)
				{
					this.intialTime -= Time.deltaTime;
				}
				else
				{
					if (base.gameObject == null)
					{
						return;
					}
					this.LongPress(base.gameObject);
					this.intialTime += this.intervalTime;
				}
			}
			else if (!this.isPress && this.intialTime != 0.4f)
			{
				this.intialTime = 0.4f;
			}
		}
	}

	public static UIEventListener Get(GameObject go)
	{
		UIEventListener uIEventListener = go.GetComponent<UIEventListener>();
		if (uIEventListener == null)
		{
			uIEventListener = go.AddComponent<UIEventListener>();
		}
		return uIEventListener;
	}

	public void SimulatePress()
	{
		this.OnPress(true);
	}

	private void OnDisable()
	{
		this.isPress = false;
	}
}
