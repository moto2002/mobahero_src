using System;
using UnityEngine;

public class GPUParticleCtrl : MonoBehaviour
{
	public GameObject mObj;

	public bool ctrlColor;

	public bool ctrlSize;

	public bool ctrlTime;

	public float fadeinTime;

	public float fadeoutTime;

	public float timeLen = 1f;

	public int loop;

	public bool tocontinue;

	public bool auto;

	private float mTime;

	private float cTime;

	private Material mMat;

	public Vector3 startSize = Vector3.zero;

	public Vector3 endSize = Vector3.zero;

	public Color startColor = Color.white;

	public Color endColor = Color.white;

	public AnimationCurve mCurveClr;

	public AnimationCurve mCurveSize;

	private float fadeintime_inv = 1f;

	private float fadeouttime_inv = 1f;

	private float playtime_inv = 1f;

	private float totletime_inv = 1f;

	private float dtime;

	private int fadestate;

	private float totletime;

	private float ltime;

	public float lagtime;

	private Color mclr;

	private Vector3 msize;

	public void clone(GPUParticleCtrl gpu)
	{
		gpu.ctrlColor = this.ctrlColor;
		gpu.ctrlSize = this.ctrlSize;
		gpu.ctrlTime = this.ctrlTime;
		gpu.fadeinTime = this.fadeinTime;
		gpu.fadeoutTime = this.fadeoutTime;
		gpu.timeLen = this.timeLen;
		gpu.loop = this.loop;
		gpu.tocontinue = this.tocontinue;
		gpu.auto = this.auto;
		gpu.mTime = this.mTime;
		gpu.cTime = this.cTime;
		gpu.startSize = this.startSize;
		gpu.endSize = this.endSize;
		gpu.startColor = this.startColor;
		gpu.endColor = this.endColor;
		gpu.mCurveClr = this.mCurveClr;
		gpu.mCurveSize = this.mCurveSize;
		gpu.fadeintime_inv = this.fadeintime_inv;
		gpu.fadeouttime_inv = this.fadeouttime_inv;
		gpu.playtime_inv = this.playtime_inv;
		gpu.totletime_inv = this.totletime_inv;
		gpu.dtime = this.dtime;
		gpu.fadestate = this.fadestate;
		gpu.totletime_inv = this.totletime;
		gpu.mclr = this.mclr;
		gpu.msize = this.msize;
		gpu.lagtime = this.lagtime;
	}

	public void start()
	{
		if (this.mObj == null || this.mMat == null)
		{
			return;
		}
		this.mTime = 0f;
		this.cTime = 0f;
		this.fadestate = 0;
		this.totletime = this.fadeinTime + this.timeLen + this.fadeoutTime;
		this.totletime_inv = 1f / this.totletime;
		if (this.mMat.HasProperty("_mTime"))
		{
			this.mMat.SetFloat("_mTime", this.mTime);
		}
		if (this.auto)
		{
			this.fadestate = -1;
			if (this.mMat.HasProperty("_manual"))
			{
				this.mMat.SetFloat("_manual", 0f);
			}
			if (this.mMat.HasProperty("_fadein"))
			{
				this.mMat.SetFloat("_fadein", 0f);
			}
			if (this.mMat.HasProperty("_fadeout"))
			{
				this.mMat.SetFloat("_fadeout", 0f);
			}
		}
		else
		{
			if (this.fadeinTime > 0f)
			{
				this.fadeintime_inv = 1f / this.fadeinTime;
				if (this.mMat.HasProperty("_fadein"))
				{
					this.mMat.SetFloat("_fadein", 1f);
				}
				if (this.mMat.HasProperty("_manual"))
				{
					this.mMat.SetFloat("_manual", 1f);
				}
				if (this.mMat.HasProperty("_fadeout"))
				{
					this.mMat.SetFloat("_fadeout", 0f);
				}
			}
			else
			{
				this.startplay();
			}
			if (this.fadeoutTime > 0f)
			{
				this.fadeouttime_inv = 1f / this.fadeoutTime;
			}
			if (this.timeLen > 0f)
			{
				this.playtime_inv = 1f / this.timeLen;
			}
		}
	}

	public void end()
	{
		this.fadestate = -1;
		if (this.mMat.HasProperty("_mTime"))
		{
			this.mMat.SetFloat("_mTime", 1f);
		}
	}

	public void startplay()
	{
		this.mTime = 0f;
		this.fadestate = 1;
		if (this.mMat.HasProperty("_fadein"))
		{
			this.mMat.SetFloat("_fadein", 0f);
		}
		if (this.mMat.HasProperty("_fadeout"))
		{
			this.mMat.SetFloat("_fadeout", 0f);
		}
		if (this.mMat.HasProperty("_manual"))
		{
			this.mMat.SetFloat("_manual", 1f);
		}
	}

	public void startend()
	{
		this.mTime = 0f;
		this.fadestate = 2;
		if (this.fadeoutTime > 0f)
		{
			if (this.mMat.HasProperty("_fadein"))
			{
				this.mMat.SetFloat("_fadein", 0f);
			}
			if (this.mMat.HasProperty("_fadeout"))
			{
				this.mMat.SetFloat("_fadeout", 1f);
			}
			if (this.mMat.HasProperty("_manual"))
			{
				this.mMat.SetFloat("_manual", 1f);
			}
		}
		else
		{
			this.fadestate = -1;
			if (this.mMat.HasProperty("_fadein"))
			{
				this.mMat.SetFloat("_fadein", 0f);
			}
			if (this.mMat.HasProperty("_fadeout"))
			{
				this.mMat.SetFloat("_fadeout", 0f);
			}
			if (this.auto)
			{
				if (this.mMat.HasProperty("_manual"))
				{
					this.mMat.SetFloat("_manual", 0f);
				}
			}
			else if (this.mMat.HasProperty("_manual"))
			{
				this.mMat.SetFloat("_manual", 1f);
			}
		}
	}

	private void OnEnable()
	{
		if (this.mObj == null)
		{
			this.mObj = base.gameObject;
		}
		this.mMat = this.mObj.GetComponent<Renderer>().material;
		this.mTime = -1f;
		this.ltime = Time.realtimeSinceStartup + this.lagtime;
	}

	public void setCtrlObject(GameObject gm)
	{
		this.mObj = gm;
		this.mMat = this.mObj.GetComponent<Renderer>().sharedMaterial;
		this.mTime = -1f;
		this.ltime = Time.realtimeSinceStartup + this.lagtime;
	}

	private void Start()
	{
	}

	public void updata(float dlt)
	{
		if (this.ltime > 0f)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (this.ltime <= realtimeSinceStartup)
			{
				this.ltime = 0f;
				this.start();
			}
			return;
		}
		if (this.lagtime <= 0f && this.mTime < 0f)
		{
			this.start();
			return;
		}
		if (this.fadestate == 0)
		{
			this.dtime = this.mTime * this.fadeintime_inv;
			this.mMat.SetFloat("_fadeintime", this.dtime);
			this.mTime += dlt;
			this.cTime += dlt;
			if (this.mTime >= this.fadeinTime)
			{
				this.mTime = this.fadeinTime;
				this.dtime = this.mTime * this.fadeintime_inv;
				this.mMat.SetFloat("_fadeintime", this.dtime);
				this.startplay();
			}
		}
		else if (this.fadestate == 1)
		{
			this.dtime = this.mTime * this.playtime_inv;
			this.mTime += dlt;
			this.cTime += dlt;
			if (this.mTime >= this.timeLen)
			{
				this.mTime = (float)(1 - this.loop) * this.timeLen;
				this.dtime = this.mTime * this.playtime_inv;
				if (this.loop == 0)
				{
					this.startend();
				}
			}
		}
		else if (this.fadestate == 2)
		{
			this.dtime = this.mTime * this.fadeouttime_inv;
			this.mMat.SetFloat("_fadeouttime", this.dtime);
			this.mTime += dlt;
			this.cTime += dlt;
			if (this.mTime >= this.fadeoutTime)
			{
				this.mTime = this.fadeoutTime;
				this.dtime = this.mTime * this.fadeouttime_inv;
				this.mMat.SetFloat("_fadeouttime", this.dtime);
				this.end();
			}
		}
		if (!this.tocontinue)
		{
			if (this.cTime > this.totletime)
			{
				this.cTime = (float)(1 - this.loop) * this.totletime;
			}
			this.dtime = this.cTime * this.totletime_inv;
		}
		else
		{
			this.dtime = this.cTime;
		}
		this.mMat.SetFloat("_mTime", this.dtime);
		if (this.ctrlColor)
		{
			float t = this.mCurveClr.Evaluate(this.dtime);
			this.mclr = Color.Lerp(this.startColor, this.endColor, t);
			if (this.mMat.HasProperty("_clr"))
			{
				this.mMat.SetColor("_clr", this.mclr);
			}
		}
		if (this.ctrlSize)
		{
			float t2 = this.mCurveSize.Evaluate(this.dtime);
			this.msize = Vector3.Lerp(this.startSize, this.endSize, t2);
			if (this.mMat.HasProperty("_sizeX"))
			{
				this.mMat.SetFloat("_sizeX", this.msize.x);
			}
			if (this.mMat.HasProperty("_sizeY"))
			{
				this.mMat.SetFloat("_sizeY", this.msize.y);
			}
		}
	}

	private void Update()
	{
		this.updata(Time.deltaTime);
	}
}
