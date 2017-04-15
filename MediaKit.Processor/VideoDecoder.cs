using MonoMedia;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MediaKit.Processor
{
	internal class VideoDecoder
	{
		private const int ELAPSED_TIME_SAMPLES_MAX = 60;

		private readonly MediaKitProcessor.OGVControl control_;

		private readonly OGGStream oggStream_;

		private Xiph.ogg_packet op;

		private Xiph.ogg_page og;

		private Xiph.ogg_stream_state to;

		private Xiph.ogg_sync_state oy;

		private Xiph.th_info ti;

		private Xiph.th_comment tc;

		private Xiph.th_setup_info ts;

		private Xiph.th_dec_ctx td;

		private Stream instream;

		private long instream_pos;

		private int stateflag;

		private int theora_p;

		private int theora_processing_headers;

		private int videobuf_ready;

		private long videobuf_granulepos = -1L;

		private int rnd_frame = -1;

		private int rdy_frame = -1;

		private int cur_frame = -1;

		private Texture2D y;

		private int width;

		private int height;

		private Color32[] colors_y;

		private Color32[] colors_y_1;

		private Color32[] colors_y_2;

		private Color32[] colors_y_rnd;

		private bool loaded;

		private bool skip = true;

		private float fps;

		private float frame_in_sec;

		private float last_frame_at;

		private double videobuf_time;

		private float video_start_time;

		private string error;

		private string info;

		private Queue<double> elapsedTimeSamples = new Queue<double>(60);

		private Queue<long> elapsedTimeTimestamps = new Queue<long>(60);

		public MediaKitProcessor.OGVControl Control
		{
			get
			{
				return this.control_;
			}
		}

		public VideoDecoder(OGGStream stream, MediaKitProcessor.OGVControl control)
		{
			this.control_ = control;
			this.oggStream_ = stream;
			this.oggStream_.RefCount++;
			this.control_.Initializing = true;
		}

		~VideoDecoder()
		{
			this.oggStream_.RefCount--;
		}

		private bool LoadVideo()
		{
			this.op = new Xiph.ogg_packet();
			this.og = new Xiph.ogg_page();
			this.to = new Xiph.ogg_stream_state();
			this.oy = new Xiph.ogg_sync_state();
			this.ti = default(Xiph.th_info);
			this.tc = new Xiph.th_comment();
			this.ts = null;
			this.fps = 0f;
			this.frame_in_sec = 0f;
			this.instream_pos = 0L;
			this.stateflag = 0;
			this.theora_p = 0;
			this.theora_processing_headers = 0;
			this.videobuf_time = 0.0;
			this.videobuf_ready = 0;
			this.videobuf_granulepos = -1L;
			this.rnd_frame = -1;
			this.rdy_frame = -1;
			this.cur_frame = -1;
			this.elapsedTimeSamples.Clear();
			this.elapsedTimeTimestamps.Clear();
			Xiph.ogg_sync_init(this.oy);
			Xiph.th_comment_init(this.tc);
			Xiph.th_info_init(ref this.ti);
			while (this.stateflag == 0)
			{
				if (this.buffer_data(this.instream, this.oy) == 0)
				{
					break;
				}
				while (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
				{
					Xiph.ogg_stream_state os = new Xiph.ogg_stream_state();
					if (Xiph.ogg_page_bos(this.og) == 0)
					{
						this.queue_page(this.og);
						this.stateflag = 1;
						break;
					}
					Xiph.ogg_stream_init(os, Xiph.ogg_page_serialno(this.og));
					Xiph.ogg_stream_pagein(os, this.og);
					int num = Xiph.ogg_stream_packetpeek(os, this.op);
					if (num == 1 && this.theora_p == 0 && (this.theora_processing_headers = Xiph.th_decode_headerin(ref this.ti, this.tc, ref this.ts, this.op)) >= 0)
					{
						this.to = Xiph.ogg_stream_clone(os);
						this.theora_p = 1;
						if (this.theora_processing_headers != 0)
						{
							Xiph.ogg_stream_packetout(this.to, null);
						}
					}
					else
					{
						Xiph.ogg_stream_clear(os);
					}
				}
			}
			while (this.theora_p != 0 && this.theora_processing_headers != 0)
			{
				int num2;
				while (this.theora_processing_headers != 0 && (num2 = Xiph.ogg_stream_packetpeek(this.to, this.op)) != 0)
				{
					if (num2 >= 0)
					{
						this.theora_processing_headers = Xiph.th_decode_headerin(ref this.ti, this.tc, ref this.ts, this.op);
						if (this.theora_processing_headers < 0)
						{
							this.error = "Error parsing Theora stream headers; corrupt stream?\n";
							return false;
						}
						if (this.theora_processing_headers > 0)
						{
							Xiph.ogg_stream_packetout(this.to, null);
						}
						this.theora_p++;
					}
				}
				if (this.theora_p == 0 || this.theora_processing_headers == 0)
				{
					break;
				}
				if (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
				{
					this.queue_page(this.og);
				}
				else if (this.buffer_data(this.instream, this.oy) == 0)
				{
					this.error = "End of file while searching for codec headers.\n";
					return false;
				}
			}
			if (this.theora_p != 0)
			{
				this.dump_comments(this.tc);
				this.td = Xiph.th_decode_alloc(this.ti, this.ts);
				this.info = string.Format("Ogg logical stream {0} is Theora {1}x{2} {3} fps video\nEncoded frame content is {4}x{5} with {6}x{7} offset\n", new object[]
				{
					this.to.serialno,
					this.ti.frame_width,
					this.ti.frame_height,
					this.ti.fps_numerator / this.ti.fps_denominator,
					this.ti.pic_width,
					this.ti.pic_height,
					this.ti.pic_x,
					this.ti.pic_y
				});
				this.fps = this.ti.fps_numerator / this.ti.fps_denominator;
				this.frame_in_sec = 1f / this.fps;
			}
			else
			{
				Xiph.th_info_clear(ref this.ti);
				Xiph.th_comment_clear(this.tc);
			}
			Xiph.th_setup_free(this.ts);
			if (this.theora_p != 0)
			{
				this.open_video();
			}
			this.stateflag = 0;
			while (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
			{
				this.queue_page(this.og);
			}
			this.colors_y_1 = new Color32[this.width * this.height];
			this.colors_y_2 = new Color32[this.width * this.height];
			this.colors_y = this.colors_y_1;
			this.colors_y_rnd = this.colors_y_2;
			while (this.theora_p != 0 && this.videobuf_ready == 0)
			{
				if (Xiph.ogg_stream_packetout(this.to, this.op) <= 0)
				{
					break;
				}
				if (Xiph.th_decode_packetin(this.td, this.op, ref this.videobuf_granulepos) >= 0)
				{
					this.videobuf_time = Xiph.th_granule_time(this.td.state, this.videobuf_granulepos);
					this.videobuf_ready = 1;
				}
			}
			this.loaded = true;
			return true;
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

		private int queue_page(Xiph.ogg_page page)
		{
			if (this.theora_p != 0)
			{
				Xiph.ogg_stream_pagein(this.to, page);
			}
			return 0;
		}

		private int dump_comments(Xiph.th_comment tc)
		{
			if (tc.user_comments != null)
			{
				for (int i = 0; i < tc.user_comments.Length; i++)
				{
				}
			}
			return 0;
		}

		private void open_video()
		{
			this.width = (int)this.ti.frame_width;
			this.height = (int)this.ti.frame_height;
			Xiph.th_stripe_callback stripe_callback;
			stripe_callback.ctx = null;
			stripe_callback.stripe_decoded = new Xiph.th_stripe_decoded_func(this.stripe_decoded);
			Xiph.th_decode_ctl_opts th_decode_ctl_opts = new Xiph.th_decode_ctl_opts();
			th_decode_ctl_opts.stripe_callback = stripe_callback;
			Xiph.th_decode_ctl(this.td, 7, th_decode_ctl_opts);
		}

		private void stripe_decoded(object ctx, ref Xiph.th_ycbcr_buffer src, int fragy0, int fragy_end)
		{
			this.skip = false;
			byte[] data = src.y.data.data;
			byte[] data2 = src.cr.data.data;
			byte[] data3 = src.cb.data.data;
			int offset = src.y.data.offset;
			int offset2 = src.cb.data.offset;
			int offset3 = src.cr.data.offset;
			int stride = src.y.stride;
			int stride2 = src.cb.stride;
			int stride3 = src.cr.stride;
			int num = src.y.width;
			int num2 = src.cb.width;
			int num3 = src.cr.width;
			for (int i = 0; i < 3; i++)
			{
				int num4 = (i == 0 || (this.ti.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) != Xiph.th_pixel_fmt.TH_PF_420) ? 0 : 1;
				int num5 = fragy_end << 3 - num4;
				for (int j = fragy0 << 3 - num4; j < num5; j++)
				{
					switch (i)
					{
					case 0:
						for (int k = 0; k < num; k++)
						{
							this.colors_y_rnd[j * this.width + k].g = data[offset + j * stride + k];
						}
						break;
					case 1:
						for (int l = 0; l < num2; l++)
						{
							byte b = data3[offset2 + j * stride2 + l];
							if (stride2 == stride)
							{
								this.colors_y_rnd[j * this.width + l].b = b;
							}
							else
							{
								this.colors_y_rnd[j * 2 * num + l * 2].b = b;
								this.colors_y_rnd[j * 2 * num + l * 2 + 1].b = b;
								this.colors_y_rnd[(j * 2 + 1) * num + l * 2].b = b;
								this.colors_y_rnd[(j * 2 + 1) * num + l * 2 + 1].b = b;
							}
						}
						break;
					case 2:
						for (int m = 0; m < num3; m++)
						{
							byte r = data2[offset3 + j * stride3 + m];
							if (stride2 == stride)
							{
								this.colors_y_rnd[j * this.width + m].r = r;
							}
							else
							{
								this.colors_y_rnd[j * 2 * num + m * 2].r = r;
								this.colors_y_rnd[j * 2 * num + m * 2 + 1].r = r;
								this.colors_y_rnd[(j * 2 + 1) * num + m * 2].r = r;
								this.colors_y_rnd[(j * 2 + 1) * num + m * 2 + 1].r = r;
							}
						}
						break;
					}
				}
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
					if (this.loaded)
					{
						this.y = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
						this.y.name = "MediaKitVideoDecoder_" + Time.time.ToString();
						this.cur_frame = (this.rdy_frame = (this.rnd_frame = 0));
						this.last_frame_at = Time.timeSinceLevelLoad;
						this.y.SetPixels32(this.colors_y);
						this.y.Apply();
						this.video_start_time = Time.timeSinceLevelLoad;
						this.control_.VideoOutput.SetTexture("_YTex", this.y);
						this.control_.Initializing = false;
						this.control_.RealScale = (double)this.control_.Scale;
						this.control_.ElapsedTime = 0.0;
						this.control_.Ready = true;
						if (this.control_.Play)
						{
							this.control_.Playing = true;
						}
						if (this.info != null)
						{
						}
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
				if (this.rdy_frame == -1)
				{
					return;
				}
				float num = this.frame_in_sec;
				if (this.control_.Scale <= 0f)
				{
					num = 999999f;
				}
				else
				{
					num = this.frame_in_sec / this.control_.Scale;
				}
				if (this.cur_frame != this.rdy_frame && Time.timeSinceLevelLoad >= this.last_frame_at + num)
				{
					this.control_.ElapsedTime = this.videobuf_time;
					this.control_.LastFrameAt = DateTime.UtcNow;
					long num2 = DateTime.Now.Ticks / 10000L;
					if (this.elapsedTimeSamples.Count == 60)
					{
						double num3 = this.control_.ElapsedTime - this.elapsedTimeSamples.Dequeue();
						double num4 = (double)(num2 - this.elapsedTimeTimestamps.Dequeue()) / 1000.0;
						this.control_.RealScale = num3 / num4;
					}
					else
					{
						this.control_.RealScale = (double)this.control_.Scale;
					}
					this.elapsedTimeSamples.Enqueue(this.control_.ElapsedTime);
					this.elapsedTimeTimestamps.Enqueue(num2);
					this.y.SetPixels32(this.colors_y);
					this.cur_frame = this.rdy_frame;
					this.last_frame_at += num;
					if (this.last_frame_at < Time.timeSinceLevelLoad - num)
					{
						this.last_frame_at = Time.timeSinceLevelLoad - num;
					}
					this.y.Apply();
					if (this.control_.Play)
					{
						this.control_.Playing = true;
					}
				}
			}
		}

		public void InBackground()
		{
			if (!this.loaded && this.instream != null)
			{
				this.LoadVideo();
			}
			if (!this.control_.Ready)
			{
				return;
			}
			if (!this.control_.Play)
			{
				return;
			}
			if (this.rnd_frame != this.rdy_frame)
			{
				if (this.cur_frame == this.rdy_frame)
				{
					if (!this.skip)
					{
						Color32[] array = this.colors_y_rnd;
						this.colors_y_rnd = this.colors_y;
						this.colors_y = array;
					}
					this.rdy_frame = this.rnd_frame;
					this.skip = true;
				}
				return;
			}
			while (this.theora_p != 0 && this.videobuf_ready == 0)
			{
				if (Xiph.ogg_stream_packetout(this.to, this.op) <= 0)
				{
					break;
				}
				if (Xiph.th_decode_packetin(this.td, this.op, ref this.videobuf_granulepos) >= 0)
				{
					this.videobuf_time = Xiph.th_granule_time(this.td.state, this.videobuf_granulepos);
					this.videobuf_ready = 1;
				}
			}
			if (this.videobuf_ready == 0 && this.instream.Length == this.instream_pos)
			{
				if (this.control_.Loop)
				{
					this.control_.PlaySession += 1u;
					this.LoadVideo();
				}
				else
				{
					this.control_.Playing = false;
				}
				return;
			}
			if (this.videobuf_ready == 0)
			{
				this.buffer_data(this.instream, this.oy);
				while (Xiph.ogg_sync_pageout(this.oy, this.og) > 0)
				{
					this.queue_page(this.og);
				}
			}
			else
			{
				this.rnd_frame++;
				if (this.cur_frame == this.rdy_frame)
				{
					if (!this.skip)
					{
						Color32[] array2 = this.colors_y_rnd;
						this.colors_y_rnd = this.colors_y;
						this.colors_y = array2;
					}
					this.rdy_frame = this.rnd_frame;
					this.skip = true;
				}
			}
			this.videobuf_ready = 0;
		}
	}
}
