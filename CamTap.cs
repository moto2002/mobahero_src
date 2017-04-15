using System;
using UnityEngine;

public class CamTap
{
	private static CamTap _instance;

	private int mask;

	private float camH;

	private Vector3 camPos0;

	private Vector3 scr0;

	private Vector3 wPos;

	private Vector3 wPosN;

	private bool tapping;

	private Vector3 dir = Vector3.zero;

	private Vector3 preDir;

	private bool sliding;

	private float slideTime;

	private float slidePeriod = 1f;

	private Vector3 slideDir = Vector3.zero;

	public static CamTap Instance
	{
		get
		{
			if (CamTap._instance == null)
			{
				CamTap._instance = new CamTap();
			}
			return CamTap._instance;
		}
	}

	public CamTap()
	{
		this.mask = LayerMask.GetMask(new string[]
		{
			"Ground"
		});
	}

	public void MoveTo(Transform tr, Camera cam)
	{
		if (cam == null)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.sliding = false;
			this.camH = tr.position.y;
			this.camPos0 = tr.position;
			this.scr0 = Input.mousePosition / 2f;
			Ray ray = cam.ScreenPointToRay(this.scr0);
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(ray, out raycastHit, 1000f, this.mask);
			if (flag)
			{
				this.wPos = raycastHit.point;
				this.tapping = true;
			}
			else
			{
				RaycastHit[] array = Physics.RaycastAll(ray);
				RaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					RaycastHit raycastHit2 = array2[i];
				}
			}
			return;
		}
		if (this.sliding)
		{
			this.Slide(tr, this.slideDir, this.camH);
			return;
		}
		if (!this.tapping)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.sliding = false;
			Ray ray2 = cam.ScreenPointToRay(Input.mousePosition / 2f);
			RaycastHit raycastHit3;
			bool flag2 = Physics.Raycast(ray2, out raycastHit3, 1000f, this.mask);
			if (flag2)
			{
				this.wPosN = raycastHit3.point;
				this.dir = this.wPosN - this.wPos;
				this.preDir = this.dir;
				tr.position -= this.dir;
				tr.position = new Vector3(tr.position.x, this.camH, tr.position.z);
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			this.tapping = false;
			if (this.preDir.magnitude > 0.01f)
			{
				this.slideDir = this.preDir.normalized;
				this.sliding = true;
				this.slideTime = 0f;
			}
		}
	}

	private void Slide(Transform tr, Vector3 dir, float camHeight)
	{
		this.slideTime += Time.deltaTime;
		float num = this.slideTime / this.slidePeriod;
		if (num > 1f)
		{
			this.sliding = false;
			return;
		}
		float d = Mathf.Lerp(4f, 0.3f, num);
		tr.position -= dir * Time.deltaTime * d;
		tr.position = new Vector3(tr.position.x, camHeight, tr.position.z);
	}
}
