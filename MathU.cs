using System;

public class MathU
{
	public static float scale(float value, float min, float max, float min2, float max2)
	{
		return min2 + (value - min) / (max - min) * (max2 - min2);
	}
}
