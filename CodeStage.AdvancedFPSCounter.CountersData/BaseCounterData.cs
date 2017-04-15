using CodeStage.AdvancedFPSCounter.Labels;
using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.CountersData
{
	[Serializable]
	public abstract class BaseCounterData
	{
		[SerializeField]
		protected bool enabled = true;

		[SerializeField]
		protected LabelAnchor anchor;

		[SerializeField]
		protected Color color;

		protected string colorCached;

		internal StringBuilder text;

		internal bool dirty;

		protected AFPSCounter main;

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.enabled == value || !Application.isPlaying)
				{
					return;
				}
				this.enabled = value;
				if (this.enabled)
				{
					this.Activate();
				}
				else
				{
					this.Deactivate();
				}
				this.main.UpdateTexts();
			}
		}

		public LabelAnchor Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				if (this.anchor == value || !Application.isPlaying)
				{
					return;
				}
				LabelAnchor labelAnchor = this.anchor;
				this.anchor = value;
				if (!this.enabled)
				{
					return;
				}
				this.dirty = true;
				this.main.MakeDrawableLabelDirty(labelAnchor);
				this.main.UpdateTexts();
			}
		}

		public Color Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color == value || !Application.isPlaying)
				{
					return;
				}
				this.color = value;
				if (!this.enabled)
				{
					return;
				}
				this.CacheCurrentColor();
				this.Refresh();
			}
		}

		public void Refresh()
		{
			if (!this.enabled || !Application.isPlaying)
			{
				return;
			}
			this.UpdateValue(true);
			this.main.UpdateTexts();
		}

		protected abstract void CacheCurrentColor();

		internal virtual void UpdateValue()
		{
			this.UpdateValue(false);
		}

		internal virtual void UpdateValue(bool force)
		{
		}

		internal void Init(AFPSCounter reference)
		{
			this.main = reference;
		}

		internal void Dispose()
		{
			this.main = null;
			if (this.text != null)
			{
				this.text.Remove(0, this.text.Length);
				this.text = null;
			}
		}

		internal virtual void Activate()
		{
			if (this.main.OperationMode == AFPSCounterOperationMode.Normal)
			{
				if (this.text == null)
				{
					this.text = new StringBuilder(100);
				}
				else
				{
					this.text.Remove(0, this.text.Length);
				}
			}
		}

		internal virtual void Deactivate()
		{
			if (this.text != null)
			{
				this.text.Remove(0, this.text.Length);
			}
			this.main.MakeDrawableLabelDirty(this.anchor);
		}
	}
}
