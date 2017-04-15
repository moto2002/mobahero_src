using MediaKit.Processor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class MediaKitProcessor : MonoBehaviour
{
	public class OGVControl
	{
		public readonly bool Preload;

		public readonly string Path;

		public readonly Material VideoOutput;

		public uint PlaySession;

		public bool Loop;

		public float Scale = 1f;

		public string Error;

		public string Info;

		public bool Initializing;

		public bool AudioAvailable;

		public bool Ready;

		public double ElapsedTime;

		public DateTime LastFrameAt;

		public double RealScale;

		public double SamplesPerMillisecond;

		public float[] Pcm;

		public long PcmSampleOffset;

		public long PcmSampleCount;

		public long PcmRate;

		public int PcmChannels;

		private bool play_;

		private bool playing_;

		public bool Play
		{
			get
			{
				return this.play_;
			}
			set
			{
				if (this.play_ != value && value)
				{
					this.PlaySession += 1u;
				}
				this.play_ = value;
			}
		}

		public bool Playing
		{
			get
			{
				return this.playing_;
			}
			set
			{
				this.playing_ = value;
			}
		}

		public OGVControl(string path, bool preload, Material videoOutput)
		{
			this.Preload = preload;
			this.Path = path;
			this.VideoOutput = videoOutput;
		}
	}

	private List<MediaKitProcessor.OGVControl> controls_ = new List<MediaKitProcessor.OGVControl>();

	private List<OGGStream> streams_ = new List<OGGStream>();

	private List<VideoDecoder> videoDecoders_ = new List<VideoDecoder>();

	private List<AudioDecoder> audioDecoders_ = new List<AudioDecoder>();

	private Thread videoThread_;

	private bool videoThreadContinue_ = true;

	private Thread audioThread_;

	private bool audioThreadContinue_ = true;

	private Exception exception_;

	public static MediaKitProcessor Instance
	{
		get
		{
			MediaKitProcessor mediaKitProcessor = UnityEngine.Object.FindObjectOfType<MediaKitProcessor>();
			if (mediaKitProcessor == null)
			{
				GameObject gameObject = new GameObject("MediaKitProcessor");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				mediaKitProcessor = gameObject.AddComponent<MediaKitProcessor>();
			}
			return mediaKitProcessor;
		}
	}

	public void Add(MediaKitProcessor.OGVControl ctrl)
	{
		List<MediaKitProcessor.OGVControl> obj = this.controls_;
		lock (obj)
		{
			this.controls_.Add(ctrl);
			this.AddNewVideo();
		}
	}

	public void Remove(MediaKitProcessor.OGVControl ctrl)
	{
		List<MediaKitProcessor.OGVControl> obj = this.controls_;
		lock (obj)
		{
			this.controls_.Remove(ctrl);
			this.RemoveOldVideo();
		}
	}

	private void Start()
	{
		base.StartCoroutine(this.StreamLoader());
		this.videoThread_ = new Thread(new ThreadStart(this.VideoThreadFunc));
		this.videoThread_.Priority = System.Threading.ThreadPriority.Highest;
		this.videoThread_.Start();
		this.audioThread_ = new Thread(new ThreadStart(this.AudioThreadFunc));
		this.audioThread_.Priority = System.Threading.ThreadPriority.Highest;
		this.audioThread_.Start();
	}

	private void OnDestroy()
	{
		int num = 0;
		this.videoThreadContinue_ = false;
		if (this.videoThread_ != null)
		{
			while (this.videoThread_.IsAlive)
			{
				num++;
				if (num > 10)
				{
					UnityEngine.Debug.LogError("Video thread has stack!");
					break;
				}
				Thread.Sleep(100);
			}
		}
		num = 0;
		this.audioThreadContinue_ = false;
		if (this.audioThread_ != null)
		{
			while (this.audioThread_.IsAlive)
			{
				num++;
				if (num > 10)
				{
					UnityEngine.Debug.LogError("Audio thread has stack!");
					break;
				}
				Thread.Sleep(100);
			}
		}
	}

	private void AudioThreadFunc()
	{
		try
		{
			int num = 0;
			while (this.audioThreadContinue_)
			{
				AudioDecoder audioDecoder = null;
				List<MediaKitProcessor.OGVControl> obj = this.controls_;
				lock (obj)
				{
					if (num < this.audioDecoders_.Count)
					{
						audioDecoder = this.audioDecoders_[num++];
					}
					else
					{
						num = 0;
						if (num < this.audioDecoders_.Count)
						{
							audioDecoder = this.audioDecoders_[num++];
						}
					}
				}
				if (audioDecoder != null)
				{
					audioDecoder.InBackground();
				}
				else
				{
					Thread.Sleep(10);
				}
			}
		}
		catch (Exception ex)
		{
			this.exception_ = ex;
		}
	}

	private void VideoThreadFunc()
	{
		try
		{
			int num = 0;
			while (this.videoThreadContinue_)
			{
				VideoDecoder videoDecoder = null;
				List<MediaKitProcessor.OGVControl> obj = this.controls_;
				lock (obj)
				{
					if (num < this.videoDecoders_.Count)
					{
						videoDecoder = this.videoDecoders_[num++];
					}
					else
					{
						num = 0;
						if (num < this.videoDecoders_.Count)
						{
							videoDecoder = this.videoDecoders_[num++];
						}
					}
				}
				if (videoDecoder != null)
				{
					videoDecoder.InBackground();
				}
				else
				{
					Thread.Sleep(10);
				}
			}
		}
		catch (Exception ex)
		{
			this.exception_ = ex;
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.audioDecoders_.Count; i++)
		{
			this.audioDecoders_[i].Update();
		}
		for (int j = 0; j < this.videoDecoders_.Count; j++)
		{
			this.videoDecoders_[j].Update();
		}
		for (int k = 0; k < this.streams_.Count; k++)
		{
			OGGStream oGGStream = this.streams_[k];
			if (oGGStream.RefCount == 0)
			{
				this.streams_.Remove(oGGStream);
				break;
			}
		}
		if (this.exception_ != null)
		{
			UnityEngine.Debug.LogException(this.exception_);
			this.exception_ = null;
		}
	}

	[DebuggerHidden]
	private IEnumerator StreamLoader()
	{
		MediaKitProcessor.<StreamLoader>c__Iterator16 <StreamLoader>c__Iterator = new MediaKitProcessor.<StreamLoader>c__Iterator16();
		<StreamLoader>c__Iterator.<>f__this = this;
		return <StreamLoader>c__Iterator;
	}

	private void AddNewVideo()
	{
		for (int i = 0; i < this.controls_.Count; i++)
		{
			MediaKitProcessor.OGVControl oGVControl = this.controls_[i];
			VideoDecoder videoDecoder = null;
			for (int j = 0; j < this.videoDecoders_.Count; j++)
			{
				if (this.videoDecoders_[j].Control == oGVControl)
				{
					videoDecoder = this.videoDecoders_[j];
					break;
				}
			}
			if (videoDecoder == null)
			{
				OGGStream oGGStream = null;
				for (int k = 0; k < this.streams_.Count; k++)
				{
					if (this.streams_[k].Path == oGVControl.Path && this.streams_[k].Preload == oGVControl.Preload)
					{
						oGGStream = this.streams_[k];
						break;
					}
				}
				if (oGGStream == null)
				{
					this.streams_.Add(oGGStream = new OGGStream(oGVControl.Path, oGVControl.Preload));
				}
				this.videoDecoders_.Add(new VideoDecoder(oGGStream, oGVControl));
				this.audioDecoders_.Add(new AudioDecoder(oGGStream, oGVControl));
			}
		}
	}

	private void RemoveOldVideo()
	{
		for (int i = 0; i < this.videoDecoders_.Count; i++)
		{
			VideoDecoder videoDecoder = this.videoDecoders_[i];
			for (int j = 0; j < this.controls_.Count; j++)
			{
				MediaKitProcessor.OGVControl oGVControl = this.controls_[j];
				if (videoDecoder.Control == oGVControl)
				{
					videoDecoder = null;
					break;
				}
			}
			if (videoDecoder != null)
			{
				for (int k = 0; k < this.audioDecoders_.Count; k++)
				{
					AudioDecoder audioDecoder = this.audioDecoders_[k];
					if (audioDecoder.Control == videoDecoder.Control)
					{
						this.audioDecoders_.Remove(audioDecoder);
					}
				}
				this.videoDecoders_.Remove(videoDecoder);
				break;
			}
		}
	}
}
