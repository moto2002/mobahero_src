using System;
using UnityEngine;

public class CamFocus
{
	private static CamFocus _instance;

	public static CamFocus Inst
	{
		get
		{
			if (CamFocus._instance == null)
			{
				CamFocus._instance = new CamFocus();
			}
			return CamFocus._instance;
		}
	}

	public void Check(Camera cam, Transform role, Transform target, Units targetUnit, float speed = 3.5f)
	{
		if (target == null || !targetUnit.isLive)
		{
			this.Focus2Role(cam, role, speed);
		}
		else
		{
			float screenDist = this.GetScreenDist(cam, role.position, target.position);
			float screenDistRate = this.GetScreenDistRate(screenDist);
			if (screenDistRate <= 0.6f)
			{
				this.Focus2Mid(cam, role, target, speed);
			}
			else if (screenDistRate < 1f)
			{
				Vector3 lhs = target.position - role.position;
				float num = Vector3.Dot(lhs, target.forward);
				float num2 = Vector3.Dot(lhs, role.forward);
				bool flag = num > 0f;
				bool flag2 = num2 < 0f;
				if (flag2)
				{
					this.Focus2Role(cam, role, speed);
				}
				else if (flag)
				{
					this.Focus2Role(cam, target, speed);
				}
				else
				{
					this.Focus2Mid(cam, role, target, speed);
				}
			}
			else
			{
				this.Focus2Role(cam, role, speed);
			}
		}
	}

	public void Focus2Role(Camera cam, Transform to, float speed)
	{
		Vector3 vector = this.GetDir(cam, to.position);
		vector += to.forward * 2.5f;
		vector = new Vector3(vector.x, 0f, vector.z);
		speed /= vector.magnitude + 0.001f;
		this.Focus(cam.transform, vector, speed);
	}

	public void Focus2Mid(Camera cam, Transform p1, Transform p2, float speed)
	{
		Vector3 dir = this.GetDir(cam, (p1.position + p2.position) / 2f);
		speed /= dir.magnitude + 0.001f;
		speed = Mathf.Max(0.5f, speed);
		this.Focus(cam.transform, dir, speed);
	}

	public void Focus(Transform cam, Vector3 dir, float speed)
	{
		cam.position = Vector3.Lerp(cam.position, cam.position + dir, Time.deltaTime * speed);
	}

	public Vector3 GetDir(Camera cam, Vector3 groundPoint)
	{
		Vector3 result = Vector3.zero;
		int num = LayerMask.NameToLayer("Ground");
		RaycastHit raycastHit;
		bool flag = Physics.Raycast(cam.transform.position, cam.transform.forward, out raycastHit, 100f, 1 << num);
		if (flag)
		{
			result = groundPoint - raycastHit.point;
			result = new Vector3(result.x, 0f, result.z);
		}
		return result;
	}

	public float GetScreenDist(Camera cam, Vector3 p1, Vector3 p2)
	{
		Vector3 vector = cam.WorldToScreenPoint(p1);
		Vector3 vector2 = cam.WorldToScreenPoint(p2);
		return Mathf.Abs(vector.x - vector2.x);
	}

	public float GetScreenDistRate(float width)
	{
		return width / (float)Screen.width;
	}
}
