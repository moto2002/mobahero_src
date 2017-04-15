using CodeStage.AdvancedFPSCounter.Labels;
using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class DeviceInfoCounterData : BaseCounterData
	{
		[HideInInspector]
		public string lastValue = string.Empty;

		[SerializeField]
		private bool cpuModel = true;

		[SerializeField]
		private bool gpuModel = true;

		[SerializeField]
		private bool ramSize = true;

		[SerializeField]
		private bool screenData = true;

		private bool inited;

		public bool CpuModel
		{
			get
			{
				return this.cpuModel;
			}
			set
			{
				if (this.cpuModel == value || !Application.isPlaying)
				{
					return;
				}
				this.cpuModel = value;
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool GpuModel
		{
			get
			{
				return this.gpuModel;
			}
			set
			{
				if (this.gpuModel == value || !Application.isPlaying)
				{
					return;
				}
				this.gpuModel = value;
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool RamSize
		{
			get
			{
				return this.ramSize;
			}
			set
			{
				if (this.ramSize == value || !Application.isPlaying)
				{
					return;
				}
				this.ramSize = value;
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool ScreenData
		{
			get
			{
				return this.screenData;
			}
			set
			{
				if (this.screenData == value || !Application.isPlaying)
				{
					return;
				}
				this.screenData = value;
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		internal DeviceInfoCounterData()
		{
			this.color = new Color32(172, 172, 172, 255);
			this.anchor = LabelAnchor.LowerLeft;
		}

		protected override void CacheCurrentColor()
		{
			this.colorCached = "<color=#" + AFPSCounter.Color32ToHex(this.color) + ">";
		}

		internal override void Activate()
		{
			if (!this.enabled || this.inited || !this.HasData())
			{
				return;
			}
			base.Activate();
			this.inited = true;
			if (this.main.OperationMode == AFPSCounterOperationMode.Normal && this.colorCached == null)
			{
				this.colorCached = "<color=#" + AFPSCounter.Color32ToHex(this.color) + ">";
			}
			if (this.text == null)
			{
				this.text = new StringBuilder();
			}
			else
			{
				this.text.Remove(0, this.text.Length);
			}
			this.UpdateValue();
		}

		internal override void Deactivate()
		{
			if (!this.inited)
			{
				return;
			}
			base.Deactivate();
			if (this.text != null)
			{
				this.text.Length = 0;
			}
			this.main.MakeDrawableLabelDirty(this.anchor);
			this.inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!this.inited && this.HasData())
			{
				this.Activate();
				return;
			}
			if (this.inited && !this.HasData())
			{
				this.Deactivate();
				return;
			}
			if (!this.enabled)
			{
				return;
			}
			bool flag = false;
			this.text.Remove(0, this.text.Length);
			if (this.cpuModel)
			{
				this.text.Append("CPU: ").Append(SystemInfo.processorType).Append(" (").Append(SystemInfo.processorCount).Append(" threads)");
				flag = true;
			}
			if (this.gpuModel)
			{
				if (flag)
				{
					this.text.Append(AFPSCounter.NEW_LINE);
				}
				this.text.Append("GPU: ").Append(SystemInfo.graphicsDeviceName);
				bool flag2 = false;
				int graphicsShaderLevel = SystemInfo.graphicsShaderLevel;
				if (graphicsShaderLevel == 20)
				{
					this.text.Append(" (SM: 2.0");
					flag2 = true;
				}
				else if (graphicsShaderLevel == 30)
				{
					this.text.Append(" (SM: 3.0");
					flag2 = true;
				}
				else if (graphicsShaderLevel == 40)
				{
					this.text.Append(" (SM: 4.0");
					flag2 = true;
				}
				else if (graphicsShaderLevel == 41)
				{
					this.text.Append(" (SM: 4.1");
					flag2 = true;
				}
				else if (graphicsShaderLevel == 50)
				{
					this.text.Append(" (SM: 5.0");
					flag2 = true;
				}
				int graphicsMemorySize = SystemInfo.graphicsMemorySize;
				if (graphicsMemorySize > 0)
				{
					if (flag2)
					{
						this.text.Append(", VRAM: ").Append(graphicsMemorySize).Append(" MB)");
					}
					else
					{
						this.text.Append("(VRAM: ").Append(graphicsMemorySize).Append(" MB)");
					}
				}
				else if (flag2)
				{
					this.text.Append(")");
				}
				flag = true;
			}
			if (this.ramSize)
			{
				if (flag)
				{
					this.text.Append(AFPSCounter.NEW_LINE);
				}
				int systemMemorySize = SystemInfo.systemMemorySize;
				if (systemMemorySize > 0)
				{
					this.text.Append("RAM: ").Append(systemMemorySize).Append(" MB");
					flag = true;
				}
			}
			if (this.screenData)
			{
				if (flag)
				{
					this.text.Append(AFPSCounter.NEW_LINE);
				}
				Resolution currentResolution = Screen.currentResolution;
				this.text.Append("Screen: ").Append(currentResolution.width).Append("x").Append(currentResolution.height).Append("@").Append(currentResolution.refreshRate).Append("Hz (window size: ").Append(Screen.width).Append("x").Append(Screen.height);
				float dpi = Screen.dpi;
				if (dpi <= 0f)
				{
					this.text.Append(")");
				}
				else
				{
					this.text.Append(", DPI: ").Append(dpi).Append(")");
				}
			}
			this.lastValue = this.text.ToString();
			if (this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				this.text.Insert(0, this.colorCached);
				this.text.Append("</color>");
			}
			else
			{
				this.text.Length = 0;
			}
			this.dirty = true;
		}

		private bool HasData()
		{
			return this.cpuModel || this.gpuModel || this.ramSize || this.screenData;
		}
	}
}
