using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public class MemoryCounterData : BaseCounterData
	{
		public const int MEMORY_DIVIDER = 1048576;

		private const string COROUTINE_NAME = "UpdateMemoryCounter";

		private const string TEXT_START = "<color=#{0}><b>";

		private const string LINE_START_TOTAL = "MEM (total): ";

		private const string LINE_START_ALLOCATED = "MEM (alloc): ";

		private const string LINE_START_MONO = "MEM (mono): ";

		private const string LINE_END = " MB";

		private const string TEXT_END = "</b></color>";

		[HideInInspector]
		public uint lastTotalValue;

		[HideInInspector]
		public uint lastAllocatedValue;

		[HideInInspector]
		public long lastMonoValue;

		[Range(0.1f, 10f), SerializeField]
		private float updateInterval = 1f;

		[SerializeField]
		private bool preciseValues;

		[SerializeField]
		private bool totalReserved = true;

		[SerializeField]
		private bool allocated = true;

		[SerializeField]
		private bool monoUsage = true;

		private bool inited;

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

		public bool PreciseValues
		{
			get
			{
				return this.preciseValues;
			}
			set
			{
				if (this.preciseValues == value || !Application.isPlaying)
				{
					return;
				}
				this.preciseValues = value;
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool TotalReserved
		{
			get
			{
				return this.totalReserved;
			}
			set
			{
				if (this.totalReserved == value || !Application.isPlaying)
				{
					return;
				}
				this.totalReserved = value;
				if (!this.totalReserved)
				{
					this.lastTotalValue = 0u;
				}
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool Allocated
		{
			get
			{
				return this.allocated;
			}
			set
			{
				if (this.allocated == value || !Application.isPlaying)
				{
					return;
				}
				this.allocated = value;
				if (!this.allocated)
				{
					this.lastAllocatedValue = 0u;
				}
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		public bool MonoUsage
		{
			get
			{
				return this.monoUsage;
			}
			set
			{
				if (this.monoUsage == value || !Application.isPlaying)
				{
					return;
				}
				this.monoUsage = value;
				if (!this.monoUsage)
				{
					this.lastMonoValue = 0L;
				}
				if (!this.enabled)
				{
					return;
				}
				base.Refresh();
			}
		}

		internal MemoryCounterData()
		{
			this.color = new Color32(234, 238, 101, 255);
		}

		protected override void CacheCurrentColor()
		{
			this.colorCached = string.Format("<color=#{0}><b>", AFPSCounter.Color32ToHex(this.color));
		}

		internal override void Activate()
		{
			if (!this.enabled || this.inited || !this.HasData())
			{
				return;
			}
			base.Activate();
			this.inited = true;
			this.lastTotalValue = 0u;
			this.lastAllocatedValue = 0u;
			this.lastMonoValue = 0L;
			if (this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				if (this.colorCached == null)
				{
					this.colorCached = string.Format("<color=#{0}><b>", AFPSCounter.Color32ToHex(this.color));
				}
				if (this.text == null)
				{
					this.text = new StringBuilder(200);
				}
				else
				{
					this.text.Length = 0;
				}
				this.text.Append(this.colorCached);
				if (this.totalReserved)
				{
					if (this.preciseValues)
					{
						this.text.Append("MEM (total): ").AppendFormat("{0:F}", 0).Append(" MB");
					}
					else
					{
						this.text.Append("MEM (total): ").Append(0).Append(" MB");
					}
				}
				if (this.allocated)
				{
					if (this.text.Length > 0)
					{
						this.text.Append(AFPSCounter.NEW_LINE);
					}
					if (this.preciseValues)
					{
						this.text.Append("MEM (alloc): ").AppendFormat("{0:F}", 0).Append(" MB");
					}
					else
					{
						this.text.Append("MEM (alloc): ").Append(0).Append(" MB");
					}
				}
				if (this.monoUsage)
				{
					if (this.text.Length > 0)
					{
						this.text.Append(AFPSCounter.NEW_LINE);
					}
					if (this.preciseValues)
					{
						this.text.Append("MEM (mono): ").AppendFormat("{0:F}", 0).Append(" MB");
					}
					else
					{
						this.text.Append("MEM (mono): ").Append(0).Append(" MB");
					}
				}
				this.text.Append("</b></color>");
				this.dirty = true;
			}
			this.main.StartCoroutine("UpdateMemoryCounter");
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
			this.main.StopCoroutine("UpdateMemoryCounter");
			this.main.MakeDrawableLabelDirty(this.anchor);
			this.inited = false;
		}

		internal override void UpdateValue(bool force)
		{
			if (!this.enabled)
			{
				return;
			}
			if (force)
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
			}
			if (this.totalReserved)
			{
				uint totalReservedMemory = Profiler.GetTotalReservedMemory();
				uint num = 0u;
				bool flag;
				if (this.preciseValues)
				{
					flag = (this.lastTotalValue != totalReservedMemory);
				}
				else
				{
					num = totalReservedMemory / 1048576u;
					flag = (this.lastTotalValue != num);
				}
				if (flag || force)
				{
					if (this.preciseValues)
					{
						this.lastTotalValue = totalReservedMemory;
					}
					else
					{
						this.lastTotalValue = num;
					}
					this.dirty = true;
				}
			}
			if (this.allocated)
			{
				uint totalAllocatedMemory = Profiler.GetTotalAllocatedMemory();
				uint num2 = 0u;
				bool flag2;
				if (this.preciseValues)
				{
					flag2 = (this.lastAllocatedValue != totalAllocatedMemory);
				}
				else
				{
					num2 = totalAllocatedMemory / 1048576u;
					flag2 = (this.lastAllocatedValue != num2);
				}
				if (flag2 || force)
				{
					if (this.preciseValues)
					{
						this.lastAllocatedValue = totalAllocatedMemory;
					}
					else
					{
						this.lastAllocatedValue = num2;
					}
					this.dirty = true;
				}
			}
			if (this.monoUsage)
			{
				long totalMemory = GC.GetTotalMemory(false);
				long num3 = 0L;
				bool flag3;
				if (this.preciseValues)
				{
					flag3 = (this.lastMonoValue != totalMemory);
				}
				else
				{
					num3 = totalMemory / 1048576L;
					flag3 = (this.lastMonoValue != num3);
				}
				if (flag3 || force)
				{
					if (this.preciseValues)
					{
						this.lastMonoValue = totalMemory;
					}
					else
					{
						this.lastMonoValue = num3;
					}
					this.dirty = true;
				}
			}
			if (this.dirty && this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				bool flag4 = false;
				this.text.Length = 0;
				this.text.Append(this.colorCached);
				if (this.totalReserved)
				{
					this.text.Append("MEM (total): ");
					if (this.preciseValues)
					{
						this.text.AppendFormat("{0:F}", this.lastTotalValue / 1048576f);
					}
					else
					{
						this.text.Append(this.lastTotalValue);
					}
					this.text.Append(" MB");
					flag4 = true;
				}
				if (this.allocated)
				{
					if (flag4)
					{
						this.text.Append(AFPSCounter.NEW_LINE);
					}
					this.text.Append("MEM (alloc): ");
					if (this.preciseValues)
					{
						this.text.AppendFormat("{0:F}", this.lastAllocatedValue / 1048576f);
					}
					else
					{
						this.text.Append(this.lastAllocatedValue);
					}
					this.text.Append(" MB");
					flag4 = true;
				}
				if (this.monoUsage)
				{
					if (flag4)
					{
						this.text.Append(AFPSCounter.NEW_LINE);
					}
					this.text.Append("MEM (mono): ");
					if (this.preciseValues)
					{
						this.text.AppendFormat("{0:F}", (float)this.lastMonoValue / 1048576f);
					}
					else
					{
						this.text.Append(this.lastMonoValue);
					}
					this.text.Append(" MB");
				}
				this.text.Append("</b></color>");
			}
		}

		private void RestartCoroutine()
		{
			this.main.StopCoroutine("UpdateMemoryCounter");
			this.main.StartCoroutine("UpdateMemoryCounter");
		}

		private bool HasData()
		{
			return this.totalReserved || this.allocated || this.monoUsage;
		}
	}
}
