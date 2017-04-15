using System;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class FPSCounterData : BaseCounterData
	{
		private const string COROUTINE_NAME = "UpdateFPSCounter";

		private const string FPS_TEXT_START = "<color=#{0}><b>FPS: ";

		private const string FPS_TEXT_END = "</b></color>";

		private const string MIN_TEXT_START = "\n<color=#{0}><b>MIN: ";

		private const string MIN_TEXT_END = "</b></color> ";

		private const string MAX_TEXT_START = "<color=#{0}><b>MAX: ";

		private const string MAX_TEXT_END = "</b></color>";

		private const string AVG_TEXT_START = " <color=#{0}><b>AVG: ";

		private const string AVG_TEXT_END = "</b></color>";

		public int warningLevelValue = 30;

		public int criticalLevelValue = 10;

		public bool resetAverageOnNewScene;

		public bool resetMinMaxOnNewScene;

		[HideInInspector]
		public int lastValue;

		[HideInInspector]
		public int lastAverageValue;

		[HideInInspector]
		public int lastMinimumValue = -1;

		[HideInInspector]
		public int lastMaximumValue = -1;

		[Range(0.1f, 10f), SerializeField]
		private float updateInterval = 0.5f;

		[SerializeField]
		private bool showAverage = true;

		[Range(0f, 100f), SerializeField]
		private int averageFromSamples = 100;

		[SerializeField]
		private bool showMinMax = true;

		[SerializeField]
		private Color colorWarning = new Color32(236, 224, 88, 255);

		[SerializeField]
		private Color colorCritical = new Color32(249, 91, 91, 255);

		internal int newValue;

		private string colorCachedMin;

		private string colorCachedMax;

		private string colorCachedAvg;

		private string colorWarningCached;

		private string colorWarningCachedMin;

		private string colorWarningCachedMax;

		private string colorWarningCachedAvg;

		private string colorCriticalCached;

		private string colorCriticalCachedMin;

		private string colorCriticalCachedMax;

		private string colorCriticalCachedAvg;

		private bool inited;

		private int currentAverageSamples;

		private float currentAverageRaw;

		private float[] accumulatedAverageSamples;

		public float UpdateInterval
		{
			get
			{
				return this.updateInterval;
			}
			set
			{
				if (Math.Abs(this.updateInterval - value) < 0.001f || !Application.isPlaying)
				{
					return;
				}
				this.updateInterval = value;
				if (!this.enabled)
				{
					return;
				}
				this.RestartCoroutine();
			}
		}

		public bool ShowAverage
		{
			get
			{
				return this.showAverage;
			}
			set
			{
				if (this.showAverage == value || !Application.isPlaying)
				{
					return;
				}
				this.showAverage = value;
				if (!this.enabled)
				{
					return;
				}
				if (!this.showAverage)
				{
					this.ResetAverage();
				}
				base.Refresh();
			}
		}

		public int AverageFromSamples
		{
			get
			{
				return this.averageFromSamples;
			}
			set
			{
				if (this.averageFromSamples == value || !Application.isPlaying)
				{
					return;
				}
				this.averageFromSamples = value;
				if (!this.enabled)
				{
					return;
				}
				if (this.averageFromSamples > 0)
				{
					if (this.accumulatedAverageSamples == null)
					{
						this.accumulatedAverageSamples = new float[this.averageFromSamples];
					}
					else if (this.accumulatedAverageSamples.Length != this.averageFromSamples)
					{
						Array.Resize<float>(ref this.accumulatedAverageSamples, this.averageFromSamples);
					}
				}
				else
				{
					this.accumulatedAverageSamples = null;
				}
				this.ResetAverage();
				base.Refresh();
			}
		}

		public bool ShowMinMax
		{
			get
			{
				return this.showMinMax;
			}
			set
			{
				if (this.showMinMax == value || !Application.isPlaying)
				{
					return;
				}
				this.showMinMax = value;
				if (!this.enabled)
				{
					return;
				}
				if (!this.showMinMax)
				{
					this.ResetMinMax();
				}
				base.Refresh();
			}
		}

		public Color ColorWarning
		{
			get
			{
				return this.colorWarning;
			}
			set
			{
				if (this.colorWarning == value || !Application.isPlaying)
				{
					return;
				}
				this.colorWarning = value;
				if (!this.enabled)
				{
					return;
				}
				this.CacheWarningColor();
				base.Refresh();
			}
		}

		public Color ColorCritical
		{
			get
			{
				return this.colorCritical;
			}
			set
			{
				if (this.colorCritical == value || !Application.isPlaying)
				{
					return;
				}
				this.colorCritical = value;
				if (!this.enabled)
				{
					return;
				}
				this.CacheCriticalColor();
				base.Refresh();
			}
		}

		internal FPSCounterData()
		{
			this.color = new Color32(85, 218, 102, 255);
		}

		public void ResetAverage()
		{
			this.lastAverageValue = 0;
			this.currentAverageSamples = 0;
			this.currentAverageRaw = 0f;
			if (this.averageFromSamples > 0 && this.accumulatedAverageSamples != null)
			{
				Array.Clear(this.accumulatedAverageSamples, 0, this.accumulatedAverageSamples.Length);
			}
		}

		public void ResetMinMax()
		{
			this.lastMinimumValue = -1;
			this.lastMaximumValue = -1;
			this.UpdateValue(true);
			this.dirty = true;
		}

		internal override void Activate()
		{
			if (!this.enabled || this.inited)
			{
				return;
			}
			base.Activate();
			this.inited = true;
			this.lastValue = 0;
			if (this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				if (this.colorCached == null)
				{
					this.CacheCurrentColor();
				}
				if (this.colorWarningCached == null)
				{
					this.CacheWarningColor();
				}
				if (this.colorCriticalCached == null)
				{
					this.CacheCriticalColor();
				}
				this.text.Append(this.colorCriticalCached).Append("0").Append("</b></color>");
				this.dirty = true;
			}
			this.main.StartCoroutine("UpdateFPSCounter");
		}

		internal override void Deactivate()
		{
			if (!this.inited)
			{
				return;
			}
			base.Deactivate();
			this.main.StopCoroutine("UpdateFPSCounter");
			this.ResetMinMax();
			this.ResetAverage();
			this.lastValue = 0;
			this.inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!this.enabled)
			{
				return;
			}
			if (this.lastValue != this.newValue || force)
			{
				this.lastValue = this.newValue;
				this.dirty = true;
			}
			int num = 0;
			if (this.showAverage)
			{
				if (this.averageFromSamples == 0)
				{
					this.currentAverageSamples++;
					this.currentAverageRaw += ((float)this.lastValue - this.currentAverageRaw) / (float)this.currentAverageSamples;
				}
				else
				{
					if (this.accumulatedAverageSamples == null)
					{
						this.accumulatedAverageSamples = new float[this.averageFromSamples];
						this.ResetAverage();
					}
					this.accumulatedAverageSamples[this.currentAverageSamples % this.averageFromSamples] = (float)this.lastValue;
					this.currentAverageSamples++;
					this.currentAverageRaw = this.GetAverageFromAccumulatedSamples();
				}
				num = Mathf.RoundToInt(this.currentAverageRaw);
				if (this.lastAverageValue != num || force)
				{
					this.lastAverageValue = num;
					this.dirty = true;
				}
			}
			if (this.showMinMax && this.dirty)
			{
				if (this.lastMinimumValue == -1)
				{
					this.lastMinimumValue = this.lastValue;
				}
				else if (this.lastValue < this.lastMinimumValue)
				{
					this.lastMinimumValue = this.lastValue;
					this.dirty = true;
				}
				if (this.lastMaximumValue == -1)
				{
					this.lastMaximumValue = this.lastValue;
				}
				else if (this.lastValue > this.lastMaximumValue)
				{
					this.lastMaximumValue = this.lastValue;
					this.dirty = true;
				}
			}
			if (this.dirty && this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				string colorCached;
				if (this.lastValue >= this.warningLevelValue)
				{
					colorCached = this.colorCached;
				}
				else if (this.lastValue <= this.criticalLevelValue)
				{
					colorCached = this.colorCriticalCached;
				}
				else
				{
					colorCached = this.colorWarningCached;
				}
				this.text.Length = 0;
				this.text.Append(colorCached).Append(this.lastValue).Append("</b></color>");
				if (this.showAverage)
				{
					if (num >= this.warningLevelValue)
					{
						colorCached = this.colorCachedAvg;
					}
					else if (num <= this.criticalLevelValue)
					{
						colorCached = this.colorCriticalCachedAvg;
					}
					else
					{
						colorCached = this.colorWarningCachedAvg;
					}
					this.text.Append(colorCached).Append(num).Append("</b></color>");
				}
				if (this.showMinMax)
				{
					if (this.lastMinimumValue >= this.warningLevelValue)
					{
						colorCached = this.colorCachedMin;
					}
					else if (this.lastMinimumValue <= this.criticalLevelValue)
					{
						colorCached = this.colorCriticalCachedMin;
					}
					else
					{
						colorCached = this.colorWarningCachedMin;
					}
					this.text.Append(colorCached).Append(this.lastMinimumValue).Append("</b></color> ");
					if (this.lastMaximumValue >= this.warningLevelValue)
					{
						colorCached = this.colorCachedMax;
					}
					else if (this.lastMaximumValue <= this.criticalLevelValue)
					{
						colorCached = this.colorCriticalCachedMax;
					}
					else
					{
						colorCached = this.colorWarningCachedMax;
					}
					this.text.Append(colorCached).Append(this.lastMaximumValue).Append("</b></color>");
				}
			}
		}

		protected override void CacheCurrentColor()
		{
			string arg = AFPSCounter.Color32ToHex(this.color);
			this.colorCached = string.Format("<color=#{0}><b>FPS: ", arg);
			this.colorCachedMin = string.Format("\n<color=#{0}><b>MIN: ", arg);
			this.colorCachedMax = string.Format("<color=#{0}><b>MAX: ", arg);
			this.colorCachedAvg = string.Format(" <color=#{0}><b>AVG: ", arg);
		}

		protected void CacheWarningColor()
		{
			string arg = AFPSCounter.Color32ToHex(this.colorWarning);
			this.colorWarningCached = string.Format("<color=#{0}><b>FPS: ", arg);
			this.colorWarningCachedMin = string.Format("\n<color=#{0}><b>MIN: ", arg);
			this.colorWarningCachedMax = string.Format("<color=#{0}><b>MAX: ", arg);
			this.colorWarningCachedAvg = string.Format(" <color=#{0}><b>AVG: ", arg);
		}

		protected void CacheCriticalColor()
		{
			string arg = AFPSCounter.Color32ToHex(this.colorCritical);
			this.colorCriticalCached = string.Format("<color=#{0}><b>FPS: ", arg);
			this.colorCriticalCachedMin = string.Format("\n<color=#{0}><b>MIN: ", arg);
			this.colorCriticalCachedMax = string.Format("<color=#{0}><b>MAX: ", arg);
			this.colorCriticalCachedAvg = string.Format(" <color=#{0}><b>AVG: ", arg);
		}

		private void RestartCoroutine()
		{
			this.main.StopCoroutine("UpdateFPSCounter");
			this.main.StartCoroutine("UpdateFPSCounter");
		}

		private float GetAverageFromAccumulatedSamples()
		{
			float num = 0f;
			for (int i = 0; i < this.averageFromSamples; i++)
			{
				num += this.accumulatedAverageSamples[i];
			}
			float result;
			if (this.currentAverageSamples < this.averageFromSamples)
			{
				result = num / (float)this.currentAverageSamples;
			}
			else
			{
				result = num / (float)this.averageFromSamples;
			}
			return result;
		}
	}
}
