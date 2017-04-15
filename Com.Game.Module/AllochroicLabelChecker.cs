using Com.Game.Utils;
using System;
using UnityEngine;

namespace Com.Game.Module
{
	[RequireComponent(typeof(UILabel))]
	public class AllochroicLabelChecker : MonoBehaviour
	{
		public UILabel theLabel;

		private ELabelStyle recordStyle;

		private ELabelStyle recordStyleRecord;

		private bool applyGradient;

		private Color topGradientColorRecord;

		private Color bottomGradientColorRecord;

		private Color effectColor;

		private Vector2 effectDistance;

		private UILabel.Effect effectStyle;

		private Color labelColor;

		private bool isRecorded;

		private void Awake()
		{
			if (this.theLabel == null)
			{
				this.theLabel = base.transform.GetComponent<UILabel>();
			}
		}

		private void Start()
		{
			this.StoreLabelStyle(false);
		}

		public void RenderLabel(int rank)
		{
			this.StoreLabelStyle(false);
			this.CheckCharmRank(rank);
			this.ApplyStyle(this.recordStyle);
		}

		public void FriendRenderLabel(int rank)
		{
			this.CheckCharmRank(rank);
			this.FriendApplyStyle(this.recordStyle);
		}

		[ContextMenu("Refresh")]
		public void Refresh()
		{
		}

		public void Refresh(ELabelStyle _style)
		{
			if (_style == this.recordStyleRecord)
			{
				return;
			}
			this.ApplyStyle(_style);
		}

		private void ApplyStyle(ELabelStyle style)
		{
			if (style == ELabelStyle.None)
			{
				this.ApplyStoredStyle();
				return;
			}
			ToolsFacade.Instance.ApplyLabelStyle(this.theLabel, style);
			this.recordStyleRecord = style;
		}

		private void FriendApplyStyle(ELabelStyle style)
		{
			if (style == ELabelStyle.None)
			{
				this.theLabel.applyGradient = false;
				this.theLabel.color = Color.white;
				return;
			}
			ToolsFacade.Instance.ApplyLabelStyle(this.theLabel, style);
			this.recordStyleRecord = style;
		}

		public void StoreLabelStyle(bool isForced = true)
		{
			if (this.isRecorded && !isForced)
			{
				return;
			}
			this.applyGradient = this.theLabel.applyGradient;
			this.topGradientColorRecord = this.theLabel.gradientTop;
			this.bottomGradientColorRecord = this.theLabel.gradientBottom;
			this.effectColor = this.theLabel.effectColor;
			this.effectDistance = this.theLabel.effectDistance;
			this.effectStyle = this.theLabel.effectStyle;
			this.labelColor = this.theLabel.color;
			this.isRecorded = true;
		}

		private void ApplyStoredStyle()
		{
			if (!this.isRecorded)
			{
				ClientLogger.Error("尚未存储样式");
				return;
			}
			this.theLabel.applyGradient = this.applyGradient;
			this.theLabel.gradientTop = this.topGradientColorRecord;
			this.theLabel.gradientBottom = this.bottomGradientColorRecord;
			this.theLabel.effectColor = this.effectColor;
			this.theLabel.effectDistance = this.effectDistance;
			this.theLabel.effectStyle = this.effectStyle;
			this.theLabel.color = this.labelColor;
		}

		private void CheckCharmRank(int _rank)
		{
			this.theLabel.gradientTop = Color.white;
			this.theLabel.gradientBottom = Color.white;
			this.theLabel.color = Color.white;
			if (_rank > 0 && _rank <= 5)
			{
				this.recordStyle = ELabelStyle.Season1_Rainbow;
			}
			else if (_rank >= 6 && _rank <= 15)
			{
				this.recordStyle = ELabelStyle.Season1_Orange;
			}
			else if (_rank >= 16 && _rank <= 30)
			{
				this.recordStyle = ELabelStyle.Season1_Purple;
			}
			else if (_rank >= 31 && _rank <= 50)
			{
				this.recordStyle = ELabelStyle.Season1_Blue;
			}
			else
			{
				this.recordStyle = ELabelStyle.None;
			}
		}
	}
}
