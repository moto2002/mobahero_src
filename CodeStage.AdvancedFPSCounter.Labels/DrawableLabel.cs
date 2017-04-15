using System;
using System.Text;
using UnityEngine;

namespace CodeStage.AdvancedFPSCounter.Labels
{
	internal class DrawableLabel
	{
		public LabelAnchor anchor;

		public GUIText guiText;

		public StringBuilder newText;

		public bool dirty;

		private Vector2 pixelOffset;

		private Font font;

		private int fontSize;

		private float lineSpacing;

		public DrawableLabel(LabelAnchor anchor, Vector2 pixelOffset, Font font, int fontSize, float lineSpacing)
		{
			this.anchor = anchor;
			this.pixelOffset = pixelOffset;
			this.font = font;
			this.fontSize = fontSize;
			this.lineSpacing = lineSpacing;
			this.NormalizeOffset();
			this.newText = new StringBuilder(1000);
		}

		internal void CheckAndUpdate()
		{
			if (this.newText.Length > 0)
			{
				if (this.guiText == null)
				{
					GameObject gameObject = new GameObject(this.anchor.ToString(), new Type[]
					{
						typeof(GUIText)
					});
					this.guiText = gameObject.guiText;
					if (this.anchor == LabelAnchor.UpperLeft)
					{
						gameObject.transform.position = new Vector3(0f, 1f);
						this.guiText.anchor = TextAnchor.UpperLeft;
						this.guiText.alignment = TextAlignment.Left;
					}
					else if (this.anchor == LabelAnchor.UpperRight)
					{
						gameObject.transform.position = new Vector3(1f, 1f);
						this.guiText.anchor = TextAnchor.UpperRight;
						this.guiText.alignment = TextAlignment.Right;
					}
					else if (this.anchor == LabelAnchor.LowerLeft)
					{
						gameObject.transform.position = new Vector3(0f, 0f);
						this.guiText.anchor = TextAnchor.LowerLeft;
						this.guiText.alignment = TextAlignment.Left;
					}
					else if (this.anchor == LabelAnchor.LowerRight)
					{
						gameObject.transform.position = new Vector3(1f, 0f);
						this.guiText.anchor = TextAnchor.LowerRight;
						this.guiText.alignment = TextAlignment.Right;
					}
					this.guiText.pixelOffset = this.pixelOffset;
					this.guiText.font = this.font;
					this.guiText.fontSize = this.fontSize;
					this.guiText.lineSpacing = this.lineSpacing;
					gameObject.layer = AFPSCounter.Instance.gameObject.layer;
					gameObject.tag = AFPSCounter.Instance.gameObject.tag;
					gameObject.transform.parent = AFPSCounter.Instance.transform;
				}
				if (this.dirty)
				{
					this.guiText.text = this.newText.ToString();
					this.dirty = false;
				}
				this.newText.Length = 0;
			}
			else if (this.guiText != null)
			{
				UnityEngine.Object.DestroyImmediate(this.guiText.gameObject);
			}
		}

		internal void Clear()
		{
			this.newText.Length = 0;
			if (this.guiText != null)
			{
				UnityEngine.Object.Destroy(this.guiText.gameObject);
			}
		}

		internal void Dispose()
		{
			this.Clear();
			this.newText = null;
		}

		internal void ChangeFont(Font labelsFont)
		{
			this.font = labelsFont;
			if (this.guiText != null)
			{
				this.guiText.font = this.font;
			}
		}

		internal void ChangeFontSize(int newSize)
		{
			this.fontSize = newSize;
			if (this.guiText != null)
			{
				this.guiText.fontSize = this.fontSize;
			}
		}

		internal void ChangeOffset(Vector2 newPixelOffset)
		{
			this.pixelOffset = newPixelOffset;
			this.NormalizeOffset();
			if (this.guiText != null)
			{
				this.guiText.pixelOffset = this.pixelOffset;
			}
		}

		private void NormalizeOffset()
		{
			if (this.anchor == LabelAnchor.UpperLeft)
			{
				this.pixelOffset.y = -this.pixelOffset.y;
			}
			else if (this.anchor == LabelAnchor.UpperRight)
			{
				this.pixelOffset.x = -this.pixelOffset.x;
				this.pixelOffset.y = -this.pixelOffset.y;
			}
			else if (this.anchor == LabelAnchor.LowerRight)
			{
				this.pixelOffset.x = -this.pixelOffset.x;
			}
		}

		internal void ChangeLineSpacing(float lineSpacing)
		{
			this.lineSpacing = lineSpacing;
			if (this.guiText != null)
			{
				this.guiText.lineSpacing = lineSpacing;
			}
		}
	}
}
