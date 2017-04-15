using System;
using System.Collections.Generic;
using UnityEngine;

public class CurveMissile : MonoBehaviour
{
	public float randTime = 0.1f;

	public int maxRandCount = 2;

	public float startAngle = 40f;

	public int count = 5;

	public float speed = 18f;

	public float intervalTime = 0.2f;

	public GameObject effect;

	public GameObject targetPos;

	public bool play;

	private List<GameObject> effects = new List<GameObject>();

	private float curTime;

	private void Start()
	{
		this.curTime = Time.deltaTime;
	}

	private void Update()
	{
		if (this.play)
		{
			this.play = false;
			base.CancelInvoke();
			for (int i = 0; i < this.effects.Count; i++)
			{
				if (this.effects[i] != null)
				{
					UnityEngine.Object.Destroy(this.effects[i]);
				}
			}
			for (int j = 0; j < this.count; j++)
			{
				base.Invoke("StartEmit", this.intervalTime * (float)j);
			}
		}
	}

	private void StartEmit()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.effect, base.transform.position, Quaternion.identity) as GameObject;
		CurveMissileObj curveMissileObj = gameObject.AddComponent<CurveMissileObj>();
		curveMissileObj.randTime = this.randTime;
		curveMissileObj.maxRandCount = this.maxRandCount;
		curveMissileObj.startAngle = this.startAngle;
		curveMissileObj.speed = this.speed;
		curveMissileObj.TargetPos = this.targetPos.transform.position;
		this.effects.Add(gameObject);
		curveMissileObj.DoStart();
	}
}
