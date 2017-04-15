using System;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
	public string Video;

	public Material VideoOutput;

	public float[] Pcm;

	public long PcmSampleOffset;

	public long PcmSampleCount;

	public long PcmRate;

	public int PcmChannels;

	public double ElapsedTime;

	public DateTime LastFrameAt;

	public double RealScale;

	public double SamplesPerMillisecond;

	public bool Preload = true;

	public bool Play;

	public uint PlaySession;

	public bool Loop = true;

	public bool Transparent;

	public bool Additive;

	public float Scale = 1f;

	public List<GameObject> OnReady;

	public List<GameObject> OnStart;

	public List<GameObject> OnStop;

	private string video_ = string.Empty;

	private bool runOnce_ = true;

	private bool isPlaying_;

	private MediaKitProcessor.OGVControl control_;

	private MediaKitProcessor processor_;

	public bool IsPlaying
	{
		get
		{
			return this.isPlaying_;
		}
		private set
		{
			if (value && !this.isPlaying_)
			{
				this.isPlaying_ = value;
				for (int i = 0; i < this.OnStart.Count; i++)
				{
					if (this.OnStart[i] != null)
					{
						this.OnStart[i].SendMessage("OnVideoStart");
					}
				}
			}
			else if (!value && this.isPlaying_)
			{
				this.isPlaying_ = value;
				for (int j = 0; j < this.OnStop.Count; j++)
				{
					if (this.OnStop[j] != null)
					{
						this.OnStop[j].SendMessage("OnVideoStop");
					}
				}
			}
			else
			{
				this.isPlaying_ = value;
			}
		}
	}

	private MediaKitProcessor Processor
	{
		get
		{
			if (this.processor_ == null)
			{
				this.processor_ = MediaKitProcessor.Instance;
			}
			return this.processor_;
		}
	}

	private void Start()
	{
		Shader shader = Shader.Find((!this.Transparent) ? "MediaKit/VideoOutput" : ((!this.Additive) ? "MediaKit/TransparentVideoOutput" : "MediaKit/AdditiveTransparentVideoOutput"));
		this.VideoOutput = new Material(shader);
		this.processor_ = MediaKitProcessor.Instance;
		this.RefreshVideoControl();
	}

	private void OnDestroy()
	{
		UnityEngine.Object.Destroy(this.VideoOutput);
		if (this.control_ != null && this.processor_ != null)
		{
			this.processor_.Remove(this.control_);
		}
		this.processor_ = null;
	}

	private void RefreshVideoControl()
	{
		if (string.IsNullOrEmpty(this.Video))
		{
			this.video_ = string.Empty;
			if (this.control_ != null)
			{
				this.Processor.Remove(this.control_);
			}
			this.control_ = null;
		}
		else if (!this.video_.Equals(this.Video))
		{
			if (this.control_ != null)
			{
				this.Processor.Remove(this.control_);
			}
			this.video_ = this.Video;
			this.control_ = new MediaKitProcessor.OGVControl(this.Video, this.Preload, this.VideoOutput);
			this.control_.Scale = this.Scale;
			this.Processor.Add(this.control_);
		}
	}

	private void Update()
	{
		this.RefreshVideoControl();
		if (this.control_ != null)
		{
			this.control_.Scale = this.Scale;
			if (this.control_.Ready)
			{
				if (this.runOnce_)
				{
					this.runOnce_ = false;
					for (int i = 0; i < this.OnReady.Count; i++)
					{
						if (this.OnReady[i] != null)
						{
							this.OnReady[i].SendMessage("OnVideoReady");
						}
					}
				}
				this.control_.Play = this.Play;
				this.control_.Loop = this.Loop;
				this.IsPlaying = this.control_.Playing;
				this.PlaySession = this.control_.PlaySession;
				this.ElapsedTime = this.control_.ElapsedTime;
				this.LastFrameAt = this.control_.LastFrameAt;
				this.RealScale = this.control_.RealScale;
				this.SamplesPerMillisecond = this.control_.SamplesPerMillisecond;
				this.Pcm = this.control_.Pcm;
				this.PcmRate = this.control_.PcmRate;
				this.PcmChannels = this.control_.PcmChannels;
				this.PcmSampleOffset = this.control_.PcmSampleOffset;
				this.PcmSampleCount = this.control_.PcmSampleCount;
			}
		}
	}
}
