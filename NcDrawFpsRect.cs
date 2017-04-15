using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class NcDrawFpsRect : MonoBehaviour
{
	public bool centerTop = true;

	public Rect startRect = new Rect(0f, 0f, 75f, 50f);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private Color color = Color.white;

	private string sFPS = string.Empty;

	private GUIStyle style;

	private void Start()
	{
		base.StartCoroutine(this.FPS());
	}

	private void Update()
	{
		this.accum += Time.timeScale / Time.deltaTime;
		this.frames++;
	}

	[DebuggerHidden]
	private IEnumerator FPS()
	{
		NcDrawFpsRect.<FPS>c__Iterator10 <FPS>c__Iterator = new NcDrawFpsRect.<FPS>c__Iterator10();
		<FPS>c__Iterator.<>f__this = this;
		return <FPS>c__Iterator;
	}

	private void OnGUI()
	{
		if (this.style == null)
		{
			this.style = new GUIStyle(GUI.skin.label);
			this.style.normal.textColor = Color.white;
			this.style.alignment = TextAnchor.MiddleCenter;
		}
		GUI.color = ((!this.updateColor) ? Color.white : this.color);
		Rect clientRect = this.startRect;
		if (this.centerTop)
		{
			clientRect.x += (float)(Screen.width / 2) - clientRect.width / 2f;
		}
		this.startRect = GUI.Window(0, clientRect, new GUI.WindowFunction(this.DoMyWindow), string.Empty);
		if (this.centerTop)
		{
			this.startRect.x = this.startRect.x - ((float)(Screen.width / 2) - clientRect.width / 2f);
		}
	}

	private void DoMyWindow(int windowID)
	{
		GUI.Label(new Rect(0f, 0f, this.startRect.width, this.startRect.height), this.sFPS + " FPS", this.style);
		if (this.allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		}
	}
}
