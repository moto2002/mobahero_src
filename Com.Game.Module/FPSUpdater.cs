using MobaHeros.Pvp;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class FPSUpdater : MonoBehaviour
	{
		public float updateInterval = 0.5f;

		private float accum;

		private int frames;

		private float timeleft;

		private float last_rendertime;

		private float last_fps;

		[SerializeField]
		private UILabel fpsText;

		[SerializeField]
		private UILabel timeText;

		[SerializeField]
		private UILabel sampleKey;

		[SerializeField]
		private UILabel sampleText;

		[SerializeField]
		private UILabel targetFPS;

		[SerializeField]
		private UILabel serverTime;

		private float m_targetFPS;

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
			if ((double)(this.last_rendertime - Time.deltaTime) > 0.001)
			{
				this.timeText.text = (Time.deltaTime * 1000f).ToString();
			}
			if (LevelManager.Instance.IsPvpBattleType && NetWorkHelper.Instance.client != null && NetWorkHelper.Instance.client.pvpserver_peer != null)
			{
				this.timeText.text = UnitsSnapReporter.Instance.NetworkDelayInMs + ", " + NetWorkHelper.Instance.client.pvpserver_peer.RoundTripTimeVariance;
				this.sampleKey.text = NetWorkHelper.Instance.client.pvpserver_peer.ServerTimeInMilliSeconds.ToString();
				this.sampleText.text = NetWorkHelper.Instance.client.pvpserver_peer.TimePingInterval.ToString();
			}
			this.last_rendertime = Time.deltaTime;
			this.timeleft -= Time.deltaTime;
			this.accum += Time.timeScale / Time.deltaTime;
			this.frames++;
			if ((double)this.timeleft <= 0.0)
			{
				float fps = this.accum / (float)this.frames;
				this.UpdateFPSText(fps);
				this.accum = 0f;
				this.frames = 0;
				this.timeleft = this.updateInterval;
			}
			this.UpdateTargetFPS();
			this.UpdateServerTime();
		}

		public void UpdateSampleText(string key, string time)
		{
		}

		public void UpdateFPSText(float fps)
		{
			this.fpsText.text = fps.ToString();
			if (fps < 30f)
			{
				this.fpsText.material.color = Color.yellow;
			}
			else if (fps < 10f)
			{
				this.fpsText.material.color = Color.red;
			}
			else
			{
				this.fpsText.material.color = Color.green;
			}
		}

		public void UpdateTargetFPS()
		{
			float num = (float)Application.targetFrameRate;
			if (this.m_targetFPS != num)
			{
				this.m_targetFPS = num;
				this.targetFPS.text = this.m_targetFPS.ToString();
			}
		}

		public void UpdateServerTime()
		{
			if (LevelManager.Instance.IsPvpBattleType)
			{
				if (UnitsSnapReporter.Instance.IsSyncServerTime)
				{
					DateTime dateTime = new DateTime(UnitsSnapReporter.Instance.SyncTicks);
					this.serverTime.text = dateTime.ToString("HH:mm:ss:ffff");
				}
				else
				{
					this.serverTime.text = string.Empty;
				}
			}
			else
			{
				this.serverTime.text = string.Empty;
			}
		}
	}
}
