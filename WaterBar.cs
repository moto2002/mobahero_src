using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WaterBar : MonoBehaviour
{
	public float count = 1f;

	public Transform trans;

	public Material mat;

	public float width = 4f;

	public float amp = 0.002f;

	public float widthTint = 0.02f;

	private float oriAmp;

	private float t;

	private float mul = 1f;

	private float period = 3f;

	private void Start()
	{
		this.SetWidth(this.count);
	}

	public void SetWidth(float count)
	{
		float num = count;
		Vector3 localScale = new Vector3(this.width * (num + (num - 1f) * 0.09f), this.trans.localScale.y, 1f);
		this.trans.localScale = localScale;
		this.mat.SetFloat("scal", localScale.x * 0.005f);
		this.mat.SetFloat("scalY", localScale.y * 0.005f);
		this.mat.SetFloat("widthTint", this.widthTint * 2f / num);
		this.mat.SetFloat("amp", this.amp * 2f / num);
		float @float = this.mat.GetFloat("pt");
		this.SetPercent(@float);
	}

	public void SetPercent(float percent)
	{
		percent = Mathf.Min(0.999f, percent);
		float @float = this.mat.GetFloat("pt");
		this.mat.SetFloat("pt", percent);
		if (Mathf.Abs(@float - percent) > 0.01f)
		{
			this.t = 0f;
			base.StartCoroutine("Amplify");
		}
	}

	[DebuggerHidden]
	private IEnumerator Amplify()
	{
		WaterBar.<Amplify>c__Iterator1DC <Amplify>c__Iterator1DC = new WaterBar.<Amplify>c__Iterator1DC();
		<Amplify>c__Iterator1DC.<>f__this = this;
		return <Amplify>c__Iterator1DC;
	}
}
