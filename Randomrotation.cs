using System;
using UnityEngine;

[Serializable]
public class Randomrotation : MonoBehaviour
{
	public override void Start()
	{
		float x = UnityEngine.Random.rotation.x;
		Quaternion rotation = this.gameObject.transform.rotation;
		float num = rotation.x = x;
		Quaternion quaternion = this.gameObject.transform.rotation = rotation;
		float y = UnityEngine.Random.rotation.y;
		Quaternion rotation2 = this.gameObject.transform.rotation;
		float num2 = rotation2.y = y;
		Quaternion quaternion2 = this.gameObject.transform.rotation = rotation2;
		float z = UnityEngine.Random.rotation.z;
		Quaternion rotation3 = this.gameObject.transform.rotation;
		float num3 = rotation3.z = z;
		Quaternion quaternion3 = this.gameObject.transform.rotation = rotation3;
	}

	public override void Main()
	{
	}
}
