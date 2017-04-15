using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class PingSignManage : MonoBehaviour
{
	private const int sampleLen = 4;

	public UILabel delayLbl;

	public UILabel delayMsLbl;

	public UISprite delaySp;

	private float[] statisticArr;

	private float accum;

	private float frames;

	private float fps;

	private long sampleCnt;

	private Coroutine _updateLabel;

	public Transform showValueTra;

	private void ResetFpsData()
	{
		this.accum = 0f;
		this.frames = 0f;
		this.fps = 30f;
		this.statisticArr = new float[4];
	}

	private void UpdateDelayText()
	{
		if (this.delayLbl == null)
		{
			return;
		}
		this.frames += 1f;
		this.accum += Time.unscaledDeltaTime;
		if (this.frames >= 15f)
		{
			this.statisticArr[(int)(checked((IntPtr)(this.sampleCnt % 4L)))] = this.accum;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < 4; i++)
			{
				if (this.statisticArr[i] != 0f)
				{
					num += 15f;
					num2 += this.statisticArr[i];
				}
			}
			this.fps = num / num2 + 0.8f;
			if (this.fps > 30f)
			{
				this.fps = 30f;
			}
			this.frames = 0f;
			this.accum = 0f;
		}
	}

	private void Start()
	{
		this.ResetFpsData();
		this._updateLabel = base.StartCoroutine(this.UpdateLabel_Coroutine());
	}

	public void ChangeShowValueTra()
	{
		this.showValueTra.gameObject.SetActive(!this.showValueTra.gameObject.activeSelf);
	}

	private void OnDisable()
	{
		if (this._updateLabel != null)
		{
			base.StopCoroutine(this._updateLabel);
		}
		this._updateLabel = null;
	}

	[DebuggerHidden]
	private IEnumerator UpdateLabel_Coroutine()
	{
		PingSignManage.<UpdateLabel_Coroutine>c__Iterator1D4 <UpdateLabel_Coroutine>c__Iterator1D = new PingSignManage.<UpdateLabel_Coroutine>c__Iterator1D4();
		<UpdateLabel_Coroutine>c__Iterator1D.<>f__this = this;
		return <UpdateLabel_Coroutine>c__Iterator1D;
	}

	private void Update()
	{
		this.UpdateDelayText();
	}
}
