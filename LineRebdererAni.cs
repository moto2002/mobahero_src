using System;
using UnityEngine;

public class LineRebdererAni : MonoBehaviour
{
	public int lengthOfLineRenderer = 20;

	public Vector3 DingDian;

	public Vector3 SuiJi;

	public Vector3 PingPong;

	public float GenSuiSpeed = 3f;

	public float RaoDongSpeed = 0.1f;

	public bool Follow;

	public float DingDianJuli = 1f;

	private LineRenderer lineR;

	private Vector3[] posDian;

	private void Start()
	{
		this.lineR = base.GetComponent<LineRenderer>();
		this.lineR.SetVertexCount(this.lengthOfLineRenderer);
		this.posDian = new Vector3[this.lengthOfLineRenderer];
		for (int i = 0; i < this.lengthOfLineRenderer; i++)
		{
			if (i == 0)
			{
				this.posDian[i] = base.transform.position;
				this.lineR.SetPosition(i, this.posDian[i]);
			}
			else
			{
				this.posDian[i] = this.posDian[0] + this.DingDian / (float)(this.lengthOfLineRenderer - 1) * (float)i;
				this.lineR.SetPosition(i, this.posDian[i]);
			}
		}
	}

	private void Update()
	{
		this.DingDianGenSui();
		for (int i = 0; i < this.lengthOfLineRenderer; i++)
		{
			if (i == 0)
			{
				this.posDian[i] = base.transform.position;
				this.lineR.SetPosition(i, this.posDian[i]);
			}
			else
			{
				this.posDian[i] = Vector3.Lerp(this.posDian[i], this.posDian[i - 1] + this.DingDian / (float)(this.lengthOfLineRenderer - 1) + this.PingPongPianYi(i), Time.deltaTime * this.GenSuiSpeed);
				this.posDian[i] += this.SuiJiPianYi();
				this.lineR.SetPosition(i, this.posDian[i]);
			}
		}
	}

	private Vector3 SuiJiPianYi()
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = UnityEngine.Random.Range(-this.SuiJi.x, this.SuiJi.x);
		vector.y = UnityEngine.Random.Range(-this.SuiJi.y, this.SuiJi.y);
		vector.z = UnityEngine.Random.Range(-this.SuiJi.z, this.SuiJi.z);
		vector = Quaternion.FromToRotation(Vector3.forward, this.DingDian - this.posDian[0]) * vector;
		return vector;
	}

	private Vector3 PingPongPianYi(int i)
	{
		if (this.RaoDongSpeed == 0f)
		{
			this.RaoDongSpeed = 0.001f;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f);
		vector.x = Mathf.Sin((float)i + Time.time * this.RaoDongSpeed) * this.PingPong.x;
		vector.y = Mathf.Sin((float)i + this.RaoDongSpeed * Time.time) * this.PingPong.y;
		vector.z = Mathf.Sin((float)i + this.RaoDongSpeed * Time.time) * this.PingPong.z;
		vector = Quaternion.FromToRotation(Vector3.forward, this.DingDian - this.posDian[0]) * vector;
		return vector;
	}

	private void DingDianGenSui()
	{
		if (this.Follow)
		{
			this.DingDian = base.transform.forward * this.DingDianJuli;
		}
	}
}
