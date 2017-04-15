using System;
using UnityEngine;

public class AllochroicPreset_RainbowForTest : AllochroicPreset
{
	public AllochroicPreset_RainbowForTest()
	{
		Color color = new Color32(252, 108, 128, 255);
		Color color2 = new Color32(252, 109, 241, 255);
		Color color3 = new Color32(143, 111, 250, 255);
		Color color4 = new Color32(113, 200, 252, 255);
		Color color5 = new Color32(115, 254, 131, 255);
		Color color6 = new Color32(236, 254, 119, 255);
		Color color7 = new Color32(252, 175, 114, 255);
		this.topColors = new Color[]
		{
			color,
			color2,
			color3,
			color4,
			color5,
			color6,
			color7
		};
		this.bottomColors = new Color[]
		{
			color7,
			color,
			color2,
			color3,
			color4,
			color5,
			color6
		};
		this.duration = 2f;
	}
}
