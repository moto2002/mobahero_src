using System;
using UnityEngine;

namespace MobaTools
{
	public class FPSUpdater : MonoBehaviour
	{
		public float updateInterval = 0.5f;

		private float accum;

		private int frames;

		private float timeleft;

		private float deltatime_last;

		private string fpsText;

		private string timeText;

		private void Start()
		{
			this.timeleft = this.updateInterval;
			Screen.lockCursor = false;
			Screen.showCursor = true;
		}

		private void OnDestroy()
		{
		}

		private void Update()
		{
			if ((double)(this.deltatime_last - Time.deltaTime) > 0.002)
			{
				this.timeText = (Time.deltaTime * 1000f).ToString();
			}
			this.deltatime_last = Time.deltaTime;
			this.timeleft -= Time.deltaTime;
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames++;
			if ((double)this.timeleft <= 0.0)
			{
				this.fpsText = (this.accum / (float)this.frames).ToString();
				this.timeleft = this.updateInterval;
				this.accum = 0f;
				this.frames = 0;
			}
		}

		private void OnGUI()
		{
			GUI.Label(new Rect((float)(Screen.width / 2), 0f, 100f, 20f), this.fpsText + " fps");
			GUI.Label(new Rect((float)(Screen.width / 2), 20f, 100f, 20f), this.timeText + " ms");
		}
	}
}
