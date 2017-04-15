using System;
using UnityEngine;

[Serializable]
public class FPSCounter : MonoBehaviour
{
	public float updateInterval;

	public int x_location;

	public int y_location;

	private double lastInterval;

	private int frames;

	private float fps;

	public FPSCounter()
	{
		this.updateInterval = 0.5f;
		this.x_location = 5;
		this.y_location = 5;
	}

	public override void Awake()
	{
		this.useGUILayout = false;
	}

	public override void OnGUI()
	{
		GUI.Label(new Rect((float)(Screen.width - this.x_location), (float)(Screen.height - this.y_location), (float)100, (float)30), "FPS: " + this.fps.ToString("f2"));
	}

	public override void Start()
	{
		this.lastInterval = (double)Time.realtimeSinceStartup;
		this.frames = 0;
	}

	public override void Update()
	{
		this.frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if ((double)realtimeSinceStartup > this.lastInterval + (double)this.updateInterval)
		{
			this.fps = (float)((double)this.frames / ((double)realtimeSinceStartup - this.lastInterval));
			this.frames = 0;
			this.lastInterval = (double)realtimeSinceStartup;
		}
	}

	public override void Main()
	{
	}
}
