using System;
using UnityEngine;

[AddComponentMenu("Perflexive Media/Floatate")]
[Serializable]
public class Floatate : MonoBehaviour
{
	public float bobSpeed;

	public float bobHeight;

	public float bobOffset;

	public float PrimaryRot;

	public float SecondaryRot;

	public float TertiaryRot;

	private float bottom;

	public Floatate()
	{
		this.bobSpeed = 3f;
		this.bobHeight = 0.5f;
		this.PrimaryRot = 80f;
		this.SecondaryRot = 40f;
		this.TertiaryRot = 20f;
	}

	public override void Awake()
	{
		if (this.bobSpeed < (float)0)
		{
			Debug.LogWarning("Negative object bobSpeed value! May result in undesired behavior. Continuing anyway.", this.gameObject);
		}
		if (this.bobHeight < (float)0)
		{
			Debug.LogWarning("Negative object bobHeight value! May result in undesired behavior. Continuing anyway.", this.gameObject);
		}
		this.bottom = this.transform.position.y;
	}

	public override void Update()
	{
		this.transform.Rotate(new Vector3((float)0, this.PrimaryRot, (float)0) * Time.deltaTime, Space.World);
		this.transform.Rotate(new Vector3(this.SecondaryRot, (float)0, (float)0) * Time.deltaTime, Space.Self);
		this.transform.Rotate(new Vector3((float)0, (float)0, this.TertiaryRot) * Time.deltaTime, Space.Self);
		float y = this.bottom + (Mathf.Cos((Time.time + this.bobOffset) * this.bobSpeed) + (float)1) / (float)2 * this.bobHeight;
		Vector3 position = this.transform.position;
		float num = position.y = y;
		Vector3 vector = this.transform.position = position;
	}

	public override void Main()
	{
	}
}
