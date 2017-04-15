using System;
using UnityEngine;

namespace Com.Game.Module
{
	public class BarrageItem : MonoBehaviour
	{
		[SerializeField]
		private UILabel mLabel;

		[SerializeField]
		private UISprite mSprite;

		[SerializeField]
		private TweenPosition mTween;

		public string text
		{
			get
			{
				return this.mLabel.text;
			}
			set
			{
				this.mLabel.text = value;
			}
		}

		public Color32 color
		{
			get
			{
				return this.mLabel.color;
			}
			set
			{
				this.mLabel.color = value;
			}
		}

		public Color32 outlineColor
		{
			get
			{
				return this.mLabel.effectColor;
			}
			set
			{
				this.mLabel.effectColor = value;
			}
		}

		public int fontSize
		{
			get
			{
				return this.mLabel.fontSize;
			}
			set
			{
				this.mLabel.fontSize = value;
			}
		}

		public int length
		{
			get
			{
				return this.mLabel.width;
			}
		}

		public bool isGradient
		{
			set
			{
				this.mLabel.applyGradient = value;
			}
		}

		public Color32 gradientTop
		{
			set
			{
				this.mLabel.gradientTop = value;
			}
		}

		public Color32 gradientBottom
		{
			set
			{
				this.mLabel.gradientBottom = value;
			}
		}

		public string spriteName
		{
			get
			{
				return this.mSprite.spriteName;
			}
			set
			{
				this.mSprite.spriteName = value;
			}
		}

		public UIAtlas atlas
		{
			get
			{
				return this.mSprite.atlas;
			}
			set
			{
				this.mSprite.atlas = value;
			}
		}

		public Vector3 tweenFrom
		{
			set
			{
				this.mTween.from = new Vector3(value.x, value.y, value.z);
				this.mTween.to = new Vector3(value.x, value.y, value.z);
			}
		}

		public float tweenFrom_x
		{
			get
			{
				return this.mTween.from.x;
			}
			set
			{
				this.mTween.from = new Vector3(value, this.mTween.from.y, this.mTween.from.z);
			}
		}

		public float tween_y
		{
			set
			{
				this.mTween.from = new Vector3(this.mTween.from.x, value, this.mTween.from.z);
				this.mTween.to = new Vector3(this.mTween.to.x, value, this.mTween.to.z);
			}
		}

		public float tween_z
		{
			set
			{
				this.mTween.from = new Vector3(this.mTween.from.x, this.mTween.from.y, value);
				this.mTween.to = new Vector3(this.mTween.to.x, this.mTween.to.y, value);
			}
		}

		public float tweenTo_x
		{
			get
			{
				return this.mTween.to.x;
			}
			set
			{
				this.mTween.to = new Vector3(value, this.mTween.to.y, this.mTween.to.z);
			}
		}

		public float pathDistance_x
		{
			get
			{
				return this.tweenFrom_x - this.tweenTo_x;
			}
		}

		public float tweenDuration
		{
			set
			{
				if (value > 0f)
				{
					this.mTween.duration = value;
				}
			}
		}

		public void ComposeText(string prefix, string content)
		{
			this.mLabel.text = string.Format("{0}<{1}", prefix, content);
		}

		public void Play()
		{
			this.mTween.enabled = true;
			this.mTween.ResetToBeginning();
			this.mTween.PlayForward();
		}

		public void RecycleToPool()
		{
			base.transform.parent = base.transform.parent.parent.parent.FindChild("Pool");
		}
	}
}
