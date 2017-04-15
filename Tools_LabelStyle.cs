using System;
using UnityEngine;

public static class Tools_LabelStyle
{
	public static bool IsAllochroic(this ToolsFacade facade)
	{
		return true;
	}

	private static TweenGradient _GetTweenScript(UILabel targetLabel)
	{
		return (!(targetLabel == null)) ? (targetLabel.GetComponent<TweenGradient>() ?? targetLabel.gameObject.AddComponent<TweenGradient>()) : null;
	}

	private static void _ResetLabel(UILabel targetLabel)
	{
		Color color = Color.white;
		targetLabel.gradientBottom = color;
		color = color;
		targetLabel.gradientTop = color;
		targetLabel.color = color;
		targetLabel.effectStyle = UILabel.Effect.None;
	}

	public static ELabelStyle GetLabelStyle(this ToolsFacade facade)
	{
		return (ELabelStyle)UnityEngine.Random.Range(4, 6);
	}

	public static void ApplyLabelStyle(this ToolsFacade facade, UILabel targetLabel, ELabelStyle style)
	{
		BaseLabelStylePreset gradientPreset = LabelStylePresetFactory.GetGradientPreset(style);
		if (gradientPreset is AllochroicPreset)
		{
			Tools_LabelStyle._ApplyTweenGradient(targetLabel, gradientPreset as AllochroicPreset);
			return;
		}
		if (gradientPreset is ColorPreset)
		{
			Tools_LabelStyle._ApplyColor(targetLabel, gradientPreset as ColorPreset);
		}
		else if (gradientPreset is GradientPreset)
		{
			Tools_LabelStyle._ApplyGradient(targetLabel, gradientPreset as GradientPreset);
		}
		if (targetLabel.GetComponent<TweenGradient>() != null)
		{
			UnityEngine.Object.Destroy(targetLabel.GetComponent<TweenGradient>());
		}
	}

	private static void _ApplyTweenGradient(UILabel targetLabel, AllochroicPreset style)
	{
		Tools_LabelStyle._ResetLabel(targetLabel);
		TweenGradient tweenGradient = Tools_LabelStyle._GetTweenScript(targetLabel);
		tweenGradient.SetStyle(style);
		tweenGradient.ActivateTweenCoroutines();
		targetLabel.effectStyle = style.effectType;
		targetLabel.effectColor = style.effectColor;
		targetLabel.effectDistance = Vector2.one * 2f;
	}

	private static void _ApplyColor(UILabel targetLabel, ColorPreset preset)
	{
		Tools_LabelStyle._ResetLabel(targetLabel);
		targetLabel.color = preset.color;
		targetLabel.applyGradient = false;
	}

	private static void _ApplyGradient(UILabel targetLabel, GradientPreset preset)
	{
		Tools_LabelStyle._ResetLabel(targetLabel);
		targetLabel.gradientTop = preset.topColor;
		targetLabel.gradientBottom = preset.bottomColor;
		targetLabel.effectStyle = preset.effectType;
		targetLabel.effectColor = preset.effectColor;
		targetLabel.effectDistance = Vector2.one * 2f;
		targetLabel.applyGradient = true;
	}
}
