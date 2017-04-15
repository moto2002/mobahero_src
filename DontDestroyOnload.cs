using System;
using UnityEngine;

[Serializable]
public class DontDestroyOnload : MonoBehaviour
{
	public override void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this.transform.gameObject);
	}

	public override void Main()
	{
	}
}
