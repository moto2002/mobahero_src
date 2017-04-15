using System;
using UnityEngine;

public class ColorPreset : BaseLabelStylePreset
{
	public Color color;

	public ColorPreset(Color col)
	{
		this.color = col;
	}
}
