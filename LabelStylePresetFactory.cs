using System;
using UnityEngine;

public static class LabelStylePresetFactory
{
	public static BaseLabelStylePreset GetGradientPreset(ELabelStyle style)
	{
		switch (style)
		{
		case ELabelStyle.Season1_Orange:
			return new GradientPreset_Season1Orange();
		case ELabelStyle.Season1_Purple:
			return new GradientPreset_Season1Purple();
		case ELabelStyle.Season1_Blue:
			return new GradientPreset_Season1Blue();
		case ELabelStyle.Season1_Rainbow:
			return new AllochroicPreset_Rainbow();
		case ELabelStyle.RainbowTest:
			return new AllochroicPreset_RainbowForTest();
		default:
			return new ColorPreset(Color.white);
		}
	}
}
