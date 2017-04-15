using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GoldViewFps : MonoBehaviour
{
	private const int sampleLen = 4;

	public UILabel fpsLbl;

	public UILabel delayLbl;

	public bool isForceShow = true;

	private float[] statisticArr;

	private float accum;

	private float frames;

	private float fps;

	private long sampleCnt;

	private Coroutine _updateLabel;

	private void ResetFpsData()
	{
		this.accum = 0f;
		this.frames = 0f;
		this.fps = 30f;
		this.statisticArr = new float[4];
	}

	private void UpdateFPSText(float fps)
	{
		if (this.fpsLbl == null)
		{
			return;
		}
		this.fpsLbl.text = fps.ToString("f0");
		if ((double)fps < 29.5)
		{
			this.fpsLbl.color = Color.yellow;
		}
		else if (fps < 10f)
		{
			this.fpsLbl.color = Color.red;
		}
		else
		{
			this.fpsLbl.color = Color.green;
		}
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
			if (GlobalSettings.Instance.isHighFPS)
			{
				if (this.fps > 60f)
				{
					this.fps = 60f;
				}
			}
			else if (this.fps > 30f)
			{
				this.fps = 30f;
			}
			this.UpdateFPSText(this.fps);
			this.frames = 0f;
			this.accum = 0f;
		}
	}

	private void Start()
	{
		this.ResetFpsData();
		this._updateLabel = base.StartCoroutine(this.UpdateLabel_Coroutine());
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
		GoldViewFps.<UpdateLabel_Coroutine>c__IteratorD0 <UpdateLabel_Coroutine>c__IteratorD = new GoldViewFps.<UpdateLabel_Coroutine>c__IteratorD0();
		<UpdateLabel_Coroutine>c__IteratorD.<>f__this = this;
		return <UpdateLabel_Coroutine>c__IteratorD;
	}

	private void Update()
	{
		if (GlobalSettings.Instance.isShowFPS || this.isForceShow)
		{
			if (this.fpsLbl != null && !this.fpsLbl.transform.parent.gameObject.activeInHierarchy && !this.delayLbl.transform.parent.gameObject.activeInHierarchy)
			{
				this.fpsLbl.transform.parent.gameObject.SetActive(true);
				this.delayLbl.transform.parent.gameObject.SetActive(true);
			}
			this.UpdateDelayText();
		}
		else if (this.fpsLbl.transform.parent.gameObject.activeInHierarchy && this.delayLbl.transform.parent.gameObject.activeInHierarchy)
		{
			this.fpsLbl.transform.parent.gameObject.SetActive(false);
			this.delayLbl.transform.parent.gameObject.SetActive(false);
		}
	}
}
