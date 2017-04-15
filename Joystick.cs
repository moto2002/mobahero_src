using System;
using UnityEngine;

[RequireComponent(typeof(GUITexture))]
[Serializable]
public class Joystick : MonoBehaviour
{
	[NonSerialized]
	private static Joystick[] joysticks;

	[NonSerialized]
	private static bool enumeratedJoysticks;

	[NonSerialized]
	private static float tapTimeDelta = 0.3f;

	public bool touchPad;

	public Rect touchZone;

	public Vector2 deadZone;

	public bool normalize;

	public Vector2 position;

	public int tapCount;

	private int lastFingerId;

	private float tapTimeWindow;

	private Vector2 fingerDownPos;

	private float fingerDownTime;

	private float firstDeltaTime;

	private GUITexture gui;

	private Rect defaultRect;

	private Boundary guiBoundary;

	private Vector2 guiTouchOffset;

	private Vector2 guiCenter;

	public Joystick()
	{
		this.deadZone = Vector2.zero;
		this.lastFingerId = -1;
		this.firstDeltaTime = 0.5f;
		this.guiBoundary = new Boundary();
	}

	public override void Start()
	{
		this.gui = (GUITexture)this.GetComponent(typeof(GUITexture));
		this.defaultRect = this.gui.pixelInset;
		this.defaultRect.x = this.defaultRect.x + this.transform.position.x * (float)Screen.width;
		this.defaultRect.y = this.defaultRect.y + this.transform.position.y * (float)Screen.height;
		float x = (float)0;
		Vector3 vector = this.transform.position;
		float num = vector.x = x;
		Vector3 vector2 = this.transform.position = vector;
		float y = (float)0;
		Vector3 vector3 = this.transform.position;
		float num2 = vector3.y = y;
		Vector3 vector4 = this.transform.position = vector3;
		if (this.touchPad)
		{
			if (this.gui.texture)
			{
				this.touchZone = this.defaultRect;
			}
		}
		else
		{
			this.guiTouchOffset.x = this.defaultRect.width * 0.5f;
			this.guiTouchOffset.y = this.defaultRect.height * 0.5f;
			this.guiCenter.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiCenter.y = this.defaultRect.y + this.guiTouchOffset.y;
			this.guiBoundary.min.x = this.defaultRect.x - this.guiTouchOffset.x;
			this.guiBoundary.max.x = this.defaultRect.x + this.guiTouchOffset.x;
			this.guiBoundary.min.y = this.defaultRect.y - this.guiTouchOffset.y;
			this.guiBoundary.max.y = this.defaultRect.y + this.guiTouchOffset.y;
		}
	}

	public override void Disable()
	{
		this.gameObject.SetActive(false);
		Joystick.enumeratedJoysticks = false;
	}

	public override void ResetJoystick()
	{
		this.gui.pixelInset = this.defaultRect;
		this.lastFingerId = -1;
		this.position = Vector2.zero;
		this.fingerDownPos = Vector2.zero;
		if (this.touchPad)
		{
			float a = 0.025f;
			Color color = this.gui.color;
			float num = color.a = a;
			Color color2 = this.gui.color = color;
		}
	}

	public override bool IsFingerDown()
	{
		return this.lastFingerId != -1;
	}

	public override void LatchedFinger(int fingerId)
	{
		if (this.lastFingerId == fingerId)
		{
			this.ResetJoystick();
		}
	}

	public override void Update()
	{
		if (!Joystick.enumeratedJoysticks)
		{
			Joystick.joysticks = (((Joystick[])UnityEngine.Object.FindObjectsOfType(typeof(Joystick))) as Joystick[]);
			Joystick.enumeratedJoysticks = true;
		}
		int touchCount = Input.touchCount;
		if (this.tapTimeWindow > (float)0)
		{
			this.tapTimeWindow -= Time.deltaTime;
		}
		else
		{
			this.tapCount = 0;
		}
		if (touchCount == 0)
		{
			this.ResetJoystick();
		}
		else
		{
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				Vector2 vector = touch.position - this.guiTouchOffset;
				bool flag = false;
				if (this.touchPad)
				{
					if (this.touchZone.Contains(touch.position))
					{
						flag = true;
					}
				}
				else if (this.gui.HitTest(touch.position))
				{
					flag = true;
				}
				if (flag && (this.lastFingerId == -1 || this.lastFingerId != touch.fingerId))
				{
					if (this.touchPad)
					{
						float a = 0.15f;
						Color color = this.gui.color;
						float num = color.a = a;
						Color color2 = this.gui.color = color;
						this.lastFingerId = touch.fingerId;
						this.fingerDownPos = touch.position;
						this.fingerDownTime = Time.time;
					}
					this.lastFingerId = touch.fingerId;
					if (this.tapTimeWindow > (float)0)
					{
						this.tapCount++;
					}
					else
					{
						this.tapCount = 1;
						this.tapTimeWindow = Joystick.tapTimeDelta;
					}
					int j = 0;
					Joystick[] array = Joystick.joysticks;
					int length = array.Length;
					while (j < length)
					{
						if (array[j] != this)
						{
							array[j].LatchedFinger(touch.fingerId);
						}
						j++;
					}
				}
				if (this.lastFingerId == touch.fingerId)
				{
					if (touch.tapCount > this.tapCount)
					{
						this.tapCount = touch.tapCount;
					}
					if (this.touchPad)
					{
						this.position.x = Mathf.Clamp((touch.position.x - this.fingerDownPos.x) / (this.touchZone.width / (float)2), (float)-1, (float)1);
						this.position.y = Mathf.Clamp((touch.position.y - this.fingerDownPos.y) / (this.touchZone.height / (float)2), (float)-1, (float)1);
					}
					else
					{
						float x = Mathf.Clamp(vector.x, this.guiBoundary.min.x, this.guiBoundary.max.x);
						Rect pixelInset = this.gui.pixelInset;
						float num2 = pixelInset.x = x;
						Rect rect = this.gui.pixelInset = pixelInset;
						float y = Mathf.Clamp(vector.y, this.guiBoundary.min.y, this.guiBoundary.max.y);
						Rect pixelInset2 = this.gui.pixelInset;
						float num3 = pixelInset2.y = y;
						Rect rect2 = this.gui.pixelInset = pixelInset2;
					}
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						this.ResetJoystick();
					}
				}
			}
		}
		if (!this.touchPad)
		{
			this.position.x = (this.gui.pixelInset.x + this.guiTouchOffset.x - this.guiCenter.x) / this.guiTouchOffset.x;
			this.position.y = (this.gui.pixelInset.y + this.guiTouchOffset.y - this.guiCenter.y) / this.guiTouchOffset.y;
		}
		float num4 = Mathf.Abs(this.position.x);
		float num5 = Mathf.Abs(this.position.y);
		if (num4 < this.deadZone.x)
		{
			this.position.x = (float)0;
		}
		else if (this.normalize)
		{
			this.position.x = Mathf.Sign(this.position.x) * (num4 - this.deadZone.x) / ((float)1 - this.deadZone.x);
		}
		if (num5 < this.deadZone.y)
		{
			this.position.y = (float)0;
		}
		else if (this.normalize)
		{
			this.position.y = Mathf.Sign(this.position.y) * (num5 - this.deadZone.y) / ((float)1 - this.deadZone.y);
		}
	}

	public override void Main()
	{
	}
}
