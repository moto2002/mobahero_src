using System;
using UnityEngine;

[Serializable]
public class Slash : MonoBehaviour
{
	public Vector3 speed;

	public Slash()
	{
		this.speed = Vector3.one;
	}

	public override void Update()
	{
		float x = this.transform.localScale.x + this.speed.x * Time.deltaTime;
		Vector3 localScale = this.transform.localScale;
		float num = localScale.x = x;
		Vector3 vector = this.transform.localScale = localScale;
		float y = this.transform.localScale.y + this.speed.y * Time.deltaTime;
		Vector3 localScale2 = this.transform.localScale;
		float num2 = localScale2.y = y;
		Vector3 vector2 = this.transform.localScale = localScale2;
		if (this.transform.localScale.y < (float)0)
		{
			int num3 = 0;
			Vector3 localScale3 = this.transform.localScale;
			float num4 = localScale3.y = (float)num3;
			Vector3 vector3 = this.transform.localScale = localScale3;
		}
	}

	public override void Main()
	{
	}
}
