using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTransmitter : MonoBehaviour
{
	private const int SPEEDACC = 20;

	public VideoPlayer VideoPlayer;

	private AudioSource audio_;

	private uint playSession_;

	private long sampleCount_;

	private long lastOffset_;

	private long swapOffset_;

	private bool playing_;

	private double scaleCorrection_ = 1.0;

	private int lastWriteIdx = -1;

	private int[] lastWritingSamples = new int[20];

	private long[] lastWritingTime = new long[20];

	private double writespeed = 44.1;

	private void Start()
	{
		this.audio_ = base.audio;
	}

	private void Update()
	{
		if (this.VideoPlayer != null)
		{
			this.playing_ = this.VideoPlayer.IsPlaying;
			if (this.playing_)
			{
				if (this.playSession_ != this.VideoPlayer.PlaySession)
				{
					this.scaleCorrection_ = 1.1;
					this.playSession_ = this.VideoPlayer.PlaySession;
					this.lastOffset_ = this.VideoPlayer.PcmSampleOffset;
					this.swapOffset_ = 44100L;
					this.sampleCount_ = this.swapOffset_;
				}
			}
			else
			{
				this.scaleCorrection_ = 1.1;
			}
		}
	}

	public void OnAudioFilterRead(float[] data, int channels)
	{
		this.lastWriteIdx++;
		long num = DateTime.Now.Ticks / 10000L;
		if (this.lastWriteIdx > 19)
		{
			for (int i = 1; i < 20; i++)
			{
				this.lastWritingSamples[i - 1] = this.lastWritingSamples[i];
				this.lastWritingTime[i - 1] = this.lastWritingTime[i];
			}
			this.lastWriteIdx = 19;
		}
		this.lastWritingSamples[this.lastWriteIdx] = data.Length / channels;
		this.lastWritingTime[this.lastWriteIdx] = num;
		long num2 = 0L;
		long num3 = 0L;
		for (int j = 1; j <= this.lastWriteIdx; j++)
		{
			num2 += (long)this.lastWritingSamples[j - 1];
			num3 += this.lastWritingTime[j] - this.lastWritingTime[j - 1];
		}
		if (this.lastWriteIdx == 19)
		{
			this.writespeed = (double)num2 / (double)num3;
		}
		else
		{
			this.writespeed = 44.1;
		}
		if (!this.playing_)
		{
			return;
		}
		if (this.VideoPlayer.Pcm == null)
		{
			return;
		}
		float[] pcm = this.VideoPlayer.Pcm;
		int pcmChannels = this.VideoPlayer.PcmChannels;
		int num4 = this.VideoPlayer.Pcm.Length;
		int num5 = this.VideoPlayer.Pcm.Length / pcmChannels;
		long pcmSampleOffset = this.VideoPlayer.PcmSampleOffset;
		long num6 = this.sampleCount_ - pcmSampleOffset;
		long num7 = this.VideoPlayer.PcmSampleCount - pcmSampleOffset;
		if (this.lastOffset_ != this.VideoPlayer.PcmSampleOffset)
		{
			this.lastOffset_ = this.VideoPlayer.PcmSampleOffset;
			this.scaleCorrection_ = 1.0 - (double)(num6 - this.swapOffset_) / 176400.0;
		}
		int num8 = data.Length / channels;
		double num9 = this.VideoPlayer.RealScale * this.scaleCorrection_;
		long num10 = (long)((double)num8 * num9);
		long num11 = num6 + num10;
		this.sampleCount_ += num10;
		double num12 = (double)(num11 - num6) / (double)num8;
		for (int k = 0; k < pcmChannels; k++)
		{
			for (int l = 0; l < num8; l++)
			{
				double num13 = num12 * (double)l;
				long num14 = num6 + (long)num13;
				if (num14 < 0L)
				{
					num14 = 0L;
				}
				long num15 = num14 + 1L;
				double num16 = num13 - (double)(num14 - num6);
				long num17 = (long)(num5 - 1);
				if (num17 >= num7)
				{
					num17 = num7;
				}
				if (num14 > num17)
				{
					num14 = num17;
				}
				if (num15 > num17)
				{
					num15 = num17;
				}
				float num18;
				float num19;
				checked
				{
					num18 = pcm[(int)((IntPtr)(unchecked(num14 * (long)pcmChannels + (long)k)))];
					num19 = pcm[(int)((IntPtr)(unchecked(num15 * (long)pcmChannels + (long)k)))];
				}
				float num20 = (float)((double)num18 * (1.0 - num16) + (double)num19 * num16);
				if (num20 > 1f)
				{
					num20 = 1f;
				}
				if (num20 < -1f)
				{
					num20 = -1f;
				}
				data[l * channels + k] = num20;
			}
		}
	}
}
