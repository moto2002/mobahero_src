using MonoMedia;
using System;
using System.IO;
using UnityEngine;

namespace MediaKit.Processor
{
	internal class AudioDecoder
	{
		private const int PCM_BUF_LENGHT = 352800;

		private const int PCM_BUF_OVERLAP = 176400;

		private const int PCM_BUF_SWITCH = 88200;

		private readonly MediaKitProcessor.OGVControl control_;

		private readonly OGGStream oggStream_;

		private float[] Pcm;

		private long PcmSampleOffset;

		private long PcmSampleCount;

		private long PcmRate;

		private int PcmChannels;

		private int curPcmOffset;

		private float[] curPcm = new float[352800];

		private float[] rdyPcm = new float[352800];

		private float[] nxtPcm = new float[352800];

		private Xiph.ogg_sync_state oy;

		private Xiph.ogg_stream_state os;

		private Xiph.ogg_page og;

		private Xiph.ogg_packet op;

		private Xiph.vorbis_info vi;

		private Xiph.vorbis_comment vc;

		private Xiph.vorbis_dsp_state vd;

		private Xiph.vorbis_block vb;

		private Stream instream;

		private long instream_pos;

		private bool vorbis_p;

		private bool need_more_data;

		private bool need_next_pcm;

		private bool stateflag;

		private bool comments_header;

		private bool eos;

		private bool last_buf;

		private bool loaded;

		private uint playSession;

		private int lastReadIdx = -1;

		private int[] lastReadSamples = new int[10];

		private long[] lastReadTime = new long[10];

		private double readspeed = 44.1;

		private float frame_in_sec;

		private float last_frame_at;

		private string error;

		private string info;

		public MediaKitProcessor.OGVControl Control
		{
			get
			{
				return this.control_;
			}
		}

		public AudioDecoder(OGGStream stream, MediaKitProcessor.OGVControl control)
		{
			this.control_ = control;
			this.oggStream_ = stream;
			this.oggStream_.RefCount++;
		}

		~AudioDecoder()
		{
			this.oggStream_.RefCount--;
		}

		private int buffer_data(Stream sin, Xiph.ogg_sync_state oy)
		{
			int num = 0;
			byte[] buffer = null;
			int offset = 0;
			lock (sin)
			{
				if (Xiph.ogg_sync_buffer(oy, 4096, out buffer, out offset))
				{
					sin.Position = this.instream_pos;
					num = sin.Read(buffer, offset, 4096);
					Xiph.ogg_sync_wrote(oy, num);
					this.instream_pos = sin.Position;
				}
			}
			return num;
		}

		private int dump_comments(Xiph.vorbis_info vi, Xiph.vorbis_comment vc)
		{
			Debug.Log("Encoded by " + vc.vendor);
			if (vc.user_comments != null)
			{
				Debug.Log("vorbis comment header:");
				for (int i = 0; i < vc.user_comments.Length; i++)
				{
					Debug.Log("\t" + vc.user_comments[i]);
				}
			}
			Debug.Log(string.Concat(new object[]
			{
				"\nBitstream is ",
				vi.channels,
				" channel, ",
				vi.rate,
				"Hz\n"
			}));
			return 0;
		}

		private bool LoadAudio()
		{
			this.oy = new Xiph.ogg_sync_state();
			this.os = new Xiph.ogg_stream_state();
			this.og = new Xiph.ogg_page();
			this.op = new Xiph.ogg_packet();
			this.vi = new Xiph.vorbis_info();
			this.vc = new Xiph.vorbis_comment();
			this.vd = new Xiph.vorbis_dsp_state();
			this.vb = new Xiph.vorbis_block();
			this.instream_pos = 0L;
			this.eos = false;
			this.last_buf = false;
			this.need_more_data = false;
			this.need_next_pcm = true;
			this.vorbis_p = false;
			this.stateflag = false;
			this.comments_header = false;
			this.instream_pos = 0L;
			this.curPcmOffset = 0;
			this.Pcm = null;
			this.PcmSampleCount = 0L;
			this.PcmSampleOffset = 0L;
			this.readspeed = (double)this.control_.Scale + 44.1;
			this.lastReadIdx = -1;
			Xiph.ogg_sync_init(this.oy);
			Xiph.vorbis_info_init(this.vi);
			Xiph.vorbis_comment_init(this.vc);
			while (!this.stateflag)
			{
				if (this.buffer_data(this.instream, this.oy) == 0)
				{
					break;
				}
				while (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
				{
					Xiph.ogg_stream_state ogg_stream_state = new Xiph.ogg_stream_state();
					if (Xiph.ogg_page_bos(this.og) == 0)
					{
						this.stateflag = true;
						break;
					}
					Xiph.ogg_stream_init(ogg_stream_state, Xiph.ogg_page_serialno(this.og));
					Xiph.ogg_stream_pagein(ogg_stream_state, this.og);
					int num = Xiph.ogg_stream_packetpeek(ogg_stream_state, this.op);
					if (num == 1 && !this.vorbis_p && Xiph.vorbis_synthesis_headerin(this.vi, this.vc, this.op) >= 0)
					{
						this.os = Xiph.ogg_stream_clone(ogg_stream_state);
						this.vorbis_p = true;
						Xiph.ogg_stream_packetout(this.os, null);
					}
					else
					{
						Xiph.ogg_stream_clear(ogg_stream_state);
					}
				}
			}
			while (this.vorbis_p && !this.comments_header)
			{
				int num2;
				while ((num2 = Xiph.ogg_stream_packetpeek(this.os, this.op)) != 0)
				{
					if (num2 >= 0)
					{
						if (Xiph.vorbis_synthesis_headerin(this.vi, this.vc, this.op) < 0)
						{
							this.error = "Error parsing Theora stream vorbis; corrupt stream?\n";
							return false;
						}
						Xiph.ogg_stream_packetout(this.os, null);
						this.comments_header = true;
					}
				}
				if (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
				{
					Xiph.ogg_stream_pagein(this.os, this.og);
				}
				else if (this.buffer_data(this.instream, this.oy) == 0)
				{
					this.error = "End of file while searching for codec headers.\n";
					return false;
				}
			}
			if (this.vorbis_p)
			{
				if (this.buffer_data(this.instream, this.oy) == 0)
				{
					this.error = "End of file before finding all Vorbis headers!\n";
					return false;
				}
				this.PcmRate = this.vi.rate;
				this.PcmChannels = this.vi.channels;
				this.PcmSampleOffset = (long)(-176400 / this.PcmChannels);
				this.PcmSampleCount = (long)(88200 / this.PcmChannels);
				this.curPcmOffset = 88200;
				this.control_.PcmChannels = this.PcmChannels;
				this.control_.PcmRate = this.PcmRate;
				this.control_.SamplesPerMillisecond = (double)this.control_.Scale * 44.1;
				this.dump_comments(this.vi, this.vc);
				if (Xiph.vorbis_synthesis_init(this.vd, this.vi) != 0)
				{
					this.error = "Wrong Vorbis stream!\n";
					return false;
				}
				Xiph.vorbis_block_init(this.vd, this.vb);
			}
			return this.vorbis_p;
		}

		private void DecodePage()
		{
			this.need_more_data = false;
			Xiph.ogg_stream_pagein(this.os, this.og);
			while (true)
			{
				int num = Xiph.ogg_stream_packetout(this.os, this.op);
				if (num == 0)
				{
					break;
				}
				if (num < 0)
				{
					goto Block_2;
				}
				if (Xiph.vorbis_synthesis(this.vb, this.op) == 0)
				{
					Xiph.vorbis_synthesis_blockin(this.vd, this.vb);
				}
				float[][] array;
				int[] array2;
				int num2;
				while ((num2 = Xiph.vorbis_synthesis_pcmout(this.vd, out array, out array2)) > 0)
				{
					int num3 = num2;
					for (int i = 0; i < this.vi.channels; i++)
					{
						int num4 = i;
						float[] array3 = array[i];
						int num5 = array2[i];
						for (int j = 0; j < num3; j++)
						{
							float num6 = array3[num5 + j];
							if (num6 > 1f)
							{
								num6 = 1f;
							}
							if (num6 < -1f)
							{
								num6 = -1f;
							}
							int num7 = this.curPcmOffset + num4;
							if (num7 < 352800)
							{
								this.curPcm[num7] = num6;
							}
							int num8 = num7 - 352800 + 176400;
							if (num8 >= 0 && num8 < 352800)
							{
								this.nxtPcm[num8] = num6;
							}
							num4 += this.vi.channels;
						}
					}
					this.PcmSampleCount += (long)num3;
					this.curPcmOffset += num3 * this.vi.channels;
					Xiph.vorbis_synthesis_read(this.vd, num3);
				}
				if (!this.need_more_data && this.curPcmOffset >= 352800)
				{
					this.need_next_pcm = false;
				}
			}
			this.need_more_data = true;
			Block_2:
			if (Xiph.ogg_page_eos(this.og) != 0)
			{
				this.need_next_pcm = false;
				this.need_more_data = false;
				this.last_buf = true;
				this.eos = true;
			}
		}

		public void Update()
		{
			if (this.control_.Initializing)
			{
				if (this.oggStream_.Ready)
				{
					if (this.instream == null)
					{
						this.instream = this.oggStream_.InMem;
						if (this.instream == null)
						{
							this.instream = this.oggStream_.InFile;
						}
					}
					if (this.loaded && this.info != null)
					{
						Debug.Log(this.info);
					}
					if (this.error != null)
					{
						Debug.LogError(this.error);
						this.error = null;
					}
				}
			}
			else
			{
				if (!this.control_.AudioAvailable)
				{
					return;
				}
				if (this.eos)
				{
					return;
				}
			}
		}

		public void InBackground()
		{
			if (this.instream == null)
			{
				return;
			}
			if (this.playSession != this.control_.PlaySession)
			{
				this.playSession = this.control_.PlaySession;
				this.Pcm = null;
				this.control_.Pcm = this.Pcm;
				this.loaded = false;
			}
			if (!this.loaded)
			{
				this.control_.AudioAvailable = this.LoadAudio();
				this.loaded = true;
				return;
			}
			if (!this.control_.AudioAvailable)
			{
				return;
			}
			if (!this.control_.Ready)
			{
				return;
			}
			if (!this.control_.Playing)
			{
				return;
			}
			while (!this.eos && this.vorbis_p && this.need_more_data && this.need_next_pcm)
			{
				int num = this.buffer_data(this.instream, this.oy);
				if (num <= 0)
				{
					this.need_next_pcm = false;
					this.need_more_data = false;
					this.eos = true;
					break;
				}
				int num2 = Xiph.ogg_sync_pageout(this.oy, this.og);
				if (num2 != 0)
				{
					if (num2 < 0)
					{
						this.error = "Corrupt or missing data in bitstream; continuing...\n";
						break;
					}
					this.need_more_data = false;
				}
			}
			if (!this.eos && this.vorbis_p && !this.need_more_data && this.need_next_pcm)
			{
				this.DecodePage();
			}
			if (!this.need_next_pcm)
			{
				long num3 = (long)(this.control_.ElapsedTime * (double)this.control_.PcmRate) + 44100L;
				if (this.Pcm != null && this.curPcmOffset < 352800 && !this.eos && num3 > this.PcmSampleOffset + (long)(176400 / this.PcmChannels))
				{
					this.control_.SamplesPerMillisecond = (double)this.control_.Scale * 44.1;
					this.need_next_pcm = true;
				}
				if ((this.Pcm == null && this.curPcmOffset >= 352800) || (this.last_buf && num3 > this.PcmSampleOffset + (long)(264600 / this.PcmChannels)) || (!this.need_next_pcm && num3 > this.PcmSampleOffset + (long)(264600 / this.PcmChannels)))
				{
					this.last_buf = false;
					this.curPcmOffset -= 176400;
					float[] array = this.curPcm;
					this.curPcm = this.nxtPcm;
					this.nxtPcm = this.rdyPcm;
					this.rdyPcm = array;
					if (this.curPcmOffset > 176400)
					{
						for (int i = 0; i < this.curPcmOffset - 352800 + 176400; i++)
						{
							int num4 = i + 352800 - 176400;
							if (num4 >= 352800)
							{
								Debug.LogError("(i > srcind)");
							}
							this.nxtPcm[i] = this.curPcm[num4];
						}
					}
					this.PcmSampleOffset += (long)(176400 / this.PcmChannels);
					this.Pcm = this.rdyPcm;
					this.lastReadIdx++;
					long num5 = DateTime.Now.Ticks / 10000L;
					if (this.lastReadIdx > 9)
					{
						for (int j = 1; j < 10; j++)
						{
							this.lastReadSamples[j - 1] = this.lastReadSamples[j];
							this.lastReadTime[j - 1] = this.lastReadTime[j];
						}
						this.lastReadIdx = 9;
					}
					this.lastReadSamples[this.lastReadIdx] = 176400 / this.PcmChannels;
					this.lastReadTime[this.lastReadIdx] = num5;
					long num6 = 0L;
					long num7 = 0L;
					for (int k = 1; k <= this.lastReadIdx; k++)
					{
						num6 += (long)this.lastReadSamples[k - 1];
						num7 += this.lastReadTime[k] - this.lastReadTime[k - 1];
					}
					if (num6 > 0L)
					{
						this.readspeed = (double)num6 / (double)num7;
					}
					this.control_.Pcm = this.Pcm;
					this.control_.PcmSampleOffset = this.PcmSampleOffset;
					this.control_.PcmSampleCount = this.PcmSampleCount;
					this.control_.SamplesPerMillisecond = this.readspeed;
				}
			}
		}
	}
}
