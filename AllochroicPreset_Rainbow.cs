using System;
using UnityEngine;

public class AllochroicPreset_Rainbow : AllochroicPreset
{
	public AllochroicPreset_Rainbow()
	{
		Color color = new Color32(249, 180, 0, 255);
		Color color2 = new Color32(251, 88, 0, 255);
		Color color3 = new Color32(250, 0, 25, 255);
		Color color4 = new Color32(249, 0, 251, 255);
		Color color5 = new Color32(174, 0, 250, 255);
		this.topColors = new Color[]
		{
			color,
			color2,
			color3,
			color4,
			color5
		};
		this.bottomColors = new Color[]
		{
			color5,
			color,
			color2,
			color3,
			color4
		};
		this.duration = 3f;
		this.effectType = UILabel.Effect.Outline;
		this.effectColor = new Color32(42, 13, 0, 255);
	}
}
