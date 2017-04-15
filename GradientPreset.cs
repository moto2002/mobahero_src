using System;
using UnityEngine;

public abstract class GradientPreset : BaseLabelStylePreset
{
	public Color topColor;

	public Color bottomColor;

	public UILabel.Effect effectType;

	public Color effectColor;

	public GradientPreset(Color _topColor, Color _bottomColor, UILabel.Effect _effectType, Color _effectColor)
	{
		this.topColor = _topColor;
		this.bottomColor = _bottomColor;
		this.effectType = _effectType;
		this.effectColor = _effectColor;
	}

	public GradientPreset(Color _topColor, Color _bottomColor)
	{
		this.topColor = _topColor;
		this.bottomColor = _bottomColor;
		this.effectType = UILabel.Effect.None;
		this.effectColor = Color.black;
	}
}
