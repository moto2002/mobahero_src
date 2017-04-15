using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace MonoMedia
{
	public static class Xiph
	{
		public struct ogg_ptr
		{
			public byte[] data;

			public int offset;

			public byte this[int i]
			{
				get
				{
					return this.data[this.offset + i];
				}
				set
				{
					this.data[this.offset + i] = value;
				}
			}

			public ogg_ptr(byte[] data, int offset)
			{
				this.data = data;
				this.offset = offset;
			}

			public ogg_ptr(Xiph.ogg_ptr src)
			{
				this.data = src.data;
				this.offset = src.offset;
			}

			public void set(byte[] data, int offset)
			{
				this.data = data;
				this.offset = offset;
			}

			public static Xiph.ogg_ptr operator +(Xiph.ogg_ptr ptr, int shift)
			{
				return new Xiph.ogg_ptr(ptr.data, ptr.offset + shift);
			}

			public static Xiph.ogg_ptr operator -(Xiph.ogg_ptr ptr, int shift)
			{
				return new Xiph.ogg_ptr(ptr.data, ptr.offset - shift);
			}

			public static Xiph.ogg_ptr operator +(Xiph.ogg_ptr ptr, long shift)
			{
				return new Xiph.ogg_ptr(ptr.data, ptr.offset + (int)shift);
			}

			public static Xiph.ogg_ptr operator -(Xiph.ogg_ptr ptr, long shift)
			{
				return new Xiph.ogg_ptr(ptr.data, ptr.offset - (int)shift);
			}

			public static int operator -(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.offset - r.offset;
			}

			public static bool operator <(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.offset < r.offset;
			}

			public static bool operator >(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.offset > r.offset;
			}

			public static bool operator >=(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.offset >= r.offset;
			}

			public static bool operator <=(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.offset <= r.offset;
			}

			public override int GetHashCode()
			{
				return this.offset;
			}

			public override bool Equals(object obj)
			{
				if (obj.GetType() != typeof(Xiph.ogg_ptr))
				{
					return false;
				}
				Xiph.ogg_ptr ogg_ptr = (Xiph.ogg_ptr)obj;
				return this.data == ogg_ptr.data && this.offset == ogg_ptr.offset;
			}

			public static bool operator ==(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.data == r.data && l.offset == r.offset;
			}

			public static bool operator !=(Xiph.ogg_ptr l, Xiph.ogg_ptr r)
			{
				return l.data != r.data || l.offset != r.offset;
			}

			public static Xiph.ogg_ptr operator ++(Xiph.ogg_ptr ptr)
			{
				ptr.offset++;
				return ptr;
			}

			public uint read(int i)
			{
				return (uint)((int)this.data[this.offset + i] | (int)this.data[this.offset + i + 1] << 8 | (int)this.data[this.offset + i + 2] << 16 | (int)this.data[this.offset + i + 3] << 24);
			}

			public void write(int i, uint v)
			{
				this.data[this.offset + i] = (byte)(v & 255u);
				this.data[this.offset + i + 1] = (byte)((v & 65280u) >> 8);
				this.data[this.offset + i + 2] = (byte)((v & 16711680u) >> 16);
				this.data[this.offset + i + 3] = (byte)((v & 4278190080u) >> 24);
			}

			public Xiph.ogg_ptr? memchr(int i, byte chr, int length)
			{
				i += this.offset;
				while (length > 0)
				{
					if (this.data[i] == chr)
					{
						return new Xiph.ogg_ptr?(new Xiph.ogg_ptr(this.data, i));
					}
					i++;
					length--;
				}
				return null;
			}
		}

		public class oggpack_buffer
		{
			public long endbyte;

			public int endbit;

			public Xiph.ogg_ptr buffer;

			public Xiph.ogg_ptr ptr;

			public long storage;
		}

		public class ogg_page
		{
			public Xiph.ogg_ptr header;

			public int header_len;

			public Xiph.ogg_ptr body;

			public int body_len;
		}

		public class ogg_stream_state
		{
			public byte[] body_data;

			public int body_storage;

			public int body_fill;

			public int body_returned;

			public int[] lacing_vals;

			public long[] granule_vals;

			public int lacing_storage;

			public int lacing_fill;

			public int lacing_packet;

			public int lacing_returned;

			public byte[] header = new byte[282];

			public int header_fill;

			public int e_o_s;

			public int b_o_s;

			public int serialno;

			public int pageno;

			public long packetno;

			public long granulepos;
		}

		public class ogg_packet
		{
			public Xiph.ogg_ptr packet;

			public int bytes;

			public int b_o_s;

			public int e_o_s;

			public long granulepos;

			public long packetno;
		}

		public class ogg_sync_state
		{
			public byte[] data;

			public int storage;

			public int fill;

			public int returned;

			public int unsynced;

			public int headerbytes;

			public int bodybytes;
		}

		public static class Encoding
		{
			public static class ASCII
			{
				public static string GetString(byte[] bytes)
				{
					StringBuilder stringBuilder = new StringBuilder(bytes.Length);
					for (int i = 0; i < bytes.Length; i++)
					{
						stringBuilder.Append((char)bytes[i]);
					}
					return stringBuilder.ToString();
				}

				public static string GetString(byte[] bytes, int start, int count)
				{
					StringBuilder stringBuilder = new StringBuilder(count);
					for (int i = start; i < count; i++)
					{
						stringBuilder.Append((char)bytes[i]);
					}
					return stringBuilder.ToString();
				}
			}
		}

		private static class MemCache<T>
		{
			private static Dictionary<int, Dictionary<int, Stack<T[]>>> heaps_ = new Dictionary<int, Dictionary<int, Stack<T[]>>>();

			public static T[] Get(int capacity)
			{
				int managedThreadId = Thread.CurrentThread.ManagedThreadId;
				Dictionary<int, Stack<T[]>> dictionary;
				if (Xiph.MemCache<T>.heaps_.ContainsKey(managedThreadId))
				{
					dictionary = Xiph.MemCache<T>.heaps_[managedThreadId];
				}
				else
				{
					dictionary = new Dictionary<int, Stack<T[]>>();
					Xiph.MemCache<T>.heaps_[managedThreadId] = dictionary;
				}
				Stack<T[]> stack;
				if (dictionary.ContainsKey(capacity))
				{
					stack = dictionary[capacity];
				}
				else
				{
					stack = new Stack<T[]>();
					dictionary[capacity] = stack;
				}
				T[] result;
				if (stack.Count > 0)
				{
					result = stack.Pop();
				}
				else
				{
					result = new T[capacity];
				}
				return result;
			}

			public static void Release(T[] arr)
			{
				int key = arr.Length;
				int managedThreadId = Thread.CurrentThread.ManagedThreadId;
				Dictionary<int, Stack<T[]>> dictionary = Xiph.MemCache<T>.heaps_[managedThreadId];
				Stack<T[]> stack = dictionary[key];
				stack.Push(arr);
			}
		}

		public struct Ptr<T>
		{
			public T[] data;

			public int offset;

			public T this[int i]
			{
				get
				{
					return this.data[this.offset + i];
				}
				set
				{
					this.data[this.offset + i] = value;
				}
			}

			public T this[long i]
			{
				get
				{
					return this.data[(int)(checked((IntPtr)(unchecked((long)this.offset + i))))];
				}
				set
				{
					this.data[(int)(checked((IntPtr)(unchecked((long)this.offset + i))))] = value;
				}
			}

			public Ptr(T[] data, int offset)
			{
				this.data = data;
				this.offset = offset;
			}

			public Ptr(T[] data, long offset)
			{
				this.data = data;
				this.offset = (int)offset;
			}

			public Ptr(Xiph.Ptr<T> src)
			{
				this.data = src.data;
				this.offset = src.offset;
			}

			public void set(T[] data, int offset)
			{
				this.data = data;
				this.offset = offset;
			}

			public static Xiph.Ptr<T>operator ++(Xiph.Ptr<T> ptr)
			{
				ptr.offset++;
				return ptr;
			}

			public override int GetHashCode()
			{
				return this.offset;
			}

			public override bool Equals(object obj)
			{
				if (obj.GetType() != typeof(Xiph.Ptr<T>))
				{
					return false;
				}
				Xiph.Ptr<T> ptr = (Xiph.Ptr<T>)obj;
				return this.data == ptr.data && this.offset == ptr.offset;
			}

			public static bool operator ==(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.data == r.data && l.offset == r.offset;
			}

			public static bool operator !=(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.data != r.data || l.offset != r.offset;
			}

			public static Xiph.Ptr<T>operator +(Xiph.Ptr<T> ptr, int shift)
			{
				return new Xiph.Ptr<T>(ptr.data, ptr.offset + shift);
			}

			public static Xiph.Ptr<T>operator -(Xiph.Ptr<T> ptr, int shift)
			{
				return new Xiph.Ptr<T>(ptr.data, ptr.offset - shift);
			}

			public static Xiph.Ptr<T>operator +(Xiph.Ptr<T> ptr, long shift)
			{
				return new Xiph.Ptr<T>(ptr.data, ptr.offset + (int)shift);
			}

			public static Xiph.Ptr<T>operator -(Xiph.Ptr<T> ptr, long shift)
			{
				return new Xiph.Ptr<T>(ptr.data, ptr.offset - (int)shift);
			}

			public static int operator -(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.offset - r.offset;
			}

			public static bool operator <(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.offset < r.offset;
			}

			public static bool operator >(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.offset > r.offset;
			}

			public static bool operator >=(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.offset >= r.offset;
			}

			public static bool operator <=(Xiph.Ptr<T> l, Xiph.Ptr<T> r)
			{
				return l.offset <= r.offset;
			}
		}

		public class oc_pack_buf
		{
			public Xiph.ogg_ptr stop;

			public Xiph.ogg_ptr ptr;

			public uint window;

			public int bits;

			public int eof;
		}

		public enum th_colorspace
		{
			TH_CS_UNSPECIFIED,
			TH_CS_ITU_REC_470M,
			TH_CS_ITU_REC_470BG,
			TH_CS_NSPACES
		}

		public enum th_pixel_fmt
		{
			TH_PF_420,
			TH_PF_RSVD,
			TH_PF_422,
			TH_PF_444,
			TH_PF_NFORMATS
		}

		public struct th_img_plane
		{
			public int width;

			public int height;

			public int stride;

			public Xiph.ogg_ptr data;
		}

		public struct th_ycbcr_buffer
		{
			public Xiph.th_img_plane y;

			public Xiph.th_img_plane cb;

			public Xiph.th_img_plane cr;

			public Xiph.th_img_plane this[int i]
			{
				get
				{
					if (i == 0)
					{
						return this.y;
					}
					if (i != 1)
					{
						return this.cr;
					}
					return this.cb;
				}
			}
		}

		public struct th_info
		{
			public byte version_major;

			public byte version_minor;

			public byte version_subminor;

			public uint frame_width;

			public uint frame_height;

			public uint pic_width;

			public uint pic_height;

			public uint pic_x;

			public uint pic_y;

			public uint fps_numerator;

			public uint fps_denominator;

			public uint aspect_numerator;

			public uint aspect_denominator;

			public Xiph.th_colorspace colorspace;

			public Xiph.th_pixel_fmt pixel_fmt;

			public int target_bitrate;

			public int quality;

			public int keyframe_granule_shift;
		}

		public class th_comment
		{
			public string[] user_comments;

			public string vendor;
		}

		public class th_quant_base
		{
			public byte[] data;

			public byte this[int i]
			{
				get
				{
					return this.data[i];
				}
				set
				{
					this.data[i] = value;
				}
			}

			public th_quant_base()
			{
				this.data = new byte[64];
			}

			public th_quant_base(Xiph.th_quant_base src)
			{
				this.data = new byte[64];
				Xiph.CopyArrays(src.data, this.data, 64);
			}
		}

		public struct th_quant_ranges
		{
			public int nranges;

			public int[] sizes;

			public Xiph.th_quant_base[] base_matrices;
		}

		public class th_quant_info
		{
			public ushort[] dc_scale = new ushort[64];

			public ushort[] ac_scale = new ushort[64];

			public byte[] loop_filter_limits = new byte[64];

			public Xiph.th_quant_ranges[,] qi_ranges = new Xiph.th_quant_ranges[2, 3];
		}

		public class th_huff_code
		{
			public uint pattern;

			public int nbits;
		}

		public class th_setup_info
		{
			public short[][] huff_tables = new short[80][];

			public Xiph.th_quant_info qinfo = new Xiph.th_quant_info();
		}

		public class oc_dec_pipeline_state
		{
			public short[] dct_coeffs = new short[128];

			public sbyte[] bounding_values = new sbyte[256];

			public long[][] ti = new long[3][];

			public long[][] ebi = new long[3][];

			public long[][] eob_runs = new long[3][];

			public Xiph.Ptr<long>[] coded_fragis = new Xiph.Ptr<long>[3];

			public Xiph.Ptr<long>[] uncoded_fragis = new Xiph.Ptr<long>[3];

			public long[] ncoded_fragis = new long[3];

			public long[] nuncoded_fragis = new long[3];

			public ushort[][][][] dequant = new ushort[3][][][];

			public int[] fragy0 = new int[3];

			public int[] fragy_end = new int[3];

			public int[][] pred_last = new int[3][];

			public int mcu_nvfrags;

			public int loop_filter;

			public int pp_level;

			public oc_dec_pipeline_state()
			{
				for (int i = 0; i < 3; i++)
				{
					this.ti[i] = new long[64];
				}
				for (int i = 0; i < 3; i++)
				{
					this.ebi[i] = new long[64];
				}
				for (int i = 0; i < 3; i++)
				{
					this.eob_runs[i] = new long[64];
				}
				for (int i = 0; i < 3; i++)
				{
					this.pred_last[i] = new int[3];
				}
				for (int i = 0; i < 3; i++)
				{
					this.dequant[i] = new ushort[3][][];
					for (int j = 0; j < 3; j++)
					{
						this.dequant[i][j] = new ushort[2][];
					}
				}
			}
		}

		public class th_dec_ctx
		{
			public Xiph.oc_theora_state state = new Xiph.oc_theora_state();

			public int packet_state;

			public Xiph.oc_pack_buf opb = new Xiph.oc_pack_buf();

			public short[][] huff_tables = new short[80][];

			public long[][] ti0 = new long[3][];

			public long[][] eob_runs = new long[3][];

			public byte[] dct_tokens;

			public byte[] extra_bits;

			public int dct_tokens_count;

			public int pp_level;

			public int[] pp_dc_scale = new int[64];

			public int[] pp_sharp_mod = new int[64];

			public byte[] dc_qis;

			public int[] variances;

			public byte[] pp_frame_data;

			public int pp_frame_state;

			public Xiph.th_ycbcr_buffer pp_frame_buf;

			public Xiph.th_stripe_callback stripe_cb;

			public Xiph.oc_dec_pipeline_state pipe = new Xiph.oc_dec_pipeline_state();

			public byte[] oc_dec_mb_modes_unpack__scheme0_alphabet = new byte[8];

			public long[][] oc_dec_residual_tokens_unpack__ntoks_left;

			public int[] oc_dec_residual_tokens_unpack__huff_idxs = new int[2];

			public long[] oc_dec_dc_coeff_unpack__run_counts = new long[64];

			public long[] oc_dec_ac_coeff_unpack__run_counts = new long[64];

			public short[] oc_dec_mv_unpack_and_frag_modes_fill__cbmvs = new short[4];

			public int[] oc_dec_mv_unpack_and_frag_modes_fill__coded = new int[13];

			public short[] oc_dec_mv_unpack_and_frag_modes_fill__lbmvs = new short[4];

			public ushort[] oc_dec_frags_recon_mcu_plane__dc_quant = new ushort[2];

			public th_dec_ctx()
			{
				this.oc_dec_residual_tokens_unpack__ntoks_left = new long[3][];
				this.oc_dec_residual_tokens_unpack__ntoks_left[0] = new long[64];
				this.oc_dec_residual_tokens_unpack__ntoks_left[1] = new long[64];
				this.oc_dec_residual_tokens_unpack__ntoks_left[2] = new long[64];
				this.ti0[0] = new long[64];
				this.ti0[1] = new long[64];
				this.ti0[2] = new long[64];
				this.eob_runs[0] = new long[64];
				this.eob_runs[1] = new long[64];
				this.eob_runs[2] = new long[64];
			}
		}

		public class th_decode_ctl_opts
		{
			public int pplevel_max;

			public int pplevel;

			public long granpos;

			public Xiph.th_stripe_callback stripe_callback;
		}

		public struct oc_sb_flags
		{
			public byte bits;

			public byte coded_fully
			{
				get
				{
					return this.bits & 1;
				}
				set
				{
					this.bits &= 254;
					this.bits |= (value & 1);
				}
			}

			public byte coded_partially
			{
				get
				{
					return (byte)((this.bits & 2) >> 1);
				}
				set
				{
					this.bits &= 253;
					this.bits |= (byte)((value & 1) << 1);
				}
			}

			public byte quad_valid
			{
				get
				{
					return (byte)((this.bits & 60) >> 2);
				}
				set
				{
					this.bits &= 195;
					this.bits |= (byte)((value & 15) << 2);
				}
			}
		}

		public struct oc_border_info
		{
			public long mask;

			public int npixels;
		}

		public struct oc_fragment
		{
			private uint bits;

			public uint coded;

			public uint invalid;

			public uint qii;

			public uint mb_mode;

			public int borderi;

			public int dc;
		}

		public struct oc_fragment_plane
		{
			public int nhfrags;

			public int nvfrags;

			public long froffset;

			public long nfrags;

			public uint nhsbs;

			public uint nvsbs;

			public uint sboffset;

			public uint nsbs;
		}

		public class oc_theora_state
		{
			public Xiph.th_info info = default(Xiph.th_info);

			public Xiph.ogg_ptr opt_data;

			public uint cpu_flags;

			public Xiph.oc_fragment_plane[] fplanes = new Xiph.oc_fragment_plane[3];

			public Xiph.oc_fragment[] frags;

			public long[] frag_buf_offs;

			public short[] frag_mvs;

			public long nfrags;

			public long[][][] sb_maps;

			public Xiph.oc_sb_flags[] sb_flags;

			public uint nsbs;

			public long[][][] mb_maps;

			public sbyte[] mb_modes;

			public uint nhmbs;

			public uint nvmbs;

			public uint nmbs;

			public long[] coded_fragis;

			public long[] ncoded_fragis = new long[3];

			public long ntotal_coded_fragis;

			public int[] ref_frame_idx = new int[6];

			public Xiph.th_ycbcr_buffer[] ref_frame_bufs = new Xiph.th_ycbcr_buffer[6];

			public byte[][] ref_frame_data = new byte[6][];

			public int[] ref_ystride = new int[3];

			public int nborders;

			public Xiph.oc_border_info[] borders = new Xiph.oc_border_info[16];

			public long keyframe_num;

			public long curframe_num;

			public long granpos;

			public sbyte frame_type;

			public byte granpos_bias;

			public byte nqis;

			public byte[] qis = new byte[3];

			public ushort[][][][] dequant_tables = new ushort[64][][][];

			public ushort[][][][] dequant_table_data = new ushort[64][][][];

			public byte[] loop_filter_limits = new byte[64];

			public short[] oc_idct8x8_w = new short[64];

			public oc_theora_state()
			{
				for (int i = 0; i < 64; i++)
				{
					this.dequant_table_data[i] = new ushort[3][][];
					for (int j = 0; j < 3; j++)
					{
						this.dequant_table_data[i][j] = new ushort[2][];
						for (int k = 0; k < 2; k++)
						{
							this.dequant_table_data[i][j][k] = new ushort[64];
						}
					}
				}
				for (int i = 0; i < 64; i++)
				{
					this.dequant_tables[i] = new ushort[3][][];
					for (int j = 0; j < 3; j++)
					{
						this.dequant_tables[i][j] = new ushort[2][];
						for (int k = 0; k < 2; k++)
						{
							this.dequant_tables[i][j][k] = new ushort[64];
						}
					}
				}
			}
		}

		private delegate void oc_set_chroma_mvs_func(short[] cbmvs, short[] lbmvs);

		private delegate void oc_mb_fill_cmapping_func(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane[] fplanes, uint xfrag0, uint yfrag0);

		public delegate void th_stripe_decoded_func(object ctx, ref Xiph.th_ycbcr_buffer buf, int yfrag0, int yfrag_end);

		public struct th_stripe_callback
		{
			public object ctx;

			public Xiph.th_stripe_decoded_func stripe_decoded;
		}

		public class vorbis_info_floor
		{
		}

		public class vorbis_look_floor
		{
		}

		public class vorbis_info_residue
		{
		}

		public class vorbis_look_residue
		{
		}

		public class vorbis_info_mapping
		{
		}

		public class vorbis_func_floor
		{
			public delegate void Pack(Xiph.vorbis_info_floor vif, Xiph.oggpack_buffer ob);

			public delegate Xiph.vorbis_info_floor Unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer ob);

			public delegate Xiph.vorbis_look_floor Look(Xiph.vorbis_dsp_state vds, Xiph.vorbis_info_floor vif);

			public delegate void FreeInfo(Xiph.vorbis_info_floor vif);

			public delegate void FreeLook(Xiph.vorbis_look_floor vlf);

			public delegate object Inverse1(Xiph.vorbis_block vb, Xiph.vorbis_look_floor vlf);

			public delegate int Inverse2(Xiph.vorbis_block vb, Xiph.vorbis_look_floor vlf, object obj, float[] _out);

			public Xiph.vorbis_func_floor.Pack pack;

			public Xiph.vorbis_func_floor.Unpack unpack;

			public Xiph.vorbis_func_floor.Look look;

			public Xiph.vorbis_func_floor.FreeInfo free_info;

			public Xiph.vorbis_func_floor.FreeLook free_look;

			public Xiph.vorbis_func_floor.Inverse1 inverse1;

			public Xiph.vorbis_func_floor.Inverse2 inverse2;
		}

		public class vorbis_info_floor0 : Xiph.vorbis_info_floor
		{
			public int order;

			public long rate;

			public long barkmap;

			public int ampbits;

			public int ampdB;

			public int numbooks;

			public int[] books = new int[16];

			public float lessthan;

			public float greaterthan;
		}

		public class vorbis_info_floor1 : Xiph.vorbis_info_floor
		{
			public int partitions;

			public int[] partitionclass = new int[31];

			public int[] class_dim = new int[16];

			public int[] class_subs = new int[16];

			public int[] class_book = new int[16];

			public int[][] class_subbook = new int[16][];

			public int mult;

			public int[] postlist = new int[65];

			public float maxover;

			public float maxunder;

			public float maxerr;

			public float twofitweight;

			public float twofitatten;

			public int n;

			public vorbis_info_floor1()
			{
				for (int i = 0; i < 16; i++)
				{
					this.class_subbook[i] = new int[8];
				}
			}
		}

		public class vorbis_func_residue
		{
			public delegate void Pack(Xiph.vorbis_info_residue vir, Xiph.oggpack_buffer ob);

			public delegate Xiph.vorbis_info_residue Unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer ob);

			public delegate Xiph.vorbis_look_residue Look(Xiph.vorbis_dsp_state vds, Xiph.vorbis_info_residue vir);

			public delegate void FreeInfo(Xiph.vorbis_info_residue vir);

			public delegate void FreeLook(Xiph.vorbis_look_residue vlr);

			public delegate long[][] Class(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vlr, int[][] a2, int[] a1, int a0);

			public delegate int Forward(Xiph.oggpack_buffer ob, Xiph.vorbis_block vb, Xiph.vorbis_look_residue vlr, int[][] a2, int[] a1, int a0, long[][] l2, int y);

			public delegate int Inverse(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vlr, float[][] f2, int[] i1, int i);

			public Xiph.vorbis_func_residue.Pack pack;

			public Xiph.vorbis_func_residue.Unpack unpack;

			public Xiph.vorbis_func_residue.Look look;

			public Xiph.vorbis_func_residue.FreeInfo free_info;

			public Xiph.vorbis_func_residue.FreeLook free_look;

			public Xiph.vorbis_func_residue.Class _class;

			public Xiph.vorbis_func_residue.Forward forward;

			public Xiph.vorbis_func_residue.Inverse inverse;
		}

		public class vorbis_info_residue0 : Xiph.vorbis_info_residue
		{
			public long begin;

			public long end;

			public int grouping;

			public int partitions;

			public int partvals;

			public int groupbook;

			public int[] secondstages = new int[64];

			public int[] booklist = new int[512];

			public int[] classmetric1 = new int[64];

			public int[] classmetric2 = new int[64];
		}

		public class vorbis_info_mapping0 : Xiph.vorbis_info_mapping
		{
			public int submaps;

			public int[] chmuxlist = new int[256];

			public int[] floorsubmap = new int[16];

			public int[] residuesubmap = new int[16];

			public int coupling_steps;

			public int[] coupling_mag = new int[256];

			public int[] coupling_ang = new int[256];
		}

		public class vorbis_func_mapping
		{
			public delegate void Pack(Xiph.vorbis_info vi, Xiph.vorbis_info_mapping vim, Xiph.oggpack_buffer ob);

			public delegate Xiph.vorbis_info_mapping Unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer ob);

			public delegate void FreeInfo(Xiph.vorbis_info_mapping vim);

			public delegate int Forward(Xiph.vorbis_block vb);

			public delegate int Inverse(Xiph.vorbis_block vb, Xiph.vorbis_info_mapping vim);

			public Xiph.vorbis_func_mapping.Pack pack;

			public Xiph.vorbis_func_mapping.Unpack unpack;

			public Xiph.vorbis_func_mapping.FreeInfo free_info;

			public Xiph.vorbis_func_mapping.Forward forward;

			public Xiph.vorbis_func_mapping.Inverse inverse;
		}

		public class bitrate_manager_state
		{
			public int managed;

			public long avg_reservoir;

			public long minmax_reservoir;

			public long avg_bitsper;

			public long min_bitsper;

			public long max_bitsper;

			public long short_per_long;

			public double avgfloat;

			public Xiph.vorbis_block vb;

			public int choice;
		}

		public class bitrate_manager_info
		{
			public long avg_rate;

			public long min_rate;

			public long max_rate;

			public long reservoir_bits;

			public double reservoir_bias;

			public double slew_damp;
		}

		public class static_codebook
		{
			public long dim;

			public long entries;

			public long[] lengthlist;

			public int maptype;

			public long q_min;

			public long q_delta;

			public int q_quant;

			public int q_sequencep;

			public long[] quantlist;

			public int allocedp;
		}

		public class codebook
		{
			public long dim;

			public long entries;

			public long used_entries;

			public Xiph.static_codebook c;

			public float[] valuelist;

			public uint[] codelist;

			public int[] dec_index;

			public byte[] dec_codelengths;

			public uint[] dec_firsttable;

			public int dec_firsttablen;

			public int dec_maxlength;

			public int quantvals;

			public int minval;

			public int delta;
		}

		public class vorbis_info
		{
			public int version;

			public int channels;

			public long rate;

			public long bitrate_upper;

			public long bitrate_nominal;

			public long bitrate_lower;

			public long bitrate_window;

			public Xiph.codec_setup_info codec_setup;
		}

		public class vorbis_dsp_state
		{
			public int analysisp;

			public Xiph.vorbis_info vi;

			public float[][] pcm;

			public int[] pcmret;

			public int pcm_storage;

			public int pcm_current;

			public int pcm_returned;

			public int preextrapolate;

			public int eofflag;

			public long lW;

			public long W;

			public long nW;

			public long centerW;

			public long granulepos;

			public long sequence;

			public long glue_bits;

			public long time_bits;

			public long floor_bits;

			public long res_bits;

			public Xiph.private_state backend_state;
		}

		public class vorbis_block
		{
			public float[][] pcm;

			public Xiph.oggpack_buffer opb = new Xiph.oggpack_buffer();

			public long lW;

			public long W;

			public long nW;

			public int pcmend;

			public int mode;

			public int eofflag;

			public long granulepos;

			public long sequence;

			public Xiph.vorbis_dsp_state vd;

			public byte[] localstore;

			public long localtop;

			public long localalloc;

			public long totaluse;

			public long glue_bits;

			public long time_bits;

			public long floor_bits;

			public long res_bits;

			public Xiph.vorbis_block_internal internl;
		}

		public class vorbis_comment
		{
			public string[] user_comments;

			public string vendor;
		}

		public class vorbis_block_internal
		{
			public float[][] pcmdelay;

			public float ampmax;

			public int blocktype;

			public Xiph.oggpack_buffer[] packetblob = new Xiph.oggpack_buffer[15];
		}

		public class vorbis_info_mode
		{
			public int blockflag;

			public int windowtype;

			public int transformtype;

			public int mapping;
		}

		public class private_state
		{
			public Xiph.envelope_lookup ve;

			public int[] window = new int[2];

			public Xiph.mdct_lookup[][] transform = new Xiph.mdct_lookup[2][];

			public Xiph.drft_lookup[] fft_look = new Xiph.drft_lookup[2];

			public int modebits;

			public Xiph.vorbis_look_floor[] flr;

			public Xiph.vorbis_look_residue[] residue;

			public Xiph.vorbis_look_psy[] psy;

			public Xiph.vorbis_look_psy_global[] psy_g_look;

			public byte[] header;

			public byte[] header1;

			public byte[] header2;

			public Xiph.bitrate_manager_state bms;

			public long sample_count;

			public private_state()
			{
				this.fft_look[0] = new Xiph.drft_lookup();
				this.fft_look[1] = new Xiph.drft_lookup();
			}
		}

		public class codec_setup_info
		{
			public long[] blocksizes = new long[2];

			public int modes;

			public int maps;

			public int floors;

			public int residues;

			public int books;

			public int psys;

			public Xiph.vorbis_info_mode[] mode_param = new Xiph.vorbis_info_mode[64];

			public int[] map_type = new int[64];

			public Xiph.vorbis_info_mapping[] map_param = new Xiph.vorbis_info_mapping[64];

			public int[] floor_type = new int[64];

			public Xiph.vorbis_info_floor[] floor_param = new Xiph.vorbis_info_floor[64];

			public int[] residue_type = new int[64];

			public Xiph.vorbis_info_residue[] residue_param = new Xiph.vorbis_info_residue[64];

			public Xiph.static_codebook[] book_param = new Xiph.static_codebook[256];

			public Xiph.codebook[] fullbooks;

			public Xiph.vorbis_info_psy[] psy_param = new Xiph.vorbis_info_psy[4];

			public Xiph.vorbis_info_psy_global psy_g_param;

			public Xiph.bitrate_manager_info bi;

			public Xiph.highlevel_encode_setup hi;

			public int halfrate_flag;
		}

		public class vorbis_look_floor1 : Xiph.vorbis_look_floor
		{
			public int[] sorted_index = new int[65];

			public int[] forward_index = new int[65];

			public int[] reverse_index = new int[65];

			public int[] hineighbor = new int[63];

			public int[] loneighbor = new int[63];

			public int posts;

			public int n;

			public int quant_q;

			public Xiph.vorbis_info_floor1 vi;

			public long phrasebits;

			public long postbits;

			public long frames;
		}

		public class envelope_filter_state
		{
			public float[] ampbuf = new float[17];

			public int ampptr;

			public float[] nearDC = new float[15];

			public float nearDC_acc;

			public float nearDC_partialacc;

			public int nearptr;
		}

		public class envelope_band
		{
			public int begin;

			public int end;

			public float[] window;

			public float total;
		}

		public class envelope_lookup
		{
			public int ch;

			public int winlength;

			public int searchstep;

			public float minenergy;

			public Xiph.mdct_lookup mdct;

			public float[] mdct_win;

			public Xiph.envelope_band[] band = new Xiph.envelope_band[7];

			public Xiph.envelope_filter_state filter;

			public int stretch;

			public int[] mark;

			public long storage;

			public long current;

			public long curmark;

			public long cursor;

			public envelope_lookup()
			{
				for (int i = 0; i < 7; i++)
				{
					this.band[i] = new Xiph.envelope_band();
				}
			}
		}

		private class vorbis_look_floor0 : Xiph.vorbis_look_floor
		{
			public int ln;

			public int m;

			public int[][] linearmap;

			public int[] n = new int[2];

			public Xiph.vorbis_info_floor0 vi;

			public long bits;

			public long frames;
		}

		private class lsfit_acc
		{
			public int x0;

			public int x1;

			public int xa;

			public int ya;

			public int x2a;

			public int y2a;

			public int xya;

			public int an;

			public int xb;

			public int yb;

			public int x2b;

			public int y2b;

			public int xyb;

			public int bn;

			public void clear()
			{
				this.x0 = (this.x1 = (this.xa = (this.ya = (this.x2a = (this.y2a = (this.xya = (this.an = (this.xb = (this.yb = (this.x2b = (this.y2b = (this.xyb = (this.bn = 0)))))))))))));
			}
		}

		public class highlevel_byblocktype
		{
			public double tone_mask_setting;

			public double tone_peaklimit_setting;

			public double noise_bias_setting;

			public double noise_compand_setting;
		}

		public class highlevel_encode_setup
		{
			public int set_in_stone;

			public object setup;

			public double base_setting;

			public double impulse_noisetune;

			public float req;

			public int managed;

			public long bitrate_min;

			public long bitrate_av;

			public double bitrate_av_damp;

			public long bitrate_max;

			public long bitrate_reservoir;

			public double bitrate_reservoir_bias;

			public int impulse_block_p;

			public int noise_normalize_p;

			public int coupling_p;

			public double stereo_point_setting;

			public double lowpass_kHz;

			public int lowpass_altered;

			public double ath_floating_dB;

			public double ath_absolute_dB;

			public double amplitude_track_dBpersec;

			public double trigger_setting;

			public Xiph.highlevel_byblocktype[] block = new Xiph.highlevel_byblocktype[4];
		}

		public class mdct_lookup
		{
			public int n;

			public int log2n;

			public float[] trig;

			public int[] bitrev;

			public float scale;
		}

		public class vorbis_info_psy
		{
			public int blockflag;

			public float ath_adjatt;

			public float ath_maxatt;

			public float[] tone_masteratt = new float[3];

			public float tone_centerboost;

			public float tone_decay;

			public float tone_abs_limit;

			public float[] toneatt = new float[17];

			public int noisemaskp;

			public float noisemaxsupp;

			public float noisewindowlo;

			public float noisewindowhi;

			public int noisewindowlomin;

			public int noisewindowhimin;

			public int noisewindowfixed;

			public float[][] noiseoff = new float[3][];

			public float[] noisecompand = new float[40];

			public float max_curve_dB;

			public int normal_p;

			public int normal_start;

			public int normal_partition;

			public double normal_thresh;

			public vorbis_info_psy()
			{
				for (int i = 0; i < 3; i++)
				{
					this.noiseoff[i] = new float[17];
				}
			}
		}

		public class vorbis_info_psy_global
		{
			public int eighth_octave_lines;

			public float[] preecho_thresh = new float[7];

			public float[] postecho_thresh = new float[7];

			public float stretch_penalty;

			public float preecho_minenergy;

			public float ampmax_att_per_sec;

			public int[] coupling_pkHz = new int[15];

			public int[][] coupling_pointlimit = new int[2][];

			public int[] coupling_prepointamp = new int[15];

			public int[] coupling_postpointamp = new int[15];

			public int[][] sliding_lowpass = new int[2][];

			public vorbis_info_psy_global()
			{
				this.coupling_pointlimit[0] = new int[15];
				this.coupling_pointlimit[1] = new int[15];
				this.sliding_lowpass[0] = new int[15];
				this.sliding_lowpass[1] = new int[15];
			}
		}

		public class vorbis_look_psy_global
		{
			public float ampmax;

			public int channels;

			public Xiph.vorbis_info_psy_global gi;

			public int[][] coupling_pointlimit = new int[2][];

			public vorbis_look_psy_global()
			{
				this.coupling_pointlimit[0] = new int[3];
				this.coupling_pointlimit[1] = new int[3];
			}
		}

		public class vorbis_look_psy
		{
			public int n;

			public Xiph.vorbis_info_psy vi;

			public float[][][] tonecurves;

			public float[][] noiseoffset;

			public float[] ath;

			public long[] octave;

			public long[] bark;

			public long firstoc;

			public long shiftoc;

			public int eighth_octave_lines;

			public int total_octave_lines;

			public long rate;

			public float m_val;
		}

		private class vorbis_look_residue0 : Xiph.vorbis_look_residue
		{
			public Xiph.vorbis_info_residue0 info;

			public int parts;

			public int stages;

			public Xiph.codebook[] fullbooks;

			public Xiph.codebook phrasebook;

			public Xiph.codebook[][] partbooks;

			public int partvals;

			public int[][] decodemap;

			public long postbits;

			public long phrasebits;

			public long frames;
		}

		private delegate int Encode(Xiph.oggpack_buffer b, int[] samples, int samples_offset, int samples_per_partition, Xiph.codebook statebook, long[] acc);

		private delegate long Decodepart(Xiph.codebook book, float[] v, int vo, Xiph.oggpack_buffer ob, int a);

		[StructLayout(LayoutKind.Explicit)]
		private struct uint_float
		{
			[FieldOffset(0)]
			public uint i;

			[FieldOffset(0)]
			public float f;
		}

		public class drft_lookup
		{
			public int n;

			public float[] trigcache;

			public int[] splitcache;
		}

		public const int BUFFER_INCREMENT = 256;

		private const int CHAR_BIT = 8;

		private const int INT_MAX = 2147483647;

		private const int LONG_MAX = 2147483647;

		private const int OC_PB_WINDOW_SIZE = 32;

		private const int OC_LOTS_OF_BITS = 1073741824;

		public const int TH_EFAULT = -1;

		public const int TH_EINVAL = -10;

		public const int TH_EBADHEADER = -20;

		public const int TH_ENOTFORMAT = -21;

		public const int TH_EVERSION = -22;

		public const int TH_EIMPL = -23;

		public const int TH_EBADPACKET = -24;

		public const int TH_DUPFRAME = 1;

		private const int TH_NHUFFMAN_TABLES = 80;

		private const int TH_NDCT_TOKENS = 32;

		private const int OC_PACKET_DATA = 0;

		private const int OC_PP_LEVEL_DISABLED = 0;

		private const int OC_PP_LEVEL_TRACKDCQI = 1;

		private const int OC_PP_LEVEL_DEBLOCKY = 2;

		private const int OC_PP_LEVEL_DERINGY = 3;

		private const int OC_PP_LEVEL_SDERINGY = 4;

		private const int OC_PP_LEVEL_DEBLOCKC = 5;

		private const int OC_PP_LEVEL_DERINGC = 6;

		private const int OC_PP_LEVEL_SDERINGC = 7;

		private const int OC_PP_LEVEL_MAX = 7;

		public const int OC_INTERNAL_DCT_TOKEN_EXTRA_BITS_LENGTH = 15;

		private const int OC_DCT_TOKEN_FAT_EOB = 0;

		private const uint OC_DCT_EOB_FINISH = 2147483647u;

		private const int OC_DCT_CW_RLEN_SHIFT = 0;

		private const int OC_DCT_CW_EOB_SHIFT = 8;

		private const int OC_DCT_CW_FLIP_BIT = 20;

		private const int OC_DCT_CW_MAG_SHIFT = 21;

		private const int OC_DCT_CW_FINISH = 0;

		private const int OC_DERING_THRESH1 = 384;

		private const int OC_DERING_THRESH2 = 1536;

		private const int OC_DERING_THRESH3 = 1920;

		private const int OC_DERING_THRESH4 = 3840;

		public const int OC_HUFF_SLUSH = 2;

		public const int OC_ROOT_HUFF_SLUSH = 7;

		public const int OC_DCT_VAL_RANGE = 580;

		public const int OC_NDCT_TOKEN_BITS = 5;

		public const int OC_DCT_EOB1_TOKEN = 0;

		public const int OC_DCT_EOB2_TOKEN = 1;

		public const int OC_DCT_EOB3_TOKEN = 2;

		public const int OC_DCT_REPEAT_RUN0_TOKEN = 3;

		public const int OC_DCT_REPEAT_RUN1_TOKEN = 4;

		public const int OC_DCT_REPEAT_RUN2_TOKEN = 5;

		public const int OC_DCT_REPEAT_RUN3_TOKEN = 6;

		public const int OC_DCT_SHORT_ZRL_TOKEN = 7;

		public const int OC_DCT_ZRL_TOKEN = 8;

		public const int OC_ONE_TOKEN = 9;

		public const int OC_MINUS_ONE_TOKEN = 10;

		public const int OC_TWO_TOKEN = 11;

		public const int OC_MINUS_TWO_TOKEN = 12;

		public const int OC_DCT_VAL_CAT2 = 13;

		public const int OC_DCT_VAL_CAT3 = 17;

		public const int OC_DCT_VAL_CAT4 = 18;

		public const int OC_DCT_VAL_CAT5 = 19;

		public const int OC_DCT_VAL_CAT6 = 20;

		public const int OC_DCT_VAL_CAT7 = 21;

		public const int OC_DCT_VAL_CAT8 = 22;

		public const int OC_DCT_RUN_CAT1A = 23;

		public const int OC_DCT_RUN_CAT1B = 28;

		public const int OC_DCT_RUN_CAT1C = 29;

		public const int OC_DCT_RUN_CAT2A = 30;

		public const int OC_DCT_RUN_CAT2B = 31;

		public const int OC_NDCT_EOB_TOKEN_MAX = 7;

		public const int OC_NDCT_ZRL_TOKEN_MAX = 9;

		public const int OC_NDCT_VAL_MAX = 23;

		public const int OC_NDCT_VAL_CAT1_MAX = 13;

		public const int OC_NDCT_VAL_CAT2_MAX = 17;

		public const int OC_NDCT_VAL_CAT2_SIZE = 4;

		public const int OC_NDCT_RUN_MAX = 32;

		public const int OC_NDCT_RUN_CAT1A_MAX = 28;

		private const int OC_C1S7 = 64277;

		private const int OC_C2S6 = 60547;

		private const int OC_C3S5 = 54491;

		private const int OC_C4S4 = 46341;

		private const int OC_C5S3 = 36410;

		private const int OC_C6S2 = 25080;

		private const int OC_C7S1 = 12785;

		public const string OC_VENDOR_STRING = "OrangeTree MonoMedia 1.0.0alpha 20120928 (Kuzya)";

		public const byte TH_VERSION_MAJOR = 3;

		public const byte TH_VERSION_MINOR = 2;

		public const byte TH_VERSION_SUB = 1;

		private const uint OC_QUANT_MAX = 4096u;

		private const int OC_INTRA_FRAME = 0;

		private const int OC_INTER_FRAME = 1;

		private const int OC_UNKWN_FRAME = -1;

		private const int OC_UMV_PADDING = 16;

		private const int OC_FRAME_GOLD = 0;

		private const int OC_FRAME_PREV = 1;

		private const int OC_FRAME_SELF = 2;

		private const int OC_FRAME_IO = 3;

		private const int OC_FRAME_GOLD_ORIG = 4;

		private const int OC_FRAME_PREV_ORIG = 5;

		private const int OC_MODE_INVALID = -1;

		private const int OC_MODE_INTER_NOMV = 0;

		private const int OC_MODE_INTRA = 1;

		private const int OC_MODE_INTER_MV = 2;

		private const int OC_MODE_INTER_MV_LAST = 3;

		private const int OC_MODE_INTER_MV_LAST2 = 4;

		private const int OC_MODE_GOLDEN_NOMV = 5;

		private const int OC_MODE_GOLDEN_MV = 6;

		private const int OC_MODE_INTER_MV_FOUR = 7;

		private const int OC_NMODES = 8;

		private const int OC_PACKET_INFO_HDR = -3;

		private const int OC_PACKET_COMMENT_HDR = -2;

		private const int OC_PACKET_SETUP_HDR = -1;

		private const int OC_PACKET_DONE = 2147483647;

		private const int TH_DECCTL_GET_PPLEVEL_MAX = 1;

		private const int TH_DECCTL_SET_PPLEVEL = 3;

		private const int TH_DECCTL_SET_GRANPOS = 5;

		public const int TH_DECCTL_SET_STRIPE_CB = 7;

		private const int TH_DECCTL_SET_TELEMETRY_MBMODE = 9;

		private const int TH_DECCTL_SET_TELEMETRY_MV = 11;

		private const int TH_DECCTL_SET_TELEMETRY_QI = 13;

		private const int TH_DECCTL_SET_TELEMETRY_BITS = 15;

		public const int VIF_POSIT = 63;

		public const int VIF_CLASS = 16;

		public const int VIF_PARTS = 31;

		private const int WORD_ALIGN = 8;

		public const int OV_FALSE = -1;

		public const int OV_EOF = -2;

		public const int OV_HOLE = -3;

		public const int OV_EREAD = -128;

		public const int OV_EFAULT = -129;

		public const int OV_EIMPL = -130;

		public const int OV_EINVAL = -131;

		public const int OV_ENOTVORBIS = -132;

		public const int OV_EBADHEADER = -133;

		public const int OV_EVERSION = -134;

		public const int OV_ENOTAUDIO = -135;

		public const int OV_EBADPACKET = -136;

		public const int OV_EBADLINK = -137;

		public const int OV_ENOSEEK = -138;

		public const int BLOCKTYPE_IMPULSE = 0;

		public const int BLOCKTYPE_PADDING = 1;

		public const int BLOCKTYPE_TRANSITION = 0;

		public const int BLOCKTYPE_LONG = 1;

		public const int PACKETBLOBS = 15;

		public const int VE_PRE = 16;

		public const int VE_WIN = 4;

		public const int VE_POST = 2;

		public const int VE_AMP = 17;

		public const int VE_BANDS = 7;

		public const int VE_NEARDC = 15;

		public const int VE_MINSTRETCH = 2;

		public const int VE_MAXSTRETCH = 12;

		private const int floor1_rangedB = 140;

		private const int MAX_ATH = 88;

		private const int EHMER_OFFSET = 16;

		private const int EHMER_MAX = 56;

		public const float cPI3_8 = 0.382683426f;

		public const float cPI2_8 = 0.707106769f;

		public const float cPI1_8 = 0.9238795f;

		private const int P_BANDS = 17;

		private const int P_LEVELS = 8;

		private const float P_LEVEL_0 = 30f;

		private const int P_NOISECURVES = 3;

		private const int NOISE_COMPAND_LEVELS = 40;

		private const float NEGINF = -9999f;

		private const int VI_TRANSFORMB = 1;

		private const int VI_WINDOWB = 1;

		private const int VI_TIMEB = 1;

		private const int VI_FLOORB = 2;

		private const int VI_RESB = 3;

		private const int VI_MAPB = 1;

		private const int VQ_FEXP = 10;

		private const int VQ_FMAN = 21;

		private const int VQ_FEXP_BIAS = 768;

		private static readonly uint[] crc_lookup = new uint[]
		{
			0u,
			79764919u,
			159529838u,
			222504665u,
			319059676u,
			398814059u,
			445009330u,
			507990021u,
			638119352u,
			583659535u,
			797628118u,
			726387553u,
			890018660u,
			835552979u,
			1015980042u,
			944750013u,
			1276238704u,
			1221641927u,
			1167319070u,
			1095957929u,
			1595256236u,
			1540665371u,
			1452775106u,
			1381403509u,
			1780037320u,
			1859660671u,
			1671105958u,
			1733955601u,
			2031960084u,
			2111593891u,
			1889500026u,
			1952343757u,
			2552477408u,
			2632100695u,
			2443283854u,
			2506133561u,
			2334638140u,
			2414271883u,
			2191915858u,
			2254759653u,
			3190512472u,
			3135915759u,
			3081330742u,
			3009969537u,
			2905550212u,
			2850959411u,
			2762807018u,
			2691435357u,
			3560074640u,
			3505614887u,
			3719321342u,
			3648080713u,
			3342211916u,
			3287746299u,
			3467911202u,
			3396681109u,
			4063920168u,
			4143685023u,
			4223187782u,
			4286162673u,
			3779000052u,
			3858754371u,
			3904687514u,
			3967668269u,
			881225847u,
			809987520u,
			1023691545u,
			969234094u,
			662832811u,
			591600412u,
			771767749u,
			717299826u,
			311336399u,
			374308984u,
			453813921u,
			533576470u,
			25881363u,
			88864420u,
			134795389u,
			214552010u,
			2023205639u,
			2086057648u,
			1897238633u,
			1976864222u,
			1804852699u,
			1867694188u,
			1645340341u,
			1724971778u,
			1587496639u,
			1516133128u,
			1461550545u,
			1406951526u,
			1302016099u,
			1230646740u,
			1142491917u,
			1087903418u,
			2896545431u,
			2825181984u,
			2770861561u,
			2716262478u,
			3215044683u,
			3143675388u,
			3055782693u,
			3001194130u,
			2326604591u,
			2389456536u,
			2200899649u,
			2280525302u,
			2578013683u,
			2640855108u,
			2418763421u,
			2498394922u,
			3769900519u,
			3832873040u,
			3912640137u,
			3992402750u,
			4088425275u,
			4151408268u,
			4197601365u,
			4277358050u,
			3334271071u,
			3263032808u,
			3476998961u,
			3422541446u,
			3585640067u,
			3514407732u,
			3694837229u,
			3640369242u,
			1762451694u,
			1842216281u,
			1619975040u,
			1682949687u,
			2047383090u,
			2127137669u,
			1938468188u,
			2001449195u,
			1325665622u,
			1271206113u,
			1183200824u,
			1111960463u,
			1543535498u,
			1489069629u,
			1434599652u,
			1363369299u,
			622672798u,
			568075817u,
			748617968u,
			677256519u,
			907627842u,
			853037301u,
			1067152940u,
			995781531u,
			51762726u,
			131386257u,
			177728840u,
			240578815u,
			269590778u,
			349224269u,
			429104020u,
			491947555u,
			4046411278u,
			4126034873u,
			4172115296u,
			4234965207u,
			3794477266u,
			3874110821u,
			3953728444u,
			4016571915u,
			3609705398u,
			3555108353u,
			3735388376u,
			3664026991u,
			3290680682u,
			3236090077u,
			3449943556u,
			3378572211u,
			3174993278u,
			3120533705u,
			3032266256u,
			2961025959u,
			2923101090u,
			2868635157u,
			2813903052u,
			2742672763u,
			2604032198u,
			2683796849u,
			2461293480u,
			2524268063u,
			2284983834u,
			2364738477u,
			2175806836u,
			2238787779u,
			1569362073u,
			1498123566u,
			1409854455u,
			1355396672u,
			1317987909u,
			1246755826u,
			1192025387u,
			1137557660u,
			2072149281u,
			2135122070u,
			1912620623u,
			1992383480u,
			1753615357u,
			1816598090u,
			1627664531u,
			1707420964u,
			295390185u,
			358241886u,
			404320391u,
			483945776u,
			43990325u,
			106832002u,
			186451547u,
			266083308u,
			932423249u,
			861060070u,
			1041341759u,
			986742920u,
			613929101u,
			542559546u,
			756411363u,
			701822548u,
			3316196985u,
			3244833742u,
			3425377559u,
			3370778784u,
			3601682597u,
			3530312978u,
			3744426955u,
			3689838204u,
			3819031489u,
			3881883254u,
			3928223919u,
			4007849240u,
			4037393693u,
			4100235434u,
			4180117107u,
			4259748804u,
			2310601993u,
			2373574846u,
			2151335527u,
			2231098320u,
			2596047829u,
			2659030626u,
			2470359227u,
			2550115596u,
			2947551409u,
			2876312838u,
			2788305887u,
			2733848168u,
			3165939309u,
			3094707162u,
			3040238851u,
			2985771188u
		};

		private static readonly ulong[] mask = new ulong[]
		{
			0uL,
			1uL,
			3uL,
			7uL,
			15uL,
			31uL,
			63uL,
			127uL,
			255uL,
			511uL,
			1023uL,
			2047uL,
			4095uL,
			8191uL,
			16383uL,
			32767uL,
			65535uL,
			131071uL,
			262143uL,
			524287uL,
			1048575uL,
			2097151uL,
			4194303uL,
			8388607uL,
			16777215uL,
			33554431uL,
			67108863uL,
			134217727uL,
			268435455uL,
			536870911uL,
			1073741823uL,
			2147483647uL,
			4294967295uL
		};

		private static readonly uint[] mask8B = new uint[]
		{
			0u,
			128u,
			192u,
			224u,
			240u,
			248u,
			252u,
			254u,
			255u
		};

		private static readonly byte[][] OC_MODE_ALPHABETS = new byte[][]
		{
			new byte[]
			{
				3,
				4,
				2,
				0,
				1,
				5,
				6,
				7
			},
			new byte[]
			{
				3,
				4,
				0,
				2,
				1,
				5,
				6,
				7
			},
			new byte[]
			{
				3,
				2,
				4,
				0,
				1,
				5,
				6,
				7
			},
			new byte[]
			{
				3,
				2,
				0,
				4,
				1,
				5,
				6,
				7
			},
			new byte[]
			{
				0,
				3,
				4,
				2,
				1,
				5,
				6,
				7
			},
			new byte[]
			{
				0,
				5,
				3,
				4,
				2,
				1,
				6,
				7
			},
			new byte[]
			{
				0,
				1,
				2,
				3,
				4,
				5,
				6,
				7
			}
		};

		private static readonly byte[] OC_INTERNAL_DCT_TOKEN_EXTRA_BITS = new byte[]
		{
			12,
			4,
			3,
			3,
			4,
			4,
			5,
			5,
			8,
			8,
			8,
			8,
			3,
			3,
			6
		};

		private static readonly int[] OC_DCT_CODE_WORD = new int[]
		{
			0,
			Xiph.OC_DCT_CW_PACK(16, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 13, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 13, 1),
			Xiph.OC_DCT_CW_PACK(0, 0, 21, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 21, 1),
			Xiph.OC_DCT_CW_PACK(0, 0, 37, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 37, 1),
			Xiph.OC_DCT_CW_PACK(0, 0, 69, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 325, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 69, 1),
			Xiph.OC_DCT_CW_PACK(0, 0, 325, 1),
			Xiph.OC_DCT_CW_PACK(0, 10, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 10, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 0, 1),
			Xiph.OC_DCT_CW_PACK(1, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(2, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(3, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 4, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 4, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 5, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 5, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, 2, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, 3, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, -2, 0),
			Xiph.OC_DCT_CW_PACK(0, 1, -3, 0),
			Xiph.OC_DCT_CW_PACK(0, 6, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 7, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 8, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 9, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 6, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 7, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 8, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 9, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, 2, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, 2, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, 3, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, 3, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, -2, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, -2, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, -3, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, -3, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 0, 1),
			Xiph.OC_DCT_CW_PACK(0, 1, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 2, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 3, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 4, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 5, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 6, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 7, 0, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 1, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -1, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 2, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -2, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 3, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -3, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 4, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -4, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 5, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -5, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 6, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -6, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 7, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 8, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -7, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -8, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 9, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 10, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 11, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, 12, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -9, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -10, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -11, 0),
			Xiph.OC_DCT_CW_PACK(0, 0, -12, 0),
			Xiph.OC_DCT_CW_PACK(8, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(9, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(10, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(11, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(12, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(13, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(14, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(15, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(4, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(5, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(6, 0, 0, 0),
			Xiph.OC_DCT_CW_PACK(7, 0, 0, 0)
		};

		private static readonly short[] OC_SB_RUN_TREE = new short[]
		{
			4,
			-257,
			-257,
			-257,
			-257,
			-257,
			-257,
			-257,
			-257,
			-770,
			-770,
			-771,
			-771,
			-1028,
			-1029,
			-1056,
			17,
			2,
			-548,
			-552,
			-588,
			-732
		};

		private static readonly short[] OC_BLOCK_RUN_TREE = new short[]
		{
			5,
			-513,
			-513,
			-513,
			-513,
			-513,
			-513,
			-513,
			-513,
			-514,
			-514,
			-514,
			-514,
			-514,
			-514,
			-514,
			-514,
			-771,
			-771,
			-771,
			-771,
			-772,
			-772,
			-772,
			-772,
			-1029,
			-1029,
			-1030,
			-1030,
			33,
			36,
			39,
			44,
			1,
			-263,
			-264,
			1,
			-265,
			-266,
			2,
			-523,
			-524,
			-525,
			-526,
			4,
			-1039,
			-1040,
			-1041,
			-1042,
			-1043,
			-1044,
			-1045,
			-1046,
			-1047,
			-1048,
			-1049,
			-1050,
			-1051,
			-1052,
			-1053,
			-1054
		};

		private static readonly short[] OC_VLC_MODE_TREE = new short[]
		{
			4,
			-256,
			-256,
			-256,
			-256,
			-256,
			-256,
			-256,
			-256,
			-513,
			-513,
			-513,
			-513,
			-770,
			-770,
			-1027,
			17,
			3,
			-260,
			-260,
			-260,
			-260,
			-517,
			-517,
			-774,
			-775
		};

		private static readonly short[] OC_CLC_MODE_TREE = new short[]
		{
			3,
			-768,
			-769,
			-770,
			-771,
			-772,
			-773,
			-774,
			-775
		};

		private static readonly short[] OC_VLC_MV_COMP_TREE = new short[]
		{
			5,
			-800,
			-800,
			-800,
			-800,
			-801,
			-801,
			-801,
			-801,
			-799,
			-799,
			-799,
			-799,
			-1058,
			-1058,
			-1054,
			-1054,
			-1059,
			-1059,
			-1053,
			-1053,
			33,
			36,
			39,
			42,
			45,
			50,
			55,
			60,
			65,
			74,
			83,
			92,
			1,
			-292,
			-284,
			1,
			-293,
			-283,
			1,
			-294,
			-282,
			1,
			-295,
			-281,
			2,
			-552,
			-536,
			-553,
			-535,
			2,
			-554,
			-534,
			-555,
			-533,
			2,
			-556,
			-532,
			-557,
			-531,
			2,
			-558,
			-530,
			-559,
			-529,
			3,
			-816,
			-784,
			-817,
			-783,
			-818,
			-782,
			-819,
			-781,
			3,
			-820,
			-780,
			-821,
			-779,
			-822,
			-778,
			-823,
			-777,
			3,
			-824,
			-776,
			-825,
			-775,
			-826,
			-774,
			-827,
			-773,
			3,
			-828,
			-772,
			-829,
			-771,
			-830,
			-770,
			-831,
			-769
		};

		private static readonly short[] OC_CLC_MV_COMP_TREE = new short[]
		{
			6,
			-1568,
			-1568,
			-1569,
			-1567,
			-1570,
			-1566,
			-1571,
			-1565,
			-1572,
			-1564,
			-1573,
			-1563,
			-1574,
			-1562,
			-1575,
			-1561,
			-1576,
			-1560,
			-1577,
			-1559,
			-1578,
			-1558,
			-1579,
			-1557,
			-1580,
			-1556,
			-1581,
			-1555,
			-1582,
			-1554,
			-1583,
			-1553,
			-1584,
			-1552,
			-1585,
			-1551,
			-1586,
			-1550,
			-1587,
			-1549,
			-1588,
			-1548,
			-1589,
			-1547,
			-1590,
			-1546,
			-1591,
			-1545,
			-1592,
			-1544,
			-1593,
			-1543,
			-1594,
			-1542,
			-1595,
			-1541,
			-1596,
			-1540,
			-1597,
			-1539,
			-1598,
			-1538,
			-1599,
			-1537
		};

		private static readonly byte[] OC_HUFF_LIST_MAX = new byte[]
		{
			1,
			6,
			15,
			28,
			64
		};

		private static readonly byte[] OC_MOD_MAX = new byte[]
		{
			24,
			32
		};

		private static readonly byte[] OC_MOD_SHIFT;

		private static readonly byte[] OC_DCT_TOKEN_MAP;

		private static readonly byte[] OC_DCT_TOKEN_MAP_LOG_NENTRIES;

		private static readonly byte[] OC_FZIG_ZAG;

		private static readonly byte[] OC_IZIG_ZAG;

		private static readonly byte[,] OC_MB_MAP;

		private static readonly byte[][] OC_MB_MAP_IDXS;

		private static readonly byte[] OC_MB_MAP_NIDXS;

		private static readonly byte[] OC_DCT_TOKEN_EXTRA_BITS;

		private static readonly uint[] OC_DC_QUANT_MIN;

		private static readonly uint[] OC_AC_QUANT_MIN;

		private static readonly Xiph.oc_set_chroma_mvs_func[] OC_SET_CHROMA_MVS_TABLE;

		private static readonly int[,,] SB_MAP;

		private static readonly Xiph.oc_mb_fill_cmapping_func[] OC_MB_FILL_CMAPPING_TABLE;

		private static readonly sbyte[][] OC_MVMAP;

		private static readonly sbyte[][] OC_MVMAP2;

		private static Xiph.vorbis_func_floor floor0_exportbundle;

		private static readonly float[] FLOOR1_fromdB_LOOKUP;

		private static Xiph.vorbis_func_floor floor1_exportbundle;

		private static Xiph.vorbis_func_mapping mapping0_exportbundle;

		private static readonly float[] ATH;

		private static readonly float[][][] tonemasks;

		private static readonly double[] stereo_threshholds;

		private static readonly double[] stereo_threshholds_limited;

		private static Xiph.vorbis_func_floor[] __floor_P;

		private static Xiph.vorbis_func_residue[] __residue_P;

		private static Xiph.vorbis_func_mapping[] __mapping_P;

		private static Xiph.vorbis_func_residue residue0_exportbundle;

		private static Xiph.vorbis_func_residue residue1_exportbundle;

		private static Xiph.vorbis_func_residue residue2_exportbundle;

		private static readonly int[] ntryh;

		private static readonly float[] vwin64;

		private static readonly float[] vwin128;

		private static readonly float[] vwin256;

		private static readonly float[] vwin512;

		private static readonly float[] vwin1024;

		private static readonly float[] vwin2048;

		private static readonly float[] vwin4096;

		private static readonly float[] vwin8192;

		private static readonly float[][] vwin;

		private static Xiph.vorbis_func_floor[] _floor_P
		{
			get
			{
				if (Xiph.__floor_P == null)
				{
					Xiph.__floor_P = new Xiph.vorbis_func_floor[]
					{
						Xiph.floor0_exportbundle,
						Xiph.floor1_exportbundle
					};
				}
				return Xiph.__floor_P;
			}
		}

		private static Xiph.vorbis_func_residue[] _residue_P
		{
			get
			{
				if (Xiph.__residue_P == null)
				{
					Xiph.__residue_P = new Xiph.vorbis_func_residue[]
					{
						Xiph.residue0_exportbundle,
						Xiph.residue1_exportbundle,
						Xiph.residue2_exportbundle
					};
				}
				return Xiph.__residue_P;
			}
		}

		private static Xiph.vorbis_func_mapping[] _mapping_P
		{
			get
			{
				if (Xiph.__mapping_P == null)
				{
					Xiph.__mapping_P = new Xiph.vorbis_func_mapping[]
					{
						Xiph.mapping0_exportbundle
					};
				}
				return Xiph.__mapping_P;
			}
		}

		public static int ogg_page_version(Xiph.ogg_page og)
		{
			return (int)og.header[4];
		}

		public static bool ogg_page_continued(Xiph.ogg_page og)
		{
			return (og.header[5] & 1) != 0;
		}

		public static int ogg_page_bos(Xiph.ogg_page og)
		{
			return (int)(og.header[5] & 2);
		}

		public static int ogg_page_eos(Xiph.ogg_page og)
		{
			return (int)(og.header[5] & 4);
		}

		public static long ogg_page_granulepos(Xiph.ogg_page og)
		{
			Xiph.ogg_ptr header = og.header;
			byte[] data = header.data;
			int offset = header.offset;
			long num = (long)(data[offset + 13] & 255);
			num = (num << 8 | (long)((ulong)data[offset + 12] & 255uL));
			num = (num << 8 | (long)((ulong)data[offset + 11] & 255uL));
			num = (num << 8 | (long)((ulong)data[offset + 10] & 255uL));
			num = (num << 8 | (long)((ulong)data[offset + 9] & 255uL));
			num = (num << 8 | (long)((ulong)data[offset + 8] & 255uL));
			num = (num << 8 | (long)((ulong)data[offset + 7] & 255uL));
			return num << 8 | (long)((ulong)data[offset + 6] & 255uL);
		}

		public static int ogg_page_serialno(Xiph.ogg_page og)
		{
			Xiph.ogg_ptr header = og.header;
			byte[] data = header.data;
			int offset = header.offset;
			return (int)data[offset + 14] | (int)data[offset + 15] << 8 | (int)data[offset + 16] << 16 | (int)data[offset + 17] << 24;
		}

		public static int ogg_page_pageno(Xiph.ogg_page og)
		{
			Xiph.ogg_ptr header = og.header;
			byte[] data = header.data;
			int offset = header.offset;
			return (int)data[offset + 18] | (int)data[offset + 19] << 8 | (int)data[offset + 20] << 16 | (int)data[offset + 21] << 24;
		}

		public static bool ogg_stream_init(Xiph.ogg_stream_state os, int serialno)
		{
			if (os != null)
			{
				os.body_fill = 0;
				os.body_returned = 0;
				os.lacing_fill = 0;
				os.lacing_packet = 0;
				os.lacing_returned = 0;
				os.header_fill = 0;
				os.e_o_s = 0;
				os.b_o_s = 0;
				os.pageno = 0;
				os.packetno = 0L;
				os.granulepos = 0L;
				os.body_storage = 16384;
				os.lacing_storage = 1024;
				try
				{
					os.body_data = new byte[os.body_storage];
					os.lacing_vals = new int[os.lacing_storage];
					os.granule_vals = new long[os.lacing_storage];
				}
				catch (OutOfMemoryException)
				{
					Xiph.ogg_stream_clear(os);
					return false;
				}
				os.serialno = serialno;
				return true;
			}
			return false;
		}

		public static Xiph.ogg_stream_state ogg_stream_clone(Xiph.ogg_stream_state os)
		{
			Xiph.ogg_stream_state ogg_stream_state = new Xiph.ogg_stream_state();
			ogg_stream_state.body_data = os.body_data;
			ogg_stream_state.body_storage = os.body_storage;
			ogg_stream_state.body_fill = os.body_fill;
			ogg_stream_state.body_returned = os.body_returned;
			ogg_stream_state.lacing_vals = os.lacing_vals;
			ogg_stream_state.granule_vals = os.granule_vals;
			ogg_stream_state.lacing_storage = os.lacing_storage;
			ogg_stream_state.lacing_fill = os.lacing_fill;
			ogg_stream_state.lacing_packet = os.lacing_packet;
			ogg_stream_state.lacing_returned = os.lacing_returned;
			Xiph.CopyArrays(os.header, ogg_stream_state.header, os.header.Length);
			ogg_stream_state.header_fill = os.header_fill;
			ogg_stream_state.e_o_s = os.e_o_s;
			ogg_stream_state.b_o_s = os.b_o_s;
			ogg_stream_state.serialno = os.serialno;
			ogg_stream_state.pageno = os.pageno;
			ogg_stream_state.packetno = os.packetno;
			ogg_stream_state.granulepos = os.granulepos;
			return ogg_stream_state;
		}

		public static bool ogg_stream_check(Xiph.ogg_stream_state os)
		{
			return os == null || os.body_data == null;
		}

		public static int ogg_stream_clear(Xiph.ogg_stream_state os)
		{
			if (os != null)
			{
				os.body_data = null;
				os.body_storage = 0;
				os.body_fill = 0;
				os.body_returned = 0;
				os.lacing_vals = null;
				os.granule_vals = null;
				os.lacing_storage = 0;
				os.lacing_fill = 0;
				os.lacing_packet = 0;
				os.lacing_returned = 0;
				Array.Clear(os.header, 0, os.header.Length);
				os.header_fill = 0;
				os.e_o_s = 0;
				os.b_o_s = 0;
				os.serialno = 0;
				os.pageno = 0;
				os.packetno = 0L;
				os.granulepos = 0L;
			}
			return 0;
		}

		public static int ogg_stream_destroy(Xiph.ogg_stream_state os)
		{
			if (os != null)
			{
				Xiph.ogg_stream_clear(os);
			}
			return 0;
		}

		private static bool _os_body_expand(Xiph.ogg_stream_state os, int needed)
		{
			if (os.body_storage <= os.body_fill + needed)
			{
				try
				{
					byte[] array = new byte[os.body_storage + needed + 1024];
					Array.Copy(os.body_data, array, os.body_data.Length);
					os.body_storage += needed + 1024;
					os.body_data = array;
				}
				catch (OutOfMemoryException)
				{
					Xiph.ogg_stream_clear(os);
					return true;
				}
				return false;
			}
			return false;
		}

		private static bool _os_lacing_expand(Xiph.ogg_stream_state os, int needed)
		{
			if (os.lacing_storage <= os.lacing_fill + needed)
			{
				try
				{
					int[] array = new int[os.lacing_storage + needed + 32];
					Array.Copy(os.lacing_vals, array, os.lacing_vals.Length);
					os.lacing_vals = array;
				}
				catch (OutOfMemoryException)
				{
					Xiph.ogg_stream_clear(os);
					bool result = true;
					return result;
				}
				try
				{
					long[] array2 = new long[os.lacing_storage + needed + 32];
					Xiph.CopyArrays(os.granule_vals, array2, os.granule_vals.Length);
					os.granule_vals = array2;
				}
				catch (OutOfMemoryException)
				{
					Xiph.ogg_stream_clear(os);
					bool result = true;
					return result;
				}
				os.lacing_storage += needed + 32;
				return false;
			}
			return false;
		}

		public static void ogg_page_checksum_set(Xiph.ogg_page og)
		{
			if (og != null)
			{
				uint num = 0u;
				byte[] data = og.header.data;
				int offset = og.header.offset;
				byte[] data2 = og.body.data;
				int offset2 = og.body.offset;
				data[offset + 22] = 0;
				data[offset + 23] = 0;
				data[offset + 24] = 0;
				data[offset + 25] = 0;
				for (int i = 0; i < og.header_len; i++)
				{
					num = (num << 8 ^ Xiph.crc_lookup[(int)((UIntPtr)((num >> 24 & 255u) ^ (uint)data[offset + i]))]);
				}
				for (int i = 0; i < og.body_len; i++)
				{
					num = (num << 8 ^ Xiph.crc_lookup[(int)((UIntPtr)((num >> 24 & 255u) ^ (uint)data2[offset2 + i]))]);
				}
				data[offset + 22] = (byte)(num & 255u);
				data[offset + 23] = (byte)(num >> 8 & 255u);
				data[offset + 24] = (byte)(num >> 16 & 255u);
				data[offset + 25] = (byte)(num >> 24 & 255u);
			}
		}

		public static void ogg_sync_init(Xiph.ogg_sync_state oy)
		{
			oy.data = null;
			oy.storage = 0;
			oy.fill = 0;
			oy.returned = 0;
			oy.unsynced = 0;
			oy.headerbytes = 0;
			oy.bodybytes = 0;
		}

		public static void ogg_sync_clear(Xiph.ogg_sync_state oy)
		{
			oy.data = null;
			oy.storage = 0;
			oy.fill = 0;
			oy.returned = 0;
			oy.unsynced = 0;
			oy.headerbytes = 0;
			oy.bodybytes = 0;
		}

		public static void ogg_sync_destroy(Xiph.ogg_sync_state oy)
		{
			Xiph.ogg_sync_clear(oy);
		}

		public static bool ogg_sync_check(Xiph.ogg_sync_state oy)
		{
			return oy.storage < 0;
		}

		public static bool ogg_sync_buffer(Xiph.ogg_sync_state oy, int size, out byte[] data, out int offset)
		{
			data = null;
			offset = 0;
			if (Xiph.ogg_sync_check(oy))
			{
				return false;
			}
			if (oy.returned > 0)
			{
				oy.fill -= oy.returned;
				if (oy.fill > 0)
				{
					Xiph.CopyArrays(oy.data, oy.returned, oy.data, 0, oy.fill);
				}
				oy.returned = 0;
			}
			if (size > oy.storage - oy.fill)
			{
				int num = size + oy.fill + 4096;
				try
				{
					byte[] array = new byte[num];
					if (oy.data != null)
					{
						for (int i = 0; i < oy.data.Length; i++)
						{
							array[i] = oy.data[i];
						}
					}
					oy.data = array;
					oy.storage = num;
					data = oy.data;
					offset = oy.fill;
				}
				catch (OutOfMemoryException)
				{
					Xiph.ogg_sync_clear(oy);
					return false;
				}
				return true;
			}
			data = oy.data;
			offset = oy.fill;
			return true;
		}

		public static int ogg_sync_wrote(Xiph.ogg_sync_state oy, int bytes)
		{
			if (Xiph.ogg_sync_check(oy))
			{
				return -1;
			}
			if (oy.fill + bytes > oy.storage)
			{
				return -1;
			}
			oy.fill += bytes;
			return 0;
		}

		public static int ogg_sync_pageseek(Xiph.ogg_sync_state oy, Xiph.ogg_page og)
		{
			Xiph.ogg_ptr ogg_ptr = new Xiph.ogg_ptr(oy.data, oy.returned);
			int num = oy.fill - oy.returned;
			if (Xiph.ogg_sync_check(oy))
			{
				return 0;
			}
			if (oy.headerbytes == 0)
			{
				if (num < 27)
				{
					return 0;
				}
				if (ogg_ptr.data[0] != 79 || ogg_ptr.data[1] != 103 || ogg_ptr.data[2] != 103 || ogg_ptr.data[3] != 83)
				{
					goto IL_1CD;
				}
				int num2 = (int)(ogg_ptr[26] + 27);
				if (num < num2)
				{
					return 0;
				}
				for (int i = 0; i < (int)ogg_ptr[26]; i++)
				{
					oy.bodybytes += (int)ogg_ptr[27 + i];
				}
				oy.headerbytes = num2;
			}
			if (oy.bodybytes + oy.headerbytes > num)
			{
				return 0;
			}
			Xiph.ogg_page ogg_page = new Xiph.ogg_page();
			uint num3 = ogg_ptr.read(22);
			ogg_ptr.write(22, 0u);
			ogg_page.header = ogg_ptr;
			ogg_page.header_len = oy.headerbytes;
			ogg_page.body = ogg_ptr + oy.headerbytes;
			ogg_page.body_len = oy.bodybytes;
			Xiph.ogg_page_checksum_set(ogg_page);
			if (num3 == ogg_ptr.read(22))
			{
				Xiph.ogg_ptr ogg_ptr2 = new Xiph.ogg_ptr(oy.data, oy.returned);
				if (og != null)
				{
					og.header = ogg_ptr2;
					og.header_len = oy.headerbytes;
					og.body = ogg_ptr2 + oy.headerbytes;
					og.body_len = oy.bodybytes;
				}
				oy.unsynced = 0;
				int result;
				oy.returned += (result = oy.headerbytes + oy.bodybytes);
				oy.headerbytes = 0;
				oy.bodybytes = 0;
				return result;
			}
			ogg_ptr.write(22, num3);
			IL_1CD:
			oy.headerbytes = 0;
			oy.bodybytes = 0;
			Xiph.ogg_ptr? ogg_ptr3 = ogg_ptr.memchr(1, 79, num - 1);
			if (!ogg_ptr3.HasValue)
			{
				ogg_ptr3 = new Xiph.ogg_ptr?(new Xiph.ogg_ptr(oy.data, oy.fill));
			}
			oy.returned = ogg_ptr3.Value.offset;
			return -(ogg_ptr3.Value.offset - ogg_ptr.offset);
		}

		public static int ogg_sync_pageout(Xiph.ogg_sync_state oy, Xiph.ogg_page og)
		{
			if (Xiph.ogg_sync_check(oy))
			{
				return 0;
			}
			while (true)
			{
				long num = (long)Xiph.ogg_sync_pageseek(oy, og);
				if (num > 0L)
				{
					break;
				}
				if (num == 0L)
				{
					return 0;
				}
				if (oy.unsynced == 0)
				{
					goto Block_4;
				}
			}
			return 1;
			Block_4:
			oy.unsynced = 1;
			return -1;
		}

		public static int ogg_stream_pagein(Xiph.ogg_stream_state os, Xiph.ogg_page og)
		{
			Xiph.ogg_ptr header = og.header;
			Xiph.ogg_ptr ptr = og.body;
			int num = og.body_len;
			int i = 0;
			int num2 = Xiph.ogg_page_version(og);
			bool flag = Xiph.ogg_page_continued(og);
			int num3 = Xiph.ogg_page_bos(og);
			int num4 = Xiph.ogg_page_eos(og);
			long num5 = Xiph.ogg_page_granulepos(og);
			int num6 = Xiph.ogg_page_serialno(og);
			int num7 = Xiph.ogg_page_pageno(og);
			int num8 = (int)header[26];
			if (Xiph.ogg_stream_check(os))
			{
				return -1;
			}
			int lacing_returned = os.lacing_returned;
			int body_returned = os.body_returned;
			if (body_returned != 0)
			{
				os.body_fill -= body_returned;
				if (os.body_fill != 0)
				{
					Xiph.CopyArrays(os.body_data, body_returned, os.body_data, 0, os.body_fill);
				}
				os.body_returned = 0;
			}
			if (lacing_returned != 0)
			{
				if (os.lacing_fill - lacing_returned != 0)
				{
					Xiph.CopyArrays(os.lacing_vals, lacing_returned, os.lacing_vals, 0, os.lacing_fill - lacing_returned);
					Xiph.CopyArrays(os.granule_vals, lacing_returned, os.granule_vals, 0, os.lacing_fill - lacing_returned);
				}
				os.lacing_fill -= lacing_returned;
				os.lacing_packet -= lacing_returned;
				os.lacing_returned = 0;
			}
			if (num6 != os.serialno)
			{
				return -1;
			}
			if (num2 > 0)
			{
				return -1;
			}
			if (Xiph._os_lacing_expand(os, num8 + 1))
			{
				return -1;
			}
			if (num7 != os.pageno)
			{
				for (int j = os.lacing_packet; j < os.lacing_fill; j++)
				{
					os.body_fill -= (os.lacing_vals[j] & 255);
				}
				os.lacing_fill = os.lacing_packet;
				if (os.pageno != -1)
				{
					os.lacing_vals[os.lacing_fill++] = 1024;
					os.lacing_packet++;
				}
			}
			if (flag && (os.lacing_fill < 1 || os.lacing_vals[os.lacing_fill - 1] == 1024))
			{
				num3 = 0;
				while (i < num8)
				{
					int num9 = (int)header[27 + i];
					ptr += num9;
					num -= num9;
					if (num9 < 255)
					{
						i++;
						break;
					}
					i++;
				}
			}
			if (num != 0)
			{
				if (Xiph._os_body_expand(os, num))
				{
					return -1;
				}
				Xiph.CopyArrays(ptr.data, ptr.offset, os.body_data, os.body_fill, num);
				os.body_fill += num;
			}
			int num10 = -1;
			while (i < num8)
			{
				int num11 = (int)header[27 + i];
				os.lacing_vals[os.lacing_fill] = num11;
				os.granule_vals[os.lacing_fill] = -1L;
				if (num3 != 0)
				{
					os.lacing_vals[os.lacing_fill] |= 256;
					num3 = 0;
				}
				if (num11 < 255)
				{
					num10 = os.lacing_fill;
				}
				os.lacing_fill++;
				i++;
				if (num11 < 255)
				{
					os.lacing_packet = os.lacing_fill;
				}
			}
			if (num10 != -1)
			{
				os.granule_vals[num10] = num5;
			}
			if (num4 != 0)
			{
				os.e_o_s = 1;
				if (os.lacing_fill > 0)
				{
					os.lacing_vals[os.lacing_fill - 1] |= 512;
				}
			}
			os.pageno = num7 + 1;
			return 0;
		}

		private static int _packetout(Xiph.ogg_stream_state os, Xiph.ogg_packet op, int adv)
		{
			int num = os.lacing_returned;
			if (os.lacing_packet <= num)
			{
				return 0;
			}
			if ((os.lacing_vals[num] & 1024) != 0)
			{
				os.lacing_returned++;
				os.packetno += 1L;
				return -1;
			}
			if (op == null && adv == 0)
			{
				return 1;
			}
			int num2 = os.lacing_vals[num] & 255;
			int num3 = num2;
			int e_o_s = os.lacing_vals[num] & 512;
			int b_o_s = os.lacing_vals[num] & 256;
			while (num2 == 255)
			{
				int num4 = os.lacing_vals[++num];
				num2 = (num4 & 255);
				if ((num4 & 512) != 0)
				{
					e_o_s = 512;
				}
				num3 += num2;
			}
			if (op != null)
			{
				op.e_o_s = e_o_s;
				op.b_o_s = b_o_s;
				op.packet = new Xiph.ogg_ptr(os.body_data, os.body_returned);
				op.packetno = os.packetno;
				op.granulepos = os.granule_vals[num];
				op.bytes = num3;
			}
			if (adv != 0)
			{
				os.body_returned += num3;
				os.lacing_returned = num + 1;
				os.packetno += 1L;
			}
			return 1;
		}

		public static int ogg_stream_packetout(Xiph.ogg_stream_state os, Xiph.ogg_packet op)
		{
			if (Xiph.ogg_stream_check(os))
			{
				return 0;
			}
			return Xiph._packetout(os, op, 1);
		}

		public static int ogg_stream_packetpeek(Xiph.ogg_stream_state os, Xiph.ogg_packet op)
		{
			if (Xiph.ogg_stream_check(os))
			{
				return 0;
			}
			return Xiph._packetout(os, op, 0);
		}

		public static void ogg_packet_clear(Xiph.ogg_packet op)
		{
			op.packet.data = null;
			op.packet.offset = 0;
			op.bytes = 0;
			op.b_o_s = 0;
			op.e_o_s = 0;
			op.granulepos = 0L;
			op.packetno = 0L;
		}

		public static void oggpack_writeinit(Xiph.oggpack_buffer b)
		{
			b.ptr = (b.buffer = new Xiph.ogg_ptr(new byte[256], 0));
			b.buffer[0] = 0;
			b.storage = 256L;
		}

		public static void oggpack_write(Xiph.oggpack_buffer b, long value, int bits)
		{
			Xiph.oggpack_write(b, (ulong)value, bits);
		}

		public static void oggpack_write(Xiph.oggpack_buffer b, ulong value, int bits)
		{
			if (bits >= 0 && bits <= 32)
			{
				if (b.endbyte >= b.storage - 4L)
				{
					if (b.ptr.data == null)
					{
						return;
					}
					if (b.storage > 2147483391L)
					{
						goto IL_1A9;
					}
					byte[] array = new byte[b.storage + 256L];
					Array.Copy(b.buffer.data, array, b.buffer.data.Length);
					b.buffer.data = array;
					b.storage += 256L;
					b.ptr = b.buffer + b.endbyte;
				}
				value &= Xiph.mask[bits];
				bits += b.endbit;
				b.ptr[0] = (b.ptr[0] | (byte)(value << b.endbit));
				if (bits >= 8)
				{
					b.ptr[1] = (byte)(value >> 8 - b.endbit);
					if (bits >= 16)
					{
						b.ptr[2] = (byte)(value >> 16 - b.endbit);
						if (bits >= 24)
						{
							b.ptr[3] = (byte)(value >> 24 - b.endbit);
							if (bits >= 32)
							{
								if (b.endbit != 0)
								{
									b.ptr[4] = (byte)(value >> 32 - b.endbit);
								}
								else
								{
									b.ptr[4] = 0;
								}
							}
						}
					}
				}
				b.endbyte += (long)(bits / 8);
				b.ptr += bits / 8;
				b.endbit = (bits & 7);
				return;
			}
			IL_1A9:
			Xiph.oggpack_writeclear(b);
		}

		public static void oggpack_writeclear(Xiph.oggpack_buffer b)
		{
			b.endbit = 0;
			b.endbyte = 0L;
			b.storage = 0L;
		}

		public static void oggpack_readinit(Xiph.oggpack_buffer b, Xiph.ogg_ptr buf, int bytes)
		{
			b.endbit = 0;
			b.endbyte = 0L;
			b.ptr = buf;
			b.buffer = buf;
			b.storage = (long)bytes;
		}

		public static long oggpack_look(Xiph.oggpack_buffer b, int bits)
		{
			if (bits < 0 || bits > 32)
			{
				return -1L;
			}
			ulong num = Xiph.mask[bits];
			bits += b.endbit;
			if (b.endbyte >= b.storage - 4L)
			{
				if (b.endbyte > b.storage - (long)(bits + 7 >> 3))
				{
					return -1L;
				}
				if (bits == 0)
				{
					return 0L;
				}
			}
			ulong num2 = (ulong)((long)(b.ptr[0] >> b.endbit));
			if (bits > 8)
			{
				num2 |= (ulong)((long)((long)b.ptr[1] << 8 - b.endbit));
				if (bits > 16)
				{
					num2 |= (ulong)((long)((long)b.ptr[2] << 16 - b.endbit));
					if (bits > 24)
					{
						num2 |= (ulong)((long)((long)b.ptr[3] << 24 - b.endbit));
						if (bits > 32 && b.endbit != 0)
						{
							num2 |= (ulong)((long)((long)b.ptr[4] << 32 - b.endbit));
						}
					}
				}
			}
			return (long)(num & num2);
		}

		public static void oggpack_adv(Xiph.oggpack_buffer b, int bits)
		{
			bits += b.endbit;
			if (b.endbyte <= b.storage - (long)(bits + 7 >> 3))
			{
				b.ptr += bits / 8;
				b.endbyte += (long)(bits / 8);
				b.endbit = (bits & 7);
				return;
			}
			b.ptr.data = null;
			b.ptr.offset = 0;
			b.endbyte = b.storage;
			b.endbit = 1;
		}

		public static long oggpack_read(Xiph.oggpack_buffer b, int bits)
		{
			if (bits >= 0 && bits <= 32)
			{
				ulong num = Xiph.mask[bits];
				bits += b.endbit;
				if (b.endbyte >= b.storage - 4L)
				{
					if (b.endbyte > b.storage - (long)(bits + 7 >> 3))
					{
						goto IL_12D;
					}
					if (bits == 0)
					{
						return 0L;
					}
				}
				long num2 = (long)(b.ptr[0] >> b.endbit);
				if (bits > 8)
				{
					num2 |= (long)((long)b.ptr[1] << 8 - b.endbit);
					if (bits > 16)
					{
						num2 |= (long)((long)b.ptr[2] << 16 - b.endbit);
						if (bits > 24)
						{
							num2 |= (long)((long)b.ptr[3] << 24 - b.endbit);
							if (bits > 32 && b.endbit != 0)
							{
								num2 |= (long)((long)b.ptr[4] << 32 - b.endbit);
							}
						}
					}
				}
				num2 &= (long)num;
				b.ptr += bits / 8;
				b.endbyte += (long)(bits / 8);
				b.endbit = (bits & 7);
				return num2;
			}
			IL_12D:
			b.ptr.data = null;
			b.endbyte = b.storage;
			b.endbit = 1;
			return -1L;
		}

		public static long oggpack_bytes(Xiph.oggpack_buffer b)
		{
			return b.endbyte + (long)((b.endbit + 7) / 8);
		}

		private static bool ArraysEqual<T>(T[] a1, T[] a2)
		{
			if (object.ReferenceEquals(a1, a2))
			{
				return true;
			}
			if (a1 == null || a2 == null)
			{
				return false;
			}
			if (a1.Length != a2.Length)
			{
				return false;
			}
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < a1.Length; i++)
			{
				if (!@default.Equals(a1[i], a2[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static void Copy3x64(long[][] src, long[][] dst)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 64; j++)
				{
					dst[i][j] = src[i][j];
				}
			}
		}

		private static void Clear3x3(int[][] arr)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					arr[i][j] = 0;
				}
			}
		}

		public static void CopyArrays(long[] src, long[] dst, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
		}

		public static void CopyArrays(short[] src, short[] dst, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
		}

		public static void CopyArrays(int[] src, int[] dst, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
		}

		public static void CopyArrays(uint[] src, uint[] dst, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
		}

		public static void CopyArrays(byte[] src, byte[] dst, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[i] = src[i];
			}
		}

		public static void CopyArrays(byte[] src, int src_offset, byte[] dst, int dst_offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[dst_offset + i] = src[src_offset + i];
			}
		}

		public static void CopyArrays(int[] src, int src_offset, int[] dst, int dst_offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[dst_offset + i] = src[src_offset + i];
			}
		}

		public static void CopyArrays(long[] src, int src_offset, long[] dst, int dst_offset, int count)
		{
			for (int i = 0; i < count; i++)
			{
				dst[dst_offset + i] = src[src_offset + i];
			}
		}

		private static int ilog(uint v)
		{
			int num = 0;
			while (v != 0u)
			{
				num++;
				v >>= 1;
			}
			return num;
		}

		private static int ilog2(uint v)
		{
			int num = 0;
			if (v != 0u)
			{
				v -= 1u;
			}
			while (v != 0u)
			{
				num++;
				v >>= 1;
			}
			return num;
		}

		private static int rint(float x)
		{
			return (int)Math.Floor((double)(x + 0.5f));
		}

		private static int rint(double x)
		{
			return (int)Math.Floor(x + 0.5);
		}

		private static float log(float x)
		{
			return (float)Math.Log((double)x);
		}

		private static float exp(float x)
		{
			return (float)Math.Exp((double)x);
		}

		private static float ceil(float x)
		{
			return (float)Math.Ceiling((double)x);
		}

		public static float floor(float x)
		{
			return (float)Math.Floor((double)x);
		}

		private static float atan(float x)
		{
			return (float)Math.Atan((double)x);
		}

		private static float cos(float x)
		{
			return (float)Math.Cos((double)x);
		}

		private static float sin(float x)
		{
			return (float)Math.Sin((double)x);
		}

		private static float pow(float x, float y)
		{
			return (float)Math.Pow((double)x, (double)y);
		}

		private static int abs(int x)
		{
			return Math.Abs(x);
		}

		private static float fabs(float x)
		{
			return Math.Abs(x);
		}

		private static float sqrt(float x)
		{
			return (float)Math.Sqrt((double)x);
		}

		private static int max(int x, int y)
		{
			return Math.Max(x, y);
		}

		private static int min(int x, int y)
		{
			return Math.Min(x, y);
		}

		private static float max(float x, float y)
		{
			return Math.Max(x, y);
		}

		private static float min(float x, float y)
		{
			return Math.Min(x, y);
		}

		private static int oc_pack_read(Xiph.oc_pack_buf b, int bits)
		{
			return Xiph.oc_pack_read_c(b, bits);
		}

		private static int oc_pack_read1(Xiph.oc_pack_buf b)
		{
			return Xiph.oc_pack_read1_c(b);
		}

		private static void oc_pack_readinit(Xiph.oc_pack_buf b, Xiph.ogg_ptr buf, int bytes)
		{
			b.window = 0u;
			b.bits = 0;
			b.eof = 0;
			b.ptr = buf;
			b.stop = buf + bytes;
		}

		private static uint oc_pack_refill(Xiph.oc_pack_buf b, int bits)
		{
			Xiph.ogg_ptr stop = b.stop;
			Xiph.ogg_ptr ptr = b.ptr;
			uint num = b.window;
			int num2 = b.bits;
			int num3 = 32 - num2;
			while (7 < num3 && ptr < stop)
			{
				num3 -= 8;
				num |= (uint)((uint)ptr.data[ptr.offset] << num3);
				ptr.offset++;
			}
			b.ptr = ptr;
			num2 = 32 - num3;
			if (bits > num2)
			{
				if (ptr >= stop)
				{
					b.eof = 1;
					num2 = 1073741824;
				}
				else
				{
					num |= (uint)(ptr[0] >> (num2 & 7));
				}
			}
			b.bits = num2;
			return num;
		}

		private static int oc_pack_read_c(Xiph.oc_pack_buf b, int bits)
		{
			uint num = b.window;
			int num2 = b.bits;
			if (bits == 0)
			{
				return 0;
			}
			if (num2 < bits)
			{
				num = Xiph.oc_pack_refill(b, bits);
				num2 = b.bits;
			}
			int result = (int)(num >> 32 - bits);
			num2 -= bits;
			num <<= 1;
			num <<= bits - 1;
			b.window = num;
			b.bits = num2;
			return result;
		}

		private static int oc_pack_read1_c(Xiph.oc_pack_buf b)
		{
			uint num = b.window;
			int num2 = b.bits;
			if (num2 < 1)
			{
				num = Xiph.oc_pack_refill(b, 1);
				num2 = b.bits;
			}
			int result = (int)(num >> 31);
			num2--;
			num <<= 1;
			b.window = num;
			b.bits = num2;
			return result;
		}

		private static int oc_pack_bytes_left(Xiph.oc_pack_buf b)
		{
			if (b.eof != 0)
			{
				return -1;
			}
			return b.stop - b.ptr + (b.bits >> 3);
		}

		private static void oc_unpack_octets(Xiph.oc_pack_buf opb, byte[] buf, int len)
		{
			int num = 0;
			while (len-- > 0)
			{
				buf[num++] = (byte)Xiph.oc_pack_read(opb, 8);
			}
		}

		private static int oc_unpack_length(Xiph.oc_pack_buf opb)
		{
			int[] array = new int[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = Xiph.oc_pack_read(opb, 8);
			}
			return array[0] | array[1] << 8 | array[2] << 16 | array[3] << 24;
		}

		private static int oc_info_unpack(Xiph.oc_pack_buf opb, ref Xiph.th_info info)
		{
			int num = Xiph.oc_pack_read(opb, 8);
			info.version_major = (byte)num;
			num = Xiph.oc_pack_read(opb, 8);
			info.version_minor = (byte)num;
			num = Xiph.oc_pack_read(opb, 8);
			info.version_subminor = (byte)num;
			if (info.version_major > 3 || (info.version_major == 3 && info.version_minor > 2))
			{
				return -22;
			}
			num = Xiph.oc_pack_read(opb, 16);
			info.frame_width = (uint)((uint)num << 4);
			num = Xiph.oc_pack_read(opb, 16);
			info.frame_height = (uint)((uint)num << 4);
			num = Xiph.oc_pack_read(opb, 24);
			info.pic_width = (uint)num;
			num = Xiph.oc_pack_read(opb, 24);
			info.pic_height = (uint)num;
			num = Xiph.oc_pack_read(opb, 8);
			info.pic_x = (uint)num;
			num = Xiph.oc_pack_read(opb, 8);
			info.pic_y = (uint)num;
			num = Xiph.oc_pack_read(opb, 32);
			info.fps_numerator = (uint)num;
			num = Xiph.oc_pack_read(opb, 32);
			info.fps_denominator = (uint)num;
			if (info.frame_width == 0u || info.frame_height == 0u || info.pic_width + info.pic_x > info.frame_width || info.pic_height + info.pic_y > info.frame_height || info.fps_numerator == 0u || info.fps_denominator == 0u)
			{
				return -20;
			}
			info.pic_y = info.frame_height - info.pic_height - info.pic_y;
			num = Xiph.oc_pack_read(opb, 24);
			info.aspect_numerator = (uint)num;
			num = Xiph.oc_pack_read(opb, 24);
			info.aspect_denominator = (uint)num;
			num = Xiph.oc_pack_read(opb, 8);
			info.colorspace = (Xiph.th_colorspace)num;
			num = Xiph.oc_pack_read(opb, 24);
			info.target_bitrate = num;
			num = Xiph.oc_pack_read(opb, 6);
			info.quality = num;
			num = Xiph.oc_pack_read(opb, 5);
			info.keyframe_granule_shift = num;
			num = Xiph.oc_pack_read(opb, 2);
			info.pixel_fmt = (Xiph.th_pixel_fmt)num;
			if (info.pixel_fmt == Xiph.th_pixel_fmt.TH_PF_RSVD)
			{
				return -20;
			}
			if (Xiph.oc_pack_read(opb, 3) != 0 || Xiph.oc_pack_bytes_left(opb) < 0)
			{
				return -20;
			}
			return 0;
		}

		private static int oc_comment_unpack(Xiph.oc_pack_buf opb, Xiph.th_comment tc)
		{
			int num = Xiph.oc_unpack_length(opb);
			if (num < 0 || num > Xiph.oc_pack_bytes_left(opb))
			{
				return -20;
			}
			byte[] array = new byte[num + 1];
			Xiph.oc_unpack_octets(opb, array, num);
			tc.vendor = Xiph.Encoding.ASCII.GetString(array, 0, num);
			int num2 = Xiph.oc_unpack_length(opb);
			num = num2;
			if (num < 0 || num > 536870911 || num << 2 > Xiph.oc_pack_bytes_left(opb))
			{
				return -20;
			}
			tc.user_comments = new string[num2];
			for (int i = 0; i < num2; i++)
			{
				num = Xiph.oc_unpack_length(opb);
				if (num < 0 || num > Xiph.oc_pack_bytes_left(opb))
				{
					return -20;
				}
				byte[] array2 = new byte[num];
				Xiph.oc_unpack_octets(opb, array2, num);
				tc.user_comments[i] = Xiph.Encoding.ASCII.GetString(array2);
			}
			if (Xiph.oc_pack_bytes_left(opb) >= 0)
			{
				return 0;
			}
			return -20;
		}

		private static int oc_setup_unpack(Xiph.oc_pack_buf opb, Xiph.th_setup_info setup)
		{
			int num = Xiph.oc_quant_params_unpack(opb, setup.qinfo);
			if (num < 0)
			{
				return num;
			}
			return Xiph.oc_huff_trees_unpack(opb, ref setup.huff_tables);
		}

		private static void oc_setup_clear(Xiph.th_setup_info setup)
		{
			Xiph.oc_quant_params_clear(setup.qinfo);
			Xiph.oc_huff_trees_clear(setup.huff_tables);
		}

		private static int oc_dec_headerin(Xiph.oc_pack_buf opb, ref Xiph.th_info info, Xiph.th_comment tc, ref Xiph.th_setup_info setup, Xiph.ogg_packet op)
		{
			byte[] array = new byte[6];
			int num = Xiph.oc_pack_read(opb, 8);
			int num2 = num;
			if ((num2 & 128) == 0 && info.frame_width > 0u && tc.vendor != null && setup != null)
			{
				return 0;
			}
			Xiph.oc_unpack_octets(opb, array, 6);
			if (array[0] != 116 | array[1] != 104 | array[2] != 101 | array[3] != 111 | array[4] != 114 | array[5] != 97)
			{
				return -21;
			}
			int num3;
			switch (num2)
			{
			case 128:
				if (op.b_o_s == 0 || info.frame_width > 0u)
				{
					return -20;
				}
				num3 = Xiph.oc_info_unpack(opb, ref info);
				if (num3 < 0)
				{
					Xiph.th_info_clear(ref info);
				}
				else
				{
					num3 = 3;
				}
				break;
			case 129:
				if (tc == null)
				{
					return -1;
				}
				if (info.frame_width == 0u || tc.vendor != null)
				{
					return -20;
				}
				num3 = Xiph.oc_comment_unpack(opb, tc);
				if (num3 < 0)
				{
					Xiph.th_comment_clear(tc);
				}
				else
				{
					num3 = 2;
				}
				break;
			case 130:
			{
				if (tc == null)
				{
					return -1;
				}
				if (info.frame_width == 0u || tc.vendor == null || setup != null)
				{
					return -20;
				}
				Xiph.th_setup_info th_setup_info = new Xiph.th_setup_info();
				num3 = Xiph.oc_setup_unpack(opb, th_setup_info);
				if (num3 < 0)
				{
					Xiph.oc_setup_clear(th_setup_info);
				}
				else
				{
					setup = th_setup_info;
					num3 = 1;
				}
				break;
			}
			default:
				return -20;
			}
			return num3;
		}

		public static int th_decode_headerin(ref Xiph.th_info info, Xiph.th_comment tc, ref Xiph.th_setup_info setup, Xiph.ogg_packet op)
		{
			Xiph.oc_pack_buf oc_pack_buf = new Xiph.oc_pack_buf();
			if (op == null)
			{
				return -20;
			}
			Xiph.oc_pack_readinit(oc_pack_buf, op.packet, op.bytes);
			return Xiph.oc_dec_headerin(oc_pack_buf, ref info, tc, ref setup, op);
		}

		public static void th_setup_free(Xiph.th_setup_info setup)
		{
			if (setup != null)
			{
				Xiph.oc_setup_clear(setup);
			}
		}

		private static bool OC_DCT_TOKEN_NEEDS_MORE(int token)
		{
			return token < 15;
		}

		private static int OC_DCT_CW_PACK(int eobs, int rlen, int mag, int flip)
		{
			return eobs << 8 | rlen | flip << 20 | mag - flip << 21;
		}

		private static int OC_DCT_TOKEN_EB_POS(int token)
		{
			return (-13 & ((token < 2) ? -1 : 0)) + (21 & ((token < 12) ? -1 : 0));
		}

		private static int oc_sb_run_unpack(Xiph.oc_pack_buf opb)
		{
			int num = Xiph.oc_huff_token_decode(opb, Xiph.OC_SB_RUN_TREE);
			if (num >= 16)
			{
				int num2 = num & 31;
				num = 6 + num2 + Xiph.oc_pack_read(opb, num - num2 >> 4);
			}
			return num;
		}

		private static int oc_block_run_unpack(Xiph.oc_pack_buf opb)
		{
			return Xiph.oc_huff_token_decode(opb, Xiph.OC_BLOCK_RUN_TREE);
		}

		private static void oc_dec_accel_init(Xiph.th_dec_ctx dec)
		{
		}

		private static int oc_dec_init(Xiph.th_dec_ctx dec, Xiph.th_info info, Xiph.th_setup_info setup)
		{
			int num = Xiph.oc_state_init(dec.state, ref info, 3);
			if (num < 0)
			{
				return num;
			}
			num = Xiph.oc_huff_trees_copy(dec.huff_tables, setup.huff_tables);
			if (num < 0)
			{
				Xiph.oc_state_clear(dec.state);
				return num;
			}
			dec.dct_tokens = new byte[129L * dec.state.nfrags];
			if (dec.dct_tokens == null)
			{
				Xiph.oc_huff_trees_clear(dec.huff_tables);
				Xiph.oc_state_clear(dec.state);
				return -1;
			}
			for (int i = 0; i < 64; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						dec.state.dequant_tables[i][j][k] = dec.state.dequant_table_data[i][j][k];
					}
				}
			}
			Xiph.oc_dequant_tables_init(dec.state.dequant_tables, dec.pp_dc_scale, setup.qinfo);
			for (int i = 0; i < 64; i++)
			{
				int num2 = 0;
				for (int k = 0; k < 2; k++)
				{
					for (int j = 0; j < 3; j++)
					{
						int num3 = k * 3 * 2 + j * 2 + i;
						int num4 = num3 / 6;
						int num5 = (num3 - num4 * 3 * 2) / 2;
						int num6 = num3 - num4 * 3 * 2 - num5 * 2;
						num2 += (int)(dec.state.dequant_tables[num4][num5][num6][12] + dec.state.dequant_tables[num4][num5][num6][17] + dec.state.dequant_tables[num4][num5][num6][18] + dec.state.dequant_tables[num4][num5][num6][24]) << ((j == 0) ? 1 : 0);
					}
				}
				dec.pp_sharp_mod[i] = -(num2 >> 11);
			}
			Xiph.CopyArrays(setup.qinfo.loop_filter_limits, dec.state.loop_filter_limits, setup.qinfo.loop_filter_limits.Length);
			Xiph.oc_dec_accel_init(dec);
			dec.pp_level = 0;
			dec.dc_qis = null;
			dec.variances = null;
			dec.pp_frame_data = null;
			dec.stripe_cb.ctx = null;
			dec.stripe_cb.stripe_decoded = null;
			return 0;
		}

		private static void oc_dec_clear(Xiph.th_dec_ctx dec)
		{
			dec.pp_frame_data = null;
			dec.variances = null;
			dec.dc_qis = null;
			dec.dct_tokens = null;
			Xiph.oc_huff_trees_clear(dec.huff_tables);
			Xiph.oc_state_clear(dec.state);
		}

		private static int oc_dec_frame_header_unpack(Xiph.th_dec_ctx dec)
		{
			long num = (long)Xiph.oc_pack_read1(dec.opb);
			if (num != 0L)
			{
				return -24;
			}
			num = (long)Xiph.oc_pack_read1(dec.opb);
			dec.state.frame_type = (sbyte)num;
			num = (long)Xiph.oc_pack_read(dec.opb, 6);
			dec.state.qis[0] = (byte)num;
			num = (long)Xiph.oc_pack_read1(dec.opb);
			if (num == 0L)
			{
				dec.state.nqis = 1;
			}
			else
			{
				num = (long)Xiph.oc_pack_read(dec.opb, 6);
				dec.state.qis[1] = (byte)num;
				num = (long)Xiph.oc_pack_read1(dec.opb);
				if (num == 0L)
				{
					dec.state.nqis = 2;
				}
				else
				{
					num = (long)Xiph.oc_pack_read(dec.opb, 6);
					dec.state.qis[2] = (byte)num;
					dec.state.nqis = 3;
				}
			}
			if (dec.state.frame_type == 0)
			{
				num = (long)Xiph.oc_pack_read(dec.opb, 3);
				if (num != 0L)
				{
					return -23;
				}
			}
			return 0;
		}

		private static void oc_dec_mark_all_intra(Xiph.th_dec_ctx dec)
		{
			long[] coded_fragis = dec.state.coded_fragis;
			long num2;
			long num = num2 = 0L;
			long[][][] sb_maps = dec.state.sb_maps;
			Xiph.oc_sb_flags[] sb_flags = dec.state.sb_flags;
			Xiph.oc_fragment[] frags = dec.state.frags;
			uint num4;
			uint num3 = num4 = 0u;
			for (int i = 0; i < 3; i++)
			{
				num3 += dec.state.fplanes[i].nsbs;
				while (num4 < num3)
				{
					for (int j = 0; j < 4; j++)
					{
						if (((int)((byte)((sb_flags[(int)((UIntPtr)num4)].bits & 60) >> 2)) & 1 << j) != 0)
						{
							for (int k = 0; k < 4; k++)
							{
								long num5 = sb_maps[(int)((UIntPtr)num4)][j][k];
								checked
								{
									if (num5 >= 0L)
									{
										frags[(int)((IntPtr)num5)].coded = 1u;
										frags[(int)((IntPtr)num5)].mb_mode = 1u;
										long[] arg_CB_0 = coded_fragis;
										long expr_C2 = num;
										num = unchecked(expr_C2 + 1L);
										arg_CB_0[(int)((IntPtr)expr_C2)] = num5;
									}
								}
							}
						}
					}
					num4 += 1u;
				}
				dec.state.ncoded_fragis[i] = num - num2;
				num2 = num;
			}
			dec.state.ntotal_coded_fragis = num;
		}

		private static uint oc_dec_partial_sb_flags_unpack(Xiph.th_dec_ctx dec)
		{
			long num = (long)Xiph.oc_pack_read1(dec.opb);
			int num2 = (int)num;
			Xiph.oc_sb_flags[] sb_flags = dec.state.sb_flags;
			uint nsbs = dec.state.nsbs;
			uint num4;
			uint num3 = num4 = 0u;
			while (num4 < nsbs)
			{
				uint num5 = (uint)Xiph.oc_sb_run_unpack(dec.opb);
				int num6 = (num5 >= 4129u) ? 1 : 0;
				do
				{
					sb_flags[(int)((UIntPtr)num4)].coded_partially = (byte)num2;
					sb_flags[(int)((UIntPtr)num4)].coded_fully = 0;
					num3 += (uint)num2;
					num4 += 1u;
				}
				while ((num5 -= 1u) > 0u && num4 < nsbs);
				if (num6 != 0 && num4 < nsbs)
				{
					num = (long)Xiph.oc_pack_read1(dec.opb);
					num2 = (int)num;
				}
				else
				{
					num2 = ((num2 == 0) ? 1 : 0);
				}
			}
			return num3;
		}

		private static void oc_dec_coded_sb_flags_unpack(Xiph.th_dec_ctx dec)
		{
			Xiph.oc_sb_flags[] sb_flags = dec.state.sb_flags;
			uint nsbs = dec.state.nsbs;
			uint num = 0u;
			while (sb_flags[(int)((UIntPtr)num)].coded_partially > 0)
			{
				num += 1u;
			}
			long num2 = (long)Xiph.oc_pack_read1(dec.opb);
			int num3 = (int)num2;
			do
			{
				uint num4 = (uint)Xiph.oc_sb_run_unpack(dec.opb);
				int num5 = (num4 >= 4129u) ? 1 : 0;
				while (num < nsbs)
				{
					if (sb_flags[(int)((UIntPtr)num)].coded_partially == 0)
					{
						if (num4-- <= 0u)
						{
							break;
						}
						sb_flags[(int)((UIntPtr)num)].coded_fully = (byte)num3;
					}
					num += 1u;
				}
				if (num5 != 0 && num < nsbs)
				{
					num2 = (long)Xiph.oc_pack_read1(dec.opb);
					num3 = (int)num2;
				}
				else
				{
					num3 = ((num3 == 0) ? 1 : 0);
				}
			}
			while (num < nsbs);
		}

		private static void oc_dec_coded_flags_unpack(Xiph.th_dec_ctx dec)
		{
			Xiph.oc_pack_buf opb = dec.opb;
			Xiph.oc_theora_state state = dec.state;
			uint num = Xiph.oc_dec_partial_sb_flags_unpack(dec);
			if (num < state.nsbs)
			{
				Xiph.oc_dec_coded_sb_flags_unpack(dec);
			}
			int num3;
			if (num > 0u)
			{
				long num2 = (long)Xiph.oc_pack_read1(opb);
				num3 = ((num2 == 0L) ? 1 : 0);
			}
			else
			{
				num3 = 0;
			}
			long[][][] sb_maps = state.sb_maps;
			Xiph.oc_sb_flags[] sb_flags = state.sb_flags;
			Xiph.oc_fragment[] frags = state.frags;
			uint num5;
			uint num4 = num5 = 0u;
			int num6 = 0;
			long[] coded_fragis = state.coded_fragis;
			int num7 = (int)state.nfrags;
			long num10;
			long num9;
			long num8 = num9 = (num10 = 0L);
			Xiph.oc_fragment_plane[] fplanes = state.fplanes;
			for (int i = 0; i < 3; i++)
			{
				num4 += fplanes[i].nsbs;
				while (num5 < num4)
				{
					long[][] array = sb_maps[(int)((UIntPtr)num5)];
					for (int j = 0; j < 4; j++)
					{
						if (((sb_flags[(int)((UIntPtr)num5)].bits & 60) >> 2 & 1 << j) != 0)
						{
							long[] array2 = array[j];
							for (int k = 0; k < 4; k++)
							{
								long num11 = array2[k];
								if (num11 >= 0L)
								{
									int num12;
									if ((sb_flags[(int)((UIntPtr)num5)].bits & 1) != 0)
									{
										num12 = 1;
									}
									else if ((sb_flags[(int)((UIntPtr)num5)].bits & 2) >> 1 == 0)
									{
										num12 = 0;
									}
									else
									{
										if (num6 <= 0)
										{
											short[] oC_BLOCK_RUN_TREE = Xiph.OC_BLOCK_RUN_TREE;
											Xiph.ogg_ptr ptr = opb.ptr;
											byte[] data = ptr.data;
											int l = ptr.offset;
											uint num13 = opb.window;
											Xiph.ogg_ptr stop = opb.stop;
											int offset = stop.offset;
											int num14 = opb.bits;
											int num15 = 0;
											int num16;
											while (true)
											{
												num16 = (int)oC_BLOCK_RUN_TREE[num15];
												if (num16 > num14)
												{
													uint num17 = (uint)(32 - num14);
													while (l < offset)
													{
														num17 -= 8u;
														num13 |= (uint)((uint)data[l] << (int)num17);
														l++;
														if (num17 < 8u)
														{
															IL_1C6:
															num14 = (int)(32u - num17);
															goto IL_1CD;
														}
													}
													num17 = 3221225472u;
													goto IL_1C6;
												}
												IL_1CD:
												long num18 = (long)((ulong)(num13 >> 32 - num16));
												num15 = (int)oC_BLOCK_RUN_TREE[(int)(checked((IntPtr)(unchecked((long)(num15 + 1) + num18))))];
												if (num15 <= 0)
												{
													break;
												}
												num13 <<= num16;
												num14 -= num16;
											}
											num15 = -num15;
											num16 = num15 >> 8;
											num13 <<= num16;
											num14 -= num16;
											opb.ptr.offset = l;
											opb.window = num13;
											opb.bits = num14;
											num6 = (num15 & 255);
											num3 = ((num3 == 0) ? 1 : 0);
										}
										num6--;
										num12 = num3;
									}
									checked
									{
										if (num12 != 0)
										{
											long[] arg_26C_0 = coded_fragis;
											long expr_263 = num8;
											num8 = unchecked(expr_263 + 1L);
											arg_26C_0[(int)((IntPtr)expr_263)] = num11;
										}
										else
										{
											coded_fragis[(int)((IntPtr)(unchecked((long)num7 - (num10 += 1L))))] = num11;
										}
										frags[(int)((IntPtr)num11)].coded = (uint)num12;
									}
								}
							}
						}
					}
					num5 += 1u;
				}
				dec.state.ncoded_fragis[i] = num8 - num9;
				num9 = num8;
			}
			dec.state.ntotal_coded_fragis = num8;
		}

		private static void oc_dec_mb_modes_unpack(Xiph.th_dec_ctx dec)
		{
			byte[] oc_dec_mb_modes_unpack__scheme0_alphabet = dec.oc_dec_mb_modes_unpack__scheme0_alphabet;
			long num = (long)Xiph.oc_pack_read(dec.opb, 3);
			int num2 = (int)num;
			byte[] array;
			if (num2 == 0)
			{
				for (int i = 0; i < 8; i++)
				{
					oc_dec_mb_modes_unpack__scheme0_alphabet[i] = 0;
				}
				for (int i = 0; i < 8; i++)
				{
					num = (long)Xiph.oc_pack_read(dec.opb, 3);
					oc_dec_mb_modes_unpack__scheme0_alphabet[(int)(checked((IntPtr)num))] = Xiph.OC_MODE_ALPHABETS[6][i];
				}
				array = oc_dec_mb_modes_unpack__scheme0_alphabet;
			}
			else
			{
				array = Xiph.OC_MODE_ALPHABETS[num2 - 1];
			}
			short[] tree = (num2 == 7) ? Xiph.OC_CLC_MODE_TREE : Xiph.OC_VLC_MODE_TREE;
			sbyte[] mb_modes = dec.state.mb_modes;
			long[][][] mb_maps = dec.state.mb_maps;
			uint nmbs = dec.state.nmbs;
			Xiph.oc_fragment[] frags = dec.state.frags;
			for (uint num3 = 0u; num3 < nmbs; num3 += 1u)
			{
				if (mb_modes[(int)((UIntPtr)num3)] != -1)
				{
					if (checked(frags[(int)((IntPtr)mb_maps[(int)(unchecked((UIntPtr)num3))][0][0])].coded != 0u || frags[(int)((IntPtr)mb_maps[(int)(unchecked((UIntPtr)num3))][0][1])].coded != 0u || frags[(int)((IntPtr)mb_maps[(int)(unchecked((UIntPtr)num3))][0][2])].coded != 0u || frags[(int)((IntPtr)mb_maps[(int)(unchecked((UIntPtr)num3))][0][3])].coded != 0u))
					{
						mb_modes[(int)((UIntPtr)num3)] = (sbyte)array[Xiph.oc_huff_token_decode(dec.opb, tree)];
					}
					else
					{
						mb_modes[(int)((UIntPtr)num3)] = 0;
					}
				}
			}
		}

		private static short oc_mv_unpack(Xiph.oc_pack_buf opb, short[] tree)
		{
			int x = Xiph.oc_huff_token_decode(opb, tree) - 32;
			int y = Xiph.oc_huff_token_decode(opb, tree) - 32;
			return Xiph.OC_MV(x, y);
		}

		private static void oc_dec_mv_unpack_and_frag_modes_fill(Xiph.th_dec_ctx dec)
		{
			short[] oc_dec_mv_unpack_and_frag_modes_fill__cbmvs = dec.oc_dec_mv_unpack_and_frag_modes_fill__cbmvs;
			int[] oc_dec_mv_unpack_and_frag_modes_fill__coded = dec.oc_dec_mv_unpack_and_frag_modes_fill__coded;
			short[] oc_dec_mv_unpack_and_frag_modes_fill__lbmvs = dec.oc_dec_mv_unpack_and_frag_modes_fill__lbmvs;
			Xiph.oc_set_chroma_mvs_func oc_set_chroma_mvs_func = Xiph.OC_SET_CHROMA_MVS_TABLE[(int)dec.state.info.pixel_fmt];
			long num = (long)Xiph.oc_pack_read1(dec.opb);
			short[] tree = (num != 0L) ? Xiph.OC_CLC_MV_COMP_TREE : Xiph.OC_VLC_MV_COMP_TREE;
			byte[] array = Xiph.OC_MB_MAP_IDXS[(int)dec.state.info.pixel_fmt];
			int num2 = (int)Xiph.OC_MB_MAP_NIDXS[(int)dec.state.info.pixel_fmt];
			short num4;
			short num3 = num4 = 0;
			Xiph.oc_fragment[] frags = dec.state.frags;
			short[] frag_mvs = dec.state.frag_mvs;
			long[][][] mb_maps = dec.state.mb_maps;
			sbyte[] mb_modes = dec.state.mb_modes;
			uint nmbs = dec.state.nmbs;
			for (uint num5 = 0u; num5 < nmbs; num5 += 1u)
			{
				int num6 = (int)mb_modes[(int)((UIntPtr)num5)];
				if (num6 != -1)
				{
					short num7 = 0;
					int num9;
					int num8 = num9 = 0;
					do
					{
						int num10 = (int)array[num8];
						long num11 = mb_maps[(int)((UIntPtr)num5)][num10 >> 2][num10 & 3];
						if (frags[(int)(checked((IntPtr)num11))].coded != 0u)
						{
							oc_dec_mv_unpack_and_frag_modes_fill__coded[num9++] = num10;
						}
					}
					while (++num8 < num2);
					if (num9 > 0)
					{
						switch (num6)
						{
						case 2:
							num4 = num3;
							num7 = (num3 = Xiph.oc_mv_unpack(dec.opb, tree));
							break;
						case 3:
							num7 = num3;
							break;
						case 4:
							num7 = num4;
							num4 = num3;
							num3 = num7;
							break;
						case 5:
							goto IL_26B;
						case 6:
							num7 = Xiph.oc_mv_unpack(dec.opb, tree);
							break;
						case 7:
						{
							oc_dec_mv_unpack_and_frag_modes_fill__coded[num9] = -1;
							int j;
							int i = j = 0;
							while (j < 4)
							{
								if (oc_dec_mv_unpack_and_frag_modes_fill__coded[i] == j)
								{
									i++;
									long num11 = mb_maps[(int)((UIntPtr)num5)][0][j];
									checked
									{
										frags[(int)((IntPtr)num11)].mb_mode = (uint)num6;
										oc_dec_mv_unpack_and_frag_modes_fill__lbmvs[j] = Xiph.oc_mv_unpack(dec.opb, tree);
										frag_mvs[(int)((IntPtr)num11)] = oc_dec_mv_unpack_and_frag_modes_fill__lbmvs[j];
									}
								}
								else
								{
									oc_dec_mv_unpack_and_frag_modes_fill__lbmvs[j] = 0;
								}
								j++;
							}
							if (i > 0)
							{
								num4 = num3;
								num3 = oc_dec_mv_unpack_and_frag_modes_fill__lbmvs[oc_dec_mv_unpack_and_frag_modes_fill__coded[i - 1]];
							}
							if (i < num9)
							{
								oc_set_chroma_mvs_func(oc_dec_mv_unpack_and_frag_modes_fill__cbmvs, oc_dec_mv_unpack_and_frag_modes_fill__lbmvs);
								while (i < num9)
								{
									int num10 = oc_dec_mv_unpack_and_frag_modes_fill__coded[i];
									j = (num10 & 3);
									long num11 = mb_maps[(int)((UIntPtr)num5)][num10 >> 2][j];
									checked
									{
										frags[(int)((IntPtr)num11)].mb_mode = (uint)num6;
										frag_mvs[(int)((IntPtr)num11)] = oc_dec_mv_unpack_and_frag_modes_fill__cbmvs[j];
									}
									i++;
								}
							}
							break;
						}
						default:
							goto IL_26B;
						}
						IL_26E:
						if (num6 != 7)
						{
							for (int i = 0; i < num9; i++)
							{
								int num10 = oc_dec_mv_unpack_and_frag_modes_fill__coded[i];
								long num11 = mb_maps[(int)((UIntPtr)num5)][num10 >> 2][num10 & 3];
								checked
								{
									frags[(int)((IntPtr)num11)].mb_mode = (uint)num6;
									frag_mvs[(int)((IntPtr)num11)] = num7;
								}
							}
							goto IL_2B5;
						}
						goto IL_2B5;
						IL_26B:
						num7 = 0;
						goto IL_26E;
					}
				}
				IL_2B5:;
			}
		}

		private static void oc_dec_block_qis_unpack(Xiph.th_dec_ctx dec)
		{
			long ntotal_coded_fragis = dec.state.ntotal_coded_fragis;
			if (ntotal_coded_fragis <= 0L)
			{
				return;
			}
			Xiph.oc_fragment[] frags = dec.state.frags;
			long[] coded_fragis = dec.state.coded_fragis;
			long num;
			if (dec.state.nqis == 1)
			{
				for (num = 0L; num < ntotal_coded_fragis; num += 1L)
				{
					frags[(int)(checked((IntPtr)coded_fragis[(int)((IntPtr)num)]))].qii = 0u;
				}
				return;
			}
			long num2 = (long)Xiph.oc_pack_read1(dec.opb);
			int num3 = (int)num2;
			int num4 = 0;
			num = 0L;
			while (num < ntotal_coded_fragis)
			{
				int num5 = Xiph.oc_sb_run_unpack(dec.opb);
				int num6 = (num5 >= 4129) ? 1 : 0;
				do
				{
					Xiph.oc_fragment[] arg_9A_0 = frags;
					long[] arg_98_0 = coded_fragis;
					long expr_92 = num;
					num = expr_92 + 1L;
					arg_9A_0[(int)arg_98_0[(int)(checked((IntPtr)expr_92))]].qii = (uint)num3;
					num4 += num3;
				}
				while (--num5 > 0 && num < ntotal_coded_fragis);
				if (num6 != 0 && num < ntotal_coded_fragis)
				{
					num2 = (long)Xiph.oc_pack_read1(dec.opb);
					num3 = (int)num2;
				}
				else
				{
					num3 = ((num3 == 0) ? 1 : 0);
				}
			}
			if (dec.state.nqis == 3 && num4 > 0)
			{
				num = 0L;
				while (frags[(int)(checked((IntPtr)coded_fragis[(int)((IntPtr)num)]))].qii == 0u)
				{
					num += 1L;
				}
				num2 = (long)Xiph.oc_pack_read1(dec.opb);
				num3 = (int)num2;
				do
				{
					int num5 = Xiph.oc_sb_run_unpack(dec.opb);
					int num7 = (num5 >= 4129) ? 1 : 0;
					while (num < ntotal_coded_fragis)
					{
						long num8 = coded_fragis[(int)(checked((IntPtr)num))];
						if (frags[(int)(checked((IntPtr)num8))].qii != 0u)
						{
							if (num5-- <= 0)
							{
								break;
							}
							Xiph.oc_fragment[] expr_175_cp_0 = frags;
							IntPtr expr_175_cp_1 = checked((IntPtr)num8);
							expr_175_cp_0[(int)expr_175_cp_1].qii = expr_175_cp_0[(int)expr_175_cp_1].qii + (uint)num3;
						}
						num += 1L;
					}
					if (num7 != 0 && num < ntotal_coded_fragis)
					{
						num2 = (long)Xiph.oc_pack_read1(dec.opb);
						num3 = (int)num2;
					}
					else
					{
						num3 = ((num3 == 0) ? 1 : 0);
					}
				}
				while (num < ntotal_coded_fragis);
			}
		}

		private static long oc_dec_dc_coeff_unpack(Xiph.th_dec_ctx dec, int[] huff_idxs, long[][] ntoks_left)
		{
			long[] oc_dec_dc_coeff_unpack__run_counts = dec.oc_dec_dc_coeff_unpack__run_counts;
			byte[] dct_tokens = dec.dct_tokens;
			Xiph.oc_fragment[] frags = dec.state.frags;
			long[] coded_fragis = dec.state.coded_fragis;
			long num4;
			long num3;
			long num2;
			long num = num2 = (num3 = (num4 = 0L));
			for (int i = 0; i < 3; i++)
			{
				num2 += dec.state.ncoded_fragis[i];
				Array.Clear(oc_dec_dc_coeff_unpack__run_counts, 0, oc_dec_dc_coeff_unpack__run_counts.Length);
				dec.eob_runs[i][0] = num3;
				dec.ti0[i][0] = num4;
				long num5 = num3;
				if (num2 - num < num5)
				{
					num5 = num2 - num;
				}
				long num6 = num5;
				num3 -= num5;
				while (true)
				{
					long expr_AB = num5;
					num5 = expr_AB - 1L;
					if (expr_AB <= 0L)
					{
						break;
					}
					Xiph.oc_fragment[] arg_9E_0 = frags;
					long[] arg_9C_0 = coded_fragis;
					long expr_95 = num;
					num = expr_95 + 1L;
					arg_9E_0[(int)arg_9C_0[(int)(checked((IntPtr)expr_95))]].dc = 0;
				}
				while (num < num2)
				{
					Xiph.oc_pack_buf opb = dec.opb;
					short[] array = dec.huff_tables[huff_idxs[i + 1 >> 1]];
					Xiph.ogg_ptr ptr = opb.ptr;
					byte[] data = ptr.data;
					int j = ptr.offset;
					uint num7 = opb.window;
					Xiph.ogg_ptr stop = opb.stop;
					int offset = stop.offset;
					int num8 = opb.bits;
					int num9 = 0;
					int num10;
					while (true)
					{
						num10 = (int)array[num9];
						if (num10 > num8)
						{
							uint num11 = (uint)(32 - num8);
							while (j < offset)
							{
								num11 -= 8u;
								num7 |= (uint)((uint)data[j] << (int)num11);
								j++;
								if (num11 < 8u)
								{
									IL_159:
									num8 = (int)(32u - num11);
									goto IL_160;
								}
							}
							num11 = 3221225472u;
							goto IL_159;
						}
						IL_160:
						long num12 = (long)((ulong)(num7 >> 32 - num10));
						num9 = (int)array[(int)(checked((IntPtr)(unchecked((long)(num9 + 1) + num12))))];
						if (num9 <= 0)
						{
							break;
						}
						num7 <<= num10;
						num8 -= num10;
					}
					num9 = -num9;
					num10 = num9 >> 8;
					num7 <<= num10;
					num8 -= num10;
					opb.ptr.offset = j;
					opb.window = num7;
					opb.bits = num8;
					int num13 = num9 & 255;
					byte[] arg_1E7_0 = dct_tokens;
					long expr_1DD = num4;
					num4 = expr_1DD + 1L;
					arg_1E7_0[(int)(checked((IntPtr)expr_1DD))] = (byte)num13;
					int num14;
					if (num13 < 15)
					{
						num14 = Xiph.oc_pack_read(dec.opb, (int)Xiph.OC_INTERNAL_DCT_TOKEN_EXTRA_BITS[num13]);
						byte[] arg_210_0 = dct_tokens;
						long expr_206 = num4;
						num4 = expr_206 + 1L;
						arg_210_0[(int)(checked((IntPtr)expr_206))] = (byte)num14;
						if (num13 == 0)
						{
							byte[] arg_224_0 = dct_tokens;
							long expr_218 = num4;
							num4 = expr_218 + 1L;
							arg_224_0[(int)(checked((IntPtr)expr_218))] = (byte)(num14 >> 8);
						}
						num14 <<= (-13 & ((num13 < 2) ? -1 : 0)) + (21 & ((num13 < 12) ? -1 : 0));
					}
					else
					{
						num14 = 0;
					}
					int num15 = Xiph.OC_DCT_CODE_WORD[num13] + num14;
					num3 = (long)(num15 >> 8 & 4095);
					if (num15 == 0)
					{
						num3 = 2147483647L;
					}
					if (num3 != 0L)
					{
						num5 = ((num2 - num < num3) ? (num2 - num) : num3);
						num6 += num5;
						num3 -= num5;
						while (true)
						{
							long expr_2B4 = num5;
							num5 = expr_2B4 - 1L;
							if (expr_2B4 <= 0L)
							{
								break;
							}
							Xiph.oc_fragment[] arg_2A7_0 = frags;
							long[] arg_2A5_0 = coded_fragis;
							long expr_29E = num;
							num = expr_29E + 1L;
							arg_2A7_0[(int)arg_2A5_0[(int)(checked((IntPtr)expr_29E))]].dc = 0;
						}
					}
					else
					{
						int num16 = (int)((byte)num15);
						num15 ^= -(num15 & 1048576);
						int dc = num15 >> 21;
						if (num16 != 0)
						{
							dc = 0;
						}
						oc_dec_dc_coeff_unpack__run_counts[num16] += 1L;
						Xiph.oc_fragment[] arg_305_0 = frags;
						long[] arg_303_0 = coded_fragis;
						long expr_2FC = num;
						num = expr_2FC + 1L;
						arg_305_0[(int)arg_303_0[(int)(checked((IntPtr)expr_2FC))]].dc = dc;
					}
				}
				oc_dec_dc_coeff_unpack__run_counts[63] += num6;
				int num17 = 63;
				while (num17-- > 0)
				{
					oc_dec_dc_coeff_unpack__run_counts[num17] += oc_dec_dc_coeff_unpack__run_counts[num17 + 1];
				}
				num17 = 64;
				while (num17-- > 0)
				{
					ntoks_left[i][num17] -= oc_dec_dc_coeff_unpack__run_counts[num17];
				}
			}
			dec.dct_tokens_count = (int)num4;
			return num3;
		}

		private static int oc_dec_ac_coeff_unpack(Xiph.th_dec_ctx dec, int zzi, int[] huff_idxs, long[][] ntoks_left_arr, long eobs)
		{
			long[] oc_dec_ac_coeff_unpack__run_counts = dec.oc_dec_ac_coeff_unpack__run_counts;
			short[][] huff_tables = dec.huff_tables;
			Xiph.oc_pack_buf opb = dec.opb;
			byte[] dct_tokens = dec.dct_tokens;
			long num = (long)dec.dct_tokens_count;
			for (int i = 0; i < 3; i++)
			{
				int num2 = huff_idxs[i + 1 >> 1];
				short[] array = huff_tables[num2];
				dec.eob_runs[i][zzi] = eobs;
				dec.ti0[i][zzi] = num;
				uint num3 = (uint)ntoks_left_arr[i][zzi];
				for (int j = 0; j < 64; j++)
				{
					oc_dec_ac_coeff_unpack__run_counts[j] = 0L;
				}
				long num4 = 0L;
				uint num5 = 0u;
				while ((ulong)num5 + (ulong)eobs < (ulong)num3)
				{
					num5 += (uint)eobs;
					num4 += eobs;
					short[] array2 = array;
					Xiph.ogg_ptr ptr = opb.ptr;
					byte[] data = ptr.data;
					int k = ptr.offset;
					uint num6 = opb.window;
					Xiph.ogg_ptr stop = opb.stop;
					int offset = stop.offset;
					int num7 = opb.bits;
					int num8 = 0;
					int num9;
					while (true)
					{
						num9 = (int)array2[num8];
						if (num9 > num7)
						{
							uint num10 = (uint)(32 - num7);
							while (k < offset)
							{
								num10 -= 8u;
								num6 |= (uint)((uint)data[k] << (int)num10);
								k++;
								if (num10 < 8u)
								{
									IL_115:
									num7 = (int)(32u - num10);
									goto IL_11C;
								}
							}
							num10 = 3221225472u;
							goto IL_115;
						}
						IL_11C:
						long num11 = (long)((ulong)(num6 >> 32 - num9));
						num8 = (int)array2[(int)(checked((IntPtr)(unchecked((long)(num8 + 1) + num11))))];
						if (num8 <= 0)
						{
							break;
						}
						num6 <<= num9;
						num7 -= num9;
					}
					num8 = -num8;
					num9 = num8 >> 8;
					num6 <<= num9;
					num7 -= num9;
					opb.ptr.offset = k;
					opb.window = num6;
					opb.bits = num7;
					int num12 = num8 & 255;
					byte[] arg_1A1_0 = dct_tokens;
					long expr_198 = num;
					num = expr_198 + 1L;
					arg_1A1_0[(int)(checked((IntPtr)expr_198))] = (byte)num12;
					int num13;
					if (num12 < 15)
					{
						num13 = Xiph.oc_pack_read(opb, (int)Xiph.OC_INTERNAL_DCT_TOKEN_EXTRA_BITS[num12]);
						byte[] arg_1C4_0 = dct_tokens;
						long expr_1BB = num;
						num = expr_1BB + 1L;
						arg_1C4_0[(int)(checked((IntPtr)expr_1BB))] = (byte)num13;
						if (num12 == 0)
						{
							byte[] arg_1D6_0 = dct_tokens;
							long expr_1CB = num;
							num = expr_1CB + 1L;
							arg_1D6_0[(int)(checked((IntPtr)expr_1CB))] = (byte)(num13 >> 8);
						}
						num13 <<= (-13 & ((num12 < 2) ? -1 : 0)) + (21 & ((num12 < 12) ? -1 : 0));
					}
					else
					{
						num13 = 0;
					}
					int num14 = Xiph.OC_DCT_CODE_WORD[num12] + num13;
					int num15 = (int)((byte)num14);
					eobs = (long)(num14 >> 8 & 4095);
					if (num14 == 0)
					{
						eobs = 2147483647L;
					}
					if (eobs == 0L)
					{
						oc_dec_ac_coeff_unpack__run_counts[num15] += 1L;
						num5 += 1u;
					}
				}
				num4 += (long)((ulong)(num3 - num5));
				eobs -= (long)((ulong)(num3 - num5));
				oc_dec_ac_coeff_unpack__run_counts[63] += num4;
				long[] array3 = oc_dec_ac_coeff_unpack__run_counts;
				long num16 = array3[0];
				long num17 = array3[1];
				long num18 = array3[2];
				long num19 = array3[3];
				long num20 = array3[4];
				long num21 = array3[5];
				long num22 = array3[6];
				long num23 = array3[7];
				long num24 = array3[8];
				long num25 = array3[9];
				long num26 = array3[10];
				long num27 = array3[11];
				long num28 = array3[12];
				long num29 = array3[13];
				long num30 = array3[14];
				long num31 = array3[15];
				long num32 = array3[16];
				long num33 = array3[17];
				long num34 = array3[18];
				long num35 = array3[19];
				long num36 = array3[20];
				long num37 = array3[21];
				long num38 = array3[22];
				long num39 = array3[23];
				long num40 = array3[24];
				long num41 = array3[25];
				long num42 = array3[26];
				long num43 = array3[27];
				long num44 = array3[28];
				long num45 = array3[29];
				long num46 = array3[30];
				long num47 = array3[31];
				long num48 = array3[32];
				long num49 = array3[33];
				long num50 = array3[34];
				long num51 = array3[35];
				long num52 = array3[36];
				long num53 = array3[37];
				long num54 = array3[38];
				long num55 = array3[39];
				long num56 = array3[40];
				long num57 = array3[41];
				long num58 = array3[42];
				long num59 = array3[43];
				long num60 = array3[44];
				long num61 = array3[45];
				long num62 = array3[46];
				long num63 = array3[47];
				long num64 = array3[48];
				long num65 = array3[49];
				long num66 = array3[50];
				long num67 = array3[51];
				long num68 = array3[52];
				long num69 = array3[53];
				long num70 = array3[54];
				long num71 = array3[55];
				long num72 = array3[56];
				long num73 = array3[57];
				long num74 = array3[58];
				long num75 = array3[59];
				long num76 = array3[60];
				long num77 = array3[61];
				long num78 = array3[62];
				long num79 = array3[63];
				num78 += num79;
				num77 += num78;
				num76 += num77;
				num75 += num76;
				num74 += num75;
				num73 += num74;
				num72 += num73;
				num71 += num72;
				num70 += num71;
				num69 += num70;
				num68 += num69;
				num67 += num68;
				num66 += num67;
				num65 += num66;
				num64 += num65;
				num63 += num64;
				num62 += num63;
				num61 += num62;
				num60 += num61;
				num59 += num60;
				num58 += num59;
				num57 += num58;
				num56 += num57;
				num55 += num56;
				num54 += num55;
				num53 += num54;
				num52 += num53;
				num51 += num52;
				num50 += num51;
				num49 += num50;
				num48 += num49;
				num47 += num48;
				num46 += num47;
				num45 += num46;
				num44 += num45;
				num43 += num44;
				num42 += num43;
				num41 += num42;
				num40 += num41;
				num39 += num40;
				num38 += num39;
				num37 += num38;
				num36 += num37;
				num35 += num36;
				num34 += num35;
				num33 += num34;
				num32 += num33;
				num31 += num32;
				num30 += num31;
				num29 += num30;
				num28 += num29;
				num27 += num28;
				num26 += num27;
				num25 += num26;
				num24 += num25;
				num23 += num24;
				num22 += num23;
				num21 += num22;
				num20 += num21;
				num19 += num20;
				num18 += num19;
				num17 += num18;
				num16 += num17;
				array3[0] = num16;
				array3[1] = num17;
				array3[2] = num18;
				array3[3] = num19;
				array3[4] = num20;
				array3[5] = num21;
				array3[6] = num22;
				array3[7] = num23;
				array3[8] = num24;
				array3[9] = num25;
				array3[10] = num26;
				array3[11] = num27;
				array3[12] = num28;
				array3[13] = num29;
				array3[14] = num30;
				array3[15] = num31;
				array3[16] = num32;
				array3[17] = num33;
				array3[18] = num34;
				array3[19] = num35;
				array3[20] = num36;
				array3[21] = num37;
				array3[22] = num38;
				array3[23] = num39;
				array3[24] = num40;
				array3[25] = num41;
				array3[26] = num42;
				array3[27] = num43;
				array3[28] = num44;
				array3[29] = num45;
				array3[30] = num46;
				array3[31] = num47;
				array3[32] = num48;
				array3[33] = num49;
				array3[34] = num50;
				array3[35] = num51;
				array3[36] = num52;
				array3[37] = num53;
				array3[38] = num54;
				array3[39] = num55;
				array3[40] = num56;
				array3[41] = num57;
				array3[42] = num58;
				array3[43] = num59;
				array3[44] = num60;
				array3[45] = num61;
				array3[46] = num62;
				array3[47] = num63;
				array3[48] = num64;
				array3[49] = num65;
				array3[50] = num66;
				array3[51] = num67;
				array3[52] = num68;
				array3[53] = num69;
				array3[54] = num70;
				array3[55] = num71;
				array3[56] = num72;
				array3[57] = num73;
				array3[58] = num74;
				array3[59] = num75;
				array3[60] = num76;
				array3[61] = num77;
				array3[62] = num78;
				array3[63] = num79;
				for (int l = 0; l < 64 - zzi; l++)
				{
					ntoks_left_arr[i][zzi + l] -= oc_dec_ac_coeff_unpack__run_counts[l];
				}
			}
			dec.dct_tokens_count = (int)num;
			return (int)eobs;
		}

		private static void moment_table(long[] lv)
		{
			long num = lv[0];
			long num2 = lv[1];
			long num3 = lv[2];
			long num4 = lv[3];
			long num5 = lv[4];
			long num6 = lv[5];
			long num7 = lv[6];
			long num8 = lv[7];
			long num9 = lv[8];
			long num10 = lv[9];
			long num11 = lv[10];
			long num12 = lv[11];
			long num13 = lv[12];
			long num14 = lv[13];
			long num15 = lv[14];
			long num16 = lv[15];
			long num17 = lv[16];
			long num18 = lv[17];
			long num19 = lv[18];
			long num20 = lv[19];
			long num21 = lv[20];
			long num22 = lv[21];
			long num23 = lv[22];
			long num24 = lv[23];
			long num25 = lv[24];
			long num26 = lv[25];
			long num27 = lv[26];
			long num28 = lv[27];
			long num29 = lv[28];
			long num30 = lv[29];
			long num31 = lv[30];
			long num32 = lv[31];
			long num33 = lv[32];
			long num34 = lv[33];
			long num35 = lv[34];
			long num36 = lv[35];
			long num37 = lv[36];
			long num38 = lv[37];
			long num39 = lv[38];
			long num40 = lv[39];
			long num41 = lv[40];
			long num42 = lv[41];
			long num43 = lv[42];
			long num44 = lv[43];
			long num45 = lv[44];
			long num46 = lv[45];
			long num47 = lv[46];
			long num48 = lv[47];
			long num49 = lv[48];
			long num50 = lv[49];
			long num51 = lv[50];
			long num52 = lv[51];
			long num53 = lv[52];
			long num54 = lv[53];
			long num55 = lv[54];
			long num56 = lv[55];
			long num57 = lv[56];
			long num58 = lv[57];
			long num59 = lv[58];
			long num60 = lv[59];
			long num61 = lv[60];
			long num62 = lv[61];
			long num63 = lv[62];
			long num64 = lv[63];
			num63 += num64;
			num62 += num63;
			num61 += num62;
			num60 += num61;
			num59 += num60;
			num58 += num59;
			num57 += num58;
			num56 += num57;
			num55 += num56;
			num54 += num55;
			num53 += num54;
			num52 += num53;
			num51 += num52;
			num50 += num51;
			num49 += num50;
			num48 += num49;
			num47 += num48;
			num46 += num47;
			num45 += num46;
			num44 += num45;
			num43 += num44;
			num42 += num43;
			num41 += num42;
			num40 += num41;
			num39 += num40;
			num38 += num39;
			num37 += num38;
			num36 += num37;
			num35 += num36;
			num34 += num35;
			num33 += num34;
			num32 += num33;
			num31 += num32;
			num30 += num31;
			num29 += num30;
			num28 += num29;
			num27 += num28;
			num26 += num27;
			num25 += num26;
			num24 += num25;
			num23 += num24;
			num22 += num23;
			num21 += num22;
			num20 += num21;
			num19 += num20;
			num18 += num19;
			num17 += num18;
			num16 += num17;
			num15 += num16;
			num14 += num15;
			num13 += num14;
			num12 += num13;
			num11 += num12;
			num10 += num11;
			num9 += num10;
			num8 += num9;
			num7 += num8;
			num6 += num7;
			num5 += num6;
			num4 += num5;
			num3 += num4;
			num2 += num3;
			num += num2;
			lv[0] = num;
			lv[1] = num2;
			lv[2] = num3;
			lv[3] = num4;
			lv[4] = num5;
			lv[5] = num6;
			lv[6] = num7;
			lv[7] = num8;
			lv[8] = num9;
			lv[9] = num10;
			lv[10] = num11;
			lv[11] = num12;
			lv[12] = num13;
			lv[13] = num14;
			lv[14] = num15;
			lv[15] = num16;
			lv[16] = num17;
			lv[17] = num18;
			lv[18] = num19;
			lv[19] = num20;
			lv[20] = num21;
			lv[21] = num22;
			lv[22] = num23;
			lv[23] = num24;
			lv[24] = num25;
			lv[25] = num26;
			lv[26] = num27;
			lv[27] = num28;
			lv[28] = num29;
			lv[29] = num30;
			lv[30] = num31;
			lv[31] = num32;
			lv[32] = num33;
			lv[33] = num34;
			lv[34] = num35;
			lv[35] = num36;
			lv[36] = num37;
			lv[37] = num38;
			lv[38] = num39;
			lv[39] = num40;
			lv[40] = num41;
			lv[41] = num42;
			lv[42] = num43;
			lv[43] = num44;
			lv[44] = num45;
			lv[45] = num46;
			lv[46] = num47;
			lv[47] = num48;
			lv[48] = num49;
			lv[49] = num50;
			lv[50] = num51;
			lv[51] = num52;
			lv[52] = num53;
			lv[53] = num54;
			lv[54] = num55;
			lv[55] = num56;
			lv[56] = num57;
			lv[57] = num58;
			lv[58] = num59;
			lv[59] = num60;
			lv[60] = num61;
			lv[61] = num62;
			lv[62] = num63;
			lv[63] = num64;
		}

		private static void oc_dec_residual_tokens_unpack(Xiph.th_dec_ctx dec)
		{
			long[][] oc_dec_residual_tokens_unpack__ntoks_left = dec.oc_dec_residual_tokens_unpack__ntoks_left;
			int[] oc_dec_residual_tokens_unpack__huff_idxs = dec.oc_dec_residual_tokens_unpack__huff_idxs;
			int j;
			for (int i = 0; i < 3; i++)
			{
				long num = dec.state.ncoded_fragis[i];
				for (j = 0; j < 64; j++)
				{
					oc_dec_residual_tokens_unpack__ntoks_left[i][j] = num;
				}
			}
			long num2 = (long)Xiph.oc_pack_read(dec.opb, 4);
			oc_dec_residual_tokens_unpack__huff_idxs[0] = (int)num2;
			num2 = (long)Xiph.oc_pack_read(dec.opb, 4);
			oc_dec_residual_tokens_unpack__huff_idxs[1] = (int)num2;
			dec.eob_runs[0][0] = 0L;
			long eobs = Xiph.oc_dec_dc_coeff_unpack(dec, oc_dec_residual_tokens_unpack__huff_idxs, oc_dec_residual_tokens_unpack__ntoks_left);
			num2 = (long)Xiph.oc_pack_read(dec.opb, 4);
			oc_dec_residual_tokens_unpack__huff_idxs[0] = (int)num2;
			num2 = (long)Xiph.oc_pack_read(dec.opb, 4);
			oc_dec_residual_tokens_unpack__huff_idxs[1] = (int)num2;
			j = 1;
			for (int k = 1; k < 5; k++)
			{
				oc_dec_residual_tokens_unpack__huff_idxs[0] += 16;
				oc_dec_residual_tokens_unpack__huff_idxs[1] += 16;
				while (j < (int)Xiph.OC_HUFF_LIST_MAX[k])
				{
					eobs = (long)Xiph.oc_dec_ac_coeff_unpack(dec, j, oc_dec_residual_tokens_unpack__huff_idxs, oc_dec_residual_tokens_unpack__ntoks_left, eobs);
					j++;
				}
			}
		}

		private static int oc_dec_postprocess_init(Xiph.th_dec_ctx dec)
		{
			if (dec.pp_level <= 0)
			{
				if (dec.dc_qis != null)
				{
					dec.dc_qis = null;
					dec.variances = null;
					dec.pp_frame_data = null;
				}
				return 1;
			}
			if (dec.dc_qis == null)
			{
				if (dec.state.frame_type != 0)
				{
					return 1;
				}
				dec.dc_qis = new byte[dec.state.nfrags];
				int num = 0;
				while ((long)num < dec.state.nfrags)
				{
					dec.dc_qis[num] = dec.state.qis[0];
					num++;
				}
			}
			else
			{
				byte[] dc_qis = dec.dc_qis;
				long[] coded_fragis = dec.state.coded_fragis;
				long num2 = dec.state.ncoded_fragis[0] + dec.state.ncoded_fragis[1] + dec.state.ncoded_fragis[2];
				byte b = dec.state.qis[0];
				for (long num3 = 0L; num3 < num2; num3 += 1L)
				{
					dc_qis[(int)(checked((IntPtr)coded_fragis[(int)((IntPtr)num3)]))] = b;
				}
			}
			if (dec.pp_level <= 1)
			{
				if (dec.variances != null)
				{
					dec.variances = null;
					dec.pp_frame_data = null;
				}
				return 1;
			}
			if (dec.variances == null)
			{
				uint num4 = dec.state.info.frame_width * dec.state.info.frame_height;
				int num5 = (dec.state.info.frame_width >> (int)(dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == 0u) ? 1 : 0;
				int num6 = (dec.state.info.frame_height >> (int)(dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == 0u) ? 1 : 0;
				uint num7 = (uint)(num5 * num6);
				num4 += num7 << 1;
				dec.pp_frame_data = new byte[num4];
				dec.variances = new int[dec.state.nfrags];
				if (dec.variances == null || dec.pp_frame_data == null)
				{
					dec.pp_frame_data = null;
					dec.variances = null;
					return 1;
				}
				dec.pp_frame_state = 0;
			}
			if (dec.pp_frame_state != 1 + ((dec.pp_level >= 5) ? 1 : 0))
			{
				if (dec.pp_level < 5)
				{
					dec.pp_frame_buf.y.width = (int)dec.state.info.frame_width;
					dec.pp_frame_buf.y.height = (int)dec.state.info.frame_height;
					dec.pp_frame_buf.y.stride = -dec.pp_frame_buf[0].width;
					dec.pp_frame_buf.y.data = new Xiph.ogg_ptr(dec.pp_frame_data, (int)((long)(1 - dec.pp_frame_buf.y.height) * (long)dec.pp_frame_buf.y.stride));
				}
				else
				{
					long num8 = (long)((ulong)(dec.state.info.frame_width * dec.state.info.frame_height));
					int num9 = (dec.state.info.frame_width >> (int)(dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == 0u) ? 1 : 0;
					int num10 = (dec.state.info.frame_height >> (int)(dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == 0u) ? 1 : 0;
					long num11 = (long)num9 * (long)((ulong)num10);
					dec.pp_frame_buf.y.width = (int)dec.state.info.frame_width;
					dec.pp_frame_buf.y.height = (int)dec.state.info.frame_height;
					dec.pp_frame_buf.y.stride = dec.pp_frame_buf.y.width;
					dec.pp_frame_buf.y.data = new Xiph.ogg_ptr(dec.pp_frame_data, 0);
					dec.pp_frame_buf.cb.width = num9;
					dec.pp_frame_buf.cb.height = num10;
					dec.pp_frame_buf.cb.stride = dec.pp_frame_buf.cb.width;
					dec.pp_frame_buf.cb.data = dec.pp_frame_buf.y.data + (int)num8;
					dec.pp_frame_buf.cr.width = num9;
					dec.pp_frame_buf.cr.height = num10;
					dec.pp_frame_buf.cr.stride = dec.pp_frame_buf.cr.width;
					dec.pp_frame_buf.cr.data = dec.pp_frame_buf.cb.data + (int)num11;
					Xiph.oc_ycbcr_buffer_flip(ref dec.pp_frame_buf, ref dec.pp_frame_buf);
				}
				dec.pp_frame_state = 1 + ((dec.pp_level >= 5) ? 1 : 0);
			}
			if (dec.pp_level < 5)
			{
				dec.pp_frame_buf.cb = dec.state.ref_frame_bufs[dec.state.ref_frame_idx[2]].cb;
				dec.pp_frame_buf.cr = dec.state.ref_frame_bufs[dec.state.ref_frame_idx[2]].cr;
			}
			return 0;
		}

		private static void oc_dec_pipeline_init(Xiph.th_dec_ctx dec, Xiph.oc_dec_pipeline_state pipe)
		{
			pipe.mcu_nvfrags = 4 << (((dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0);
			Xiph.Copy3x64(dec.ti0, pipe.ti);
			Xiph.Copy3x64(dec.eob_runs, pipe.eob_runs);
			Xiph.Ptr<long> ptr = new Xiph.Ptr<long>(dec.state.coded_fragis, 0);
			Xiph.Ptr<long> ptr2 = ptr + (int)dec.state.nfrags;
			for (int i = 0; i < 3; i++)
			{
				pipe.coded_fragis[i] = ptr;
				pipe.uncoded_fragis[i] = ptr2;
				long num = dec.state.ncoded_fragis[i];
				ptr.offset += (int)num;
				ptr2.offset += (int)(num - dec.state.fplanes[i].nfrags);
			}
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < (int)dec.state.nqis; j++)
				{
					for (int k = 0; k < 2; k++)
					{
						pipe.dequant[i][j][k] = dec.state.dequant_tables[(int)dec.state.qis[j]][i][k];
					}
				}
			}
			Xiph.Clear3x3(pipe.pred_last);
			int num2 = (int)dec.state.loop_filter_limits[(int)dec.state.qis[0]];
			pipe.loop_filter = ((num2 != 0) ? 1 : 0);
			if (num2 != 0)
			{
				Xiph.oc_loop_filter_init(dec.state, pipe.bounding_values, num2);
			}
			if (Xiph.oc_dec_postprocess_init(dec) == 0)
			{
				pipe.pp_level = dec.pp_level;
			}
			else
			{
				pipe.pp_level = 0;
				dec.pp_frame_buf = dec.state.ref_frame_bufs[dec.state.ref_frame_idx[2]];
			}
			for (int l = 0; l < 64; l++)
			{
				pipe.dct_coeffs[l] = 0;
			}
		}

		private static void oc_dec_dc_unpredict_mcu_plane(Xiph.th_dec_ctx dec, Xiph.oc_dec_pipeline_state pipe, int pli)
		{
			int num = pipe.fragy0[pli];
			int num2 = pipe.fragy_end[pli];
			int nhfrags = dec.state.fplanes[pli].nhfrags;
			Xiph.oc_fragment[] frags = dec.state.frags;
			long num3 = 0L;
			long num4 = dec.state.fplanes[pli].froffset + (long)num * (long)nhfrags;
			for (int i = num; i < num2; i++)
			{
				if (i == 0)
				{
					int j = 0;
					while (j < nhfrags)
					{
						if (frags[(int)(checked((IntPtr)num4))].coded != 0u)
						{
							int mb_mode = (int)frags[(int)(checked((IntPtr)num4))].mb_mode;
							int num5 = 268505377 >> mb_mode * 4 & 15;
							int[] arg_C9_0 = pipe.pred_last[pli];
							int arg_C9_1 = num5;
							Xiph.oc_fragment[] expr_AD_cp_0 = frags;
							IntPtr expr_AD_cp_1 = checked((IntPtr)num4);
							arg_C9_0[arg_C9_1] = (expr_AD_cp_0[(int)expr_AD_cp_1].dc = expr_AD_cp_0[(int)expr_AD_cp_1].dc + pipe.pred_last[pli][num5]);
							num3 += 1L;
						}
						j++;
						num4 += 1L;
					}
				}
				else
				{
					int mb_mode2 = (int)frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4))))].mb_mode;
					int num6 = 268505377 >> mb_mode2 * 4 & 15;
					int num7 = -1;
					int num8 = -1;
					int num9 = (frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4))))].coded != 0u) ? num6 : -1;
					int j = 0;
					while (j < nhfrags)
					{
						int num10;
						if (j + 1 >= nhfrags)
						{
							num10 = -1;
						}
						else
						{
							int mb_mode3 = (int)frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].mb_mode;
							int num11 = 268505377 >> mb_mode3 * 4 & 15;
							num10 = ((frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].coded != 0u) ? num11 : -1);
						}
						if (frags[(int)(checked((IntPtr)num4))].coded != 0u)
						{
							int mb_mode4 = (int)frags[(int)(checked((IntPtr)num4))].mb_mode;
							int num12 = 268505377 >> mb_mode4 * 4 & 15;
							int num13;
							switch (((num7 == num12) ? 1 : 0) | ((num8 == num12) ? 1 : 0) << 1 | ((num9 == num12) ? 1 : 0) << 2 | ((num10 == num12) ? 1 : 0) << 3)
							{
							case 1:
							case 3:
								num13 = frags[(int)(checked((IntPtr)(unchecked(num4 - 1L))))].dc;
								break;
							case 2:
								num13 = frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 - 1L))))].dc;
								break;
							case 4:
							case 6:
							case 12:
								num13 = frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4))))].dc;
								break;
							case 5:
								num13 = (frags[(int)(checked((IntPtr)(unchecked(num4 - 1L))))].dc + frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4))))].dc) / 2;
								break;
							case 7:
							case 15:
							{
								int dc;
								int dc2;
								int dc3;
								checked
								{
									dc = frags[(int)((IntPtr)(unchecked(num4 - 1L)))].dc;
									dc2 = frags[(int)((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 - 1L)))].dc;
									dc3 = frags[(int)((IntPtr)(unchecked((long)(-(long)nhfrags) + num4)))].dc;
								}
								num13 = (29 * (dc + dc3) - 26 * dc2) / 32;
								int num14 = num13 - dc;
								if (num14 < 0)
								{
									num14 = -num14;
								}
								int num15 = num13 - dc2;
								if (num15 < 0)
								{
									num15 = -num15;
								}
								int num16 = num13 - dc3;
								if (num16 < 0)
								{
									num16 = -num16;
								}
								if (num16 > 128)
								{
									num13 = dc3;
								}
								else if (num14 > 128)
								{
									num13 = dc;
								}
								else if (num15 > 128)
								{
									num13 = dc2;
								}
								break;
							}
							case 8:
								num13 = frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].dc;
								break;
							case 9:
							case 11:
							case 13:
								num13 = (75 * frags[(int)(checked((IntPtr)(unchecked(num4 - 1L))))].dc + 53 * frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].dc) / 128;
								break;
							case 10:
								num13 = (frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 - 1L))))].dc + frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].dc) / 2;
								break;
							case 14:
								num13 = (3 * (frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 - 1L))))].dc + frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4 + 1L))))].dc) + 10 * frags[(int)(checked((IntPtr)(unchecked((long)(-(long)nhfrags) + num4))))].dc) / 16;
								break;
							default:
								num13 = pipe.pred_last[pli][num12];
								break;
							}
							int[] arg_461_0 = pipe.pred_last[pli];
							int arg_461_1 = num12;
							Xiph.oc_fragment[] expr_44E_cp_0 = frags;
							IntPtr expr_44E_cp_1 = checked((IntPtr)num4);
							arg_461_0[arg_461_1] = (expr_44E_cp_0[(int)expr_44E_cp_1].dc = expr_44E_cp_0[(int)expr_44E_cp_1].dc + num13);
							num3 += 1L;
							num7 = num12;
						}
						else
						{
							num7 = -1;
						}
						num8 = num9;
						num9 = num10;
						j++;
						num4 += 1L;
					}
				}
			}
			pipe.ncoded_fragis[pli] = num3;
			pipe.nuncoded_fragis[pli] = (long)(num2 - num) * (long)nhfrags - num3;
		}

		private static void oc_dec_frags_recon_mcu_plane(Xiph.th_dec_ctx dec, Xiph.oc_dec_pipeline_state pipe, int pli)
		{
			ushort[] oc_dec_frags_recon_mcu_plane__dc_quant = dec.oc_dec_frags_recon_mcu_plane__dc_quant;
			byte[] dct_tokens = dec.dct_tokens;
			byte[] data = dec.state.opt_data.data;
			int offset = dec.state.opt_data.offset;
			Xiph.oc_fragment[] frags = dec.state.frags;
			long[] data2 = pipe.coded_fragis[pli].data;
			int num = pipe.coded_fragis[pli].offset;
			long num2 = pipe.ncoded_fragis[pli];
			for (int i = 0; i < 2; i++)
			{
				oc_dec_frags_recon_mcu_plane__dc_quant[i] = pipe.dequant[pli][0][i][0];
			}
			for (long num3 = 0L; num3 < num2; num3 += 1L)
			{
				int num4 = 0;
				long num5 = data2[num + (int)num3];
				int i = (frags[(int)(checked((IntPtr)num5))].mb_mode != 1u) ? 1 : 0;
				ushort[] array = pipe.dequant[pli][(int)((UIntPtr)frags[(int)(checked((IntPtr)num5))].qii)][i];
				int j;
				long num9;
				for (j = 0; j < 64; j += ((num9 == 0L) ? 1 : 0))
				{
					num4 = j;
					if (pipe.eob_runs[pli][j] != 0L)
					{
						pipe.eob_runs[pli][j] -= 1L;
						break;
					}
					int num6 = (int)pipe.ti[pli][j];
					int num7 = (int)dct_tokens[num6++];
					int num8 = Xiph.OC_DCT_CODE_WORD[num7];
					if (num7 < 15)
					{
						num8 += (int)dct_tokens[num6++] << (-13 & ((num7 < 2) ? -1 : 0)) + (21 & ((num7 < 12) ? -1 : 0));
					}
					num9 = (long)(num8 >> 8 & 4095);
					if (num7 == 0)
					{
						num9 += (long)((long)dct_tokens[num6++] << 8);
						if (num9 == 0L)
						{
							num9 = 2147483647L;
						}
					}
					int num10 = (int)((byte)num8);
					num8 ^= -(num8 & 1048576);
					int num11 = num8 >> 21;
					pipe.eob_runs[pli][j] = num9;
					pipe.ti[pli][j] = (long)num6;
					j += num10;
					pipe.dct_coeffs[(int)data[offset + j]] = (short)(num11 * (int)array[j]);
				}
				int arg_215_0 = (j < 64) ? j : 64;
				pipe.dct_coeffs[0] = (short)frags[(int)(checked((IntPtr)num5))].dc;
				Xiph.oc_theora_state state = dec.state;
				short[] dct_coeffs = pipe.dct_coeffs;
				ushort num12 = oc_dec_frags_recon_mcu_plane__dc_quant[i];
				if (num4 < 2)
				{
					short num13 = (short)(dct_coeffs[0] * (short)num12 + 15 >> 5);
					for (int k = 0; k < 64; k++)
					{
						dct_coeffs[64 + k] = num13;
					}
				}
				else
				{
					dct_coeffs[0] = dct_coeffs[0] * (short)num12;
					short[] array2 = dct_coeffs;
					int num14 = 64;
					short[] array3 = dct_coeffs;
					int num15 = 0;
					short[] oc_idct8x8_w = state.oc_idct8x8_w;
					int num16 = 0;
					if (num4 <= 3)
					{
						short[] array4 = oc_idct8x8_w;
						int num17 = num16;
						short[] array5 = array3;
						int num18 = num15;
						short num19 = array5[num18];
						short num20 = array5[num18 + 1];
						int num21 = 46341 * (int)num19 >> 16;
						int num22 = 12785 * num20 >> 16;
						int num23 = 64277 * (int)num20 >> 16;
						int num24 = 46341 * num22 >> 16;
						int num25 = 46341 * num23 >> 16;
						int num26 = num25 + num24;
						num24 = num25 - num24;
						num25 = num26;
						array4[num17] = (short)(num21 + num23);
						array4[num17 + 8] = (short)(num21 + num25);
						array4[num17 + 16] = (short)(num21 + num24);
						array4[num17 + 24] = (short)(num21 + num22);
						array4[num17 + 32] = (short)(num21 - num22);
						array4[num17 + 40] = (short)(num21 - num24);
						array4[num17 + 48] = (short)(num21 - num25);
						array4[num17 + 56] = (short)(num21 - num23);
						short[] array6 = oc_idct8x8_w;
						int num27 = num16 + 1;
						short[] array7 = array3;
						int num28 = num15 + 8;
						array6[num27] = (array6[num27 + 8] = (array6[num27 + 16] = (array6[num27 + 24] = (array6[num27 + 32] = (array6[num27 + 40] = (array6[num27 + 48] = (array6[num27 + 56] = (short)(46341 * (int)array7[num28] >> 16))))))));
						short[] array8 = array2;
						short[] array9 = oc_idct8x8_w;
						for (int l = 0; l < 8; l++)
						{
							int num29 = num14 + l;
							int num30 = num16 + l * 8;
							short num31 = array9[num30];
							short num32 = array9[num30 + 1];
							int num33 = 46341 * (int)num31 >> 16;
							int num34 = 12785 * num32 >> 16;
							int num35 = 64277 * (int)num32 >> 16;
							int num36 = 46341 * num34 >> 16;
							int num37 = 46341 * num35 >> 16;
							int num38 = num37 + num36;
							num36 = num37 - num36;
							num37 = num38;
							array8[num29] = (short)(num33 + num35);
							array8[num29 + 8] = (short)(num33 + num37);
							array8[num29 + 16] = (short)(num33 + num36);
							array8[num29 + 24] = (short)(num33 + num34);
							array8[num29 + 32] = (short)(num33 - num34);
							array8[num29 + 40] = (short)(num33 - num36);
							array8[num29 + 48] = (short)(num33 - num37);
							array8[num29 + 56] = (short)(num33 - num35);
						}
						for (int l = 0; l < 64; l++)
						{
							array2[num14 + l] = (short)(array2[num14 + l] + 8 >> 4);
						}
						if (num15 != num14)
						{
							array3[num15] = (array3[num15 + 1] = (array3[num15 + 8] = 0));
						}
					}
					else if (num4 <= 10)
					{
						short[] array10 = oc_idct8x8_w;
						int num39 = num16;
						short[] array11 = array3;
						int num40 = num15;
						short num41 = array11[num40];
						short num42 = array11[num40 + 1];
						short num43 = array11[num40 + 2];
						short num44 = array11[num40 + 3];
						int num45 = 46341 * (int)num41 >> 16;
						int num46 = 25080 * num43 >> 16;
						int num47 = 60547 * (int)num43 >> 16;
						int num48 = 12785 * num42 >> 16;
						int num49 = -(36410 * (int)num44 >> 16);
						int num50 = 54491 * (int)num44 >> 16;
						int num51 = 64277 * (int)num42 >> 16;
						int num52 = num48 + num49;
						num49 = 46341 * (int)((short)(num48 - num49)) >> 16;
						num48 = num52;
						num52 = num51 + num50;
						num50 = 46341 * (int)((short)(num51 - num50)) >> 16;
						num51 = num52;
						int num53 = num45 + num46;
						num46 = num45 - num46;
						num52 = num45 + num47;
						num47 = num45 - num47;
						num45 = num52;
						num52 = num50 + num49;
						num49 = num50 - num49;
						num50 = num52;
						array10[num39] = (short)(num45 + num51);
						array10[num39 + 8] = (short)(num53 + num50);
						array10[num39 + 16] = (short)(num46 + num49);
						array10[num39 + 24] = (short)(num47 + num48);
						array10[num39 + 32] = (short)(num47 - num48);
						array10[num39 + 40] = (short)(num46 - num49);
						array10[num39 + 48] = (short)(num53 - num50);
						array10[num39 + 56] = (short)(num45 - num51);
						short[] array12 = oc_idct8x8_w;
						int num54 = num16 + 1;
						short[] array13 = array3;
						int num55 = num15 + 8;
						short num56 = array13[num55];
						short num57 = array13[num55 + 1];
						short num58 = array13[num55 + 2];
						int num59 = 46341 * (int)num56 >> 16;
						int num60 = 25080 * num58 >> 16;
						int num61 = 60547 * (int)num58 >> 16;
						int num62 = 12785 * num57 >> 16;
						int num63 = 64277 * (int)num57 >> 16;
						int num64 = 46341 * num62 >> 16;
						int num65 = 46341 * num63 >> 16;
						int num66 = num59 + num60;
						num60 = num59 - num60;
						int num67 = num59 + num61;
						num61 = num59 - num61;
						num59 = num67;
						num67 = num65 + num64;
						num64 = num65 - num64;
						num65 = num67;
						array12[num54] = (short)(num59 + num63);
						array12[num54 + 8] = (short)(num66 + num65);
						array12[num54 + 16] = (short)(num60 + num64);
						array12[num54 + 24] = (short)(num61 + num62);
						array12[num54 + 32] = (short)(num61 - num62);
						array12[num54 + 40] = (short)(num60 - num64);
						array12[num54 + 48] = (short)(num66 - num65);
						array12[num54 + 56] = (short)(num59 - num63);
						short[] array14 = oc_idct8x8_w;
						int num68 = num16 + 2;
						short[] array15 = array3;
						int num69 = num15 + 16;
						short num70 = array15[num69];
						short num71 = array15[num69 + 1];
						int num72 = 46341 * (int)num70 >> 16;
						int num73 = 12785 * num71 >> 16;
						int num74 = 64277 * (int)num71 >> 16;
						int num75 = 46341 * num73 >> 16;
						int num76 = 46341 * num74 >> 16;
						int num77 = num76 + num75;
						num75 = num76 - num75;
						num76 = num77;
						array14[num68] = (short)(num72 + num74);
						array14[num68 + 8] = (short)(num72 + num76);
						array14[num68 + 16] = (short)(num72 + num75);
						array14[num68 + 24] = (short)(num72 + num73);
						array14[num68 + 32] = (short)(num72 - num73);
						array14[num68 + 40] = (short)(num72 - num75);
						array14[num68 + 48] = (short)(num72 - num76);
						array14[num68 + 56] = (short)(num72 - num74);
						short[] array16 = oc_idct8x8_w;
						int num78 = num16 + 3;
						short[] array17 = array3;
						int num79 = num15 + 24;
						array16[num78] = (array16[num78 + 8] = (array16[num78 + 16] = (array16[num78 + 24] = (array16[num78 + 32] = (array16[num78 + 40] = (array16[num78 + 48] = (array16[num78 + 56] = (short)(46341 * (int)array17[num79] >> 16))))))));
						for (int l = 0; l < 8; l++)
						{
							short[] array18 = array2;
							int num80 = num14 + l;
							short[] array19 = oc_idct8x8_w;
							int num81 = num16 + l * 8;
							short num82 = array19[num81];
							short num83 = array19[num81 + 1];
							short num84 = array19[num81 + 2];
							short num85 = array19[num81 + 3];
							int num86 = 46341 * (int)num82 >> 16;
							int num87 = 25080 * num84 >> 16;
							int num88 = 60547 * (int)num84 >> 16;
							int num89 = 12785 * num83 >> 16;
							int num90 = -(36410 * (int)num85 >> 16);
							int num91 = 54491 * (int)num85 >> 16;
							int num92 = 64277 * (int)num83 >> 16;
							int num93 = num89 + num90;
							num90 = 46341 * (int)((short)(num89 - num90)) >> 16;
							num89 = num93;
							num93 = num92 + num91;
							num91 = 46341 * (int)((short)(num92 - num91)) >> 16;
							num92 = num93;
							int num94 = num86 + num87;
							num87 = num86 - num87;
							num93 = num86 + num88;
							num88 = num86 - num88;
							num86 = num93;
							num93 = num91 + num90;
							num90 = num91 - num90;
							num91 = num93;
							array18[num80] = (short)(num86 + num92);
							array18[num80 + 8] = (short)(num94 + num91);
							array18[num80 + 16] = (short)(num87 + num90);
							array18[num80 + 24] = (short)(num88 + num89);
							array18[num80 + 32] = (short)(num88 - num89);
							array18[num80 + 40] = (short)(num87 - num90);
							array18[num80 + 48] = (short)(num94 - num91);
							array18[num80 + 56] = (short)(num86 - num92);
						}
						for (int l = 0; l < 64; l++)
						{
							array2[num14 + l] = (short)(array2[num14 + l] + 8 >> 4);
						}
						if (num15 != num14)
						{
							array3[num15] = 0;
							array3[num15 + 1] = 0;
							array3[num15 + 2] = 0;
							array3[num15 + 3] = 0;
							array3[num15 + 8] = 0;
							array3[num15 + 9] = 0;
							array3[num15 + 10] = 0;
							array3[num15 + 16] = 0;
							array3[num15 + 17] = 0;
							array3[num15 + 24] = 0;
						}
					}
					else
					{
						for (int l = 0; l < 8; l++)
						{
							short[] array20 = oc_idct8x8_w;
							int num95 = num16 + l;
							short[] array21 = array3;
							int num96 = num15 + l * 8;
							short num97 = array21[num96];
							short num98 = array21[num96 + 1];
							short num99 = array21[num96 + 2];
							short num100 = array21[num96 + 3];
							short num101 = array21[num96 + 4];
							short num102 = array21[num96 + 5];
							short num103 = array21[num96 + 6];
							short num104 = array21[num96 + 7];
							int num105 = 46341 * (int)(num97 + num101) >> 16;
							int num106 = 46341 * (int)(num97 - num101) >> 16;
							int num107 = (25080 * num99 >> 16) - (60547 * (int)num103 >> 16);
							int num108 = (60547 * (int)num99 >> 16) + (25080 * num103 >> 16);
							int num109 = (12785 * num98 >> 16) - (64277 * (int)num104 >> 16);
							int num110 = (54491 * (int)num102 >> 16) - (36410 * (int)num100 >> 16);
							int num111 = (36410 * (int)num102 >> 16) + (54491 * (int)num100 >> 16);
							int num112 = (64277 * (int)num98 >> 16) + (12785 * num104 >> 16);
							int num113 = num109 + num110;
							num110 = 46341 * (int)((short)(num109 - num110)) >> 16;
							num109 = num113;
							num113 = num112 + num111;
							num111 = 46341 * (int)((short)(num112 - num111)) >> 16;
							num112 = num113;
							num113 = num105 + num108;
							num108 = num105 - num108;
							num105 = num113;
							num113 = num106 + num107;
							num107 = num106 - num107;
							num106 = num113;
							num113 = num111 + num110;
							num110 = num111 - num110;
							num111 = num113;
							array20[num95] = (short)(num105 + num112);
							array20[num95 + 8] = (short)(num106 + num111);
							array20[num95 + 16] = (short)(num107 + num110);
							array20[num95 + 24] = (short)(num108 + num109);
							array20[num95 + 32] = (short)(num108 - num109);
							array20[num95 + 40] = (short)(num107 - num110);
							array20[num95 + 48] = (short)(num106 - num111);
							array20[num95 + 56] = (short)(num105 - num112);
						}
						for (int l = 0; l < 8; l++)
						{
							short[] array22 = array2;
							int num114 = num14 + l;
							short[] array23 = oc_idct8x8_w;
							int num115 = num16 + l * 8;
							short num116 = array23[num115];
							short num117 = array23[num115 + 1];
							short num118 = array23[num115 + 2];
							short num119 = array23[num115 + 3];
							short num120 = array23[num115 + 4];
							short num121 = array23[num115 + 5];
							short num122 = array23[num115 + 6];
							short num123 = array23[num115 + 7];
							int num124 = 46341 * (int)(num116 + num120) >> 16;
							int num125 = 46341 * (int)(num116 - num120) >> 16;
							int num126 = (25080 * num118 >> 16) - (60547 * (int)num122 >> 16);
							int num127 = (60547 * (int)num118 >> 16) + (25080 * num122 >> 16);
							int num128 = (12785 * num117 >> 16) - (64277 * (int)num123 >> 16);
							int num129 = (54491 * (int)num121 >> 16) - (36410 * (int)num119 >> 16);
							int num130 = (36410 * (int)num121 >> 16) + (54491 * (int)num119 >> 16);
							int num131 = (64277 * (int)num117 >> 16) + (12785 * num123 >> 16);
							int num132 = num128 + num129;
							num129 = 46341 * (int)((short)(num128 - num129)) >> 16;
							num128 = num132;
							num132 = num131 + num130;
							num130 = 46341 * (int)((short)(num131 - num130)) >> 16;
							num131 = num132;
							num132 = num124 + num127;
							num127 = num124 - num127;
							num124 = num132;
							num132 = num125 + num126;
							num126 = num125 - num126;
							num125 = num132;
							num132 = num130 + num129;
							num129 = num130 - num129;
							num130 = num132;
							array22[num114] = (short)(num124 + num131);
							array22[num114 + 8] = (short)(num125 + num130);
							array22[num114 + 16] = (short)(num126 + num129);
							array22[num114 + 24] = (short)(num127 + num128);
							array22[num114 + 32] = (short)(num127 - num128);
							array22[num114 + 40] = (short)(num126 - num129);
							array22[num114 + 48] = (short)(num125 - num130);
							array22[num114 + 56] = (short)(num124 - num131);
						}
						for (int l = 0; l < 64; l++)
						{
							array2[num14 + l] = (short)(array2[num14 + l] + 8 >> 4);
						}
						if (num15 != num14)
						{
							for (int l = 0; l < 64; l++)
							{
								array3[num15 + l] = 0;
							}
						}
					}
				}
				long num133;
				int mb_mode;
				int num134;
				byte[] array24;
				checked
				{
					num133 = state.frag_buf_offs[(int)((IntPtr)num5)];
					mb_mode = (int)state.frags[(int)((IntPtr)num5)].mb_mode;
					num134 = state.ref_ystride[pli];
					array24 = state.ref_frame_data[state.ref_frame_idx[2]];
				}
				if (mb_mode == 1)
				{
					int num135 = (int)num133;
					short[] array25 = dct_coeffs;
					int num136 = 64;
					for (int m = 0; m < 8; m++)
					{
						for (int n = 0; n < 8; n++)
						{
							int num137 = (int)(array25[num136 + m * 8 + n] + 128);
							if (num137 < 0)
							{
								array24[num135 + n] = 0;
							}
							else if (num137 > 255)
							{
								array24[num135 + n] = 255;
							}
							else
							{
								array24[num135 + n] = (byte)num137;
							}
						}
						num135 += num134;
					}
				}
				else
				{
					int num138 = 0;
					int num139 = 268505377 >> mb_mode * 4 & 15;
					byte[] array26 = state.ref_frame_data[state.ref_frame_idx[num139]];
					short num140 = state.frag_mvs[(int)(checked((IntPtr)num5))];
					int num141 = state.ref_ystride[pli];
					int num142 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
					int num143 = (int)((sbyte)num140);
					int num144 = num140 >> 8;
					int num145 = (int)Xiph.OC_MVMAP[num142][num144 + 31];
					int num146 = (int)Xiph.OC_MVMAP2[num142][num144 + 31];
					int num147 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
					int num148 = (int)Xiph.OC_MVMAP[num147][num143 + 31];
					int num149 = (int)Xiph.OC_MVMAP2[num147][num143 + 31];
					int num150 = num145 * num141 + num148;
					int num151;
					int num152;
					if (num149 != 0 || num146 != 0)
					{
						num138 = num150 + num146 * num141 + num149;
						num151 = num150;
						num152 = 2;
					}
					else
					{
						num151 = num150;
						num152 = 1;
					}
					if (num152 > 1)
					{
						int num153 = (int)num133;
						byte[] array27 = array26;
						int num154 = (int)num133 + num151;
						byte[] array28 = array26;
						int num155 = (int)num133 + num138;
						short[] array29 = dct_coeffs;
						int num156 = 64;
						for (int num157 = 0; num157 < 8; num157++)
						{
							for (int num158 = 0; num158 < 8; num158++)
							{
								int num159 = (int)array29[num156 + num157 * 8 + num158] + (array27[num154 + num158] + array28[num155 + num158] >> 1);
								if (num159 < 0)
								{
									array24[num153 + num158] = 0;
								}
								else if (num159 > 255)
								{
									array24[num153 + num158] = 255;
								}
								else
								{
									array24[num153 + num158] = (byte)num159;
								}
							}
							num153 += num134;
							num154 += num134;
							num155 += num134;
						}
					}
					else
					{
						int num160 = (int)num133;
						byte[] array30 = array26;
						int num161 = (int)num133 + num151;
						short[] array31 = dct_coeffs;
						int num162 = 64;
						for (int num163 = 0; num163 < 8; num163++)
						{
							for (int num164 = 0; num164 < 8; num164++)
							{
								int num165 = (int)(array31[num162 + num163 * 8 + num164] + (short)array30[num161 + num164]);
								if (num165 < 0)
								{
									array24[num160 + num164] = 0;
								}
								else if (num165 > 255)
								{
									array24[num160 + num164] = 255;
								}
								else
								{
									array24[num160 + num164] = (byte)num165;
								}
							}
							num160 += num134;
							num161 += num134;
						}
					}
				}
			}
			num += (int)num2;
			pipe.coded_fragis[pli].offset = num;
			if (pipe.nuncoded_fragis[pli] > 0L)
			{
				long[] data3 = pipe.uncoded_fragis[pli].data;
				int num166 = pipe.uncoded_fragis[pli].offset;
				num166 -= (int)pipe.nuncoded_fragis[pli];
				pipe.uncoded_fragis[pli].offset = num166;
				Xiph.oc_frag_copy_list(dec.state, dec.state.ref_frame_data[dec.state.ref_frame_idx[2]], 0, dec.state.ref_frame_data[dec.state.ref_frame_idx[1]], 0, dec.state.ref_ystride[pli], data3, num166, pipe.nuncoded_fragis[pli], dec.state.frag_buf_offs);
			}
		}

		private static void oc_filter_hedge(Xiph.ogg_ptr dst, int dst_ystride, Xiph.ogg_ptr src, int src_ystride, int qstep, int flimit, Xiph.Ptr<int> variance0, Xiph.Ptr<int> variance1)
		{
			int[] array = new int[10];
			Xiph.ogg_ptr ogg_ptr = dst;
			Xiph.ogg_ptr ogg_ptr2 = src;
			for (int i = 0; i < 8; i++)
			{
				Xiph.ogg_ptr ptr = ogg_ptr;
				Xiph.ogg_ptr ptr2 = ogg_ptr2;
				for (int j = 0; j < 10; j++)
				{
					array[j] = (int)ptr2[0];
					ptr2 += src_ystride;
				}
				int num2;
				int num = num2 = 0;
				for (int j = 0; j < 4; j++)
				{
					num2 += Math.Abs(array[j + 1] - array[j]);
					num += Math.Abs(array[j + 5] - array[j + 6]);
				}
				variance0[0] = variance0[0] + Xiph.OC_MINI<int>(255, num2);
				variance1[0] = variance1[0] + Xiph.OC_MINI<int>(255, num);
				if (num2 < flimit && num < flimit && array[5] - array[4] < qstep && array[4] - array[5] < qstep)
				{
					ptr[0] = (byte)(array[0] * 3 + array[1] * 2 + array[2] + array[3] + array[4] + 4 >> 3);
					ptr += dst_ystride;
					ptr[0] = (byte)(array[0] * 2 + array[1] + array[2] * 2 + array[3] + array[4] + array[5] + 4 >> 3);
					ptr += dst_ystride;
					for (int j = 0; j < 4; j++)
					{
						ptr[0] = (byte)(array[j] + array[j + 1] + array[j + 2] + array[j + 3] * 2 + array[j + 4] + array[j + 5] + array[j + 6] + 4 >> 3);
						ptr += dst_ystride;
					}
					ptr[0] = (byte)(array[4] + array[5] + array[6] + array[7] * 2 + array[8] + array[9] * 2 + 4 >> 3);
					(ptr + dst_ystride)[0] = (byte)(array[5] + array[6] + array[7] + array[8] * 2 + array[9] * 3 + 4 >> 3);
				}
				else
				{
					for (int j = 1; j <= 8; j++)
					{
						ptr[0] = (byte)array[j];
						ptr += dst_ystride;
					}
				}
				ogg_ptr = ++ogg_ptr;
				ogg_ptr2 = ++ogg_ptr2;
			}
		}

		private static void oc_filter_vedge(Xiph.ogg_ptr dst, int dst_ystride, int qstep, int flimit, Xiph.Ptr<int> variances)
		{
			int[] array = new int[10];
			Xiph.ogg_ptr ogg_ptr = dst;
			for (int i = 0; i < 8; i++)
			{
				Xiph.ogg_ptr ptr = ogg_ptr - 1;
				Xiph.ogg_ptr ptr2 = ogg_ptr;
				for (int j = 0; j < 10; j++)
				{
					array[j] = (int)ptr[0];
					ptr = ++ptr;
				}
				int num2;
				int num = num2 = 0;
				for (int j = 0; j < 4; j++)
				{
					num2 += Math.Abs(array[j + 1] - array[j]);
					num += Math.Abs(array[j + 5] - array[j + 6]);
				}
				variances[0] = variances[0] + Xiph.OC_MINI<int>(255, num2);
				variances[1] = variances[1] + Xiph.OC_MINI<int>(255, num);
				if (num2 < flimit && num < flimit && array[5] - array[4] < qstep && array[4] - array[5] < qstep)
				{
					ptr2[0] = (byte)(array[0] * 3 + array[1] * 2 + array[2] + array[3] + array[4] + 4 >> 3);
					ptr2 = ++ptr2;
					ptr2[0] = (byte)(array[0] * 2 + array[1] + array[2] * 2 + array[3] + array[4] + array[5] + 4 >> 3);
					ptr2 = ++ptr2;
					for (int j = 0; j < 4; j++)
					{
						ptr2[0] = (byte)(array[j] + array[j + 1] + array[j + 2] + array[j + 3] * 2 + array[j + 4] + array[j + 5] + array[j + 6] + 4 >> 3);
						ptr2 = ++ptr2;
					}
					ptr2[0] = (byte)(array[4] + array[5] + array[6] + array[7] * 2 + array[8] + array[9] * 2 + 4 >> 3);
					(++ptr2)[0] = (byte)(array[5] + array[6] + array[7] + array[8] * 2 + array[9] * 3 + 4 >> 3);
				}
				ogg_ptr += dst_ystride;
			}
		}

		private static void oc_dec_deblock_frag_rows(Xiph.th_dec_ctx dec, Xiph.th_img_plane dstp, Xiph.th_img_plane srcp, int pli, int fragy0, int fragy_end)
		{
			Xiph.oc_fragment_plane oc_fragment_plane = dec.state.fplanes[pli];
			int nhfrags = oc_fragment_plane.nhfrags;
			long offset = oc_fragment_plane.froffset + (long)fragy0 * (long)nhfrags;
			Xiph.Ptr<int> ptr = new Xiph.Ptr<int>(dec.variances, offset);
			Xiph.Ptr<byte> ptr2 = new Xiph.Ptr<byte>(dec.dc_qis, offset);
			int num = (fragy0 > 0) ? 1 : 0;
			int num2 = (fragy_end < oc_fragment_plane.nvfrags) ? 1 : 0;
			Array.Clear(ptr.data, ptr.offset + (nhfrags & -num), (fragy_end + num2 - fragy0 - num) * nhfrags);
			int i = (fragy0 << 3) + (num << 2);
			int stride = dstp.stride;
			int stride2 = srcp.stride;
			Xiph.ogg_ptr ogg_ptr = dstp.data + (long)i * (long)stride;
			Xiph.ogg_ptr ptr3 = srcp.data + (long)i * (long)stride2;
			int width = dstp.width;
			while (i < 4)
			{
				Xiph.CopyArrays(ptr3.data, ptr3.offset, ogg_ptr.data, ogg_ptr.offset, width);
				ogg_ptr += stride;
				ptr3 += stride2;
				i++;
			}
			int num3 = fragy_end - ((num2 == 0) ? 1 : 0) << 3;
			while (i < num3)
			{
				int num4 = dec.pp_dc_scale[(int)ptr2[0]];
				int num5 = num4 * 3 >> 2;
				Xiph.oc_filter_hedge(ogg_ptr, stride, ptr3 - stride2, stride2, num4, num5, ptr, ptr + nhfrags);
				ptr = ++ptr;
				ptr2 = ++ptr2;
				for (int j = 8; j < width; j += 8)
				{
					num4 = dec.pp_dc_scale[(int)ptr2[0]];
					num5 = num4 * 3 >> 2;
					Xiph.oc_filter_hedge(ogg_ptr + j, stride, ptr3 + j - stride2, stride2, num4, num5, ptr, ptr + nhfrags);
					Xiph.oc_filter_vedge(ogg_ptr + j - (stride << 2) - 4, stride, num4, num5, ptr - 1);
					ptr = ++ptr;
					ptr2 = ++ptr2;
				}
				ogg_ptr += stride << 3;
				ptr3 += stride2 << 3;
				i += 8;
			}
			if (num2 == 0)
			{
				int height = dstp.height;
				while (i < height)
				{
					Xiph.CopyArrays(ptr3.data, ptr3.offset, ogg_ptr.data, ogg_ptr.offset, width);
					ogg_ptr += stride;
					ptr3 += stride2;
					i++;
				}
				ptr2 = ++ptr2;
				for (int j = 8; j < width; j += 8)
				{
					int num4 = dec.pp_dc_scale[(int)ptr2[0]];
					ptr2 = ++ptr2;
					int num5 = num4 * 3 >> 2;
					Xiph.ogg_ptr arg_2E3_0 = ogg_ptr + j - (stride << 3) - 4;
					int arg_2E3_1 = stride;
					int arg_2E3_2 = num4;
					int arg_2E3_3 = num5;
					Xiph.Ptr<int> expr_2DC = ptr;
					ptr = ++expr_2DC;
					Xiph.oc_filter_vedge(arg_2E3_0, arg_2E3_1, arg_2E3_2, arg_2E3_3, expr_2DC);
				}
			}
		}

		private static void oc_dering_block(Xiph.ogg_ptr idata, int ystride, int bf, int dc_scale, int sharp_mod, int strong)
		{
			int[] array = new int[72];
			int[] array2 = new int[72];
			int c = Xiph.OC_MINI<int>(3 * dc_scale, (int)Xiph.OC_MOD_MAX[strong]);
			Xiph.ogg_ptr ogg_ptr = idata;
			Xiph.ogg_ptr ogg_ptr2 = ogg_ptr;
			Xiph.ogg_ptr ptr = ogg_ptr2 - (ystride & -(((bf & 4) == 0) ? 1 : 0));
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = 32 + dc_scale - (Math.Abs((int)(ogg_ptr2[j] - ptr[j])) << (int)Xiph.OC_MOD_SHIFT[strong]);
					array[(i << 3) + j] = ((num < -64) ? sharp_mod : Xiph.OC_CLAMPI<int>(0, num, c));
				}
				ptr = ogg_ptr2;
				ogg_ptr2 += (ystride & -((((bf & 8) == 0) ? 1 : 0) | ((i < 7) ? 1 : 0)));
			}
			Xiph.ogg_ptr ogg_ptr3 = ogg_ptr;
			ptr = ogg_ptr - (((bf & 1) == 0) ? 1 : 0);
			for (int j = 0; j < 9; j++)
			{
				ogg_ptr2 = ogg_ptr3;
				for (int i = 0; i < 8; i++)
				{
					int num2 = 32 + dc_scale - (Math.Abs((int)(ogg_ptr2[0] - ptr[0])) << (int)Xiph.OC_MOD_SHIFT[strong]);
					array2[(j << 3) + i] = ((num2 < -64) ? sharp_mod : Xiph.OC_CLAMPI<int>(0, num2, c));
					ptr += ystride;
					ogg_ptr2 += ystride;
				}
				ptr = ogg_ptr3;
				ogg_ptr3 += ((((bf & 2) == 0) ? 1 : 0) | ((j < 7) ? 1 : 0));
			}
			ogg_ptr2 = ogg_ptr;
			ptr = ogg_ptr2 - (ystride & -(((bf & 4) == 0) ? 1 : 0));
			ogg_ptr3 = ogg_ptr2 + ystride;
			for (int i = 0; i < 8; i++)
			{
				int num3 = 128;
				int num4 = 64;
				int num5 = array2[i];
				num3 -= num5;
				num4 += num5 * (int)(ogg_ptr2 - (((bf & 1) == 0) ? 1 : 0))[0];
				num5 = array[i << 3];
				num3 -= num5;
				num4 += num5 * (int)ptr[0];
				num5 = array[i + 1 << 3];
				num3 -= num5;
				num4 += num5 * (int)ogg_ptr3[0];
				num5 = array2[8 + i];
				num3 -= num5;
				num4 += num5 * (int)ogg_ptr2[1];
				ogg_ptr[0] = Xiph.OC_CLAMP255(num3 * (int)ogg_ptr2[0] + num4 >> 7);
				for (int j = 1; j < 7; j++)
				{
					num3 = 128;
					num4 = 64;
					num5 = array2[(j << 3) + i];
					num3 -= num5;
					num4 += num5 * (int)ogg_ptr2[j - 1];
					num5 = array[(i << 3) + j];
					num3 -= num5;
					num4 += num5 * (int)ptr[j];
					num5 = array[(i + 1 << 3) + j];
					num3 -= num5;
					num4 += num5 * (int)ogg_ptr3[j];
					num5 = array2[(j + 1 << 3) + i];
					num3 -= num5;
					num4 += num5 * (int)ogg_ptr2[j + 1];
					ogg_ptr[j] = Xiph.OC_CLAMP255(num3 * (int)ogg_ptr2[j] + num4 >> 7);
				}
				num3 = 128;
				num4 = 64;
				num5 = array2[56 + i];
				num3 -= num5;
				num4 += num5 * (int)ogg_ptr2[6];
				num5 = array[(i << 3) + 7];
				num3 -= num5;
				num4 += num5 * (int)ptr[7];
				num5 = array[(i + 1 << 3) + 7];
				num3 -= num5;
				num4 += num5 * (int)ogg_ptr3[7];
				num5 = array2[64 + i];
				num3 -= num5;
				num4 += num5 * (int)ogg_ptr2[7 + (((bf & 2) == 0) ? 1 : 0)];
				ogg_ptr[7] = Xiph.OC_CLAMP255(num3 * (int)ogg_ptr2[7] + num4 >> 7);
				ogg_ptr += ystride;
				ptr = ogg_ptr2;
				ogg_ptr2 = ogg_ptr3;
				ogg_ptr3 += (ystride & -((((bf & 8) == 0) ? 1 : 0) | ((i < 6) ? 1 : 0)));
			}
		}

		private static void oc_dec_dering_frag_rows(Xiph.th_dec_ctx dec, Xiph.th_img_plane iplane, int pli, int fragy0, int fragy_end)
		{
			Xiph.oc_fragment_plane oc_fragment_plane = dec.state.fplanes[pli];
			int nhfrags = oc_fragment_plane.nhfrags;
			long offset = oc_fragment_plane.froffset + (long)fragy0 * (long)nhfrags;
			Xiph.Ptr<int> ptr = new Xiph.Ptr<int>(dec.variances, offset);
			Xiph.Ptr<Xiph.oc_fragment> ptr2 = new Xiph.Ptr<Xiph.oc_fragment>(dec.state.frags, offset);
			int num = (dec.pp_level >= ((pli != 0) ? 7 : 4)) ? 1 : 0;
			int num2 = (pli != 0) ? 3840 : 1920;
			int i = fragy0 << 3;
			int stride = iplane.stride;
			Xiph.ogg_ptr ptr3 = iplane.data + (long)i * (long)stride;
			int num3 = fragy_end << 3;
			int width = iplane.width;
			int height = iplane.height;
			while (i < num3)
			{
				for (int j = 0; j < width; j += 8)
				{
					int num4 = (int)dec.state.qis[(int)((UIntPtr)ptr2[0].qii)];
					int num5 = ptr[0];
					int num6 = ((j <= 0) ? 1 : 0) | ((j + 8 >= width) ? 1 : 0) << 1 | ((i <= 0) ? 1 : 0) << 2 | ((i + 8 >= height) ? 1 : 0) << 3;
					if (num != 0 && num5 > num2)
					{
						Xiph.oc_dering_block(ptr3 + j, stride, num6, dec.pp_dc_scale[num4], dec.pp_sharp_mod[num4], 1);
						if (pli != 0 || ((num6 & 1) == 0 && ptr[-1] > 3840) || ((num6 & 2) == 0 && ptr[1] > 3840) || ((num6 & 4) == 0 && ptr[-nhfrags] > 3840) || ((num6 & 8) == 0 && ptr[nhfrags] > 3840))
						{
							Xiph.oc_dering_block(ptr3 + j, stride, num6, dec.pp_dc_scale[num4], dec.pp_sharp_mod[num4], 1);
							Xiph.oc_dering_block(ptr3 + j, stride, num6, dec.pp_dc_scale[num4], dec.pp_sharp_mod[num4], 1);
						}
					}
					else if (num5 > 1536)
					{
						Xiph.oc_dering_block(ptr3 + j, stride, num6, dec.pp_dc_scale[num4], dec.pp_sharp_mod[num4], 1);
					}
					else if (num5 > 384)
					{
						Xiph.oc_dering_block(ptr3 + j, stride, num6, dec.pp_dc_scale[num4], dec.pp_sharp_mod[num4], 0);
					}
					ptr2 = ++ptr2;
					ptr = ++ptr;
				}
				ptr3 += stride << 3;
				i += 8;
			}
		}

		public static Xiph.th_dec_ctx th_decode_alloc(Xiph.th_info info, Xiph.th_setup_info setup)
		{
			if (setup == null)
			{
				return null;
			}
			Xiph.th_dec_ctx th_dec_ctx = new Xiph.th_dec_ctx();
			if (Xiph.oc_dec_init(th_dec_ctx, info, setup) < 0)
			{
				return null;
			}
			th_dec_ctx.state.curframe_num = 0L;
			return th_dec_ctx;
		}

		public static void th_decode_free(Xiph.th_dec_ctx dec)
		{
			if (dec != null)
			{
				Xiph.oc_dec_clear(dec);
			}
		}

		public static int th_decode_ctl(Xiph.th_dec_ctx dec, int req, Xiph.th_decode_ctl_opts opts)
		{
			switch (req)
			{
			case 1:
				if (dec == null || opts == null)
				{
					return -1;
				}
				opts.pplevel_max = 7;
				return 0;
			case 3:
			{
				if (dec == null || opts == null)
				{
					return -1;
				}
				int pplevel_max = opts.pplevel_max;
				if (pplevel_max < 0 || pplevel_max > 7)
				{
					return -10;
				}
				dec.pp_level = pplevel_max;
				return 0;
			}
			case 5:
			{
				if (dec == null || opts == null)
				{
					return -1;
				}
				long granpos = opts.granpos;
				if (granpos < 0L)
				{
					return -10;
				}
				dec.state.granpos = granpos;
				dec.state.keyframe_num = (granpos >> dec.state.info.keyframe_granule_shift) - (long)((ulong)dec.state.granpos_bias);
				dec.state.curframe_num = dec.state.keyframe_num + (granpos & (long)((1 << dec.state.info.keyframe_granule_shift) - 1));
				return 0;
			}
			case 7:
			{
				if (dec == null || opts == null)
				{
					return -1;
				}
				Xiph.th_stripe_callback stripe_callback = opts.stripe_callback;
				dec.stripe_cb.ctx = stripe_callback.ctx;
				dec.stripe_cb.stripe_decoded = stripe_callback.stripe_decoded;
				return 0;
			}
			}
			return -23;
		}

		private static void oc_dec_init_dummy_frame(Xiph.th_dec_ctx dec)
		{
			dec.state.ref_frame_idx[0] = 0;
			dec.state.ref_frame_idx[1] = 0;
			dec.state.ref_frame_idx[2] = 0;
			Xiph.th_info info = dec.state.info;
			int num = (int)(info.frame_width + 32u);
			int num2 = (int)(info.frame_height + 32u);
			int num3 = (num >> (int)(info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == 0) ? 1 : 0;
			int num4 = (num2 >> (int)(info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == 0) ? 1 : 0;
			uint num5 = (uint)(num * num2);
			uint num6 = (uint)(num3 * num4);
			int num7 = 0;
			while ((long)num7 < (long)((ulong)(num5 + 2u * num6)))
			{
				dec.state.ref_frame_data[0][num7] = 128;
				num7++;
			}
		}

		public static int th_decode_packetin(Xiph.th_dec_ctx dec, Xiph.ogg_packet op, ref long granpos)
		{
			if (dec == null || op == null)
			{
				return -1;
			}
			if (op.bytes == 0)
			{
				dec.state.frame_type = 1;
				dec.state.ntotal_coded_fragis = 0L;
			}
			else
			{
				Xiph.oc_pack_readinit(dec.opb, op.packet, op.bytes);
				int num = Xiph.oc_dec_frame_header_unpack(dec);
				if (num < 0)
				{
					return num;
				}
				if (dec.state.frame_type == 0)
				{
					Xiph.oc_dec_mark_all_intra(dec);
				}
				else
				{
					Xiph.oc_dec_coded_flags_unpack(dec);
				}
			}
			if (dec.state.frame_type != 0 && (dec.state.ref_frame_idx[0] < 0 || dec.state.ref_frame_idx[1] < 0))
			{
				Xiph.oc_dec_init_dummy_frame(dec);
			}
			if (dec.state.ntotal_coded_fragis <= 0L)
			{
				dec.state.granpos = (dec.state.keyframe_num + (long)((ulong)dec.state.granpos_bias) << dec.state.info.keyframe_granule_shift) + (dec.state.curframe_num - dec.state.keyframe_num);
				dec.state.curframe_num += 1L;
				granpos = dec.state.granpos;
				return 1;
			}
			Xiph.th_ycbcr_buffer th_ycbcr_buffer = default(Xiph.th_ycbcr_buffer);
			int num2 = 0;
			while (num2 == dec.state.ref_frame_idx[0] || num2 == dec.state.ref_frame_idx[1])
			{
				num2++;
			}
			dec.state.ref_frame_idx[2] = num2;
			if (dec.state.frame_type == 0)
			{
				dec.state.keyframe_num = dec.state.curframe_num;
			}
			else
			{
				Xiph.oc_dec_mb_modes_unpack(dec);
				Xiph.oc_dec_mv_unpack_and_frag_modes_fill(dec);
			}
			Xiph.oc_dec_block_qis_unpack(dec);
			Xiph.oc_dec_residual_tokens_unpack(dec);
			dec.state.granpos = (dec.state.keyframe_num + (long)((ulong)dec.state.granpos_bias) << dec.state.info.keyframe_granule_shift) + (dec.state.curframe_num - dec.state.keyframe_num);
			dec.state.curframe_num += 1L;
			granpos = dec.state.granpos;
			Xiph.oc_dec_pipeline_init(dec, dec.pipe);
			Xiph.oc_ycbcr_buffer_flip(ref th_ycbcr_buffer, ref dec.pp_frame_buf);
			int num3 = 0;
			int num4 = 1;
			int num5 = 0;
			while (num4 != 0)
			{
				int num7;
				int num6 = num7 = dec.state.fplanes[0].nvfrags;
				num4 = ((num5 + dec.pipe.mcu_nvfrags < num6) ? 1 : 0);
				for (int i = 0; i < 3; i++)
				{
					int num8 = (i != 0 && (dec.state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
					dec.pipe.fragy0[i] = num5 >> num8;
					dec.pipe.fragy_end[i] = Xiph.OC_MINI<int>(dec.state.fplanes[i].nvfrags, dec.pipe.fragy0[i] + (dec.pipe.mcu_nvfrags >> num8));
					Xiph.oc_dec_dc_unpredict_mcu_plane(dec, dec.pipe, i);
					Xiph.oc_dec_frags_recon_mcu_plane(dec, dec.pipe, i);
					int num10;
					int num9 = num10 = 0;
					if (dec.pipe.loop_filter != 0)
					{
						num10 += num3;
						num9 += num4;
						Xiph.oc_state_loop_filter_frag_rows(dec.state, dec.pipe.bounding_values, num2, i, dec.pipe.fragy0[i] - num10, dec.pipe.fragy_end[i] - num9);
					}
					Xiph.oc_state_borders_fill_rows(dec.state, num2, i, (dec.pipe.fragy0[i] - num10 << 3) - (num10 << 1), (dec.pipe.fragy_end[i] - num9 << 3) - (num9 << 1));
					int num11 = 3 * ((i != 0) ? 1 : 0);
					if (dec.pipe.pp_level >= 2 + num11)
					{
						num10 += num3;
						num9 += num4;
						Xiph.oc_dec_deblock_frag_rows(dec, dec.pp_frame_buf[i], dec.state.ref_frame_bufs[num2][i], i, dec.pipe.fragy0[i] - num10, dec.pipe.fragy_end[i] - num9);
						if (dec.pipe.pp_level >= 3 + num11)
						{
							num10 += num3;
							num9 += num4;
							Xiph.oc_dec_dering_frag_rows(dec, dec.pp_frame_buf[i], i, dec.pipe.fragy0[i] - num10, dec.pipe.fragy_end[i] - num9);
						}
					}
					else if (dec.pipe.loop_filter != 0)
					{
						num10 += num3;
						num9 += num4;
					}
					num7 = Xiph.OC_MINI<int>(num7, dec.pipe.fragy0[i] - num10 << num8);
					num6 = Xiph.OC_MINI<int>(num6, dec.pipe.fragy_end[i] - num9 << num8);
				}
				if (dec.stripe_cb.stripe_decoded != null)
				{
					dec.stripe_cb.stripe_decoded(dec.stripe_cb.ctx, ref th_ycbcr_buffer, dec.state.fplanes[0].nvfrags - num6, dec.state.fplanes[0].nvfrags - num7);
				}
				num3 = 1;
				num5 += dec.pipe.mcu_nvfrags;
			}
			for (int i = 0; i < 3; i++)
			{
				Xiph.oc_state_borders_fill_caps(dec.state, num2, i);
			}
			if (dec.state.frame_type == 0)
			{
				dec.state.ref_frame_idx[0] = (dec.state.ref_frame_idx[1] = dec.state.ref_frame_idx[2]);
			}
			else
			{
				dec.state.ref_frame_idx[1] = dec.state.ref_frame_idx[2];
			}
			return 0;
		}

		private static int oc_quant_params_unpack(Xiph.oc_pack_buf opb, Xiph.th_quant_info qinfo)
		{
			int[] array = new int[64];
			int[] array2 = new int[64];
			int num = Xiph.oc_pack_read(opb, 3);
			int bits = num;
			for (int i = 0; i < 64; i++)
			{
				num = Xiph.oc_pack_read(opb, bits);
				qinfo.loop_filter_limits[i] = (byte)num;
			}
			num = Xiph.oc_pack_read(opb, 4);
			bits = num + 1;
			for (int i = 0; i < 64; i++)
			{
				num = Xiph.oc_pack_read(opb, bits);
				qinfo.ac_scale[i] = (ushort)num;
			}
			num = Xiph.oc_pack_read(opb, 4);
			bits = num + 1;
			for (int i = 0; i < 64; i++)
			{
				num = Xiph.oc_pack_read(opb, bits);
				qinfo.dc_scale[i] = (ushort)num;
			}
			num = Xiph.oc_pack_read(opb, 9);
			int num2 = num + 1;
			Xiph.th_quant_base[] array3 = new Xiph.th_quant_base[num2];
			if (array3 == null)
			{
				return -1;
			}
			for (int j = 0; j < num2; j++)
			{
				array3[j] = new Xiph.th_quant_base();
				for (int k = 0; k < 64; k++)
				{
					num = Xiph.oc_pack_read(opb, 8);
					array3[j].data[k] = (byte)num;
				}
			}
			bits = Xiph.oc_ilog(num2 - 1);
			for (int l = 0; l < 6; l++)
			{
				int num4;
				int num3 = num4 = l / 3;
				int num6;
				int num5 = num6 = l % 3;
				Xiph.th_quant_ranges th_quant_ranges = qinfo.qi_ranges[num4, num6];
				if (l > 0 && Xiph.oc_pack_read1(opb) == 0)
				{
					int num7;
					int num8;
					if (num3 > 0)
					{
						num = Xiph.oc_pack_read1(opb);
						if (num != 0)
						{
							num7 = num3 - 1;
							num8 = num5;
						}
						else
						{
							num7 = (l - 1) / 3;
							num8 = (l - 1) % 3;
						}
					}
					else
					{
						num7 = (l - 1) / 3;
						num8 = (l - 1) % 3;
					}
					qinfo.qi_ranges[num4, num6] = qinfo.qi_ranges[num7, num8];
				}
				else
				{
					num = Xiph.oc_pack_read(opb, bits);
					array2[0] = num;
					int i;
					int num9 = i = 0;
					while (i < 63)
					{
						num = Xiph.oc_pack_read(opb, Xiph.oc_ilog(62 - i));
						array[num9] = num + 1;
						i += num + 1;
						num = Xiph.oc_pack_read(opb, bits);
						array2[++num9] = num;
					}
					if (i > 63)
					{
						return -20;
					}
					th_quant_ranges.nranges = num9;
					int[] dst = th_quant_ranges.sizes = new int[num9];
					if (th_quant_ranges.sizes == null)
					{
						return -1;
					}
					Xiph.CopyArrays(array, dst, num9);
					Xiph.th_quant_base[] array4 = new Xiph.th_quant_base[num9 + 1];
					if (array4 == null)
					{
						return -1;
					}
					th_quant_ranges.base_matrices = array4;
					qinfo.qi_ranges[num4, num6] = th_quant_ranges;
					while (true)
					{
						int j = array2[num9];
						if (j >= num2)
						{
							break;
						}
						array4[num9] = new Xiph.th_quant_base(array3[j]);
						if (num9-- <= 0)
						{
							goto IL_298;
						}
					}
					return -20;
				}
				IL_298:;
			}
			return 0;
		}

		private static void oc_quant_params_clear(Xiph.th_quant_info qinfo)
		{
			int num = 6;
			while (num-- > 0)
			{
				int num2 = num / 3;
				int num3 = num % 3;
				if (num > 0)
				{
					int num4 = (num - 1) / 3;
					int num5 = (num - 1) % 3;
					if (qinfo.qi_ranges[num2, num3].sizes == qinfo.qi_ranges[num4, num5].sizes)
					{
						qinfo.qi_ranges[num2, num3].sizes = null;
					}
					if (qinfo.qi_ranges[num2, num3].base_matrices == qinfo.qi_ranges[num4, num5].base_matrices)
					{
						qinfo.qi_ranges[num2, num3].base_matrices = null;
					}
				}
				if (num2 > 0)
				{
					if (qinfo.qi_ranges[1, num3].sizes == qinfo.qi_ranges[0, num3].sizes)
					{
						qinfo.qi_ranges[1, num3].sizes = null;
					}
					if (qinfo.qi_ranges[1, num3].base_matrices == qinfo.qi_ranges[0, num3].base_matrices)
					{
						qinfo.qi_ranges[1, num3].base_matrices = null;
					}
				}
				qinfo.qi_ranges[num2, num3].sizes = null;
				qinfo.qi_ranges[num2, num3].base_matrices = null;
			}
		}

		private static void oc_frag_copy_c(byte[] dst, int dsto, byte[] src, int srco, int ystride)
		{
			int num = 8;
			while (num-- > 0)
			{
				Xiph.CopyArrays(src, srco, dst, dsto, 8);
				dsto += ystride;
				srco += ystride;
			}
		}

		private static void oc_frag_copy_list_c(byte[] dst_frame, int dst_frameo, byte[] src_frame, int src_frameo, int ystride, long[] fragis, int fragiso, long nfragis, long[] frag_buf_offs)
		{
			for (long num = 0L; num < nfragis; num += 1L)
			{
				long num2 = frag_buf_offs[(int)(checked((IntPtr)fragis[(int)((IntPtr)(unchecked((long)fragiso + num)))]))];
				long num3 = (long)dst_frameo + num2;
				long num4 = (long)src_frameo + num2;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
				checked
				{
					dst_frame[(int)((IntPtr)num3)] = src_frame[(int)((IntPtr)num4)];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 1L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 1L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 2L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 2L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 3L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 3L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 4L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 4L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 5L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 5L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 6L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 6L)))];
					dst_frame[(int)((IntPtr)(unchecked(num3 + 7L)))] = src_frame[(int)((IntPtr)(unchecked(num4 + 7L)))];
				}
				num3 += (long)ystride;
				num4 += (long)ystride;
			}
		}

		private static void oc_frag_recon_intra_c(byte[] dst, int dsto, int ystride, short[] residue, int residueo)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (int)(residue[residueo + i * 8 + j] + 128);
					dst[dsto + j] = (byte)(((num < 0) ? 1 : -1) & (num | -((num > 255) ? 1 : 0)));
				}
				dsto += ystride;
			}
		}

		private static void oc_frag_recon_inter_c(byte[] dst, int dsto, byte[] src, int srco, int ystride, short[] residue, int residueo)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (int)(residue[residueo + i * 8 + j] + (short)src[srco + j]);
					dst[dsto + j] = (byte)(((num < 0) ? 1 : -1) & (num | -((num > 255) ? 1 : 0)));
				}
				dsto += ystride;
				srco += ystride;
			}
		}

		private static void oc_frag_recon_inter2_c(byte[] dst, int dsto, byte[] src1, int src1o, byte[] src2, int src2o, int ystride, short[] residue, int residueo)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (int)residue[residueo + i * 8 + j] + (src1[src1o + j] + src2[src2o + j] >> 1);
					dst[dsto + j] = (byte)(((num < 0) ? 1 : -1) & (num | -((num > 255) ? 1 : 0)));
				}
				dsto += ystride;
				src1o += ystride;
				src2o += ystride;
			}
		}

		private static void oc_restore_fpu_c()
		{
		}

		private static int oc_huff_tree_unpack(Xiph.oc_pack_buf opb, byte[,] tokens)
		{
			uint num = 0u;
			int num4;
			int num3;
			int num2 = num3 = (num4 = 0);
			while (true)
			{
				int num5 = Xiph.oc_pack_read1(opb);
				if (Xiph.oc_pack_bytes_left(opb) < 0)
				{
					break;
				}
				if (num5 == 0)
				{
					num3++;
					if (num3 > 32)
					{
						return -20;
					}
				}
				else
				{
					if (++num4 > 32)
					{
						return -20;
					}
					num5 = Xiph.oc_pack_read(opb, 5);
					int num6 = (int)Xiph.OC_DCT_TOKEN_MAP_LOG_NENTRIES[num5];
					int num7 = (int)Xiph.OC_DCT_TOKEN_MAP[num5];
					int num8 = 1 << num6;
					while (num8-- > 0)
					{
						tokens[num2, 0] = (byte)num7++;
						tokens[num2, 1] = (byte)(num3 + num6);
						num2++;
					}
					uint num9 = 2147483648u >> num3 - 1;
					while (num3 > 0 && (num & num9) != 0u)
					{
						num ^= num9;
						num9 <<= 1;
						num3--;
					}
					if (num3 <= 0)
					{
						return num2;
					}
					num |= num9;
				}
			}
			return -20;
		}

		private static int oc_huff_subtree_tokens(byte[,] tokens, int cursor, int depth)
		{
			uint num = 0u;
			int num2 = cursor;
			do
			{
				if ((int)tokens[num2, 1] - depth < 32)
				{
					num += 2147483648u >> ((int)tokens[num2++, 1] - depth & 31);
				}
				else
				{
					num += 1u;
					num2 += Xiph.oc_huff_subtree_tokens(tokens, num2, depth + 31);
				}
			}
			while (num < 2147483648u);
			return num2 - cursor;
		}

		private static int oc_huff_tree_collapse_depth(byte[,] tokens, int cursor, int ntokens, int depth)
		{
			int result = 1;
			int num = (depth > 0) ? 2 : 7;
			int num2 = 1;
			int num3 = 2;
			int num4 = 1;
			int num5;
			do
			{
				int i = cursor;
				if (num4 != 0)
				{
					result = num2;
				}
				num2++;
				num4 = 0;
				num5 = num3;
				num3 = 0;
				while (i < cursor + ntokens)
				{
					if ((int)tokens[i, 1] < depth + num2)
					{
						i++;
					}
					else if ((int)tokens[i, 1] == depth + num2)
					{
						num4 = 1;
						i++;
					}
					else
					{
						i += Xiph.oc_huff_subtree_tokens(tokens, i, depth + num2);
					}
					num3++;
				}
			}
			while (num3 > num5 && num3 * num >= 1 << num2);
			return result;
		}

		private static uint oc_huff_node_size(int nbits)
		{
			return 1u + (1u << nbits);
		}

		private static uint oc_huff_tree_collapse(short[] tree, byte[,] tokens, int ntokens)
		{
			short[] array = new short[34];
			byte[] array2 = new byte[34];
			byte[] array3 = new byte[34];
			array2[0] = 0;
			array3[0] = (byte)(ntokens - 1);
			uint num = 0u;
			int num2 = 0;
			int num3 = 0;
			while (true)
			{
				int num4 = Xiph.oc_huff_tree_collapse_depth(tokens, num2, (int)(array3[num3] + 1) - num2, (int)array2[num3]);
				array[num3] = (short)num;
				num += Xiph.oc_huff_node_size(num4);
				if (tree != null)
				{
					short[] expr_61_cp_0 = array;
					int expr_61_cp_1 = num3;
					short num5;
					expr_61_cp_0[expr_61_cp_1] = (num5 = expr_61_cp_0[expr_61_cp_1]) + 1;
					tree[(int)num5] = (short)num4;
				}
				while (true)
				{
					if (num2 > (int)array3[num3] || (int)tokens[num2, 1] > (int)array2[num3] + num4)
					{
						if (num2 <= (int)array3[num3])
						{
							goto Block_4;
						}
						if (num3-- > 0)
						{
							num4 = (int)(array2[num3 + 1] - array2[num3]);
						}
						if (num3 < 0)
						{
							break;
						}
					}
					else
					{
						if (tree != null)
						{
							int num6 = 1 << (int)array2[num3] + num4 - (int)tokens[num2, 1];
							short num7 = (short)(-(short)((int)(tokens[num2, 1] - array2[num3]) << 8 | (int)tokens[num2, 0]));
							while (num6-- > 0)
							{
								short[] expr_BE_cp_0 = array;
								int expr_BE_cp_1 = num3;
								short num8;
								expr_BE_cp_0[expr_BE_cp_1] = (num8 = expr_BE_cp_0[expr_BE_cp_1]) + 1;
								tree[(int)num8] = num7;
							}
						}
						num2++;
					}
				}
				IL_176:
				if (num3 < 0)
				{
					break;
				}
				continue;
				Block_4:
				array2[num3 + 1] = (byte)((int)array2[num3] + num4);
				if (tree != null)
				{
					short[] expr_123_cp_0 = array;
					int expr_123_cp_1 = num3;
					short num9;
					expr_123_cp_0[expr_123_cp_1] = (num9 = expr_123_cp_0[expr_123_cp_1]) + 1;
					tree[(int)num9] = (short)num;
				}
				num3++;
				array3[num3] = (byte)(num2 + Xiph.oc_huff_subtree_tokens(tokens, num2, (int)array2[num3]) - 1);
				goto IL_176;
			}
			return num;
		}

		private static int oc_huff_trees_unpack(Xiph.oc_pack_buf opb, ref short[][] nodes)
		{
			int result = 0;
			for (int i = 0; i < 80; i++)
			{
				byte[,] tokens = new byte[256, 2];
				int num = Xiph.oc_huff_tree_unpack(opb, tokens);
				if (num < 0)
				{
					result = num;
					break;
				}
				uint num2 = Xiph.oc_huff_tree_collapse(null, tokens, num);
				if (num2 > 32767u)
				{
					result = -23;
					break;
				}
				short[] array = new short[num2];
				if (array == null)
				{
					result = -1;
					break;
				}
				Xiph.oc_huff_tree_collapse(array, tokens, num);
				nodes[i] = array;
			}
			return result;
		}

		private static uint oc_huff_tree_size(short[] tree, int node)
		{
			int num = (int)tree[node];
			uint num2 = Xiph.oc_huff_node_size(num);
			int num3 = 1 << num;
			int num4 = 0;
			do
			{
				int num5 = (int)tree[node + num4 + 1];
				if (num5 <= 0)
				{
					num4 += 1 << num - (-num5 >> 8);
				}
				else
				{
					num2 += Xiph.oc_huff_tree_size(tree, num5);
					num4++;
				}
			}
			while (num4 < num3);
			return num2;
		}

		private static int oc_huff_trees_copy(short[][] dst, short[][] src)
		{
			uint num = 0u;
			for (int i = 0; i < 80; i++)
			{
				uint num2 = Xiph.oc_huff_tree_size(src[i], 0);
				num += num2;
				dst[i] = new short[num2];
				if (dst[i] == null)
				{
					while (i-- > 0)
					{
						dst[i] = null;
					}
					return -1;
				}
				Xiph.CopyArrays(src[i], dst[i], (int)num2);
			}
			return 0;
		}

		public static void oc_huff_trees_clear(short[][] nodes)
		{
			for (int i = 0; i < 80; i++)
			{
				nodes[i] = null;
			}
		}

		private static int oc_huff_token_decode(Xiph.oc_pack_buf opb, short[] tree)
		{
			Xiph.ogg_ptr ptr = opb.ptr;
			byte[] data = ptr.data;
			int i = ptr.offset;
			uint num = opb.window;
			Xiph.ogg_ptr stop = opb.stop;
			int offset = stop.offset;
			int num2 = opb.bits;
			int num3 = 0;
			int num4;
			while (true)
			{
				num4 = (int)tree[num3];
				if (num4 > num2)
				{
					uint num5 = (uint)(32 - num2);
					while (i < offset)
					{
						num5 -= 8u;
						num |= (uint)((uint)data[i] << (int)num5);
						i++;
						if (num5 < 8u)
						{
							IL_77:
							num2 = (int)(32u - num5);
							goto IL_7E;
						}
					}
					num5 = 3221225472u;
					goto IL_77;
				}
				IL_7E:
				long num6 = (long)((ulong)(num >> 32 - num4));
				num3 = (int)tree[(int)(checked((IntPtr)(unchecked((long)(num3 + 1) + num6))))];
				if (num3 <= 0)
				{
					break;
				}
				num <<= num4;
				num2 -= num4;
			}
			num3 = -num3;
			num4 = num3 >> 8;
			num <<= num4;
			num2 -= num4;
			opb.ptr.offset = i;
			opb.window = num;
			opb.bits = num2;
			return num3 & 255;
		}

		private static void idct8(short[] y, int yo, short[] x, int xo)
		{
			short num = x[xo];
			short num2 = x[xo + 1];
			short num3 = x[xo + 2];
			short num4 = x[xo + 3];
			short num5 = x[xo + 4];
			short num6 = x[xo + 5];
			short num7 = x[xo + 6];
			short num8 = x[xo + 7];
			int num9 = 46341 * (int)(num + num5) >> 16;
			int num10 = 46341 * (int)(num - num5) >> 16;
			int num11 = (25080 * num3 >> 16) - (60547 * (int)num7 >> 16);
			int num12 = (60547 * (int)num3 >> 16) + (25080 * num7 >> 16);
			int num13 = (12785 * num2 >> 16) - (64277 * (int)num8 >> 16);
			int num14 = (54491 * (int)num6 >> 16) - (36410 * (int)num4 >> 16);
			int num15 = (36410 * (int)num6 >> 16) + (54491 * (int)num4 >> 16);
			int num16 = (64277 * (int)num2 >> 16) + (12785 * num8 >> 16);
			int num17 = num13 + num14;
			num14 = 46341 * (int)((short)(num13 - num14)) >> 16;
			num13 = num17;
			num17 = num16 + num15;
			num15 = 46341 * (int)((short)(num16 - num15)) >> 16;
			num16 = num17;
			num17 = num9 + num12;
			num12 = num9 - num12;
			num9 = num17;
			num17 = num10 + num11;
			num11 = num10 - num11;
			num10 = num17;
			num17 = num15 + num14;
			num14 = num15 - num14;
			num15 = num17;
			y[yo] = (short)(num9 + num16);
			y[yo + 8] = (short)(num10 + num15);
			y[yo + 16] = (short)(num11 + num14);
			y[yo + 24] = (short)(num12 + num13);
			y[yo + 32] = (short)(num12 - num13);
			y[yo + 40] = (short)(num11 - num14);
			y[yo + 48] = (short)(num10 - num15);
			y[yo + 56] = (short)(num9 - num16);
		}

		private static void idct8_4(short[] y, int yo, short[] x, int xo)
		{
			short num = x[xo];
			short num2 = x[xo + 1];
			short num3 = x[xo + 2];
			short num4 = x[xo + 3];
			int num5 = 46341 * (int)num >> 16;
			int num6 = 25080 * num3 >> 16;
			int num7 = 60547 * (int)num3 >> 16;
			int num8 = 12785 * num2 >> 16;
			int num9 = -(36410 * (int)num4 >> 16);
			int num10 = 54491 * (int)num4 >> 16;
			int num11 = 64277 * (int)num2 >> 16;
			int num12 = num8 + num9;
			num9 = 46341 * (int)((short)(num8 - num9)) >> 16;
			num8 = num12;
			num12 = num11 + num10;
			num10 = 46341 * (int)((short)(num11 - num10)) >> 16;
			num11 = num12;
			int num13 = num5 + num6;
			num6 = num5 - num6;
			num12 = num5 + num7;
			num7 = num5 - num7;
			num5 = num12;
			num12 = num10 + num9;
			num9 = num10 - num9;
			num10 = num12;
			y[yo] = (short)(num5 + num11);
			y[yo + 8] = (short)(num13 + num10);
			y[yo + 16] = (short)(num6 + num9);
			y[yo + 24] = (short)(num7 + num8);
			y[yo + 32] = (short)(num7 - num8);
			y[yo + 40] = (short)(num6 - num9);
			y[yo + 48] = (short)(num13 - num10);
			y[yo + 56] = (short)(num5 - num11);
		}

		private static void idct8_3(short[] y, int yo, short[] x, int xo)
		{
			short num = x[xo];
			short num2 = x[xo + 1];
			short num3 = x[xo + 2];
			int num4 = 46341 * (int)num >> 16;
			int num5 = 25080 * num3 >> 16;
			int num6 = 60547 * (int)num3 >> 16;
			int num7 = 12785 * num2 >> 16;
			int num8 = 64277 * (int)num2 >> 16;
			int num9 = 46341 * num7 >> 16;
			int num10 = 46341 * num8 >> 16;
			int num11 = num4 + num5;
			num5 = num4 - num5;
			int num12 = num4 + num6;
			num6 = num4 - num6;
			num4 = num12;
			num12 = num10 + num9;
			num9 = num10 - num9;
			num10 = num12;
			y[yo] = (short)(num4 + num8);
			y[yo + 8] = (short)(num11 + num10);
			y[yo + 16] = (short)(num5 + num9);
			y[yo + 24] = (short)(num6 + num7);
			y[yo + 32] = (short)(num6 - num7);
			y[yo + 40] = (short)(num5 - num9);
			y[yo + 48] = (short)(num11 - num10);
			y[yo + 56] = (short)(num4 - num8);
		}

		private static void idct8_2(short[] y, int yo, short[] x, int xo)
		{
			short num = x[xo];
			short num2 = x[xo + 1];
			int num3 = 46341 * (int)num >> 16;
			int num4 = 12785 * num2 >> 16;
			int num5 = 64277 * (int)num2 >> 16;
			int num6 = 46341 * num4 >> 16;
			int num7 = 46341 * num5 >> 16;
			int num8 = num7 + num6;
			num6 = num7 - num6;
			num7 = num8;
			y[yo] = (short)(num3 + num5);
			y[yo + 8] = (short)(num3 + num7);
			y[yo + 16] = (short)(num3 + num6);
			y[yo + 24] = (short)(num3 + num4);
			y[yo + 32] = (short)(num3 - num4);
			y[yo + 40] = (short)(num3 - num6);
			y[yo + 48] = (short)(num3 - num7);
			y[yo + 56] = (short)(num3 - num5);
		}

		private static void idct8_1(short[] y, int yo, short[] x, int xo)
		{
			y[yo] = (y[yo + 8] = (y[yo + 16] = (y[yo + 24] = (y[yo + 32] = (y[yo + 40] = (y[yo + 48] = (y[yo + 56] = (short)(46341 * (int)x[xo] >> 16))))))));
		}

		private static void oc_idct8x8_3(short[] y, int yo, short[] x, int xo, short[] w)
		{
			int num = 0;
			Xiph.idct8_2(w, num, x, xo);
			Xiph.idct8_1(w, num + 1, x, xo + 8);
			for (int i = 0; i < 8; i++)
			{
				Xiph.idct8_2(y, yo + i, w, num + i * 8);
			}
			for (int i = 0; i < 64; i++)
			{
				y[yo + i] = (short)(y[yo + i] + 8 >> 4);
			}
			if (xo != yo)
			{
				x[xo] = (x[xo + 1] = (x[xo + 8] = 0));
			}
		}

		private static void oc_idct8x8_10(short[] y, int yo, short[] x, int xo, short[] w)
		{
			int num = 0;
			Xiph.idct8_4(w, num, x, xo);
			Xiph.idct8_3(w, num + 1, x, xo + 8);
			Xiph.idct8_2(w, num + 2, x, xo + 16);
			Xiph.idct8_1(w, num + 3, x, xo + 24);
			for (int i = 0; i < 8; i++)
			{
				Xiph.idct8_4(y, yo + i, w, num + i * 8);
			}
			for (int i = 0; i < 64; i++)
			{
				y[yo + i] = (short)(y[yo + i] + 8 >> 4);
			}
			if (xo != yo)
			{
				x[xo] = 0;
				x[xo + 1] = 0;
				x[xo + 2] = 0;
				x[xo + 3] = 0;
				x[xo + 8] = 0;
				x[xo + 9] = 0;
				x[xo + 10] = 0;
				x[xo + 16] = 0;
				x[xo + 17] = 0;
				x[xo + 24] = 0;
			}
		}

		private static void oc_idct8x8_slow(short[] y, int yo, short[] x, int xo, short[] w)
		{
			int num = 0;
			for (int i = 0; i < 8; i++)
			{
				Xiph.idct8(w, num + i, x, xo + i * 8);
			}
			for (int i = 0; i < 8; i++)
			{
				Xiph.idct8(y, yo + i, w, num + i * 8);
			}
			for (int i = 0; i < 64; i++)
			{
				y[yo + i] = (short)(y[yo + i] + 8 >> 4);
			}
			if (xo != yo)
			{
				for (int i = 0; i < 64; i++)
				{
					x[xo + i] = 0;
				}
			}
		}

		private static void oc_idct8x8_c(short[] y, int yo, short[] x, int xo, int last_zzi, short[] w)
		{
			if (last_zzi <= 3)
			{
				Xiph.oc_idct8x8_3(y, yo, x, xo, w);
				return;
			}
			if (last_zzi <= 10)
			{
				Xiph.oc_idct8x8_10(y, yo, x, xo, w);
				return;
			}
			Xiph.oc_idct8x8_slow(y, yo, x, xo, w);
		}

		private static void oc_idct8x8_f(short[] y, int yo, short[] x, int xo, int last_zzi, short[] w)
		{
			if (last_zzi <= 3)
			{
				int num = 0;
				int num2 = num;
				short num3 = x[xo];
				short num4 = x[xo + 1];
				int num5 = 46341 * (int)num3 >> 16;
				int num6 = 12785 * num4 >> 16;
				int num7 = 64277 * (int)num4 >> 16;
				int num8 = 46341 * num6 >> 16;
				int num9 = 46341 * num7 >> 16;
				int num10 = num9 + num8;
				num8 = num9 - num8;
				num9 = num10;
				w[num2] = (short)(num5 + num7);
				w[num2 + 8] = (short)(num5 + num9);
				w[num2 + 16] = (short)(num5 + num8);
				w[num2 + 24] = (short)(num5 + num6);
				w[num2 + 32] = (short)(num5 - num6);
				w[num2 + 40] = (short)(num5 - num8);
				w[num2 + 48] = (short)(num5 - num9);
				w[num2 + 56] = (short)(num5 - num7);
				int num11 = num + 1;
				int num12 = xo + 8;
				w[num11] = (w[num11 + 8] = (w[num11 + 16] = (w[num11 + 24] = (w[num11 + 32] = (w[num11 + 40] = (w[num11 + 48] = (w[num11 + 56] = (short)(46341 * (int)x[num12] >> 16))))))));
				for (int i = 0; i < 8; i++)
				{
					int num13 = yo + i;
					int num14 = num + i * 8;
					short num15 = w[num14];
					short num16 = w[num14 + 1];
					int num17 = 46341 * (int)num15 >> 16;
					int num18 = 12785 * num16 >> 16;
					int num19 = 64277 * (int)num16 >> 16;
					int num20 = 46341 * num18 >> 16;
					int num21 = 46341 * num19 >> 16;
					int num22 = num21 + num20;
					num20 = num21 - num20;
					num21 = num22;
					y[num13] = (short)(num17 + num19);
					y[num13 + 8] = (short)(num17 + num21);
					y[num13 + 16] = (short)(num17 + num20);
					y[num13 + 24] = (short)(num17 + num18);
					y[num13 + 32] = (short)(num17 - num18);
					y[num13 + 40] = (short)(num17 - num20);
					y[num13 + 48] = (short)(num17 - num21);
					y[num13 + 56] = (short)(num17 - num19);
				}
				for (int i = 0; i < 64; i++)
				{
					y[yo + i] = (short)(y[yo + i] + 8 >> 4);
				}
				if (xo != yo)
				{
					x[xo] = (x[xo + 1] = (x[xo + 8] = 0));
					return;
				}
			}
			else if (last_zzi <= 10)
			{
				int num23 = 0;
				int num24 = num23;
				short num25 = x[xo];
				short num26 = x[xo + 1];
				short num27 = x[xo + 2];
				short num28 = x[xo + 3];
				int num29 = 46341 * (int)num25 >> 16;
				int num30 = 25080 * num27 >> 16;
				int num31 = 60547 * (int)num27 >> 16;
				int num32 = 12785 * num26 >> 16;
				int num33 = -(36410 * (int)num28 >> 16);
				int num34 = 54491 * (int)num28 >> 16;
				int num35 = 64277 * (int)num26 >> 16;
				int num36 = num32 + num33;
				num33 = 46341 * (int)((short)(num32 - num33)) >> 16;
				num32 = num36;
				num36 = num35 + num34;
				num34 = 46341 * (int)((short)(num35 - num34)) >> 16;
				num35 = num36;
				int num37 = num29 + num30;
				num30 = num29 - num30;
				num36 = num29 + num31;
				num31 = num29 - num31;
				num29 = num36;
				num36 = num34 + num33;
				num33 = num34 - num33;
				num34 = num36;
				w[num24] = (short)(num29 + num35);
				w[num24 + 8] = (short)(num37 + num34);
				w[num24 + 16] = (short)(num30 + num33);
				w[num24 + 24] = (short)(num31 + num32);
				w[num24 + 32] = (short)(num31 - num32);
				w[num24 + 40] = (short)(num30 - num33);
				w[num24 + 48] = (short)(num37 - num34);
				w[num24 + 56] = (short)(num29 - num35);
				int num38 = num23 + 1;
				int num39 = xo + 8;
				short num40 = x[num39];
				short num41 = x[num39 + 1];
				short num42 = x[num39 + 2];
				int num43 = 46341 * (int)num40 >> 16;
				int num44 = 25080 * num42 >> 16;
				int num45 = 60547 * (int)num42 >> 16;
				int num46 = 12785 * num41 >> 16;
				int num47 = 64277 * (int)num41 >> 16;
				int num48 = 46341 * num46 >> 16;
				int num49 = 46341 * num47 >> 16;
				int num50 = num43 + num44;
				num44 = num43 - num44;
				int num51 = num43 + num45;
				num45 = num43 - num45;
				num43 = num51;
				num51 = num49 + num48;
				num48 = num49 - num48;
				num49 = num51;
				w[num38] = (short)(num43 + num47);
				w[num38 + 8] = (short)(num50 + num49);
				w[num38 + 16] = (short)(num44 + num48);
				w[num38 + 24] = (short)(num45 + num46);
				w[num38 + 32] = (short)(num45 - num46);
				w[num38 + 40] = (short)(num44 - num48);
				w[num38 + 48] = (short)(num50 - num49);
				w[num38 + 56] = (short)(num43 - num47);
				int num52 = num23 + 2;
				int num53 = xo + 16;
				short num54 = x[num53];
				short num55 = x[num53 + 1];
				int num56 = 46341 * (int)num54 >> 16;
				int num57 = 12785 * num55 >> 16;
				int num58 = 64277 * (int)num55 >> 16;
				int num59 = 46341 * num57 >> 16;
				int num60 = 46341 * num58 >> 16;
				int num61 = num60 + num59;
				num59 = num60 - num59;
				num60 = num61;
				w[num52] = (short)(num56 + num58);
				w[num52 + 8] = (short)(num56 + num60);
				w[num52 + 16] = (short)(num56 + num59);
				w[num52 + 24] = (short)(num56 + num57);
				w[num52 + 32] = (short)(num56 - num57);
				w[num52 + 40] = (short)(num56 - num59);
				w[num52 + 48] = (short)(num56 - num60);
				w[num52 + 56] = (short)(num56 - num58);
				int num62 = num23 + 3;
				int num63 = xo + 24;
				w[num62] = (w[num62 + 8] = (w[num62 + 16] = (w[num62 + 24] = (w[num62 + 32] = (w[num62 + 40] = (w[num62 + 48] = (w[num62 + 56] = (short)(46341 * (int)x[num63] >> 16))))))));
				for (int j = 0; j < 8; j++)
				{
					int num64 = yo + j;
					int num65 = num23 + j * 8;
					short num66 = w[num65];
					short num67 = w[num65 + 1];
					short num68 = w[num65 + 2];
					short num69 = w[num65 + 3];
					int num70 = 46341 * (int)num66 >> 16;
					int num71 = 25080 * num68 >> 16;
					int num72 = 60547 * (int)num68 >> 16;
					int num73 = 12785 * num67 >> 16;
					int num74 = -(36410 * (int)num69 >> 16);
					int num75 = 54491 * (int)num69 >> 16;
					int num76 = 64277 * (int)num67 >> 16;
					int num77 = num73 + num74;
					num74 = 46341 * (int)((short)(num73 - num74)) >> 16;
					num73 = num77;
					num77 = num76 + num75;
					num75 = 46341 * (int)((short)(num76 - num75)) >> 16;
					num76 = num77;
					int num78 = num70 + num71;
					num71 = num70 - num71;
					num77 = num70 + num72;
					num72 = num70 - num72;
					num70 = num77;
					num77 = num75 + num74;
					num74 = num75 - num74;
					num75 = num77;
					y[num64] = (short)(num70 + num76);
					y[num64 + 8] = (short)(num78 + num75);
					y[num64 + 16] = (short)(num71 + num74);
					y[num64 + 24] = (short)(num72 + num73);
					y[num64 + 32] = (short)(num72 - num73);
					y[num64 + 40] = (short)(num71 - num74);
					y[num64 + 48] = (short)(num78 - num75);
					y[num64 + 56] = (short)(num70 - num76);
				}
				for (int j = 0; j < 64; j++)
				{
					y[yo + j] = (short)(y[yo + j] + 8 >> 4);
				}
				if (xo != yo)
				{
					x[xo] = 0;
					x[xo + 1] = 0;
					x[xo + 2] = 0;
					x[xo + 3] = 0;
					x[xo + 8] = 0;
					x[xo + 9] = 0;
					x[xo + 10] = 0;
					x[xo + 16] = 0;
					x[xo + 17] = 0;
					x[xo + 24] = 0;
					return;
				}
			}
			else
			{
				int num79 = 0;
				for (int k = 0; k < 8; k++)
				{
					int num80 = num79 + k;
					int num81 = xo + k * 8;
					short num82 = x[num81];
					short num83 = x[num81 + 1];
					short num84 = x[num81 + 2];
					short num85 = x[num81 + 3];
					short num86 = x[num81 + 4];
					short num87 = x[num81 + 5];
					short num88 = x[num81 + 6];
					short num89 = x[num81 + 7];
					int num90 = 46341 * (int)(num82 + num86) >> 16;
					int num91 = 46341 * (int)(num82 - num86) >> 16;
					int num92 = (25080 * num84 >> 16) - (60547 * (int)num88 >> 16);
					int num93 = (60547 * (int)num84 >> 16) + (25080 * num88 >> 16);
					int num94 = (12785 * num83 >> 16) - (64277 * (int)num89 >> 16);
					int num95 = (54491 * (int)num87 >> 16) - (36410 * (int)num85 >> 16);
					int num96 = (36410 * (int)num87 >> 16) + (54491 * (int)num85 >> 16);
					int num97 = (64277 * (int)num83 >> 16) + (12785 * num89 >> 16);
					int num98 = num94 + num95;
					num95 = 46341 * (int)((short)(num94 - num95)) >> 16;
					num94 = num98;
					num98 = num97 + num96;
					num96 = 46341 * (int)((short)(num97 - num96)) >> 16;
					num97 = num98;
					num98 = num90 + num93;
					num93 = num90 - num93;
					num90 = num98;
					num98 = num91 + num92;
					num92 = num91 - num92;
					num91 = num98;
					num98 = num96 + num95;
					num95 = num96 - num95;
					num96 = num98;
					w[num80] = (short)(num90 + num97);
					w[num80 + 8] = (short)(num91 + num96);
					w[num80 + 16] = (short)(num92 + num95);
					w[num80 + 24] = (short)(num93 + num94);
					w[num80 + 32] = (short)(num93 - num94);
					w[num80 + 40] = (short)(num92 - num95);
					w[num80 + 48] = (short)(num91 - num96);
					w[num80 + 56] = (short)(num90 - num97);
				}
				for (int k = 0; k < 8; k++)
				{
					int num99 = yo + k;
					int num100 = num79 + k * 8;
					short num101 = w[num100];
					short num102 = w[num100 + 1];
					short num103 = w[num100 + 2];
					short num104 = w[num100 + 3];
					short num105 = w[num100 + 4];
					short num106 = w[num100 + 5];
					short num107 = w[num100 + 6];
					short num108 = w[num100 + 7];
					int num109 = 46341 * (int)(num101 + num105) >> 16;
					int num110 = 46341 * (int)(num101 - num105) >> 16;
					int num111 = (25080 * num103 >> 16) - (60547 * (int)num107 >> 16);
					int num112 = (60547 * (int)num103 >> 16) + (25080 * num107 >> 16);
					int num113 = (12785 * num102 >> 16) - (64277 * (int)num108 >> 16);
					int num114 = (54491 * (int)num106 >> 16) - (36410 * (int)num104 >> 16);
					int num115 = (36410 * (int)num106 >> 16) + (54491 * (int)num104 >> 16);
					int num116 = (64277 * (int)num102 >> 16) + (12785 * num108 >> 16);
					int num117 = num113 + num114;
					num114 = 46341 * (int)((short)(num113 - num114)) >> 16;
					num113 = num117;
					num117 = num116 + num115;
					num115 = 46341 * (int)((short)(num116 - num115)) >> 16;
					num116 = num117;
					num117 = num109 + num112;
					num112 = num109 - num112;
					num109 = num117;
					num117 = num110 + num111;
					num111 = num110 - num111;
					num110 = num117;
					num117 = num115 + num114;
					num114 = num115 - num114;
					num115 = num117;
					y[num99] = (short)(num109 + num116);
					y[num99 + 8] = (short)(num110 + num115);
					y[num99 + 16] = (short)(num111 + num114);
					y[num99 + 24] = (short)(num112 + num113);
					y[num99 + 32] = (short)(num112 - num113);
					y[num99 + 40] = (short)(num111 - num114);
					y[num99 + 48] = (short)(num110 - num115);
					y[num99 + 56] = (short)(num109 - num116);
				}
				for (int k = 0; k < 64; k++)
				{
					y[yo + k] = (short)(y[yo + k] + 8 >> 4);
				}
				if (xo != yo)
				{
					for (int k = 0; k < 64; k++)
					{
						x[xo + k] = 0;
					}
				}
			}
		}

		public static void th_info_init(ref Xiph.th_info info)
		{
			info.version_major = 3;
			info.version_minor = 2;
			info.version_subminor = 1;
			info.keyframe_granule_shift = 6;
		}

		public static void th_info_clear(ref Xiph.th_info info)
		{
			info.version_major = 0;
			info.version_minor = 0;
			info.version_subminor = 0;
			info.frame_width = 0u;
			info.frame_height = 0u;
			info.pic_width = 0u;
			info.pic_height = 0u;
			info.pic_x = 0u;
			info.pic_y = 0u;
			info.fps_numerator = 0u;
			info.fps_denominator = 0u;
			info.aspect_numerator = 0u;
			info.aspect_denominator = 0u;
			info.colorspace = Xiph.th_colorspace.TH_CS_UNSPECIFIED;
			info.pixel_fmt = Xiph.th_pixel_fmt.TH_PF_420;
			info.target_bitrate = 0;
			info.quality = 0;
			info.keyframe_granule_shift = 0;
		}

		public static void th_comment_init(Xiph.th_comment tc)
		{
			tc.user_comments = null;
			tc.vendor = null;
		}

		public static void th_comment_clear(Xiph.th_comment tc)
		{
			if (tc != null)
			{
				tc.user_comments = null;
				tc.vendor = null;
			}
		}

		public static bool TH_VERSION_CHECK(Xiph.th_info info, byte maj, byte min, byte sub)
		{
			return info.version_major > maj || (info.version_major == maj && (info.version_minor > min || (info.version_minor == min && info.version_subminor >= sub)));
		}

		private static int oc_ilog(uint v)
		{
			int num = 0;
			while (v != 0u)
			{
				v >>= 1;
				num++;
			}
			return num;
		}

		private static int oc_ilog(int v)
		{
			int num = 0;
			while (v != 0)
			{
				v >>= 1;
				num++;
			}
			return num;
		}

		private static void oc_ycbcr_buffer_flip(ref Xiph.th_ycbcr_buffer dst, ref Xiph.th_ycbcr_buffer src)
		{
			Xiph.os_img_plane_flip(ref dst.y, ref src.y);
			Xiph.os_img_plane_flip(ref dst.cb, ref src.cb);
			Xiph.os_img_plane_flip(ref dst.cr, ref src.cr);
		}

		private static void os_img_plane_flip(ref Xiph.th_img_plane dst, ref Xiph.th_img_plane src)
		{
			dst.width = src.width;
			dst.height = src.height;
			dst.stride = -src.stride;
			dst.data.data = src.data.data;
			dst.data.offset = src.data.offset + (1 - dst.height) * dst.stride;
		}

		private static string th_version_string()
		{
			return "OrangeTree MonoMedia 1.0.0alpha 20120928 (Kuzya)";
		}

		private static uint th_version_number()
		{
			return 197121u;
		}

		private static int th_packet_isheader(Xiph.ogg_packet op)
		{
			if (op.bytes <= 0)
			{
				return 0;
			}
			return op.packet[0] >> 7;
		}

		private static int th_packet_iskeyframe(Xiph.ogg_packet op)
		{
			if (op.bytes <= 0)
			{
				return 0;
			}
			if ((op.packet[0] & 128) != 0)
			{
				return -1;
			}
			if ((op.packet[0] & 64) == 0)
			{
				return 1;
			}
			return 0;
		}

		private static T OC_MAXI<T>(T a, T b) where T : IComparable<T>
		{
			if (a.CompareTo(b) <= 0)
			{
				return b;
			}
			return a;
		}

		private static T OC_MINI<T>(T a, T b) where T : IComparable<T>
		{
			if (a.CompareTo(b) <= 0)
			{
				return a;
			}
			return b;
		}

		private static T OC_CLAMPI<T>(T a, T b, T c) where T : IComparable<T>
		{
			return Xiph.OC_MAXI<T>(a, Xiph.OC_MINI<T>(b, c));
		}

		private static byte OC_CLAMP255(int x)
		{
			return (byte)(((x < 0) ? 1 : -1) & (x | -((x > 255) ? 1 : 0)));
		}

		private static int OC_SIGNMASK(int a)
		{
			return -((a < 0) ? 1 : 0);
		}

		private static int OC_DIV_ROUND_POW2(int dividend, int shift, int rval)
		{
			return dividend + Xiph.OC_SIGNMASK(dividend) + rval >> shift;
		}

		private static int OC_UNIBBLE_TABLE32(int a, int b, int c, int d, int e, int f, int g, int h, int i)
		{
			return ((a & 15) | (b & 15) << 4 | (c & 15) << 8 | (d & 15) << 12 | (e & 15) << 16 | (f & 15) << 20 | (g & 15) << 24 | (h & 15) << 28) >> i * 4 & 15;
		}

		private static void oc_dequant_tables_init(ushort[][][][] dequant, int[] pp_dc_scale, Xiph.th_quant_info qinfo)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					int num = 0;
					for (int k = 0; k <= qinfo.qi_ranges[i, j].nranges; k++)
					{
						Xiph.th_quant_base th_quant_base = new Xiph.th_quant_base(qinfo.qi_ranges[i, j].base_matrices[k]);
						int num2 = num;
						int num3;
						if (k == qinfo.qi_ranges[i, j].nranges)
						{
							num3 = num + 1;
						}
						else
						{
							num3 = num + qinfo.qi_ranges[i, j].sizes[k];
						}
						while (true)
						{
							uint num4 = (uint)(qinfo.dc_scale[num] * (ushort)th_quant_base[0]);
							if (pp_dc_scale != null)
							{
								pp_dc_scale[num] = (int)(num4 / 160u);
							}
							uint num5 = num4 / 100u << 2;
							num5 = Xiph.OC_CLAMPI<uint>(Xiph.OC_DC_QUANT_MIN[i], num5, 4096u);
							dequant[num][j][i][0] = (ushort)num5;
							for (int l = 1; l < 64; l++)
							{
								num5 = (uint)((uint)(qinfo.ac_scale[num] * (ushort)th_quant_base[(int)Xiph.OC_FZIG_ZAG[l]] / 100) << 2);
								num5 = Xiph.OC_CLAMPI<uint>(Xiph.OC_AC_QUANT_MIN[i], num5, 4096u);
								dequant[num][j][i][l] = (ushort)num5;
							}
							int num6;
							int m = num6 = 0;
							int n;
							for (n = 0; n <= i; n++)
							{
								for (m = 0; m < ((n < i) ? 3 : j); m++)
								{
									if (Xiph.ArraysEqual<ushort>(dequant[num][j][i], dequant[num][m][n]))
									{
										num6 = 1;
										break;
									}
								}
								if (num6 != 0)
								{
									break;
								}
							}
							if (num6 != 0)
							{
								dequant[num][j][i] = dequant[num][m][n];
							}
							if (++num >= num3)
							{
								break;
							}
							for (int num7 = 0; num7 < 64; num7++)
							{
								th_quant_base[num7] = (byte)((2 * ((num3 - num) * (int)qinfo.qi_ranges[i, j].base_matrices[k][num7] + (num - num2) * (int)qinfo.qi_ranges[i, j].base_matrices[k + 1][num7]) + qinfo.qi_ranges[i, j].sizes[k]) / (2 * qinfo.qi_ranges[i, j].sizes[k]));
							}
						}
					}
				}
			}
		}

		private static void oc_frag_copy_list(Xiph.oc_theora_state state, byte[] dst_frame, int dst_frameo, byte[] src_frame, int src_frameo, int ystride, long[] fragis, int fragiso, long nfragis, long[] frag_buf_offs)
		{
			Xiph.oc_frag_copy_list_c(dst_frame, dst_frameo, src_frame, src_frameo, ystride, fragis, fragiso, nfragis, frag_buf_offs);
		}

		private static void oc_frag_recon_intra(Xiph.oc_theora_state state, byte[] dst, int dsto, int ystride, short[] residue, int residueo)
		{
			Xiph.oc_frag_recon_intra_c(dst, dsto, ystride, residue, residueo);
		}

		private static void oc_frag_recon_inter(Xiph.oc_theora_state state, byte[] dst, int dsto, byte[] src, int srco, int ystride, short[] residue, int residueo)
		{
			Xiph.oc_frag_recon_inter_c(dst, dsto, src, srco, ystride, residue, residueo);
		}

		private static void oc_frag_recon_inter2(Xiph.oc_theora_state state, byte[] dst, int dsto, byte[] src1, int src1o, byte[] src2, int src2o, int ystride, short[] residue, int residueo)
		{
			Xiph.oc_frag_recon_inter2_c(dst, dsto, src1, src1o, src2, src2o, ystride, residue, residueo);
		}

		private static void oc_idct8x8(Xiph.oc_theora_state state, short[] y, int yo, short[] x, int xo, int last_zzi)
		{
			Xiph.oc_idct8x8_c(y, yo, x, xo, last_zzi, state.oc_idct8x8_w);
		}

		private static void oc_loop_filter_init(Xiph.oc_theora_state state, sbyte[] bv, int flimit)
		{
			Xiph.oc_loop_filter_init_c(bv, flimit);
		}

		private static void oc_state_loop_filter_frag_rows(Xiph.oc_theora_state state, sbyte[] bv, int refi, int pli, int fragy0, int fragy_end)
		{
			Xiph.oc_state_loop_filter_frag_rows_c(state, bv, refi, pli, fragy0, fragy_end);
		}

		private static void oc_state_loop_filter_frag_rows(Xiph.oc_theora_state state)
		{
		}

		private static int OC_FRAME_FOR_MODE(int x)
		{
			return Xiph.OC_UNIBBLE_TABLE32(1, 2, 1, 1, 1, 0, 0, 1, x);
		}

		private static short OC_MV(int x, int y)
		{
			return (short)((x & 255) | y << 8);
		}

		private static sbyte OC_MV_X(short mv)
		{
			return (sbyte)mv;
		}

		private static int OC_MV_Y(short mv)
		{
			return mv >> 8;
		}

		private static void oc_set_chroma_mvs00(short[] cbmvs, short[] lbmvs)
		{
			int dividend = (int)(Xiph.OC_MV_X(lbmvs[0]) + Xiph.OC_MV_X(lbmvs[1]) + Xiph.OC_MV_X(lbmvs[2]) + Xiph.OC_MV_X(lbmvs[3]));
			int dividend2 = Xiph.OC_MV_Y(lbmvs[0]) + Xiph.OC_MV_Y(lbmvs[1]) + Xiph.OC_MV_Y(lbmvs[2]) + Xiph.OC_MV_Y(lbmvs[3]);
			cbmvs[0] = Xiph.OC_MV(Xiph.OC_DIV_ROUND_POW2(dividend, 2, 2), Xiph.OC_DIV_ROUND_POW2(dividend2, 2, 2));
		}

		private static void oc_set_chroma_mvs01(short[] cbmvs, short[] lbmvs)
		{
			int dividend = (int)(Xiph.OC_MV_X(lbmvs[0]) + Xiph.OC_MV_X(lbmvs[2]));
			int dividend2 = Xiph.OC_MV_Y(lbmvs[0]) + Xiph.OC_MV_Y(lbmvs[2]);
			cbmvs[0] = Xiph.OC_MV(Xiph.OC_DIV_ROUND_POW2(dividend, 1, 1), Xiph.OC_DIV_ROUND_POW2(dividend2, 1, 1));
			dividend = (int)(Xiph.OC_MV_X(lbmvs[1]) + Xiph.OC_MV_X(lbmvs[3]));
			dividend2 = Xiph.OC_MV_Y(lbmvs[1]) + Xiph.OC_MV_Y(lbmvs[3]);
			cbmvs[1] = Xiph.OC_MV(Xiph.OC_DIV_ROUND_POW2(dividend, 1, 1), Xiph.OC_DIV_ROUND_POW2(dividend2, 1, 1));
		}

		private static void oc_set_chroma_mvs10(short[] cbmvs, short[] lbmvs)
		{
			int dividend = (int)(Xiph.OC_MV_X(lbmvs[0]) + Xiph.OC_MV_X(lbmvs[1]));
			int dividend2 = Xiph.OC_MV_Y(lbmvs[0]) + Xiph.OC_MV_Y(lbmvs[1]);
			cbmvs[0] = Xiph.OC_MV(Xiph.OC_DIV_ROUND_POW2(dividend, 1, 1), Xiph.OC_DIV_ROUND_POW2(dividend2, 1, 1));
			dividend = (int)(Xiph.OC_MV_X(lbmvs[2]) + Xiph.OC_MV_X(lbmvs[3]));
			dividend2 = Xiph.OC_MV_Y(lbmvs[2]) + Xiph.OC_MV_Y(lbmvs[3]);
			cbmvs[2] = Xiph.OC_MV(Xiph.OC_DIV_ROUND_POW2(dividend, 1, 1), Xiph.OC_DIV_ROUND_POW2(dividend2, 1, 1));
		}

		private static void oc_set_chroma_mvs11(short[] cbmvs, short[] lbmvs)
		{
			cbmvs[0] = lbmvs[0];
			cbmvs[1] = lbmvs[1];
			cbmvs[2] = lbmvs[2];
			cbmvs[3] = lbmvs[3];
		}

		private static long oc_sb_quad_top_left_frag(long[][][] sb_map, uint sbi, int quadi)
		{
			return sb_map[(int)((UIntPtr)sbi)][quadi][quadi & quadi << 1];
		}

		private static void oc_sb_create_plane_mapping(long[][][] sb_maps, uint sbmoffset, Xiph.oc_sb_flags[] sb_flags, uint sbfoffset, long frag0, int hfrags, int vfrags)
		{
			uint num = 0u;
			long num2 = frag0;
			int num3 = 0;
			while (true)
			{
				int num4 = vfrags - num3;
				if (num4 > 4)
				{
					num4 = 4;
				}
				else if (num4 <= 0)
				{
					break;
				}
				int num5 = 0;
				while (true)
				{
					uint num6 = num + sbmoffset;
					int num7 = hfrags - num5;
					if (num7 > 4)
					{
						num7 = 4;
					}
					else if (num7 <= 0)
					{
						break;
					}
					sb_maps[(int)((UIntPtr)num6)][0][0] = (sb_maps[(int)((UIntPtr)num6)][0][1] = (sb_maps[(int)((UIntPtr)num6)][0][2] = (sb_maps[(int)((UIntPtr)num6)][0][3] = -1L)));
					sb_maps[(int)((UIntPtr)num6)][1][0] = (sb_maps[(int)((UIntPtr)num6)][1][1] = (sb_maps[(int)((UIntPtr)num6)][1][2] = (sb_maps[(int)((UIntPtr)num6)][1][3] = -1L)));
					sb_maps[(int)((UIntPtr)num6)][2][0] = (sb_maps[(int)((UIntPtr)num6)][2][1] = (sb_maps[(int)((UIntPtr)num6)][2][2] = (sb_maps[(int)((UIntPtr)num6)][2][3] = -1L)));
					sb_maps[(int)((UIntPtr)num6)][3][0] = (sb_maps[(int)((UIntPtr)num6)][3][1] = (sb_maps[(int)((UIntPtr)num6)][3][2] = (sb_maps[(int)((UIntPtr)num6)][3][3] = -1L)));
					long num8 = num2 + (long)num5;
					for (int i = 0; i < num4; i++)
					{
						for (int j = 0; j < num7; j++)
						{
							sb_maps[(int)((UIntPtr)num6)][Xiph.SB_MAP[i, j, 0]][Xiph.SB_MAP[i, j, 1]] = num8 + (long)j;
						}
						num8 += (long)hfrags;
					}
					for (int k = 0; k < 4; k++)
					{
						UIntPtr expr_178_cp_1 = (UIntPtr)(num + sbfoffset);
						sb_flags[(int)expr_178_cp_1].quad_valid = (sb_flags[(int)expr_178_cp_1].quad_valid | (byte)(((Xiph.oc_sb_quad_top_left_frag(sb_maps, num6, k) >= 0L) ? 1 : 0) << (k & 31)));
					}
					num5 += 4;
					num += 1u;
				}
				num2 += (long)((long)hfrags << 2);
				num3 += 4;
			}
		}

		private static void oc_mb_fill_ymapping(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane fplane, uint xfrag0, uint yfrag0)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					mb_map[(int)((UIntPtr)mbi)][0][i << 1 | j] = (long)(((ulong)yfrag0 + (ulong)((long)i)) * (ulong)((long)fplane.nhfrags) + (ulong)xfrag0 + (ulong)((long)j));
				}
			}
		}

		private static void oc_mb_fill_cmapping00(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane[] fplanes, uint xfrag0, uint yfrag0)
		{
			xfrag0 >>= 1;
			yfrag0 >>= 1;
			long num = (long)((ulong)yfrag0 * (ulong)((long)fplanes[1].nhfrags) + (ulong)xfrag0);
			mb_map[(int)((UIntPtr)mbi)][1][0] = num + fplanes[1].froffset;
			mb_map[(int)((UIntPtr)mbi)][2][0] = num + fplanes[2].froffset;
		}

		private static void oc_mb_fill_cmapping01(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane[] fplanes, uint xfrag0, uint yfrag0)
		{
			yfrag0 >>= 1;
			long num = (long)((ulong)yfrag0 * (ulong)((long)fplanes[1].nhfrags) + (ulong)xfrag0);
			for (int i = 0; i < 2; i++)
			{
				mb_map[(int)((UIntPtr)mbi)][1][i] = num + fplanes[1].froffset;
				mb_map[(int)((UIntPtr)mbi)][2][i] = num + fplanes[2].froffset;
				num += 1L;
			}
		}

		private static void oc_mb_fill_cmapping10(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane[] fplanes, uint xfrag0, uint yfrag0)
		{
			xfrag0 >>= 1;
			long num = (long)((ulong)yfrag0 * (ulong)((long)fplanes[1].nhfrags) + (ulong)xfrag0);
			for (int i = 0; i < 2; i++)
			{
				mb_map[(int)((UIntPtr)mbi)][1][i << 1] = num + fplanes[1].froffset;
				mb_map[(int)((UIntPtr)mbi)][2][i << 1] = num + fplanes[2].froffset;
				num += (long)fplanes[1].nhfrags;
			}
		}

		private static void oc_mb_fill_cmapping11(long[][][] mb_map, uint mbi, Xiph.oc_fragment_plane[] fplanes, uint xfrag0, uint yfrag0)
		{
			for (int i = 0; i < 4; i++)
			{
				mb_map[(int)((UIntPtr)mbi)][1][i] = mb_map[(int)((UIntPtr)mbi)][0][i] + fplanes[1].froffset;
				mb_map[(int)((UIntPtr)mbi)][2][i] = mb_map[(int)((UIntPtr)mbi)][0][i] + fplanes[2].froffset;
			}
		}

		private static void oc_mb_create_mapping(long[][][] mb_maps, sbyte[] mb_modes, Xiph.oc_fragment_plane[] fplanes, Xiph.th_pixel_fmt pixel_fmt)
		{
			Xiph.oc_mb_fill_cmapping_func oc_mb_fill_cmapping_func = Xiph.OC_MB_FILL_CMAPPING_TABLE[(int)pixel_fmt];
			uint num2;
			uint num = num2 = 0u;
			while ((ulong)num < (ulong)((long)fplanes[0].nvfrags))
			{
				uint num3 = 0u;
				while ((ulong)num3 < (ulong)((long)fplanes[0].nhfrags))
				{
					for (uint num4 = 0u; num4 < 2u; num4 += 1u)
					{
						for (uint num5 = 0u; num5 < 2u; num5 += 1u)
						{
							uint num6 = num2 << 2 | (uint)Xiph.OC_MB_MAP[(int)((UIntPtr)num4), (int)((UIntPtr)num5)];
							uint num7 = num3 | num5 << 1;
							uint num8 = num | num4 << 1;
							mb_maps[(int)((UIntPtr)num6)][0][0] = (mb_maps[(int)((UIntPtr)num6)][0][1] = (mb_maps[(int)((UIntPtr)num6)][0][2] = (mb_maps[(int)((UIntPtr)num6)][0][3] = -1L)));
							mb_maps[(int)((UIntPtr)num6)][1][0] = (mb_maps[(int)((UIntPtr)num6)][1][1] = (mb_maps[(int)((UIntPtr)num6)][1][2] = (mb_maps[(int)((UIntPtr)num6)][1][3] = -1L)));
							mb_maps[(int)((UIntPtr)num6)][2][0] = (mb_maps[(int)((UIntPtr)num6)][2][1] = (mb_maps[(int)((UIntPtr)num6)][2][2] = (mb_maps[(int)((UIntPtr)num6)][2][3] = -1L)));
							if ((ulong)num7 >= (ulong)((long)fplanes[0].nhfrags) || (ulong)num8 >= (ulong)((long)fplanes[0].nvfrags))
							{
								mb_modes[(int)((UIntPtr)num6)] = -1;
							}
							else
							{
								Xiph.oc_mb_fill_ymapping(mb_maps, num6, fplanes[0], num7, num8);
								oc_mb_fill_cmapping_func(mb_maps, num6, fplanes, num7, num8);
							}
						}
					}
					num3 += 4u;
					num2 += 1u;
				}
				num += 4u;
			}
		}

		private static void oc_state_border_init(Xiph.oc_theora_state state)
		{
			state.nborders = 0;
			Xiph.Ptr<Xiph.oc_fragment> r = new Xiph.Ptr<Xiph.oc_fragment>(state.frags, 0);
			Xiph.Ptr<Xiph.oc_fragment> ptr = new Xiph.Ptr<Xiph.oc_fragment>(state.frags, 0);
			for (int i = 0; i < 3; i++)
			{
				Xiph.Ptr<Xiph.oc_fragment_plane> ptr2 = new Xiph.Ptr<Xiph.oc_fragment_plane>(state.fplanes, i);
				uint num = state.info.pic_x;
				uint num2 = state.info.pic_x + state.info.pic_width;
				uint num3 = state.info.pic_y;
				uint num4 = state.info.pic_y + state.info.pic_height;
				if (i > 0)
				{
					if ((state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420)
					{
						num >>= 1;
						num2 = num2 + 1u >> 1;
					}
					if ((state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420)
					{
						num3 >>= 1;
						num4 = num4 + 1u >> 1;
					}
				}
				int num5 = 0;
				r.offset += (int)ptr2[0].nfrags;
				while (ptr < r)
				{
					int num6 = 0;
					Xiph.Ptr<Xiph.oc_fragment> r2 = ptr + ptr2[0].nhfrags;
					while (ptr < r2)
					{
						if ((long)(num6 + 8) <= (long)((ulong)num) || (ulong)num2 <= (ulong)((long)num6) || (long)(num5 + 8) <= (long)((ulong)num3) || (ulong)num4 <= (ulong)((long)num5) || num >= num2 || num3 >= num4)
						{
							ptr.data[ptr.offset].invalid = 1u;
						}
						else
						{
							if (((long)num6 < (long)((ulong)num) && (ulong)num < (ulong)((long)(num6 + 8))) || ((long)num6 < (long)((ulong)num2) && (ulong)num2 < (ulong)((long)(num6 + 8))) || ((long)num5 < (long)((ulong)num3) && (ulong)num3 < (ulong)((long)(num5 + 8))) || ((long)num5 < (long)((ulong)num4) && (ulong)num4 < (ulong)((long)(num5 + 8))))
							{
								int num8;
								long num7 = (long)(num8 = 0);
								int j;
								for (j = 0; j < 8; j++)
								{
									for (int k = 0; k < 8; k++)
									{
										if ((long)(num6 + k) >= (long)((ulong)num) && (long)(num6 + k) < (long)((ulong)num2) && (long)(num5 + j) >= (long)((ulong)num3) && (long)(num5 + j) < (long)((ulong)num4))
										{
											num7 |= 1L << (j << 3 | k);
											num8++;
										}
									}
								}
								for (j = 0; j < state.nborders; j++)
								{
									if (state.borders[j].mask == num7)
									{
										IL_267:
										ptr.data[ptr.offset].borderi = j;
										goto IL_2A4;
									}
								}
								state.nborders++;
								state.borders[j].mask = num7;
								state.borders[j].npixels = num8;
								goto IL_267;
							}
							ptr.data[ptr.offset].borderi = -1;
						}
						IL_2A4:
						ptr = ++ptr;
						num6 += 8;
					}
					num5 += 8;
				}
			}
		}

		private static int oc_state_frarray_init(Xiph.oc_theora_state state)
		{
			int num = (int)(state.info.frame_width >> 3);
			int num2 = (int)(state.info.frame_height >> 3);
			int num3 = ((state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num4 = ((state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num5 = num + num3 >> num3;
			int num6 = num2 + num4 >> num4;
			long num7 = (long)num * (long)num2;
			long num8 = (long)num5 * (long)num6;
			long num9 = num7 + 2L * num8;
			uint num10 = (uint)(num + 3 >> 2);
			uint num11 = (uint)(num2 + 3 >> 2);
			uint num12 = (uint)(num5 + 3 >> 2);
			uint num13 = (uint)(num6 + 3 >> 2);
			uint num14 = num10 * num11;
			uint num15 = num12 * num13;
			uint num16 = num14 + 2u * num15;
			uint num17 = num14 << 2;
			if (num7 / (long)num != (long)num2 || 2L * num8 < num8 || num9 < num7 || num14 / num10 != num11 || 2u * num15 < num15 || num16 < num14 || num17 >> 2 != num14)
			{
				return -23;
			}
			state.fplanes[0].nhfrags = num;
			state.fplanes[0].nvfrags = num2;
			state.fplanes[0].froffset = 0L;
			state.fplanes[0].nfrags = num7;
			state.fplanes[0].nhsbs = num10;
			state.fplanes[0].nvsbs = num11;
			state.fplanes[0].sboffset = 0u;
			state.fplanes[0].nsbs = num14;
			state.fplanes[1].nhfrags = (state.fplanes[2].nhfrags = num5);
			state.fplanes[1].nvfrags = (state.fplanes[2].nvfrags = num6);
			state.fplanes[1].froffset = num7;
			state.fplanes[2].froffset = num7 + num8;
			state.fplanes[1].nfrags = (state.fplanes[2].nfrags = num8);
			state.fplanes[1].nhsbs = (state.fplanes[2].nhsbs = num12);
			state.fplanes[1].nvsbs = (state.fplanes[2].nvsbs = num13);
			state.fplanes[1].sboffset = num14;
			state.fplanes[2].sboffset = num14 + num15;
			state.fplanes[1].nsbs = (state.fplanes[2].nsbs = num15);
			state.nfrags = num9;
			state.frags = new Xiph.oc_fragment[num9];
			state.frag_mvs = new short[num9];
			state.nsbs = num16;
			state.sb_maps = new long[num16][][];
			int num18 = 0;
			while ((long)num18 < (long)((ulong)num16))
			{
				long[][] array = new long[4][];
				for (int i = 0; i < 4; i++)
				{
					array[i] = new long[4];
				}
				state.sb_maps[num18] = array;
				num18++;
			}
			state.sb_flags = new Xiph.oc_sb_flags[num16];
			state.nhmbs = num10 << 1;
			state.nvmbs = num11 << 1;
			state.nmbs = num17;
			state.mb_maps = new long[num17][][];
			num18 = 0;
			while ((long)num18 < (long)((ulong)num17))
			{
				long[][] array2 = new long[3][];
				for (int i = 0; i < 3; i++)
				{
					array2[i] = new long[4];
				}
				state.mb_maps[num18] = array2;
				num18++;
			}
			state.mb_modes = new sbyte[num17];
			state.coded_fragis = new long[num9];
			for (int j = 0; j < 3; j++)
			{
				Xiph.oc_fragment_plane oc_fragment_plane = state.fplanes[j];
				Xiph.oc_sb_create_plane_mapping(state.sb_maps, oc_fragment_plane.sboffset, state.sb_flags, oc_fragment_plane.sboffset, oc_fragment_plane.froffset, oc_fragment_plane.nhfrags, oc_fragment_plane.nvfrags);
			}
			Xiph.oc_mb_create_mapping(state.mb_maps, state.mb_modes, state.fplanes, state.info.pixel_fmt);
			Xiph.oc_state_border_init(state);
			return 0;
		}

		private static void oc_state_frarray_clear(Xiph.oc_theora_state state)
		{
			state.coded_fragis = null;
			state.mb_modes = null;
			state.mb_maps = null;
			state.sb_flags = null;
			state.sb_maps = null;
			state.frag_mvs = null;
			state.frags = null;
		}

		private static int oc_state_ref_bufs_init(Xiph.oc_theora_state state, int nrefs)
		{
			if (nrefs < 3 || nrefs > 6)
			{
				return -10;
			}
			Xiph.th_info info = state.info;
			int num = ((info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num2 = ((info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num3 = (int)(info.frame_width + 32u);
			int num4 = (int)(info.frame_height + 32u);
			int num5 = num3 >> num;
			int num6 = num4 >> num2;
			uint num7 = (uint)(num3 * num4);
			uint num8 = (uint)(num5 * num6);
			long num9 = 16L + 16L * (long)num3;
			long num10 = (long)(16 >> num) + (long)(16 >> num2) * (long)num5;
			uint num11 = num7 + 2u * num8;
			uint num12 = (uint)(nrefs * (int)num11);
			if ((ulong)num7 / (ulong)((long)num3) != (ulong)((long)num4) || 2u * num8 < num8 || num11 < num7 || (ulong)num12 / (ulong)((long)nrefs) != (ulong)num11)
			{
				return -23;
			}
			long[] array = state.frag_buf_offs = new long[state.nfrags];
			state.ref_frame_bufs[0].y.width = (int)info.frame_width;
			state.ref_frame_bufs[0].y.height = (int)info.frame_height;
			state.ref_frame_bufs[0].y.stride = num3;
			state.ref_frame_bufs[0].cb.width = (state.ref_frame_bufs[0].cr.width = (int)(info.frame_width >> num));
			state.ref_frame_bufs[0].cb.height = (state.ref_frame_bufs[0].cr.height = (int)(info.frame_height >> num2));
			state.ref_frame_bufs[0].cb.stride = (state.ref_frame_bufs[0].cr.stride = num5);
			for (int i = 1; i < nrefs; i++)
			{
				state.ref_frame_bufs[i] = state.ref_frame_bufs[0];
			}
			for (int i = 0; i < nrefs; i++)
			{
				state.ref_frame_data[i] = new byte[num11];
				state.ref_frame_bufs[i].y.data = new Xiph.ogg_ptr(state.ref_frame_data[i], (int)num9);
				state.ref_frame_bufs[i].cb.data = new Xiph.ogg_ptr(state.ref_frame_data[i], (int)((ulong)num7 + (ulong)num10));
				state.ref_frame_bufs[i].cr.data = new Xiph.ogg_ptr(state.ref_frame_data[i], (int)((ulong)(num7 + num8) + (ulong)num10));
				Xiph.oc_ycbcr_buffer_flip(ref state.ref_frame_bufs[i], ref state.ref_frame_bufs[i]);
			}
			state.ref_ystride[0] = -num3;
			state.ref_ystride[1] = (state.ref_ystride[2] = -num5);
			byte[] arg_2FF_0 = state.ref_frame_data[0];
			long num13 = 0L;
			for (int j = 0; j < 3; j++)
			{
				Xiph.th_img_plane th_img_plane = state.ref_frame_bufs[0][j];
				Xiph.oc_fragment_plane oc_fragment_plane = state.fplanes[j];
				Xiph.ogg_ptr src = new Xiph.ogg_ptr(th_img_plane.data);
				long num14 = oc_fragment_plane.froffset + oc_fragment_plane.nfrags;
				int nhfrags = oc_fragment_plane.nhfrags;
				long num15 = (long)th_img_plane.stride;
				while (num13 < num14)
				{
					Xiph.ogg_ptr ptr = new Xiph.ogg_ptr(src);
					long num16 = num13 + (long)nhfrags;
					while (num13 < num16)
					{
						array[(int)(checked((IntPtr)num13))] = (long)ptr.offset;
						ptr += 8;
						num13 += 1L;
					}
					src.offset += (int)num15 << 3;
				}
			}
			state.ref_frame_idx[0] = (state.ref_frame_idx[1] = (state.ref_frame_idx[4] = (state.ref_frame_idx[5] = (state.ref_frame_idx[2] = (state.ref_frame_idx[3] = -1)))));
			return 0;
		}

		private static void oc_state_ref_bufs_clear(Xiph.oc_theora_state state)
		{
			state.frag_buf_offs = null;
			state.ref_frame_data[0] = null;
		}

		private static void oc_state_accel_init(Xiph.oc_theora_state state)
		{
			state.cpu_flags = 0u;
			state.opt_data = new Xiph.ogg_ptr(Xiph.OC_FZIG_ZAG, 0);
		}

		public static int oc_state_init(Xiph.oc_theora_state state, ref Xiph.th_info info, int nrefs)
		{
			if ((info.frame_width & 15u) != 0u || (info.frame_height & 15u) != 0u || info.frame_width <= 0u || info.frame_width >= 1048576u || info.frame_height <= 0u || info.frame_height >= 1048576u || info.pic_x + info.pic_width > info.frame_width || info.pic_y + info.pic_height > info.frame_height || info.pic_x > 255u || info.frame_height - info.pic_height - info.pic_y > 255u || info.colorspace < Xiph.th_colorspace.TH_CS_UNSPECIFIED || info.colorspace >= Xiph.th_colorspace.TH_CS_NSPACES || info.pixel_fmt < Xiph.th_pixel_fmt.TH_PF_420 || info.pixel_fmt >= Xiph.th_pixel_fmt.TH_PF_NFORMATS)
			{
				return -10;
			}
			state.info = info;
			state.info.pic_y = info.frame_height - info.pic_height - info.pic_y;
			state.frame_type = -1;
			Xiph.oc_state_accel_init(state);
			int num = Xiph.oc_state_frarray_init(state);
			if (num >= 0)
			{
				num = Xiph.oc_state_ref_bufs_init(state, nrefs);
			}
			if (num < 0)
			{
				Xiph.oc_state_frarray_clear(state);
				return num;
			}
			if (info.keyframe_granule_shift < 0 || info.keyframe_granule_shift > 31)
			{
				state.info.keyframe_granule_shift = 31;
			}
			state.keyframe_num = 0L;
			state.curframe_num = -1L;
			state.granpos_bias = (Xiph.TH_VERSION_CHECK(info, 3, 2, 1) ? 1 : 0);
			return 0;
		}

		private static void oc_state_clear(Xiph.oc_theora_state state)
		{
			Xiph.oc_state_ref_bufs_clear(state);
			Xiph.oc_state_frarray_clear(state);
		}

		private static void oc_state_borders_fill_rows(Xiph.oc_theora_state state, int refi, int pli, int y0, int yend)
		{
			int num = 16 >> (((pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0) & 31);
			Xiph.th_img_plane th_img_plane = state.ref_frame_bufs[refi][pli];
			int stride = th_img_plane.stride;
			Xiph.ogg_ptr data = th_img_plane.data;
			byte[] data2 = data.data;
			int offset = data.offset;
			int num2 = offset + (int)((long)y0 * (long)stride);
			int num3 = num2 + (th_img_plane.width - 1);
			int num4 = offset + (int)((long)yend * (long)stride);
			while (num2 != num4)
			{
				for (int i = 0; i < num; i++)
				{
					data2[num2 - num + i] = data2[num2];
				}
				for (int i = 0; i < num; i++)
				{
					data2[num3 + 1 + i] = data2[num3];
				}
				num2 += stride;
				num3 += stride;
			}
		}

		private static void oc_state_borders_fill_caps(Xiph.oc_theora_state state, int refi, int pli)
		{
			int num = 16 >> (((pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0) & 31);
			int num2 = 16 >> (((pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0) & 31);
			Xiph.th_img_plane th_img_plane = state.ref_frame_bufs[refi][pli];
			int stride = th_img_plane.stride;
			int count = th_img_plane.width + (num << 1);
			Xiph.ogg_ptr ogg_ptr = th_img_plane.data - num;
			Xiph.ogg_ptr ptr = th_img_plane.data + (long)(th_img_plane.height - 1) * (long)stride - num;
			Xiph.ogg_ptr r = ogg_ptr - (long)stride * (long)num2;
			while (ogg_ptr != r)
			{
				Xiph.CopyArrays(ogg_ptr.data, ogg_ptr.offset, ogg_ptr.data, ogg_ptr.offset - stride, count);
				Xiph.CopyArrays(ptr.data, ptr.offset, ptr.data, ptr.offset + stride, count);
				ogg_ptr -= stride;
				ptr += stride;
			}
		}

		private static int oc_state_get_mv_offsets(Xiph.oc_theora_state state, ref int offset0, ref int offset1, int pli, short mv)
		{
			int num = state.ref_ystride[pli];
			int num2 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num3 = (int)Xiph.OC_MV_X(mv);
			int num4 = Xiph.OC_MV_Y(mv);
			int num5 = (int)Xiph.OC_MVMAP[num2][num4 + 31];
			int num6 = (int)Xiph.OC_MVMAP2[num2][num4 + 31];
			int num7 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num8 = (int)Xiph.OC_MVMAP[num7][num3 + 31];
			int num9 = (int)Xiph.OC_MVMAP2[num7][num3 + 31];
			int num10 = num5 * num + num8;
			if (num9 != 0 || num6 != 0)
			{
				offset1 = num10 + num6 * num + num9;
				offset0 = num10;
				return 2;
			}
			offset0 = num10;
			return 1;
		}

		private static void oc_state_frag_recon(Xiph.oc_theora_state state, long fragi, int pli, short[] dct_coeffs, int last_zzi, ushort dc_quant)
		{
			if (last_zzi < 2)
			{
				short num = (short)(dct_coeffs[0] * (short)dc_quant + 15 >> 5);
				for (int i = 0; i < 64; i++)
				{
					dct_coeffs[64 + i] = num;
				}
			}
			else
			{
				dct_coeffs[0] = dct_coeffs[0] * (short)dc_quant;
				int num2 = 64;
				int num3 = 0;
				short[] oc_idct8x8_w = state.oc_idct8x8_w;
				int num4 = 0;
				if (last_zzi <= 3)
				{
					short[] array = oc_idct8x8_w;
					int num5 = num4;
					int num6 = num3;
					short num7 = dct_coeffs[num6];
					short num8 = dct_coeffs[num6 + 1];
					int num9 = 46341 * (int)num7 >> 16;
					int num10 = 12785 * num8 >> 16;
					int num11 = 64277 * (int)num8 >> 16;
					int num12 = 46341 * num10 >> 16;
					int num13 = 46341 * num11 >> 16;
					int num14 = num13 + num12;
					num12 = num13 - num12;
					num13 = num14;
					array[num5] = (short)(num9 + num11);
					array[num5 + 8] = (short)(num9 + num13);
					array[num5 + 16] = (short)(num9 + num12);
					array[num5 + 24] = (short)(num9 + num10);
					array[num5 + 32] = (short)(num9 - num10);
					array[num5 + 40] = (short)(num9 - num12);
					array[num5 + 48] = (short)(num9 - num13);
					array[num5 + 56] = (short)(num9 - num11);
					short[] array2 = oc_idct8x8_w;
					int num15 = num4 + 1;
					int num16 = num3 + 8;
					array2[num15] = (array2[num15 + 8] = (array2[num15 + 16] = (array2[num15 + 24] = (array2[num15 + 32] = (array2[num15 + 40] = (array2[num15 + 48] = (array2[num15 + 56] = (short)(46341 * (int)dct_coeffs[num16] >> 16))))))));
					for (int j = 0; j < 8; j++)
					{
						int num17 = num2 + j;
						short[] array3 = oc_idct8x8_w;
						int num18 = num4 + j * 8;
						short num19 = array3[num18];
						short num20 = array3[num18 + 1];
						int num21 = 46341 * (int)num19 >> 16;
						int num22 = 12785 * num20 >> 16;
						int num23 = 64277 * (int)num20 >> 16;
						int num24 = 46341 * num22 >> 16;
						int num25 = 46341 * num23 >> 16;
						int num26 = num25 + num24;
						num24 = num25 - num24;
						num25 = num26;
						dct_coeffs[num17] = (short)(num21 + num23);
						dct_coeffs[num17 + 8] = (short)(num21 + num25);
						dct_coeffs[num17 + 16] = (short)(num21 + num24);
						dct_coeffs[num17 + 24] = (short)(num21 + num22);
						dct_coeffs[num17 + 32] = (short)(num21 - num22);
						dct_coeffs[num17 + 40] = (short)(num21 - num24);
						dct_coeffs[num17 + 48] = (short)(num21 - num25);
						dct_coeffs[num17 + 56] = (short)(num21 - num23);
					}
					for (int j = 0; j < 64; j++)
					{
						dct_coeffs[num2 + j] = (short)(dct_coeffs[num2 + j] + 8 >> 4);
					}
					if (num3 != num2)
					{
						dct_coeffs[num3] = (dct_coeffs[num3 + 1] = (dct_coeffs[num3 + 8] = 0));
					}
				}
				else if (last_zzi <= 10)
				{
					short[] array4 = oc_idct8x8_w;
					int num27 = num4;
					int num28 = num3;
					short num29 = dct_coeffs[num28];
					short num30 = dct_coeffs[num28 + 1];
					short num31 = dct_coeffs[num28 + 2];
					short num32 = dct_coeffs[num28 + 3];
					int num33 = 46341 * (int)num29 >> 16;
					int num34 = 25080 * num31 >> 16;
					int num35 = 60547 * (int)num31 >> 16;
					int num36 = 12785 * num30 >> 16;
					int num37 = -(36410 * (int)num32 >> 16);
					int num38 = 54491 * (int)num32 >> 16;
					int num39 = 64277 * (int)num30 >> 16;
					int num40 = num36 + num37;
					num37 = 46341 * (int)((short)(num36 - num37)) >> 16;
					num36 = num40;
					num40 = num39 + num38;
					num38 = 46341 * (int)((short)(num39 - num38)) >> 16;
					num39 = num40;
					int num41 = num33 + num34;
					num34 = num33 - num34;
					num40 = num33 + num35;
					num35 = num33 - num35;
					num33 = num40;
					num40 = num38 + num37;
					num37 = num38 - num37;
					num38 = num40;
					array4[num27] = (short)(num33 + num39);
					array4[num27 + 8] = (short)(num41 + num38);
					array4[num27 + 16] = (short)(num34 + num37);
					array4[num27 + 24] = (short)(num35 + num36);
					array4[num27 + 32] = (short)(num35 - num36);
					array4[num27 + 40] = (short)(num34 - num37);
					array4[num27 + 48] = (short)(num41 - num38);
					array4[num27 + 56] = (short)(num33 - num39);
					short[] array5 = oc_idct8x8_w;
					int num42 = num4 + 1;
					int num43 = num3 + 8;
					short num44 = dct_coeffs[num43];
					short num45 = dct_coeffs[num43 + 1];
					short num46 = dct_coeffs[num43 + 2];
					int num47 = 46341 * (int)num44 >> 16;
					int num48 = 25080 * num46 >> 16;
					int num49 = 60547 * (int)num46 >> 16;
					int num50 = 12785 * num45 >> 16;
					int num51 = 64277 * (int)num45 >> 16;
					int num52 = 46341 * num50 >> 16;
					int num53 = 46341 * num51 >> 16;
					int num54 = num47 + num48;
					num48 = num47 - num48;
					int num55 = num47 + num49;
					num49 = num47 - num49;
					num47 = num55;
					num55 = num53 + num52;
					num52 = num53 - num52;
					num53 = num55;
					array5[num42] = (short)(num47 + num51);
					array5[num42 + 8] = (short)(num54 + num53);
					array5[num42 + 16] = (short)(num48 + num52);
					array5[num42 + 24] = (short)(num49 + num50);
					array5[num42 + 32] = (short)(num49 - num50);
					array5[num42 + 40] = (short)(num48 - num52);
					array5[num42 + 48] = (short)(num54 - num53);
					array5[num42 + 56] = (short)(num47 - num51);
					short[] array6 = oc_idct8x8_w;
					int num56 = num4 + 2;
					int num57 = num3 + 16;
					short num58 = dct_coeffs[num57];
					short num59 = dct_coeffs[num57 + 1];
					int num60 = 46341 * (int)num58 >> 16;
					int num61 = 12785 * num59 >> 16;
					int num62 = 64277 * (int)num59 >> 16;
					int num63 = 46341 * num61 >> 16;
					int num64 = 46341 * num62 >> 16;
					int num65 = num64 + num63;
					num63 = num64 - num63;
					num64 = num65;
					array6[num56] = (short)(num60 + num62);
					array6[num56 + 8] = (short)(num60 + num64);
					array6[num56 + 16] = (short)(num60 + num63);
					array6[num56 + 24] = (short)(num60 + num61);
					array6[num56 + 32] = (short)(num60 - num61);
					array6[num56 + 40] = (short)(num60 - num63);
					array6[num56 + 48] = (short)(num60 - num64);
					array6[num56 + 56] = (short)(num60 - num62);
					short[] array7 = oc_idct8x8_w;
					int num66 = num4 + 3;
					int num67 = num3 + 24;
					array7[num66] = (array7[num66 + 8] = (array7[num66 + 16] = (array7[num66 + 24] = (array7[num66 + 32] = (array7[num66 + 40] = (array7[num66 + 48] = (array7[num66 + 56] = (short)(46341 * (int)dct_coeffs[num67] >> 16))))))));
					for (int j = 0; j < 8; j++)
					{
						int num68 = num2 + j;
						short[] array8 = oc_idct8x8_w;
						int num69 = num4 + j * 8;
						short num70 = array8[num69];
						short num71 = array8[num69 + 1];
						short num72 = array8[num69 + 2];
						short num73 = array8[num69 + 3];
						int num74 = 46341 * (int)num70 >> 16;
						int num75 = 25080 * num72 >> 16;
						int num76 = 60547 * (int)num72 >> 16;
						int num77 = 12785 * num71 >> 16;
						int num78 = -(36410 * (int)num73 >> 16);
						int num79 = 54491 * (int)num73 >> 16;
						int num80 = 64277 * (int)num71 >> 16;
						int num81 = num77 + num78;
						num78 = 46341 * (int)((short)(num77 - num78)) >> 16;
						num77 = num81;
						num81 = num80 + num79;
						num79 = 46341 * (int)((short)(num80 - num79)) >> 16;
						num80 = num81;
						int num82 = num74 + num75;
						num75 = num74 - num75;
						num81 = num74 + num76;
						num76 = num74 - num76;
						num74 = num81;
						num81 = num79 + num78;
						num78 = num79 - num78;
						num79 = num81;
						dct_coeffs[num68] = (short)(num74 + num80);
						dct_coeffs[num68 + 8] = (short)(num82 + num79);
						dct_coeffs[num68 + 16] = (short)(num75 + num78);
						dct_coeffs[num68 + 24] = (short)(num76 + num77);
						dct_coeffs[num68 + 32] = (short)(num76 - num77);
						dct_coeffs[num68 + 40] = (short)(num75 - num78);
						dct_coeffs[num68 + 48] = (short)(num82 - num79);
						dct_coeffs[num68 + 56] = (short)(num74 - num80);
					}
					for (int j = 0; j < 64; j++)
					{
						dct_coeffs[num2 + j] = (short)(dct_coeffs[num2 + j] + 8 >> 4);
					}
					if (num3 != num2)
					{
						dct_coeffs[num3] = 0;
						dct_coeffs[num3 + 1] = 0;
						dct_coeffs[num3 + 2] = 0;
						dct_coeffs[num3 + 3] = 0;
						dct_coeffs[num3 + 8] = 0;
						dct_coeffs[num3 + 9] = 0;
						dct_coeffs[num3 + 10] = 0;
						dct_coeffs[num3 + 16] = 0;
						dct_coeffs[num3 + 17] = 0;
						dct_coeffs[num3 + 24] = 0;
					}
				}
				else
				{
					for (int j = 0; j < 8; j++)
					{
						short[] array9 = oc_idct8x8_w;
						int num83 = num4 + j;
						int num84 = num3 + j * 8;
						short num85 = dct_coeffs[num84];
						short num86 = dct_coeffs[num84 + 1];
						short num87 = dct_coeffs[num84 + 2];
						short num88 = dct_coeffs[num84 + 3];
						short num89 = dct_coeffs[num84 + 4];
						short num90 = dct_coeffs[num84 + 5];
						short num91 = dct_coeffs[num84 + 6];
						short num92 = dct_coeffs[num84 + 7];
						int num93 = 46341 * (int)(num85 + num89) >> 16;
						int num94 = 46341 * (int)(num85 - num89) >> 16;
						int num95 = (25080 * num87 >> 16) - (60547 * (int)num91 >> 16);
						int num96 = (60547 * (int)num87 >> 16) + (25080 * num91 >> 16);
						int num97 = (12785 * num86 >> 16) - (64277 * (int)num92 >> 16);
						int num98 = (54491 * (int)num90 >> 16) - (36410 * (int)num88 >> 16);
						int num99 = (36410 * (int)num90 >> 16) + (54491 * (int)num88 >> 16);
						int num100 = (64277 * (int)num86 >> 16) + (12785 * num92 >> 16);
						int num101 = num97 + num98;
						num98 = 46341 * (int)((short)(num97 - num98)) >> 16;
						num97 = num101;
						num101 = num100 + num99;
						num99 = 46341 * (int)((short)(num100 - num99)) >> 16;
						num100 = num101;
						num101 = num93 + num96;
						num96 = num93 - num96;
						num93 = num101;
						num101 = num94 + num95;
						num95 = num94 - num95;
						num94 = num101;
						num101 = num99 + num98;
						num98 = num99 - num98;
						num99 = num101;
						array9[num83] = (short)(num93 + num100);
						array9[num83 + 8] = (short)(num94 + num99);
						array9[num83 + 16] = (short)(num95 + num98);
						array9[num83 + 24] = (short)(num96 + num97);
						array9[num83 + 32] = (short)(num96 - num97);
						array9[num83 + 40] = (short)(num95 - num98);
						array9[num83 + 48] = (short)(num94 - num99);
						array9[num83 + 56] = (short)(num93 - num100);
					}
					for (int j = 0; j < 8; j++)
					{
						int num102 = num2 + j;
						short[] array10 = oc_idct8x8_w;
						int num103 = num4 + j * 8;
						short num104 = array10[num103];
						short num105 = array10[num103 + 1];
						short num106 = array10[num103 + 2];
						short num107 = array10[num103 + 3];
						short num108 = array10[num103 + 4];
						short num109 = array10[num103 + 5];
						short num110 = array10[num103 + 6];
						short num111 = array10[num103 + 7];
						int num112 = 46341 * (int)(num104 + num108) >> 16;
						int num113 = 46341 * (int)(num104 - num108) >> 16;
						int num114 = (25080 * num106 >> 16) - (60547 * (int)num110 >> 16);
						int num115 = (60547 * (int)num106 >> 16) + (25080 * num110 >> 16);
						int num116 = (12785 * num105 >> 16) - (64277 * (int)num111 >> 16);
						int num117 = (54491 * (int)num109 >> 16) - (36410 * (int)num107 >> 16);
						int num118 = (36410 * (int)num109 >> 16) + (54491 * (int)num107 >> 16);
						int num119 = (64277 * (int)num105 >> 16) + (12785 * num111 >> 16);
						int num120 = num116 + num117;
						num117 = 46341 * (int)((short)(num116 - num117)) >> 16;
						num116 = num120;
						num120 = num119 + num118;
						num118 = 46341 * (int)((short)(num119 - num118)) >> 16;
						num119 = num120;
						num120 = num112 + num115;
						num115 = num112 - num115;
						num112 = num120;
						num120 = num113 + num114;
						num114 = num113 - num114;
						num113 = num120;
						num120 = num118 + num117;
						num117 = num118 - num117;
						num118 = num120;
						dct_coeffs[num102] = (short)(num112 + num119);
						dct_coeffs[num102 + 8] = (short)(num113 + num118);
						dct_coeffs[num102 + 16] = (short)(num114 + num117);
						dct_coeffs[num102 + 24] = (short)(num115 + num116);
						dct_coeffs[num102 + 32] = (short)(num115 - num116);
						dct_coeffs[num102 + 40] = (short)(num114 - num117);
						dct_coeffs[num102 + 48] = (short)(num113 - num118);
						dct_coeffs[num102 + 56] = (short)(num112 - num119);
					}
					for (int j = 0; j < 64; j++)
					{
						dct_coeffs[num2 + j] = (short)(dct_coeffs[num2 + j] + 8 >> 4);
					}
					if (num3 != num2)
					{
						for (int j = 0; j < 64; j++)
						{
							dct_coeffs[num3 + j] = 0;
						}
					}
				}
			}
			long num121;
			int mb_mode;
			int num122;
			byte[] array11;
			checked
			{
				num121 = state.frag_buf_offs[(int)((IntPtr)fragi)];
				mb_mode = (int)state.frags[(int)((IntPtr)fragi)].mb_mode;
				num122 = state.ref_ystride[pli];
				array11 = state.ref_frame_data[state.ref_frame_idx[2]];
			}
			if (mb_mode == 1)
			{
				int num123 = (int)num121;
				int num124 = 64;
				for (int k = 0; k < 8; k++)
				{
					for (int l = 0; l < 8; l++)
					{
						int num125 = (int)(dct_coeffs[num124 + k * 8 + l] + 128);
						array11[num123 + l] = (byte)(((num125 < 0) ? 1 : -1) & (num125 | -((num125 > 255) ? 1 : 0)));
					}
					num123 += num122;
				}
				return;
			}
			int num126 = 0;
			int num127 = 268505377 >> mb_mode * 4 & 15;
			byte[] array12 = state.ref_frame_data[state.ref_frame_idx[num127]];
			short num128 = state.frag_mvs[(int)(checked((IntPtr)fragi))];
			int num129 = state.ref_ystride[pli];
			int num130 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_422) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num131 = (int)((sbyte)num128);
			int num132 = num128 >> 8;
			int num133 = (int)Xiph.OC_MVMAP[num130][num132 + 31];
			int num134 = (int)Xiph.OC_MVMAP2[num130][num132 + 31];
			int num135 = (pli != 0 && (state.info.pixel_fmt & Xiph.th_pixel_fmt.TH_PF_RSVD) == Xiph.th_pixel_fmt.TH_PF_420) ? 1 : 0;
			int num136 = (int)Xiph.OC_MVMAP[num135][num131 + 31];
			int num137 = (int)Xiph.OC_MVMAP2[num135][num131 + 31];
			int num138 = num133 * num129 + num136;
			if (num137 != 0 || num134 != 0)
			{
				num126 = num138 + num134 * num129 + num137;
			}
			int num139 = num138;
			int num140 = 1;
			if (num140 > 1)
			{
				int num141 = (int)num121;
				byte[] array13 = array12;
				int num142 = (int)num121 + num139;
				byte[] array14 = array12;
				int num143 = (int)num121 + num126;
				int num144 = 64;
				for (int m = 0; m < 8; m++)
				{
					for (int n = 0; n < 8; n++)
					{
						int num145 = (int)dct_coeffs[num144 + m * 8 + n] + (array13[num142 + n] + array14[num143 + n] >> 1);
						array11[num141 + n] = (byte)(((num145 < 0) ? 1 : -1) & (num145 | -((num145 > 255) ? 1 : 0)));
					}
					num141 += num122;
					num142 += num122;
					num143 += num122;
				}
				return;
			}
			int num146 = (int)num121;
			byte[] array15 = array12;
			int num147 = (int)num121 + num139;
			int num148 = 64;
			for (int num149 = 0; num149 < 8; num149++)
			{
				for (int num150 = 0; num150 < 8; num150++)
				{
					int num151 = (int)(dct_coeffs[num148 + num149 * 8 + num150] + (short)array15[num147 + num150]);
					array11[num146 + num150] = (byte)(((num151 < 0) ? 1 : -1) & (num151 | -((num151 > 255) ? 1 : 0)));
				}
				num146 += num122;
				num147 += num122;
			}
		}

		private static void loop_filter_h(byte[] pix, long pixo, int ystride, sbyte[] bv, int bvo)
		{
			pixo -= 2L;
			byte b;
			byte b2;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			int num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			int num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + 1L)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + 2L)))];
			}
			num = (int)(pix[(int)(checked((IntPtr)pixo))] - pix[(int)(checked((IntPtr)(unchecked(pixo + 3L))))] + 3 * (b2 - b));
			num = (int)bv[bvo + (num + 4 >> 3)];
			num2 = (int)b + num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 1L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			num2 = (int)b2 - num;
			pix[(int)(checked((IntPtr)(unchecked(pixo + 2L))))] = (byte)(((num2 < 0) ? 1 : -1) & (num2 | -((num2 > 255) ? 1 : 0)));
			pixo += (long)ystride;
		}

		private static void loop_filter_v(byte[] pix, long pixo, int ystride, sbyte[] bv, int bvo)
		{
			pixo -= (long)(ystride * 2);
			int num = 0;
			byte b;
			byte b2;
			byte b3;
			byte b4;
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			int num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			int num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			checked
			{
				b = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 3) + (long)num)))];
				b2 = pix[(int)((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num)))];
				b3 = pix[(int)((IntPtr)(unchecked(pixo + (long)ystride + (long)num)))];
				b4 = pix[(int)((IntPtr)(unchecked(pixo + 0L + (long)num)))];
			}
			num2 = (int)(b4 - b + 3 * (b2 - b3));
			num2 = (int)bv[bvo + (num2 + 4 >> 3)];
			num3 = (int)b3 + num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)ystride + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
			num3 = (int)b2 - num2;
			pix[(int)(checked((IntPtr)(unchecked(pixo + (long)(ystride * 2) + (long)num))))] = (byte)(((num3 < 0) ? 1 : -1) & (num3 | -((num3 > 255) ? 1 : 0)));
		}

		private static void oc_loop_filter_init_c(sbyte[] bv, int flimit)
		{
			Array.Clear(bv, 0, 256);
			for (int i = 0; i < flimit; i++)
			{
				if (127 - i - flimit >= 0)
				{
					bv[127 - i - flimit] = (sbyte)(i - flimit);
				}
				bv[127 - i] = (sbyte)(-(sbyte)i);
				bv[127 + i] = (sbyte)i;
				if (127 + i + flimit < 256)
				{
					bv[127 + i + flimit] = (sbyte)(flimit - i);
				}
			}
		}

		private static void oc_state_loop_filter_frag_rows_c(Xiph.oc_theora_state state, sbyte[] bv, int refi, int pli, int fragy0, int fragy_end)
		{
			int nhfrags = state.fplanes[pli].nhfrags;
			long froffset = state.fplanes[pli].froffset;
			long num = froffset + state.fplanes[pli].nfrags;
			long num2 = froffset + (long)fragy0 * (long)nhfrags;
			long num3 = froffset + (long)fragy_end * (long)nhfrags;
			int num4 = state.ref_ystride[pli];
			long[] frag_buf_offs = state.frag_buf_offs;
			byte[] array = state.ref_frame_data[refi];
			while (num2 < num3)
			{
				long num5 = num2;
				long num6 = num5 + (long)nhfrags;
				while (num5 < num6)
				{
					if (state.frags[(int)(checked((IntPtr)num5))].coded != 0u)
					{
						long num7 = frag_buf_offs[(int)(checked((IntPtr)num5))];
						if (num5 > num2)
						{
							byte[] array2 = array;
							long num8 = num7;
							int num9 = 127;
							num8 -= 2L;
							for (int i = 0; i < 8; i++)
							{
								byte b;
								byte b2;
								checked
								{
									b = array2[(int)((IntPtr)(unchecked(num8 + 1L)))];
									b2 = array2[(int)((IntPtr)(unchecked(num8 + 2L)))];
								}
								int num10 = (int)(array2[(int)(checked((IntPtr)num8))] - array2[(int)(checked((IntPtr)(unchecked(num8 + 3L))))] + 3 * (b2 - b));
								num10 = (int)bv[num9 + (num10 + 4 >> 3)];
								int num11 = (int)b + num10;
								array2[(int)(checked((IntPtr)(unchecked(num8 + 1L))))] = (byte)(((num11 < 0) ? 1 : -1) & (num11 | -((num11 > 255) ? 1 : 0)));
								num11 = (int)b2 - num10;
								array2[(int)(checked((IntPtr)(unchecked(num8 + 2L))))] = (byte)(((num11 < 0) ? 1 : -1) & (num11 | -((num11 > 255) ? 1 : 0)));
								num8 += (long)num4;
							}
						}
						if (num2 > froffset)
						{
							byte[] array3 = array;
							long num12 = num7;
							int num13 = 127;
							num12 -= (long)(num4 * 2);
							for (int j = 0; j < 8; j++)
							{
								byte b3;
								byte b4;
								byte b5;
								byte b6;
								checked
								{
									b3 = array3[(int)((IntPtr)(unchecked(num12 + (long)(num4 * 3))))];
									b4 = array3[(int)((IntPtr)(unchecked(num12 + (long)(num4 * 2))))];
									b5 = array3[(int)((IntPtr)(unchecked(num12 + (long)num4)))];
									b6 = array3[(int)((IntPtr)(unchecked(num12 + 0L)))];
								}
								int num14 = (int)(b6 - b3 + 3 * (b4 - b5));
								num14 = (int)bv[num13 + (num14 + 4 >> 3)];
								int num15 = (int)b5 + num14;
								array3[(int)(checked((IntPtr)(unchecked(num12 + (long)num4))))] = (byte)(((num15 < 0) ? 1 : -1) & (num15 | -((num15 > 255) ? 1 : 0)));
								num15 = (int)b4 - num14;
								array3[(int)(checked((IntPtr)(unchecked(num12 + (long)(num4 * 2)))))] = (byte)(((num15 < 0) ? 1 : -1) & (num15 | -((num15 > 255) ? 1 : 0)));
							}
						}
						if (num5 + 1L < num6 && state.frags[(int)(checked((IntPtr)(unchecked(num5 + 1L))))].coded == 0u)
						{
							byte[] array4 = array;
							long num16 = num7 + 8L;
							int num17 = 127;
							num16 -= 2L;
							for (int k = 0; k < 8; k++)
							{
								byte b7;
								byte b8;
								checked
								{
									b7 = array4[(int)((IntPtr)(unchecked(num16 + 1L)))];
									b8 = array4[(int)((IntPtr)(unchecked(num16 + 2L)))];
								}
								int num18 = (int)(array4[(int)(checked((IntPtr)num16))] - array4[(int)(checked((IntPtr)(unchecked(num16 + 3L))))] + 3 * (b8 - b7));
								num18 = (int)bv[num17 + (num18 + 4 >> 3)];
								int num19 = (int)b7 + num18;
								array4[(int)(checked((IntPtr)(unchecked(num16 + 1L))))] = (byte)(((num19 < 0) ? 1 : -1) & (num19 | -((num19 > 255) ? 1 : 0)));
								num19 = (int)b8 - num18;
								array4[(int)(checked((IntPtr)(unchecked(num16 + 2L))))] = (byte)(((num19 < 0) ? 1 : -1) & (num19 | -((num19 > 255) ? 1 : 0)));
								num16 += (long)num4;
							}
						}
						if (num5 + (long)nhfrags < num && state.frags[(int)(checked((IntPtr)(unchecked(num5 + (long)nhfrags))))].coded == 0u)
						{
							byte[] array5 = array;
							long num20 = num7 + (long)((long)num4 << 3);
							int num21 = 127;
							num20 -= (long)(num4 * 2);
							for (int l = 0; l < 8; l++)
							{
								byte b9;
								byte b10;
								byte b11;
								byte b12;
								checked
								{
									b9 = array5[(int)((IntPtr)(unchecked(num20 + (long)(num4 * 3))))];
									b10 = array5[(int)((IntPtr)(unchecked(num20 + (long)(num4 * 2))))];
									b11 = array5[(int)((IntPtr)(unchecked(num20 + (long)num4)))];
									b12 = array5[(int)((IntPtr)(unchecked(num20 + 0L)))];
								}
								int num22 = (int)(b12 - b9 + 3 * (b10 - b11));
								num22 = (int)bv[num21 + (num22 + 4 >> 3)];
								int num23 = (int)b11 + num22;
								array5[(int)(checked((IntPtr)(unchecked(num20 + (long)num4))))] = (byte)(((num23 < 0) ? 1 : -1) & (num23 | -((num23 > 255) ? 1 : 0)));
								num23 = (int)b10 - num22;
								array5[(int)(checked((IntPtr)(unchecked(num20 + (long)(num4 * 2)))))] = (byte)(((num23 < 0) ? 1 : -1) & (num23 | -((num23 > 255) ? 1 : 0)));
							}
						}
					}
					num5 += 1L;
				}
				num2 += (long)nhfrags;
			}
		}

		private static long th_granule_frame(Xiph.oc_theora_state state, long granpos)
		{
			if (granpos >= 0L)
			{
				long num = granpos >> state.info.keyframe_granule_shift;
				long num2 = granpos - (num << state.info.keyframe_granule_shift);
				return num + num2 - (Xiph.TH_VERSION_CHECK(state.info, 3, 2, 1) ? 1L : 0L);
			}
			return -1L;
		}

		public static double th_granule_time(Xiph.oc_theora_state state, long granpos)
		{
			if (granpos >= 0L)
			{
				return (double)(Xiph.th_granule_frame(state, granpos) + 1L) * (state.info.fps_denominator / state.info.fps_numerator);
			}
			return -1.0;
		}

		public static int vorbis_bitrate_managed(Xiph.vorbis_block vb)
		{
			Xiph.vorbis_dsp_state vd = vb.vd;
			Xiph.private_state backend_state = vd.backend_state;
			Xiph.bitrate_manager_state bms = backend_state.bms;
			if (bms != null && bms.managed != 0)
			{
				return 1;
			}
			return 0;
		}

		public static int vorbis_block_init(Xiph.vorbis_dsp_state v, Xiph.vorbis_block vb)
		{
			vb.vd = v;
			vb.localalloc = 0L;
			vb.localstore = null;
			if (v.analysisp != 0)
			{
				Xiph.vorbis_block_internal vorbis_block_internal = vb.internl = new Xiph.vorbis_block_internal();
				vorbis_block_internal.ampmax = -9999f;
				for (int i = 0; i < 15; i++)
				{
					if (i == 7)
					{
						vorbis_block_internal.packetblob[i] = vb.opb;
					}
					else
					{
						vorbis_block_internal.packetblob[i] = new Xiph.oggpack_buffer();
					}
					Xiph.oggpack_writeinit(vorbis_block_internal.packetblob[i]);
				}
			}
			return 0;
		}

		private static void _vorbis_block_ripcord(Xiph.vorbis_block vb)
		{
			if (vb.pcm != null)
			{
				for (int i = 0; i < vb.pcm.Length; i++)
				{
					Xiph.MemCache<float>.Release(vb.pcm[i]);
				}
				Xiph.MemCache<float[]>.Release(vb.pcm);
				vb.pcm = null;
			}
			if (vb.totaluse != 0L)
			{
				vb.localstore = new byte[vb.totaluse + vb.localalloc];
				vb.localalloc += vb.totaluse;
				vb.totaluse = 0L;
			}
			vb.localtop = 0L;
		}

		public static int vorbis_block_clear(Xiph.vorbis_block vb)
		{
			Xiph.vorbis_block_internal internl = vb.internl;
			Xiph._vorbis_block_ripcord(vb);
			vb.localstore = null;
			if (internl != null)
			{
				for (int i = 0; i < 15; i++)
				{
					Xiph.oggpack_writeclear(internl.packetblob[i]);
					if (i != 7)
					{
						internl.packetblob[i] = null;
					}
				}
			}
			return 0;
		}

		private static int _vds_shared_init(Xiph.vorbis_dsp_state v, Xiph.vorbis_info vi, int encp)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			if (codec_setup == null)
			{
				return 1;
			}
			int halfrate_flag = codec_setup.halfrate_flag;
			Xiph.vorbis_dsp_clear(v);
			Xiph.private_state private_state = v.backend_state = new Xiph.private_state();
			v.vi = vi;
			private_state.modebits = Xiph.ilog2((uint)codec_setup.modes);
			private_state.transform[0] = new Xiph.mdct_lookup[1];
			private_state.transform[1] = new Xiph.mdct_lookup[1];
			private_state.transform[0][0] = new Xiph.mdct_lookup();
			private_state.transform[1][0] = new Xiph.mdct_lookup();
			Xiph.mdct_init(private_state.transform[0][0], (int)(codec_setup.blocksizes[0] >> halfrate_flag));
			Xiph.mdct_init(private_state.transform[1][0], (int)(codec_setup.blocksizes[1] >> halfrate_flag));
			private_state.window[0] = Xiph.ilog2((uint)codec_setup.blocksizes[0]) - 6;
			private_state.window[1] = Xiph.ilog2((uint)codec_setup.blocksizes[1]) - 6;
			if (encp != 0)
			{
				Xiph.drft_init(private_state.fft_look[0], (int)codec_setup.blocksizes[0]);
				Xiph.drft_init(private_state.fft_look[1], (int)codec_setup.blocksizes[1]);
				if (codec_setup.fullbooks == null)
				{
					codec_setup.fullbooks = new Xiph.codebook[codec_setup.books];
					for (int i = 0; i < codec_setup.books; i++)
					{
						codec_setup.fullbooks[i] = new Xiph.codebook();
						Xiph.vorbis_book_init_encode(codec_setup.fullbooks[i], codec_setup.book_param[i]);
					}
				}
				private_state.psy = new Xiph.vorbis_look_psy[codec_setup.psys];
				for (int i = 0; i < codec_setup.psys; i++)
				{
					private_state.psy[i] = new Xiph.vorbis_look_psy();
					Xiph._vp_psy_init(private_state.psy[i], codec_setup.psy_param[i], codec_setup.psy_g_param, (int)codec_setup.blocksizes[codec_setup.psy_param[i].blockflag] / 2, vi.rate);
				}
				v.analysisp = 1;
			}
			else if (codec_setup.fullbooks == null)
			{
				codec_setup.fullbooks = new Xiph.codebook[codec_setup.books];
				for (int i = 0; i < codec_setup.books; i++)
				{
					codec_setup.fullbooks[i] = new Xiph.codebook();
				}
				for (int i = 0; i < codec_setup.books; i++)
				{
					if (codec_setup.book_param[i] == null || Xiph.vorbis_book_init_decode(codec_setup.fullbooks[i], codec_setup.book_param[i]) != 0)
					{
						for (i = 0; i < codec_setup.books; i++)
						{
							if (codec_setup.book_param[i] != null)
							{
								Xiph.vorbis_staticbook_destroy(codec_setup.book_param[i]);
								codec_setup.book_param[i] = null;
							}
						}
						Xiph.vorbis_dsp_clear(v);
						return -1;
					}
					Xiph.vorbis_staticbook_destroy(codec_setup.book_param[i]);
					codec_setup.book_param[i] = null;
				}
			}
			v.pcm_storage = (int)codec_setup.blocksizes[1];
			v.pcm = new float[vi.channels][];
			v.pcmret = new int[vi.channels];
			for (int i = 0; i < vi.channels; i++)
			{
				v.pcm[i] = new float[v.pcm_storage];
			}
			v.lW = 0L;
			v.W = 0L;
			v.centerW = codec_setup.blocksizes[1] / 2L;
			v.pcm_current = (int)v.centerW;
			private_state.flr = new Xiph.vorbis_look_floor[codec_setup.floors];
			private_state.residue = new Xiph.vorbis_look_residue[codec_setup.residues];
			for (int i = 0; i < codec_setup.floors; i++)
			{
				private_state.flr[i] = Xiph._floor_P[codec_setup.floor_type[i]].look(v, codec_setup.floor_param[i]);
			}
			for (int i = 0; i < codec_setup.residues; i++)
			{
				private_state.residue[i] = Xiph._residue_P[codec_setup.residue_type[i]].look(v, codec_setup.residue_param[i]);
			}
			return 0;
		}

		public static void vorbis_dsp_clear(Xiph.vorbis_dsp_state v)
		{
		}

		public static int vorbis_synthesis_restart(Xiph.vorbis_dsp_state v)
		{
			Xiph.vorbis_info vi = v.vi;
			if (v.backend_state == null)
			{
				return -1;
			}
			if (vi == null)
			{
				return -1;
			}
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			if (codec_setup == null)
			{
				return -1;
			}
			int halfrate_flag = codec_setup.halfrate_flag;
			v.centerW = codec_setup.blocksizes[1] >> halfrate_flag + 1;
			v.pcm_current = (int)(v.centerW >> halfrate_flag);
			v.pcm_returned = -1;
			v.granulepos = -1L;
			v.sequence = -1L;
			v.eofflag = 0;
			v.backend_state.sample_count = -1L;
			return 0;
		}

		public static int vorbis_synthesis_init(Xiph.vorbis_dsp_state v, Xiph.vorbis_info vi)
		{
			if (Xiph._vds_shared_init(v, vi, 0) != 0)
			{
				Xiph.vorbis_dsp_clear(v);
				return 1;
			}
			Xiph.vorbis_synthesis_restart(v);
			return 0;
		}

		public static int vorbis_synthesis_blockin(Xiph.vorbis_dsp_state v, Xiph.vorbis_block vb)
		{
			Xiph.vorbis_info vi = v.vi;
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			Xiph.private_state backend_state = v.backend_state;
			int halfrate_flag = codec_setup.halfrate_flag;
			if (vb == null)
			{
				return -131;
			}
			if (v.pcm_current > v.pcm_returned && v.pcm_returned != -1)
			{
				return -131;
			}
			v.lW = v.W;
			v.W = vb.W;
			v.nW = -1L;
			if (v.sequence == -1L || v.sequence + 1L != vb.sequence)
			{
				v.granulepos = -1L;
				backend_state.sample_count = -1L;
			}
			v.sequence = vb.sequence;
			if (vb.pcm != null)
			{
				int num = (int)(codec_setup.blocksizes[(int)(checked((IntPtr)v.W))] >> halfrate_flag + 1);
				int num2 = (int)(codec_setup.blocksizes[0] >> halfrate_flag + 1);
				int num3 = (int)(codec_setup.blocksizes[1] >> halfrate_flag + 1);
				v.glue_bits += vb.glue_bits;
				v.time_bits += vb.time_bits;
				v.floor_bits += vb.floor_bits;
				v.res_bits += vb.res_bits;
				int num4;
				int num5;
				if (v.centerW != 0L)
				{
					num4 = num3;
					num5 = 0;
				}
				else
				{
					num4 = 0;
					num5 = num3;
				}
				for (int i = 0; i < vi.channels; i++)
				{
					if (v.lW != 0L)
					{
						if (v.W != 0L)
						{
							float[] array = Xiph._vorbis_window_get(backend_state.window[1] - halfrate_flag);
							float[] array2 = v.pcm[i];
							int num6 = num5;
							float[] array3 = vb.pcm[i];
							for (int j = 0; j < num3; j++)
							{
								array2[num6 + j] = array2[num6 + j] * array[num3 - j - 1] + array3[j] * array[j];
							}
						}
						else
						{
							float[] array4 = Xiph._vorbis_window_get(backend_state.window[0] - halfrate_flag);
							float[] array5 = v.pcm[i];
							int num7 = num5 + num3 / 2 - num2 / 2;
							float[] array6 = vb.pcm[i];
							for (int j = 0; j < num2; j++)
							{
								array5[num7 + j] = array5[num7 + j] * array4[num2 - j - 1] + array6[j] * array4[j];
							}
						}
					}
					else if (v.W != 0L)
					{
						float[] array7 = Xiph._vorbis_window_get(backend_state.window[0] - halfrate_flag);
						float[] array8 = v.pcm[i];
						int num8 = num5;
						float[] array9 = vb.pcm[i];
						int num9 = num3 / 2 - num2 / 2;
						int j;
						for (j = 0; j < num2; j++)
						{
							array8[num8 + j] = array8[num8 + j] * array7[num2 - j - 1] + array9[num9 + j] * array7[j];
						}
						while (j < num3 / 2 + num2 / 2)
						{
							array8[num8 + j] = array9[num9 + j];
							j++;
						}
					}
					else
					{
						float[] array10 = Xiph._vorbis_window_get(backend_state.window[0] - halfrate_flag);
						float[] array11 = v.pcm[i];
						int num10 = num5;
						float[] array12 = vb.pcm[i];
						for (int j = 0; j < num2; j++)
						{
							array11[num10 + j] = array11[num10 + j] * array10[num2 - j - 1] + array12[j] * array10[j];
						}
					}
					float[] array13 = v.pcm[i];
					int num11 = num4;
					float[] array14 = vb.pcm[i];
					int num12 = num;
					for (int j = 0; j < num; j++)
					{
						array13[num11 + j] = array14[num12 + j];
					}
				}
				if (v.centerW != 0L)
				{
					v.centerW = 0L;
				}
				else
				{
					v.centerW = (long)num3;
				}
				if (v.pcm_returned == -1)
				{
					v.pcm_returned = num4;
					v.pcm_current = num4;
				}
				else
				{
					v.pcm_returned = num5;
					v.pcm_current = (int)((long)num5 + (codec_setup.blocksizes[(int)(checked((IntPtr)v.lW))] / 4L + codec_setup.blocksizes[(int)(checked((IntPtr)v.W))] / 4L >> halfrate_flag));
				}
			}
			if (backend_state.sample_count == -1L)
			{
				backend_state.sample_count = 0L;
			}
			else
			{
				backend_state.sample_count += codec_setup.blocksizes[(int)(checked((IntPtr)v.lW))] / 4L + codec_setup.blocksizes[(int)(checked((IntPtr)v.W))] / 4L;
			}
			if (v.granulepos == -1L)
			{
				if (vb.granulepos != -1L)
				{
					v.granulepos = vb.granulepos;
					if (backend_state.sample_count > v.granulepos)
					{
						long num13 = backend_state.sample_count - vb.granulepos;
						if (num13 < 0L)
						{
							num13 = 0L;
						}
						int arg_4A6_0 = vb.eofflag;
						if (num13 > (long)((long)(v.pcm_current - v.pcm_returned) << halfrate_flag))
						{
							num13 = (long)((long)(v.pcm_current - v.pcm_returned) << halfrate_flag);
						}
						v.pcm_current -= (int)(num13 >> halfrate_flag);
					}
				}
			}
			else
			{
				v.granulepos += codec_setup.blocksizes[(int)(checked((IntPtr)v.lW))] / 4L + codec_setup.blocksizes[(int)(checked((IntPtr)v.W))] / 4L;
				if (vb.granulepos != -1L && v.granulepos != vb.granulepos)
				{
					if (v.granulepos > vb.granulepos)
					{
						long num14 = v.granulepos - vb.granulepos;
						if (num14 != 0L && vb.eofflag != 0)
						{
							if (num14 > (long)((long)(v.pcm_current - v.pcm_returned) << halfrate_flag))
							{
								num14 = (long)((long)(v.pcm_current - v.pcm_returned) << halfrate_flag);
							}
							if (num14 < 0L)
							{
								num14 = 0L;
							}
							v.pcm_current -= (int)(num14 >> halfrate_flag);
						}
					}
					v.granulepos = vb.granulepos;
				}
			}
			if (vb.eofflag != 0)
			{
				v.eofflag = 1;
			}
			return 0;
		}

		public static int vorbis_synthesis_pcmout(Xiph.vorbis_dsp_state v, out float[][] pcm, out int[] pcmret)
		{
			Xiph.vorbis_info vi = v.vi;
			pcm = v.pcm;
			pcmret = v.pcmret;
			if (v.pcm_returned > -1 && v.pcm_returned < v.pcm_current)
			{
				if (pcm != null)
				{
					for (int i = 0; i < vi.channels; i++)
					{
						v.pcmret[i] = v.pcm_returned;
					}
				}
				return v.pcm_current - v.pcm_returned;
			}
			return 0;
		}

		public static int vorbis_synthesis_read(Xiph.vorbis_dsp_state v, int n)
		{
			if (n != 0 && v.pcm_returned + n > v.pcm_current)
			{
				return -131;
			}
			v.pcm_returned += n;
			return 0;
		}

		public static int vorbis_synthesis_lapout(Xiph.vorbis_dsp_state v, out float[][] pcm, out int[] pcmret)
		{
			Xiph.vorbis_info vi = v.vi;
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			int halfrate_flag = codec_setup.halfrate_flag;
			int num = (int)(codec_setup.blocksizes[(int)(checked((IntPtr)v.W))] >> halfrate_flag + 1);
			int num2 = (int)(codec_setup.blocksizes[0] >> halfrate_flag + 1);
			int num3 = (int)(codec_setup.blocksizes[1] >> halfrate_flag + 1);
			pcm = v.pcm;
			pcmret = v.pcmret;
			if (v.pcm_returned < 0)
			{
				return 0;
			}
			if (v.centerW == (long)num3)
			{
				for (int i = 0; i < vi.channels; i++)
				{
					float[] array = v.pcm[i];
					for (int j = 0; j < num3; j++)
					{
						float num4 = array[j];
						array[j] = array[j + num3];
						array[j + num3] = num4;
					}
				}
				v.pcm_current -= num3;
				v.pcm_returned -= num3;
				v.centerW = 0L;
			}
			if ((v.lW ^ v.W) == 1L)
			{
				for (int i = 0; i < vi.channels; i++)
				{
					float[] array2 = v.pcm[i];
					float[] array3 = v.pcm[i];
					int num5 = (num3 - num2) / 2;
					for (int j = (num3 + num2) / 2 - 1; j >= 0; j--)
					{
						array3[num5 + j] = array2[j];
					}
				}
				v.pcm_returned += (num3 - num2) / 2;
				v.pcm_current += (num3 - num2) / 2;
			}
			else if (v.lW == 0L)
			{
				for (int i = 0; i < vi.channels; i++)
				{
					float[] array4 = v.pcm[i];
					float[] array5 = v.pcm[i];
					int num6 = num3 - num2;
					for (int j = num2 - 1; j >= 0; j--)
					{
						array5[num6 + j] = array4[j];
					}
				}
				v.pcm_returned += num3 - num2;
				v.pcm_current += num3 - num2;
			}
			for (int j = 0; j < vi.channels; j++)
			{
				v.pcmret[j] = v.pcm_returned;
			}
			return num3 + num - v.pcm_returned;
		}

		public static float[] vorbis_window(Xiph.vorbis_dsp_state v, int W)
		{
			Xiph.vorbis_info vi = v.vi;
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			int halfrate_flag = codec_setup.halfrate_flag;
			Xiph.private_state backend_state = v.backend_state;
			if (backend_state.window[W] - 1 < 0)
			{
				return null;
			}
			return Xiph._vorbis_window_get(backend_state.window[W] - halfrate_flag);
		}

		private static Xiph.static_codebook vorbis_staticbook_unpack(Xiph.oggpack_buffer opb)
		{
			Xiph.static_codebook static_codebook = new Xiph.static_codebook();
			static_codebook.allocedp = 1;
			if (Xiph.oggpack_read(opb, 24) == 5653314L)
			{
				static_codebook.dim = Xiph.oggpack_read(opb, 16);
				static_codebook.entries = Xiph.oggpack_read(opb, 24);
				if (static_codebook.entries != -1L && Xiph._ilog(static_codebook.dim) + Xiph._ilog(static_codebook.entries) <= 24)
				{
					switch ((int)Xiph.oggpack_read(opb, 1))
					{
					case 0:
					{
						long num = Xiph.oggpack_read(opb, 1);
						if (static_codebook.entries * ((num != 0L) ? 1L : 5L) + 7L >> 3 > opb.storage - Xiph.oggpack_bytes(opb))
						{
							goto IL_32E;
						}
						static_codebook.lengthlist = new long[static_codebook.entries];
						if (num != 0L)
						{
							for (long num2 = 0L; num2 < static_codebook.entries; num2 += 1L)
							{
								if (Xiph.oggpack_read(opb, 1) != 0L)
								{
									long num3 = Xiph.oggpack_read(opb, 5);
									if (num3 == -1L)
									{
										goto IL_32E;
									}
									static_codebook.lengthlist[(int)(checked((IntPtr)num2))] = num3 + 1L;
								}
								else
								{
									static_codebook.lengthlist[(int)(checked((IntPtr)num2))] = 0L;
								}
							}
						}
						else
						{
							for (long num2 = 0L; num2 < static_codebook.entries; num2 += 1L)
							{
								long num4 = Xiph.oggpack_read(opb, 5);
								if (num4 == -1L)
								{
									goto IL_32E;
								}
								static_codebook.lengthlist[(int)(checked((IntPtr)num2))] = num4 + 1L;
							}
						}
						break;
					}
					case 1:
					{
						long num5 = Xiph.oggpack_read(opb, 5) + 1L;
						if (num5 == 0L)
						{
							goto IL_32E;
						}
						static_codebook.lengthlist = new long[static_codebook.entries];
						long num2 = 0L;
						while (num2 < static_codebook.entries)
						{
							long num6 = Xiph.oggpack_read(opb, Xiph._ilog(static_codebook.entries - num2));
							if (num6 == -1L || num5 > 32L || num6 > static_codebook.entries - num2 || (num6 > 0L && num6 - 1L >> (int)num5 - 1 > 1L) || num5 > 32L)
							{
								goto IL_32E;
							}
							long num7 = 0L;
							while (num7 < num6)
							{
								static_codebook.lengthlist[(int)(checked((IntPtr)num2))] = num5;
								num7 += 1L;
								num2 += 1L;
							}
							num5 += 1L;
						}
						break;
					}
					default:
						goto IL_32E;
					}
					switch (static_codebook.maptype = (int)Xiph.oggpack_read(opb, 4))
					{
					case 0:
						break;
					case 1:
					case 2:
					{
						static_codebook.q_min = Xiph.oggpack_read(opb, 32);
						static_codebook.q_delta = Xiph.oggpack_read(opb, 32);
						static_codebook.q_quant = (int)Xiph.oggpack_read(opb, 4) + 1;
						static_codebook.q_sequencep = (int)Xiph.oggpack_read(opb, 1);
						if (static_codebook.q_sequencep == -1)
						{
							goto IL_32E;
						}
						int num8 = 0;
						switch (static_codebook.maptype)
						{
						case 1:
							num8 = ((static_codebook.dim == 0L) ? 0 : ((int)Xiph._book_maptype1_quantvals(static_codebook)));
							break;
						case 2:
							num8 = (int)(static_codebook.entries * static_codebook.dim);
							break;
						}
						if ((long)(num8 * static_codebook.q_quant + 7 >> 3) > opb.storage - Xiph.oggpack_bytes(opb))
						{
							goto IL_32E;
						}
						static_codebook.quantlist = new long[num8];
						for (long num2 = 0L; num2 < (long)num8; num2 += 1L)
						{
							static_codebook.quantlist[(int)(checked((IntPtr)num2))] = Xiph.oggpack_read(opb, static_codebook.q_quant);
						}
						if (num8 != 0 && static_codebook.quantlist[num8 - 1] == -1L)
						{
							goto IL_32E;
						}
						break;
					}
					default:
						goto IL_32E;
					}
					return static_codebook;
				}
			}
			IL_32E:
			Xiph.vorbis_staticbook_destroy(static_codebook);
			return null;
		}

		public static int vorbis_book_encode(Xiph.codebook book, int a, Xiph.oggpack_buffer b)
		{
			if (a < 0 || (long)a >= book.c.entries)
			{
				return 0;
			}
			Xiph.oggpack_write(b, (ulong)book.codelist[a], (int)book.c.lengthlist[a]);
			return (int)book.c.lengthlist[a];
		}

		private static uint bitreverse(uint x)
		{
			x = ((x >> 16 & 65535u) | (x << 16 & 4294901760u));
			x = ((x >> 8 & 16711935u) | (x << 8 & 4278255360u));
			x = ((x >> 4 & 252645135u) | (x << 4 & 4042322160u));
			x = ((x >> 2 & 858993459u) | (x << 2 & 3435973836u));
			return (x >> 1 & 1431655765u) | (x << 1 & 2863311530u);
		}

		private static long decode_packed_entry_number(Xiph.codebook book, Xiph.oggpack_buffer b)
		{
			int num = book.dec_maxlength;
			long num2 = 0L;
			int num3 = book.dec_firsttablen;
			bool flag = false;
			if (num3 < 0 || num3 > 32)
			{
				flag = true;
				num2 = -1L;
			}
			if (!flag)
			{
				ulong num4 = Xiph.mask[num3];
				num3 += b.endbit;
				if (b.endbyte >= b.storage - 4L)
				{
					if (b.endbyte > b.storage - (long)(num3 + 7 >> 3))
					{
						flag = true;
						num2 = -1L;
					}
					else if (num3 == 0)
					{
						flag = true;
						num2 = 0L;
					}
				}
				if (!flag)
				{
					ulong num5 = (ulong)((long)(b.ptr.data[b.ptr.offset] >> b.endbit));
					if (num3 > 8)
					{
						num5 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 1] << 8 - b.endbit));
						if (num3 > 16)
						{
							num5 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 2] << 16 - b.endbit));
							if (num3 > 24)
							{
								num5 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 3] << 24 - b.endbit));
								if (num3 > 32 && b.endbit != 0)
								{
									num5 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 4] << 32 - b.endbit));
								}
							}
						}
					}
					num2 = (long)(num4 & num5);
				}
			}
			long num8;
			long num9;
			if (num2 >= 0L)
			{
				long num6 = (long)((ulong)book.dec_firsttable[(int)(checked((IntPtr)num2))]);
				if ((num6 & -2147483648L) == 0L)
				{
					int num7 = (int)book.dec_codelengths[(int)(checked((IntPtr)(unchecked(num6 - 1L))))];
					num7 += b.endbit;
					if (b.endbyte > b.storage - (long)(num7 + 7 >> 3))
					{
						b.ptr.data = null;
						b.ptr.offset = 0;
						b.endbyte = b.storage;
						b.endbit = 1;
					}
					else
					{
						b.ptr.offset = b.ptr.offset + num7 / 8;
						b.endbyte += (long)(num7 / 8);
						b.endbit = (num7 & 7);
					}
					return num6 - 1L;
				}
				num8 = (num6 >> 15 & 32767L);
				num9 = book.used_entries - (num6 & 32767L);
			}
			else
			{
				num8 = 0L;
				num9 = book.used_entries;
			}
			int num10 = num;
			bool flag2 = false;
			if (num10 < 0 || num10 > 32)
			{
				flag2 = true;
				num2 = -1L;
			}
			if (!flag2)
			{
				ulong num11 = Xiph.mask[num10];
				num10 += b.endbit;
				if (b.endbyte >= b.storage - 4L)
				{
					if (b.endbyte > b.storage - (long)(num10 + 7 >> 3))
					{
						flag2 = true;
						num2 = -1L;
					}
					else if (num10 == 0)
					{
						flag2 = true;
						num2 = 0L;
					}
				}
				if (!flag2)
				{
					ulong num12 = (ulong)((long)(b.ptr.data[b.ptr.offset] >> b.endbit));
					if (num10 > 8)
					{
						num12 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 1] << 8 - b.endbit));
						if (num10 > 16)
						{
							num12 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 2] << 16 - b.endbit));
							if (num10 > 24)
							{
								num12 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 3] << 24 - b.endbit));
								if (num10 > 32 && b.endbit != 0)
								{
									num12 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 4] << 32 - b.endbit));
								}
							}
						}
					}
					num2 = (long)(num11 & num12);
				}
			}
			while (num2 < 0L && num > 1)
			{
				int num13;
				num = (num13 = num - 1);
				bool flag3 = false;
				if (num13 < 0 || num13 > 32)
				{
					flag3 = true;
					num2 = -1L;
				}
				if (!flag3)
				{
					ulong num14 = Xiph.mask[num13];
					num13 += b.endbit;
					if (b.endbyte >= b.storage - 4L)
					{
						if (b.endbyte > b.storage - (long)(num13 + 7 >> 3))
						{
							flag3 = true;
							num2 = -1L;
						}
						else if (num13 == 0)
						{
							flag3 = true;
							num2 = 0L;
						}
					}
					if (!flag3)
					{
						ulong num15 = (ulong)((long)(b.ptr.data[b.ptr.offset] >> b.endbit));
						if (num13 > 8)
						{
							num15 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 1] << 8 - b.endbit));
							if (num13 > 16)
							{
								num15 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 2] << 16 - b.endbit));
								if (num13 > 24)
								{
									num15 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 3] << 24 - b.endbit));
									if (num13 > 32 && b.endbit != 0)
									{
										num15 |= (ulong)((long)((long)b.ptr.data[b.ptr.offset + 4] << 32 - b.endbit));
									}
								}
							}
						}
						num2 = (long)(num14 & num15);
					}
				}
			}
			if (num2 < 0L)
			{
				return -1L;
			}
			uint num16 = (uint)num2;
			num16 = ((num16 >> 16 & 65535u) | (num16 << 16 & 4294901760u));
			num16 = ((num16 >> 8 & 16711935u) | (num16 << 8 & 4278255360u));
			num16 = ((num16 >> 4 & 252645135u) | (num16 << 4 & 4042322160u));
			num16 = ((num16 >> 2 & 858993459u) | (num16 << 2 & 3435973836u));
			num16 = ((num16 >> 1 & 1431655765u) | (num16 << 1 & 2863311530u));
			uint num17 = num16;
			while (num9 - num8 > 1L)
			{
				long num18 = num9 - num8 >> 1;
				long num19 = (book.codelist[(int)(checked((IntPtr)(unchecked(num8 + num18))))] > num17) ? 1L : 0L;
				num8 += (num18 & num19 - 1L);
				num9 -= (num18 & -num19);
			}
			if ((int)book.dec_codelengths[(int)(checked((IntPtr)num8))] <= num)
			{
				int num20 = (int)book.dec_codelengths[(int)(checked((IntPtr)num8))];
				num20 += b.endbit;
				if (b.endbyte > b.storage - (long)(num20 + 7 >> 3))
				{
					b.ptr.data = null;
					b.ptr.offset = 0;
					b.endbyte = b.storage;
					b.endbit = 1;
				}
				else
				{
					b.ptr.offset = b.ptr.offset + num20 / 8;
					b.endbyte += (long)(num20 / 8);
					b.endbit = (num20 & 7);
				}
				return num8;
			}
			Xiph.oggpack_adv(b, num);
			return -1L;
		}

		public static long vorbis_book_decode(Xiph.codebook book, Xiph.oggpack_buffer b)
		{
			if (book.used_entries > 0L)
			{
				long num = Xiph.decode_packed_entry_number(book, b);
				if (num >= 0L)
				{
					return (long)book.dec_index[(int)(checked((IntPtr)num))];
				}
			}
			return -1L;
		}

		public static long vorbis_book_decodevs_add(Xiph.codebook book, float[] a, int ao, Xiph.oggpack_buffer b, int n)
		{
			if (book.used_entries > 0L)
			{
				int num = (int)((long)n / book.dim);
				long[] array = new long[num];
				long[] array2 = new long[num];
				int i;
				for (i = 0; i < num; i++)
				{
					array[i] = Xiph.decode_packed_entry_number(book, b);
					if (array[i] == -1L)
					{
						return -1L;
					}
					array2[i] = array[i] * book.dim;
				}
				i = 0;
				int num2 = 0;
				while ((long)i < book.dim)
				{
					for (int j = 0; j < num; j++)
					{
						a[ao + num2 + j] += book.valuelist[(int)(checked((IntPtr)(unchecked(array2[j] + (long)i))))];
					}
					i++;
					num2 += num;
				}
			}
			return 0L;
		}

		public static long vorbis_book_decodev_add(Xiph.codebook book, float[] a, int ao, Xiph.oggpack_buffer b, int n)
		{
			if (book.used_entries > 0L)
			{
				if (book.dim > 8L)
				{
					int i = 0;
					while (i < n)
					{
						int num = (int)Xiph.decode_packed_entry_number(book, b);
						if (num == -1)
						{
							return -1L;
						}
						int num2 = (int)((long)num * book.dim);
						int num3 = 0;
						while ((long)num3 < book.dim)
						{
							a[ao + i++] += book.valuelist[num2 + num3++];
						}
					}
				}
				else
				{
					int i = 0;
					while (i < n)
					{
						int num = (int)Xiph.decode_packed_entry_number(book, b);
						if (num == -1)
						{
							return -1L;
						}
						int num2 = (int)((long)num * book.dim);
						int num3 = 0;
						switch ((int)book.dim)
						{
						case 0:
							continue;
						case 1:
							break;
						case 2:
							goto IL_1BC;
						case 3:
							goto IL_195;
						case 4:
							goto IL_16E;
						case 5:
							goto IL_147;
						case 6:
							goto IL_120;
						case 7:
							goto IL_F9;
						case 8:
							a[ao + i++] += book.valuelist[num2 + num3++];
							goto IL_F9;
						default:
							continue;
						}
						IL_1E3:
						a[ao + i++] += book.valuelist[num2 + num3++];
						continue;
						IL_1BC:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_1E3;
						IL_195:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_1BC;
						IL_16E:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_195;
						IL_147:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_16E;
						IL_120:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_147;
						IL_F9:
						a[ao + i++] += book.valuelist[num2 + num3++];
						goto IL_120;
					}
				}
			}
			return 0L;
		}

		public static long vorbis_book_decodev_set(Xiph.codebook book, float[] a, int ao, Xiph.oggpack_buffer b, int n)
		{
			if (book.used_entries > 0L)
			{
				int i = 0;
				while (i < n)
				{
					int num = (int)Xiph.decode_packed_entry_number(book, b);
					if (num == -1)
					{
						return -1L;
					}
					int num2 = (int)((long)num * book.dim);
					int num3 = 0;
					while (i < n && (long)num3 < book.dim)
					{
						a[ao + i++] = book.valuelist[num2 + num3++];
					}
				}
			}
			else
			{
				int j = 0;
				while (j < n)
				{
					a[ao + j++] = 0f;
				}
			}
			return 0L;
		}

		public static long vorbis_book_decodevv_add(Xiph.codebook book, float[][] a, long offset, int ch, Xiph.oggpack_buffer b, int n)
		{
			int num = 0;
			if (book.used_entries > 0L)
			{
				long num2 = offset / (long)ch;
				while (num2 < (offset + (long)n) / (long)ch)
				{
					long num3 = (long)((int)Xiph.decode_packed_entry_number(book, b));
					if (num3 == -1L)
					{
						return -1L;
					}
					int num4 = (int)(num3 * book.dim);
					for (long num5 = 0L; num5 < book.dim; num5 += 1L)
					{
						a[num++][(int)(checked((IntPtr)num2))] += book.valuelist[(int)(checked((IntPtr)(unchecked((long)num4 + num5))))];
						if (num == ch)
						{
							num = 0;
							num2 += 1L;
						}
					}
				}
			}
			return 0L;
		}

		private static void _ve_envelope_clear(Xiph.envelope_lookup e)
		{
			Xiph.mdct_clear(e.mdct);
			for (int i = 0; i < 7; i++)
			{
				e.band[i].window = null;
			}
			e.mdct_win = null;
			e.filter = null;
			e.mark = null;
			e.ch = 0;
			e.curmark = 0L;
			e.cursor = 0L;
			e.minenergy = 0f;
			e.searchstep = 0;
			e.storage = 0L;
			e.stretch = 0;
			e.winlength = 0;
		}

		private static void floor0_free_info(Xiph.vorbis_info_floor i)
		{
		}

		private static void floor0_free_look(Xiph.vorbis_look_floor i)
		{
		}

		private static Xiph.vorbis_info_floor floor0_unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			Xiph.vorbis_info_floor0 vorbis_info_floor = new Xiph.vorbis_info_floor0();
			vorbis_info_floor.order = (int)Xiph.oggpack_read(opb, 8);
			vorbis_info_floor.rate = Xiph.oggpack_read(opb, 16);
			vorbis_info_floor.barkmap = Xiph.oggpack_read(opb, 16);
			vorbis_info_floor.ampbits = (int)Xiph.oggpack_read(opb, 6);
			vorbis_info_floor.ampdB = (int)Xiph.oggpack_read(opb, 8);
			vorbis_info_floor.numbooks = (int)(Xiph.oggpack_read(opb, 4) + 1L);
			if (vorbis_info_floor.order >= 1 && vorbis_info_floor.rate >= 1L && vorbis_info_floor.barkmap >= 1L && vorbis_info_floor.numbooks >= 1)
			{
				for (int i = 0; i < vorbis_info_floor.numbooks; i++)
				{
					vorbis_info_floor.books[i] = (int)Xiph.oggpack_read(opb, 8);
					if (vorbis_info_floor.books[i] < 0 || vorbis_info_floor.books[i] >= codec_setup.books || codec_setup.book_param[vorbis_info_floor.books[i]].maptype == 0 || codec_setup.book_param[vorbis_info_floor.books[i]].dim < 1L)
					{
						goto IL_F9;
					}
				}
				return vorbis_info_floor;
			}
			IL_F9:
			Xiph.floor0_free_info(vorbis_info_floor);
			return null;
		}

		private static void floor0_map_lazy_init(Xiph.vorbis_block vb, Xiph.vorbis_info_floor infoX, Xiph.vorbis_look_floor0 look)
		{
			if (look.linearmap[(int)(checked((IntPtr)vb.W))] == null)
			{
				Xiph.vorbis_dsp_state vd = vb.vd;
				Xiph.vorbis_info vi = vd.vi;
				Xiph.codec_setup_info codec_setup = vi.codec_setup;
				Xiph.vorbis_info_floor0 vorbis_info_floor = (Xiph.vorbis_info_floor0)infoX;
				int num = (int)vb.W;
				int num2 = (int)codec_setup.blocksizes[num] / 2;
				float num3 = (float)look.ln / Xiph.toBARK((float)vorbis_info_floor.rate / 2f);
				look.linearmap[num] = new int[num2 + 1];
				int i;
				for (i = 0; i < num2; i++)
				{
					int num4 = (int)Xiph.floor(Xiph.toBARK((float)vorbis_info_floor.rate / 2f / (float)num2 * (float)i) * num3);
					if (num4 >= look.ln)
					{
						num4 = look.ln - 1;
					}
					look.linearmap[num][i] = num4;
				}
				look.linearmap[num][i] = -1;
				look.n[num] = num2;
			}
		}

		private static Xiph.vorbis_look_floor floor0_look(Xiph.vorbis_dsp_state vd, Xiph.vorbis_info_floor i)
		{
			Xiph.vorbis_info_floor0 vorbis_info_floor = (Xiph.vorbis_info_floor0)i;
			return new Xiph.vorbis_look_floor0
			{
				m = vorbis_info_floor.order,
				ln = (int)vorbis_info_floor.barkmap,
				vi = vorbis_info_floor,
				linearmap = new int[2][]
			};
		}

		private static object floor0_inverse1(Xiph.vorbis_block vb, Xiph.vorbis_look_floor i)
		{
			Xiph.vorbis_look_floor0 vorbis_look_floor = (Xiph.vorbis_look_floor0)i;
			Xiph.vorbis_info_floor0 vi = vorbis_look_floor.vi;
			int num = (int)Xiph.oggpack_read(vb.opb, vi.ampbits);
			if (num > 0)
			{
				long num2 = (long)((1 << vi.ampbits) - 1);
				float num3 = (float)num / (float)num2 * (float)vi.ampdB;
				int num4 = (int)Xiph.oggpack_read(vb.opb, Xiph._ilog((long)vi.numbooks));
				if (num4 != -1 && num4 < vi.numbooks)
				{
					Xiph.codec_setup_info codec_setup = vb.vd.vi.codec_setup;
					Xiph.codebook codebook = codec_setup.fullbooks[vi.books[num4]];
					float num5 = 0f;
					float[] array = new float[(long)vorbis_look_floor.m + codebook.dim + 1L];
					if (Xiph.vorbis_book_decodev_set(codebook, array, 0, vb.opb, vorbis_look_floor.m) != -1L)
					{
						int j = 0;
						while (j < vorbis_look_floor.m)
						{
							int num6 = 0;
							while (j < vorbis_look_floor.m && (long)num6 < codebook.dim)
							{
								array[j] += num5;
								num6++;
								j++;
							}
							num5 = array[j - 1];
						}
						array[vorbis_look_floor.m] = num3;
						return array;
					}
				}
			}
			return null;
		}

		private static int floor0_inverse2(Xiph.vorbis_block vb, Xiph.vorbis_look_floor i, object memo, float[] _out)
		{
			Xiph.vorbis_look_floor0 vorbis_look_floor = (Xiph.vorbis_look_floor0)i;
			Xiph.vorbis_info_floor0 vi = vorbis_look_floor.vi;
			Xiph.floor0_map_lazy_init(vb, vi, vorbis_look_floor);
			checked
			{
				if (memo != null)
				{
					float[] array = (float[])memo;
					float amp = array[vorbis_look_floor.m];
					Xiph.vorbis_lsp_to_curve(_out, vorbis_look_floor.linearmap[(int)((IntPtr)vb.W)], vorbis_look_floor.n[(int)((IntPtr)vb.W)], vorbis_look_floor.ln, array, vorbis_look_floor.m, amp, (float)vi.ampdB);
					return 1;
				}
			}
			for (int j = 0; j < vorbis_look_floor.n[(int)(checked((IntPtr)vb.W))]; j++)
			{
				_out[j] = 0f;
			}
			return 0;
		}

		private static void floor1_free_info(Xiph.vorbis_info_floor i)
		{
		}

		private static void floor1_free_look(Xiph.vorbis_look_floor i)
		{
		}

		private static void floor1_pack(Xiph.vorbis_info_floor i, Xiph.oggpack_buffer opb)
		{
			Xiph.vorbis_info_floor1 vorbis_info_floor = (Xiph.vorbis_info_floor1)i;
			int num = 0;
			int v = vorbis_info_floor.postlist[1];
			int num2 = -1;
			Xiph.oggpack_write(opb, (long)vorbis_info_floor.partitions, 5);
			int j;
			for (j = 0; j < vorbis_info_floor.partitions; j++)
			{
				Xiph.oggpack_write(opb, (long)vorbis_info_floor.partitionclass[j], 4);
				if (num2 < vorbis_info_floor.partitionclass[j])
				{
					num2 = vorbis_info_floor.partitionclass[j];
				}
			}
			int k;
			for (j = 0; j < num2 + 1; j++)
			{
				Xiph.oggpack_write(opb, (long)(vorbis_info_floor.class_dim[j] - 1), 3);
				Xiph.oggpack_write(opb, (long)vorbis_info_floor.class_subs[j], 2);
				if (vorbis_info_floor.class_subs[j] != 0)
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_floor.class_book[j], 8);
				}
				for (k = 0; k < 1 << vorbis_info_floor.class_subs[j]; k++)
				{
					Xiph.oggpack_write(opb, (long)(vorbis_info_floor.class_subbook[j][k] + 1), 8);
				}
			}
			Xiph.oggpack_write(opb, (long)(vorbis_info_floor.mult - 1), 2);
			Xiph.oggpack_write(opb, (long)Xiph.ilog2((uint)v), 4);
			int bits = Xiph.ilog2((uint)v);
			j = 0;
			k = 0;
			while (j < vorbis_info_floor.partitions)
			{
				num += vorbis_info_floor.class_dim[vorbis_info_floor.partitionclass[j]];
				while (k < num)
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_floor.postlist[k + 2], bits);
					k++;
				}
				j++;
			}
		}

		private static void sortpointer_qsort(int[] q, int[] sort, int left, int right)
		{
			int i = left;
			int num = right;
			int num2 = sort[(left + right) / 2];
			while (i <= num)
			{
				while (q[sort[i]] < q[num2])
				{
					i++;
				}
				while (q[sort[num]] > q[num2])
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = sort[i];
					sort[i] = sort[num];
					sort[num] = num3;
					i++;
					num--;
				}
			}
			if (left < num)
			{
				Xiph.sortpointer_qsort(q, sort, left, num);
			}
			if (i < right)
			{
				Xiph.sortpointer_qsort(q, sort, i, right);
			}
		}

		private static Xiph.vorbis_info_floor floor1_unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			int num = 0;
			int num2 = -1;
			Xiph.vorbis_info_floor1 vorbis_info_floor = new Xiph.vorbis_info_floor1();
			vorbis_info_floor.partitions = (int)Xiph.oggpack_read(opb, 5);
			for (int i = 0; i < vorbis_info_floor.partitions; i++)
			{
				vorbis_info_floor.partitionclass[i] = (int)Xiph.oggpack_read(opb, 4);
				if (vorbis_info_floor.partitionclass[i] < 0)
				{
					IL_23B:
					return null;
				}
				if (num2 < vorbis_info_floor.partitionclass[i])
				{
					num2 = vorbis_info_floor.partitionclass[i];
				}
			}
			for (int i = 0; i < num2 + 1; i++)
			{
				vorbis_info_floor.class_dim[i] = (int)(Xiph.oggpack_read(opb, 3) + 1L);
				vorbis_info_floor.class_subs[i] = (int)Xiph.oggpack_read(opb, 2);
				if (vorbis_info_floor.class_subs[i] < 0)
				{
					goto IL_23B;
				}
				if (vorbis_info_floor.class_subs[i] != 0)
				{
					vorbis_info_floor.class_book[i] = (int)Xiph.oggpack_read(opb, 8);
				}
				if (vorbis_info_floor.class_book[i] < 0 || vorbis_info_floor.class_book[i] >= codec_setup.books)
				{
					goto IL_23B;
				}
				for (int j = 0; j < 1 << vorbis_info_floor.class_subs[i]; j++)
				{
					vorbis_info_floor.class_subbook[i][j] = (int)(Xiph.oggpack_read(opb, 8) - 1L);
					if (vorbis_info_floor.class_subbook[i][j] < -1 || vorbis_info_floor.class_subbook[i][j] >= codec_setup.books)
					{
						goto IL_23B;
					}
				}
			}
			vorbis_info_floor.mult = (int)(Xiph.oggpack_read(opb, 2) + 1L);
			int num3 = (int)Xiph.oggpack_read(opb, 4);
			if (num3 >= 0)
			{
				int i = 0;
				int j = 0;
				while (i < vorbis_info_floor.partitions)
				{
					num += vorbis_info_floor.class_dim[vorbis_info_floor.partitionclass[i]];
					if (num > 63)
					{
						goto IL_23B;
					}
					while (j < num)
					{
						int num4 = vorbis_info_floor.postlist[j + 2] = (int)Xiph.oggpack_read(opb, num3);
						if (num4 < 0 || num4 >= 1 << num3)
						{
							goto IL_23B;
						}
						j++;
					}
					i++;
				}
				vorbis_info_floor.postlist[0] = 0;
				vorbis_info_floor.postlist[1] = 1 << num3;
				int[] array = new int[65];
				for (i = 0; i < num + 2; i++)
				{
					array[i] = i;
				}
				Xiph.sortpointer_qsort(vorbis_info_floor.postlist, array, 0, num + 1);
				for (i = 1; i < num + 2; i++)
				{
					if (array[i - 1] == array[i])
					{
						goto IL_23B;
					}
				}
				return vorbis_info_floor;
			}
			goto IL_23B;
		}

		private static Xiph.vorbis_look_floor floor1_look(Xiph.vorbis_dsp_state vd, Xiph.vorbis_info_floor _in)
		{
			int[] array = new int[65];
			Xiph.vorbis_info_floor1 vorbis_info_floor = (Xiph.vorbis_info_floor1)_in;
			Xiph.vorbis_look_floor1 vorbis_look_floor = new Xiph.vorbis_look_floor1();
			int num = 0;
			vorbis_look_floor.vi = vorbis_info_floor;
			vorbis_look_floor.n = vorbis_info_floor.postlist[1];
			for (int i = 0; i < vorbis_info_floor.partitions; i++)
			{
				num += vorbis_info_floor.class_dim[vorbis_info_floor.partitionclass[i]];
			}
			num += 2;
			vorbis_look_floor.posts = num;
			for (int i = 0; i < num; i++)
			{
				array[i] = i;
			}
			Xiph.sortpointer_qsort(vorbis_info_floor.postlist, array, 0, num - 1);
			for (int i = 0; i < num; i++)
			{
				vorbis_look_floor.forward_index[i] = array[i];
			}
			for (int i = 0; i < num; i++)
			{
				vorbis_look_floor.reverse_index[vorbis_look_floor.forward_index[i]] = i;
			}
			for (int i = 0; i < num; i++)
			{
				vorbis_look_floor.sorted_index[i] = vorbis_info_floor.postlist[vorbis_look_floor.forward_index[i]];
			}
			switch (vorbis_info_floor.mult)
			{
			case 1:
				vorbis_look_floor.quant_q = 256;
				break;
			case 2:
				vorbis_look_floor.quant_q = 128;
				break;
			case 3:
				vorbis_look_floor.quant_q = 86;
				break;
			case 4:
				vorbis_look_floor.quant_q = 64;
				break;
			}
			for (int i = 0; i < num - 2; i++)
			{
				int num2 = 0;
				int num3 = 1;
				int num4 = 0;
				int num5 = vorbis_look_floor.n;
				int num6 = vorbis_info_floor.postlist[i + 2];
				for (int j = 0; j < i + 2; j++)
				{
					int num7 = vorbis_info_floor.postlist[j];
					if (num7 > num4 && num7 < num6)
					{
						num2 = j;
						num4 = num7;
					}
					if (num7 < num5 && num7 > num6)
					{
						num3 = j;
						num5 = num7;
					}
				}
				vorbis_look_floor.loneighbor[i] = num2;
				vorbis_look_floor.hineighbor[i] = num3;
			}
			return vorbis_look_floor;
		}

		private static int render_point(int x0, int x1, int y0, int y1, int x)
		{
			y0 &= 32767;
			y1 &= 32767;
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Xiph.abs(num);
			int num4 = num3 * (x - x0);
			int num5 = num4 / num2;
			if (num < 0)
			{
				return y0 - num5;
			}
			return y0 + num5;
		}

		private static int vorbis_dBquant(float[] x, int xo)
		{
			int num = (int)(x[xo] * 7.31428576f + 1023.5f);
			if (num > 1023)
			{
				return 1023;
			}
			if (num < 0)
			{
				return 0;
			}
			return num;
		}

		private static void render_line(int n, int x0, int x1, int y0, int y1, float[] d, int _do)
		{
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Xiph.abs(num);
			int num4 = num / num2;
			int num5 = (num < 0) ? (num4 - 1) : (num4 + 1);
			int num6 = x0;
			int num7 = y0;
			int num8 = 0;
			num3 -= Xiph.abs(num4 * num2);
			if (n > x1)
			{
				n = x1;
			}
			if (num6 < n)
			{
				d[_do + num6] *= Xiph.FLOOR1_fromdB_LOOKUP[num7];
			}
			while (++num6 < n)
			{
				num8 += num3;
				if (num8 >= num2)
				{
					num8 -= num2;
					num7 += num5;
				}
				else
				{
					num7 += num4;
				}
				d[_do + num6] *= Xiph.FLOOR1_fromdB_LOOKUP[num7];
			}
		}

		private static void render_line0(int n, int x0, int x1, int y0, int y1, int[] d, int _do)
		{
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Xiph.abs(num);
			int num4 = num / num2;
			int num5 = (num < 0) ? (num4 - 1) : (num4 + 1);
			int num6 = x0;
			int num7 = y0;
			int num8 = 0;
			num3 -= Xiph.abs(num4 * num2);
			if (n > x1)
			{
				n = x1;
			}
			if (num6 < n)
			{
				d[_do + num6] = num7;
			}
			while (++num6 < n)
			{
				num8 += num3;
				if (num8 >= num2)
				{
					num8 -= num2;
					num7 += num5;
				}
				else
				{
					num7 += num4;
				}
				d[_do + num6] = num7;
			}
		}

		private static int accumulate_fit(float[] flr, int flro, float[] mdct, int mdcto, int x0, int x1, Xiph.lsfit_acc a, int n, Xiph.vorbis_info_floor1 info)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			a.clear();
			a.x0 = x0;
			a.x1 = x1;
			if (x1 >= n)
			{
				x1 = n - 1;
			}
			for (long num13 = (long)x0; num13 <= (long)x1; num13 += 1L)
			{
				int num14 = Xiph.vorbis_dBquant(flr, (int)((long)flro + num13));
				if (num14 != 0)
				{
					if (mdct[(int)(checked((IntPtr)(unchecked((long)mdcto + num13))))] + info.twofitatten >= flr[(int)(checked((IntPtr)(unchecked((long)flro + num13))))])
					{
						num += (int)num13;
						num2 += num14;
						num3 += (int)(num13 * num13);
						num4 += num14 * num14;
						num5 += (int)(num13 * (long)num14);
						num6++;
					}
					else
					{
						num7 += (int)num13;
						num8 += num14;
						num9 += (int)(num13 * num13);
						num10 += num14 * num14;
						num11 += (int)(num13 * (long)num14);
						num12++;
					}
				}
			}
			a.xa = num;
			a.ya = num2;
			a.x2a = num3;
			a.y2a = num4;
			a.xya = num5;
			a.an = num6;
			a.xb = num7;
			a.yb = num8;
			a.x2b = num9;
			a.y2b = num10;
			a.xyb = num11;
			a.bn = num12;
			return num6;
		}

		private static int fit_line(Xiph.lsfit_acc[] a, int ao, int fits, ref int y0, ref int y1, Xiph.vorbis_info_floor1 info)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			int x = a[ao].x0;
			int x2 = a[ao + fits - 1].x1;
			for (int i = 0; i < fits; i++)
			{
				double num7 = (double)((float)(a[ao + i].bn + a[ao + i].an) * info.twofitweight / (float)(a[ao + i].an + 1)) + 1.0;
				num += (double)a[ao + i].xb + (double)a[ao + i].xa * num7;
				num2 += (double)a[ao + i].yb + (double)a[ao + i].ya * num7;
				num3 += (double)a[ao + i].x2b + (double)a[ao + i].x2a * num7;
				num4 += (double)a[ao + i].y2b + (double)a[ao + i].y2a * num7;
				num5 += (double)a[ao + i].xyb + (double)a[ao + i].xya * num7;
				num6 += (double)a[ao + i].bn + (double)a[ao + i].an * num7;
			}
			if (y0 >= 0)
			{
				num += (double)x;
				num2 += (double)y0;
				num3 += (double)(x * x);
				num4 += (double)(y0 * y0);
				num5 += (double)(y0 * x);
				num6 += 1.0;
			}
			if (y1 >= 0)
			{
				num += (double)x2;
				num2 += (double)y1;
				num3 += (double)(x2 * x2);
				num4 += (double)(y1 * y1);
				num5 += (double)(y1 * x2);
				num6 += 1.0;
			}
			double num8 = num6 * num3 - num * num;
			if (num8 > 0.0)
			{
				double num9 = (num2 * num3 - num5 * num) / num8;
				double num10 = (num6 * num5 - num * num2) / num8;
				y0 = Xiph.rint(num9 + num10 * (double)x);
				y1 = Xiph.rint(num9 + num10 * (double)x2);
				if (y0 > 1023)
				{
					y0 = 1023;
				}
				if (y1 > 1023)
				{
					y1 = 1023;
				}
				if (y0 < 0)
				{
					y0 = 0;
				}
				if (y1 < 0)
				{
					y1 = 0;
				}
				return 0;
			}
			y0 = 0;
			y1 = 0;
			return 1;
		}

		private static int inspect_error(int x0, int x1, int y0, int y1, float[] mask, int masko, float[] mdct, int mdcto, Xiph.vorbis_info_floor1 info)
		{
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Xiph.abs(num);
			int num4 = num / num2;
			int num5 = (num < 0) ? (num4 - 1) : (num4 + 1);
			int num6 = x0;
			int num7 = y0;
			int num8 = 0;
			int num9 = Xiph.vorbis_dBquant(mask, masko + num6);
			int num10 = 0;
			num3 -= Xiph.abs(num4 * num2);
			int num11 = num7 - num9;
			num11 *= num11;
			num10++;
			if (mdct[mdcto + num6] + info.twofitatten >= mask[masko + num6])
			{
				if ((float)num7 + info.maxover < (float)num9)
				{
					return 1;
				}
				if ((float)num7 - info.maxunder > (float)num9)
				{
					return 1;
				}
			}
			while (++num6 < x1)
			{
				num8 += num3;
				if (num8 >= num2)
				{
					num8 -= num2;
					num7 += num5;
				}
				else
				{
					num7 += num4;
				}
				num9 = Xiph.vorbis_dBquant(mask, masko + num6);
				num11 += (num7 - num9) * (num7 - num9);
				num10++;
				if (mdct[mdcto + num6] + info.twofitatten >= mask[masko + num6] && num9 != 0)
				{
					if ((float)num7 + info.maxover < (float)num9)
					{
						return 1;
					}
					if ((float)num7 - info.maxunder > (float)num9)
					{
						return 1;
					}
				}
			}
			if (info.maxover * info.maxover / (float)num10 > info.maxerr)
			{
				return 0;
			}
			if (info.maxunder * info.maxunder / (float)num10 > info.maxerr)
			{
				return 0;
			}
			if ((float)(num11 / num10) > info.maxerr)
			{
				return 1;
			}
			return 0;
		}

		private static int post_Y(int[] A, int[] B, int pos)
		{
			if (A[pos] < 0)
			{
				return B[pos];
			}
			if (B[pos] < 0)
			{
				return A[pos];
			}
			return A[pos] + B[pos] >> 1;
		}

		private static int[] floor1_fit(Xiph.vorbis_block vb, Xiph.vorbis_look_floor1 look, float[] logmdct, int logmdcto, float[] logmask, int logmasko)
		{
			Xiph.vorbis_info_floor1 vi = look.vi;
			long num = (long)look.n;
			long num2 = (long)look.posts;
			long num3 = 0L;
			Xiph.lsfit_acc[] array = new Xiph.lsfit_acc[64];
			int[] array2 = new int[65];
			int[] array3 = new int[65];
			int[] array4 = new int[65];
			int[] array5 = new int[65];
			int[] array6 = null;
			int[] array7 = new int[65];
			for (long num4 = 0L; num4 < num2; num4 += 1L)
			{
				array2[(int)(checked((IntPtr)num4))] = -200;
			}
			for (long num4 = 0L; num4 < num2; num4 += 1L)
			{
				array3[(int)(checked((IntPtr)num4))] = -200;
			}
			for (long num4 = 0L; num4 < num2; num4 += 1L)
			{
				array4[(int)(checked((IntPtr)num4))] = 0;
			}
			for (long num4 = 0L; num4 < num2; num4 += 1L)
			{
				array5[(int)(checked((IntPtr)num4))] = 1;
			}
			for (long num4 = 0L; num4 < num2; num4 += 1L)
			{
				array7[(int)(checked((IntPtr)num4))] = -1;
			}
			if (num2 == 0L)
			{
				num3 += (long)Xiph.accumulate_fit(logmask, logmasko, logmdct, logmdcto, 0, (int)num, array[0], (int)num, vi);
			}
			else
			{
				for (long num4 = 0L; num4 < num2 - 1L; num4 += 1L)
				{
					num3 += (long)checked(Xiph.accumulate_fit(logmask, logmasko, logmdct, logmdcto, look.sorted_index[(int)((IntPtr)num4)], look.sorted_index[(int)((IntPtr)(unchecked(num4 + 1L)))], array[(int)((IntPtr)num4)], unchecked((int)num), vi));
				}
			}
			if (num3 != 0L)
			{
				int num5 = -200;
				int num6 = -200;
				Xiph.fit_line(array, 0, (int)(num2 - 1L), ref num5, ref num6, vi);
				array2[0] = num5;
				array3[0] = num5;
				array3[1] = num6;
				array2[1] = num6;
				for (long num4 = 2L; num4 < num2; num4 += 1L)
				{
					checked
					{
						int num7 = look.reverse_index[(int)((IntPtr)num4)];
						int num8 = array4[num7];
						int num9 = array5[num7];
						if (array7[num8] != num9)
						{
							int num10 = look.reverse_index[num8];
							int num11 = look.reverse_index[num9];
							array7[num8] = num9;
							int x = vi.postlist[num8];
							int x2 = vi.postlist[num9];
							int num12 = Xiph.post_Y(array2, array3, num8);
							int num13 = Xiph.post_Y(array2, array3, num9);
							if (num12 == -1 || num13 == -1)
							{
								Environment.Exit(1);
							}
							if (Xiph.inspect_error(x, x2, num12, num13, logmask, logmasko, logmdct, logmdcto, vi) != 0)
							{
								int num14 = -200;
								int num15 = -200;
								int num16 = -200;
								int num17 = -200;
								int num18;
								int num19;
								unchecked
								{
									num18 = Xiph.fit_line(array, num10, num7 - num10, ref num14, ref num15, vi);
									num19 = Xiph.fit_line(array, num7, num11 - num7, ref num16, ref num17, vi);
									if (num18 != 0)
									{
										num14 = num12;
										num15 = num16;
									}
									if (num19 != 0)
									{
										num16 = num15;
										num17 = num13;
									}
								}
								if (num18 != 0 && num19 != 0)
								{
									array2[(int)((IntPtr)num4)] = -200;
									array3[(int)((IntPtr)num4)] = -200;
								}
								else
								{
									array3[num8] = num14;
									if (num8 == 0)
									{
										array2[num8] = num14;
									}
									array2[(int)((IntPtr)num4)] = num15;
									array3[(int)((IntPtr)num4)] = num16;
									array2[num9] = num17;
									if (num9 == 1)
									{
										array3[num9] = num17;
									}
									unchecked
									{
										if (num15 >= 0 || num16 >= 0)
										{
											long num20 = (long)(num7 - 1);
											while (num20 >= 0L && array5[(int)(checked((IntPtr)num20))] == num9)
											{
												array5[(int)(checked((IntPtr)num20))] = (int)num4;
												num20 -= 1L;
											}
											for (num20 = (long)(num7 + 1); num20 < num2; num20 += 1L)
											{
												if (array4[(int)(checked((IntPtr)num20))] != num8)
												{
													break;
												}
												array4[(int)(checked((IntPtr)num20))] = (int)num4;
											}
										}
									}
								}
							}
							else
							{
								array2[(int)((IntPtr)num4)] = -200;
								array3[(int)((IntPtr)num4)] = -200;
							}
						}
					}
				}
				array6 = new int[num2];
				array6[0] = Xiph.post_Y(array2, array3, 0);
				array6[1] = Xiph.post_Y(array2, array3, 1);
				for (long num4 = 2L; num4 < num2; num4 += 1L)
				{
					checked
					{
						int num21 = look.loneighbor[(int)((IntPtr)(unchecked(num4 - 2L)))];
						int num22 = look.hineighbor[(int)((IntPtr)(unchecked(num4 - 2L)))];
						int x3 = vi.postlist[num21];
						int x4 = vi.postlist[num22];
						int y = array6[num21];
						int y2 = array6[num22];
						int num23 = Xiph.render_point(x3, x4, y, y2, vi.postlist[(int)((IntPtr)num4)]);
						int num24 = Xiph.post_Y(array2, array3, unchecked((int)num4));
						if (num24 >= 0 && num23 != num24)
						{
							array6[(int)((IntPtr)num4)] = num24;
						}
						else
						{
							array6[(int)((IntPtr)num4)] = (num23 | 32768);
						}
					}
				}
			}
			return array6;
		}

		private static int[] floor1_interpolate_fit(Xiph.vorbis_block vb, Xiph.vorbis_look_floor1 look, int[] A, int[] B, int del)
		{
			long num = (long)look.posts;
			int[] array = null;
			if (A != null && B != null)
			{
				array = new int[num];
				for (long num2 = 0L; num2 < num; num2 += 1L)
				{
					array[(int)(checked((IntPtr)num2))] = (65536 - del) * (A[(int)(checked((IntPtr)num2))] & 32767) + del * (B[(int)(checked((IntPtr)num2))] & 32767) + 32768 >> 16;
					checked
					{
						if ((A[(int)((IntPtr)num2)] & 32768) != 0 && (B[(int)((IntPtr)num2)] & 32768) != 0)
						{
							array[(int)((IntPtr)num2)] |= 32768;
						}
					}
				}
			}
			return array;
		}

		private static int floor1_encode(Xiph.oggpack_buffer opb, Xiph.vorbis_block vb, Xiph.vorbis_look_floor1 look, int[] post, int[] ilogmask)
		{
			Xiph.vorbis_info_floor1 vi = look.vi;
			long num = (long)look.posts;
			Xiph.codec_setup_info codec_setup = vb.vd.vi.codec_setup;
			int[] array = new int[65];
			Xiph.static_codebook[] book_param = codec_setup.book_param;
			Xiph.codebook[] fullbooks = codec_setup.fullbooks;
			if (post != null)
			{
				long num2;
				for (num2 = 0L; num2 < num; num2 += 1L)
				{
					checked
					{
						int num3 = post[(int)((IntPtr)num2)] & 32767;
						switch (vi.mult)
						{
						case 1:
							num3 >>= 2;
							break;
						case 2:
							num3 >>= 3;
							break;
						case 3:
							num3 /= 12;
							break;
						case 4:
							num3 >>= 4;
							break;
						}
						post[(int)((IntPtr)num2)] = (num3 | (post[(int)((IntPtr)num2)] & 32768));
					}
				}
				array[0] = post[0];
				array[1] = post[1];
				for (num2 = 2L; num2 < num; num2 += 1L)
				{
					checked
					{
						int num4 = look.loneighbor[(int)((IntPtr)(unchecked(num2 - 2L)))];
						int num5 = look.hineighbor[(int)((IntPtr)(unchecked(num2 - 2L)))];
						int x = vi.postlist[num4];
						int x2 = vi.postlist[num5];
						int y = post[num4];
						int y2 = post[num5];
						int num6 = Xiph.render_point(x, x2, y, y2, vi.postlist[(int)((IntPtr)num2)]);
						if ((post[(int)((IntPtr)num2)] & 32768) != 0 || num6 == post[(int)((IntPtr)num2)])
						{
							post[(int)((IntPtr)num2)] = (num6 | 32768);
							array[(int)((IntPtr)num2)] = 0;
						}
						else
						{
							int num8;
							unchecked
							{
								int num7 = (look.quant_q - num6 < num6) ? (look.quant_q - num6) : num6;
								num8 = post[(int)(checked((IntPtr)num2))] - num6;
								if (num8 < 0)
								{
									if (num8 < -num7)
									{
										num8 = num7 - num8 - 1;
									}
									else
									{
										num8 = -1 - (num8 << 1);
									}
								}
								else if (num8 >= num7)
								{
									num8 += num7;
								}
								else
								{
									num8 <<= 1;
								}
							}
							array[(int)((IntPtr)num2)] = num8;
							post[num4] &= 32767;
							post[num5] &= 32767;
						}
					}
				}
				Xiph.oggpack_write(opb, 1L, 1);
				look.frames += 1L;
				look.postbits += (long)(Xiph.ilog((uint)(look.quant_q - 1)) * 2);
				Xiph.oggpack_write(opb, (long)array[0], Xiph.ilog((uint)(look.quant_q - 1)));
				Xiph.oggpack_write(opb, (long)array[1], Xiph.ilog((uint)(look.quant_q - 1)));
				num2 = 0L;
				long num9 = 2L;
				while (num2 < (long)vi.partitions)
				{
					int num10 = vi.partitionclass[(int)(checked((IntPtr)num2))];
					int num11 = vi.class_dim[num10];
					int num12 = vi.class_subs[num10];
					int num13 = 1 << num12;
					int[] array2 = new int[8];
					int[] array3 = array2;
					int num14 = 0;
					int num15 = 0;
					if (num12 != 0)
					{
						int[] array4 = new int[8];
						for (int i = 0; i < num13; i++)
						{
							int num16 = vi.class_subbook[num10][i];
							if (num16 < 0)
							{
								array4[i] = 1;
							}
							else
							{
								array4[i] = (int)book_param[vi.class_subbook[num10][i]].entries;
							}
						}
						for (int i = 0; i < num11; i++)
						{
							for (int j = 0; j < num13; j++)
							{
								int num17 = array[(int)(checked((IntPtr)(unchecked(num9 + (long)i))))];
								if (num17 < array4[j])
								{
									array3[i] = j;
									break;
								}
							}
							num14 |= array3[i] << num15;
							num15 += num12;
						}
						look.phrasebits += (long)Xiph.vorbis_book_encode(fullbooks[vi.class_book[num10]], num14, opb);
					}
					for (int i = 0; i < num11; i++)
					{
						int num18 = vi.class_subbook[num10][array3[i]];
						if (num18 >= 0 && (long)array[(int)(checked((IntPtr)(unchecked(num9 + (long)i))))] < fullbooks[num18].entries)
						{
							look.postbits += (long)Xiph.vorbis_book_encode(fullbooks[num18], array[(int)(checked((IntPtr)(unchecked(num9 + (long)i))))], opb);
						}
					}
					num9 += (long)num11;
					num2 += 1L;
				}
				int num19 = 0;
				int x3 = 0;
				int num20 = post[0] * vi.mult;
				int n = (int)(codec_setup.blocksizes[(int)(checked((IntPtr)vb.W))] / 2L);
				for (num9 = 1L; num9 < (long)look.posts; num9 += 1L)
				{
					int num21 = look.forward_index[(int)(checked((IntPtr)num9))];
					int num22 = post[num21] & 32767;
					if (num22 == post[num21])
					{
						num22 *= vi.mult;
						num19 = vi.postlist[num21];
						Xiph.render_line0(n, x3, num19, num20, num22, ilogmask, 0);
						x3 = num19;
						num20 = num22;
					}
				}
				for (num9 = (long)num19; num9 < (long)(vb.pcmend / 2); num9 += 1L)
				{
					ilogmask[(int)(checked((IntPtr)num9))] = num20;
				}
				return 1;
			}
			Xiph.oggpack_write(opb, 0L, 1);
			for (long num2 = 0L; num2 < (long)(vb.pcmend / 2); num2 += 1L)
			{
				ilogmask[(int)(checked((IntPtr)num2))] = 0;
			}
			return 0;
		}

		private static object floor1_inverse1(Xiph.vorbis_block vb, Xiph.vorbis_look_floor _in)
		{
			Xiph.vorbis_look_floor1 vorbis_look_floor = (Xiph.vorbis_look_floor1)_in;
			Xiph.vorbis_info_floor1 vi = vorbis_look_floor.vi;
			Xiph.codec_setup_info codec_setup = vb.vd.vi.codec_setup;
			Xiph.codebook[] fullbooks = codec_setup.fullbooks;
			if (Xiph.oggpack_read(vb.opb, 1) == 1L)
			{
				int[] array = new int[vorbis_look_floor.posts];
				array[0] = (int)Xiph.oggpack_read(vb.opb, Xiph.ilog((uint)(vorbis_look_floor.quant_q - 1)));
				array[1] = (int)Xiph.oggpack_read(vb.opb, Xiph.ilog((uint)(vorbis_look_floor.quant_q - 1)));
				int i = 0;
				int num = 2;
				while (i < vi.partitions)
				{
					int num2 = vi.partitionclass[i];
					int num3 = vi.class_dim[num2];
					int num4 = vi.class_subs[num2];
					int num5 = 1 << num4;
					int num6 = 0;
					if (num4 != 0)
					{
						num6 = (int)Xiph.vorbis_book_decode(fullbooks[vi.class_book[num2]], vb.opb);
						if (num6 == -1)
						{
							goto IL_27F;
						}
					}
					for (int j = 0; j < num3; j++)
					{
						int num7 = vi.class_subbook[num2][num6 & num5 - 1];
						num6 >>= num4;
						if (num7 >= 0)
						{
							if ((array[num + j] = (int)Xiph.vorbis_book_decode(fullbooks[num7], vb.opb)) == -1)
							{
								goto IL_27F;
							}
						}
						else
						{
							array[num + j] = 0;
						}
					}
					num += num3;
					i++;
				}
				for (i = 2; i < vorbis_look_floor.posts; i++)
				{
					int num8 = Xiph.render_point(vi.postlist[vorbis_look_floor.loneighbor[i - 2]], vi.postlist[vorbis_look_floor.hineighbor[i - 2]], array[vorbis_look_floor.loneighbor[i - 2]], array[vorbis_look_floor.hineighbor[i - 2]], vi.postlist[i]);
					int num9 = vorbis_look_floor.quant_q - num8;
					int num10 = num8;
					int num11 = ((num9 < num10) ? num9 : num10) << 1;
					int num12 = array[i];
					if (num12 != 0)
					{
						if (num12 >= num11)
						{
							if (num9 > num10)
							{
								num12 -= num10;
							}
							else
							{
								num12 = -1 - (num12 - num9);
							}
						}
						else if ((num12 & 1) != 0)
						{
							num12 = -(num12 + 1 >> 1);
						}
						else
						{
							num12 >>= 1;
						}
						array[i] = (num12 + num8 & 32767);
						array[vorbis_look_floor.loneighbor[i - 2]] &= 32767;
						array[vorbis_look_floor.hineighbor[i - 2]] &= 32767;
					}
					else
					{
						array[i] = (num8 | 32768);
					}
				}
				return array;
			}
			IL_27F:
			return null;
		}

		private static int floor1_inverse2(Xiph.vorbis_block vb, Xiph.vorbis_look_floor _in, object memo, float[] _out)
		{
			Xiph.vorbis_look_floor1 vorbis_look_floor = (Xiph.vorbis_look_floor1)_in;
			Xiph.vorbis_info_floor1 vi = vorbis_look_floor.vi;
			Xiph.codec_setup_info codec_setup = vb.vd.vi.codec_setup;
			int num = (int)codec_setup.blocksizes[(int)(checked((IntPtr)vb.W))] / 2;
			if (memo != null)
			{
				int[] array = (int[])memo;
				int num2 = 0;
				int x = 0;
				int num3 = array[0] * vi.mult;
				num3 = ((num3 < 0) ? 0 : ((num3 > 255) ? 255 : num3));
				for (int i = 1; i < vorbis_look_floor.posts; i++)
				{
					int num4 = vorbis_look_floor.forward_index[i];
					int num5 = array[num4] & 32767;
					if (num5 == array[num4])
					{
						num2 = vi.postlist[num4];
						num5 *= vi.mult;
						num5 = ((num5 < 0) ? 0 : ((num5 > 255) ? 255 : num5));
						Xiph.render_line(num, x, num2, num3, num5, _out, 0);
						x = num2;
						num3 = num5;
					}
				}
				for (int i = num2; i < num; i++)
				{
					_out[i] *= Xiph.FLOOR1_fromdB_LOOKUP[num3];
				}
				return 1;
			}
			for (int i = 0; i < num; i++)
			{
				_out[i] = 0f;
			}
			return 0;
		}

		private static void _v_readstring(Xiph.oggpack_buffer o, char[] buf, int bytes)
		{
			for (int i = 0; i < bytes; i++)
			{
				buf[i] = (char)Xiph.oggpack_read(o, 8);
			}
		}

		private static string _v_readstring2(Xiph.oggpack_buffer o, int bytes)
		{
			byte[] array = new byte[bytes];
			for (int i = 0; i < bytes; i++)
			{
				array[i] = (byte)Xiph.oggpack_read(o, 8);
			}
			return System.Text.Encoding.UTF8.GetString(array);
		}

		public static void vorbis_comment_init(Xiph.vorbis_comment vc)
		{
			vc.user_comments = null;
			vc.vendor = null;
		}

		public static void vorbis_comment_clear(Xiph.vorbis_comment vc)
		{
			if (vc != null)
			{
				vc.user_comments = null;
				vc.vendor = null;
			}
		}

		public static void vorbis_info_init(Xiph.vorbis_info vi)
		{
			vi.codec_setup = new Xiph.codec_setup_info();
		}

		public static void vorbis_info_clear(Xiph.vorbis_info vi)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			vi.codec_setup = null;
			vi.bitrate_lower = 0L;
			vi.bitrate_nominal = 0L;
			vi.bitrate_upper = 0L;
			vi.bitrate_window = 0L;
			vi.channels = 0;
			vi.rate = 0L;
			vi.version = 0;
		}

		private static int _vorbis_unpack_info(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			if (codec_setup == null)
			{
				return -129;
			}
			vi.version = (int)Xiph.oggpack_read(opb, 32);
			if (vi.version != 0)
			{
				return -134;
			}
			vi.channels = (int)Xiph.oggpack_read(opb, 8);
			vi.rate = Xiph.oggpack_read(opb, 32);
			vi.bitrate_upper = Xiph.oggpack_read(opb, 32);
			vi.bitrate_nominal = Xiph.oggpack_read(opb, 32);
			vi.bitrate_lower = Xiph.oggpack_read(opb, 32);
			codec_setup.blocksizes[0] = 1L << (int)((byte)Xiph.oggpack_read(opb, 4) & 31);
			codec_setup.blocksizes[1] = 1L << (int)((byte)Xiph.oggpack_read(opb, 4) & 31);
			if (vi.rate >= 1L && vi.channels >= 1 && codec_setup.blocksizes[0] >= 64L && codec_setup.blocksizes[1] >= codec_setup.blocksizes[0] && codec_setup.blocksizes[1] <= 8192L && Xiph.oggpack_read(opb, 1) == 1L)
			{
				return 0;
			}
			Xiph.vorbis_info_clear(vi);
			return -133;
		}

		private static int _vorbis_unpack_comment(Xiph.vorbis_comment vc, Xiph.oggpack_buffer opb)
		{
			int num = (int)Xiph.oggpack_read(opb, 32);
			if (num >= 0 && (long)num <= opb.storage - 8L)
			{
				vc.vendor = Xiph._v_readstring2(opb, num);
				int i = (int)Xiph.oggpack_read(opb, 32);
				if (i >= 0 && (long)i <= opb.storage - Xiph.oggpack_bytes(opb) >> 2)
				{
					vc.user_comments = new string[i];
					for (i = 0; i < vc.user_comments.Length; i++)
					{
						int num2 = (int)Xiph.oggpack_read(opb, 32);
						if (num2 < 0 || (long)num2 > opb.storage - Xiph.oggpack_bytes(opb))
						{
							goto IL_A9;
						}
						vc.user_comments[i] = Xiph._v_readstring2(opb, num2);
					}
					if (Xiph.oggpack_read(opb, 1) == 1L)
					{
						return 0;
					}
				}
			}
			IL_A9:
			Xiph.vorbis_comment_clear(vc);
			return -133;
		}

		private static int _vorbis_unpack_books(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			if (codec_setup == null)
			{
				return -129;
			}
			codec_setup.books = (int)Xiph.oggpack_read(opb, 8) + 1;
			if (codec_setup.books > 0)
			{
				for (int i = 0; i < codec_setup.books; i++)
				{
					codec_setup.book_param[i] = Xiph.vorbis_staticbook_unpack(opb);
					if (codec_setup.book_param[i] == null)
					{
						goto IL_311;
					}
				}
				int num = (int)Xiph.oggpack_read(opb, 6) + 1;
				if (num > 0)
				{
					for (int i = 0; i < num; i++)
					{
						int num2 = (int)Xiph.oggpack_read(opb, 16);
						if (num2 < 0 || num2 >= 1)
						{
							goto IL_311;
						}
					}
					codec_setup.floors = (int)Xiph.oggpack_read(opb, 6) + 1;
					if (codec_setup.floors > 0)
					{
						for (int i = 0; i < codec_setup.floors; i++)
						{
							codec_setup.floor_type[i] = (int)Xiph.oggpack_read(opb, 16);
							if (codec_setup.floor_type[i] < 0 || codec_setup.floor_type[i] >= 2)
							{
								goto IL_311;
							}
							codec_setup.floor_param[i] = Xiph._floor_P[codec_setup.floor_type[i]].unpack(vi, opb);
							if (codec_setup.floor_param[i] == null)
							{
								goto IL_311;
							}
						}
						codec_setup.residues = (int)Xiph.oggpack_read(opb, 6) + 1;
						if (codec_setup.residues > 0)
						{
							for (int i = 0; i < codec_setup.residues; i++)
							{
								codec_setup.residue_type[i] = (int)Xiph.oggpack_read(opb, 16);
								if (codec_setup.residue_type[i] < 0 || codec_setup.residue_type[i] >= 3)
								{
									goto IL_311;
								}
								codec_setup.residue_param[i] = Xiph._residue_P[codec_setup.residue_type[i]].unpack(vi, opb);
								if (codec_setup.residue_param[i] == null)
								{
									goto IL_311;
								}
							}
							codec_setup.maps = (int)Xiph.oggpack_read(opb, 6) + 1;
							if (codec_setup.maps > 0)
							{
								for (int i = 0; i < codec_setup.maps; i++)
								{
									codec_setup.map_type[i] = (int)Xiph.oggpack_read(opb, 16);
									if (codec_setup.map_type[i] < 0 || codec_setup.map_type[i] >= 1)
									{
										goto IL_311;
									}
									codec_setup.map_param[i] = Xiph._mapping_P[codec_setup.map_type[i]].unpack(vi, opb);
									if (codec_setup.map_param[i] == null)
									{
										goto IL_311;
									}
								}
								codec_setup.modes = (int)Xiph.oggpack_read(opb, 6) + 1;
								if (codec_setup.modes > 0)
								{
									for (int i = 0; i < codec_setup.modes; i++)
									{
										codec_setup.mode_param[i] = new Xiph.vorbis_info_mode();
										codec_setup.mode_param[i].blockflag = (int)Xiph.oggpack_read(opb, 1);
										codec_setup.mode_param[i].windowtype = (int)Xiph.oggpack_read(opb, 16);
										codec_setup.mode_param[i].transformtype = (int)Xiph.oggpack_read(opb, 16);
										codec_setup.mode_param[i].mapping = (int)Xiph.oggpack_read(opb, 8);
										if (codec_setup.mode_param[i].windowtype >= 1 || codec_setup.mode_param[i].transformtype >= 1 || codec_setup.mode_param[i].mapping >= codec_setup.maps || codec_setup.mode_param[i].mapping < 0)
										{
											goto IL_311;
										}
									}
									if (Xiph.oggpack_read(opb, 1) == 1L)
									{
										return 0;
									}
								}
							}
						}
					}
				}
			}
			IL_311:
			Xiph.vorbis_info_clear(vi);
			return -133;
		}

		public static int vorbis_synthesis_headerin(Xiph.vorbis_info vi, Xiph.vorbis_comment vc, Xiph.ogg_packet op)
		{
			Xiph.oggpack_buffer oggpack_buffer = new Xiph.oggpack_buffer();
			if (op == null)
			{
				return -133;
			}
			Xiph.oggpack_readinit(oggpack_buffer, op.packet, op.bytes);
			char[] array = new char[6];
			int num = (int)Xiph.oggpack_read(oggpack_buffer, 8);
			Xiph._v_readstring(oggpack_buffer, array, 6);
			if (array[0] != 'v' || array[1] != 'o' || array[2] != 'r' || array[3] != 'b' || array[4] != 'i' || array[5] != 's')
			{
				return -132;
			}
			switch (num)
			{
			case 1:
				if (op.b_o_s == 0)
				{
					return -133;
				}
				if (vi.rate != 0L)
				{
					return -133;
				}
				return Xiph._vorbis_unpack_info(vi, oggpack_buffer);
			case 3:
				if (vi.rate == 0L)
				{
					return -133;
				}
				return Xiph._vorbis_unpack_comment(vc, oggpack_buffer);
			case 5:
				if (vi.rate == 0L || vc.vendor == null)
				{
					return -133;
				}
				return Xiph._vorbis_unpack_books(vi, oggpack_buffer);
			}
			return -133;
		}

		private static void vorbis_lsp_to_curve(float[] curve, int[] map, int n, int ln, float[] lsp, int m, float amp, float ampoffset)
		{
			float num = (float)(3.1415926535897931 / (double)ln);
			int i;
			for (i = 0; i < m; i++)
			{
				lsp[i] = 2f * Xiph.cos(lsp[i]);
			}
			i = 0;
			while (i < n)
			{
				int num2 = map[i];
				float num3 = 0.5f;
				float num4 = 0.5f;
				float num5 = 2f * Xiph.cos(num * (float)num2);
				int j;
				for (j = 1; j < m; j += 2)
				{
					num4 *= num5 - lsp[j - 1];
					num3 *= num5 - lsp[j];
				}
				if (j == m)
				{
					num4 *= num5 - lsp[j - 1];
					num3 *= num3 * (4f - num5 * num5);
					num4 *= num4;
				}
				else
				{
					num3 *= num3 * (2f - num5);
					num4 *= num4 * (2f + num5);
				}
				num4 = Xiph.fromdB(amp / Xiph.sqrt(num3 + num4) - ampoffset);
				curve[i] *= num4;
				while (map[++i] == num2)
				{
					curve[i] *= num4;
				}
			}
		}

		private static void mapping0_free_info(Xiph.vorbis_info_mapping i)
		{
		}

		private static void mapping0_pack(Xiph.vorbis_info vi, Xiph.vorbis_info_mapping vm, Xiph.oggpack_buffer opb)
		{
			Xiph.vorbis_info_mapping0 vorbis_info_mapping = (Xiph.vorbis_info_mapping0)vm;
			if (vorbis_info_mapping.submaps > 1)
			{
				Xiph.oggpack_write(opb, 1L, 1);
				Xiph.oggpack_write(opb, (long)(vorbis_info_mapping.submaps - 1), 4);
			}
			else
			{
				Xiph.oggpack_write(opb, 0L, 1);
			}
			if (vorbis_info_mapping.coupling_steps > 0)
			{
				Xiph.oggpack_write(opb, 1L, 1);
				Xiph.oggpack_write(opb, (long)(vorbis_info_mapping.coupling_steps - 1), 8);
				for (int i = 0; i < vorbis_info_mapping.coupling_steps; i++)
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_mapping.coupling_mag[i], Xiph.ilog2((uint)vi.channels));
					Xiph.oggpack_write(opb, (long)vorbis_info_mapping.coupling_ang[i], Xiph.ilog2((uint)vi.channels));
				}
			}
			else
			{
				Xiph.oggpack_write(opb, 0L, 1);
			}
			Xiph.oggpack_write(opb, 0L, 2);
			if (vorbis_info_mapping.submaps > 1)
			{
				for (int i = 0; i < vi.channels; i++)
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_mapping.chmuxlist[i], 4);
				}
			}
			for (int i = 0; i < vorbis_info_mapping.submaps; i++)
			{
				Xiph.oggpack_write(opb, 0L, 8);
				Xiph.oggpack_write(opb, (long)vorbis_info_mapping.floorsubmap[i], 8);
				Xiph.oggpack_write(opb, (long)vorbis_info_mapping.residuesubmap[i], 8);
			}
		}

		private static Xiph.vorbis_info_mapping mapping0_unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			Xiph.vorbis_info_mapping0 vorbis_info_mapping = new Xiph.vorbis_info_mapping0();
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			int num = (int)Xiph.oggpack_read(opb, 1);
			if (num >= 0)
			{
				if (num != 0)
				{
					vorbis_info_mapping.submaps = (int)(Xiph.oggpack_read(opb, 4) + 1L);
					if (vorbis_info_mapping.submaps <= 0)
					{
						goto IL_1C9;
					}
				}
				else
				{
					vorbis_info_mapping.submaps = 1;
				}
				num = (int)Xiph.oggpack_read(opb, 1);
				if (num >= 0)
				{
					if (num != 0)
					{
						vorbis_info_mapping.coupling_steps = (int)(Xiph.oggpack_read(opb, 8) + 1L);
						if (vorbis_info_mapping.coupling_steps <= 0)
						{
							goto IL_1C9;
						}
						for (int i = 0; i < vorbis_info_mapping.coupling_steps; i++)
						{
							int num2 = vorbis_info_mapping.coupling_mag[i] = (int)Xiph.oggpack_read(opb, Xiph.ilog2((uint)vi.channels));
							int num3 = vorbis_info_mapping.coupling_ang[i] = (int)Xiph.oggpack_read(opb, Xiph.ilog2((uint)vi.channels));
							if (num2 < 0 || num3 < 0 || num2 == num3 || num2 >= vi.channels || num3 >= vi.channels)
							{
								goto IL_1C9;
							}
						}
					}
					if (Xiph.oggpack_read(opb, 2) == 0L)
					{
						if (vorbis_info_mapping.submaps > 1)
						{
							for (int i = 0; i < vi.channels; i++)
							{
								vorbis_info_mapping.chmuxlist[i] = (int)Xiph.oggpack_read(opb, 4);
								if (vorbis_info_mapping.chmuxlist[i] >= vorbis_info_mapping.submaps || vorbis_info_mapping.chmuxlist[i] < 0)
								{
									goto IL_1C9;
								}
							}
						}
						for (int i = 0; i < vorbis_info_mapping.submaps; i++)
						{
							Xiph.oggpack_read(opb, 8);
							vorbis_info_mapping.floorsubmap[i] = (int)Xiph.oggpack_read(opb, 8);
							if (vorbis_info_mapping.floorsubmap[i] >= codec_setup.floors || vorbis_info_mapping.floorsubmap[i] < 0)
							{
								goto IL_1C9;
							}
							vorbis_info_mapping.residuesubmap[i] = (int)Xiph.oggpack_read(opb, 8);
							if (vorbis_info_mapping.residuesubmap[i] >= codec_setup.residues || vorbis_info_mapping.residuesubmap[i] < 0)
							{
								goto IL_1C9;
							}
						}
						return vorbis_info_mapping;
					}
				}
			}
			IL_1C9:
			Xiph.mapping0_free_info(vorbis_info_mapping);
			return null;
		}

		private static int mapping0_forward(Xiph.vorbis_block vb)
		{
			Xiph.vorbis_dsp_state vd = vb.vd;
			Xiph.vorbis_info vi = vd.vi;
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			Xiph.private_state backend_state = vb.vd.backend_state;
			Xiph.vorbis_block_internal internl = vb.internl;
			int pcmend = vb.pcmend;
			int[] array = new int[vi.channels];
			float[][] array2 = new float[vi.channels][];
			int[][] array3 = new int[vi.channels][];
			int[][][] array4 = new int[vi.channels][][];
			float num = internl.ampmax;
			float[] array5 = new float[vi.channels];
			int blocktype = internl.blocktype;
			int num2 = (int)vb.W;
			Xiph.vorbis_info_mapping0 vorbis_info_mapping = (Xiph.vorbis_info_mapping0)codec_setup.map_param[num2];
			Xiph.vorbis_look_psy p = backend_state.psy[blocktype + ((vb.W != 0L) ? 2 : 0)];
			vb.mode = num2;
			for (int i = 0; i < vi.channels; i++)
			{
				float x = 4f / (float)pcmend;
				float[] array6 = vb.pcm[i];
				float[] array7 = array6;
				array3[i] = new int[pcmend / 2];
				array2[i] = new float[pcmend / 2];
				float num3 = Xiph.todB(x) + 0.345f;
				Xiph._vorbis_apply_window(array6, backend_state.window, codec_setup.blocksizes, (int)vb.lW, (int)vb.W, (int)vb.nW);
				checked
				{
					Xiph.mdct_forward(backend_state.transform[(int)((IntPtr)vb.W)][0], array6, array2[i]);
					Xiph.drft_forward(backend_state.fft_look[(int)((IntPtr)vb.W)], array6, 0);
				}
				array7[0] = num3 + Xiph.todB(array6[0]) + 0.345f;
				array5[i] = array7[0];
				for (int j = 1; j < pcmend - 1; j += 2)
				{
					float num4 = array6[j] * array6[j] + array6[j + 1] * array6[j + 1];
					num4 = (array7[j + 1 >> 1] = num3 + 0.5f * Xiph.todB(num4) + 0.345f);
					if (num4 > array5[i])
					{
						array5[i] = num4;
					}
				}
				if (array5[i] > 0f)
				{
					array5[i] = 0f;
				}
				if (array5[i] > num)
				{
					num = array5[i];
				}
			}
			float[] array8 = new float[pcmend / 2];
			float[] array9 = new float[pcmend / 2];
			for (int i = 0; i < vi.channels; i++)
			{
				int num5 = vorbis_info_mapping.chmuxlist[i];
				float[] array10 = array2[i];
				float[] array11 = vb.pcm[i];
				int num6 = pcmend / 2;
				int logmasko = 0;
				vb.mode = num2;
				array4[i] = new int[15][];
				for (int j = 0; j < pcmend / 2; j++)
				{
					array11[num6 + j] = Xiph.todB(array10[j]) + 0.345f;
				}
				Xiph._vp_noisemask(p, array11, num6, array8, 0);
				Xiph._vp_tonemask(p, array11, 0, array9, 0, num, array5[i]);
				Xiph._vp_offset_and_mix(p, array8, array9, 1, array11, array10, array11, num6);
				if (codec_setup.floor_type[vorbis_info_mapping.floorsubmap[num5]] != 1)
				{
					return -1;
				}
				array4[i][7] = Xiph.floor1_fit(vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num5]], array11, num6, array11, logmasko);
				if (Xiph.vorbis_bitrate_managed(vb) != 0 && array4[i][7] != null)
				{
					Xiph._vp_offset_and_mix(p, array8, array9, 2, array11, array10, array11, num6);
					array4[i][14] = Xiph.floor1_fit(vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num5]], array11, num6, array11, logmasko);
					Xiph._vp_offset_and_mix(p, array8, array9, 0, array11, array10, array11, num6);
					array4[i][0] = Xiph.floor1_fit(vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num5]], array11, num6, array11, logmasko);
					for (int k = 1; k < 7; k++)
					{
						array4[i][k] = Xiph.floor1_interpolate_fit(vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num5]], array4[i][0], array4[i][7], k * 65536 / 7);
					}
					for (int k = 8; k < 14; k++)
					{
						array4[i][k] = Xiph.floor1_interpolate_fit(vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num5]], array4[i][7], array4[i][14], (k - 7) * 65536 / 7);
					}
				}
			}
			internl.ampmax = num;
			int[][] array12 = new int[vi.channels][];
			int[] array13 = new int[vi.channels];
			for (int k = (Xiph.vorbis_bitrate_managed(vb) != 0) ? 0 : 7; k <= ((Xiph.vorbis_bitrate_managed(vb) != 0) ? 14 : 7); k++)
			{
				Xiph.oggpack_buffer oggpack_buffer = internl.packetblob[k];
				Xiph.oggpack_write(oggpack_buffer, 0L, 1);
				Xiph.oggpack_write(oggpack_buffer, (long)num2, backend_state.modebits);
				if (vb.W != 0L)
				{
					Xiph.oggpack_write(oggpack_buffer, vb.lW, 1);
					Xiph.oggpack_write(oggpack_buffer, vb.nW, 1);
				}
				for (int i = 0; i < vi.channels; i++)
				{
					int num7 = vorbis_info_mapping.chmuxlist[i];
					int[] ilogmask = array3[i];
					array[i] = Xiph.floor1_encode(oggpack_buffer, vb, (Xiph.vorbis_look_floor1)backend_state.flr[vorbis_info_mapping.floorsubmap[num7]], array4[i][k], ilogmask);
				}
				Xiph._vp_couple_quantize_normalize(k, codec_setup.psy_g_param, p, vorbis_info_mapping, array2, array3, array, codec_setup.psy_g_param.sliding_lowpass[(int)(checked((IntPtr)vb.W))][k], vi.channels);
				for (int i = 0; i < vorbis_info_mapping.submaps; i++)
				{
					int num8 = 0;
					int num9 = vorbis_info_mapping.residuesubmap[i];
					for (int j = 0; j < vi.channels; j++)
					{
						if (vorbis_info_mapping.chmuxlist[j] == i)
						{
							array13[num8] = 0;
							if (array[j] != 0)
							{
								array13[num8] = 1;
							}
							array12[num8++] = array3[j];
						}
					}
					long[][] l = Xiph._residue_P[codec_setup.residue_type[num9]]._class(vb, backend_state.residue[num9], array12, array13, num8);
					num8 = 0;
					for (int j = 0; j < vi.channels; j++)
					{
						if (vorbis_info_mapping.chmuxlist[j] == i)
						{
							array12[num8++] = array3[j];
						}
					}
					Xiph._residue_P[codec_setup.residue_type[num9]].forward(oggpack_buffer, vb, backend_state.residue[num9], array12, array13, num8, l, i);
				}
			}
			return 0;
		}

		private static int mapping0_inverse(Xiph.vorbis_block vb, Xiph.vorbis_info_mapping l)
		{
			Xiph.vorbis_dsp_state vd = vb.vd;
			Xiph.vorbis_info vi = vd.vi;
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			Xiph.private_state backend_state = vd.backend_state;
			Xiph.vorbis_info_mapping0 vorbis_info_mapping = (Xiph.vorbis_info_mapping0)l;
			long num = (long)(vb.pcmend = (int)codec_setup.blocksizes[(int)(checked((IntPtr)vb.W))]);
			float[][] array = new float[vi.channels][];
			int[] array2 = new int[vi.channels];
			int[] array3 = new int[vi.channels];
			object[] array4 = new object[vi.channels];
			for (int i = 0; i < vi.channels; i++)
			{
				int num2 = vorbis_info_mapping.chmuxlist[i];
				array4[i] = Xiph._floor_P[codec_setup.floor_type[vorbis_info_mapping.floorsubmap[num2]]].inverse1(vb, backend_state.flr[vorbis_info_mapping.floorsubmap[num2]]);
				if (array4[i] != null)
				{
					array3[i] = 1;
				}
				else
				{
					array3[i] = 0;
				}
				int j = 0;
				while ((long)j < num / 2L)
				{
					vb.pcm[i][j] = 0f;
					j++;
				}
			}
			for (int i = 0; i < vorbis_info_mapping.coupling_steps; i++)
			{
				if (array3[vorbis_info_mapping.coupling_mag[i]] != 0 || array3[vorbis_info_mapping.coupling_ang[i]] != 0)
				{
					array3[vorbis_info_mapping.coupling_mag[i]] = 1;
					array3[vorbis_info_mapping.coupling_ang[i]] = 1;
				}
			}
			for (int i = 0; i < vorbis_info_mapping.submaps; i++)
			{
				int num3 = 0;
				for (int j = 0; j < vi.channels; j++)
				{
					if (vorbis_info_mapping.chmuxlist[j] == i)
					{
						if (array3[j] != 0)
						{
							array2[num3] = 1;
						}
						else
						{
							array2[num3] = 0;
						}
						array[num3++] = vb.pcm[j];
					}
				}
				Xiph._residue_P[codec_setup.residue_type[vorbis_info_mapping.residuesubmap[i]]].inverse(vb, backend_state.residue[vorbis_info_mapping.residuesubmap[i]], array, array2, num3);
			}
			for (int i = vorbis_info_mapping.coupling_steps - 1; i >= 0; i--)
			{
				float[] array5 = vb.pcm[vorbis_info_mapping.coupling_mag[i]];
				float[] array6 = vb.pcm[vorbis_info_mapping.coupling_ang[i]];
				int j = 0;
				while ((long)j < num / 2L)
				{
					float num4 = array5[j];
					float num5 = array6[j];
					if (num4 > 0f)
					{
						if (num5 > 0f)
						{
							array5[j] = num4;
							array6[j] = num4 - num5;
						}
						else
						{
							array6[j] = num4;
							array5[j] = num4 + num5;
						}
					}
					else if (num5 > 0f)
					{
						array5[j] = num4;
						array6[j] = num4 + num5;
					}
					else
					{
						array6[j] = num4;
						array5[j] = num4 - num5;
					}
					j++;
				}
			}
			for (int i = 0; i < vi.channels; i++)
			{
				float[] @out = vb.pcm[i];
				int num6 = vorbis_info_mapping.chmuxlist[i];
				Xiph._floor_P[codec_setup.floor_type[vorbis_info_mapping.floorsubmap[num6]]].inverse2(vb, backend_state.flr[vorbis_info_mapping.floorsubmap[num6]], array4[i], @out);
			}
			for (int i = 0; i < vi.channels; i++)
			{
				float[] array7 = vb.pcm[i];
				Xiph.mdct_backward(backend_state.transform[(int)(checked((IntPtr)vb.W))][0], array7, array7);
			}
			return 0;
		}

		private static void mdct_init(Xiph.mdct_lookup lookup, int n)
		{
			int[] array = new int[n / 4];
			float[] array2 = new float[n + n / 4];
			int num = n >> 1;
			int num2 = lookup.log2n = (int)Math.Floor(Math.Log((double)((float)n)) / Math.Log(2.0) + 0.5);
			lookup.n = n;
			lookup.trig = array2;
			lookup.bitrev = array;
			for (int i = 0; i < n / 4; i++)
			{
				array2[i * 2] = (float)Math.Cos(3.1415926535897931 / (double)n * (double)(4 * i));
				array2[i * 2 + 1] = (float)(-(float)Math.Sin(3.1415926535897931 / (double)n * (double)(4 * i)));
				array2[num + i * 2] = (float)Math.Cos(3.1415926535897931 / (double)(2 * n) * (double)(2 * i + 1));
				array2[num + i * 2 + 1] = (float)Math.Sin(3.1415926535897931 / (double)(2 * n) * (double)(2 * i + 1));
			}
			for (int i = 0; i < n / 8; i++)
			{
				array2[n + i * 2] = (float)(Math.Cos(3.1415926535897931 / (double)n * (double)(4 * i + 2)) * 0.5);
				array2[n + i * 2 + 1] = (float)(-(float)Math.Sin(3.1415926535897931 / (double)n * (double)(4 * i + 2)) * 0.5);
			}
			int num3 = (1 << num2 - 1) - 1;
			int num4 = 1 << num2 - 2;
			for (int i = 0; i < n / 8; i++)
			{
				int num5 = 0;
				int num6 = 0;
				while (num4 >> num6 != 0)
				{
					if ((num4 >> num6 & i) != 0)
					{
						num5 |= 1 << num6;
					}
					num6++;
				}
				array[i * 2] = (~num5 & num3) - 1;
				array[i * 2 + 1] = num5;
			}
			lookup.scale = 4f / (float)n;
		}

		private static void mdct_butterfly_8(float[] x, int xo)
		{
			float num = x[xo + 6] + x[xo + 2];
			float num2 = x[xo + 6] - x[xo + 2];
			float num3 = x[xo + 4] + x[xo];
			float num4 = x[xo + 4] - x[xo];
			x[xo + 6] = num + num3;
			x[xo + 4] = num - num3;
			num = x[xo + 5] - x[xo + 1];
			num3 = x[xo + 7] - x[xo + 3];
			x[xo] = num2 + num;
			x[xo + 2] = num2 - num;
			num = x[xo + 5] + x[xo + 1];
			num2 = x[xo + 7] + x[xo + 3];
			x[xo + 3] = num3 + num4;
			x[xo + 1] = num3 - num4;
			x[xo + 7] = num2 + num;
			x[xo + 5] = num2 - num;
		}

		private static void mdct_butterfly_16(float[] x, int xo)
		{
			float num = x[xo + 1] - x[xo + 9];
			float num2 = x[xo] - x[xo + 8];
			x[xo + 8] += x[xo];
			x[xo + 9] += x[xo + 1];
			x[xo] = (num + num2) * 0.707106769f;
			x[xo + 1] = (num - num2) * 0.707106769f;
			num = x[xo + 3] - x[xo + 11];
			num2 = x[xo + 10] - x[xo + 2];
			x[xo + 10] += x[xo + 2];
			x[xo + 11] += x[xo + 3];
			x[xo + 2] = num;
			x[xo + 3] = num2;
			num = x[xo + 12] - x[xo + 4];
			num2 = x[xo + 13] - x[xo + 5];
			x[xo + 12] += x[xo + 4];
			x[xo + 13] += x[xo + 5];
			x[xo + 4] = (num - num2) * 0.707106769f;
			x[xo + 5] = (num + num2) * 0.707106769f;
			num = x[xo + 14] - x[xo + 6];
			num2 = x[xo + 15] - x[xo + 7];
			x[xo + 14] += x[xo + 6];
			x[xo + 15] += x[xo + 7];
			x[xo + 6] = num;
			x[xo + 7] = num2;
			Xiph.mdct_butterfly_8(x, xo);
			Xiph.mdct_butterfly_8(x, xo + 8);
		}

		private static void mdct_butterfly_32(float[] x, int xo)
		{
			float num = x[xo + 30] - x[xo + 14];
			float num2 = x[xo + 31] - x[xo + 15];
			x[xo + 30] += x[xo + 14];
			x[xo + 31] += x[xo + 15];
			x[xo + 14] = num;
			x[xo + 15] = num2;
			num = x[xo + 28] - x[xo + 12];
			num2 = x[xo + 29] - x[xo + 13];
			x[xo + 28] += x[xo + 12];
			x[xo + 29] += x[xo + 13];
			x[xo + 12] = num * 0.9238795f - num2 * 0.382683426f;
			x[xo + 13] = num * 0.382683426f + num2 * 0.9238795f;
			num = x[xo + 26] - x[xo + 10];
			num2 = x[xo + 27] - x[xo + 11];
			x[xo + 26] += x[xo + 10];
			x[xo + 27] += x[xo + 11];
			x[xo + 10] = (num - num2) * 0.707106769f;
			x[xo + 11] = (num + num2) * 0.707106769f;
			num = x[xo + 24] - x[xo + 8];
			num2 = x[xo + 25] - x[xo + 9];
			x[xo + 24] += x[xo + 8];
			x[xo + 25] += x[xo + 9];
			x[xo + 8] = num * 0.382683426f - num2 * 0.9238795f;
			x[xo + 9] = num2 * 0.382683426f + num * 0.9238795f;
			num = x[xo + 22] - x[xo + 6];
			num2 = x[xo + 7] - x[xo + 23];
			x[xo + 22] += x[xo + 6];
			x[xo + 23] += x[xo + 7];
			x[xo + 6] = num2;
			x[xo + 7] = num;
			num = x[xo + 4] - x[xo + 20];
			num2 = x[xo + 5] - x[xo + 21];
			x[xo + 20] += x[xo + 4];
			x[xo + 21] += x[xo + 5];
			x[xo + 4] = num2 * 0.9238795f + num * 0.382683426f;
			x[xo + 5] = num2 * 0.382683426f - num * 0.9238795f;
			num = x[xo + 2] - x[xo + 18];
			num2 = x[xo + 3] - x[xo + 19];
			x[xo + 18] += x[xo + 2];
			x[xo + 19] += x[xo + 3];
			x[xo + 2] = (num2 + num) * 0.707106769f;
			x[xo + 3] = (num2 - num) * 0.707106769f;
			num = x[xo] - x[xo + 16];
			num2 = x[xo + 1] - x[xo + 17];
			x[xo + 16] += x[xo];
			x[xo + 17] += x[xo + 1];
			x[xo] = num2 * 0.382683426f + num * 0.9238795f;
			x[xo + 1] = num2 * 0.9238795f - num * 0.382683426f;
			Xiph.mdct_butterfly_16(x, xo);
			Xiph.mdct_butterfly_16(x, xo + 16);
		}

		private static void mdct_butterfly_first(float[] T, float[] x, int xo, int points)
		{
			int num = 0;
			int num2 = xo + points - 8;
			int num3 = xo + (points >> 1) - 8;
			do
			{
				float num4 = x[num2 + 6] - x[num3 + 6];
				float num5 = x[num2 + 7] - x[num3 + 7];
				x[num2 + 6] += x[num3 + 6];
				x[num2 + 7] += x[num3 + 7];
				x[num3 + 6] = num5 * T[num + 1] + num4 * T[num];
				x[num3 + 7] = num5 * T[num] - num4 * T[num + 1];
				num4 = x[num2 + 4] - x[num3 + 4];
				num5 = x[num2 + 5] - x[num3 + 5];
				x[num2 + 4] += x[num3 + 4];
				x[num2 + 5] += x[num3 + 5];
				x[num3 + 4] = num5 * T[num + 5] + num4 * T[num + 4];
				x[num3 + 5] = num5 * T[num + 4] - num4 * T[num + 5];
				num4 = x[num2 + 2] - x[num3 + 2];
				num5 = x[num2 + 3] - x[num3 + 3];
				x[num2 + 2] += x[num3 + 2];
				x[num2 + 3] += x[num3 + 3];
				x[num3 + 2] = num5 * T[num + 9] + num4 * T[num + 8];
				x[num3 + 3] = num5 * T[num + 8] - num4 * T[num + 9];
				num4 = x[num2] - x[num3];
				num5 = x[num2 + 1] - x[num3 + 1];
				x[num2] += x[num3];
				x[num2 + 1] += x[num3 + 1];
				x[num3] = num5 * T[num + 13] + num4 * T[num + 12];
				x[num3 + 1] = num5 * T[num + 12] - num4 * T[num + 13];
				num2 -= 8;
				num3 -= 8;
				num += 16;
			}
			while (num3 >= xo);
		}

		private static void mdct_butterfly_generic(float[] T, float[] x, int xo, int points, int trigint)
		{
			int num = 0;
			int num2 = xo + points - 8;
			int num3 = xo + (points >> 1) - 8;
			do
			{
				float num4 = x[num2 + 6] - x[num3 + 6];
				float num5 = x[num2 + 7] - x[num3 + 7];
				x[num2 + 6] += x[num3 + 6];
				x[num2 + 7] += x[num3 + 7];
				x[num3 + 6] = num5 * T[num + 1] + num4 * T[num];
				x[num3 + 7] = num5 * T[num] - num4 * T[num + 1];
				num += trigint;
				num4 = x[num2 + 4] - x[num3 + 4];
				num5 = x[num2 + 5] - x[num3 + 5];
				x[num2 + 4] += x[num3 + 4];
				x[num2 + 5] += x[num3 + 5];
				x[num3 + 4] = num5 * T[num + 1] + num4 * T[num];
				x[num3 + 5] = num5 * T[num] - num4 * T[num + 1];
				num += trigint;
				num4 = x[num2 + 2] - x[num3 + 2];
				num5 = x[num2 + 3] - x[num3 + 3];
				x[num2 + 2] += x[num3 + 2];
				x[num2 + 3] += x[num3 + 3];
				x[num3 + 2] = num5 * T[num + 1] + num4 * T[num];
				x[num3 + 3] = num5 * T[num] - num4 * T[num + 1];
				num += trigint;
				num4 = x[num2] - x[num3];
				num5 = x[num2 + 1] - x[num3 + 1];
				x[num2] += x[num3];
				x[num2 + 1] += x[num3 + 1];
				x[num3] = num5 * T[num + 1] + num4 * T[num];
				x[num3 + 1] = num5 * T[num] - num4 * T[num + 1];
				num += trigint;
				num2 -= 8;
				num3 -= 8;
			}
			while (num3 >= xo);
		}

		private static void mdct_butterflies(Xiph.mdct_lookup init, float[] x, int xo, int points)
		{
			float[] trig = init.trig;
			int num = init.log2n - 5;
			if (--num > 0)
			{
				Xiph.mdct_butterfly_first(trig, x, xo, points);
			}
			int num2 = 1;
			while (--num > 0)
			{
				for (int i = 0; i < 1 << num2; i++)
				{
					Xiph.mdct_butterfly_generic(trig, x, xo + (points >> num2) * i, points >> num2, 4 << num2);
				}
				num2++;
			}
			for (int i = 0; i < points; i += 32)
			{
				Xiph.mdct_butterfly_32(x, xo + i);
			}
		}

		private static void mdct_clear(Xiph.mdct_lookup l)
		{
			if (l != null)
			{
				l.trig = null;
				l.bitrev = null;
				l.log2n = 0;
				l.n = 0;
				l.scale = 0f;
			}
		}

		private static void mdct_bitreverse(Xiph.mdct_lookup init, float[] x)
		{
			int n = init.n;
			int[] bitrev = init.bitrev;
			int num = 0;
			int num2 = 0;
			int num4;
			int num3 = num4 = num2 + (n >> 1);
			int num5 = n;
			do
			{
				int num6 = num3 + bitrev[num];
				int num7 = num3 + bitrev[num + 1];
				float num8 = x[num6 + 1] - x[num7 + 1];
				float num9 = x[num6] + x[num7];
				float num10 = num9 * init.trig[num5] + num8 * init.trig[num5 + 1];
				float num11 = num9 * init.trig[num5 + 1] - num8 * init.trig[num5];
				num4 -= 4;
				num8 = (x[num6 + 1] + x[num7 + 1]) * 0.5f;
				num9 = (x[num6] - x[num7]) * 0.5f;
				x[num2] = num8 + num10;
				x[num4 + 2] = num8 - num10;
				x[num2 + 1] = num9 + num11;
				x[num4 + 3] = num11 - num9;
				num6 = num3 + bitrev[num + 2];
				num7 = num3 + bitrev[num + 3];
				num8 = x[num6 + 1] - x[num7 + 1];
				num9 = x[num6] + x[num7];
				num10 = num9 * init.trig[num5 + 2] + num8 * init.trig[num5 + 3];
				num11 = num9 * init.trig[num5 + 3] - num8 * init.trig[num5 + 2];
				num8 = (x[num6 + 1] + x[num7 + 1]) * 0.5f;
				num9 = (x[num6] - x[num7]) * 0.5f;
				x[num2 + 2] = num8 + num10;
				x[num4] = num8 - num10;
				x[num2 + 3] = num9 + num11;
				x[num4 + 1] = num11 - num9;
				num5 += 4;
				num += 4;
				num2 += 4;
			}
			while (num2 < num4);
		}

		private static void mdct_backward(Xiph.mdct_lookup init, float[] _in, float[] _out)
		{
			int n = init.n;
			int num = n >> 1;
			int num2 = n >> 2;
			int num3 = num - 7;
			int num4 = num + num2;
			int num5 = num2;
			do
			{
				num4 -= 4;
				_out[num4] = -_in[num3 + 2] * init.trig[num5 + 3] - _in[num3] * init.trig[num5 + 2];
				_out[num4 + 1] = _in[num3] * init.trig[num5 + 3] - _in[num3 + 2] * init.trig[num5 + 2];
				_out[num4 + 2] = -_in[num3 + 6] * init.trig[num5 + 1] - _in[num3 + 4] * init.trig[num5];
				_out[num4 + 3] = _in[num3 + 4] * init.trig[num5 + 1] - _in[num3 + 6] * init.trig[num5];
				num3 -= 8;
				num5 += 4;
			}
			while (num3 >= 0);
			num3 = num - 8;
			num4 = num + num2;
			num5 = num2;
			do
			{
				num5 -= 4;
				_out[num4] = _in[num3 + 4] * init.trig[num5 + 3] + _in[num3 + 6] * init.trig[num5 + 2];
				_out[num4 + 1] = _in[num3 + 4] * init.trig[num5 + 2] - _in[num3 + 6] * init.trig[num5 + 3];
				_out[num4 + 2] = _in[num3] * init.trig[num5 + 1] + _in[num3 + 2] * init.trig[num5];
				_out[num4 + 3] = _in[num3] * init.trig[num5] - _in[num3 + 2] * init.trig[num5 + 1];
				num3 -= 8;
				num4 += 4;
			}
			while (num3 >= 0);
			Xiph.mdct_butterflies(init, _out, num, num);
			Xiph.mdct_bitreverse(init, _out);
			int num6 = num + num2;
			int num7 = num + num2;
			num5 = num;
			num3 = 0;
			do
			{
				num6 -= 4;
				_out[num6 + 3] = _out[num3] * init.trig[num5 + 1] - _out[num3 + 1] * init.trig[num5];
				_out[num7] = -(_out[num3] * init.trig[num5] + _out[num3 + 1] * init.trig[num5 + 1]);
				_out[num6 + 2] = _out[num3 + 2] * init.trig[num5 + 3] - _out[num3 + 3] * init.trig[num5 + 2];
				_out[num7 + 1] = -(_out[num3 + 2] * init.trig[num5 + 2] + _out[num3 + 3] * init.trig[num5 + 3]);
				_out[num6 + 1] = _out[num3 + 4] * init.trig[num5 + 5] - _out[num3 + 5] * init.trig[num5 + 4];
				_out[num7 + 2] = -(_out[num3 + 4] * init.trig[num5 + 4] + _out[num3 + 5] * init.trig[num5 + 5]);
				_out[num6] = _out[num3 + 6] * init.trig[num5 + 7] - _out[num3 + 7] * init.trig[num5 + 6];
				_out[num7 + 3] = -(_out[num3 + 6] * init.trig[num5 + 6] + _out[num3 + 7] * init.trig[num5 + 7]);
				num7 += 4;
				num3 += 8;
				num5 += 8;
			}
			while (num3 < num6);
			num3 = num + num2;
			num6 = num2;
			num7 = num6;
			do
			{
				num6 -= 4;
				num3 -= 4;
				_out[num7] = -(_out[num6 + 3] = _out[num3 + 3]);
				_out[num7 + 1] = -(_out[num6 + 2] = _out[num3 + 2]);
				_out[num7 + 2] = -(_out[num6 + 1] = _out[num3 + 1]);
				_out[num7 + 3] = -(_out[num6] = _out[num3]);
				num7 += 4;
			}
			while (num7 < num3);
			num3 = num + num2;
			num6 = num + num2;
			num7 = num;
			do
			{
				num6 -= 4;
				_out[num6] = _out[num3 + 3];
				_out[num6 + 1] = _out[num3 + 2];
				_out[num6 + 2] = _out[num3 + 1];
				_out[num6 + 3] = _out[num3];
				num3 += 4;
			}
			while (num6 > num7);
		}

		private static void mdct_forward(Xiph.mdct_lookup init, float[] _in, float[] _out)
		{
			int n = init.n;
			int num = n >> 1;
			int num2 = n >> 2;
			int num3 = n >> 3;
			float[] array = new float[n];
			int num4 = 0;
			int num5 = num;
			int num6 = num + num2;
			int num7 = num6 + 1;
			int num8 = num;
			int i;
			for (i = 0; i < num3; i += 2)
			{
				num6 -= 4;
				num8 -= 2;
				float num9 = _in[num6 + 2] + _in[num7];
				float num10 = _in[num6] + _in[num7 + 2];
				array[num4 + num5 + i] = num10 * init.trig[num8 + 1] + num9 * init.trig[num8];
				array[num4 + num5 + i + 1] = num10 * init.trig[num8] - num9 * init.trig[num8 + 1];
				num7 += 4;
			}
			num7 = 1;
			while (i < num - num3)
			{
				num8 -= 2;
				num6 -= 4;
				float num9 = _in[num6 + 2] - _in[num7];
				float num10 = _in[num6] - _in[num7 + 2];
				array[num4 + num5 + i] = num10 * init.trig[num8 + 1] + num9 * init.trig[num8];
				array[num4 + num5 + i + 1] = num10 * init.trig[num8] - num9 * init.trig[num8 + 1];
				num7 += 4;
				i += 2;
			}
			num6 = n;
			while (i < num)
			{
				num8 -= 2;
				num6 -= 4;
				float num9 = -_in[num6 + 2] - _in[num7];
				float num10 = -_in[num6] - _in[num7 + 2];
				array[num4 + num5 + i] = num10 * init.trig[num8 + 1] + num9 * init.trig[num8];
				array[num4 + num5 + i + 1] = num10 * init.trig[num8] - num9 * init.trig[num8 + 1];
				num7 += 4;
				i += 2;
			}
			Xiph.mdct_butterflies(init, array, num, num);
			Xiph.mdct_bitreverse(init, array);
			num8 = num;
			num6 = num;
			for (i = 0; i < num2; i++)
			{
				num6--;
				_out[i] = (array[num4] * init.trig[num8] + array[num4 + 1] * init.trig[num8 + 1]) * init.scale;
				_out[num6] = (array[num4] * init.trig[num8 + 1] - array[num4 + 1] * init.trig[num8]) * init.scale;
				num4 += 2;
				num8 += 2;
			}
		}

		private static void min_curve(float[] c, float[] c2)
		{
			for (int i = 0; i < 56; i++)
			{
				if (c2[i] < c[i])
				{
					c[i] = c2[i];
				}
			}
		}

		private static void max_curve(float[] c, float[] c2)
		{
			for (int i = 0; i < 56; i++)
			{
				if (c2[i] > c[i])
				{
					c[i] = c2[i];
				}
			}
		}

		private static void attenuate_curve(float[] c, float att)
		{
			for (int i = 0; i < 56; i++)
			{
				c[i] += att;
			}
		}

		private static float[][][] setup_tone_curves(float[] curveatt_dB, float binHz, int n, float center_boost, float center_decay_rate)
		{
			float[] array = new float[56];
			float[][][] array2 = new float[17][][];
			for (int i = 0; i < 17; i++)
			{
				array2[i] = new float[8][];
				for (int j = 0; j < 8; j++)
				{
					array2[i][j] = new float[56];
				}
			}
			float[][] array3 = new float[8][];
			for (int i = 0; i < 8; i++)
			{
				array3[i] = new float[56];
			}
			float[] array4 = new float[n];
			float[][][] array5 = new float[17][][];
			for (int i = 0; i < 17; i++)
			{
				int num = i * 4;
				for (int j = 0; j < 56; j++)
				{
					float num2 = 999f;
					for (int k = 0; k < 4; k++)
					{
						if (j + k + num < 88)
						{
							if (num2 > Xiph.ATH[j + k + num])
							{
								num2 = Xiph.ATH[j + k + num];
							}
						}
						else if (num2 > Xiph.ATH[87])
						{
							num2 = Xiph.ATH[87];
						}
					}
					array[j] = num2;
				}
				for (int j = 0; j < 6; j++)
				{
					for (int l = 0; l < 56; l++)
					{
						array2[i][j + 2][l] = Xiph.tonemasks[i][j][l];
					}
				}
				for (int l = 0; l < 56; l++)
				{
					array2[i][0][l] = Xiph.tonemasks[i][0][l];
				}
				for (int l = 0; l < 56; l++)
				{
					array2[i][1][l] = Xiph.tonemasks[i][1][l];
				}
				for (int j = 0; j < 8; j++)
				{
					for (int k = 0; k < 56; k++)
					{
						float num3 = center_boost + (float)Xiph.abs(16 - k) * center_decay_rate;
						if (num3 < 0f && center_boost > 0f)
						{
							num3 = 0f;
						}
						if (num3 > 0f && center_boost < 0f)
						{
							num3 = 0f;
						}
						array2[i][j][k] += num3;
					}
				}
				for (int j = 0; j < 8; j++)
				{
					Xiph.attenuate_curve(array2[i][j], curveatt_dB[i] + 100f - (float)((j < 2) ? 2 : j) * 10f - 30f);
					for (int l = 0; l < 56; l++)
					{
						array3[j][l] = array[l];
					}
					Xiph.attenuate_curve(array3[j], 100f - (float)j * 10f - 30f);
					Xiph.max_curve(array3[j], array2[i][j]);
				}
				for (int j = 1; j < 8; j++)
				{
					Xiph.min_curve(array3[j], array3[j - 1]);
					Xiph.min_curve(array2[i][j], array3[j]);
				}
			}
			for (int i = 0; i < 17; i++)
			{
				array5[i] = new float[8][];
				int num4 = (int)Xiph.floor(Xiph.fromOC((float)i * 0.5f) / binHz);
				int num5 = (int)Xiph.ceil(Xiph.toOC((float)num4 * binHz + 1f) * 2f);
				int num6 = (int)Xiph.floor(Xiph.toOC((float)(num4 + 1) * binHz) * 2f);
				if (num5 > i)
				{
					num5 = i;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 >= 17)
				{
					num6 = 16;
				}
				for (int l = 0; l < 8; l++)
				{
					array5[i][l] = new float[58];
					int j;
					for (j = 0; j < n; j++)
					{
						array4[j] = 999f;
					}
					for (int k = num5; k <= num6; k++)
					{
						int m = 0;
						for (j = 0; j < 56; j++)
						{
							int num7 = (int)(Xiph.fromOC((float)j * 0.125f + (float)k * 0.5f - 2.0625f) / binHz);
							int num8 = (int)(Xiph.fromOC((float)j * 0.125f + (float)k * 0.5f - 1.9375f) / binHz + 1f);
							if (num7 < 0)
							{
								num7 = 0;
							}
							if (num7 > n)
							{
								num7 = n;
							}
							if (num7 < m)
							{
								m = num7;
							}
							if (num8 < 0)
							{
								num8 = 0;
							}
							if (num8 > n)
							{
								num8 = n;
							}
							while (m < num8 && m < n)
							{
								if (array4[m] > array2[k][l][j])
								{
									array4[m] = array2[k][l][j];
								}
								m++;
							}
						}
						while (m < n)
						{
							if (array4[m] > array2[k][l][55])
							{
								array4[m] = array2[k][l][55];
							}
							m++;
						}
					}
					if (i + 1 < 17)
					{
						int num9 = 0;
						int k = i + 1;
						for (j = 0; j < 56; j++)
						{
							int num10 = (int)(Xiph.fromOC((float)j * 0.125f + (float)i * 0.5f - 2.0625f) / binHz);
							int num11 = (int)(Xiph.fromOC((float)j * 0.125f + (float)i * 0.5f - 1.9375f) / binHz + 1f);
							if (num10 < 0)
							{
								num10 = 0;
							}
							if (num10 > n)
							{
								num10 = n;
							}
							if (num10 < num9)
							{
								num9 = num10;
							}
							if (num11 < 0)
							{
								num11 = 0;
							}
							if (num11 > n)
							{
								num11 = n;
							}
							while (num9 < num11 && num9 < n)
							{
								if (array4[num9] > array2[k][l][j])
								{
									array4[num9] = array2[k][l][j];
								}
								num9++;
							}
						}
						while (num9 < n)
						{
							if (array4[num9] > array2[k][l][55])
							{
								array4[num9] = array2[k][l][55];
							}
							num9++;
						}
					}
					for (j = 0; j < 56; j++)
					{
						int num12 = (int)(Xiph.fromOC((float)j * 0.125f + (float)i * 0.5f - 2f) / binHz);
						if (num12 < 0)
						{
							array5[i][l][j + 2] = -999f;
						}
						else if (num12 >= n)
						{
							array5[i][l][j + 2] = -999f;
						}
						else
						{
							array5[i][l][j + 2] = array4[num12];
						}
					}
					j = 0;
					while (j < 16 && array5[i][l][j + 2] <= -200f)
					{
						j++;
					}
					array5[i][l][0] = (float)j;
					j = 55;
					while (j > 17 && array5[i][l][j + 2] <= -200f)
					{
						j--;
					}
					array5[i][l][1] = (float)j;
				}
			}
			return array5;
		}

		private static void _vp_psy_init(Xiph.vorbis_look_psy p, Xiph.vorbis_info_psy vi, Xiph.vorbis_info_psy_global gi, int n, long rate)
		{
			long num = -99L;
			long num2 = 1L;
			p.eighth_octave_lines = gi.eighth_octave_lines;
			p.shiftoc = (long)(Xiph.rint(Math.Log((double)((float)gi.eighth_octave_lines * 8f)) / Math.Log(2.0)) - 1);
			p.firstoc = (long)(Xiph.toOC(0.25f * (float)rate * 0.5f / (float)n) * (float)(1 << (int)(p.shiftoc + 1L)) - (float)gi.eighth_octave_lines);
			long num3 = (long)(Xiph.toOC(((float)n + 0.25f) * (float)rate * 0.5f / (float)n) * (float)(1 << (int)(p.shiftoc + 1L)) + 0.5f);
			p.total_octave_lines = (int)(num3 - p.firstoc + 1L);
			p.ath = new float[n];
			p.octave = new long[n];
			p.bark = new long[n];
			p.vi = vi;
			p.n = n;
			p.rate = rate;
			p.m_val = 1f;
			if (rate < 26000L)
			{
				p.m_val = 0f;
			}
			else if (rate < 38000L)
			{
				p.m_val = 0.94f;
			}
			else if (rate > 46000L)
			{
				p.m_val = 1.275f;
			}
			long num4 = 0L;
			long num5 = 0L;
			while (num4 < 87L)
			{
				int num6 = Xiph.rint(Xiph.fromOC((float)(num4 + 1L) * 0.125f - 2f) * 2f * (float)n / (float)rate);
				float num7 = Xiph.ATH[(int)(checked((IntPtr)num4))];
				if (num5 < (long)num6)
				{
					float num8 = (Xiph.ATH[(int)(checked((IntPtr)(unchecked(num4 + 1L))))] - num7) / (float)((long)num6 - num5);
					while (num5 < (long)num6 && num5 < (long)n)
					{
						p.ath[(int)(checked((IntPtr)num5))] = num7 + 100f;
						num7 += num8;
						num5 += 1L;
					}
				}
				num4 += 1L;
			}
			while (num5 < (long)n)
			{
				checked
				{
					p.ath[(int)((IntPtr)num5)] = p.ath[(int)((IntPtr)(unchecked(num5 - 1L)))];
				}
				num5 += 1L;
			}
			for (num4 = 0L; num4 < (long)n; num4 += 1L)
			{
				float num9 = Xiph.toBARK((float)(rate / (long)(2 * n) * num4));
				while (num + (long)vi.noisewindowlomin < num4)
				{
					if (Xiph.toBARK((float)(rate / (long)(2 * n) * num)) >= num9 - vi.noisewindowlo)
					{
						break;
					}
					num += 1L;
				}
				while (num2 <= (long)n && (num2 < num4 + (long)vi.noisewindowhimin || Xiph.toBARK((float)(rate / (long)(2 * n) * num2)) < num9 + vi.noisewindowhi))
				{
					num2 += 1L;
				}
				p.bark[(int)(checked((IntPtr)num4))] = (num - 1L << 16) + (num2 - 1L);
			}
			for (num4 = 0L; num4 < (long)n; num4 += 1L)
			{
				p.octave[(int)(checked((IntPtr)num4))] = (long)(Xiph.toOC(((float)num4 + 0.25f) * 0.5f * (float)rate / (float)n) * (float)(1 << (int)(p.shiftoc + 1L)) + 0.5f);
			}
			p.tonecurves = Xiph.setup_tone_curves(vi.toneatt, (float)rate * 0.5f / (float)n, n, vi.tone_centerboost, vi.tone_decay);
			p.noiseoffset = new float[3][];
			for (num4 = 0L; num4 < 3L; num4 += 1L)
			{
				p.noiseoffset[(int)(checked((IntPtr)num4))] = new float[n];
			}
			for (num4 = 0L; num4 < (long)n; num4 += 1L)
			{
				float num10 = Xiph.toOC(((float)num4 + 0.5f) * (float)rate / (2f * (float)n)) * 2f;
				if (num10 < 0f)
				{
					num10 = 0f;
				}
				if (num10 >= 16f)
				{
					num10 = 16f;
				}
				int num11 = (int)num10;
				float num12 = num10 - (float)num11;
				for (num5 = 0L; num5 < 3L; num5 += 1L)
				{
					checked(p.noiseoffset[(int)((IntPtr)num5)][(int)((IntPtr)num4)]) = p.vi.noiseoff[(int)(checked((IntPtr)num5))][num11] * (1f - num12) + p.vi.noiseoff[(int)(checked((IntPtr)num5))][num11 + 1] * num12;
				}
			}
		}

		private static void seed_curve(float[] seed, float[][] curves, float amp, int oc, int n, int linesper, float dBoffset)
		{
			int num = (int)((amp + dBoffset - 30f) * 0.1f);
			num = Xiph.max(num, 0);
			num = Xiph.min(num, 7);
			float[] array = curves[num];
			int num2 = 2;
			int num3 = (int)array[1];
			int num4 = oc + (int)(array[0] - 16f) * linesper - (linesper >> 1);
			for (int i = (int)array[0]; i < num3; i++)
			{
				if (num4 > 0)
				{
					float num5 = amp + array[num2 + i];
					if (seed[num4] < num5)
					{
						seed[num4] = num5;
					}
				}
				num4 += linesper;
				if (num4 >= n)
				{
					return;
				}
			}
		}

		private static void seed_loop(Xiph.vorbis_look_psy p, float[][][] curves, float[] f, int fo, float[] flr, int flro, float[] seed, float specmax)
		{
			Xiph.vorbis_info_psy vi = p.vi;
			long num = (long)p.n;
			float dBoffset = vi.max_curve_dB - specmax;
			for (long num2 = 0L; num2 < num; num2 += 1L)
			{
				float num3;
				long num4;
				checked
				{
					num3 = f[(int)((IntPtr)(unchecked((long)fo + num2)))];
					num4 = p.octave[(int)((IntPtr)num2)];
					while (unchecked(num2 + 1L) < num && p.octave[(int)((IntPtr)(unchecked(num2 + 1L)))] == num4)
					{
						unchecked
						{
							num2 += 1L;
						}
						if (f[(int)((IntPtr)(unchecked((long)fo + num2)))] > num3)
						{
							num3 = f[(int)((IntPtr)(unchecked((long)fo + num2)))];
						}
					}
				}
				if (num3 + 6f > flr[(int)(checked((IntPtr)(unchecked((long)flro + num2))))])
				{
					num4 >>= (int)p.shiftoc;
					if (num4 >= 17L)
					{
						num4 = 16L;
					}
					if (num4 < 0L)
					{
						num4 = 0L;
					}
					Xiph.seed_curve(seed, curves[(int)(checked((IntPtr)num4))], num3, (int)(p.octave[(int)(checked((IntPtr)num2))] - p.firstoc), p.total_octave_lines, p.eighth_octave_lines, dBoffset);
				}
			}
		}

		private static void seed_chase(float[] seeds, int linesper, long n)
		{
			long[] array = new long[n];
			float[] array2 = new float[n];
			long num = 0L;
			long num2 = 0L;
			for (long num3 = 0L; num3 < n; num3 += 1L)
			{
				checked
				{
					if (num < 2L)
					{
						array[(int)((IntPtr)num)] = num3;
						float[] arg_37_0 = array2;
						long expr_2C = num;
						num = unchecked(expr_2C + 1L);
						arg_37_0[(int)((IntPtr)expr_2C)] = seeds[(int)((IntPtr)num3)];
					}
					else
					{
						while (seeds[(int)((IntPtr)num3)] >= array2[(int)((IntPtr)(unchecked(num - 1L)))])
						{
							if (num3 >= unchecked(array[(int)(checked((IntPtr)(unchecked(num - 1L))))] + (long)linesper) || num <= 1L || array2[(int)((IntPtr)(unchecked(num - 1L)))] > array2[(int)((IntPtr)(unchecked(num - 2L)))] || num3 >= unchecked(array[(int)(checked((IntPtr)(unchecked(num - 2L))))] + (long)linesper))
							{
								array[(int)((IntPtr)num)] = num3;
								float[] arg_A9_0 = array2;
								long expr_9E = num;
								num = unchecked(expr_9E + 1L);
								arg_A9_0[(int)((IntPtr)expr_9E)] = seeds[(int)((IntPtr)num3)];
								goto IL_AA;
							}
							unchecked
							{
								num -= 1L;
							}
						}
						array[(int)((IntPtr)num)] = num3;
						float[] arg_5B_0 = array2;
						long expr_50 = num;
						num = unchecked(expr_50 + 1L);
						arg_5B_0[(int)((IntPtr)expr_50)] = seeds[(int)((IntPtr)num3)];
					}
					IL_AA:;
				}
			}
			for (long num3 = 0L; num3 < num; num3 += 1L)
			{
				long num4;
				if (num3 < num - 1L && checked(array2[(int)((IntPtr)(unchecked(num3 + 1L)))] > array2[(int)((IntPtr)num3)]))
				{
					num4 = array[(int)(checked((IntPtr)(unchecked(num3 + 1L))))];
				}
				else
				{
					num4 = array[(int)(checked((IntPtr)num3))] + (long)linesper + 1L;
				}
				if (num4 > n)
				{
					num4 = n;
				}
				while (num2 < num4)
				{
					checked
					{
						seeds[(int)((IntPtr)num2)] = array2[(int)((IntPtr)num3)];
					}
					num2 += 1L;
				}
			}
		}

		private static void max_seeds(Xiph.vorbis_look_psy p, float[] seed, float[] flr, int flro)
		{
			long n = (long)p.total_octave_lines;
			int eighth_octave_lines = p.eighth_octave_lines;
			long num = 0L;
			Xiph.seed_chase(seed, eighth_octave_lines, n);
			long num2 = p.octave[0] - p.firstoc - (long)(eighth_octave_lines >> 1);
			while (num + 1L < (long)p.n)
			{
				float num3 = seed[(int)(checked((IntPtr)num2))];
				long num4 = (p.octave[(int)(checked((IntPtr)num))] + p.octave[(int)(checked((IntPtr)(unchecked(num + 1L))))] >> 1) - p.firstoc;
				if (num3 > p.vi.tone_abs_limit)
				{
					num3 = p.vi.tone_abs_limit;
				}
				while (num2 + 1L <= num4)
				{
					num2 += 1L;
					checked
					{
						if ((seed[(int)((IntPtr)num2)] > -9999f && seed[(int)((IntPtr)num2)] < num3) || num3 == -9999f)
						{
							num3 = seed[(int)((IntPtr)num2)];
						}
					}
				}
				num4 = num2 + p.firstoc;
				while (num < (long)p.n && p.octave[(int)(checked((IntPtr)num))] <= num4)
				{
					checked
					{
						if (flr[(int)((IntPtr)(unchecked((long)flro + num)))] < num3)
						{
							flr[(int)((IntPtr)(unchecked((long)flro + num)))] = num3;
						}
					}
					num += 1L;
				}
			}
			float num5 = seed[p.total_octave_lines - 1];
			while (num < (long)p.n)
			{
				checked
				{
					if (flr[(int)((IntPtr)(unchecked((long)flro + num)))] < num5)
					{
						flr[(int)((IntPtr)(unchecked((long)flro + num)))] = num5;
					}
				}
				num += 1L;
			}
		}

		private static void bark_noise_hybridmp(int n, long[] b, int bo, float[] f, int fo, float[] noise, int noiseo, float offset, int _fixed)
		{
			float[] array = new float[n];
			float[] array2 = new float[n];
			float[] array3 = new float[n];
			float[] array4 = new float[n];
			float[] array5 = new float[n];
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num8;
			float num7;
			float num6;
			float num5;
			float num4 = num5 = (num6 = (num7 = (num8 = 0f)));
			float num9 = f[fo] + offset;
			if (num9 < 1f)
			{
				num9 = 1f;
			}
			float num10 = num9 * num9 * 0.5f;
			num5 += num10;
			num4 += num10;
			num7 += num10 * num9;
			array[0] = num5;
			array2[0] = num4;
			array3[0] = num6;
			array4[0] = num7;
			array5[0] = num8;
			int i = 1;
			float num11 = 1f;
			while (i < n)
			{
				num9 = f[fo + i] + offset;
				if (num9 < 1f)
				{
					num9 = 1f;
				}
				num10 = num9 * num9;
				num5 += num10;
				num4 += num10 * num11;
				num6 += num10 * num11 * num11;
				num7 += num10 * num9;
				num8 += num10 * num11 * num9;
				array[i] = num5;
				array2[i] = num4;
				array3[i] = num6;
				array4[i] = num7;
				array5[i] = num8;
				i++;
				num11 += 1f;
			}
			i = 0;
			num11 = 0f;
			while (true)
			{
				int num12 = (int)(b[bo + i] >> 16);
				if (num12 >= 0)
				{
					break;
				}
				int num13 = (int)(b[bo + i] & 65535L);
				num5 = array[num13] + array[-num12];
				num4 = array2[num13] - array2[-num12];
				num6 = array3[num13] + array3[-num12];
				num7 = array4[num13] + array4[-num12];
				num8 = array5[num13] - array5[-num12];
				num = num7 * num6 - num4 * num8;
				num2 = num5 * num8 - num4 * num7;
				num3 = num5 * num6 - num4 * num4;
				float num14 = (num + num11 * num2) / num3;
				if (num14 < 0f)
				{
					num14 = 0f;
				}
				noise[noiseo + i] = num14 - offset;
				i++;
				num11 += 1f;
			}
			while (true)
			{
				int num12 = (int)(b[bo + i] >> 16);
				int num13 = (int)(b[bo + i] & 65535L);
				if (num13 >= n)
				{
					break;
				}
				num5 = array[num13] - array[num12];
				num4 = array2[num13] - array2[num12];
				num6 = array3[num13] - array3[num12];
				num7 = array4[num13] - array4[num12];
				num8 = array5[num13] - array5[num12];
				num = num7 * num6 - num4 * num8;
				num2 = num5 * num8 - num4 * num7;
				num3 = num5 * num6 - num4 * num4;
				float num14 = (num + num11 * num2) / num3;
				if (num14 < 0f)
				{
					num14 = 0f;
				}
				noise[noiseo + i] = num14 - offset;
				i++;
				num11 += 1f;
			}
			while (i < n)
			{
				float num14 = (num + num11 * num2) / num3;
				if (num14 < 0f)
				{
					num14 = 0f;
				}
				noise[noiseo + i] = num14 - offset;
				i++;
				num11 += 1f;
			}
			if (_fixed <= 0)
			{
				return;
			}
			i = 0;
			num11 = 0f;
			while (true)
			{
				int num13 = i + _fixed / 2;
				int num12 = num13 - _fixed;
				if (num12 >= 0)
				{
					break;
				}
				num5 = array[num13] + array[-num12];
				num4 = array2[num13] - array2[-num12];
				num6 = array3[num13] + array3[-num12];
				num7 = array4[num13] + array4[-num12];
				num8 = array5[num13] - array5[-num12];
				num = num7 * num6 - num4 * num8;
				num2 = num5 * num8 - num4 * num7;
				num3 = num5 * num6 - num4 * num4;
				float num14 = (num + num11 * num2) / num3;
				if (num14 - offset < noise[noiseo + i])
				{
					noise[noiseo + i] = num14 - offset;
				}
				i++;
				num11 += 1f;
			}
			while (true)
			{
				int num13 = i + _fixed / 2;
				int num12 = num13 - _fixed;
				if (num13 >= n)
				{
					break;
				}
				num5 = array[num13] - array[num12];
				num4 = array2[num13] - array2[num12];
				num6 = array3[num13] - array3[num12];
				num7 = array4[num13] - array4[num12];
				num8 = array5[num13] - array5[num12];
				num = num7 * num6 - num4 * num8;
				num2 = num5 * num8 - num4 * num7;
				num3 = num5 * num6 - num4 * num4;
				float num14 = (num + num11 * num2) / num3;
				if (num14 - offset < noise[noiseo + i])
				{
					noise[noiseo + i] = num14 - offset;
				}
				i++;
				num11 += 1f;
			}
			while (i < n)
			{
				float num14 = (num + num11 * num2) / num3;
				if (num14 - offset < noise[noiseo + i])
				{
					noise[noiseo + i] = num14 - offset;
				}
				i++;
				num11 += 1f;
			}
		}

		private static void _vp_noisemask(Xiph.vorbis_look_psy p, float[] logmdct, int logmdcto, float[] logmask, int logmasko)
		{
			int n = p.n;
			float[] array = new float[n];
			Xiph.bark_noise_hybridmp(n, p.bark, 0, logmdct, logmdcto, logmask, logmasko, 140f, -1);
			for (int i = 0; i < n; i++)
			{
				array[i] = logmdct[logmdcto + i] - logmask[logmasko + i];
			}
			Xiph.bark_noise_hybridmp(n, p.bark, 0, array, 0, logmask, logmasko, 0f, p.vi.noisewindowfixed);
			for (int i = 0; i < n; i++)
			{
				array[i] = logmdct[logmdcto + i] - array[i];
			}
			for (int i = 0; i < n; i++)
			{
				int num = (int)(logmask[logmasko + i] + 0.5f);
				if (num >= 40)
				{
					num = 39;
				}
				if (num < 0)
				{
					num = 0;
				}
				logmask[logmasko + i] = array[i] + p.vi.noisecompand[num];
			}
		}

		private static void _vp_tonemask(Xiph.vorbis_look_psy p, float[] logfft, int logffto, float[] logmask, int logmasko, float global_specmax, float local_specmax)
		{
			int n = p.n;
			float[] array = new float[p.total_octave_lines];
			float num = local_specmax + p.vi.ath_adjatt;
			for (int i = 0; i < p.total_octave_lines; i++)
			{
				array[i] = -9999f;
			}
			if (num < p.vi.ath_maxatt)
			{
				num = p.vi.ath_maxatt;
			}
			for (int i = 0; i < n; i++)
			{
				logmask[logmasko + i] = p.ath[i] + num;
			}
			Xiph.seed_loop(p, p.tonecurves, logfft, logffto, logmask, logmasko, array, global_specmax);
			Xiph.max_seeds(p, array, logmask, logmasko);
		}

		private static void _vp_offset_and_mix(Xiph.vorbis_look_psy p, float[] noise, float[] tone, int offset_select, float[] logmask, float[] mdct, float[] logmdct, int logmdcto)
		{
			int n = p.n;
			float num = p.vi.tone_masteratt[offset_select];
			float val = p.m_val;
			for (int i = 0; i < n; i++)
			{
				float num2 = noise[i] + p.noiseoffset[offset_select][i];
				if (num2 > p.vi.noisemaxsupp)
				{
					num2 = p.vi.noisemaxsupp;
				}
				logmask[i] = Xiph.max(num2, tone[i] + num);
				if (offset_select == 1)
				{
					float num3 = -17.2f;
					num2 -= logmdct[logmdcto + i];
					float num4;
					if (num2 > num3)
					{
						num4 = 1f - (num2 - num3) * 0.005f * val;
						if (num4 < 0f)
						{
							num4 = 0.0001f;
						}
					}
					else
					{
						num4 = 1f - (num2 - num3) * 0.0003f * val;
					}
					mdct[i] *= num4;
				}
			}
		}

		private static void sort_qsort(float[] q, int[] sort, int left, int right)
		{
			int i = left;
			int num = right;
			int num2 = sort[(left + right) / 2];
			while (i <= num)
			{
				while (q[sort[i]] < q[num2])
				{
					i++;
				}
				while (q[sort[num]] > q[num2])
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = sort[i];
					sort[i] = sort[num];
					sort[num] = num3;
					i++;
					num--;
				}
			}
			if (left < num)
			{
				Xiph.sort_qsort(q, sort, left, num);
			}
			if (i < right)
			{
				Xiph.sort_qsort(q, sort, i, right);
			}
		}

		private static void flag_lossless(int limit, float prepoint, float postpoint, float[] mdct, int mdcto, float[] floor, int[] flag, int i, int jn)
		{
			for (int j = 0; j < jn; j++)
			{
				float num = (j >= limit - i) ? postpoint : prepoint;
				float num2 = Xiph.fabs(mdct[mdcto + j]) / floor[j];
				if (num2 < num)
				{
					flag[j] = 0;
				}
				else
				{
					flag[j] = 1;
				}
			}
		}

		private static float noise_normalize(Xiph.vorbis_look_psy p, int limit, float[] r, float[] q, float[] f, int[] flags, float acc, int i, int n, int[] _out, int _outo)
		{
			Xiph.vorbis_info_psy vi = p.vi;
			int[] array = new int[n];
			int num = 0;
			int num2 = (vi.normal_p != 0) ? (vi.normal_start - i) : n;
			if (num2 > n)
			{
				num2 = n;
			}
			acc = 0f;
			int j;
			for (j = 0; j < num2; j++)
			{
				if (flags == null || flags[j] == 0)
				{
					float x = q[j] / f[j];
					if (r[j] < 0f)
					{
						_out[_outo + j] = -Xiph.rint(Xiph.sqrt(x));
					}
					else
					{
						_out[_outo + j] = Xiph.rint(Xiph.sqrt(x));
					}
				}
			}
			while (j < n)
			{
				if (flags == null || flags[j] == 0)
				{
					float num3 = q[j] / f[j];
					if (num3 < 0.25f && (flags == null || j >= limit - i))
					{
						acc += num3;
						array[num++] = j;
					}
					else
					{
						if (r[j] < 0f)
						{
							_out[_outo + j] = -Xiph.rint(Xiph.sqrt(num3));
						}
						else
						{
							_out[_outo + j] = Xiph.rint(Xiph.sqrt(num3));
						}
						q[j] = (float)(_out[_outo + j] * _out[_outo + j]) * f[j];
					}
				}
				j++;
			}
			if (num != 0)
			{
				Xiph.sort_qsort(q, array, 0, num - 1);
				for (j = 0; j < num; j++)
				{
					int num4 = array[j];
					if ((double)acc >= vi.normal_thresh)
					{
						_out[_outo + num4] = (int)Xiph.unitnorm(r[num4]);
						acc -= 1f;
						q[num4] = f[num4];
					}
					else
					{
						_out[_outo + num4] = 0;
						q[num4] = 0f;
					}
				}
			}
			return acc;
		}

		private static void _vp_couple_quantize_normalize(int blobno, Xiph.vorbis_info_psy_global g, Xiph.vorbis_look_psy p, Xiph.vorbis_info_mapping0 vi, float[][] mdct, int[][] iwork, int[] nonzero, int sliding_lowpass, int ch)
		{
			int n = p.n;
			int num = (p.vi.normal_p != 0) ? p.vi.normal_partition : 16;
			int num2 = g.coupling_pointlimit[p.vi.blockflag][blobno];
			float prepoint = (float)Xiph.stereo_threshholds[g.coupling_prepointamp[blobno]];
			float postpoint = (float)Xiph.stereo_threshholds[g.coupling_postpointamp[blobno]];
			float[][] array = new float[ch][];
			float[][] array2 = new float[ch][];
			float[][] array3 = new float[ch][];
			int[][] array4 = new int[ch][];
			int[] array5 = new int[ch];
			float[] array6 = new float[ch + vi.coupling_steps];
			if (n > 1000)
			{
				postpoint = (float)Xiph.stereo_threshholds_limited[g.coupling_postpointamp[blobno]];
			}
			for (int i = 0; i < ch; i++)
			{
				array[i] = new float[num];
				array2[i] = new float[num];
				array3[i] = new float[num];
				array4[i] = new int[num];
			}
			for (int i = 0; i < ch + vi.coupling_steps; i++)
			{
				array6[i] = 0f;
			}
			for (int i = 0; i < n; i += num)
			{
				int num3 = (num > n - i) ? (n - i) : num;
				int num4 = 0;
				for (int j = 0; j < ch; j++)
				{
					array5[j] = nonzero[j];
				}
				for (int j = 0; j < ch; j++)
				{
					for (int k = 0; k < num; k++)
					{
						array4[j][k] = 0;
					}
				}
				for (int k = 0; k < ch; k++)
				{
					int num5 = i;
					if (array5[k] != 0)
					{
						for (int j = 0; j < num3; j++)
						{
							array3[k][j] = Xiph.FLOOR1_fromdB_LOOKUP[iwork[k][num5 + j]];
						}
						Xiph.flag_lossless(num2, prepoint, postpoint, mdct[k], i, array3[k], array4[k], i, num3);
						for (int j = 0; j < num3; j++)
						{
							array2[k][j] = (array[k][j] = mdct[k][i + j] * mdct[k][i + j]);
							if (mdct[k][i + j] < 0f)
							{
								array[k][j] *= -1f;
							}
							array3[k][j] *= array3[k][j];
						}
						array6[num4] = Xiph.noise_normalize(p, num2, array[k], array2[k], array3[k], null, array6[num4], i, num3, iwork[k], num5);
					}
					else
					{
						for (int j = 0; j < num3; j++)
						{
							array3[k][j] = 1E-10f;
							array[k][j] = 0f;
							array2[k][j] = 0f;
							array4[k][j] = 0;
							iwork[k][num5 + j] = 0;
						}
						array6[num4] = 0f;
					}
					num4++;
				}
				for (int l = 0; l < vi.coupling_steps; l++)
				{
					int num6 = vi.coupling_mag[l];
					int num7 = vi.coupling_ang[l];
					int num8 = i;
					int num9 = i;
					float[] array7 = array[num6];
					float[] array8 = array[num7];
					float[] array9 = array2[num6];
					float[] array10 = array2[num7];
					float[] array11 = array3[num6];
					float[] array12 = array3[num7];
					int[] array13 = array4[num6];
					int[] array14 = array4[num7];
					if (array5[num6] != 0 || array5[num7] != 0)
					{
						array5[num6] = (array5[num7] = 1);
						for (int j = 0; j < num3; j++)
						{
							if (j < sliding_lowpass - i)
							{
								if (array13[j] != 0 || array14[j] != 0)
								{
									array7[j] = Xiph.fabs(array7[j]) + Xiph.fabs(array8[j]);
									array9[j] += array10[j];
									array13[j] = (array14[j] = 1);
									int num10 = iwork[num6][num8 + j];
									int num11 = iwork[num7][num9 + j];
									if (Xiph.abs(num10) > Xiph.abs(num11))
									{
										iwork[num7][num9 + j] = ((num10 > 0) ? (num10 - num11) : (num11 - num10));
									}
									else
									{
										iwork[num7][num9 + j] = ((num11 > 0) ? (num10 - num11) : (num11 - num10));
										iwork[num6][num8 + j] = num11;
									}
									if (iwork[num7][num9 + j] >= Xiph.abs(iwork[num6][num8 + j]) * 2)
									{
										iwork[num7][num9 + j] = -iwork[num7][num9 + j];
										iwork[num6][num8 + j] = -iwork[num6][num8 + j];
									}
								}
								else
								{
									if (j < num2 - i)
									{
										array7[j] += array8[j];
										array9[j] = Xiph.fabs(array7[j]);
									}
									else if (array7[j] + array8[j] < 0f)
									{
										array7[j] = -(array9[j] = Xiph.fabs(array7[j]) + Xiph.fabs(array8[j]));
									}
									else
									{
										array7[j] = (array9[j] = Xiph.fabs(array7[j]) + Xiph.fabs(array8[j]));
									}
									array8[j] = (array10[j] = 0f);
									array14[j] = 1;
									iwork[num7][num9 + j] = 0;
								}
							}
							array11[j] = (array12[j] = array11[j] + array12[j]);
						}
						array6[num4] = Xiph.noise_normalize(p, num2, array[num6], array2[num6], array3[num6], array4[num6], array6[num4], i, num3, iwork[num6], num8);
						num4++;
					}
				}
			}
			for (int i = 0; i < vi.coupling_steps; i++)
			{
				if (nonzero[vi.coupling_mag[i]] != 0 || nonzero[vi.coupling_ang[i]] != 0)
				{
					nonzero[vi.coupling_mag[i]] = 1;
					nonzero[vi.coupling_ang[i]] = 1;
				}
			}
		}

		private static void res0_free_info(Xiph.vorbis_info_residue i)
		{
		}

		private static void res0_free_look(Xiph.vorbis_look_residue i)
		{
			if (i != null)
			{
				Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)i;
				vorbis_look_residue.partbooks = null;
				vorbis_look_residue.decodemap = null;
			}
		}

		private static int icount(uint v)
		{
			int num = 0;
			while (v != 0u)
			{
				num += (int)(v & 1u);
				v >>= 1;
			}
			return num;
		}

		private static void res0_pack(Xiph.vorbis_info_residue vr, Xiph.oggpack_buffer opb)
		{
			Xiph.vorbis_info_residue0 vorbis_info_residue = (Xiph.vorbis_info_residue0)vr;
			int num = 0;
			Xiph.oggpack_write(opb, vorbis_info_residue.begin, 24);
			Xiph.oggpack_write(opb, vorbis_info_residue.end, 24);
			Xiph.oggpack_write(opb, (long)(vorbis_info_residue.grouping - 1), 24);
			Xiph.oggpack_write(opb, (long)(vorbis_info_residue.partitions - 1), 6);
			Xiph.oggpack_write(opb, (long)vorbis_info_residue.groupbook, 8);
			for (int i = 0; i < vorbis_info_residue.partitions; i++)
			{
				if (Xiph.ilog((uint)vorbis_info_residue.secondstages[i]) > 3)
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_residue.secondstages[i], 3);
					Xiph.oggpack_write(opb, 1L, 1);
					Xiph.oggpack_write(opb, (long)(vorbis_info_residue.secondstages[i] >> 3), 5);
				}
				else
				{
					Xiph.oggpack_write(opb, (long)vorbis_info_residue.secondstages[i], 4);
				}
				num += Xiph.icount((uint)vorbis_info_residue.secondstages[i]);
			}
			for (int i = 0; i < num; i++)
			{
				Xiph.oggpack_write(opb, (long)vorbis_info_residue.booklist[i], 8);
			}
		}

		private static Xiph.vorbis_info_residue res0_unpack(Xiph.vorbis_info vi, Xiph.oggpack_buffer opb)
		{
			int num = 0;
			Xiph.vorbis_info_residue0 vorbis_info_residue = new Xiph.vorbis_info_residue0();
			Xiph.codec_setup_info codec_setup = vi.codec_setup;
			vorbis_info_residue.begin = Xiph.oggpack_read(opb, 24);
			vorbis_info_residue.end = Xiph.oggpack_read(opb, 24);
			vorbis_info_residue.grouping = (int)(Xiph.oggpack_read(opb, 24) + 1L);
			vorbis_info_residue.partitions = (int)(Xiph.oggpack_read(opb, 6) + 1L);
			vorbis_info_residue.groupbook = (int)Xiph.oggpack_read(opb, 8);
			if (vorbis_info_residue.groupbook >= 0)
			{
				for (int i = 0; i < vorbis_info_residue.partitions; i++)
				{
					int num2 = (int)Xiph.oggpack_read(opb, 3);
					int num3 = (int)Xiph.oggpack_read(opb, 1);
					if (num3 < 0)
					{
						goto IL_18D;
					}
					if (num3 != 0)
					{
						int num4 = (int)Xiph.oggpack_read(opb, 5);
						if (num4 < 0)
						{
							goto IL_18D;
						}
						num2 |= num4 << 3;
					}
					vorbis_info_residue.secondstages[i] = num2;
					num += Xiph.icount((uint)num2);
				}
				for (int i = 0; i < num; i++)
				{
					int num5 = (int)Xiph.oggpack_read(opb, 8);
					if (num5 < 0)
					{
						goto IL_18D;
					}
					vorbis_info_residue.booklist[i] = num5;
				}
				if (vorbis_info_residue.groupbook < codec_setup.books)
				{
					for (int i = 0; i < num; i++)
					{
						if (vorbis_info_residue.booklist[i] >= codec_setup.books || codec_setup.book_param[vorbis_info_residue.booklist[i]].maptype == 0)
						{
							goto IL_18D;
						}
					}
					int num6 = (int)codec_setup.book_param[vorbis_info_residue.groupbook].entries;
					int j = (int)codec_setup.book_param[vorbis_info_residue.groupbook].dim;
					int num7 = 1;
					if (j >= 1)
					{
						while (j > 0)
						{
							num7 *= vorbis_info_residue.partitions;
							if (num7 > num6)
							{
								goto IL_18D;
							}
							j--;
						}
						vorbis_info_residue.partvals = num7;
						return vorbis_info_residue;
					}
				}
			}
			IL_18D:
			Xiph.res0_free_info(vorbis_info_residue);
			return null;
		}

		private static Xiph.vorbis_look_residue res0_look(Xiph.vorbis_dsp_state vd, Xiph.vorbis_info_residue vr)
		{
			Xiph.vorbis_info_residue0 vorbis_info_residue = (Xiph.vorbis_info_residue0)vr;
			Xiph.vorbis_look_residue0 vorbis_look_residue = new Xiph.vorbis_look_residue0();
			Xiph.codec_setup_info codec_setup = vd.vi.codec_setup;
			int num = 0;
			int num2 = 0;
			vorbis_look_residue.info = vorbis_info_residue;
			vorbis_look_residue.parts = vorbis_info_residue.partitions;
			vorbis_look_residue.fullbooks = codec_setup.fullbooks;
			vorbis_look_residue.phrasebook = codec_setup.fullbooks[vorbis_info_residue.groupbook];
			int num3 = (int)vorbis_look_residue.phrasebook.dim;
			vorbis_look_residue.partbooks = new Xiph.codebook[vorbis_look_residue.parts][];
			for (int i = 0; i < vorbis_look_residue.parts; i++)
			{
				int num4 = Xiph.ilog((uint)vorbis_info_residue.secondstages[i]);
				if (num4 != 0)
				{
					if (num4 > num2)
					{
						num2 = num4;
					}
					vorbis_look_residue.partbooks[i] = new Xiph.codebook[num4];
					for (int j = 0; j < num4; j++)
					{
						if ((vorbis_info_residue.secondstages[i] & 1 << j) != 0)
						{
							vorbis_look_residue.partbooks[i][j] = codec_setup.fullbooks[vorbis_info_residue.booklist[num++]];
						}
					}
				}
			}
			vorbis_look_residue.partvals = 1;
			for (int i = 0; i < num3; i++)
			{
				vorbis_look_residue.partvals *= vorbis_look_residue.parts;
			}
			vorbis_look_residue.stages = num2;
			vorbis_look_residue.decodemap = new int[vorbis_look_residue.partvals][];
			for (int i = 0; i < vorbis_look_residue.partvals; i++)
			{
				long num5 = (long)i;
				long num6 = (long)(vorbis_look_residue.partvals / vorbis_look_residue.parts);
				vorbis_look_residue.decodemap[i] = new int[num3];
				for (int j = 0; j < num3; j++)
				{
					long num7 = num5 / num6;
					num5 -= num7 * num6;
					num6 /= (long)vorbis_look_residue.parts;
					vorbis_look_residue.decodemap[i][j] = (int)num7;
				}
			}
			return vorbis_look_residue;
		}

		private static int local_book_besterror(Xiph.codebook book, int[] a, int ao)
		{
			int num = (int)book.dim;
			int minval = book.minval;
			int delta = book.delta;
			int quantvals = book.quantvals;
			int num2 = quantvals >> 1;
			int num3 = 0;
			int[] array = new int[8];
			int[] array2 = array;
			if (delta != 1)
			{
				int i = 0;
				int j = num;
				while (i < num)
				{
					int num4 = (a[ao + --j] - minval + (delta >> 1)) / delta;
					int num5 = (num4 < num2) ? ((num2 - num4 << 1) - 1) : (num4 - num2 << 1);
					num3 = num3 * quantvals + ((num5 < 0) ? 0 : ((num5 >= quantvals) ? (quantvals - 1) : num5));
					array2[j] = num4 * delta + minval;
					i++;
				}
			}
			else
			{
				int i = 0;
				int j = num;
				while (i < num)
				{
					int num6 = a[ao + --j] - minval;
					int num7 = (num6 < num2) ? ((num2 - num6 << 1) - 1) : (num6 - num2 << 1);
					num3 = num3 * quantvals + ((num7 < 0) ? 0 : ((num7 >= quantvals) ? (quantvals - 1) : num7));
					array2[j] = num6 * delta + minval;
					i++;
				}
			}
			if (book.c.lengthlist[num3] <= 0L)
			{
				Xiph.static_codebook c = book.c;
				int num8 = -1;
				int[] array3 = new int[8];
				int[] array4 = array3;
				int num9 = book.minval + book.delta * (book.quantvals - 1);
				int i = 0;
				while ((long)i < book.entries)
				{
					int k;
					if (c.lengthlist[i] > 0L)
					{
						int num10 = 0;
						for (k = 0; k < num; k++)
						{
							int num11 = array4[k] - a[ao + k];
							num10 += num11 * num11;
						}
						if (num8 == -1 || num10 < num8)
						{
							for (int j = 0; j < 8; j++)
							{
								array4[j] = array2[j];
							}
							num8 = num10;
							num3 = i;
						}
					}
					k = 0;
					while (array4[k] >= num9)
					{
						array4[k++] = 0;
					}
					if (array4[k] >= 0)
					{
						array4[k] += book.delta;
					}
					array4[k] = -array4[k];
					i++;
				}
			}
			if (num3 > -1)
			{
				for (int i = 0; i < num; i++)
				{
					a[ao + i] -= array2[i];
				}
			}
			return num3;
		}

		private static int _encodepart(Xiph.oggpack_buffer opb, int[] vec, int vo, int n, Xiph.codebook book, long[] acc)
		{
			int num = 0;
			int num2 = (int)book.dim;
			int num3 = n / num2;
			for (int i = 0; i < num3; i++)
			{
				int a = Xiph.local_book_besterror(book, vec, vo + i * num2);
				num += Xiph.vorbis_book_encode(book, a, opb);
			}
			return num;
		}

		private static long[][] _01class(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int ch)
		{
			Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)vl;
			Xiph.vorbis_info_residue0 info = vorbis_look_residue.info;
			int grouping = info.grouping;
			int partitions = info.partitions;
			int num = (int)(info.end - info.begin);
			int num2 = num / grouping;
			long[][] array = new long[ch][];
			float num3 = 100f / (float)grouping;
			for (long num4 = 0L; num4 < (long)ch; num4 += 1L)
			{
				array[(int)(checked((IntPtr)num4))] = new long[num / grouping];
			}
			for (long num4 = 0L; num4 < (long)num2; num4 += 1L)
			{
				int num5 = (int)(num4 * (long)grouping + info.begin);
				for (long num6 = 0L; num6 < (long)ch; num6 += 1L)
				{
					int num7 = 0;
					int num8 = 0;
					long num9;
					for (num9 = 0L; num9 < (long)grouping; num9 += 1L)
					{
						checked
						{
							if (Xiph.abs(_in[(int)((IntPtr)num6)][(int)((IntPtr)(unchecked((long)num5 + num9)))]) > num7)
							{
								num7 = Xiph.abs(_in[(int)((IntPtr)num6)][(int)((IntPtr)(unchecked((long)num5 + num9)))]);
							}
						}
						num8 += Xiph.abs(checked(_in[(int)((IntPtr)num6)][(int)((IntPtr)(unchecked((long)num5 + num9)))]));
					}
					num8 *= (int)num3;
					num9 = 0L;
					while (num9 < (long)(partitions - 1) && checked(num7 > info.classmetric1[(int)((IntPtr)num9)] || (info.classmetric2[(int)((IntPtr)num9)] >= 0 && num8 >= info.classmetric2[(int)((IntPtr)num9)])))
					{
						num9 += 1L;
					}
					checked(array[(int)((IntPtr)num6)][(int)((IntPtr)num4)]) = num9;
				}
			}
			vorbis_look_residue.frames += 1L;
			return array;
		}

		private static long[][] _2class(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int ch)
		{
			Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)vl;
			Xiph.vorbis_info_residue0 info = vorbis_look_residue.info;
			int grouping = info.grouping;
			int partitions = info.partitions;
			int num = (int)(info.end - info.begin);
			int num2 = num / grouping;
			long[][] array = new long[][]
			{
				new long[num2]
			};
			long num3 = 0L;
			long num4 = info.begin / (long)ch;
			while (num3 < (long)num2)
			{
				int num5 = 0;
				int num6 = 0;
				long num7;
				for (num7 = 0L; num7 < (long)grouping; num7 += (long)ch)
				{
					checked
					{
						if (Xiph.abs(_in[0][(int)((IntPtr)num4)]) > num5)
						{
							num5 = Xiph.abs(_in[0][(int)((IntPtr)num4)]);
						}
					}
					for (long num8 = 1L; num8 < (long)ch; num8 += 1L)
					{
						checked
						{
							if (Xiph.abs(_in[(int)((IntPtr)num8)][(int)((IntPtr)num4)]) > num6)
							{
								num6 = Xiph.abs(_in[(int)((IntPtr)num8)][(int)((IntPtr)num4)]);
							}
						}
					}
					num4 += 1L;
				}
				num7 = 0L;
				while (num7 < (long)(partitions - 1) && checked(num5 > info.classmetric1[(int)((IntPtr)num7)] || num6 > info.classmetric2[(int)((IntPtr)num7)]))
				{
					num7 += 1L;
				}
				array[0][(int)(checked((IntPtr)num3))] = num7;
				num3 += 1L;
			}
			vorbis_look_residue.frames += 1L;
			return array;
		}

		private static int _01forward(Xiph.oggpack_buffer opb, Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int ch, long[][] partword, Xiph.Encode encode, int submap)
		{
			Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)vl;
			Xiph.vorbis_info_residue0 info = vorbis_look_residue.info;
			int grouping = info.grouping;
			int partitions = info.partitions;
			int num = (int)vorbis_look_residue.phrasebook.dim;
			int num2 = (int)(info.end - info.begin);
			int num3 = num2 / grouping;
			long[] array = new long[128];
			long[] array2 = new long[128];
			for (long num4 = 0L; num4 < (long)vorbis_look_residue.stages; num4 += 1L)
			{
				long num5 = 0L;
				while (num5 < (long)num3)
				{
					long num8;
					if (num4 == 0L)
					{
						for (long num6 = 0L; num6 < (long)ch; num6 += 1L)
						{
							long num7 = checked(partword[(int)((IntPtr)num6)][(int)((IntPtr)num5)]);
							for (num8 = 1L; num8 < (long)num; num8 += 1L)
							{
								num7 *= (long)partitions;
								if (num5 + num8 < (long)num3)
								{
									num7 += checked(partword[(int)((IntPtr)num6)][(int)((IntPtr)(unchecked(num5 + num8)))]);
								}
							}
							if (num7 < vorbis_look_residue.phrasebook.entries)
							{
								vorbis_look_residue.phrasebits += (long)Xiph.vorbis_book_encode(vorbis_look_residue.phrasebook, (int)num7, opb);
							}
						}
					}
					num8 = 0L;
					while (num8 < (long)num && num5 < (long)num3)
					{
						long num9 = num5 * (long)grouping + info.begin;
						for (long num6 = 0L; num6 < (long)ch; num6 += 1L)
						{
							if (num4 == 0L)
							{
								array2[(int)(checked((IntPtr)partword[(int)((IntPtr)num6)][(int)((IntPtr)num5)]))] += (long)grouping;
							}
							if ((info.secondstages[(int)(checked((IntPtr)partword[(int)((IntPtr)num6)][(int)((IntPtr)num5)]))] & 1 << (int)num4) != 0)
							{
								Xiph.codebook codebook = checked(vorbis_look_residue.partbooks[(int)((IntPtr)partword[(int)((IntPtr)num6)][(int)((IntPtr)num5)])][(int)((IntPtr)num4)]);
								if (codebook != null)
								{
									long[] acc = null;
									int num10 = encode(opb, _in[(int)(checked((IntPtr)num6))], (int)num9, grouping, codebook, acc);
									vorbis_look_residue.postbits += (long)num10;
									array[(int)(checked((IntPtr)partword[(int)((IntPtr)num6)][(int)((IntPtr)num5)]))] += (long)num10;
								}
							}
						}
						num8 += 1L;
						num5 += 1L;
					}
				}
			}
			return 0;
		}

		private static int _01inverse(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, float[][] _in, int ch, Xiph.Decodepart decodepart)
		{
			Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)vl;
			Xiph.vorbis_info_residue0 info = vorbis_look_residue.info;
			int grouping = info.grouping;
			int num = (int)vorbis_look_residue.phrasebook.dim;
			int num2 = vb.pcmend >> 1;
			int num3 = (info.end < (long)num2) ? ((int)info.end) : num2;
			int num4 = (int)((long)num3 - info.begin);
			if (num4 > 0)
			{
				int num5 = num4 / grouping;
				int num6 = (num5 + num - 1) / num;
				int[][][] array = new int[ch][][];
				for (long num7 = 0L; num7 < (long)ch; num7 += 1L)
				{
					array[(int)(checked((IntPtr)num7))] = new int[num6][];
				}
				for (long num8 = 0L; num8 < (long)vorbis_look_residue.stages; num8 += 1L)
				{
					long num9 = 0L;
					long num10 = 0L;
					while (num9 < (long)num5)
					{
						if (num8 == 0L)
						{
							for (long num7 = 0L; num7 < (long)ch; num7 += 1L)
							{
								int num11 = (int)Xiph.vorbis_book_decode(vorbis_look_residue.phrasebook, vb.opb);
								if (num11 == -1 || num11 >= info.partvals)
								{
									return 0;
								}
								checked
								{
									array[(int)((IntPtr)num7)][(int)((IntPtr)num10)] = vorbis_look_residue.decodemap[num11];
									if (array[(int)((IntPtr)num7)][(int)((IntPtr)num10)] == null)
									{
										return 0;
									}
								}
							}
						}
						long num12 = 0L;
						while (num12 < (long)num && num9 < (long)num5)
						{
							for (long num7 = 0L; num7 < (long)ch; num7 += 1L)
							{
								long num13 = info.begin + num9 * (long)grouping;
								if ((info.secondstages[checked(array[(int)((IntPtr)num7)][(int)((IntPtr)num10)][(int)((IntPtr)num12)])] & 1 << (int)num8) != 0)
								{
									Xiph.codebook codebook = checked(vorbis_look_residue.partbooks[array[(int)((IntPtr)num7)][(int)((IntPtr)num10)][(int)((IntPtr)num12)]][(int)((IntPtr)num8)]);
									if (codebook != null && decodepart(codebook, _in[(int)(checked((IntPtr)num7))], (int)num13, vb.opb, grouping) == -1L)
									{
										return 0;
									}
								}
							}
							num12 += 1L;
							num9 += 1L;
						}
						num10 += 1L;
					}
				}
			}
			return 0;
		}

		private static int res0_inverse(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, float[][] _in, int[] nonzero, int ch)
		{
			int num = 0;
			for (int i = 0; i < ch; i++)
			{
				if (nonzero[i] != 0)
				{
					_in[num++] = _in[i];
				}
			}
			if (num != 0)
			{
				return Xiph._01inverse(vb, vl, _in, num, new Xiph.Decodepart(Xiph.vorbis_book_decodevs_add));
			}
			return 0;
		}

		private static int res1_forward(Xiph.oggpack_buffer opb, Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int[] nonzero, int ch, long[][] partword, int submap)
		{
			int num = 0;
			for (int i = 0; i < ch; i++)
			{
				if (nonzero[i] != 0)
				{
					_in[num++] = _in[i];
				}
			}
			if (num != 0)
			{
				return Xiph._01forward(opb, vb, vl, _in, num, partword, new Xiph.Encode(Xiph._encodepart), submap);
			}
			return 0;
		}

		private static long[][] res1_class(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int[] nonzero, int ch)
		{
			int num = 0;
			for (int i = 0; i < ch; i++)
			{
				if (nonzero[i] != 0)
				{
					_in[num++] = _in[i];
				}
			}
			if (num != 0)
			{
				return Xiph._01class(vb, vl, _in, num);
			}
			return null;
		}

		private static int res1_inverse(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, float[][] _in, int[] nonzero, int ch)
		{
			int num = 0;
			for (int i = 0; i < ch; i++)
			{
				if (nonzero[i] != 0)
				{
					_in[num++] = _in[i];
				}
			}
			if (num != 0)
			{
				return Xiph._01inverse(vb, vl, _in, num, new Xiph.Decodepart(Xiph.vorbis_book_decodev_add));
			}
			return 0;
		}

		private static long[][] res2_class(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int[] nonzero, int ch)
		{
			int num = 0;
			for (int i = 0; i < ch; i++)
			{
				if (nonzero[i] != 0)
				{
					num++;
				}
			}
			if (num != 0)
			{
				return Xiph._2class(vb, vl, _in, ch);
			}
			return null;
		}

		private static int res2_forward(Xiph.oggpack_buffer opb, Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, int[][] _in, int[] nonzero, int ch, long[][] partword, int submap)
		{
			long num = (long)(vb.pcmend / 2);
			long num2 = 0L;
			int[] array = new int[(long)ch * num];
			for (long num3 = 0L; num3 < (long)ch; num3 += 1L)
			{
				int[] array2 = _in[(int)(checked((IntPtr)num3))];
				if (nonzero[(int)(checked((IntPtr)num3))] != 0)
				{
					num2 += 1L;
				}
				long num4 = 0L;
				long num5 = num3;
				while (num4 < num)
				{
					checked
					{
						array[(int)((IntPtr)num5)] = array2[(int)((IntPtr)num4)];
					}
					num4 += 1L;
					num5 += (long)ch;
				}
			}
			if (num2 != 0L)
			{
				return Xiph._01forward(opb, vb, vl, new int[][]
				{
					array
				}, 1, partword, new Xiph.Encode(Xiph._encodepart), submap);
			}
			return 0;
		}

		private static int res2_inverse(Xiph.vorbis_block vb, Xiph.vorbis_look_residue vl, float[][] _in, int[] nonzero, int ch)
		{
			Xiph.vorbis_look_residue0 vorbis_look_residue = (Xiph.vorbis_look_residue0)vl;
			Xiph.vorbis_info_residue0 info = vorbis_look_residue.info;
			int grouping = info.grouping;
			int num = (int)vorbis_look_residue.phrasebook.dim;
			int num2 = vb.pcmend * ch >> 1;
			int num3 = (info.end < (long)num2) ? ((int)info.end) : num2;
			int num4 = (int)((long)num3 - info.begin);
			if (num4 > 0)
			{
				int num5 = num4 / grouping;
				int num6 = (num5 + num - 1) / num;
				int[][] array = new int[num6][];
				long num7 = 0L;
				while (num7 < (long)ch && nonzero[(int)(checked((IntPtr)num7))] == 0)
				{
					num7 += 1L;
				}
				if (num7 == (long)ch)
				{
					return 0;
				}
				for (long num8 = 0L; num8 < (long)vorbis_look_residue.stages; num8 += 1L)
				{
					num7 = 0L;
					long num9 = 0L;
					while (num7 < (long)num5)
					{
						if (num8 == 0L)
						{
							int num10 = (int)Xiph.vorbis_book_decode(vorbis_look_residue.phrasebook, vb.opb);
							if (num10 == -1 || num10 >= info.partvals)
							{
								return 0;
							}
							checked
							{
								array[(int)((IntPtr)num9)] = vorbis_look_residue.decodemap[num10];
								if (array[(int)((IntPtr)num9)] == null)
								{
									return 0;
								}
							}
						}
						long num11 = 0L;
						while (num11 < (long)num && num7 < (long)num5)
						{
							if ((info.secondstages[checked(array[(int)((IntPtr)num9)][(int)((IntPtr)num11)])] & 1 << (int)num8) != 0)
							{
								Xiph.codebook codebook = checked(vorbis_look_residue.partbooks[array[(int)((IntPtr)num9)][(int)((IntPtr)num11)]][(int)((IntPtr)num8)]);
								if (codebook != null && Xiph.vorbis_book_decodevv_add(codebook, _in, num7 * (long)grouping + info.begin, ch, vb.opb, grouping) == -1L)
								{
									return 0;
								}
							}
							num11 += 1L;
							num7 += 1L;
						}
						num9 += 1L;
					}
				}
			}
			return 0;
		}

		private static float todB(float x)
		{
			Xiph.uint_float uint_float;
			uint_float.i = 0u;
			uint_float.f = x;
			uint_float.i &= 2147483647u;
			return uint_float.i * 7.1771143E-07f - 764.6162f;
		}

		private static float unitnorm(float x)
		{
			if (x < 0f)
			{
				return -1f;
			}
			return 1f;
		}

		private static float fromdB(float x)
		{
			return Xiph.exp(x * 0.115129247f);
		}

		private static float toBARK(float n)
		{
			return 13.1f * Xiph.atan(0.00074f * n) + 2.24f * Xiph.atan(n * n * 1.85E-08f) + 0.0001f * n;
		}

		private static float toOC(float n)
		{
			return Xiph.log(n) * 1.442695f - 5.965784f;
		}

		private static float fromOC(float o)
		{
			return Xiph.exp((o + 5.965784f) * 0.693147f);
		}

		private static int _ilog(uint v)
		{
			int num = 0;
			while (v != 0u)
			{
				num++;
				v >>= 1;
			}
			return num;
		}

		private static int _ilog(long lv)
		{
			uint num = (uint)lv;
			int num2 = 0;
			while (num != 0u)
			{
				num2++;
				num >>= 1;
			}
			return num2;
		}

		private static long _float32_pack(float val)
		{
			int num = 0;
			if (val < 0f)
			{
				num = -2147483648;
				val = -val;
			}
			long num2 = (long)Math.Floor(Math.Log((double)val) / Math.Log(2.0) + 0.001);
			long num3 = (long)Xiph.rint((double)val * Math.Pow(2.0, (double)(20L - num2)) + 0.5);
			num2 = num2 + 768L << 21;
			return (long)num | num2 | num3;
		}

		private static float _float32_unpack(long val)
		{
			double num = (double)(val & 2097151L);
			int num2 = (int)(val & -2147483648L);
			long num3 = (val & 2145386496L) >> 21;
			if (num2 != 0)
			{
				num = -num;
			}
			return (float)(num * Math.Pow(2.0, (double)(num3 - 20L - 768L)));
		}

		private static uint[] _make_words(long[] l, long n, long sparsecount)
		{
			long num = 0L;
			uint[] array = new uint[33];
			uint[] array2 = new uint[(sparsecount != 0L) ? sparsecount : n];
			long num2;
			for (num2 = 0L; num2 < n; num2 += 1L)
			{
				long num3 = l[(int)(checked((IntPtr)num2))];
				if (num3 > 0L)
				{
					uint num4 = array[(int)(checked((IntPtr)num3))];
					if (num3 < 32L && num4 >> (int)num3 != 0u)
					{
						return null;
					}
					uint[] arg_59_0 = array2;
					long expr_51 = num;
					num = expr_51 + 1L;
					arg_59_0[(int)(checked((IntPtr)expr_51))] = num4;
					long num5 = num3;
					while (num5 > 0L)
					{
						if ((array[(int)(checked((IntPtr)num5))] & 1u) != 0u)
						{
							if (num5 == 1L)
							{
								array[1] += 1u;
								break;
							}
							checked
							{
								array[(int)((IntPtr)num5)] = array[(int)((IntPtr)(unchecked(num5 - 1L)))] << 1;
								break;
							}
						}
						else
						{
							array[(int)(checked((IntPtr)num5))] += 1u;
							num5 -= 1L;
						}
					}
					for (num5 = num3 + 1L; num5 < 33L; num5 += 1L)
					{
						checked
						{
							if (array[(int)((IntPtr)num5)] >> 1 != num4)
							{
								break;
							}
							num4 = array[(int)((IntPtr)num5)];
							array[(int)((IntPtr)num5)] = array[(int)((IntPtr)(unchecked(num5 - 1L)))] << 1;
						}
					}
				}
				else if (sparsecount == 0L)
				{
					num += 1L;
				}
			}
			if (sparsecount != 1L)
			{
				for (num2 = 1L; num2 < 33L; num2 += 1L)
				{
					if (((ulong)array[(int)(checked((IntPtr)num2))] & (ulong)-1 >> (int)(32L - num2)) != 0uL)
					{
						return null;
					}
				}
			}
			num2 = 0L;
			num = 0L;
			while (num2 < n)
			{
				uint num6 = 0u;
				for (long num5 = 0L; num5 < l[(int)(checked((IntPtr)num2))]; num5 += 1L)
				{
					num6 <<= 1;
					num6 |= (array2[(int)(checked((IntPtr)num))] >> (int)num5 & 1u);
				}
				if (sparsecount != 0L)
				{
					if (l[(int)(checked((IntPtr)num2))] != 0L)
					{
						uint[] arg_171_0 = array2;
						long expr_169 = num;
						num = expr_169 + 1L;
						arg_171_0[(int)(checked((IntPtr)expr_169))] = num6;
					}
				}
				else
				{
					uint[] arg_17F_0 = array2;
					long expr_177 = num;
					num = expr_177 + 1L;
					arg_17F_0[(int)(checked((IntPtr)expr_177))] = num6;
				}
				num2 += 1L;
			}
			return array2;
		}

		private static long _book_maptype1_quantvals(Xiph.static_codebook b)
		{
			long num = (long)Math.Floor(Math.Pow((double)((float)b.entries), (double)(1f / (float)b.dim)));
			while (true)
			{
				long num2 = 1L;
				long num3 = 1L;
				int num4 = 0;
				while ((long)num4 < b.dim)
				{
					num2 *= num;
					num3 *= num + 1L;
					num4++;
				}
				if (num2 <= b.entries && num3 > b.entries)
				{
					break;
				}
				if (num2 > b.entries)
				{
					num -= 1L;
				}
				else
				{
					num += 1L;
				}
			}
			return num;
		}

		private static float[] _book_unquantize(Xiph.static_codebook b, int n, int[] sparsemap)
		{
			long num = 0L;
			if (b.maptype == 1 || b.maptype == 2)
			{
				float num2 = Xiph._float32_unpack(b.q_min);
				float num3 = Xiph._float32_unpack(b.q_delta);
				float[] array = new float[(long)n * b.dim];
				switch (b.maptype)
				{
				case 1:
				{
					int num4 = (int)Xiph._book_maptype1_quantvals(b);
					for (long num5 = 0L; num5 < b.entries; num5 += 1L)
					{
						if ((sparsemap != null && b.lengthlist[(int)(checked((IntPtr)num5))] != 0L) || sparsemap == null)
						{
							float num6 = 0f;
							int num7 = 1;
							for (long num8 = 0L; num8 < b.dim; num8 += 1L)
							{
								int num9 = (int)(num5 / (long)num7 % (long)num4);
								float num10 = (float)b.quantlist[num9];
								num10 = Xiph.fabs(num10) * num3 + num2 + num6;
								if (b.q_sequencep != 0)
								{
									num6 = num10;
								}
								checked
								{
									if (sparsemap != null)
									{
										array[(int)((IntPtr)(unchecked((long)sparsemap[(int)(checked((IntPtr)num))] * b.dim + num8)))] = num10;
									}
									else
									{
										array[(int)((IntPtr)(unchecked(num * b.dim + num8)))] = num10;
									}
								}
								num7 *= num4;
							}
							num += 1L;
						}
					}
					break;
				}
				case 2:
				{
					long num5 = 0L;
					while (num5 < b.entries)
					{
						if (sparsemap != null)
						{
							long arg_137_0 = b.lengthlist[(int)(checked((IntPtr)num5))];
							goto IL_13D;
						}
						if (sparsemap == null)
						{
							goto IL_13D;
						}
						IL_1B8:
						num5 += 1L;
						continue;
						IL_13D:
						float num11 = 0f;
						for (long num8 = 0L; num8 < b.dim; num8 += 1L)
						{
							float num12 = (float)b.quantlist[(int)(checked((IntPtr)(unchecked(num5 * b.dim + num8))))];
							num12 = Xiph.fabs(num12) * num3 + num2 + num11;
							if (b.q_sequencep != 0)
							{
								num11 = num12;
							}
							checked
							{
								if (sparsemap != null)
								{
									array[(int)((IntPtr)(unchecked((long)sparsemap[(int)(checked((IntPtr)num))] * b.dim + num8)))] = num12;
								}
								else
								{
									array[(int)((IntPtr)(unchecked(num * b.dim + num8)))] = num12;
								}
							}
						}
						num += 1L;
						goto IL_1B8;
					}
					break;
				}
				}
				return array;
			}
			return null;
		}

		private static void vorbis_staticbook_destroy(Xiph.static_codebook b)
		{
			b.quantlist = null;
			b.lengthlist = null;
		}

		private static void vorbis_book_clear(Xiph.codebook b)
		{
		}

		private static int vorbis_book_init_encode(Xiph.codebook c, Xiph.static_codebook s)
		{
			c.c = s;
			c.entries = s.entries;
			c.used_entries = s.entries;
			c.dim = s.dim;
			c.codelist = Xiph._make_words(s.lengthlist, s.entries, 0L);
			c.quantvals = (int)Xiph._book_maptype1_quantvals(s);
			c.minval = Xiph.rint(Xiph._float32_unpack(s.q_min));
			c.delta = Xiph.rint(Xiph._float32_unpack(s.q_delta));
			return 0;
		}

		private static void codes_qsort(uint[] codes, int[] codep, int left, int right)
		{
			int i = left;
			int num = right;
			int num2 = codep[(left + right) / 2];
			while (i <= num)
			{
				while (codes[codep[i]] < codes[num2])
				{
					i++;
				}
				while (codes[codep[num]] > codes[num2])
				{
					num--;
				}
				if (i <= num)
				{
					int num3 = codep[i];
					codep[i] = codep[num];
					codep[num] = num3;
					i++;
					num--;
				}
			}
			if (left < num)
			{
				Xiph.codes_qsort(codes, codep, left, num);
			}
			if (i < right)
			{
				Xiph.codes_qsort(codes, codep, i, right);
			}
		}

		private static int vorbis_book_init_decode(Xiph.codebook c, Xiph.static_codebook s)
		{
			int num = 0;
			int i = 0;
			while ((long)i < s.entries)
			{
				if (s.lengthlist[i] > 0L)
				{
					num++;
				}
				i++;
			}
			c.entries = s.entries;
			c.used_entries = (long)num;
			c.dim = s.dim;
			if (num > 0)
			{
				uint[] array = Xiph._make_words(s.lengthlist, s.entries, c.used_entries);
				int[] array2 = new int[num];
				if (array == null)
				{
					Xiph.vorbis_book_clear(c);
					return -1;
				}
				for (i = 0; i < num; i++)
				{
					array[i] = Xiph.bitreverse(array[i]);
					array2[i] = i;
				}
				Xiph.codes_qsort(array, array2, 0, num - 1);
				int[] array3 = new int[num];
				c.codelist = new uint[num];
				for (i = 0; i < num; i++)
				{
					int num2 = array2[i];
					array3[num2] = i;
				}
				for (i = 0; i < num; i++)
				{
					c.codelist[array3[i]] = array[i];
				}
				c.valuelist = Xiph._book_unquantize(s, num, array3);
				c.dec_index = new int[num];
				num = 0;
				i = 0;
				while ((long)i < s.entries)
				{
					if (s.lengthlist[i] > 0L)
					{
						c.dec_index[array3[num++]] = i;
					}
					i++;
				}
				c.dec_codelengths = new byte[num];
				num = 0;
				i = 0;
				while ((long)i < s.entries)
				{
					if (s.lengthlist[i] > 0L)
					{
						c.dec_codelengths[array3[num++]] = (byte)s.lengthlist[i];
					}
					i++;
				}
				c.dec_firsttablen = Xiph._ilog(c.used_entries) - 4;
				if (c.dec_firsttablen < 5)
				{
					c.dec_firsttablen = 5;
				}
				if (c.dec_firsttablen > 8)
				{
					c.dec_firsttablen = 8;
				}
				int num3 = 1 << c.dec_firsttablen;
				c.dec_firsttable = new uint[num3];
				c.dec_maxlength = 0;
				for (i = 0; i < num; i++)
				{
					if (c.dec_maxlength < (int)c.dec_codelengths[i])
					{
						c.dec_maxlength = (int)c.dec_codelengths[i];
					}
					if ((int)c.dec_codelengths[i] <= c.dec_firsttablen)
					{
						uint num4 = Xiph.bitreverse(c.codelist[i]);
						for (int j = 0; j < 1 << c.dec_firsttablen - (int)c.dec_codelengths[i]; j++)
						{
							c.dec_firsttable[(int)(checked((IntPtr)(unchecked((ulong)num4 | (ulong)((long)((long)j << (int)c.dec_codelengths[i]))))))] = (uint)(i + 1);
						}
					}
				}
				uint num5 = (uint)((uint)((ulong)-2) << 31 - c.dec_firsttablen);
				long num6 = 0L;
				long num7 = 0L;
				for (i = 0; i < num3; i++)
				{
					uint num8 = (uint)((uint)i << 32 - c.dec_firsttablen);
					if (c.dec_firsttable[(int)((UIntPtr)Xiph.bitreverse(num8))] == 0u)
					{
						while (num6 + 1L < (long)num)
						{
							if (c.codelist[(int)(checked((IntPtr)(unchecked(num6 + 1L))))] > num8)
							{
								break;
							}
							num6 += 1L;
						}
						while (num7 < (long)num && num8 >= (c.codelist[(int)(checked((IntPtr)num7))] & num5))
						{
							num7 += 1L;
						}
						ulong num9 = (ulong)num6;
						ulong num10 = (ulong)((long)num - num7);
						if (num9 > 32767uL)
						{
							num9 = 32767uL;
						}
						if (num10 > 32767uL)
						{
							num10 = 32767uL;
						}
						c.dec_firsttable[(int)((UIntPtr)Xiph.bitreverse(num8))] = (uint)((ulong)-2147483648 | num9 << 15 | num10);
					}
				}
			}
			return 0;
		}

		private static void drfti1(int n, float[] wa, int wao, int[] ifac)
		{
			int num = 0;
			int i = -1;
			int num2 = n;
			int num3 = 0;
			while (true)
			{
				i++;
				if (i < 4)
				{
					num = Xiph.ntryh[i];
				}
				else
				{
					num += 2;
				}
				while (true)
				{
					int num4 = num2 / num;
					if (num2 - num * num4 != 0)
					{
						break;
					}
					num3++;
					ifac[num3 + 1] = num;
					num2 = num4;
					if (num == 2 && num3 != 1)
					{
						for (int j = 1; j < num3; j++)
						{
							int num5 = num3 - j + 1;
							ifac[num5 + 1] = ifac[num5];
						}
						ifac[2] = 2;
					}
					if (num2 == 1)
					{
						goto Block_6;
					}
				}
			}
			Block_6:
			ifac[0] = n;
			ifac[1] = num3;
			float num6 = 6.28318548f / (float)n;
			int num7 = 0;
			int num8 = num3 - 1;
			int num9 = 1;
			if (num8 == 0)
			{
				return;
			}
			for (int k = 0; k < num8; k++)
			{
				int num10 = ifac[k + 2];
				int num11 = 0;
				int num12 = num9 * num10;
				int num13 = n / num12;
				int num14 = num10 - 1;
				for (i = 0; i < num14; i++)
				{
					num11 += num9;
					int j = num7;
					float num15 = (float)num11 * num6;
					float num16 = 0f;
					for (int l = 2; l < num13; l += 2)
					{
						num16 += 1f;
						float num17 = num16 * num15;
						wa[wao + wao + j++] = (float)Math.Cos((double)num17);
						wa[wao + wao + j++] = (float)Math.Sin((double)num17);
					}
					num7 += num13;
				}
				num9 = num12;
			}
		}

		private static void fdrffti(int n, float[] wsave, int[] ifac)
		{
			if (n == 1)
			{
				return;
			}
			Xiph.drfti1(n, wsave, n, ifac);
		}

		private static void dradf2(int ido, int l1, float[] cc, int cco, float[] ch, int cho, float[] wa1, int wa1o)
		{
			int num = 0;
			int num3;
			int num2 = num3 = l1 * ido;
			int num4 = ido << 1;
			for (int i = 0; i < l1; i++)
			{
				ch[cho + num << 1] = cc[cco + num] + cc[cco + num2];
				ch[cho + (num << 1) + num4 - 1] = cc[cco + num] - cc[cco + num2];
				num += ido;
				num2 += ido;
			}
			if (ido < 2)
			{
				return;
			}
			if (ido != 2)
			{
				num = 0;
				num2 = num3;
				for (int i = 0; i < l1; i++)
				{
					num4 = num2;
					int num5 = (num << 1) + (ido << 1);
					int num6 = num;
					int num7 = num + num;
					for (int j = 2; j < ido; j += 2)
					{
						num4 += 2;
						num5 -= 2;
						num6 += 2;
						num7 += 2;
						float num8 = wa1[wa1o + j - 2] * cc[cco + num4 - 1] + wa1[wa1o + j - 1] * cc[cco + num4];
						float num9 = wa1[wa1o + j - 2] * cc[cco + num4] - wa1[wa1o + j - 1] * cc[cco + num4 - 1];
						ch[cho + num7] = cc[cco + num6] + num9;
						ch[cho + num5] = num9 - cc[cco + num6];
						ch[cho + num7 - 1] = cc[cco + num6 - 1] + num8;
						ch[cho + num5 - 1] = cc[cco + num6 - 1] - num8;
					}
					num += ido;
					num2 += ido;
				}
				if (ido % 2 == 1)
				{
					return;
				}
			}
			num = ido;
			num2 = (num4 = ido - 1) + num3;
			for (int i = 0; i < l1; i++)
			{
				ch[cho + num] = -cc[cco + num2];
				ch[cho + num - 1] = cc[cco + num4];
				num += ido << 1;
				num2 += ido;
				num4 += ido;
			}
		}

		private static void dradf4(int ido, int l1, float[] cc, int cco, float[] ch, int cho, float[] wa1, int wa1o, float[] wa2, int wa2o, float[] wa3, int wa3o)
		{
			int num = l1 * ido;
			int num2 = num;
			int num3 = num2 << 1;
			int num4 = num2 + (num2 << 1);
			int num5 = 0;
			int num8;
			for (int i = 0; i < l1; i++)
			{
				float num6 = cc[cco + num2] + cc[cco + num4];
				float num7 = cc[cco + num5] + cc[cco + num3];
				ch[cho + (num8 = num5 << 2)] = num6 + num7;
				ch[cho + (ido << 2) + num8 - 1] = num7 - num6;
				ch[cho + (num8 += ido << 1) - 1] = cc[cco + num5] - cc[cco + num3];
				ch[cho + num8] = cc[cco + num4] - cc[cco + num2];
				num2 += ido;
				num4 += ido;
				num5 += ido;
				num3 += ido;
			}
			if (ido < 2)
			{
				return;
			}
			int num9;
			if (ido != 2)
			{
				num2 = 0;
				for (int i = 0; i < l1; i++)
				{
					num4 = num2;
					num3 = num2 << 2;
					num8 = (num9 = ido << 1) + num3;
					for (int j = 2; j < ido; j += 2)
					{
						num4 = (num5 = num4 + 2);
						num3 += 2;
						num8 -= 2;
						num5 += num;
						float num10 = wa1[wa1o + j - 2] * cc[cco + num5 - 1] + wa1[wa1o + j - 1] * cc[cco + num5];
						float num11 = wa1[wa1o + j - 2] * cc[cco + num5] - wa1[wa1o + j - 1] * cc[cco + num5 - 1];
						num5 += num;
						float num12 = wa2[wa2o + j - 2] * cc[cco + num5 - 1] + wa2[wa2o + j - 1] * cc[cco + num5];
						float num13 = wa2[wa2o + j - 2] * cc[cco + num5] - wa2[wa2o + j - 1] * cc[cco + num5 - 1];
						num5 += num;
						float num14 = wa3[wa3o + j - 2] * cc[cco + num5 - 1] + wa3[wa3o + j - 1] * cc[cco + num5];
						float num15 = wa3[wa3o + j - 2] * cc[cco + num5] - wa3[wa3o + j - 1] * cc[cco + num5 - 1];
						float num6 = num10 + num14;
						float num16 = num14 - num10;
						float num17 = num11 + num15;
						float num18 = num11 - num15;
						float num19 = cc[cco + num4] + num13;
						float num20 = cc[cco + num4] - num13;
						float num7 = cc[cco + num4 - 1] + num12;
						float num21 = cc[cco + num4 - 1] - num12;
						ch[cho + num3 - 1] = num6 + num7;
						ch[cho + num3] = num17 + num19;
						ch[cho + num8 - 1] = num21 - num18;
						ch[cho + num8] = num16 - num20;
						ch[cho + num3 + num9 - 1] = num18 + num21;
						ch[cho + num3 + num9] = num16 + num20;
						ch[cho + num8 + num9 - 1] = num7 - num6;
						ch[cho + num8 + num9] = num17 - num19;
					}
					num2 += ido;
				}
				if ((ido & 1) != 0)
				{
					return;
				}
			}
			num4 = (num2 = num + ido - 1) + (num << 1);
			num5 = ido << 2;
			num3 = ido;
			num8 = ido << 1;
			num9 = ido;
			for (int i = 0; i < l1; i++)
			{
				float num17 = -0.707106769f * (cc[cco + num2] + cc[cco + num4]);
				float num6 = 0.707106769f * (cc[cco + num2] - cc[cco + num4]);
				ch[cho + num3 - 1] = num6 + cc[cco + num9 - 1];
				ch[cho + num3 + num8 - 1] = cc[cco + num9 - 1] - num6;
				ch[cho + num3] = num17 - cc[cco + num2 + num];
				ch[cho + num3 + num8] = num17 + cc[cco + num2 + num];
				num2 += ido;
				num4 += ido;
				num3 += num5;
				num9 += ido;
			}
		}

		private static void dradfg(int ido, int ip, int l1, int idl1, float[] cc, int cco, float[] c1, int c1o, float[] c2, int c2o, float[] ch, int cho, float[] ch2, int ch2o, float[] wa, int wao)
		{
			float num = 6.28318548f / (float)ip;
			float num2 = (float)Math.Cos((double)num);
			float num3 = (float)Math.Sin((double)num);
			int num4 = ip + 1 >> 1;
			int num5 = ido - 1 >> 1;
			int num6 = l1 * ido;
			int num7 = ip * ido;
			int num8;
			int num9;
			int num12;
			int num13;
			int num14;
			if (ido != 1)
			{
				for (int i = 0; i < idl1; i++)
				{
					ch2[i] = c2[i];
				}
				num8 = 0;
				for (int j = 1; j < ip; j++)
				{
					num8 += num6;
					num9 = num8;
					for (int k = 0; k < l1; k++)
					{
						ch[cho + num9] = c1[num9];
						num9 += ido;
					}
				}
				int num10 = -ido;
				num8 = 0;
				if (num5 > l1)
				{
					for (int j = 1; j < ip; j++)
					{
						num8 += num6;
						num10 += ido;
						num9 = -ido + num8;
						for (int k = 0; k < l1; k++)
						{
							int num11 = num10 - 1;
							num9 += ido;
							num12 = num9;
							for (int m = 2; m < ido; m += 2)
							{
								num11 += 2;
								num12 += 2;
								ch[cho + num12 - 1] = wa[wao + num11 - 1] * c1[num12 - 1] + wa[wao + num11] * c1[num12];
								ch[cho + num12] = wa[wao + num11 - 1] * c1[num12] - wa[wao + num11] * c1[num12 - 1];
							}
						}
					}
				}
				else
				{
					for (int j = 1; j < ip; j++)
					{
						num10 += ido;
						int num11 = num10 - 1;
						num8 += num6;
						num9 = num8;
						for (int m = 2; m < ido; m += 2)
						{
							num11 += 2;
							num9 += 2;
							num12 = num9;
							for (int k = 0; k < l1; k++)
							{
								ch[cho + num12 - 1] = wa[wao + num11 - 1] * c1[num12 - 1] + wa[wao + num11] * c1[num12];
								ch[cho + num12] = wa[wao + num11 - 1] * c1[num12] - wa[wao + num11] * c1[num12 - 1];
								num12 += ido;
							}
						}
					}
				}
				num8 = 0;
				num9 = ip * num6;
				if (num5 < l1)
				{
					for (int j = 1; j < num4; j++)
					{
						num8 += num6;
						num9 -= num6;
						num12 = num8;
						num13 = num9;
						for (int m = 2; m < ido; m += 2)
						{
							num12 += 2;
							num13 += 2;
							num14 = num12 - ido;
							int num15 = num13 - ido;
							for (int k = 0; k < l1; k++)
							{
								num14 += ido;
								num15 += ido;
								c1[num14 - 1] = ch[cho + num14 - 1] + ch[cho + num15 - 1];
								c1[num15 - 1] = ch[cho + num14] - ch[cho + num15];
								c1[num14] = ch[cho + num14] + ch[cho + num15];
								c1[num15] = ch[cho + num15 - 1] - ch[cho + num14 - 1];
							}
						}
					}
				}
				else
				{
					for (int j = 1; j < num4; j++)
					{
						num8 += num6;
						num9 -= num6;
						num12 = num8;
						num13 = num9;
						for (int k = 0; k < l1; k++)
						{
							num14 = num12;
							int num15 = num13;
							for (int m = 2; m < ido; m += 2)
							{
								num14 += 2;
								num15 += 2;
								c1[num14 - 1] = ch[cho + num14 - 1] + ch[cho + num15 - 1];
								c1[num15 - 1] = ch[cho + num14] - ch[cho + num15];
								c1[num14] = ch[cho + num14] + ch[cho + num15];
								c1[num15] = ch[cho + num15 - 1] - ch[cho + num14 - 1];
							}
							num12 += ido;
							num13 += ido;
						}
					}
				}
			}
			for (int i = 0; i < idl1; i++)
			{
				c2[i] = ch2[i];
			}
			num8 = 0;
			num9 = ip * idl1;
			for (int j = 1; j < num4; j++)
			{
				num8 += num6;
				num9 -= num6;
				num12 = num8 - ido;
				num13 = num9 - ido;
				for (int k = 0; k < l1; k++)
				{
					num12 += ido;
					num13 += ido;
					c1[num12] = ch[cho + num12] + ch[cho + num13];
					c1[num13] = ch[cho + num13] - ch[cho + num12];
				}
			}
			float num16 = 1f;
			float num17 = 0f;
			num8 = 0;
			num9 = ip * idl1;
			num12 = (ip - 1) * idl1;
			for (int n = 1; n < num4; n++)
			{
				num8 += idl1;
				num9 -= idl1;
				float num18 = num2 * num16 - num3 * num17;
				num17 = num2 * num17 + num3 * num16;
				num16 = num18;
				num13 = num8;
				num14 = num9;
				int num15 = num12;
				int num19 = idl1;
				for (int i = 0; i < idl1; i++)
				{
					ch2[num13++] = c2[i] + num16 * c2[num19++];
					ch2[num14++] = num17 * c2[num15++];
				}
				float num20 = num16;
				float num21 = num17;
				float num22 = num16;
				float num23 = num17;
				num13 = idl1;
				num14 = (ip - 1) * idl1;
				for (int j = 2; j < num4; j++)
				{
					num13 += idl1;
					num14 -= idl1;
					float num24 = num20 * num22 - num21 * num23;
					num23 = num20 * num23 + num21 * num22;
					num22 = num24;
					num15 = num8;
					num19 = num9;
					int num25 = num13;
					int num26 = num14;
					for (int i = 0; i < idl1; i++)
					{
						ch2[num15++] += num22 * c2[num25++];
						ch2[num19++] += num23 * c2[num26++];
					}
				}
			}
			num8 = 0;
			for (int j = 1; j < num4; j++)
			{
				num8 += idl1;
				num9 = num8;
				for (int i = 0; i < idl1; i++)
				{
					ch2[i] += c2[num9++];
				}
			}
			if (ido >= l1)
			{
				num8 = 0;
				num9 = 0;
				for (int k = 0; k < l1; k++)
				{
					num12 = num8;
					num13 = num9;
					for (int m = 0; m < ido; m++)
					{
						cc[cco + num13++] = ch[cho + num12++];
					}
					num8 += ido;
					num9 += num7;
				}
			}
			else
			{
				for (int m = 0; m < ido; m++)
				{
					num8 = m;
					num9 = m;
					for (int k = 0; k < l1; k++)
					{
						cc[cco + num9] = ch[cho + num8];
						num8 += ido;
						num9 += num7;
					}
				}
			}
			num8 = 0;
			num9 = ido << 1;
			num12 = 0;
			num13 = ip * num6;
			for (int j = 1; j < num4; j++)
			{
				num8 += num9;
				num12 += num6;
				num13 -= num6;
				num14 = num8;
				int num15 = num12;
				int num19 = num13;
				for (int k = 0; k < l1; k++)
				{
					cc[cco + num14 - 1] = ch[cho + num15];
					cc[cco + num14] = ch[cho + num19];
					num14 += num7;
					num15 += ido;
					num19 += ido;
				}
			}
			if (ido == 1)
			{
				return;
			}
			if (num5 >= l1)
			{
				num8 = -ido;
				num12 = 0;
				num13 = 0;
				num14 = ip * num6;
				for (int j = 1; j < num4; j++)
				{
					num8 += num9;
					num12 += num9;
					num13 += num6;
					num14 -= num6;
					int num15 = num8;
					int num19 = num12;
					int num25 = num13;
					int num26 = num14;
					for (int k = 0; k < l1; k++)
					{
						for (int m = 2; m < ido; m += 2)
						{
							int num27 = ido - m;
							cc[cco + m + num19 - 1] = ch[cho + m + num25 - 1] + ch[cho + m + num26 - 1];
							cc[cco + num27 + num15 - 1] = ch[cho + m + num25 - 1] - ch[cho + m + num26 - 1];
							cc[cco + m + num19] = ch[cho + m + num25] + ch[cho + m + num26];
							cc[cco + num27 + num15] = ch[cho + m + num26] - ch[cho + m + num25];
						}
						num15 += num7;
						num19 += num7;
						num25 += ido;
						num26 += ido;
					}
				}
				return;
			}
			num8 = -ido;
			num12 = 0;
			num13 = 0;
			num14 = ip * num6;
			for (int j = 1; j < num4; j++)
			{
				num8 += num9;
				num12 += num9;
				num13 += num6;
				num14 -= num6;
				for (int m = 2; m < ido; m += 2)
				{
					int num15 = ido + num8 - m;
					int num19 = m + num12;
					int num25 = m + num13;
					int num26 = m + num14;
					for (int k = 0; k < l1; k++)
					{
						cc[cco + num19 - 1] = ch[cho + num25 - 1] + ch[cho + num26 - 1];
						cc[cco + num15 - 1] = ch[cho + num25 - 1] - ch[cho + num26 - 1];
						cc[cco + num19] = ch[cho + num25] + ch[cho + num26];
						cc[cco + num15] = ch[cho + num26] - ch[cho + num25];
						num15 += num7;
						num19 += num7;
						num25 += ido;
						num26 += ido;
					}
				}
			}
		}

		private static void drftf1(int n, float[] c, int co, float[] ch, int cho, float[] wa, int wao, int[] ifac)
		{
			int num = ifac[1];
			int num2 = 1;
			int num3 = n;
			int num4 = n;
			for (int i = 0; i < num; i++)
			{
				int num5 = num - i;
				int num6 = ifac[num5 + 1];
				int num7 = num3 / num6;
				int num8 = n / num3;
				int idl = num8 * num7;
				num4 -= (num6 - 1) * num8;
				num2 = 1 - num2;
				if (num6 == 4)
				{
					int num9 = num4 + num8;
					int num10 = num9 + num8;
					if (num2 != 0)
					{
						Xiph.dradf4(num8, num7, ch, cho, c, co, wa, num4 - 1, wa, num9 - 1, wa, num10 - 1);
					}
					else
					{
						Xiph.dradf4(num8, num7, c, co, ch, cho, wa, num4 - 1, wa, num9 - 1, wa, num10 - 1);
					}
				}
				else if (num6 == 2)
				{
					if (num2 == 0)
					{
						Xiph.dradf2(num8, num7, c, co, ch, cho, wa, num4 - 1);
					}
					else
					{
						Xiph.dradf2(num8, num7, ch, cho, c, co, wa, num4 - 1);
					}
				}
				else
				{
					if (num8 == 1)
					{
						num2 = 1 - num2;
					}
					if (num2 == 0)
					{
						Xiph.dradfg(num8, num6, num7, idl, c, co, c, co, c, co, ch, cho, ch, cho, wa, num4 - 1);
						num2 = 1;
					}
					else
					{
						Xiph.dradfg(num8, num6, num7, idl, ch, cho, ch, cho, ch, cho, c, co, c, co, wa, num4 - 1);
						num2 = 0;
					}
				}
				num3 = num7;
			}
			if (num2 == 1)
			{
				return;
			}
			for (int j = 0; j < n; j++)
			{
				c[co + j] = ch[cho + j];
			}
		}

		private static void dradb2(int ido, int l1, float[] cc, int cco, float[] ch, int cho, float[] wa1, int wa1o)
		{
			int num = l1 * ido;
			int num2 = 0;
			int num3 = 0;
			int num4 = (ido << 1) - 1;
			for (int i = 0; i < l1; i++)
			{
				ch[cho + num2] = cc[cco + num3] + cc[cco + num4 + num3];
				ch[cho + num2 + num] = cc[cco + num3] - cc[cco + num4 + num3];
				num3 = (num2 += ido) << 1;
			}
			if (ido < 2)
			{
				return;
			}
			if (ido != 2)
			{
				num2 = 0;
				num3 = 0;
				for (int i = 0; i < l1; i++)
				{
					num4 = num2;
					int num6;
					int num5 = (num6 = num3) + (ido << 1);
					int num7 = num + num2;
					for (int j = 2; j < ido; j += 2)
					{
						num4 += 2;
						num6 += 2;
						num5 -= 2;
						num7 += 2;
						ch[cho + num4 - 1] = cc[cco + num6 - 1] + cc[cco + num5 - 1];
						float num8 = cc[cco + num6 - 1] - cc[cco + num5 - 1];
						ch[cho + num4] = cc[cco + num6] - cc[cco + num5];
						float num9 = cc[cco + num6] + cc[cco + num5];
						ch[cho + num7 - 1] = wa1[wa1o + j - 2] * num8 - wa1[wa1o + j - 1] * num9;
						ch[cho + num7] = wa1[wa1o + j - 2] * num9 + wa1[wa1o + j - 1] * num8;
					}
					num3 = (num2 += ido) << 1;
				}
				if (ido % 2 == 1)
				{
					return;
				}
			}
			num2 = ido - 1;
			num3 = ido - 1;
			for (int i = 0; i < l1; i++)
			{
				ch[cho + num2] = cc[cco + num3] + cc[cco + num3];
				ch[cho + num2 + num] = -(cc[cco + num3 + 1] + cc[cco + num3 + 1]);
				num2 += ido;
				num3 += ido << 1;
			}
		}

		private static void dradb3(int ido, int l1, float[] cc, int cco, float[] ch, int cho, float[] wa1, int wa1o, float[] wa2, int wa2o)
		{
			int num = l1 * ido;
			int num2 = 0;
			int num3 = num << 1;
			int num4 = ido << 1;
			int num5 = ido + (ido << 1);
			int num6 = 0;
			for (int i = 0; i < l1; i++)
			{
				float num7 = cc[cco + num4 - 1] + cc[cco + num4 - 1];
				float num8 = cc[cco + num6] + -0.5f * num7;
				ch[cho + num2] = cc[cco + num6] + num7;
				float num9 = 0.8660254f * (cc[cco + num4] + cc[cco + num4]);
				ch[cho + num2 + num] = num8 - num9;
				ch[cho + num2 + num3] = num8 + num9;
				num2 += ido;
				num4 += num5;
				num6 += num5;
			}
			if (ido == 1)
			{
				return;
			}
			num2 = 0;
			num4 = ido << 1;
			for (int i = 0; i < l1; i++)
			{
				int num10 = num2 + (num2 << 1);
				int num11;
				num6 = (num11 = num10 + num4);
				int num12 = num2;
				int num14;
				int num13 = (num14 = num2 + num) + num;
				for (int j = 2; j < ido; j += 2)
				{
					num6 += 2;
					num11 -= 2;
					num10 += 2;
					num12 += 2;
					num14 += 2;
					num13 += 2;
					float num7 = cc[cco + num6 - 1] + cc[cco + num11 - 1];
					float num8 = cc[cco + num10 - 1] + -0.5f * num7;
					ch[cho + num12 - 1] = cc[cco + num10 - 1] + num7;
					float num15 = cc[cco + num6] - cc[cco + num11];
					float num16 = cc[cco + num10] + -0.5f * num15;
					ch[cho + num12] = cc[cco + num10] + num15;
					float num17 = 0.8660254f * (cc[cco + num6 - 1] - cc[cco + num11 - 1]);
					float num9 = 0.8660254f * (cc[cco + num6] + cc[cco + num11]);
					float num18 = num8 - num9;
					float num19 = num8 + num9;
					float num20 = num16 + num17;
					float num21 = num16 - num17;
					ch[cho + num14 - 1] = wa1[wa1o + j - 2] * num18 - wa1[wa1o + j - 1] * num20;
					ch[cho + num14] = wa1[wa1o + j - 2] * num20 + wa1[wa1o + j - 1] * num18;
					ch[cho + num13 - 1] = wa2[wa2o + j - 2] * num19 - wa2[wa2o + j - 1] * num21;
					ch[cho + num13] = wa2[wa2o + j - 2] * num21 + wa2[wa2o + j - 1] * num19;
				}
				num2 += ido;
			}
		}

		private static void dradb4(int ido, int l1, float[] cc, int cco, float[] ch, int cho, float[] wa1, int wa1o, float[] wa2, int wa2o, float[] wa3, int wa3o)
		{
			int num = l1 * ido;
			int num2 = 0;
			int num3 = ido << 2;
			int num4 = 0;
			int num5 = ido << 1;
			int num6;
			for (int i = 0; i < l1; i++)
			{
				num6 = num4 + num5;
				int num7 = num2;
				float num8 = cc[cco + num6 - 1] + cc[cco + num6 - 1];
				float num9 = cc[cco + num6] + cc[cco + num6];
				float num10 = cc[cco + num4] - cc[cco + (num6 += num5) - 1];
				float num11 = cc[cco + num4] + cc[cco + num6 - 1];
				ch[cho + num7] = num11 + num8;
				ch[cho + (num7 += num)] = num10 - num9;
				ch[cho + (num7 += num)] = num11 - num8;
				ch[cho + (num7 + num)] = num10 + num9;
				num2 += ido;
				num4 += num3;
			}
			if (ido < 2)
			{
				return;
			}
			if (ido != 2)
			{
				num2 = 0;
				for (int i = 0; i < l1; i++)
				{
					int num7 = (num6 = (num4 = (num3 = num2 << 2) + num5)) + num5;
					int num12 = num2;
					for (int j = 2; j < ido; j += 2)
					{
						num3 += 2;
						num4 += 2;
						num6 -= 2;
						num7 -= 2;
						num12 += 2;
						float num13 = cc[cco + num3] + cc[cco + num7];
						float num14 = cc[cco + num3] - cc[cco + num7];
						float num15 = cc[cco + num4] - cc[cco + num6];
						float num9 = cc[cco + num4] + cc[cco + num6];
						float num10 = cc[cco + num3 - 1] - cc[cco + num7 - 1];
						float num11 = cc[cco + num3 - 1] + cc[cco + num7 - 1];
						float num16 = cc[cco + num4 - 1] - cc[cco + num6 - 1];
						float num8 = cc[cco + num4 - 1] + cc[cco + num6 - 1];
						ch[cho + num12 - 1] = num11 + num8;
						float num17 = num11 - num8;
						ch[cho + num12] = num14 + num15;
						float num18 = num14 - num15;
						float num19 = num10 - num9;
						float num20 = num10 + num9;
						float num21 = num13 + num16;
						float num22 = num13 - num16;
						int num23;
						ch[cho + (num23 = num12 + num) - 1] = wa1[wa1o + j - 2] * num19 - wa1[wa1o + j - 1] * num21;
						ch[cho + num23] = wa1[wa1o + j - 2] * num21 + wa1[wa1o + j - 1] * num19;
						ch[cho + (num23 += num) - 1] = wa2[wa2o + j - 2] * num17 - wa2[wa2o + j - 1] * num18;
						ch[cho + num23] = wa2[wa2o + j - 2] * num18 + wa2[wa2o + j - 1] * num17;
						ch[cho + (num23 += num) - 1] = wa3[wa3o + j - 2] * num20 - wa3[wa3o + j - 1] * num22;
						ch[cho + num23] = wa3[wa3o + j - 2] * num22 + wa3[wa3o + j - 1] * num20;
					}
					num2 += ido;
				}
				if (ido % 2 == 1)
				{
					return;
				}
			}
			num2 = ido;
			num3 = ido << 2;
			num4 = ido - 1;
			num6 = ido + (ido << 1);
			for (int i = 0; i < l1; i++)
			{
				int num7 = num4;
				float num13 = cc[cco + num2] + cc[cco + num6];
				float num14 = cc[cco + num6] - cc[cco + num2];
				float num10 = cc[cco + num2 - 1] - cc[cco + num6 - 1];
				float num11 = cc[cco + num2 - 1] + cc[cco + num6 - 1];
				ch[cho + num7] = num11 + num11;
				ch[cho + (num7 += num)] = 1.41421354f * (num10 - num13);
				ch[cho + (num7 += num)] = num14 + num14;
				ch[cho + (num7 + num)] = -1.41421354f * (num10 + num13);
				num4 += ido;
				num2 += num3;
				num6 += num3;
			}
		}

		private static void dradbg(int ido, int ip, int l1, int idl1, float[] cc, int cco, float[] c1, int c1o, float[] c2, int c2o, float[] ch, int cho, float[] ch2, int ch2o, float[] wa, int wao)
		{
			int num = ip * ido;
			int num2 = l1 * ido;
			float num3 = 6.28318548f / (float)ip;
			float num4 = (float)Math.Cos((double)num3);
			float num5 = (float)Math.Sin((double)num3);
			int num6 = ido - 1 >> 1;
			int num7 = ip + 1 >> 1;
			int num8;
			int num9;
			int num10;
			if (ido >= l1)
			{
				num8 = 0;
				num9 = 0;
				for (int i = 0; i < l1; i++)
				{
					num10 = num8;
					int num11 = num9;
					for (int j = 0; j < ido; j++)
					{
						ch[cho + num10] = cc[cco + num11];
						num10++;
						num11++;
					}
					num8 += ido;
					num9 += num;
				}
			}
			else
			{
				num8 = 0;
				for (int j = 0; j < ido; j++)
				{
					num9 = num8;
					num10 = num8;
					for (int i = 0; i < l1; i++)
					{
						ch[cho + num9] = cc[cco + num10];
						num9 += ido;
						num10 += num;
					}
					num8++;
				}
			}
			num8 = 0;
			num9 = ip * num2;
			int num13;
			int num12 = num13 = ido << 1;
			for (int k = 1; k < num7; k++)
			{
				num8 += num2;
				num9 -= num2;
				num10 = num8;
				int num11 = num9;
				int num14 = num12;
				for (int i = 0; i < l1; i++)
				{
					ch[cho + num10] = cc[cco + num14 - 1] + cc[cco + num14 - 1];
					ch[cho + num11] = cc[cco + num14] + cc[cco + num14];
					num10 += ido;
					num11 += ido;
					num14 += num;
				}
				num12 += num13;
			}
			int num16;
			if (ido != 1)
			{
				if (num6 >= l1)
				{
					num8 = 0;
					num9 = ip * num2;
					num13 = 0;
					for (int k = 1; k < num7; k++)
					{
						num8 += num2;
						num9 -= num2;
						num10 = num8;
						int num11 = num9;
						num13 += ido << 1;
						int num15 = num13;
						for (int i = 0; i < l1; i++)
						{
							num12 = num10;
							int num14 = num11;
							num16 = num15;
							int num17 = num15;
							for (int j = 2; j < ido; j += 2)
							{
								num12 += 2;
								num14 += 2;
								num16 += 2;
								num17 -= 2;
								ch[cho + num12 - 1] = cc[cco + num16 - 1] + cc[cco + num17 - 1];
								ch[cho + num14 - 1] = cc[cco + num16 - 1] - cc[cco + num17 - 1];
								ch[cho + num12] = cc[cco + num16] - cc[cco + num17];
								ch[cho + num14] = cc[cco + num16] + cc[cco + num17];
							}
							num10 += ido;
							num11 += ido;
							num15 += num;
						}
					}
				}
				else
				{
					num8 = 0;
					num9 = ip * num2;
					num13 = 0;
					for (int k = 1; k < num7; k++)
					{
						num8 += num2;
						num9 -= num2;
						num10 = num8;
						int num11 = num9;
						num13 += ido << 1;
						int num15 = num13;
						num16 = num13;
						for (int j = 2; j < ido; j += 2)
						{
							num10 += 2;
							num11 += 2;
							num15 += 2;
							num16 -= 2;
							num12 = num10;
							int num14 = num11;
							int num17 = num15;
							int num18 = num16;
							for (int i = 0; i < l1; i++)
							{
								ch[cho + num12 - 1] = cc[cco + num17 - 1] + cc[cco + num18 - 1];
								ch[cho + num14 - 1] = cc[cco + num17 - 1] - cc[cco + num18 - 1];
								ch[cho + num12] = cc[cco + num17] - cc[cco + num18];
								ch[cho + num14] = cc[cco + num17] + cc[cco + num18];
								num12 += ido;
								num14 += ido;
								num17 += num;
								num18 += num;
							}
						}
					}
				}
			}
			float num19 = 1f;
			float num20 = 0f;
			num8 = 0;
			num9 = (num16 = ip * idl1);
			num10 = (ip - 1) * idl1;
			for (int m = 1; m < num7; m++)
			{
				num8 += idl1;
				num9 -= idl1;
				float num21 = num4 * num19 - num5 * num20;
				num20 = num4 * num20 + num5 * num19;
				num19 = num21;
				int num11 = num8;
				num12 = num9;
				int num14 = 0;
				num13 = idl1;
				int num15 = num10;
				for (int n = 0; n < idl1; n++)
				{
					c2[num11++] = ch2[num14++] + num19 * ch2[num13++];
					c2[num12++] = num20 * ch2[num15++];
				}
				float num22 = num19;
				float num23 = num20;
				float num24 = num19;
				float num25 = num20;
				num14 = idl1;
				num13 = num16 - idl1;
				for (int k = 2; k < num7; k++)
				{
					num14 += idl1;
					num13 -= idl1;
					float num26 = num22 * num24 - num23 * num25;
					num25 = num22 * num25 + num23 * num24;
					num24 = num26;
					num11 = num8;
					num12 = num9;
					int num17 = num14;
					int num18 = num13;
					for (int n = 0; n < idl1; n++)
					{
						c2[num11++] += num24 * ch2[num17++];
						c2[num12++] += num25 * ch2[num18++];
					}
				}
			}
			num8 = 0;
			for (int k = 1; k < num7; k++)
			{
				num8 += idl1;
				num9 = num8;
				for (int n = 0; n < idl1; n++)
				{
					ch2[n] += ch2[num9++];
				}
			}
			num8 = 0;
			num9 = ip * num2;
			for (int k = 1; k < num7; k++)
			{
				num8 += num2;
				num9 -= num2;
				num10 = num8;
				int num11 = num9;
				for (int i = 0; i < l1; i++)
				{
					ch[cho + num10] = c1[num10] - c1[num11];
					ch[cho + num11] = c1[num10] + c1[num11];
					num10 += ido;
					num11 += ido;
				}
			}
			if (ido != 1)
			{
				if (num6 >= l1)
				{
					num8 = 0;
					num9 = ip * num2;
					for (int k = 1; k < num7; k++)
					{
						num8 += num2;
						num9 -= num2;
						num10 = num8;
						int num11 = num9;
						for (int i = 0; i < l1; i++)
						{
							num12 = num10;
							int num14 = num11;
							for (int j = 2; j < ido; j += 2)
							{
								num12 += 2;
								num14 += 2;
								ch[cho + num12 - 1] = c1[num12 - 1] - c1[num14];
								ch[cho + num14 - 1] = c1[num12 - 1] + c1[num14];
								ch[cho + num12] = c1[num12] + c1[num14 - 1];
								ch[cho + num14] = c1[num12] - c1[num14 - 1];
							}
							num10 += ido;
							num11 += ido;
						}
					}
				}
				else
				{
					num8 = 0;
					num9 = ip * num2;
					for (int k = 1; k < num7; k++)
					{
						num8 += num2;
						num9 -= num2;
						num10 = num8;
						int num11 = num9;
						for (int j = 2; j < ido; j += 2)
						{
							num10 += 2;
							num11 += 2;
							num12 = num10;
							int num14 = num11;
							for (int i = 0; i < l1; i++)
							{
								ch[cho + num12 - 1] = c1[num12 - 1] - c1[num14];
								ch[cho + num14 - 1] = c1[num12 - 1] + c1[num14];
								ch[cho + num12] = c1[num12] + c1[num14 - 1];
								ch[cho + num14] = c1[num12] - c1[num14 - 1];
								num12 += ido;
								num14 += ido;
							}
						}
					}
				}
			}
			if (ido == 1)
			{
				return;
			}
			for (int n = 0; n < idl1; n++)
			{
				c2[n] = ch2[n];
			}
			num8 = 0;
			for (int k = 1; k < ip; k++)
			{
				num8 = (num9 = num8 + num2);
				for (int i = 0; i < l1; i++)
				{
					c1[num9] = ch[cho + num9];
					num9 += ido;
				}
			}
			int num27;
			if (num6 <= l1)
			{
				num27 = -ido - 1;
				num8 = 0;
				for (int k = 1; k < ip; k++)
				{
					num27 += ido;
					num8 += num2;
					int num28 = num27;
					num9 = num8;
					for (int j = 2; j < ido; j += 2)
					{
						num9 += 2;
						num28 += 2;
						num10 = num9;
						for (int i = 0; i < l1; i++)
						{
							c1[num10 - 1] = wa[wao + num28 - 1] * ch[cho + num10 - 1] - wa[wao + num28] * ch[cho + num10];
							c1[num10] = wa[wao + num28 - 1] * ch[cho + num10] + wa[wao + num28] * ch[cho + num10 - 1];
							num10 += ido;
						}
					}
				}
				return;
			}
			num27 = -ido - 1;
			num8 = 0;
			for (int k = 1; k < ip; k++)
			{
				num27 += ido;
				num8 += num2;
				num9 = num8;
				for (int i = 0; i < l1; i++)
				{
					int num28 = num27;
					num10 = num9;
					for (int j = 2; j < ido; j += 2)
					{
						num28 += 2;
						num10 += 2;
						c1[num10 - 1] = wa[wao + num28 - 1] * ch[cho + num10 - 1] - wa[wao + num28] * ch[cho + num10];
						c1[num10] = wa[wao + num28 - 1] * ch[cho + num10] + wa[wao + num28] * ch[cho + num10 - 1];
					}
					num9 += ido;
				}
			}
		}

		private static void drftb1(int n, float[] c, int co, float[] ch, int cho, float[] wa, int wao, int[] ifac)
		{
			int num = ifac[1];
			int num2 = 0;
			int num3 = 1;
			int num4 = 1;
			for (int i = 0; i < num; i++)
			{
				int num5 = ifac[i + 2];
				int num6 = num5 * num3;
				int num7 = n / num6;
				int idl = num7 * num3;
				if (num5 == 4)
				{
					int num8 = num4 + num7;
					int num9 = num8 + num7;
					if (num2 != 0)
					{
						Xiph.dradb4(num7, num3, ch, cho, c, co, wa, num4 - 1, wa, num8 - 1, wa, num9 - 1);
					}
					else
					{
						Xiph.dradb4(num7, num3, c, co, ch, cho, wa, num4 - 1, wa, num8 - 1, wa, num9 - 1);
					}
					num2 = 1 - num2;
				}
				else if (num5 == 2)
				{
					if (num2 != 0)
					{
						Xiph.dradb2(num7, num3, ch, cho, c, co, wa, num4 - 1);
					}
					else
					{
						Xiph.dradb2(num7, num3, c, co, ch, cho, wa, num4 - 1);
					}
					num2 = 1 - num2;
				}
				else if (num5 == 3)
				{
					int num8 = num4 + num7;
					if (num2 != 0)
					{
						Xiph.dradb3(num7, num3, ch, cho, c, co, wa, num4 - 1, wa, num8 - 1);
					}
					else
					{
						Xiph.dradb3(num7, num3, c, co, ch, cho, wa, num4 - 1, wa, num8 - 1);
					}
					num2 = 1 - num2;
				}
				else
				{
					if (num2 != 0)
					{
						Xiph.dradbg(num7, num5, num3, idl, ch, cho, ch, cho, ch, cho, c, co, c, co, wa, num4 - 1);
					}
					else
					{
						Xiph.dradbg(num7, num5, num3, idl, c, co, c, co, c, co, ch, cho, ch, cho, wa, num4 - 1);
					}
					if (num7 == 1)
					{
						num2 = 1 - num2;
					}
				}
				num3 = num6;
				num4 += (num5 - 1) * num7;
			}
			if (num2 == 0)
			{
				return;
			}
			for (int j = 0; j < n; j++)
			{
				c[co + j] = ch[cho + j];
			}
		}

		private static void drft_forward(Xiph.drft_lookup l, float[] data, int offset)
		{
			if (l.n == 1)
			{
				return;
			}
			Xiph.drftf1(l.n, data, offset, l.trigcache, 0, l.trigcache, l.n, l.splitcache);
		}

		private static void drft_backward(Xiph.drft_lookup l, float[] data, int offset)
		{
			if (l.n == 1)
			{
				return;
			}
			Xiph.drftb1(l.n, data, offset, l.trigcache, 0, l.trigcache, l.n, l.splitcache);
		}

		private static void drft_init(Xiph.drft_lookup l, int n)
		{
			l.n = n;
			l.trigcache = new float[3 * n];
			l.splitcache = new int[32];
			Xiph.fdrffti(n, l.trigcache, l.splitcache);
		}

		private static void drft_clear(Xiph.drft_lookup l)
		{
			if (l != null)
			{
				l.trigcache = null;
				l.splitcache = null;
			}
			l.n = 0;
		}

		public static int vorbis_synthesis(Xiph.vorbis_block vb, Xiph.ogg_packet op)
		{
			Xiph.vorbis_dsp_state vorbis_dsp_state = (vb != null) ? vb.vd : null;
			Xiph.private_state private_state = (vorbis_dsp_state != null) ? vorbis_dsp_state.backend_state : null;
			Xiph.vorbis_info vorbis_info = (vorbis_dsp_state != null) ? vorbis_dsp_state.vi : null;
			Xiph.codec_setup_info codec_setup_info = (vorbis_info != null) ? vorbis_info.codec_setup : null;
			Xiph.oggpack_buffer oggpack_buffer = (vb != null) ? vb.opb : null;
			if (vorbis_dsp_state == null || private_state == null || vorbis_info == null || codec_setup_info == null || oggpack_buffer == null)
			{
				return -136;
			}
			Xiph._vorbis_block_ripcord(vb);
			Xiph.oggpack_readinit(oggpack_buffer, op.packet, op.bytes);
			if (Xiph.oggpack_read(oggpack_buffer, 1) != 0L)
			{
				return -135;
			}
			int num = (int)Xiph.oggpack_read(oggpack_buffer, private_state.modebits);
			if (num == -1)
			{
				return -136;
			}
			vb.mode = num;
			if (codec_setup_info.mode_param[num] == null)
			{
				return -136;
			}
			vb.W = (long)codec_setup_info.mode_param[num].blockflag;
			if (vb.W != 0L)
			{
				vb.lW = Xiph.oggpack_read(oggpack_buffer, 1);
				vb.nW = Xiph.oggpack_read(oggpack_buffer, 1);
				if (vb.nW == -1L)
				{
					return -136;
				}
			}
			else
			{
				vb.lW = 0L;
				vb.nW = 0L;
			}
			vb.granulepos = op.granulepos;
			vb.sequence = op.packetno;
			vb.eofflag = op.e_o_s;
			vb.pcmend = (int)codec_setup_info.blocksizes[(int)(checked((IntPtr)vb.W))];
			vb.pcm = Xiph.MemCache<float[]>.Get(vorbis_info.channels);
			for (int i = 0; i < vorbis_info.channels; i++)
			{
				vb.pcm[i] = Xiph.MemCache<float>.Get(vb.pcmend);
			}
			int num2 = codec_setup_info.map_type[codec_setup_info.mode_param[num].mapping];
			return Xiph._mapping_P[num2].inverse(vb, codec_setup_info.map_param[codec_setup_info.mode_param[num].mapping]);
		}

		private static float[] _vorbis_window_get(int n)
		{
			return Xiph.vwin[n];
		}

		private static void _vorbis_apply_window(float[] d, int[] winno, long[] blocksizes, int lW, int W, int nW)
		{
			lW = ((W != 0) ? lW : 0);
			nW = ((W != 0) ? nW : 0);
			float[] array = Xiph.vwin[winno[lW]];
			float[] array2 = Xiph.vwin[winno[nW]];
			long num = blocksizes[W];
			long num2 = blocksizes[lW];
			long num3 = blocksizes[nW];
			long num4 = num / 4L - num2 / 4L;
			long num5 = num4 + num2 / 2L;
			long num6 = num / 2L + num / 4L - num3 / 4L;
			long num7 = num6 + num3 / 2L;
			long num8;
			for (num8 = 0L; num8 < num4; num8 += 1L)
			{
				d[(int)(checked((IntPtr)num8))] = 0f;
			}
			long num9 = 0L;
			while (num8 < num5)
			{
				d[(int)(checked((IntPtr)num8))] *= array[(int)(checked((IntPtr)num9))];
				num8 += 1L;
				num9 += 1L;
			}
			num8 = num6;
			num9 = num3 / 2L - 1L;
			while (num8 < num7)
			{
				d[(int)(checked((IntPtr)num8))] *= array2[(int)(checked((IntPtr)num9))];
				num8 += 1L;
				num9 -= 1L;
			}
			while (num8 < num)
			{
				d[(int)(checked((IntPtr)num8))] = 0f;
				num8 += 1L;
			}
		}

		static Xiph()
		{
			// 注意: 此类型已标记为 'beforefieldinit'.
			byte[] array = new byte[2];
			array[0] = 1;
			Xiph.OC_MOD_SHIFT = array;
			Xiph.OC_DCT_TOKEN_MAP = new byte[]
			{
				15,
				16,
				17,
				88,
				80,
				1,
				0,
				48,
				14,
				56,
				57,
				58,
				59,
				60,
				62,
				64,
				66,
				68,
				72,
				2,
				4,
				6,
				8,
				18,
				20,
				22,
				24,
				26,
				32,
				12,
				28,
				40
			};
			Xiph.OC_DCT_TOKEN_MAP_LOG_NENTRIES = new byte[]
			{
				0,
				0,
				0,
				2,
				3,
				0,
				0,
				3,
				0,
				0,
				0,
				0,
				0,
				1,
				1,
				1,
				1,
				2,
				3,
				1,
				1,
				1,
				2,
				1,
				1,
				1,
				1,
				1,
				3,
				1,
				2,
				3
			};
			Xiph.OC_FZIG_ZAG = new byte[]
			{
				0,
				1,
				8,
				16,
				9,
				2,
				3,
				10,
				17,
				24,
				32,
				25,
				18,
				11,
				4,
				5,
				12,
				19,
				26,
				33,
				40,
				48,
				41,
				34,
				27,
				20,
				13,
				6,
				7,
				14,
				21,
				28,
				35,
				42,
				49,
				56,
				57,
				50,
				43,
				36,
				29,
				22,
				15,
				23,
				30,
				37,
				44,
				51,
				58,
				59,
				52,
				45,
				38,
				31,
				39,
				46,
				53,
				60,
				61,
				54,
				47,
				55,
				62,
				63,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64,
				64
			};
			Xiph.OC_IZIG_ZAG = new byte[]
			{
				0,
				1,
				5,
				6,
				14,
				15,
				27,
				28,
				2,
				4,
				7,
				13,
				16,
				26,
				29,
				42,
				3,
				8,
				12,
				17,
				25,
				30,
				41,
				43,
				9,
				11,
				18,
				24,
				31,
				40,
				44,
				53,
				10,
				19,
				23,
				32,
				39,
				45,
				52,
				54,
				20,
				22,
				33,
				38,
				46,
				51,
				55,
				60,
				21,
				34,
				37,
				47,
				50,
				56,
				59,
				61,
				35,
				36,
				48,
				49,
				57,
				58,
				62,
				63
			};
			Xiph.OC_MB_MAP = new byte[,]
			{
				{
					0,
					3
				},
				{
					1,
					2
				}
			};
			Xiph.OC_MB_MAP_IDXS = new byte[][]
			{
				new byte[]
				{
					0,
					1,
					2,
					3,
					4,
					8,
					8,
					8,
					8,
					8,
					8,
					8
				},
				new byte[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					8,
					9,
					9,
					9,
					9,
					9
				},
				new byte[]
				{
					0,
					1,
					2,
					3,
					4,
					6,
					8,
					10,
					10,
					10,
					10,
					10
				},
				new byte[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10,
					11
				}
			};
			Xiph.OC_MB_MAP_NIDXS = new byte[]
			{
				6,
				8,
				8,
				12
			};
			Xiph.OC_DCT_TOKEN_EXTRA_BITS = new byte[]
			{
				0,
				0,
				0,
				2,
				3,
				4,
				12,
				3,
				6,
				0,
				0,
				0,
				0,
				1,
				1,
				1,
				1,
				2,
				3,
				4,
				5,
				6,
				10,
				1,
				1,
				1,
				1,
				1,
				3,
				4,
				2,
				3
			};
			Xiph.OC_DC_QUANT_MIN = new uint[]
			{
				16u,
				32u
			};
			Xiph.OC_AC_QUANT_MIN = new uint[]
			{
				8u,
				16u
			};
			Xiph.OC_SET_CHROMA_MVS_TABLE = new Xiph.oc_set_chroma_mvs_func[]
			{
				new Xiph.oc_set_chroma_mvs_func(Xiph.oc_set_chroma_mvs00),
				new Xiph.oc_set_chroma_mvs_func(Xiph.oc_set_chroma_mvs01),
				new Xiph.oc_set_chroma_mvs_func(Xiph.oc_set_chroma_mvs10),
				new Xiph.oc_set_chroma_mvs_func(Xiph.oc_set_chroma_mvs11)
			};
			Xiph.SB_MAP = new int[,,]
			{
				{
					{
						0,
						0
					},
					{
						0,
						1
					},
					{
						3,
						2
					},
					{
						3,
						3
					}
				},
				{
					{
						0,
						3
					},
					{
						0,
						2
					},
					{
						3,
						1
					},
					{
						3,
						0
					}
				},
				{
					{
						1,
						0
					},
					{
						1,
						3
					},
					{
						2,
						0
					},
					{
						2,
						3
					}
				},
				{
					{
						1,
						1
					},
					{
						1,
						2
					},
					{
						2,
						1
					},
					{
						2,
						2
					}
				}
			};
			Xiph.OC_MB_FILL_CMAPPING_TABLE = new Xiph.oc_mb_fill_cmapping_func[]
			{
				new Xiph.oc_mb_fill_cmapping_func(Xiph.oc_mb_fill_cmapping00),
				new Xiph.oc_mb_fill_cmapping_func(Xiph.oc_mb_fill_cmapping01),
				new Xiph.oc_mb_fill_cmapping_func(Xiph.oc_mb_fill_cmapping10),
				new Xiph.oc_mb_fill_cmapping_func(Xiph.oc_mb_fill_cmapping11)
			};
			Xiph.OC_MVMAP = new sbyte[][]
			{
				new sbyte[]
				{
					-15,
					-15,
					-14,
					-14,
					-13,
					-13,
					-12,
					-12,
					-11,
					-11,
					-10,
					-10,
					-9,
					-9,
					-8,
					-8,
					-7,
					-7,
					-6,
					-6,
					-5,
					-5,
					-4,
					-4,
					-3,
					-3,
					-2,
					-2,
					-1,
					-1,
					0,
					0,
					0,
					1,
					1,
					2,
					2,
					3,
					3,
					4,
					4,
					5,
					5,
					6,
					6,
					7,
					7,
					8,
					8,
					9,
					9,
					10,
					10,
					11,
					11,
					12,
					12,
					13,
					13,
					14,
					14,
					15,
					15
				},
				new sbyte[]
				{
					-7,
					-7,
					-7,
					-7,
					-6,
					-6,
					-6,
					-6,
					-5,
					-5,
					-5,
					-5,
					-4,
					-4,
					-4,
					-4,
					-3,
					-3,
					-3,
					-3,
					-2,
					-2,
					-2,
					-2,
					-1,
					-1,
					-1,
					-1,
					0,
					0,
					0,
					0,
					0,
					0,
					0,
					1,
					1,
					1,
					1,
					2,
					2,
					2,
					2,
					3,
					3,
					3,
					3,
					4,
					4,
					4,
					4,
					5,
					5,
					5,
					5,
					6,
					6,
					6,
					6,
					7,
					7,
					7,
					7
				}
			};
			Xiph.OC_MVMAP2 = new sbyte[][]
			{
				new sbyte[]
				{
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					-1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1,
					0,
					1
				},
				new sbyte[]
				{
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					-1,
					-1,
					-1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1,
					0,
					1,
					1,
					1
				}
			};
			Xiph.floor0_exportbundle = new Xiph.vorbis_func_floor
			{
				pack = null,
				unpack = new Xiph.vorbis_func_floor.Unpack(Xiph.floor0_unpack),
				look = new Xiph.vorbis_func_floor.Look(Xiph.floor0_look),
				free_info = new Xiph.vorbis_func_floor.FreeInfo(Xiph.floor0_free_info),
				free_look = new Xiph.vorbis_func_floor.FreeLook(Xiph.floor0_free_look),
				inverse1 = new Xiph.vorbis_func_floor.Inverse1(Xiph.floor0_inverse1),
				inverse2 = new Xiph.vorbis_func_floor.Inverse2(Xiph.floor0_inverse2)
			};
			Xiph.FLOOR1_fromdB_LOOKUP = new float[]
			{
				1.06498632E-07f,
				1.1341951E-07f,
				1.20790148E-07f,
				1.28639783E-07f,
				1.369995E-07f,
				1.459025E-07f,
				1.55384086E-07f,
				1.65481808E-07f,
				1.76235744E-07f,
				1.87688556E-07f,
				1.998856E-07f,
				2.128753E-07f,
				2.26709133E-07f,
				2.41441967E-07f,
				2.57132228E-07f,
				2.73842119E-07f,
				2.91637917E-07f,
				3.10590224E-07f,
				3.307741E-07f,
				3.52269666E-07f,
				3.75162131E-07f,
				3.995423E-07f,
				4.255068E-07f,
				4.53158634E-07f,
				4.82607447E-07f,
				5.1397E-07f,
				5.47370632E-07f,
				5.829419E-07f,
				6.208247E-07f,
				6.611694E-07f,
				7.041359E-07f,
				7.49894639E-07f,
				7.98627E-07f,
				8.505263E-07f,
				9.057983E-07f,
				9.646621E-07f,
				1.02735135E-06f,
				1.0941144E-06f,
				1.16521608E-06f,
				1.24093845E-06f,
				1.32158164E-06f,
				1.40746545E-06f,
				1.49893049E-06f,
				1.59633942E-06f,
				1.70007854E-06f,
				1.81055918E-06f,
				1.92821949E-06f,
				2.053526E-06f,
				2.18697573E-06f,
				2.3290977E-06f,
				2.48045581E-06f,
				2.64164964E-06f,
				2.813319E-06f,
				2.9961443E-06f,
				3.19085052E-06f,
				3.39821E-06f,
				3.619045E-06f,
				3.85423073E-06f,
				4.10470057E-06f,
				4.371447E-06f,
				4.6555283E-06f,
				4.958071E-06f,
				5.280274E-06f,
				5.623416E-06f,
				5.988857E-06f,
				6.37804669E-06f,
				6.79252844E-06f,
				7.23394533E-06f,
				7.704048E-06f,
				8.2047E-06f,
				8.737888E-06f,
				9.305725E-06f,
				9.910464E-06f,
				1.05545014E-05f,
				1.12403923E-05f,
				1.19708557E-05f,
				1.27487892E-05f,
				1.3577278E-05f,
				1.44596061E-05f,
				1.53992714E-05f,
				1.64000048E-05f,
				1.74657689E-05f,
				1.86007928E-05f,
				1.98095768E-05f,
				2.10969138E-05f,
				2.24679115E-05f,
				2.39280016E-05f,
				2.54829774E-05f,
				2.71390054E-05f,
				2.890265E-05f,
				3.078091E-05f,
				3.27812268E-05f,
				3.49115326E-05f,
				3.718028E-05f,
				3.95964671E-05f,
				4.21696677E-05f,
				4.491009E-05f,
				4.7828602E-05f,
				5.09367746E-05f,
				5.424693E-05f,
				5.77722021E-05f,
				6.152657E-05f,
				6.552491E-05f,
				6.97830837E-05f,
				7.43179844E-05f,
				7.914758E-05f,
				8.429104E-05f,
				8.976875E-05f,
				9.560242E-05f,
				0.000101815211f,
				0.000108431741f,
				0.000115478237f,
				0.000122982674f,
				0.000130974775f,
				0.000139486248f,
				0.000148550855f,
				0.000158204537f,
				0.000168485552f,
				0.00017943469f,
				0.000191095358f,
				0.000203513817f,
				0.0002167393f,
				0.000230824226f,
				0.000245824485f,
				0.000261799549f,
				0.000278812746f,
				0.000296931568f,
				0.000316227874f,
				0.000336778146f,
				0.000358663878f,
				0.000381971884f,
				0.00040679457f,
				0.000433230365f,
				0.0004613841f,
				0.0004913675f,
				0.00052329927f,
				0.0005573062f,
				0.0005935231f,
				0.0006320936f,
				0.0006731706f,
				0.000716917f,
				0.0007635063f,
				0.000813123246f,
				0.000865964568f,
				0.000922239851f,
				0.0009821722f,
				0.00104599923f,
				0.00111397426f,
				0.00118636654f,
				0.00126346329f,
				0.0013455702f,
				0.00143301289f,
				0.00152613816f,
				0.00162531529f,
				0.00173093739f,
				0.00184342347f,
				0.00196321961f,
				0.00209080055f,
				0.0022266726f,
				0.00237137428f,
				0.00252547953f,
				0.00268959929f,
				0.00286438479f,
				0.0030505287f,
				0.003248769f,
				0.00345989247f,
				0.00368473586f,
				0.00392419053f,
				0.00417920668f,
				0.004450795f,
				0.004740033f,
				0.005048067f,
				0.0053761187f,
				0.005725489f,
				0.00609756354f,
				0.00649381755f,
				0.00691582263f,
				0.00736525143f,
				0.007843887f,
				0.008353627f,
				0.008896492f,
				0.009474637f,
				0.010090352f,
				0.01074608f,
				0.0114444206f,
				0.012188144f,
				0.0129801976f,
				0.0138237253f,
				0.0147220679f,
				0.0156787913f,
				0.0166976862f,
				0.0177827962f,
				0.0189384222f,
				0.0201691482f,
				0.0214798544f,
				0.0228757355f,
				0.02436233f,
				0.0259455312f,
				0.0276316181f,
				0.0294272769f,
				0.0313396268f,
				0.03337625f,
				0.0355452262f,
				0.0378551558f,
				0.0403152f,
				0.0429351069f,
				0.0457252748f,
				0.0486967564f,
				0.05186135f,
				0.05523159f,
				0.05882085f,
				0.0626433641f,
				0.06671428f,
				0.07104975f,
				0.0756669641f,
				0.08058423f,
				0.08582105f,
				0.09139818f,
				0.0973377451f,
				0.1036633f,
				0.110399932f,
				0.117574342f,
				0.125214979f,
				0.133352146f,
				0.142018124f,
				0.151247263f,
				0.161076173f,
				0.1715438f,
				0.182691678f,
				0.194564015f,
				0.207207873f,
				0.220673427f,
				0.235014021f,
				0.250286549f,
				0.266551584f,
				0.283873618f,
				0.3023213f,
				0.32196787f,
				0.342891127f,
				0.365174145f,
				0.3889052f,
				0.414178461f,
				0.44109413f,
				0.4697589f,
				0.50028646f,
				0.532797933f,
				0.5674221f,
				0.6042964f,
				0.643566966f,
				0.6853896f,
				0.729930043f,
				0.777365f,
				0.8278826f,
				0.881683052f,
				0.9389798f,
				1f
			};
			Xiph.floor1_exportbundle = new Xiph.vorbis_func_floor
			{
				pack = new Xiph.vorbis_func_floor.Pack(Xiph.floor1_pack),
				unpack = new Xiph.vorbis_func_floor.Unpack(Xiph.floor1_unpack),
				look = new Xiph.vorbis_func_floor.Look(Xiph.floor1_look),
				free_info = new Xiph.vorbis_func_floor.FreeInfo(Xiph.floor1_free_info),
				free_look = new Xiph.vorbis_func_floor.FreeLook(Xiph.floor1_free_look),
				inverse1 = new Xiph.vorbis_func_floor.Inverse1(Xiph.floor1_inverse1),
				inverse2 = new Xiph.vorbis_func_floor.Inverse2(Xiph.floor1_inverse2)
			};
			Xiph.mapping0_exportbundle = new Xiph.vorbis_func_mapping
			{
				pack = new Xiph.vorbis_func_mapping.Pack(Xiph.mapping0_pack),
				unpack = new Xiph.vorbis_func_mapping.Unpack(Xiph.mapping0_unpack),
				free_info = new Xiph.vorbis_func_mapping.FreeInfo(Xiph.mapping0_free_info),
				forward = new Xiph.vorbis_func_mapping.Forward(Xiph.mapping0_forward),
				inverse = new Xiph.vorbis_func_mapping.Inverse(Xiph.mapping0_inverse)
			};
			Xiph.ATH = new float[]
			{
				-51f,
				-52f,
				-53f,
				-54f,
				-55f,
				-56f,
				-57f,
				-58f,
				-59f,
				-60f,
				-61f,
				-62f,
				-63f,
				-64f,
				-65f,
				-66f,
				-67f,
				-68f,
				-69f,
				-70f,
				-71f,
				-72f,
				-73f,
				-74f,
				-75f,
				-76f,
				-77f,
				-78f,
				-80f,
				-81f,
				-82f,
				-83f,
				-84f,
				-85f,
				-86f,
				-87f,
				-88f,
				-88f,
				-89f,
				-89f,
				-90f,
				-91f,
				-91f,
				-92f,
				-93f,
				-94f,
				-95f,
				-96f,
				-96f,
				-97f,
				-98f,
				-98f,
				-99f,
				-99f,
				-100f,
				-100f,
				-101f,
				-102f,
				-103f,
				-104f,
				-106f,
				-107f,
				-107f,
				-107f,
				-107f,
				-105f,
				-103f,
				-102f,
				-101f,
				-99f,
				-98f,
				-96f,
				-95f,
				-95f,
				-96f,
				-97f,
				-96f,
				-95f,
				-93f,
				-90f,
				-80f,
				-70f,
				-50f,
				-40f,
				-30f,
				-30f,
				-30f,
				-30f
			};
			Xiph.tonemasks = new float[][][]
			{
				new float[][]
				{
					new float[]
					{
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-60f,
						-62f,
						-62f,
						-65f,
						-73f,
						-69f,
						-68f,
						-68f,
						-67f,
						-70f,
						-70f,
						-72f,
						-74f,
						-75f,
						-79f,
						-79f,
						-80f,
						-83f,
						-88f,
						-93f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-48f,
						-53f,
						-61f,
						-66f,
						-66f,
						-68f,
						-67f,
						-70f,
						-76f,
						-76f,
						-72f,
						-73f,
						-75f,
						-76f,
						-78f,
						-79f,
						-83f,
						-88f,
						-93f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-37f,
						-37f,
						-37f,
						-37f,
						-37f,
						-37f,
						-37f,
						-37f,
						-38f,
						-40f,
						-42f,
						-46f,
						-48f,
						-53f,
						-55f,
						-62f,
						-65f,
						-58f,
						-56f,
						-56f,
						-61f,
						-60f,
						-65f,
						-67f,
						-69f,
						-71f,
						-77f,
						-77f,
						-78f,
						-80f,
						-82f,
						-84f,
						-88f,
						-93f,
						-98f,
						-106f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-25f,
						-25f,
						-25f,
						-25f,
						-25f,
						-25f,
						-25f,
						-25f,
						-25f,
						-26f,
						-27f,
						-29f,
						-32f,
						-38f,
						-48f,
						-52f,
						-52f,
						-50f,
						-48f,
						-48f,
						-51f,
						-52f,
						-54f,
						-60f,
						-67f,
						-67f,
						-66f,
						-68f,
						-69f,
						-73f,
						-73f,
						-76f,
						-80f,
						-81f,
						-81f,
						-85f,
						-85f,
						-86f,
						-88f,
						-93f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-16f,
						-16f,
						-16f,
						-16f,
						-16f,
						-16f,
						-16f,
						-16f,
						-17f,
						-19f,
						-20f,
						-22f,
						-26f,
						-28f,
						-31f,
						-40f,
						-47f,
						-39f,
						-39f,
						-40f,
						-42f,
						-43f,
						-47f,
						-51f,
						-57f,
						-52f,
						-55f,
						-55f,
						-60f,
						-58f,
						-62f,
						-63f,
						-70f,
						-67f,
						-69f,
						-72f,
						-73f,
						-77f,
						-80f,
						-82f,
						-83f,
						-87f,
						-90f,
						-94f,
						-98f,
						-104f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-8f,
						-10f,
						-11f,
						-15f,
						-19f,
						-25f,
						-30f,
						-34f,
						-31f,
						-30f,
						-31f,
						-29f,
						-32f,
						-35f,
						-42f,
						-48f,
						-42f,
						-44f,
						-46f,
						-50f,
						-50f,
						-51f,
						-52f,
						-59f,
						-54f,
						-55f,
						-55f,
						-58f,
						-62f,
						-63f,
						-66f,
						-72f,
						-73f,
						-76f,
						-75f,
						-78f,
						-80f,
						-80f,
						-81f,
						-84f,
						-88f,
						-90f,
						-94f,
						-98f,
						-101f,
						-106f,
						-110f
					}
				},
				new float[][]
				{
					new float[]
					{
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-66f,
						-67f,
						-67f,
						-67f,
						-76f,
						-72f,
						-71f,
						-74f,
						-76f,
						-76f,
						-75f,
						-78f,
						-79f,
						-79f,
						-81f,
						-83f,
						-86f,
						-89f,
						-93f,
						-97f,
						-100f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-47f,
						-48f,
						-51f,
						-55f,
						-59f,
						-66f,
						-66f,
						-66f,
						-67f,
						-66f,
						-68f,
						-69f,
						-70f,
						-74f,
						-79f,
						-77f,
						-77f,
						-78f,
						-80f,
						-81f,
						-82f,
						-84f,
						-86f,
						-88f,
						-91f,
						-95f,
						-100f,
						-108f,
						-116f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-37f,
						-37f,
						-41f,
						-44f,
						-48f,
						-51f,
						-58f,
						-62f,
						-60f,
						-57f,
						-59f,
						-59f,
						-60f,
						-63f,
						-65f,
						-72f,
						-71f,
						-70f,
						-72f,
						-74f,
						-77f,
						-76f,
						-78f,
						-81f,
						-81f,
						-80f,
						-83f,
						-86f,
						-91f,
						-96f,
						-100f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-28f,
						-28f,
						-28f,
						-28f,
						-28f,
						-28f,
						-28f,
						-28f,
						-28f,
						-30f,
						-32f,
						-32f,
						-33f,
						-35f,
						-41f,
						-49f,
						-50f,
						-49f,
						-47f,
						-48f,
						-48f,
						-52f,
						-51f,
						-57f,
						-65f,
						-61f,
						-59f,
						-61f,
						-64f,
						-69f,
						-70f,
						-74f,
						-77f,
						-77f,
						-78f,
						-81f,
						-84f,
						-85f,
						-87f,
						-90f,
						-92f,
						-96f,
						-100f,
						-107f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-19f,
						-19f,
						-19f,
						-19f,
						-19f,
						-19f,
						-19f,
						-19f,
						-20f,
						-21f,
						-23f,
						-27f,
						-30f,
						-35f,
						-36f,
						-41f,
						-46f,
						-44f,
						-42f,
						-40f,
						-41f,
						-41f,
						-43f,
						-48f,
						-55f,
						-53f,
						-52f,
						-53f,
						-56f,
						-59f,
						-58f,
						-60f,
						-67f,
						-66f,
						-69f,
						-71f,
						-72f,
						-75f,
						-79f,
						-81f,
						-84f,
						-87f,
						-90f,
						-93f,
						-97f,
						-101f,
						-107f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-9f,
						-9f,
						-9f,
						-9f,
						-9f,
						-9f,
						-9f,
						-9f,
						-11f,
						-12f,
						-12f,
						-15f,
						-16f,
						-20f,
						-23f,
						-30f,
						-37f,
						-34f,
						-33f,
						-34f,
						-31f,
						-32f,
						-32f,
						-38f,
						-47f,
						-44f,
						-41f,
						-40f,
						-47f,
						-49f,
						-46f,
						-46f,
						-58f,
						-50f,
						-50f,
						-54f,
						-58f,
						-62f,
						-64f,
						-67f,
						-67f,
						-70f,
						-72f,
						-76f,
						-79f,
						-83f,
						-87f,
						-91f,
						-96f,
						-100f,
						-104f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-62f,
						-63f,
						-64f,
						-66f,
						-67f,
						-66f,
						-68f,
						-75f,
						-72f,
						-76f,
						-75f,
						-76f,
						-78f,
						-79f,
						-82f,
						-84f,
						-85f,
						-90f,
						-94f,
						-101f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-59f,
						-60f,
						-60f,
						-61f,
						-63f,
						-66f,
						-71f,
						-68f,
						-70f,
						-70f,
						-71f,
						-72f,
						-72f,
						-75f,
						-81f,
						-78f,
						-79f,
						-82f,
						-83f,
						-86f,
						-90f,
						-97f,
						-103f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-53f,
						-53f,
						-53f,
						-53f,
						-53f,
						-53f,
						-53f,
						-53f,
						-53f,
						-54f,
						-55f,
						-57f,
						-56f,
						-57f,
						-55f,
						-61f,
						-65f,
						-60f,
						-60f,
						-62f,
						-63f,
						-63f,
						-66f,
						-68f,
						-74f,
						-73f,
						-75f,
						-75f,
						-78f,
						-80f,
						-80f,
						-82f,
						-85f,
						-90f,
						-96f,
						-101f,
						-108f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-46f,
						-47f,
						-47f,
						-47f,
						-47f,
						-48f,
						-51f,
						-57f,
						-51f,
						-49f,
						-50f,
						-51f,
						-53f,
						-54f,
						-59f,
						-66f,
						-60f,
						-62f,
						-67f,
						-67f,
						-70f,
						-72f,
						-75f,
						-76f,
						-78f,
						-81f,
						-85f,
						-88f,
						-94f,
						-97f,
						-104f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-36f,
						-39f,
						-41f,
						-42f,
						-42f,
						-39f,
						-38f,
						-41f,
						-43f,
						-52f,
						-44f,
						-40f,
						-39f,
						-37f,
						-37f,
						-40f,
						-47f,
						-54f,
						-50f,
						-48f,
						-50f,
						-55f,
						-61f,
						-59f,
						-62f,
						-66f,
						-66f,
						-66f,
						-69f,
						-69f,
						-73f,
						-74f,
						-74f,
						-75f,
						-77f,
						-79f,
						-82f,
						-87f,
						-91f,
						-95f,
						-100f,
						-108f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-28f,
						-26f,
						-24f,
						-22f,
						-20f,
						-20f,
						-23f,
						-29f,
						-30f,
						-31f,
						-28f,
						-27f,
						-28f,
						-28f,
						-28f,
						-35f,
						-40f,
						-33f,
						-32f,
						-29f,
						-30f,
						-30f,
						-30f,
						-37f,
						-45f,
						-41f,
						-37f,
						-38f,
						-45f,
						-47f,
						-47f,
						-48f,
						-53f,
						-49f,
						-48f,
						-50f,
						-49f,
						-49f,
						-51f,
						-52f,
						-58f,
						-56f,
						-57f,
						-56f,
						-60f,
						-61f,
						-62f,
						-70f,
						-72f,
						-74f,
						-78f,
						-83f,
						-88f,
						-93f,
						-100f,
						-106f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-105f,
						-100f,
						-95f,
						-91f,
						-87f,
						-83f,
						-80f,
						-78f,
						-76f,
						-78f,
						-78f,
						-81f,
						-83f,
						-85f,
						-86f,
						-85f,
						-86f,
						-87f,
						-90f,
						-97f,
						-107f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-110f,
						-105f,
						-100f,
						-95f,
						-90f,
						-85f,
						-81f,
						-77f,
						-73f,
						-70f,
						-67f,
						-67f,
						-68f,
						-75f,
						-73f,
						-70f,
						-69f,
						-70f,
						-72f,
						-75f,
						-79f,
						-84f,
						-83f,
						-84f,
						-86f,
						-88f,
						-89f,
						-89f,
						-93f,
						-98f,
						-105f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-105f,
						-100f,
						-95f,
						-90f,
						-85f,
						-80f,
						-76f,
						-71f,
						-68f,
						-68f,
						-65f,
						-63f,
						-63f,
						-62f,
						-62f,
						-64f,
						-65f,
						-64f,
						-61f,
						-62f,
						-63f,
						-64f,
						-66f,
						-68f,
						-73f,
						-73f,
						-74f,
						-75f,
						-76f,
						-81f,
						-83f,
						-85f,
						-88f,
						-89f,
						-92f,
						-95f,
						-100f,
						-108f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-80f,
						-75f,
						-71f,
						-68f,
						-65f,
						-63f,
						-62f,
						-61f,
						-61f,
						-61f,
						-61f,
						-59f,
						-56f,
						-57f,
						-53f,
						-50f,
						-58f,
						-52f,
						-50f,
						-50f,
						-52f,
						-53f,
						-54f,
						-58f,
						-67f,
						-63f,
						-67f,
						-68f,
						-72f,
						-75f,
						-78f,
						-80f,
						-81f,
						-81f,
						-82f,
						-85f,
						-89f,
						-90f,
						-93f,
						-97f,
						-101f,
						-107f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-65f,
						-61f,
						-59f,
						-57f,
						-56f,
						-55f,
						-55f,
						-56f,
						-56f,
						-57f,
						-55f,
						-53f,
						-52f,
						-47f,
						-44f,
						-44f,
						-50f,
						-44f,
						-41f,
						-39f,
						-39f,
						-42f,
						-40f,
						-46f,
						-51f,
						-49f,
						-50f,
						-53f,
						-54f,
						-63f,
						-60f,
						-61f,
						-62f,
						-66f,
						-66f,
						-66f,
						-70f,
						-73f,
						-74f,
						-75f,
						-76f,
						-75f,
						-79f,
						-85f,
						-89f,
						-91f,
						-96f,
						-102f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-52f,
						-50f,
						-49f,
						-49f,
						-48f,
						-48f,
						-48f,
						-49f,
						-50f,
						-50f,
						-49f,
						-46f,
						-43f,
						-39f,
						-35f,
						-33f,
						-38f,
						-36f,
						-32f,
						-29f,
						-32f,
						-32f,
						-32f,
						-35f,
						-44f,
						-39f,
						-38f,
						-38f,
						-46f,
						-50f,
						-45f,
						-46f,
						-53f,
						-50f,
						-50f,
						-50f,
						-54f,
						-54f,
						-53f,
						-53f,
						-56f,
						-57f,
						-59f,
						-66f,
						-70f,
						-72f,
						-74f,
						-79f,
						-83f,
						-85f,
						-90f,
						-97f,
						-114f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-105f,
						-100f,
						-95f,
						-90f,
						-86f,
						-80f,
						-75f,
						-75f,
						-79f,
						-80f,
						-79f,
						-80f,
						-81f,
						-82f,
						-88f,
						-95f,
						-103f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-88f,
						-83f,
						-79f,
						-78f,
						-75f,
						-71f,
						-67f,
						-68f,
						-73f,
						-73f,
						-72f,
						-73f,
						-75f,
						-77f,
						-80f,
						-82f,
						-88f,
						-93f,
						-100f,
						-107f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-110f,
						-105f,
						-101f,
						-96f,
						-90f,
						-86f,
						-81f,
						-77f,
						-73f,
						-69f,
						-66f,
						-61f,
						-62f,
						-66f,
						-64f,
						-62f,
						-65f,
						-66f,
						-70f,
						-72f,
						-76f,
						-81f,
						-80f,
						-84f,
						-90f,
						-95f,
						-102f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-107f,
						-103f,
						-97f,
						-92f,
						-88f,
						-83f,
						-79f,
						-74f,
						-70f,
						-66f,
						-59f,
						-53f,
						-58f,
						-62f,
						-55f,
						-54f,
						-54f,
						-54f,
						-58f,
						-61f,
						-62f,
						-72f,
						-70f,
						-72f,
						-75f,
						-78f,
						-80f,
						-81f,
						-80f,
						-83f,
						-83f,
						-88f,
						-93f,
						-100f,
						-107f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-105f,
						-100f,
						-95f,
						-90f,
						-85f,
						-80f,
						-75f,
						-70f,
						-66f,
						-62f,
						-56f,
						-48f,
						-44f,
						-48f,
						-46f,
						-46f,
						-43f,
						-46f,
						-48f,
						-48f,
						-51f,
						-58f,
						-58f,
						-59f,
						-60f,
						-62f,
						-62f,
						-61f,
						-61f,
						-65f,
						-64f,
						-65f,
						-68f,
						-70f,
						-74f,
						-75f,
						-78f,
						-81f,
						-86f,
						-95f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-105f,
						-100f,
						-95f,
						-90f,
						-85f,
						-80f,
						-75f,
						-70f,
						-65f,
						-61f,
						-55f,
						-49f,
						-39f,
						-33f,
						-40f,
						-35f,
						-32f,
						-38f,
						-40f,
						-33f,
						-35f,
						-37f,
						-46f,
						-41f,
						-45f,
						-44f,
						-46f,
						-42f,
						-45f,
						-46f,
						-52f,
						-50f,
						-50f,
						-50f,
						-54f,
						-54f,
						-55f,
						-57f,
						-62f,
						-64f,
						-66f,
						-68f,
						-70f,
						-76f,
						-81f,
						-90f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-98f,
						-90f,
						-85f,
						-82f,
						-83f,
						-80f,
						-78f,
						-84f,
						-79f,
						-80f,
						-83f,
						-87f,
						-89f,
						-91f,
						-93f,
						-99f,
						-106f,
						-117f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-98f,
						-90f,
						-85f,
						-80f,
						-75f,
						-70f,
						-68f,
						-74f,
						-72f,
						-74f,
						-77f,
						-80f,
						-82f,
						-85f,
						-87f,
						-92f,
						-89f,
						-91f,
						-95f,
						-100f,
						-106f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-98f,
						-90f,
						-83f,
						-75f,
						-71f,
						-63f,
						-64f,
						-67f,
						-62f,
						-64f,
						-67f,
						-70f,
						-73f,
						-77f,
						-81f,
						-84f,
						-83f,
						-85f,
						-89f,
						-90f,
						-93f,
						-98f,
						-104f,
						-109f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-103f,
						-96f,
						-88f,
						-81f,
						-75f,
						-68f,
						-58f,
						-54f,
						-56f,
						-54f,
						-56f,
						-56f,
						-58f,
						-60f,
						-63f,
						-66f,
						-74f,
						-69f,
						-72f,
						-72f,
						-75f,
						-74f,
						-77f,
						-81f,
						-81f,
						-82f,
						-84f,
						-87f,
						-93f,
						-96f,
						-99f,
						-104f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-102f,
						-96f,
						-91f,
						-85f,
						-80f,
						-74f,
						-68f,
						-60f,
						-51f,
						-46f,
						-48f,
						-46f,
						-43f,
						-45f,
						-47f,
						-47f,
						-49f,
						-48f,
						-56f,
						-53f,
						-55f,
						-58f,
						-57f,
						-63f,
						-58f,
						-60f,
						-66f,
						-64f,
						-67f,
						-70f,
						-70f,
						-74f,
						-77f,
						-84f,
						-86f,
						-89f,
						-91f,
						-93f,
						-94f,
						-101f,
						-109f,
						-118f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-88f,
						-83f,
						-78f,
						-73f,
						-68f,
						-60f,
						-53f,
						-44f,
						-35f,
						-38f,
						-38f,
						-34f,
						-34f,
						-36f,
						-40f,
						-41f,
						-44f,
						-51f,
						-45f,
						-46f,
						-47f,
						-46f,
						-54f,
						-50f,
						-49f,
						-50f,
						-50f,
						-50f,
						-51f,
						-54f,
						-57f,
						-58f,
						-60f,
						-66f,
						-66f,
						-66f,
						-64f,
						-65f,
						-68f,
						-77f,
						-82f,
						-87f,
						-95f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-102f,
						-97f,
						-92f,
						-87f,
						-83f,
						-78f,
						-75f,
						-82f,
						-79f,
						-83f,
						-85f,
						-89f,
						-92f,
						-95f,
						-98f,
						-101f,
						-105f,
						-109f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-106f,
						-100f,
						-95f,
						-90f,
						-86f,
						-81f,
						-78f,
						-74f,
						-69f,
						-74f,
						-74f,
						-76f,
						-79f,
						-83f,
						-84f,
						-86f,
						-89f,
						-92f,
						-97f,
						-93f,
						-100f,
						-103f,
						-107f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-106f,
						-100f,
						-95f,
						-90f,
						-87f,
						-83f,
						-80f,
						-75f,
						-69f,
						-60f,
						-66f,
						-66f,
						-68f,
						-70f,
						-74f,
						-78f,
						-79f,
						-81f,
						-81f,
						-83f,
						-84f,
						-87f,
						-93f,
						-96f,
						-99f,
						-103f,
						-107f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-89f,
						-85f,
						-82f,
						-78f,
						-71f,
						-62f,
						-55f,
						-58f,
						-58f,
						-54f,
						-54f,
						-55f,
						-59f,
						-61f,
						-62f,
						-70f,
						-66f,
						-66f,
						-67f,
						-70f,
						-72f,
						-75f,
						-78f,
						-84f,
						-84f,
						-84f,
						-88f,
						-91f,
						-90f,
						-95f,
						-98f,
						-102f,
						-103f,
						-106f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-94f,
						-90f,
						-87f,
						-82f,
						-79f,
						-73f,
						-67f,
						-58f,
						-47f,
						-50f,
						-45f,
						-41f,
						-45f,
						-48f,
						-44f,
						-44f,
						-49f,
						-54f,
						-51f,
						-48f,
						-47f,
						-49f,
						-50f,
						-51f,
						-57f,
						-58f,
						-60f,
						-63f,
						-69f,
						-70f,
						-69f,
						-71f,
						-74f,
						-78f,
						-82f,
						-90f,
						-95f,
						-101f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-105f,
						-101f,
						-97f,
						-93f,
						-90f,
						-85f,
						-80f,
						-77f,
						-72f,
						-65f,
						-56f,
						-48f,
						-37f,
						-40f,
						-36f,
						-34f,
						-40f,
						-50f,
						-47f,
						-38f,
						-41f,
						-47f,
						-38f,
						-35f,
						-39f,
						-38f,
						-43f,
						-40f,
						-45f,
						-50f,
						-45f,
						-44f,
						-47f,
						-50f,
						-55f,
						-48f,
						-48f,
						-52f,
						-66f,
						-70f,
						-76f,
						-82f,
						-90f,
						-97f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-86f,
						-79f,
						-76f,
						-83f,
						-81f,
						-85f,
						-87f,
						-89f,
						-93f,
						-98f,
						-102f,
						-107f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-86f,
						-79f,
						-71f,
						-77f,
						-74f,
						-77f,
						-79f,
						-81f,
						-84f,
						-85f,
						-90f,
						-92f,
						-93f,
						-92f,
						-98f,
						-101f,
						-108f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-103f,
						-98f,
						-93f,
						-87f,
						-78f,
						-68f,
						-65f,
						-66f,
						-62f,
						-65f,
						-67f,
						-70f,
						-73f,
						-75f,
						-78f,
						-82f,
						-82f,
						-83f,
						-84f,
						-91f,
						-93f,
						-98f,
						-102f,
						-106f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-100f,
						-95f,
						-90f,
						-82f,
						-74f,
						-62f,
						-57f,
						-58f,
						-56f,
						-51f,
						-52f,
						-52f,
						-54f,
						-54f,
						-58f,
						-66f,
						-59f,
						-60f,
						-63f,
						-66f,
						-69f,
						-73f,
						-79f,
						-83f,
						-84f,
						-80f,
						-81f,
						-81f,
						-82f,
						-88f,
						-92f,
						-98f,
						-105f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-102f,
						-97f,
						-92f,
						-84f,
						-79f,
						-69f,
						-57f,
						-47f,
						-52f,
						-47f,
						-44f,
						-45f,
						-50f,
						-52f,
						-42f,
						-42f,
						-53f,
						-43f,
						-43f,
						-48f,
						-51f,
						-56f,
						-55f,
						-52f,
						-57f,
						-59f,
						-61f,
						-62f,
						-67f,
						-71f,
						-78f,
						-83f,
						-86f,
						-94f,
						-98f,
						-103f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-100f,
						-95f,
						-90f,
						-84f,
						-78f,
						-70f,
						-61f,
						-51f,
						-41f,
						-40f,
						-38f,
						-40f,
						-46f,
						-52f,
						-51f,
						-41f,
						-40f,
						-46f,
						-40f,
						-38f,
						-38f,
						-41f,
						-46f,
						-41f,
						-46f,
						-47f,
						-43f,
						-43f,
						-45f,
						-41f,
						-45f,
						-56f,
						-67f,
						-68f,
						-83f,
						-87f,
						-90f,
						-95f,
						-102f,
						-107f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-109f,
						-105f,
						-101f,
						-96f,
						-91f,
						-84f,
						-77f,
						-82f,
						-82f,
						-85f,
						-89f,
						-94f,
						-100f,
						-106f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-106f,
						-103f,
						-98f,
						-92f,
						-85f,
						-80f,
						-71f,
						-75f,
						-72f,
						-76f,
						-80f,
						-84f,
						-86f,
						-89f,
						-93f,
						-100f,
						-107f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-104f,
						-101f,
						-97f,
						-92f,
						-88f,
						-84f,
						-80f,
						-64f,
						-66f,
						-63f,
						-64f,
						-66f,
						-69f,
						-73f,
						-77f,
						-83f,
						-83f,
						-86f,
						-91f,
						-98f,
						-104f,
						-111f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-104f,
						-101f,
						-97f,
						-92f,
						-90f,
						-84f,
						-74f,
						-57f,
						-58f,
						-52f,
						-55f,
						-54f,
						-50f,
						-52f,
						-50f,
						-52f,
						-63f,
						-62f,
						-69f,
						-76f,
						-77f,
						-78f,
						-78f,
						-79f,
						-82f,
						-88f,
						-94f,
						-100f,
						-106f,
						-111f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-106f,
						-102f,
						-98f,
						-95f,
						-90f,
						-85f,
						-83f,
						-78f,
						-70f,
						-50f,
						-50f,
						-41f,
						-44f,
						-49f,
						-47f,
						-50f,
						-50f,
						-44f,
						-55f,
						-46f,
						-47f,
						-48f,
						-48f,
						-54f,
						-49f,
						-49f,
						-58f,
						-62f,
						-71f,
						-81f,
						-87f,
						-92f,
						-97f,
						-102f,
						-108f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-106f,
						-102f,
						-98f,
						-95f,
						-90f,
						-85f,
						-83f,
						-78f,
						-70f,
						-45f,
						-43f,
						-41f,
						-47f,
						-50f,
						-51f,
						-50f,
						-49f,
						-45f,
						-47f,
						-41f,
						-44f,
						-41f,
						-39f,
						-43f,
						-38f,
						-37f,
						-40f,
						-41f,
						-44f,
						-50f,
						-58f,
						-65f,
						-73f,
						-79f,
						-85f,
						-92f,
						-97f,
						-101f,
						-105f,
						-109f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-100f,
						-95f,
						-87f,
						-81f,
						-85f,
						-83f,
						-88f,
						-93f,
						-100f,
						-107f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-101f,
						-95f,
						-88f,
						-83f,
						-76f,
						-73f,
						-72f,
						-79f,
						-84f,
						-90f,
						-95f,
						-100f,
						-105f,
						-110f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-104f,
						-98f,
						-92f,
						-87f,
						-81f,
						-70f,
						-65f,
						-62f,
						-67f,
						-71f,
						-74f,
						-80f,
						-85f,
						-91f,
						-95f,
						-99f,
						-103f,
						-108f,
						-111f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-103f,
						-97f,
						-90f,
						-85f,
						-76f,
						-60f,
						-56f,
						-54f,
						-60f,
						-62f,
						-61f,
						-56f,
						-63f,
						-65f,
						-73f,
						-74f,
						-77f,
						-75f,
						-78f,
						-81f,
						-86f,
						-87f,
						-88f,
						-91f,
						-94f,
						-98f,
						-103f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-100f,
						-97f,
						-92f,
						-86f,
						-81f,
						-79f,
						-70f,
						-57f,
						-51f,
						-47f,
						-51f,
						-58f,
						-60f,
						-56f,
						-53f,
						-50f,
						-58f,
						-52f,
						-50f,
						-50f,
						-53f,
						-55f,
						-64f,
						-69f,
						-71f,
						-85f,
						-82f,
						-78f,
						-81f,
						-85f,
						-95f,
						-102f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-100f,
						-97f,
						-92f,
						-85f,
						-83f,
						-79f,
						-72f,
						-49f,
						-40f,
						-43f,
						-43f,
						-54f,
						-56f,
						-51f,
						-50f,
						-40f,
						-43f,
						-38f,
						-36f,
						-35f,
						-37f,
						-38f,
						-37f,
						-44f,
						-54f,
						-60f,
						-57f,
						-60f,
						-70f,
						-75f,
						-84f,
						-92f,
						-103f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-102f,
						-95f,
						-89f,
						-82f,
						-83f,
						-84f,
						-90f,
						-92f,
						-99f,
						-107f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-101f,
						-95f,
						-89f,
						-83f,
						-72f,
						-74f,
						-78f,
						-85f,
						-88f,
						-88f,
						-90f,
						-92f,
						-98f,
						-105f,
						-111f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-109f,
						-103f,
						-97f,
						-93f,
						-87f,
						-81f,
						-70f,
						-70f,
						-67f,
						-75f,
						-73f,
						-76f,
						-79f,
						-81f,
						-83f,
						-88f,
						-89f,
						-97f,
						-103f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-107f,
						-100f,
						-94f,
						-88f,
						-83f,
						-75f,
						-63f,
						-59f,
						-59f,
						-63f,
						-66f,
						-60f,
						-62f,
						-67f,
						-67f,
						-77f,
						-76f,
						-81f,
						-88f,
						-86f,
						-92f,
						-96f,
						-102f,
						-109f,
						-116f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-98f,
						-92f,
						-86f,
						-81f,
						-73f,
						-56f,
						-52f,
						-47f,
						-55f,
						-60f,
						-58f,
						-52f,
						-51f,
						-45f,
						-49f,
						-50f,
						-53f,
						-54f,
						-61f,
						-71f,
						-70f,
						-69f,
						-78f,
						-79f,
						-87f,
						-90f,
						-96f,
						-104f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-103f,
						-96f,
						-90f,
						-86f,
						-78f,
						-70f,
						-51f,
						-42f,
						-47f,
						-48f,
						-55f,
						-54f,
						-54f,
						-53f,
						-42f,
						-35f,
						-28f,
						-33f,
						-38f,
						-37f,
						-44f,
						-47f,
						-49f,
						-54f,
						-63f,
						-68f,
						-78f,
						-82f,
						-89f,
						-94f,
						-99f,
						-104f,
						-109f,
						-114f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-90f,
						-79f,
						-85f,
						-81f,
						-82f,
						-82f,
						-89f,
						-94f,
						-99f,
						-103f,
						-109f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-97f,
						-85f,
						-72f,
						-74f,
						-70f,
						-70f,
						-70f,
						-76f,
						-85f,
						-91f,
						-93f,
						-97f,
						-103f,
						-109f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-112f,
						-93f,
						-81f,
						-68f,
						-62f,
						-60f,
						-60f,
						-57f,
						-63f,
						-70f,
						-77f,
						-82f,
						-90f,
						-93f,
						-98f,
						-104f,
						-109f,
						-113f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-113f,
						-100f,
						-93f,
						-84f,
						-63f,
						-58f,
						-48f,
						-53f,
						-54f,
						-52f,
						-52f,
						-57f,
						-64f,
						-66f,
						-76f,
						-83f,
						-81f,
						-85f,
						-85f,
						-90f,
						-95f,
						-98f,
						-101f,
						-103f,
						-106f,
						-108f,
						-111f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-105f,
						-95f,
						-86f,
						-74f,
						-53f,
						-50f,
						-38f,
						-43f,
						-49f,
						-43f,
						-42f,
						-39f,
						-39f,
						-46f,
						-52f,
						-57f,
						-56f,
						-72f,
						-69f,
						-74f,
						-81f,
						-87f,
						-92f,
						-94f,
						-97f,
						-99f,
						-102f,
						-105f,
						-108f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-108f,
						-99f,
						-90f,
						-76f,
						-66f,
						-45f,
						-43f,
						-41f,
						-44f,
						-47f,
						-43f,
						-47f,
						-40f,
						-30f,
						-31f,
						-31f,
						-39f,
						-33f,
						-40f,
						-41f,
						-43f,
						-53f,
						-59f,
						-70f,
						-73f,
						-77f,
						-79f,
						-82f,
						-84f,
						-87f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-91f,
						-76f,
						-75f,
						-85f,
						-93f,
						-98f,
						-104f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-91f,
						-70f,
						-70f,
						-75f,
						-86f,
						-89f,
						-94f,
						-98f,
						-101f,
						-106f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-95f,
						-80f,
						-60f,
						-65f,
						-64f,
						-74f,
						-83f,
						-88f,
						-91f,
						-95f,
						-99f,
						-103f,
						-107f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-95f,
						-80f,
						-58f,
						-55f,
						-49f,
						-66f,
						-68f,
						-71f,
						-78f,
						-78f,
						-80f,
						-88f,
						-85f,
						-89f,
						-97f,
						-100f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-95f,
						-80f,
						-53f,
						-52f,
						-41f,
						-59f,
						-59f,
						-49f,
						-58f,
						-56f,
						-63f,
						-86f,
						-79f,
						-90f,
						-93f,
						-98f,
						-103f,
						-107f,
						-112f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-97f,
						-91f,
						-73f,
						-45f,
						-40f,
						-33f,
						-53f,
						-61f,
						-49f,
						-54f,
						-50f,
						-50f,
						-60f,
						-52f,
						-67f,
						-74f,
						-81f,
						-92f,
						-96f,
						-100f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-113f,
						-106f,
						-99f,
						-92f,
						-77f,
						-80f,
						-88f,
						-97f,
						-106f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-116f,
						-109f,
						-102f,
						-95f,
						-89f,
						-74f,
						-72f,
						-88f,
						-87f,
						-95f,
						-102f,
						-109f,
						-116f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-116f,
						-109f,
						-102f,
						-95f,
						-89f,
						-75f,
						-66f,
						-74f,
						-77f,
						-78f,
						-86f,
						-87f,
						-90f,
						-96f,
						-105f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-115f,
						-108f,
						-101f,
						-94f,
						-88f,
						-66f,
						-56f,
						-61f,
						-70f,
						-65f,
						-78f,
						-72f,
						-83f,
						-84f,
						-93f,
						-98f,
						-105f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-105f,
						-95f,
						-89f,
						-82f,
						-57f,
						-52f,
						-52f,
						-59f,
						-56f,
						-59f,
						-58f,
						-69f,
						-67f,
						-88f,
						-82f,
						-82f,
						-89f,
						-94f,
						-100f,
						-108f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-101f,
						-96f,
						-90f,
						-83f,
						-77f,
						-54f,
						-43f,
						-38f,
						-50f,
						-48f,
						-52f,
						-48f,
						-42f,
						-42f,
						-51f,
						-52f,
						-53f,
						-59f,
						-65f,
						-71f,
						-78f,
						-85f,
						-95f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-120f,
						-105f,
						-86f,
						-68f,
						-78f,
						-79f,
						-90f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-120f,
						-105f,
						-86f,
						-66f,
						-73f,
						-77f,
						-88f,
						-96f,
						-105f,
						-115f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-120f,
						-105f,
						-92f,
						-80f,
						-61f,
						-64f,
						-68f,
						-80f,
						-87f,
						-92f,
						-100f,
						-110f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-120f,
						-104f,
						-91f,
						-79f,
						-52f,
						-60f,
						-54f,
						-64f,
						-69f,
						-77f,
						-80f,
						-82f,
						-84f,
						-85f,
						-87f,
						-88f,
						-90f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-118f,
						-100f,
						-87f,
						-77f,
						-49f,
						-50f,
						-44f,
						-58f,
						-61f,
						-61f,
						-67f,
						-65f,
						-62f,
						-62f,
						-62f,
						-65f,
						-68f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-115f,
						-98f,
						-84f,
						-62f,
						-49f,
						-44f,
						-38f,
						-46f,
						-49f,
						-49f,
						-46f,
						-39f,
						-37f,
						-39f,
						-40f,
						-42f,
						-43f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-88f,
						-74f,
						-77f,
						-82f,
						-82f,
						-85f,
						-90f,
						-94f,
						-99f,
						-104f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-88f,
						-66f,
						-70f,
						-81f,
						-80f,
						-81f,
						-84f,
						-88f,
						-91f,
						-93f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-88f,
						-61f,
						-63f,
						-70f,
						-71f,
						-74f,
						-77f,
						-80f,
						-83f,
						-85f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-86f,
						-62f,
						-63f,
						-62f,
						-62f,
						-58f,
						-52f,
						-50f,
						-50f,
						-52f,
						-54f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-118f,
						-108f,
						-84f,
						-53f,
						-50f,
						-50f,
						-50f,
						-55f,
						-47f,
						-45f,
						-40f,
						-40f,
						-40f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-118f,
						-100f,
						-73f,
						-43f,
						-37f,
						-42f,
						-43f,
						-53f,
						-38f,
						-37f,
						-35f,
						-35f,
						-38f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				},
				new float[][]
				{
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-91f,
						-84f,
						-74f,
						-80f,
						-80f,
						-80f,
						-80f,
						-80f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-91f,
						-84f,
						-74f,
						-68f,
						-68f,
						-68f,
						-68f,
						-68f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-86f,
						-78f,
						-70f,
						-60f,
						-45f,
						-30f,
						-21f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-87f,
						-78f,
						-67f,
						-48f,
						-38f,
						-29f,
						-21f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-86f,
						-69f,
						-56f,
						-45f,
						-35f,
						-33f,
						-29f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					},
					new float[]
					{
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-110f,
						-100f,
						-83f,
						-71f,
						-48f,
						-27f,
						-38f,
						-37f,
						-34f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f,
						-999f
					}
				}
			};
			Xiph.stereo_threshholds = new double[]
			{
				0.0,
				0.5,
				1.0,
				1.5,
				2.5,
				4.5,
				8.5,
				16.5,
				90000000000.0
			};
			Xiph.stereo_threshholds_limited = new double[]
			{
				0.0,
				0.5,
				1.0,
				1.5,
				2.0,
				2.5,
				4.5,
				8.5,
				90000000000.0
			};
			Xiph.__floor_P = null;
			Xiph.__residue_P = null;
			Xiph.__mapping_P = null;
			Xiph.residue0_exportbundle = new Xiph.vorbis_func_residue
			{
				pack = null,
				unpack = new Xiph.vorbis_func_residue.Unpack(Xiph.res0_unpack),
				look = new Xiph.vorbis_func_residue.Look(Xiph.res0_look),
				free_info = new Xiph.vorbis_func_residue.FreeInfo(Xiph.res0_free_info),
				free_look = new Xiph.vorbis_func_residue.FreeLook(Xiph.res0_free_look),
				_class = null,
				forward = null,
				inverse = new Xiph.vorbis_func_residue.Inverse(Xiph.res0_inverse)
			};
			Xiph.residue1_exportbundle = new Xiph.vorbis_func_residue
			{
				pack = new Xiph.vorbis_func_residue.Pack(Xiph.res0_pack),
				unpack = new Xiph.vorbis_func_residue.Unpack(Xiph.res0_unpack),
				look = new Xiph.vorbis_func_residue.Look(Xiph.res0_look),
				free_info = new Xiph.vorbis_func_residue.FreeInfo(Xiph.res0_free_info),
				free_look = new Xiph.vorbis_func_residue.FreeLook(Xiph.res0_free_look),
				_class = new Xiph.vorbis_func_residue.Class(Xiph.res1_class),
				forward = new Xiph.vorbis_func_residue.Forward(Xiph.res1_forward),
				inverse = new Xiph.vorbis_func_residue.Inverse(Xiph.res1_inverse)
			};
			Xiph.residue2_exportbundle = new Xiph.vorbis_func_residue
			{
				pack = new Xiph.vorbis_func_residue.Pack(Xiph.res0_pack),
				unpack = new Xiph.vorbis_func_residue.Unpack(Xiph.res0_unpack),
				look = new Xiph.vorbis_func_residue.Look(Xiph.res0_look),
				free_info = new Xiph.vorbis_func_residue.FreeInfo(Xiph.res0_free_info),
				free_look = new Xiph.vorbis_func_residue.FreeLook(Xiph.res0_free_look),
				_class = new Xiph.vorbis_func_residue.Class(Xiph.res2_class),
				forward = new Xiph.vorbis_func_residue.Forward(Xiph.res2_forward),
				inverse = new Xiph.vorbis_func_residue.Inverse(Xiph.res2_inverse)
			};
			Xiph.ntryh = new int[]
			{
				4,
				2,
				3,
				5
			};
			Xiph.vwin64 = new float[]
			{
				0.0009460463f,
				0.008500647f,
				0.0235352255f,
				0.0458950549f,
				0.07533519f,
				0.111507304f,
				0.1539458f,
				0.202055752f,
				0.255105674f,
				0.312227666f,
				0.372427016f,
				0.434602767f,
				0.497579f,
				0.560146f,
				0.621108532f,
				0.6793383f,
				0.733825266f,
				0.7837246f,
				0.828393936f,
				0.867418647f,
				0.900622249f,
				0.9280615f,
				0.9500073f,
				0.966913164f,
				0.979374051f,
				0.9880793f,
				0.9937636f,
				0.9971583f,
				0.998946249f,
				0.999723f,
				0.9999639f,
				0.9999995f
			};
			Xiph.vwin128 = new float[]
			{
				0.0002365472f,
				0.00212806859f,
				0.00590652553f,
				0.0115626547f,
				0.0190823451f,
				0.0284463726f,
				0.0396300927f,
				0.0526030436f,
				0.06732853f,
				0.0837631747f,
				0.101856485f,
				0.121550411f,
				0.142778933f,
				0.1654678f,
				0.1895342f,
				0.21488671f,
				0.241425261f,
				0.2690412f,
				0.2976178f,
				0.3270304f,
				0.357147336f,
				0.387830615f,
				0.418936938f,
				0.4503188f,
				0.481825918f,
				0.513306439f,
				0.544608653f,
				0.5755826f,
				0.6060816f,
				0.635964f,
				0.665094733f,
				0.693347037f,
				0.7206038f,
				0.746759f,
				0.77171874f,
				0.795402467f,
				0.8177436f,
				0.8386903f,
				0.8582054f,
				0.876266956f,
				0.8928678f,
				0.9080153f,
				0.921730638f,
				0.934048057f,
				0.9450138f,
				0.9546851f,
				0.9631287f,
				0.9704194f,
				0.976639f,
				0.9818741f,
				0.9862152f,
				0.9897546f,
				0.992585242f,
				0.9947991f,
				0.9964857f,
				0.997730851f,
				0.9986155f,
				0.9992144f,
				0.999595344f,
				0.9998179f,
				0.9999331f,
				0.999982536f,
				0.999997735f,
				1f
			};
			Xiph.vwin256 = new float[]
			{
				5.9139E-05f,
				0.0005321979f,
				0.00147803011f,
				0.0028960635f,
				0.004785436f,
				0.00714499271f,
				0.009973277f,
				0.0132685294f,
				0.0170286745f,
				0.0212513115f,
				0.02593371f,
				0.0310727954f,
				0.03666513f,
				0.0427069142f,
				0.04919396f,
				0.0561216921f,
				0.06348511f,
				0.0712788f,
				0.07949691f,
				0.08813314f,
				0.0971807f,
				0.106632352f,
				0.116480343f,
				0.126716435f,
				0.137331858f,
				0.148317337f,
				0.159663051f,
				0.171358675f,
				0.1833933f,
				0.195755512f,
				0.208433345f,
				0.221414253f,
				0.234685227f,
				0.248232663f,
				0.262042463f,
				0.276100039f,
				0.290390283f,
				0.3048976f,
				0.319605947f,
				0.334498882f,
				0.349559516f,
				0.3647706f,
				0.380114466f,
				0.395573229f,
				0.4111287f,
				0.4267624f,
				0.4424557f,
				0.458189756f,
				0.473945677f,
				0.48970446f,
				0.5054471f,
				0.5211546f,
				0.5368081f,
				0.5523887f,
				0.567878f,
				0.583257556f,
				0.598509252f,
				0.613615453f,
				0.628558755f,
				0.6433223f,
				0.6578896f,
				0.6722449f,
				0.686372936f,
				0.7002589f,
				0.713888943f,
				0.727249742f,
				0.7403288f,
				0.753114343f,
				0.7655955f,
				0.7777621f,
				0.789605f,
				0.8011159f,
				0.8122873f,
				0.8231127f,
				0.833586633f,
				0.8437044f,
				0.8534623f,
				0.8628576f,
				0.871888459f,
				0.8805541f,
				0.8888544f,
				0.8967904f,
				0.904363751f,
				0.9115773f,
				0.918434441f,
				0.924939454f,
				0.931097448f,
				0.936914146f,
				0.942396164f,
				0.947550535f,
				0.9523851f,
				0.9569083f,
				0.9611289f,
				0.96505636f,
				0.968700469f,
				0.9720714f,
				0.975179851f,
				0.9780366f,
				0.98065275f,
				0.9830396f,
				0.9852087f,
				0.9871716f,
				0.9889398f,
				0.990525067f,
				0.991939f,
				0.993193f,
				0.9942985f,
				0.995266736f,
				0.9961087f,
				0.9968351f,
				0.997456431f,
				0.9979828f,
				0.998423934f,
				0.998789251f,
				0.999087632f,
				0.9993276f,
				0.999517143f,
				0.999663651f,
				0.999774158f,
				0.999855f,
				0.999911964f,
				0.9999503f,
				0.9999745f,
				0.999988556f,
				0.9999958f,
				0.9999989f,
				0.9999999f,
				1f
			};
			Xiph.vwin512 = new float[]
			{
				1.47849E-05f,
				0.0001330607f,
				0.0003695946f,
				0.0007243509f,
				0.00119727594f,
				0.00178829825f,
				0.00249732845f,
				0.00332425884f,
				0.00426896336f,
				0.00533129741f,
				0.00651109824f,
				0.00780818425f,
				0.009222354f,
				0.0107533876f,
				0.0124010462f,
				0.01416507f,
				0.01604518f,
				0.0180410761f,
				0.0201524366f,
				0.0223789234f,
				0.0247201715f,
				0.0271757953f,
				0.02974539f,
				0.03242853f,
				0.0352247544f,
				0.0381336f,
				0.041154556f,
				0.0442871042f,
				0.0475307f,
				0.05088477f,
				0.05434871f,
				0.0579219051f,
				0.0616037f,
				0.06539342f,
				0.0692903548f,
				0.07329378f,
				0.0774029344f,
				0.08161703f,
				0.08593525f,
				0.0903567448f,
				0.09488064f,
				0.09950603f,
				0.104231969f,
				0.109057508f,
				0.113981627f,
				0.119003311f,
				0.124121495f,
				0.129335076f,
				0.134642929f,
				0.1400439f,
				0.1455368f,
				0.15112038f,
				0.156793416f,
				0.162554577f,
				0.168402553f,
				0.174335986f,
				0.180353478f,
				0.186453611f,
				0.1926349f,
				0.198895872f,
				0.205234975f,
				0.211650655f,
				0.218141317f,
				0.224705338f,
				0.231341034f,
				0.2380467f,
				0.244820654f,
				0.2516611f,
				0.258566231f,
				0.265534222f,
				0.272563249f,
				0.2796514f,
				0.286796749f,
				0.293997377f,
				0.3012513f,
				0.308556467f,
				0.3159109f,
				0.323312551f,
				0.330759257f,
				0.338249f,
				0.345779568f,
				0.353348851f,
				0.360954672f,
				0.3685948f,
				0.376267016f,
				0.3839691f,
				0.391698778f,
				0.399453759f,
				0.407231778f,
				0.4150305f,
				0.422847658f,
				0.430680871f,
				0.438527822f,
				0.446386129f,
				0.454253465f,
				0.462127447f,
				0.4700057f,
				0.477885872f,
				0.485765547f,
				0.4936424f,
				0.501514f,
				0.509378f,
				0.51723206f,
				0.525073767f,
				0.5329008f,
				0.5407108f,
				0.548501432f,
				0.556270361f,
				0.564015269f,
				0.5717339f,
				0.579423964f,
				0.587083161f,
				0.5947093f,
				0.6023001f,
				0.6098535f,
				0.6173672f,
				0.6248391f,
				0.6322671f,
				0.639649153f,
				0.646983147f,
				0.6542671f,
				0.6614989f,
				0.668676853f,
				0.675798833f,
				0.682863057f,
				0.6898677f,
				0.6968109f,
				0.703690946f,
				0.7105062f,
				0.7172549f,
				0.7239355f,
				0.7305464f,
				0.7370861f,
				0.743553162f,
				0.7499461f,
				0.7562636f,
				0.762504339f,
				0.768667042f,
				0.7747505f,
				0.780753553f,
				0.7866751f,
				0.7925142f,
				0.798269749f,
				0.803940833f,
				0.8095266f,
				0.815026343f,
				0.8204391f,
				0.825764358f,
				0.8310014f,
				0.836149633f,
				0.8412086f,
				0.8461777f,
				0.8510567f,
				0.8558452f,
				0.8605429f,
				0.865149558f,
				0.869664967f,
				0.8740891f,
				0.878421962f,
				0.882663369f,
				0.8868135f,
				0.8908725f,
				0.8948404f,
				0.898717642f,
				0.902504265f,
				0.906200767f,
				0.9098075f,
				0.913324833f,
				0.916753352f,
				0.920093536f,
				0.9233459f,
				0.9265113f,
				0.929590166f,
				0.9325834f,
				0.9354916f,
				0.938315749f,
				0.941056669f,
				0.943715155f,
				0.9462922f,
				0.9487889f,
				0.951206f,
				0.9535448f,
				0.955806255f,
				0.9579915f,
				0.9601016f,
				0.9621379f,
				0.964101434f,
				0.9659935f,
				0.9678154f,
				0.9695684f,
				0.9712537f,
				0.972872734f,
				0.974426746f,
				0.9759172f,
				0.9773454f,
				0.978712738f,
				0.980020642f,
				0.9812705f,
				0.9824638f,
				0.983601868f,
				0.984686255f,
				0.9857183f,
				0.9866996f,
				0.98763144f,
				0.9885154f,
				0.9893528f,
				0.990145266f,
				0.9908942f,
				0.991600931f,
				0.992267f,
				0.9928939f,
				0.9934828f,
				0.9940354f,
				0.994553f,
				0.99503696f,
				0.995488644f,
				0.995909452f,
				0.9963007f,
				0.996663749f,
				0.9969999f,
				0.9973104f,
				0.997596562f,
				0.997859538f,
				0.9981007f,
				0.9983211f,
				0.998522043f,
				0.998704553f,
				0.9988698f,
				0.999018848f,
				0.9991528f,
				0.999272645f,
				0.9993794f,
				0.999474049f,
				0.9995575f,
				0.9996307f,
				0.9996944f,
				0.999749541f,
				0.9997969f,
				0.9998372f,
				0.999871254f,
				0.9998997f,
				0.9999231f,
				0.9999422f,
				0.9999575f,
				0.999969542f,
				0.9999788f,
				0.9999858f,
				0.9999909f,
				0.999994457f,
				0.9999969f,
				0.9999984f,
				0.9999993f,
				0.999999762f,
				0.99999994f,
				1f,
				1f
			};
			Xiph.vwin1024 = new float[]
			{
				3.6962E-06f,
				3.32659E-05f,
				9.24041E-05f,
				0.0001811086f,
				0.0002993761f,
				0.0004472021f,
				0.0006245811f,
				0.0008315063f,
				0.00106797f,
				0.00133396313f,
				0.00162947574f,
				0.00195449661f,
				0.00230901339f,
				0.00269301259f,
				0.00310647977f,
				0.00354939885f,
				0.004021753f,
				0.004523525f,
				0.0050546946f,
				0.005615242f,
				0.00620514527f,
				0.00682438165f,
				0.00747292768f,
				0.008150758f,
				0.008857846f,
				0.009594166f,
				0.0103596859f,
				0.0111543788f,
				0.0119782118f,
				0.0128311533f,
				0.01371317f,
				0.0146242259f,
				0.0155642852f,
				0.0165333115f,
				0.0175312646f,
				0.0185581036f,
				0.01961379f,
				0.02069828f,
				0.0218115281f,
				0.0229534917f,
				0.0241241213f,
				0.02532337f,
				0.0265511889f,
				0.0278075263f,
				0.02909233f,
				0.0304055475f,
				0.0317471251f,
				0.033117f,
				0.0345151238f,
				0.0359414257f,
				0.0373958573f,
				0.038878344f,
				0.0403888337f,
				0.041927252f,
				0.0434935354f,
				0.0450876132f,
				0.046709422f,
				0.0483588837f,
				0.0500359274f,
				0.051740475f,
				0.05347246f,
				0.05523179f,
				0.0570184f,
				0.0588322f,
				0.0606731065f,
				0.06254104f,
				0.06443591f,
				0.06635763f,
				0.06830611f,
				0.07028126f,
				0.0722829849f,
				0.07431119f,
				0.07636578f,
				0.07844665f,
				0.08055371f,
				0.08268686f,
				0.084845975f,
				0.08703098f,
				0.0892417356f,
				0.0914781541f,
				0.09374011f,
				0.09602751f,
				0.09834021f,
				0.100678124f,
				0.103041112f,
				0.105429053f,
				0.107841842f,
				0.110279337f,
				0.112741411f,
				0.115227945f,
				0.117738806f,
				0.120273851f,
				0.122832961f,
				0.125416f,
				0.1280228f,
				0.130653247f,
				0.133307189f,
				0.1359845f,
				0.138685f,
				0.141408563f,
				0.144155025f,
				0.146924242f,
				0.149716049f,
				0.152530313f,
				0.155366838f,
				0.158225492f,
				0.1611061f,
				0.164008483f,
				0.1669325f,
				0.169877961f,
				0.1728447f,
				0.17583254f,
				0.178841308f,
				0.181870818f,
				0.1849209f,
				0.187991381f,
				0.191082045f,
				0.194192737f,
				0.197323233f,
				0.200473383f,
				0.203642949f,
				0.206831768f,
				0.210039645f,
				0.213266358f,
				0.216511711f,
				0.219775513f,
				0.223057538f,
				0.2263576f,
				0.229675472f,
				0.233010948f,
				0.236363828f,
				0.23973386f,
				0.243120864f,
				0.246524587f,
				0.249944836f,
				0.253381371f,
				0.256833971f,
				0.2603024f,
				0.263786435f,
				0.267285824f,
				0.270800382f,
				0.2743298f,
				0.277873933f,
				0.28143245f,
				0.285005152f,
				0.2885918f,
				0.292192161f,
				0.295805931f,
				0.299432933f,
				0.30307284f,
				0.306725472f,
				0.310390532f,
				0.3140678f,
				0.317756981f,
				0.321457833f,
				0.32517007f,
				0.328893483f,
				0.332627743f,
				0.336372644f,
				0.3401279f,
				0.34389323f,
				0.34766835f,
				0.351453036f,
				0.355246961f,
				0.359049916f,
				0.362861574f,
				0.366681665f,
				0.370509923f,
				0.374346077f,
				0.378189832f,
				0.3820409f,
				0.385899f,
				0.3897639f,
				0.3936352f,
				0.397512764f,
				0.4013962f,
				0.405285239f,
				0.4091796f,
				0.413079023f,
				0.4169832f,
				0.4208918f,
				0.424804568f,
				0.4287212f,
				0.432641417f,
				0.436564922f,
				0.4404914f,
				0.4444206f,
				0.448352218f,
				0.452285916f,
				0.456221431f,
				0.460158467f,
				0.4640967f,
				0.468035877f,
				0.471975654f,
				0.47591576f,
				0.479855925f,
				0.4837958f,
				0.487735122f,
				0.4916736f,
				0.495610863f,
				0.4995467f,
				0.5034808f,
				0.507412851f,
				0.5113425f,
				0.515269637f,
				0.519193769f,
				0.5231146f,
				0.527032f,
				0.530945539f,
				0.534855f,
				0.53876f,
				0.542660356f,
				0.5465557f,
				0.5504458f,
				0.5543303f,
				0.558208942f,
				0.5620814f,
				0.5659475f,
				0.5698068f,
				0.5736592f,
				0.5775042f,
				0.581341743f,
				0.585171342f,
				0.5889929f,
				0.592806f,
				0.596610367f,
				0.6004058f,
				0.6041921f,
				0.607968748f,
				0.6117357f,
				0.6154926f,
				0.619239151f,
				0.6229752f,
				0.6267003f,
				0.630414367f,
				0.634117f,
				0.6378081f,
				0.641487241f,
				0.645154238f,
				0.6488089f,
				0.65245086f,
				0.656079948f,
				0.6596959f,
				0.663298547f,
				0.6668875f,
				0.670462668f,
				0.6740237f,
				0.677570343f,
				0.6811025f,
				0.684619844f,
				0.6881222f,
				0.691609263f,
				0.695080936f,
				0.6985369f,
				0.701976955f,
				0.7054009f,
				0.708808541f,
				0.7121997f,
				0.715574f,
				0.7189315f,
				0.7222718f,
				0.7255948f,
				0.7289003f,
				0.7321881f,
				0.735457957f,
				0.738709748f,
				0.7419433f,
				0.7451584f,
				0.7483549f,
				0.7515326f,
				0.754691362f,
				0.757831037f,
				0.7609514f,
				0.764052331f,
				0.7671337f,
				0.770195365f,
				0.7732371f,
				0.7762588f,
				0.7792604f,
				0.782241642f,
				0.785202444f,
				0.7881427f,
				0.791062236f,
				0.7939609f,
				0.7968387f,
				0.799695432f,
				0.802531f,
				0.8053453f,
				0.808138251f,
				0.8109097f,
				0.8136595f,
				0.8163877f,
				0.8190941f,
				0.821778655f,
				0.8244413f,
				0.8270819f,
				0.8297004f,
				0.8322968f,
				0.834870934f,
				0.8374227f,
				0.83995223f,
				0.842459261f,
				0.8449439f,
				0.8474059f,
				0.8498454f,
				0.8522623f,
				0.8546566f,
				0.8570281f,
				0.859376967f,
				0.8617031f,
				0.8640064f,
				0.866286933f,
				0.8685447f,
				0.8707796f,
				0.8729917f,
				0.8751809f,
				0.8773473f,
				0.879490852f,
				0.8816116f,
				0.8837095f,
				0.885784566f,
				0.8878368f,
				0.8898663f,
				0.891873f,
				0.893857f,
				0.895818233f,
				0.8977568f,
				0.899672747f,
				0.9015661f,
				0.90343684f,
				0.90528506f,
				0.90711087f,
				0.9089142f,
				0.9106952f,
				0.91245383f,
				0.9141903f,
				0.9159045f,
				0.917596638f,
				0.9192667f,
				0.9209148f,
				0.922541f,
				0.9241454f,
				0.9257281f,
				0.927289069f,
				0.9288285f,
				0.9303465f,
				0.931843042f,
				0.9333184f,
				0.9347725f,
				0.9362055f,
				0.9376176f,
				0.9390088f,
				0.9403792f,
				0.94172895f,
				0.9430582f,
				0.944367051f,
				0.9456555f,
				0.9469239f,
				0.9481722f,
				0.949400544f,
				0.950609148f,
				0.951798f,
				0.9529674f,
				0.954117358f,
				0.955248058f,
				0.9563596f,
				0.957452238f,
				0.958525956f,
				0.959581f,
				0.960617542f,
				0.961635649f,
				0.9626355f,
				0.963617265f,
				0.964581132f,
				0.9655271f,
				0.9664555f,
				0.967366457f,
				0.96826005f,
				0.969136536f,
				0.969996f,
				0.970838666f,
				0.9716646f,
				0.9724741f,
				0.973267257f,
				0.974044263f,
				0.9748053f,
				0.9755505f,
				0.976280034f,
				0.9769941f,
				0.977692842f,
				0.9783765f,
				0.9790452f,
				0.979699135f,
				0.980338454f,
				0.9809634f,
				0.981574059f,
				0.9821707f,
				0.9827534f,
				0.983322442f,
				0.983877957f,
				0.9844201f,
				0.9849491f,
				0.9854651f,
				0.985968351f,
				0.986458957f,
				0.9869371f,
				0.987403035f,
				0.987856865f,
				0.9882988f,
				0.98872906f,
				0.9891477f,
				0.9895551f,
				0.9899513f,
				0.990336537f,
				0.990711f,
				0.991074741f,
				0.991428137f,
				0.9917712f,
				0.992104232f,
				0.992427349f,
				0.99274075f,
				0.9930446f,
				0.993339062f,
				0.9936244f,
				0.993900657f,
				0.994168043f,
				0.994426847f,
				0.9946771f,
				0.994919062f,
				0.995152831f,
				0.9953787f,
				0.9955967f,
				0.9958071f,
				0.99601f,
				0.9962056f,
				0.9963941f,
				0.9965756f,
				0.9967503f,
				0.9969183f,
				0.997079849f,
				0.9972351f,
				0.9973842f,
				0.997527242f,
				0.9976644f,
				0.9977959f,
				0.9979218f,
				0.9980424f,
				0.9981577f,
				0.9982679f,
				0.998373151f,
				0.9984736f,
				0.9985693f,
				0.998660564f,
				0.9987474f,
				0.99883f,
				0.9989085f,
				0.998983f,
				0.9990537f,
				0.999120653f,
				0.999184f,
				0.999244f,
				0.999300539f,
				0.9993539f,
				0.9994042f,
				0.999451458f,
				0.9994959f,
				0.999537647f,
				0.999576747f,
				0.9996133f,
				0.999647439f,
				0.9996793f,
				0.999708951f,
				0.999736547f,
				0.9997621f,
				0.9997858f,
				0.999807656f,
				0.9998278f,
				0.999846339f,
				0.9998633f,
				0.9998789f,
				0.999893069f,
				0.999905944f,
				0.9999177f,
				0.999928236f,
				0.9999378f,
				0.999946356f,
				0.999954f,
				0.9999608f,
				0.9999668f,
				0.9999721f,
				0.9999767f,
				0.999980748f,
				0.9999842f,
				0.999987245f,
				0.999989748f,
				0.9999919f,
				0.9999937f,
				0.9999952f,
				0.999996364f,
				0.9999973f,
				0.9999981f,
				0.9999987f,
				0.9999991f,
				0.9999994f,
				0.999999642f,
				0.9999998f,
				0.9999999f,
				0.99999994f,
				1f,
				1f,
				1f,
				1f
			};
			Xiph.vwin2048 = new float[]
			{
				9.241E-07f,
				8.3165E-06f,
				2.31014E-05f,
				4.52785E-05f,
				7.48476E-05f,
				0.0001118085f,
				0.0001561608f,
				0.0002079041f,
				0.0002670379f,
				0.0003335617f,
				0.0004074748f,
				0.0004887765f,
				0.0005774661f,
				0.0006735427f,
				0.0007770054f,
				0.0008878533f,
				0.00100608531f,
				0.00113170024f,
				0.00126469694f,
				0.00140507414f,
				0.00155283068f,
				0.001707965f,
				0.00187047559f,
				0.002040361f,
				0.00221761968f,
				0.00240224972f,
				0.00259424956f,
				0.00279361731f,
				0.00300035113f,
				0.003214449f,
				0.00343590882f,
				0.00366472849f,
				0.003900906f,
				0.004144439f,
				0.004395325f,
				0.00465356233f,
				0.004919147f,
				0.005192078f,
				0.005472352f,
				0.00575996656f,
				0.00605491828f,
				0.006357205f,
				0.00666682376f,
				0.00698377145f,
				0.007308045f,
				0.007639641f,
				0.007978557f,
				0.008324788f,
				0.008678333f,
				0.009039187f,
				0.009407347f,
				0.009782809f,
				0.01016557f,
				0.0105556259f,
				0.0109529728f,
				0.0113576064f,
				0.0117695238f,
				0.01218872f,
				0.0126151918f,
				0.0130489338f,
				0.0134899421f,
				0.0139382128f,
				0.0143937413f,
				0.0148565229f,
				0.015326554f,
				0.015803827f,
				0.016288342f,
				0.01678009f,
				0.0172790661f,
				0.0177852679f,
				0.0182986874f,
				0.0188193228f,
				0.0193471666f,
				0.0198822133f,
				0.02042446f,
				0.0209738966f,
				0.021530522f,
				0.02209433f,
				0.02266531f,
				0.0232434627f,
				0.0238287784f,
				0.0244212523f,
				0.0250208769f,
				0.0256276485f,
				0.0262415577f,
				0.0268626f,
				0.02749077f,
				0.028126061f,
				0.0287684631f,
				0.029417973f,
				0.0300745834f,
				0.0307382867f,
				0.0314090736f,
				0.0320869423f,
				0.03277188f,
				0.0334638841f,
				0.034162946f,
				0.03486906f,
				0.03558221f,
				0.0363024f,
				0.0370296165f,
				0.03776385f,
				0.038505096f,
				0.0392533466f,
				0.04000859f,
				0.04077082f,
				0.04154003f,
				0.0423162133f,
				0.043099355f,
				0.04388945f,
				0.0446864925f,
				0.04549047f,
				0.0463013723f,
				0.0471191965f,
				0.0479439273f,
				0.04877556f,
				0.049614083f,
				0.05045949f,
				0.0513117649f,
				0.0521709025f,
				0.0530368946f,
				0.05390973f,
				0.0547893979f,
				0.05567589f,
				0.0565691926f,
				0.0574693f,
				0.0583762042f,
				0.0592898875f,
				0.06021034f,
				0.061137557f,
				0.0620715246f,
				0.0630122349f,
				0.0639596656f,
				0.0649138242f,
				0.06587469f,
				0.06684224f,
				0.06781648f,
				0.0687974f,
				0.06978498f,
				0.0707792f,
				0.07178006f,
				0.07278755f,
				0.07380166f,
				0.0748223662f,
				0.07584966f,
				0.07688354f,
				0.0779239759f,
				0.07897097f,
				0.0800244957f,
				0.08108456f,
				0.08215113f,
				0.08322421f,
				0.08430377f,
				0.08538981f,
				0.0864823f,
				0.0875812545f,
				0.08868664f,
				0.0897984356f,
				0.09091665f,
				0.092041254f,
				0.09317224f,
				0.09430958f,
				0.09545328f,
				0.09660331f,
				0.09775967f,
				0.0989223346f,
				0.100091286f,
				0.101266526f,
				0.102448016f,
				0.103635758f,
				0.104829736f,
				0.106029928f,
				0.107236326f,
				0.1084489f,
				0.109667651f,
				0.110892557f,
				0.112123594f,
				0.113360755f,
				0.114604026f,
				0.115853384f,
				0.117108814f,
				0.1183703f,
				0.119637832f,
				0.120911382f,
				0.122190937f,
				0.123476483f,
				0.124768f,
				0.126065463f,
				0.127368867f,
				0.1286782f,
				0.129993424f,
				0.131314531f,
				0.132641509f,
				0.133974329f,
				0.135312989f,
				0.136657447f,
				0.1380077f,
				0.139363736f,
				0.140725508f,
				0.142093033f,
				0.143466264f,
				0.1448452f,
				0.146229818f,
				0.147620082f,
				0.149016f,
				0.150417522f,
				0.151824653f,
				0.153237358f,
				0.15465562f,
				0.156079426f,
				0.157508761f,
				0.1589436f,
				0.1603839f,
				0.16182965f,
				0.163280845f,
				0.164737463f,
				0.166199476f,
				0.167666852f,
				0.1691396f,
				0.170617655f,
				0.172101021f,
				0.173589677f,
				0.1750836f,
				0.176582769f,
				0.17808716f,
				0.179596752f,
				0.181111515f,
				0.182631418f,
				0.184156463f,
				0.185686618f,
				0.187221855f,
				0.188762158f,
				0.1903075f,
				0.191857845f,
				0.1934132f,
				0.194973513f,
				0.196538761f,
				0.198108941f,
				0.199684009f,
				0.201263949f,
				0.202848747f,
				0.204438359f,
				0.206032783f,
				0.207631961f,
				0.2092359f,
				0.210844561f,
				0.212457925f,
				0.214075953f,
				0.21569863f,
				0.217325941f,
				0.218957841f,
				0.220594317f,
				0.222235337f,
				0.223880872f,
				0.225530908f,
				0.227185413f,
				0.228844345f,
				0.2305077f,
				0.23217544f,
				0.233847544f,
				0.235523984f,
				0.237204731f,
				0.238889754f,
				0.240579039f,
				0.242272541f,
				0.24397023f,
				0.2456721f,
				0.247378111f,
				0.249088243f,
				0.250802457f,
				0.2525207f,
				0.254243016f,
				0.255969316f,
				0.2576996f,
				0.2594338f,
				0.261171937f,
				0.262913972f,
				0.264659852f,
				0.266409546f,
				0.268163055f,
				0.269920319f,
				0.271681339f,
				0.273446083f,
				0.275214463f,
				0.276986539f,
				0.278762221f,
				0.28054148f,
				0.2823243f,
				0.284110665f,
				0.285900533f,
				0.287693858f,
				0.2894906f,
				0.2912908f,
				0.293094337f,
				0.294901252f,
				0.296711445f,
				0.298524946f,
				0.3003417f,
				0.302161664f,
				0.303984821f,
				0.305811137f,
				0.307640582f,
				0.309473127f,
				0.3113087f,
				0.313147366f,
				0.314988971f,
				0.3168336f,
				0.3186811f,
				0.320531517f,
				0.322384834f,
				0.324240953f,
				0.3260999f,
				0.3279616f,
				0.329826027f,
				0.331693172f,
				0.333563f,
				0.33543545f,
				0.3373105f,
				0.339188129f,
				0.3410683f,
				0.34295097f,
				0.3448361f,
				0.346723676f,
				0.34861365f,
				0.350505978f,
				0.352400661f,
				0.354297638f,
				0.35619688f,
				0.358098358f,
				0.360002f,
				0.36190784f,
				0.3638158f,
				0.365725875f,
				0.367637962f,
				0.3695521f,
				0.371468246f,
				0.373386323f,
				0.3753063f,
				0.377228171f,
				0.3791519f,
				0.381077468f,
				0.383004785f,
				0.384933829f,
				0.3868646f,
				0.388797045f,
				0.390731126f,
				0.3926668f,
				0.394604027f,
				0.396542817f,
				0.398483068f,
				0.4004248f,
				0.402367949f,
				0.404312462f,
				0.406258345f,
				0.408205539f,
				0.410154f,
				0.4121037f,
				0.414054632f,
				0.4160067f,
				0.417959929f,
				0.419914216f,
				0.4218696f,
				0.423825979f,
				0.425783366f,
				0.427741677f,
				0.429700941f,
				0.43166104f,
				0.433622f,
				0.43558377f,
				0.437546283f,
				0.439509541f,
				0.441473484f,
				0.443438083f,
				0.4454033f,
				0.4473691f,
				0.449335456f,
				0.45130232f,
				0.453269631f,
				0.4552374f,
				0.457205564f,
				0.459174067f,
				0.4611429f,
				0.463112026f,
				0.465081424f,
				0.467051f,
				0.469020754f,
				0.470990658f,
				0.472960651f,
				0.474930733f,
				0.476900816f,
				0.4788709f,
				0.480840921f,
				0.482810885f,
				0.4847807f,
				0.486750364f,
				0.488719821f,
				0.490689069f,
				0.492658019f,
				0.494626671f,
				0.496595f,
				0.4985629f,
				0.5005304f,
				0.5024975f,
				0.50446403f,
				0.50643003f,
				0.5083955f,
				0.51036036f,
				0.5123246f,
				0.5142881f,
				0.5162509f,
				0.518213f,
				0.520174265f,
				0.5221347f,
				0.524094343f,
				0.526053f,
				0.5280108f,
				0.529967546f,
				0.5319233f,
				0.533878f,
				0.5358317f,
				0.5377842f,
				0.539735556f,
				0.54168576f,
				0.543634653f,
				0.545582354f,
				0.547528744f,
				0.549473763f,
				0.5514174f,
				0.5533597f,
				0.5553005f,
				0.55723983f,
				0.559177637f,
				0.5611139f,
				0.563048542f,
				0.5649816f,
				0.566912949f,
				0.568842649f,
				0.570770562f,
				0.572696745f,
				0.574621141f,
				0.5765437f,
				0.5784643f,
				0.580383062f,
				0.5822999f,
				0.5842147f,
				0.5861275f,
				0.588038266f,
				0.5899469f,
				0.5918535f,
				0.5937579f,
				0.5956601f,
				0.5975601f,
				0.59945786f,
				0.6013533f,
				0.6032464f,
				0.605137169f,
				0.6070255f,
				0.608911455f,
				0.6107949f,
				0.6126759f,
				0.614554346f,
				0.6164302f,
				0.618303537f,
				0.62017417f,
				0.6220422f,
				0.623907447f,
				0.625770032f,
				0.6276299f,
				0.6294869f,
				0.6313411f,
				0.6331924f,
				0.6350409f,
				0.6368864f,
				0.638729f,
				0.640568554f,
				0.6424051f,
				0.644238651f,
				0.64606905f,
				0.6478964f,
				0.64972055f,
				0.6515416f,
				0.653359354f,
				0.6551739f,
				0.6569852f,
				0.6587932f,
				0.660597861f,
				0.6623992f,
				0.6641971f,
				0.665991545f,
				0.6677826f,
				0.669570148f,
				0.671354234f,
				0.673134744f,
				0.6749117f,
				0.676685035f,
				0.678454757f,
				0.680220842f,
				0.681983232f,
				0.6837419f,
				0.685496867f,
				0.687248051f,
				0.6889954f,
				0.690739f,
				0.692478657f,
				0.694214463f,
				0.6959464f,
				0.6976744f,
				0.6993984f,
				0.7011184f,
				0.7028345f,
				0.704546452f,
				0.706254363f,
				0.707958162f,
				0.7096579f,
				0.7113534f,
				0.7130448f,
				0.714732051f,
				0.716415f,
				0.718093753f,
				0.7197682f,
				0.721438348f,
				0.7231042f,
				0.7247657f,
				0.726422846f,
				0.728075564f,
				0.72972393f,
				0.7313678f,
				0.733007252f,
				0.734642148f,
				0.736272633f,
				0.7378985f,
				0.739519835f,
				0.7411366f,
				0.7427488f,
				0.744356334f,
				0.7459593f,
				0.7475575f,
				0.7491511f,
				0.750739932f,
				0.7523241f,
				0.753903449f,
				0.7554781f,
				0.7570479f,
				0.758612931f,
				0.760173142f,
				0.761728466f,
				0.763278961f,
				0.764824569f,
				0.76636523f,
				0.767901f,
				0.7694318f,
				0.770957649f,
				0.7724785f,
				0.7739944f,
				0.7755053f,
				0.7770111f,
				0.7785119f,
				0.7800076f,
				0.7814982f,
				0.7829837f,
				0.7844641f,
				0.7859394f,
				0.787409544f,
				0.7888745f,
				0.7903343f,
				0.7917889f,
				0.7932382f,
				0.7946823f,
				0.79612124f,
				0.7975549f,
				0.7989833f,
				0.800406337f,
				0.801824152f,
				0.8032366f,
				0.80464375f,
				0.806045532f,
				0.807442f,
				0.8088331f,
				0.8102188f,
				0.811599135f,
				0.8129741f,
				0.8143436f,
				0.8157077f,
				0.8170663f,
				0.8184196f,
				0.8197673f,
				0.8211096f,
				0.8224464f,
				0.8237777f,
				0.8251035f,
				0.8264238f,
				0.827738643f,
				0.829047859f,
				0.8303516f,
				0.8316498f,
				0.8329424f,
				0.834229469f,
				0.835510969f,
				0.836786866f,
				0.8380572f,
				0.839322f,
				0.8405811f,
				0.8418346f,
				0.843082547f,
				0.8443248f,
				0.8455615f,
				0.8467925f,
				0.848017931f,
				0.8492377f,
				0.850451767f,
				0.851660252f,
				0.852863f,
				0.8540601f,
				0.8552516f,
				0.8564374f,
				0.8576175f,
				0.8587919f,
				0.8599606f,
				0.8611237f,
				0.862281f,
				0.8634327f,
				0.864578664f,
				0.865718961f,
				0.866853535f,
				0.8679824f,
				0.8691055f,
				0.870223f,
				0.871334732f,
				0.8724408f,
				0.8735411f,
				0.874635756f,
				0.8757247f,
				0.876807868f,
				0.877885342f,
				0.8789571f,
				0.8800232f,
				0.881083548f,
				0.8821382f,
				0.8831871f,
				0.8842304f,
				0.8852679f,
				0.8862997f,
				0.8873259f,
				0.8883463f,
				0.889361f,
				0.8903701f,
				0.891373456f,
				0.8923711f,
				0.8933631f,
				0.8943494f,
				0.8953301f,
				0.896305f,
				0.8972743f,
				0.898237944f,
				0.8991959f,
				0.9001482f,
				0.901094854f,
				0.9020359f,
				0.902971268f,
				0.903901041f,
				0.904825151f,
				0.905743659f,
				0.9066565f,
				0.9075638f,
				0.908465445f,
				0.909361541f,
				0.910252035f,
				0.9111369f,
				0.9120163f,
				0.9128901f,
				0.9137583f,
				0.914620936f,
				0.915478051f,
				0.9163296f,
				0.9171757f,
				0.918016255f,
				0.918851256f,
				0.919680834f,
				0.920504868f,
				0.9213234f,
				0.922136545f,
				0.9229442f,
				0.923746347f,
				0.924543142f,
				0.925334454f,
				0.926120341f,
				0.926900864f,
				0.927675962f,
				0.928445637f,
				0.92921f,
				0.929969f,
				0.9307226f,
				0.931470931f,
				0.9322139f,
				0.9329515f,
				0.9336839f,
				0.9344109f,
				0.9351327f,
				0.935849249f,
				0.9365605f,
				0.9372665f,
				0.9379673f,
				0.9386629f,
				0.9393533f,
				0.9400385f,
				0.9407186f,
				0.941393435f,
				0.9420632f,
				0.9427278f,
				0.9433873f,
				0.9440417f,
				0.944691062f,
				0.9453353f,
				0.9459745f,
				0.9466087f,
				0.947237849f,
				0.947861969f,
				0.948481143f,
				0.9490953f,
				0.9497045f,
				0.950308859f,
				0.9509082f,
				0.9515026f,
				0.9520922f,
				0.9526769f,
				0.953256667f,
				0.9538317f,
				0.954401851f,
				0.9549672f,
				0.9555277f,
				0.956083536f,
				0.9566346f,
				0.957180858f,
				0.9577224f,
				0.9582593f,
				0.9587915f,
				0.959319f,
				0.9598419f,
				0.960360169f,
				0.9608738f,
				0.961382866f,
				0.96188736f,
				0.962387264f,
				0.962882638f,
				0.963373542f,
				0.9638599f,
				0.9643418f,
				0.964819252f,
				0.9652923f,
				0.9657609f,
				0.9662251f,
				0.9666849f,
				0.9671404f,
				0.967591465f,
				0.968038261f,
				0.968480766f,
				0.968919039f,
				0.969352961f,
				0.9697827f,
				0.9702082f,
				0.9706296f,
				0.9710467f,
				0.9714597f,
				0.9718685f,
				0.9722733f,
				0.972673953f,
				0.9730705f,
				0.973463f,
				0.9738515f,
				0.974236f,
				0.9746165f,
				0.97499305f,
				0.975365639f,
				0.9757343f,
				0.9760991f,
				0.97646f,
				0.976817f,
				0.9771702f,
				0.9775196f,
				0.9778652f,
				0.978207f,
				0.97854507f,
				0.978879452f,
				0.9792101f,
				0.97953707f,
				0.979860365f,
				0.98018f,
				0.980496049f,
				0.9808085f,
				0.981117368f,
				0.9814227f,
				0.98172456f,
				0.9820228f,
				0.9823176f,
				0.982609034f,
				0.9828969f,
				0.9831815f,
				0.9834626f,
				0.9837403f,
				0.9840147f,
				0.9842858f,
				0.9845536f,
				0.984818041f,
				0.9850793f,
				0.9853373f,
				0.9855921f,
				0.9858437f,
				0.98609215f,
				0.9863374f,
				0.986579657f,
				0.986818731f,
				0.9870547f,
				0.987287641f,
				0.9875176f,
				0.9877445f,
				0.987968445f,
				0.9881894f,
				0.988407433f,
				0.988622546f,
				0.9888348f,
				0.98904413f,
				0.98925066f,
				0.9894543f,
				0.9896552f,
				0.9898533f,
				0.990048647f,
				0.9902412f,
				0.99043113f,
				0.990618348f,
				0.9908029f,
				0.9909848f,
				0.9911641f,
				0.991340756f,
				0.991514862f,
				0.9916864f,
				0.991855443f,
				0.9920219f,
				0.99218595f,
				0.9923475f,
				0.9925066f,
				0.9926633f,
				0.9928176f,
				0.9929695f,
				0.993119061f,
				0.993266344f,
				0.993411243f,
				0.9935539f,
				0.993694246f,
				0.9938324f,
				0.9939683f,
				0.994102f,
				0.994233549f,
				0.99436295f,
				0.9944902f,
				0.9946153f,
				0.99473834f,
				0.994859338f,
				0.994978249f,
				0.995095134f,
				0.995210052f,
				0.995322943f,
				0.9954339f,
				0.995542943f,
				0.99565f,
				0.9957552f,
				0.9958585f,
				0.995959938f,
				0.9960596f,
				0.9961574f,
				0.9962534f,
				0.9963476f,
				0.9964401f,
				0.996530831f,
				0.9966199f,
				0.9967072f,
				0.9967929f,
				0.9968769f,
				0.9969593f,
				0.9970401f,
				0.997119248f,
				0.997196853f,
				0.997272968f,
				0.9973475f,
				0.9974205f,
				0.997492f,
				0.997562051f,
				0.997630656f,
				0.99769783f,
				0.9977636f,
				0.9978279f,
				0.9978909f,
				0.997952461f,
				0.9980128f,
				0.99807173f,
				0.998129368f,
				0.9981857f,
				0.9982408f,
				0.998294652f,
				0.9983473f,
				0.998398662f,
				0.9984489f,
				0.9984979f,
				0.9985458f,
				0.998592556f,
				0.998638153f,
				0.9986827f,
				0.9987261f,
				0.998768449f,
				0.998809755f,
				0.998850048f,
				0.998889267f,
				0.998927534f,
				0.9989648f,
				0.9990011f,
				0.9990364f,
				0.999070764f,
				0.999104261f,
				0.999136865f,
				0.9991685f,
				0.999199331f,
				0.9992293f,
				0.9992584f,
				0.9992867f,
				0.9993142f,
				0.999340832f,
				0.99936676f,
				0.9993919f,
				0.9994163f,
				0.999439955f,
				0.999462843f,
				0.9994851f,
				0.9995066f,
				0.999527454f,
				0.99954766f,
				0.9995672f,
				0.9995861f,
				0.9996044f,
				0.999622047f,
				0.999639153f,
				0.9996556f,
				0.9996716f,
				0.999686956f,
				0.999701738f,
				0.999716043f,
				0.9997298f,
				0.9997431f,
				0.9997559f,
				0.9997682f,
				0.99978f,
				0.9997914f,
				0.999802351f,
				0.999812841f,
				0.9998229f,
				0.9998326f,
				0.9998418f,
				0.9998507f,
				0.9998592f,
				0.9998673f,
				0.9998751f,
				0.9998825f,
				0.9998896f,
				0.9998964f,
				0.999902844f,
				0.999909f,
				0.9999149f,
				0.9999204f,
				0.999925733f,
				0.999930739f,
				0.9999355f,
				0.999940038f,
				0.99994427f,
				0.9999483f,
				0.999952137f,
				0.9999558f,
				0.9999592f,
				0.99996233f,
				0.99996537f,
				0.9999682f,
				0.9999708f,
				0.9999733f,
				0.9999756f,
				0.999977767f,
				0.9999798f,
				0.999981642f,
				0.99998343f,
				0.999985039f,
				0.9999865f,
				0.9999879f,
				0.999989152f,
				0.999990344f,
				0.9999914f,
				0.9999924f,
				0.999993265f,
				0.9999941f,
				0.9999948f,
				0.99999547f,
				0.9999961f,
				0.999996662f,
				0.999997139f,
				0.999997556f,
				0.9999979f,
				0.9999983f,
				0.999998569f,
				0.9999988f,
				0.999999f,
				0.9999992f,
				0.999999344f,
				0.9999995f,
				0.9999996f,
				0.9999997f,
				0.999999762f,
				0.9999998f,
				0.9999999f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
			Xiph.vwin4096 = new float[]
			{
				2.31E-07f,
				2.0791E-06f,
				5.7754E-06f,
				1.13197E-05f,
				1.87121E-05f,
				2.79526E-05f,
				3.90412E-05f,
				5.19777E-05f,
				6.67623E-05f,
				8.33949E-05f,
				0.0001018753f,
				0.0001222036f,
				0.0001443798f,
				0.0001684037f,
				0.0001942754f,
				0.0002219947f,
				0.0002515616f,
				0.0002829761f,
				0.000316238f,
				0.0003513472f,
				0.0003883038f,
				0.0004271076f,
				0.0004677584f,
				0.0005102563f,
				0.0005546011f,
				0.0006007928f,
				0.0006488311f,
				0.000698716f,
				0.0007504474f,
				0.0008040251f,
				0.000859449f,
				0.0009167191f,
				0.0009758351f,
				0.001036797f,
				0.00109960441f,
				0.00116425741f,
				0.00123075582f,
				0.00129909941f,
				0.001369288f,
				0.00144132157f,
				0.00151519978f,
				0.00159092259f,
				0.00166848977f,
				0.00174790109f,
				0.00182915654f,
				0.00191225566f,
				0.00199719821f,
				0.00208398444f,
				0.00217261375f,
				0.00226308615f,
				0.00235540117f,
				0.00244955881f,
				0.00254555885f,
				0.0026434008f,
				0.00274308468f,
				0.00284461025f,
				0.00294797728f,
				0.0030531853f,
				0.00316023431f,
				0.00326912384f,
				0.003379854f,
				0.0034924238f,
				0.00360683375f,
				0.0037230833f,
				0.00384117221f,
				0.0039611f,
				0.00408286648f,
				0.00420647161f,
				0.00433191471f,
				0.0044591953f,
				0.004588314f,
				0.00471926946f,
				0.004852062f,
				0.00498669129f,
				0.005123157f,
				0.00526145846f,
				0.0054015955f,
				0.00554356771f,
				0.00568737462f,
				0.00583301671f,
				0.00598049257f,
				0.00612980267f,
				0.006280946f,
				0.006433923f,
				0.006588732f,
				0.006745374f,
				0.00690384768f,
				0.007064153f,
				0.00722628972f,
				0.00739025744f,
				0.00755605567f,
				0.007723684f,
				0.007893141f,
				0.008064428f,
				0.008237544f,
				0.008412489f,
				0.008589261f,
				0.008767861f,
				0.008948289f,
				0.009130542f,
				0.009314623f,
				0.009500528f,
				0.009688259f,
				0.009877815f,
				0.0100691961f,
				0.0102624f,
				0.0104574282f,
				0.010654279f,
				0.0108529525f,
				0.0110534476f,
				0.0112557653f,
				0.0114599029f,
				0.0116658621f,
				0.01187364f,
				0.0120832389f,
				0.0122946557f,
				0.0125078913f,
				0.0127229458f,
				0.0129398163f,
				0.0131585049f,
				0.0133790094f,
				0.0136013292f,
				0.013825465f,
				0.014051415f,
				0.0142791793f,
				0.0145087568f,
				0.0147401486f,
				0.0149733517f,
				0.0152083663f,
				0.0154451933f,
				0.01568383f,
				0.0159242768f,
				0.0161665343f,
				0.0164106f,
				0.0166564733f,
				0.0169041548f,
				0.0171536449f,
				0.01740494f,
				0.01765804f,
				0.0179129466f,
				0.0181696564f,
				0.0184281711f,
				0.0186884888f,
				0.0189506076f,
				0.01921453f,
				0.0194802526f,
				0.0197477769f,
				0.0200171f,
				0.0202882234f,
				0.0205611456f,
				0.0208358634f,
				0.02111238f,
				0.0213906933f,
				0.0216708016f,
				0.0219527036f,
				0.0222364012f,
				0.0225218926f,
				0.0228091776f,
				0.0230982527f,
				0.02338912f,
				0.0236817785f,
				0.0239762254f,
				0.0242724624f,
				0.0245704874f,
				0.0248703f,
				0.0251719f,
				0.0254752859f,
				0.0257804561f,
				0.0260874126f,
				0.02639615f,
				0.0267066713f,
				0.0270189755f,
				0.02733306f,
				0.0276489258f,
				0.027966572f,
				0.0282859951f,
				0.0286071971f,
				0.028930176f,
				0.02925493f,
				0.0295814611f,
				0.0299097653f,
				0.0302398447f,
				0.0305716954f,
				0.03090532f,
				0.0312407129f,
				0.0315778777f,
				0.0319168121f,
				0.0322575159f,
				0.0325999856f,
				0.03294422f,
				0.0332902223f,
				0.03363799f,
				0.0339875221f,
				0.0343388133f,
				0.03469187f,
				0.0350466855f,
				0.0354032628f,
				0.0357616f,
				0.0361216925f,
				0.0364835449f,
				0.03684715f,
				0.0372125171f,
				0.0375796333f,
				0.0379485041f,
				0.0383191258f,
				0.0386915021f,
				0.0390656255f,
				0.0394415f,
				0.03981912f,
				0.0401984937f,
				0.04057961f,
				0.04096247f,
				0.0413470753f,
				0.0417334251f,
				0.0421215147f,
				0.042511344f,
				0.0429029167f,
				0.04329623f,
				0.0436912775f,
				0.0440880619f,
				0.0444865823f,
				0.04488684f,
				0.0452888273f,
				0.04569255f,
				0.0460979976f,
				0.046505183f,
				0.0469140932f,
				0.0473247319f,
				0.0477371f,
				0.04815119f,
				0.04856701f,
				0.0489845462f,
				0.04940381f,
				0.04982479f,
				0.05024749f,
				0.0506719127f,
				0.0510980524f,
				0.0515259057f,
				0.0519554764f,
				0.05238676f,
				0.0528197549f,
				0.0532544628f,
				0.05369088f,
				0.05412901f,
				0.0545688421f,
				0.0550103821f,
				0.05545363f,
				0.055898577f,
				0.0563452281f,
				0.0567935854f,
				0.0572436377f,
				0.0576953925f,
				0.0581488423f,
				0.0586039871f,
				0.05906083f,
				0.0595193654f,
				0.05997959f,
				0.0604415126f,
				0.06090512f,
				0.0613704175f,
				0.0618374f,
				0.06230607f,
				0.062776424f,
				0.06324846f,
				0.06372218f,
				0.06419758f,
				0.06467465f,
				0.06515341f,
				0.06563384f,
				0.0661159456f,
				0.06659973f,
				0.06708518f,
				0.0675722957f,
				0.06806109f,
				0.06855155f,
				0.0690436661f,
				0.06953745f,
				0.07003291f,
				0.07053002f,
				0.07102879f,
				0.0715292245f,
				0.07203132f,
				0.07253506f,
				0.07304046f,
				0.07354751f,
				0.0740562156f,
				0.07456657f,
				0.07507857f,
				0.07559222f,
				0.07610752f,
				0.07662445f,
				0.07714303f,
				0.07766325f,
				0.07818511f,
				0.078708604f,
				0.0792337358f,
				0.07976051f,
				0.0802889f,
				0.08081893f,
				0.08135059f,
				0.08188388f,
				0.08241879f,
				0.08295533f,
				0.083493486f,
				0.08403327f,
				0.08457467f,
				0.08511769f,
				0.08566233f,
				0.0862085745f,
				0.08675644f,
				0.08730591f,
				0.08785699f,
				0.0884096846f,
				0.0889639854f,
				0.08951989f,
				0.09007739f,
				0.09063649f,
				0.0911972f,
				0.0917595f,
				0.0923234f,
				0.09288889f,
				0.09345598f,
				0.09402465f,
				0.09459491f,
				0.09516676f,
				0.09574019f,
				0.09631521f,
				0.09689181f,
				0.09746999f,
				0.098049745f,
				0.09863108f,
				0.09921398f,
				0.09979846f,
				0.100384511f,
				0.100972123f,
				0.101561308f,
				0.102152057f,
				0.102744371f,
				0.103338242f,
				0.10393367f,
				0.104530662f,
				0.105129205f,
				0.1057293f,
				0.106330946f,
				0.106934145f,
				0.107538886f,
				0.108145177f,
				0.108753011f,
				0.109362386f,
				0.1099733f,
				0.110585749f,
				0.111199737f,
				0.111815259f,
				0.112432308f,
				0.113050893f,
				0.113671005f,
				0.114292637f,
				0.114915796f,
				0.115540475f,
				0.116166674f,
				0.116794392f,
				0.117423624f,
				0.118054368f,
				0.118686624f,
				0.119320385f,
				0.119955651f,
				0.12059243f,
				0.121230707f,
				0.121870488f,
				0.122511759f,
				0.123154536f,
				0.1237988f,
				0.124444559f,
				0.1250918f,
				0.125740543f,
				0.12639077f,
				0.127042472f,
				0.12769565f,
				0.128350317f,
				0.12900646f,
				0.129664063f,
				0.130323157f,
				0.13098371f,
				0.131645724f,
				0.132309213f,
				0.132974178f,
				0.133640587f,
				0.134308457f,
				0.134977773f,
				0.135648564f,
				0.136320785f,
				0.136994466f,
				0.1376696f,
				0.138346165f,
				0.139024183f,
				0.139703631f,
				0.140384525f,
				0.141066864f,
				0.141750619f,
				0.1424358f,
				0.14312242f,
				0.143810466f,
				0.144499928f,
				0.14519082f,
				0.145883128f,
				0.146576852f,
				0.147271991f,
				0.147968531f,
				0.148666486f,
				0.149365842f,
				0.150066614f,
				0.150768772f,
				0.151472345f,
				0.1521773f,
				0.152883664f,
				0.153591409f,
				0.154300541f,
				0.155011058f,
				0.155722961f,
				0.15643625f,
				0.157150909f,
				0.157866955f,
				0.158584371f,
				0.159303144f,
				0.1600233f,
				0.160744816f,
				0.1614677f,
				0.162191942f,
				0.162917539f,
				0.1636445f,
				0.1643728f,
				0.165102467f,
				0.165833473f,
				0.166565821f,
				0.167299509f,
				0.168034539f,
				0.168770909f,
				0.1695086f,
				0.170247644f,
				0.170988f,
				0.171729684f,
				0.1724727f,
				0.173217028f,
				0.173962668f,
				0.174709633f,
				0.17545791f,
				0.1762075f,
				0.176958382f,
				0.177710578f,
				0.17846407f,
				0.179218858f,
				0.179974958f,
				0.18073234f,
				0.181491f,
				0.182250962f,
				0.1830122f,
				0.183774725f,
				0.184538528f,
				0.185303614f,
				0.18606995f,
				0.186837569f,
				0.187606454f,
				0.1883766f,
				0.189148024f,
				0.1899207f,
				0.190694615f,
				0.191469789f,
				0.192246214f,
				0.19302389f,
				0.1938028f,
				0.194582969f,
				0.195364356f,
				0.19614698f,
				0.196930841f,
				0.197715938f,
				0.198502243f,
				0.199289784f,
				0.200078547f,
				0.200868517f,
				0.2016597f,
				0.2024521f,
				0.2032457f,
				0.204040512f,
				0.204836518f,
				0.20563373f,
				0.206432134f,
				0.207231715f,
				0.2080325f,
				0.208834469f,
				0.209637627f,
				0.210441962f,
				0.211247459f,
				0.212054148f,
				0.212861985f,
				0.213671014f,
				0.21448119f,
				0.215292528f,
				0.216105029f,
				0.216918677f,
				0.217733487f,
				0.218549445f,
				0.219366536f,
				0.220184773f,
				0.221004143f,
				0.221824661f,
				0.2226463f,
				0.223469064f,
				0.224292964f,
				0.225117981f,
				0.225944117f,
				0.22677137f,
				0.227599725f,
				0.2284292f,
				0.229259774f,
				0.230091453f,
				0.230924234f,
				0.2317581f,
				0.23259306f,
				0.233429119f,
				0.234266251f,
				0.235104471f,
				0.235943764f,
				0.236784145f,
				0.237625584f,
				0.2384681f,
				0.23931168f,
				0.240156323f,
				0.241002008f,
				0.241848767f,
				0.242696568f,
				0.243545413f,
				0.244395316f,
				0.245246246f,
				0.24609822f,
				0.246951222f,
				0.247805268f,
				0.248660326f,
				0.249516413f,
				0.2503735f,
				0.251231641f,
				0.252090782f,
				0.252950937f,
				0.253812075f,
				0.254674226f,
				0.2555374f,
				0.2564015f,
				0.25726667f,
				0.2581328f,
				0.258999884f,
				0.259868f,
				0.260737062f,
				0.261607081f,
				0.2624781f,
				0.26335007f,
				0.264223f,
				0.2650969f,
				0.26597178f,
				0.266847581f,
				0.267724335f,
				0.268602f,
				0.269480646f,
				0.270360231f,
				0.271240741f,
				0.272122175f,
				0.273004532f,
				0.2738878f,
				0.274772018f,
				0.275657147f,
				0.27654317f,
				0.277430117f,
				0.278317958f,
				0.2792067f,
				0.280096322f,
				0.280986845f,
				0.281878263f,
				0.282770574f,
				0.28366375f,
				0.2845578f,
				0.285452724f,
				0.286348522f,
				0.287245184f,
				0.2881427f,
				0.2890411f,
				0.289940357f,
				0.290840417f,
				0.291741371f,
				0.29264313f,
				0.293545753f,
				0.2944492f,
				0.295353472f,
				0.2962586f,
				0.29716453f,
				0.298071265f,
				0.298978835f,
				0.2998872f,
				0.3007964f,
				0.301706374f,
				0.302617162f,
				0.303528756f,
				0.304441124f,
				0.305354267f,
				0.306268215f,
				0.307182938f,
				0.308098435f,
				0.3090147f,
				0.309931755f,
				0.310849547f,
				0.3117681f,
				0.312687427f,
				0.313607484f,
				0.3145283f,
				0.315449864f,
				0.316372156f,
				0.3172952f,
				0.318218946f,
				0.319143444f,
				0.320068657f,
				0.3209946f,
				0.321921229f,
				0.3228486f,
				0.323776662f,
				0.324705422f,
				0.3256349f,
				0.326565057f,
				0.3274959f,
				0.328427434f,
				0.32935968f,
				0.330292553f,
				0.33122614f,
				0.332160383f,
				0.333095282f,
				0.334030867f,
				0.334967077f,
				0.335903972f,
				0.3368415f,
				0.337779671f,
				0.338718474f,
				0.339657933f,
				0.340598017f,
				0.341538727f,
				0.342480063f,
				0.343422f,
				0.344364583f,
				0.345307767f,
				0.346251547f,
				0.347195923f,
				0.348140925f,
				0.3490865f,
				0.3500327f,
				0.350979447f,
				0.351926774f,
				0.3528747f,
				0.353823185f,
				0.35477224f,
				0.355721861f,
				0.356672049f,
				0.357622772f,
				0.358574063f,
				0.3595259f,
				0.360478282f,
				0.361431181f,
				0.362384647f,
				0.363338619f,
				0.364293128f,
				0.365248144f,
				0.3662037f,
				0.367159754f,
				0.368116319f,
				0.3690734f,
				0.37003094f,
				0.370989025f,
				0.371947557f,
				0.3729066f,
				0.373866141f,
				0.374826133f,
				0.3757866f,
				0.376747549f,
				0.377708942f,
				0.3786708f,
				0.379633129f,
				0.3805959f,
				0.381559134f,
				0.3825228f,
				0.383486867f,
				0.384451419f,
				0.385416359f,
				0.386381745f,
				0.387347549f,
				0.38831377f,
				0.3892804f,
				0.390247464f,
				0.3912149f,
				0.392182738f,
				0.393150955f,
				0.3941196f,
				0.395088583f,
				0.396058f,
				0.397027731f,
				0.3979979f,
				0.398968369f,
				0.399939239f,
				0.400910437f,
				0.401882023f,
				0.402853936f,
				0.4038262f,
				0.4047988f,
				0.405771762f,
				0.406745017f,
				0.407718629f,
				0.408692539f,
				0.409666777f,
				0.4106413f,
				0.411616176f,
				0.412591338f,
				0.4135668f,
				0.414542526f,
				0.415518582f,
				0.4164949f,
				0.4174715f,
				0.4184484f,
				0.419425547f,
				0.420402974f,
				0.421380669f,
				0.4223586f,
				0.4233368f,
				0.424315244f,
				0.425293922f,
				0.426272869f,
				0.427252024f,
				0.428231418f,
				0.42921102f,
				0.430190861f,
				0.43117094f,
				0.4321512f,
				0.4331317f,
				0.43411237f,
				0.435093254f,
				0.436074317f,
				0.4370556f,
				0.438037038f,
				0.439018667f,
				0.440000445f,
				0.440982431f,
				0.441964567f,
				0.442946881f,
				0.443929344f,
				0.444911927f,
				0.4458947f,
				0.4468776f,
				0.447860628f,
				0.4488438f,
				0.449827135f,
				0.450810552f,
				0.4517941f,
				0.452777773f,
				0.453761548f,
				0.4547454f,
				0.4557294f,
				0.456713468f,
				0.45769766f,
				0.4586819f,
				0.459666252f,
				0.460650682f,
				0.461635172f,
				0.462619722f,
				0.463604361f,
				0.464589059f,
				0.4655738f,
				0.466558576f,
				0.467543423f,
				0.4685283f,
				0.469513237f,
				0.470498174f,
				0.471483171f,
				0.472468168f,
				0.473453164f,
				0.47443822f,
				0.475423247f,
				0.4764083f,
				0.477393329f,
				0.4783784f,
				0.4793634f,
				0.480348438f,
				0.481333435f,
				0.4823184f,
				0.483303338f,
				0.484288245f,
				0.485273123f,
				0.48625797f,
				0.487242758f,
				0.4882275f,
				0.489212155f,
				0.490196764f,
				0.4911813f,
				0.4921658f,
				0.4931502f,
				0.494134545f,
				0.4951188f,
				0.496102929f,
				0.497087f,
				0.498070955f,
				0.499054819f,
				0.500038564f,
				0.5010222f,
				0.502005756f,
				0.5029892f,
				0.5039724f,
				0.5049556f,
				0.5059386f,
				0.50692147f,
				0.507904232f,
				0.508886755f,
				0.5098692f,
				0.5108515f,
				0.5118336f,
				0.512815535f,
				0.5137973f,
				0.5147789f,
				0.5157603f,
				0.5167415f,
				0.517722547f,
				0.5187034f,
				0.519684f,
				0.520664454f,
				0.5216447f,
				0.522624731f,
				0.5236045f,
				0.524584055f,
				0.5255634f,
				0.526542544f,
				0.527521431f,
				0.5285f,
				0.529478431f,
				0.5304566f,
				0.5314345f,
				0.5324121f,
				0.533389449f,
				0.534366548f,
				0.535343349f,
				0.5363199f,
				0.5372962f,
				0.538272142f,
				0.5392478f,
				0.540223241f,
				0.5411983f,
				0.5421731f,
				0.543147564f,
				0.544121742f,
				0.545095563f,
				0.5460691f,
				0.547042251f,
				0.5480151f,
				0.5489876f,
				0.549959838f,
				0.550931633f,
				0.5519031f,
				0.552874267f,
				0.553845048f,
				0.5548154f,
				0.5557855f,
				0.5567551f,
				0.5577244f,
				0.558693349f,
				0.559661865f,
				0.560629964f,
				0.5615977f,
				0.562565f,
				0.563531935f,
				0.5644985f,
				0.565464556f,
				0.5664303f,
				0.5673955f,
				0.5683604f,
				0.5693248f,
				0.5702888f,
				0.5712523f,
				0.5722154f,
				0.573178f,
				0.5741402f,
				0.5751019f,
				0.5760632f,
				0.577024f,
				0.577984333f,
				0.5789442f,
				0.579903543f,
				0.580862463f,
				0.581820846f,
				0.582778752f,
				0.5837362f,
				0.5846931f,
				0.5856495f,
				0.58660537f,
				0.5875608f,
				0.588515639f,
				0.589469969f,
				0.590423763f,
				0.5913771f,
				0.5923298f,
				0.593282f,
				0.594233632f,
				0.595184743f,
				0.5961353f,
				0.5970853f,
				0.598034739f,
				0.5989836f,
				0.5999319f,
				0.6008796f,
				0.6018268f,
				0.6027733f,
				0.6037193f,
				0.6046647f,
				0.6056095f,
				0.6065536f,
				0.6074972f,
				0.608440161f,
				0.6093825f,
				0.610324264f,
				0.611265361f,
				0.612205863f,
				0.6131457f,
				0.614084959f,
				0.615023553f,
				0.6159615f,
				0.6168988f,
				0.6178354f,
				0.618771434f,
				0.61970675f,
				0.6206414f,
				0.6215754f,
				0.622508764f,
				0.6234414f,
				0.6243734f,
				0.625304639f,
				0.626235247f,
				0.6271652f,
				0.6280944f,
				0.6290229f,
				0.6299507f,
				0.6308778f,
				0.631804168f,
				0.6327299f,
				0.633654833f,
				0.634579062f,
				0.6355025f,
				0.6364253f,
				0.637347341f,
				0.6382686f,
				0.6391891f,
				0.640108943f,
				0.641028f,
				0.641946256f,
				0.6428638f,
				0.6437805f,
				0.644696534f,
				0.645611763f,
				0.6465262f,
				0.647439837f,
				0.648352742f,
				0.6492648f,
				0.6501761f,
				0.6510866f,
				0.6519963f,
				0.6529052f,
				0.6538133f,
				0.6547206f,
				0.6556271f,
				0.6565327f,
				0.6574375f,
				0.6583415f,
				0.659244657f,
				0.660147f,
				0.6610485f,
				0.661949158f,
				0.662848949f,
				0.6637479f,
				0.664646f,
				0.665543258f,
				0.666439652f,
				0.667335153f,
				0.6682298f,
				0.6691236f,
				0.6700165f,
				0.6709086f,
				0.67179966f,
				0.67269f,
				0.673579335f,
				0.6744678f,
				0.6753554f,
				0.676242054f,
				0.677127838f,
				0.678012669f,
				0.6788966f,
				0.679779649f,
				0.6806618f,
				0.681543f,
				0.682423234f,
				0.6833026f,
				0.684181f,
				0.6850585f,
				0.685935f,
				0.6868106f,
				0.687685251f,
				0.688558936f,
				0.689431667f,
				0.690303445f,
				0.691174269f,
				0.6920441f,
				0.692913f,
				0.6937809f,
				0.694647849f,
				0.6955138f,
				0.696378767f,
				0.697242737f,
				0.698105752f,
				0.698967755f,
				0.6998288f,
				0.7006888f,
				0.7015478f,
				0.7024058f,
				0.7032628f,
				0.704118848f,
				0.7049738f,
				0.7058278f,
				0.7066807f,
				0.7075326f,
				0.7083835f,
				0.709233344f,
				0.7100822f,
				0.71092993f,
				0.7117767f,
				0.7126224f,
				0.713467f,
				0.714310646f,
				0.715153158f,
				0.715994656f,
				0.7168351f,
				0.717674434f,
				0.7185128f,
				0.71935f,
				0.7201861f,
				0.721021235f,
				0.7218552f,
				0.722688138f,
				0.72352f,
				0.72435075f,
				0.7251804f,
				0.726008952f,
				0.726836443f,
				0.7276628f,
				0.7284881f,
				0.729312241f,
				0.7301353f,
				0.73095727f,
				0.7317781f,
				0.732597768f,
				0.7334164f,
				0.734233856f,
				0.7350502f,
				0.7358654f,
				0.7366795f,
				0.737492442f,
				0.738304257f,
				0.73911494f,
				0.7399245f,
				0.740732849f,
				0.7415401f,
				0.742346168f,
				0.7431511f,
				0.7439549f,
				0.744757533f,
				0.745559f,
				0.7463593f,
				0.7471584f,
				0.747956336f,
				0.74875313f,
				0.749548733f,
				0.7503432f,
				0.7511364f,
				0.7519285f,
				0.7527194f,
				0.753509045f,
				0.754297554f,
				0.7550849f,
				0.755871f,
				0.756655931f,
				0.7574396f,
				0.7582221f,
				0.75900346f,
				0.7597835f,
				0.7605624f,
				0.7613401f,
				0.762116551f,
				0.762891769f,
				0.7636658f,
				0.7644386f,
				0.765210152f,
				0.765980542f,
				0.7667496f,
				0.7675175f,
				0.768284142f,
				0.7690496f,
				0.7698137f,
				0.770576656f,
				0.771338344f,
				0.7720988f,
				0.772857964f,
				0.7736159f,
				0.7743726f,
				0.775128f,
				0.7758822f,
				0.7766351f,
				0.7773868f,
				0.778137147f,
				0.778886259f,
				0.7796341f,
				0.7803807f,
				0.781126f,
				0.781870067f,
				0.78261286f,
				0.783354342f,
				0.7840945f,
				0.784833431f,
				0.7855711f,
				0.786307454f,
				0.7870425f,
				0.7877763f,
				0.7885087f,
				0.789239943f,
				0.7899698f,
				0.7906984f,
				0.7914257f,
				0.7921517f,
				0.792876363f,
				0.7935997f,
				0.794321835f,
				0.7950426f,
				0.795762f,
				0.7964802f,
				0.797197f,
				0.7979125f,
				0.798626661f,
				0.799339533f,
				0.8000511f,
				0.8007613f,
				0.80147016f,
				0.8021777f,
				0.802884f,
				0.803588867f,
				0.80429244f,
				0.8049947f,
				0.8056956f,
				0.8063952f,
				0.8070934f,
				0.8077903f,
				0.808485866f,
				0.80918f,
				0.8098729f,
				0.8105644f,
				0.811254561f,
				0.8119434f,
				0.812630832f,
				0.813316941f,
				0.814001739f,
				0.8146851f,
				0.815367162f,
				0.816047847f,
				0.816727161f,
				0.817405164f,
				0.818081737f,
				0.818757f,
				0.8194309f,
				0.8201034f,
				0.8207745f,
				0.8214443f,
				0.8221127f,
				0.8227797f,
				0.8234454f,
				0.8241097f,
				0.8247726f,
				0.8254341f,
				0.82609427f,
				0.826753f,
				0.82741046f,
				0.828066468f,
				0.8287211f,
				0.8293743f,
				0.8300262f,
				0.8306767f,
				0.831325769f,
				0.831973433f,
				0.8326198f,
				0.8332647f,
				0.8339082f,
				0.8345504f,
				0.835191131f,
				0.83583045f,
				0.836468458f,
				0.837105f,
				0.8377402f,
				0.8383739f,
				0.8390063f,
				0.8396373f,
				0.8402668f,
				0.840895f,
				0.84152174f,
				0.8421471f,
				0.842771053f,
				0.8433936f,
				0.844014764f,
				0.844634533f,
				0.8452529f,
				0.8458698f,
				0.8464853f,
				0.8470994f,
				0.8477121f,
				0.8483234f,
				0.8489333f,
				0.8495417f,
				0.8501488f,
				0.85075444f,
				0.851358652f,
				0.851961434f,
				0.852562845f,
				0.8531628f,
				0.8537614f,
				0.854358554f,
				0.854954243f,
				0.855548561f,
				0.856141448f,
				0.8567329f,
				0.857323f,
				0.8579116f,
				0.8584988f,
				0.8590846f,
				0.85966897f,
				0.8602519f,
				0.860833466f,
				0.861413538f,
				0.86199224f,
				0.8625695f,
				0.863145351f,
				0.863719761f,
				0.864292741f,
				0.8648643f,
				0.8654344f,
				0.866003156f,
				0.8665704f,
				0.8671363f,
				0.8677007f,
				0.8682637f,
				0.8688253f,
				0.8693854f,
				0.869944155f,
				0.870501459f,
				0.871057332f,
				0.8716118f,
				0.8721648f,
				0.8727164f,
				0.8732666f,
				0.8738153f,
				0.874362648f,
				0.8749085f,
				0.875452936f,
				0.875996f,
				0.876537561f,
				0.877077758f,
				0.8776165f,
				0.8781538f,
				0.8786897f,
				0.8792242f,
				0.8797572f,
				0.8802888f,
				0.880818963f,
				0.8813477f,
				0.881875038f,
				0.882401f,
				0.882925451f,
				0.8834485f,
				0.8839701f,
				0.8844903f,
				0.88500905f,
				0.8855264f,
				0.8860423f,
				0.8865568f,
				0.8870699f,
				0.8875815f,
				0.888091743f,
				0.8886005f,
				0.8891079f,
				0.889613867f,
				0.890118361f,
				0.8906215f,
				0.8911232f,
				0.8916234f,
				0.892122269f,
				0.892619669f,
				0.89311564f,
				0.893610239f,
				0.894103348f,
				0.8945951f,
				0.895085454f,
				0.895574331f,
				0.8960618f,
				0.896547854f,
				0.8970325f,
				0.8975157f,
				0.897997558f,
				0.898478f,
				0.898956954f,
				0.8994345f,
				0.8999106f,
				0.9003854f,
				0.90085876f,
				0.90133065f,
				0.901801169f,
				0.902270257f,
				0.902738f,
				0.903204262f,
				0.9036691f,
				0.904132545f,
				0.90459466f,
				0.9050553f,
				0.905514538f,
				0.905972362f,
				0.9064288f,
				0.906883836f,
				0.9073375f,
				0.907789767f,
				0.908240557f,
				0.908690035f,
				0.909138f,
				0.9095847f,
				0.910029948f,
				0.910473764f,
				0.910916269f,
				0.9113573f,
				0.911797f,
				0.91223526f,
				0.9126721f,
				0.913107634f,
				0.913541734f,
				0.913974464f,
				0.914405763f,
				0.9148357f,
				0.9152643f,
				0.915691435f,
				0.916117251f,
				0.916541636f,
				0.9169647f,
				0.917386353f,
				0.9178066f,
				0.9182255f,
				0.918643f,
				0.919059157f,
				0.919473946f,
				0.9198873f,
				0.920299351f,
				0.92071f,
				0.9211193f,
				0.9215272f,
				0.92193377f,
				0.922338963f,
				0.9227428f,
				0.923145235f,
				0.9235463f,
				0.9239461f,
				0.9243444f,
				0.924741447f,
				0.9251371f,
				0.925531447f,
				0.925924361f,
				0.926315963f,
				0.926706254f,
				0.9270951f,
				0.927482665f,
				0.9278689f,
				0.9282537f,
				0.928637266f,
				0.929019451f,
				0.929400265f,
				0.929779768f,
				0.9301579f,
				0.9305347f,
				0.93091017f,
				0.9312843f,
				0.931657135f,
				0.932028651f,
				0.9323988f,
				0.9327676f,
				0.9331351f,
				0.9335013f,
				0.933866143f,
				0.9342297f,
				0.9345919f,
				0.934952736f,
				0.935312331f,
				0.9356706f,
				0.9360275f,
				0.9363832f,
				0.9367375f,
				0.9370905f,
				0.9374422f,
				0.9377926f,
				0.9381417f,
				0.9384895f,
				0.938836f,
				0.9391812f,
				0.9395251f,
				0.9398677f,
				0.940209031f,
				0.940549f,
				0.940887749f,
				0.941225231f,
				0.941561341f,
				0.94189626f,
				0.9422298f,
				0.942562163f,
				0.942893147f,
				0.94322294f,
				0.9435514f,
				0.9438786f,
				0.9442045f,
				0.9445292f,
				0.9448526f,
				0.945174754f,
				0.9454956f,
				0.9458152f,
				0.946133554f,
				0.946450651f,
				0.946766436f,
				0.947081f,
				0.9473944f,
				0.9477064f,
				0.94801724f,
				0.9483268f,
				0.948635161f,
				0.948942244f,
				0.9492481f,
				0.9495527f,
				0.9498561f,
				0.950158238f,
				0.9504591f,
				0.9507588f,
				0.951057255f,
				0.9513545f,
				0.9516505f,
				0.951945245f,
				0.9522388f,
				0.952531159f,
				0.952822268f,
				0.9531122f,
				0.9534009f,
				0.9536884f,
				0.953974664f,
				0.954259753f,
				0.9545436f,
				0.9548263f,
				0.955107749f,
				0.955388069f,
				0.955667138f,
				0.955945f,
				0.9562217f,
				0.956497252f,
				0.956771553f,
				0.9570447f,
				0.9573167f,
				0.9575875f,
				0.9578571f,
				0.958125532f,
				0.9583928f,
				0.9586589f,
				0.9589238f,
				0.959187567f,
				0.9594502f,
				0.9597116f,
				0.9599719f,
				0.960231f,
				0.960489f,
				0.9607458f,
				0.9610015f,
				0.961256f,
				0.9615094f,
				0.961761653f,
				0.962012768f,
				0.9622627f,
				0.962511539f,
				0.962759256f,
				0.9630058f,
				0.963251233f,
				0.963495553f,
				0.963738739f,
				0.9639808f,
				0.9642218f,
				0.9644616f,
				0.964700341f,
				0.9649379f,
				0.965174437f,
				0.9654099f,
				0.9656442f,
				0.965877354f,
				0.966109455f,
				0.9663404f,
				0.9665704f,
				0.9667992f,
				0.9670269f,
				0.967253566f,
				0.9674791f,
				0.9677036f,
				0.967927f,
				0.9681493f,
				0.968370557f,
				0.968590736f,
				0.968809843f,
				0.969027936f,
				0.9692449f,
				0.969460845f,
				0.96967566f,
				0.9698895f,
				0.970102251f,
				0.970313966f,
				0.9705246f,
				0.970734239f,
				0.9709428f,
				0.971150339f,
				0.9713568f,
				0.971562266f,
				0.9717667f,
				0.9719701f,
				0.9721725f,
				0.972373843f,
				0.9725741f,
				0.972773433f,
				0.972971737f,
				0.973169f,
				0.973365247f,
				0.9735605f,
				0.973754764f,
				0.973948f,
				0.9741403f,
				0.9743315f,
				0.974521756f,
				0.974711f,
				0.9748993f,
				0.97508657f,
				0.9752729f,
				0.9754582f,
				0.9756425f,
				0.9758259f,
				0.976008236f,
				0.9761897f,
				0.9763701f,
				0.9765496f,
				0.976728141f,
				0.9769057f,
				0.977082253f,
				0.9772579f,
				0.9774326f,
				0.977606356f,
				0.97777915f,
				0.977951f,
				0.978121936f,
				0.978291869f,
				0.9784609f,
				0.978629053f,
				0.9787962f,
				0.9789624f,
				0.979127765f,
				0.979292154f,
				0.97945565f,
				0.9796182f,
				0.97977984f,
				0.9799406f,
				0.980100453f,
				0.980259359f,
				0.9804174f,
				0.9805745f,
				0.9807307f,
				0.980886042f,
				0.9810405f,
				0.9811941f,
				0.9813467f,
				0.9814985f,
				0.9816494f,
				0.9817994f,
				0.9819486f,
				0.982096851f,
				0.982244253f,
				0.9823908f,
				0.9825365f,
				0.982681334f,
				0.9828253f,
				0.9829684f,
				0.983110666f,
				0.983252048f,
				0.9833926f,
				0.9835323f,
				0.9836712f,
				0.983809233f,
				0.983946443f,
				0.9840828f,
				0.984218359f,
				0.984353065f,
				0.984486938f,
				0.98462f,
				0.984752238f,
				0.984883666f,
				0.9850143f,
				0.985144138f,
				0.9852731f,
				0.985401332f,
				0.9855287f,
				0.9856553f,
				0.985781133f,
				0.9859061f,
				0.98603034f,
				0.9861538f,
				0.986276448f,
				0.9863983f,
				0.9865194f,
				0.9866397f,
				0.986759245f,
				0.986878f,
				0.986996f,
				0.987113237f,
				0.9872297f,
				0.987345457f,
				0.9874604f,
				0.9875746f,
				0.987688065f,
				0.9878008f,
				0.9879127f,
				0.988023937f,
				0.988134444f,
				0.9882442f,
				0.9883532f,
				0.9884615f,
				0.9885691f,
				0.9886759f,
				0.988782f,
				0.98888737f,
				0.9889921f,
				0.989096045f,
				0.9891993f,
				0.98930186f,
				0.989403665f,
				0.9895048f,
				0.989605248f,
				0.989704967f,
				0.989804f,
				0.9899024f,
				0.990000069f,
				0.990097046f,
				0.990193367f,
				0.990289f,
				0.9903839f,
				0.9904782f,
				0.9905718f,
				0.9906647f,
				0.990757f,
				0.9908486f,
				0.990939558f,
				0.991029859f,
				0.9911195f,
				0.9912085f,
				0.9912968f,
				0.9913845f,
				0.9914716f,
				0.991557956f,
				0.9916437f,
				0.9917289f,
				0.9918134f,
				0.9918973f,
				0.991980553f,
				0.992063165f,
				0.9921452f,
				0.992226541f,
				0.992307365f,
				0.992387533f,
				0.992467046f,
				0.992546f,
				0.992624342f,
				0.9927021f,
				0.992779255f,
				0.9928558f,
				0.9929318f,
				0.9930071f,
				0.9930819f,
				0.993156135f,
				0.993229747f,
				0.993302763f,
				0.993375242f,
				0.9934471f,
				0.9935185f,
				0.9935892f,
				0.9936594f,
				0.993729f,
				0.9937981f,
				0.993866563f,
				0.9939345f,
				0.9940019f,
				0.9940688f,
				0.994135141f,
				0.9942009f,
				0.9942661f,
				0.9943308f,
				0.994394958f,
				0.994458556f,
				0.9945217f,
				0.9945842f,
				0.994646251f,
				0.9947078f,
				0.9947688f,
				0.9948293f,
				0.994889259f,
				0.994948745f,
				0.9950077f,
				0.9950661f,
				0.995124042f,
				0.9951815f,
				0.9952385f,
				0.9952949f,
				0.9953509f,
				0.9954063f,
				0.995461345f,
				0.9955158f,
				0.9955699f,
				0.9956234f,
				0.995676458f,
				0.9957291f,
				0.9957812f,
				0.99583286f,
				0.995884061f,
				0.9959348f,
				0.995985031f,
				0.996034861f,
				0.9960842f,
				0.9961331f,
				0.996181548f,
				0.9962295f,
				0.9962771f,
				0.996324241f,
				0.9963709f,
				0.996417165f,
				0.996462941f,
				0.9965083f,
				0.996553242f,
				0.996597767f,
				0.9966419f,
				0.996685565f,
				0.9967288f,
				0.996771634f,
				0.9968141f,
				0.9968561f,
				0.996897638f,
				0.9969389f,
				0.996979654f,
				0.997020066f,
				0.99706f,
				0.997099638f,
				0.9971388f,
				0.9971776f,
				0.997216046f,
				0.9972541f,
				0.997291744f,
				0.997329f,
				0.9973659f,
				0.99740237f,
				0.9974385f,
				0.997474253f,
				0.997509658f,
				0.9975447f,
				0.997579336f,
				0.9976136f,
				0.9976476f,
				0.997681141f,
				0.9977144f,
				0.997747242f,
				0.9977798f,
				0.997812f,
				0.9978438f,
				0.9978753f,
				0.9979064f,
				0.9979372f,
				0.99796766f,
				0.9979978f,
				0.9980276f,
				0.998057067f,
				0.9980862f,
				0.998115063f,
				0.998143554f,
				0.998171747f,
				0.9981996f,
				0.9982272f,
				0.998254359f,
				0.9982813f,
				0.998307943f,
				0.9983342f,
				0.9983602f,
				0.998385966f,
				0.998411357f,
				0.998436451f,
				0.998461246f,
				0.9984858f,
				0.99851f,
				0.998533964f,
				0.9985576f,
				0.998581f,
				0.998604059f,
				0.9986269f,
				0.9986494f,
				0.998671651f,
				0.998693645f,
				0.998715341f,
				0.9987368f,
				0.998757958f,
				0.9987789f,
				0.998799562f,
				0.998819947f,
				0.998840034f,
				0.998859942f,
				0.998879552f,
				0.9988989f,
				0.998918056f,
				0.998936951f,
				0.998955548f,
				0.9989739f,
				0.9989921f,
				0.999009967f,
				0.999027669f,
				0.9990451f,
				0.9990623f,
				0.9990792f,
				0.999096f,
				0.9991125f,
				0.999128759f,
				0.999144852f,
				0.9991607f,
				0.9991763f,
				0.9991917f,
				0.9992069f,
				0.999221861f,
				0.999236643f,
				0.9992512f,
				0.999265552f,
				0.9992797f,
				0.9992936f,
				0.9993074f,
				0.9993209f,
				0.9993343f,
				0.9993474f,
				0.9993603f,
				0.9993731f,
				0.999385655f,
				0.999398053f,
				0.9994103f,
				0.999422252f,
				0.999434054f,
				0.999445736f,
				0.9994572f,
				0.9994685f,
				0.9994796f,
				0.9994905f,
				0.9995013f,
				0.9995119f,
				0.9995223f,
				0.9995326f,
				0.999542654f,
				0.9995526f,
				0.9995624f,
				0.999572f,
				0.999581456f,
				0.999590755f,
				0.9995999f,
				0.9996089f,
				0.9996177f,
				0.9996264f,
				0.9996349f,
				0.9996433f,
				0.999651551f,
				0.999659657f,
				0.999667645f,
				0.999675453f,
				0.999683142f,
				0.9996907f,
				0.9996981f,
				0.9997054f,
				0.9997125f,
				0.99971956f,
				0.9997264f,
				0.9997332f,
				0.9997398f,
				0.9997463f,
				0.99975276f,
				0.999759f,
				0.999765158f,
				0.9997712f,
				0.999777138f,
				0.9997829f,
				0.9997886f,
				0.9997942f,
				0.9997996f,
				0.999805f,
				0.9998102f,
				0.9998154f,
				0.9998204f,
				0.999825358f,
				0.9998302f,
				0.9998349f,
				0.999839544f,
				0.9998441f,
				0.999848545f,
				0.999852836f,
				0.9998571f,
				0.9998613f,
				0.999865353f,
				0.9998693f,
				0.9998732f,
				0.999877f,
				0.999880731f,
				0.999884367f,
				0.9998879f,
				0.999891341f,
				0.999894738f,
				0.9998981f,
				0.9999013f,
				0.9999044f,
				0.9999075f,
				0.9999105f,
				0.9999134f,
				0.999916255f,
				0.999919057f,
				0.9999218f,
				0.9999244f,
				0.999927f,
				0.9999295f,
				0.999931931f,
				0.9999343f,
				0.99993664f,
				0.9999389f,
				0.9999411f,
				0.999943256f,
				0.999945343f,
				0.999947369f,
				0.9999493f,
				0.9999512f,
				0.9999531f,
				0.9999549f,
				0.9999566f,
				0.999958336f,
				0.999959946f,
				0.999961555f,
				0.9999631f,
				0.9999646f,
				0.9999661f,
				0.999967456f,
				0.9999688f,
				0.9999702f,
				0.999971449f,
				0.9999727f,
				0.9999739f,
				0.999975f,
				0.999976158f,
				0.999977231f,
				0.9999783f,
				0.9999793f,
				0.9999803f,
				0.9999812f,
				0.9999821f,
				0.999983f,
				0.999983847f,
				0.9999846f,
				0.9999854f,
				0.9999862f,
				0.9999869f,
				0.999987543f,
				0.999988258f,
				0.999988854f,
				0.99998945f,
				0.999990046f,
				0.999990642f,
				0.9999912f,
				0.999991655f,
				0.999992132f,
				0.9999926f,
				0.9999931f,
				0.9999935f,
				0.9999939f,
				0.9999943f,
				0.999994636f,
				0.999995f,
				0.999995351f,
				0.999995649f,
				0.999995947f,
				0.999996245f,
				0.9999965f,
				0.9999968f,
				0.999997f,
				0.9999972f,
				0.999997437f,
				0.9999976f,
				0.999997854f,
				0.999998033f,
				0.999998152f,
				0.999998331f,
				0.99999845f,
				0.9999986f,
				0.999998748f,
				0.999998868f,
				0.999999f,
				0.999999046f,
				0.999999166f,
				0.9999992f,
				0.999999344f,
				0.9999994f,
				0.999999464f,
				0.9999995f,
				0.9999996f,
				0.999999642f,
				0.9999997f,
				0.9999997f,
				0.999999762f,
				0.999999762f,
				0.9999998f,
				0.9999998f,
				0.9999999f,
				0.9999999f,
				0.9999999f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
			Xiph.vwin8192 = new float[]
			{
				5.78E-08f,
				5.198E-07f,
				1.4438E-06f,
				2.8299E-06f,
				4.678E-06f,
				6.9882E-06f,
				9.7604E-06f,
				1.29945E-05f,
				1.66908E-05f,
				2.0849E-05f,
				2.54692E-05f,
				3.05515E-05f,
				3.60958E-05f,
				4.21021E-05f,
				4.85704E-05f,
				5.55006E-05f,
				6.28929E-05f,
				7.07472E-05f,
				7.90635E-05f,
				8.78417E-05f,
				9.7082E-05f,
				0.0001067842f,
				0.0001169483f,
				0.0001275744f,
				0.0001386625f,
				0.0001502126f,
				0.0001622245f,
				0.0001746984f,
				0.0001876343f,
				0.000201032f,
				0.0002148917f,
				0.0002292132f,
				0.0002439967f,
				0.0002592421f,
				0.0002749493f,
				0.0002911184f,
				0.0003077493f,
				0.0003248421f,
				0.0003423967f,
				0.0003604132f,
				0.0003788915f,
				0.0003978316f,
				0.0004172335f,
				0.0004370971f,
				0.0004574226f,
				0.0004782098f,
				0.0004994587f,
				0.0005211694f,
				0.0005433418f,
				0.0005659759f,
				0.0005890717f,
				0.0006126292f,
				0.0006366484f,
				0.0006611292f,
				0.0006860716f,
				0.0007114757f,
				0.0007373414f,
				0.0007636687f,
				0.0007904576f,
				0.000817708f,
				0.00084542f,
				0.0008735935f,
				0.0009022285f,
				0.000931325f,
				0.000960883f,
				0.0009909025f,
				0.00102138345f,
				0.00105232571f,
				0.00108372956f,
				0.00111559464f,
				0.00114792108f,
				0.001180709f,
				0.00121395825f,
				0.00124766876f,
				0.00128184049f,
				0.00131647359f,
				0.00135156792f,
				0.00138712348f,
				0.00142314017f,
				0.00145961822f,
				0.00149655726f,
				0.00153395755f,
				0.001571819f,
				0.00161014148f,
				0.00164892513f,
				0.00168816978f,
				0.00172787544f,
				0.0017680421f,
				0.00180866977f,
				0.00184975844f,
				0.001891308f,
				0.00193331845f,
				0.00197579f,
				0.002018722f,
				0.00206211512f,
				0.002105969f,
				0.00215028366f,
				0.002195059f,
				0.00224029529f,
				0.002285992f,
				0.00233214977f,
				0.00237876782f,
				0.00242584664f,
				0.002473386f,
				0.00252138614f,
				0.00256984658f,
				0.00261876755f,
				0.002668149f,
				0.00271799113f,
				0.00276829349f,
				0.00281905616f,
				0.00287027936f,
				0.00292196288f,
				0.0029741067f,
				0.00302671059f,
				0.00307977479f,
				0.00313329929f,
				0.003187284f,
				0.003241729f,
				0.00329663372f,
				0.00335199875f,
				0.00340782385f,
				0.0034641088f,
				0.00352085382f,
				0.003578059f,
				0.00363572361f,
				0.00369384862f,
				0.003752433f,
				0.00381147745f,
				0.00387098175f,
				0.00393094542f,
				0.003991369f,
				0.00405225251f,
				0.00411359547f,
				0.00417539757f,
				0.00423765974f,
				0.00430038152f,
				0.00436356245f,
				0.004427203f,
				0.00449130265f,
				0.004555862f,
				0.004620881f,
				0.00468635838f,
				0.00475229556f,
				0.004818692f,
				0.00488554733f,
				0.004952862f,
				0.00502063567f,
				0.00508886855f,
				0.00515756f,
				0.005226711f,
				0.00529632f,
				0.005366389f,
				0.00543691637f,
				0.00550790271f,
				0.00557934726f,
				0.005651251f,
				0.00572361331f,
				0.00579643436f,
				0.00586971361f,
				0.00594345154f,
				0.006017648f,
				0.00609230343f,
				0.00616741646f,
				0.006242988f,
				0.006319018f,
				0.00639550667f,
				0.006472453f,
				0.006549858f,
				0.006627721f,
				0.00670604175f,
				0.00678482046f,
				0.00686405739f,
				0.00694375252f,
				0.007023905f,
				0.00710451556f,
				0.007185584f,
				0.00726711f,
				0.007349094f,
				0.00743153552f,
				0.0075144344f,
				0.007597791f,
				0.00768160541f,
				0.00776587659f,
				0.007850606f,
				0.007935792f,
				0.008021436f,
				0.00810753647f,
				0.00819409452f,
				0.00828111f,
				0.00836858153f,
				0.00845651049f,
				0.008544897f,
				0.00863374f,
				0.00872304f,
				0.008812797f,
				0.008903011f,
				0.008993681f,
				0.009084808f,
				0.009176391f,
				0.009268431f,
				0.009360927f,
				0.009453881f,
				0.009547289f,
				0.009641156f,
				0.009735477f,
				0.009830255f,
				0.009925489f,
				0.01002118f,
				0.0101173259f,
				0.0102139283f,
				0.0103109861f,
				0.0104085f,
				0.01050647f,
				0.0106048957f,
				0.010703777f,
				0.0108031137f,
				0.0109029058f,
				0.0110031534f,
				0.0111038564f,
				0.0112050148f,
				0.0113066286f,
				0.0114086978f,
				0.0115112225f,
				0.0116142016f,
				0.0117176361f,
				0.0118215252f,
				0.01192587f,
				0.0120306686f,
				0.0121359229f,
				0.0122416308f,
				0.0123477941f,
				0.0124544119f,
				0.0125614842f,
				0.012669012f,
				0.0127769932f,
				0.012885428f,
				0.0129943183f,
				0.0131036621f,
				0.01321346f,
				0.0133237122f,
				0.0134344185f,
				0.0135455793f,
				0.0136571927f,
				0.0137692606f,
				0.0138817821f,
				0.0139947571f,
				0.0141081857f,
				0.0142220678f,
				0.0143364035f,
				0.0144511927f,
				0.0145664345f,
				0.01468213f,
				0.0147982789f,
				0.0149148805f,
				0.0150319356f,
				0.0151494434f,
				0.0152674038f,
				0.0153858168f,
				0.0155046824f,
				0.0156240016f,
				0.0157437734f,
				0.015863996f,
				0.0159846731f,
				0.0161058f,
				0.0162273813f,
				0.0163494144f,
				0.0164718982f,
				0.0165948365f,
				0.0167182256f,
				0.0168420654f,
				0.0169663578f,
				0.0170911029f,
				0.0172162987f,
				0.0173419453f,
				0.0174680445f,
				0.0175945964f,
				0.0177215971f,
				0.01784905f,
				0.0179769546f,
				0.01810531f,
				0.0182341151f,
				0.0183633734f,
				0.01849308f,
				0.01862324f,
				0.018753849f,
				0.01888491f,
				0.01901642f,
				0.0191483814f,
				0.0192807931f,
				0.0194136538f,
				0.0195469651f,
				0.0196807273f,
				0.01981494f,
				0.019949602f,
				0.0200847127f,
				0.0202202741f,
				0.0203562845f,
				0.0204927456f,
				0.0206296556f,
				0.0207670163f,
				0.0209048241f,
				0.0210430827f,
				0.02118179f,
				0.0213209465f,
				0.0214605518f,
				0.0216006059f,
				0.021741109f,
				0.02188206f,
				0.02202346f,
				0.02216531f,
				0.0223076064f,
				0.0224503521f,
				0.0225935467f,
				0.0227371883f,
				0.0228812788f,
				0.0230258163f,
				0.0231708f,
				0.0233162362f,
				0.0234621167f,
				0.0236084461f,
				0.0237552226f,
				0.023902446f,
				0.0240501184f,
				0.0241982359f,
				0.0243468024f,
				0.024495814f,
				0.0246452745f,
				0.02479518f,
				0.0249455329f,
				0.0250963327f,
				0.02524758f,
				0.0253992714f,
				0.02555141f,
				0.0257039964f,
				0.0258570276f,
				0.0260105059f,
				0.02616443f,
				0.0263187978f,
				0.0264736135f,
				0.0266288742f,
				0.02678458f,
				0.0269407332f,
				0.02709733f,
				0.0272543728f,
				0.02741186f,
				0.0275697932f,
				0.02772817f,
				0.0278869923f,
				0.02804626f,
				0.0282059722f,
				0.028366128f,
				0.028526729f,
				0.0286877751f,
				0.0288492646f,
				0.0290111974f,
				0.0291735753f,
				0.0293363966f,
				0.0294996612f,
				0.02966337f,
				0.029827524f,
				0.0299921185f,
				0.0301571582f,
				0.0303226411f,
				0.0304885674f,
				0.0306549352f,
				0.0308217481f,
				0.0309890024f,
				0.0311566982f,
				0.0313248374f,
				0.03149342f,
				0.0316624455f,
				0.0318319127f,
				0.0320018232f,
				0.0321721733f,
				0.0323429666f,
				0.0325142f,
				0.03268588f,
				0.0328579955f,
				0.0330305547f,
				0.0332035571f,
				0.033377f,
				0.0335508846f,
				0.0337252058f,
				0.0338999741f,
				0.03407518f,
				0.0342508256f,
				0.0344269127f,
				0.034603443f,
				0.03478041f,
				0.03495782f,
				0.0351356678f,
				0.0353139564f,
				0.0354926847f,
				0.0356718525f,
				0.03585146f,
				0.036031507f,
				0.0362119935f,
				0.03639292f,
				0.03657428f,
				0.0367560871f,
				0.03693833f,
				0.0371210128f,
				0.0373041332f,
				0.03748769f,
				0.03767169f,
				0.0378561243f,
				0.0380409956f,
				0.0382263064f,
				0.0384120569f,
				0.0385982431f,
				0.03878487f,
				0.03897193f,
				0.0391594321f,
				0.03934737f,
				0.0395357423f,
				0.0397245549f,
				0.0399138033f,
				0.0401034877f,
				0.0402936079f,
				0.0404841676f,
				0.04067516f,
				0.04086659f,
				0.04105846f,
				0.04125076f,
				0.0414434969f,
				0.0416366719f,
				0.0418302827f,
				0.04202433f,
				0.0422188081f,
				0.0424137264f,
				0.0426090769f,
				0.04280486f,
				0.04300108f,
				0.0431977361f,
				0.0433948264f,
				0.0435923524f,
				0.04379031f,
				0.0439887f,
				0.0441875271f,
				0.04438679f,
				0.0445864834f,
				0.04478661f,
				0.0449871719f,
				0.0451881662f,
				0.0453895926f,
				0.045591455f,
				0.04579375f,
				0.0459964722f,
				0.04619963f,
				0.0464032255f,
				0.04660725f,
				0.0468117036f,
				0.04701659f,
				0.04722191f,
				0.04742766f,
				0.0476338454f,
				0.04784046f,
				0.0480475053f,
				0.04825498f,
				0.04846289f,
				0.04867123f,
				0.04888f,
				0.0490892f,
				0.04929883f,
				0.049508892f,
				0.0497193821f,
				0.0499303043f,
				0.0501416549f,
				0.0503534377f,
				0.0505656451f,
				0.05077829f,
				0.0509913564f,
				0.0512048528f,
				0.05141878f,
				0.05163314f,
				0.0518479235f,
				0.0520631373f,
				0.0522787757f,
				0.0524948463f,
				0.0527113453f,
				0.0529282726f,
				0.0531456247f,
				0.0533634052f,
				0.053581614f,
				0.05380025f,
				0.0540193133f,
				0.0542388037f,
				0.0544587225f,
				0.0546790659f,
				0.0548998378f,
				0.0551210344f,
				0.0553426556f,
				0.0555647053f,
				0.05578718f,
				0.0560100824f,
				0.0562334061f,
				0.05645716f,
				0.056681335f,
				0.0569059364f,
				0.0571309663f,
				0.0573564172f,
				0.0575822927f,
				0.0578085929f,
				0.0580353178f,
				0.0582624674f,
				0.05849004f,
				0.05871804f,
				0.05894646f,
				0.0591753051f,
				0.0594045743f,
				0.0596342646f,
				0.0598643757f,
				0.0600949153f,
				0.0603258722f,
				0.0605572537f,
				0.06078906f,
				0.061021287f,
				0.0612539351f,
				0.0614870042f,
				0.0617204979f,
				0.06195441f,
				0.0621887445f,
				0.0624235f,
				0.062658675f,
				0.06289428f,
				0.0631303f,
				0.06336673f,
				0.0636035949f,
				0.06384087f,
				0.06407857f,
				0.06431669f,
				0.06455523f,
				0.06479419f,
				0.06503356f,
				0.06527336f,
				0.06551357f,
				0.06575421f,
				0.06599526f,
				0.0662367344f,
				0.0664786249f,
				0.06672093f,
				0.06696366f,
				0.0672068f,
				0.06745036f,
				0.0676943362f,
				0.06793873f,
				0.06818355f,
				0.06842878f,
				0.06867442f,
				0.06892048f,
				0.06916696f,
				0.0694138557f,
				0.06966116f,
				0.06990889f,
				0.07015703f,
				0.07040559f,
				0.0706545562f,
				0.07090395f,
				0.0711537451f,
				0.0714039654f,
				0.0716545954f,
				0.0719056353f,
				0.0721571f,
				0.07240897f,
				0.07266126f,
				0.07291396f,
				0.07316707f,
				0.0734206f,
				0.07367454f,
				0.0739288852f,
				0.07418365f,
				0.0744388252f,
				0.07469442f,
				0.07495042f,
				0.07520683f,
				0.07546365f,
				0.07572089f,
				0.07597854f,
				0.0762366f,
				0.07649507f,
				0.0767539442f,
				0.07701323f,
				0.07727293f,
				0.0775330439f,
				0.07779356f,
				0.07805449f,
				0.07831583f,
				0.07857758f,
				0.0788397342f,
				0.0791023f,
				0.0793652758f,
				0.07962866f,
				0.07989245f,
				0.08015665f,
				0.080421254f,
				0.08068627f,
				0.08095169f,
				0.08121752f,
				0.08148376f,
				0.0817504f,
				0.08201745f,
				0.08228491f,
				0.0825527757f,
				0.08282104f,
				0.08308972f,
				0.0833587945f,
				0.08362828f,
				0.08389817f,
				0.08416847f,
				0.0844391659f,
				0.08471028f,
				0.0849817842f,
				0.0852537f,
				0.08552601f,
				0.08579873f,
				0.0860718638f,
				0.08634539f,
				0.08661932f,
				0.0868936554f,
				0.0871683955f,
				0.08744353f,
				0.0877190754f,
				0.0879950151f,
				0.0882713646f,
				0.08854811f,
				0.0888252556f,
				0.08910281f,
				0.08938076f,
				0.08965911f,
				0.0899378657f,
				0.09021702f,
				0.09049657f,
				0.0907765254f,
				0.0910568759f,
				0.09133763f,
				0.09161878f,
				0.09190033f,
				0.09218228f,
				0.0924646258f,
				0.09274737f,
				0.09303051f,
				0.09331405f,
				0.09359799f,
				0.09388233f,
				0.09416707f,
				0.0944521949f,
				0.09473772f,
				0.09502365f,
				0.09530997f,
				0.095596686f,
				0.0958838f,
				0.09617131f,
				0.09645921f,
				0.09674751f,
				0.0970362052f,
				0.0973252952f,
				0.09761478f,
				0.09790466f,
				0.09819493f,
				0.0984856f,
				0.09877665f,
				0.09906811f,
				0.09935995f,
				0.09965219f,
				0.09994483f,
				0.100237854f,
				0.100531265f,
				0.100825079f,
				0.101119272f,
				0.101413868f,
				0.101708852f,
				0.102004223f,
				0.102299988f,
				0.102596141f,
				0.102892689f,
				0.103189625f,
				0.103486955f,
				0.103784665f,
				0.104082771f,
				0.104381263f,
				0.104680151f,
				0.104979418f,
				0.105279081f,
				0.10557913f,
				0.105879568f,
				0.106180392f,
				0.106481604f,
				0.1067832f,
				0.107085183f,
				0.107387558f,
				0.107690312f,
				0.107993461f,
				0.10829699f,
				0.108600907f,
				0.108905211f,
				0.109209895f,
				0.109514967f,
				0.109820426f,
				0.110126272f,
				0.1104325f,
				0.110739104f,
				0.1110461f,
				0.111353472f,
				0.111661233f,
				0.111969382f,
				0.1122779f,
				0.112586811f,
				0.112896107f,
				0.113205776f,
				0.113515832f,
				0.113826267f,
				0.114137083f,
				0.114448287f,
				0.114759862f,
				0.115071826f,
				0.115384161f,
				0.115696885f,
				0.11600998f,
				0.116323464f,
				0.116637319f,
				0.116951555f,
				0.117266171f,
				0.117581166f,
				0.117896535f,
				0.11821229f,
				0.118528418f,
				0.118844919f,
				0.1191618f,
				0.11947906f,
				0.119796693f,
				0.120114706f,
				0.120433092f,
				0.120751858f,
				0.121070996f,
				0.121390514f,
				0.1217104f,
				0.122030668f,
				0.1223513f,
				0.122672312f,
				0.1229937f,
				0.123315461f,
				0.123637594f,
				0.1239601f,
				0.124282978f,
				0.124606229f,
				0.124929853f,
				0.125253856f,
				0.125578225f,
				0.125902966f,
				0.126228064f,
				0.12655355f,
				0.126879409f,
				0.127205625f,
				0.127532214f,
				0.127859175f,
				0.128186509f,
				0.128514215f,
				0.128842279f,
				0.129170716f,
				0.129499525f,
				0.1298287f,
				0.130158246f,
				0.130488157f,
				0.130818427f,
				0.131149083f,
				0.131480083f,
				0.13181147f,
				0.132143214f,
				0.132475317f,
				0.132807791f,
				0.133140638f,
				0.133473843f,
				0.1338074f,
				0.134141341f,
				0.134475648f,
				0.134810314f,
				0.135145336f,
				0.135480732f,
				0.135816485f,
				0.1361526f,
				0.136489078f,
				0.136825919f,
				0.137163118f,
				0.137500674f,
				0.1378386f,
				0.138176888f,
				0.138515532f,
				0.138854548f,
				0.1391939f,
				0.139533639f,
				0.139873728f,
				0.140214175f,
				0.140554979f,
				0.140896142f,
				0.141237661f,
				0.141579539f,
				0.141921774f,
				0.142264381f,
				0.142607331f,
				0.142950639f,
				0.1432943f,
				0.143638328f,
				0.1439827f,
				0.144327432f,
				0.144672528f,
				0.145017967f,
				0.145363763f,
				0.145709917f,
				0.146056429f,
				0.146403283f,
				0.1467505f,
				0.147098064f,
				0.147445992f,
				0.147794262f,
				0.148142889f,
				0.148491859f,
				0.148841187f,
				0.149190873f,
				0.1495409f,
				0.149891287f,
				0.150242016f,
				0.1505931f,
				0.150944531f,
				0.151296318f,
				0.151648447f,
				0.152000934f,
				0.152353764f,
				0.152706936f,
				0.153060466f,
				0.153414339f,
				0.153768554f,
				0.154123127f,
				0.154478043f,
				0.1548333f,
				0.1551889f,
				0.155544862f,
				0.155901149f,
				0.1562578f,
				0.15661478f,
				0.15697211f,
				0.1573298f,
				0.157687813f,
				0.158046171f,
				0.158404887f,
				0.15876393f,
				0.159123331f,
				0.15948306f,
				0.159843132f,
				0.160203561f,
				0.160564318f,
				0.160925418f,
				0.161286861f,
				0.161648631f,
				0.162010759f,
				0.162373215f,
				0.162736014f,
				0.163099155f,
				0.163462639f,
				0.163826451f,
				0.1641906f,
				0.164555088f,
				0.164919928f,
				0.165285081f,
				0.165650591f,
				0.16601643f,
				0.1663826f,
				0.16674912f,
				0.167115957f,
				0.167483136f,
				0.167850658f,
				0.168218508f,
				0.168586686f,
				0.1689552f,
				0.169324055f,
				0.169693232f,
				0.170062751f,
				0.1704326f,
				0.170802787f,
				0.171173289f,
				0.171544135f,
				0.171915308f,
				0.172286823f,
				0.172658652f,
				0.173030823f,
				0.173403308f,
				0.173776135f,
				0.17414929f,
				0.174522772f,
				0.174896583f,
				0.175270721f,
				0.175645187f,
				0.176019967f,
				0.176395088f,
				0.176770538f,
				0.177146316f,
				0.1775224f,
				0.177898824f,
				0.17827557f,
				0.178652644f,
				0.179030046f,
				0.179407761f,
				0.1797858f,
				0.180164173f,
				0.180542871f,
				0.180921882f,
				0.181301221f,
				0.181680873f,
				0.182060853f,
				0.18244116f,
				0.18282178f,
				0.183202714f,
				0.183583975f,
				0.183965564f,
				0.184347466f,
				0.18472968f,
				0.185112223f,
				0.185495079f,
				0.185878247f,
				0.186261743f,
				0.186645553f,
				0.187029675f,
				0.187414125f,
				0.187798887f,
				0.188183948f,
				0.188569352f,
				0.188955054f,
				0.189341068f,
				0.189727411f,
				0.190114051f,
				0.190501019f,
				0.1908883f,
				0.19127588f,
				0.191663787f,
				0.192051992f,
				0.192440525f,
				0.192829356f,
				0.1932185f,
				0.193607956f,
				0.193997726f,
				0.194387808f,
				0.1947782f,
				0.1951689f,
				0.1955599f,
				0.195951208f,
				0.196342841f,
				0.196734771f,
				0.197127f,
				0.197519541f,
				0.1979124f,
				0.198305562f,
				0.198699012f,
				0.19909279f,
				0.199486867f,
				0.199881241f,
				0.200275928f,
				0.200670913f,
				0.2010662f,
				0.201461792f,
				0.201857686f,
				0.202253878f,
				0.202650383f,
				0.203047186f,
				0.203444287f,
				0.2038417f,
				0.2042394f,
				0.204637408f,
				0.2050357f,
				0.205434307f,
				0.205833212f,
				0.206232414f,
				0.206631914f,
				0.207031712f,
				0.207431808f,
				0.2078322f,
				0.20823288f,
				0.20863387f,
				0.209035143f,
				0.20943673f,
				0.2098386f,
				0.210240766f,
				0.210643217f,
				0.21104598f,
				0.211449027f,
				0.211852357f,
				0.212256f,
				0.212659925f,
				0.213064134f,
				0.213468641f,
				0.213873446f,
				0.214278534f,
				0.21468392f,
				0.215089589f,
				0.215495542f,
				0.215901792f,
				0.21630834f,
				0.216715157f,
				0.217122272f,
				0.217529684f,
				0.217937365f,
				0.218345344f,
				0.2187536f,
				0.219162151f,
				0.219570979f,
				0.2199801f,
				0.220389515f,
				0.2207992f,
				0.221209168f,
				0.221619427f,
				0.222029954f,
				0.222440779f,
				0.222851887f,
				0.223263264f,
				0.223674938f,
				0.224086881f,
				0.2244991f,
				0.224911615f,
				0.2253244f,
				0.225737482f,
				0.226150826f,
				0.226564452f,
				0.226978347f,
				0.227392539f,
				0.227806985f,
				0.228221729f,
				0.228636742f,
				0.229052022f,
				0.229467586f,
				0.229883432f,
				0.230299547f,
				0.23071593f,
				0.2311326f,
				0.231549531f,
				0.231966734f,
				0.23238422f,
				0.232801974f,
				0.23322f,
				0.2336383f,
				0.23405686f,
				0.2344757f,
				0.234894812f,
				0.23531419f,
				0.235733852f,
				0.236153767f,
				0.236573949f,
				0.2369944f,
				0.23741512f,
				0.237836123f,
				0.238257378f,
				0.238678887f,
				0.23910068f,
				0.23952274f,
				0.239945054f,
				0.240367636f,
				0.240790486f,
				0.2412136f,
				0.241636977f,
				0.242060617f,
				0.242484525f,
				0.242908686f,
				0.2433331f,
				0.2437578f,
				0.244182736f,
				0.244607955f,
				0.245033413f,
				0.245459139f,
				0.245885134f,
				0.246311381f,
				0.246737882f,
				0.247164637f,
				0.247591659f,
				0.248018935f,
				0.248446465f,
				0.248874247f,
				0.2493023f,
				0.249730587f,
				0.250159144f,
				0.25058794f,
				0.251017f,
				0.251446337f,
				0.2518759f,
				0.252305716f,
				0.2527358f,
				0.2531661f,
				0.2535967f,
				0.254027516f,
				0.2544586f,
				0.2548899f,
				0.2553215f,
				0.2557533f,
				0.256185383f,
				0.256617725f,
				0.257050276f,
				0.2574831f,
				0.257916152f,
				0.258349478f,
				0.258783f,
				0.259216815f,
				0.259650856f,
				0.260085166f,
				0.260519683f,
				0.260954469f,
				0.2613895f,
				0.261824757f,
				0.262260258f,
				0.262696f,
				0.263132f,
				0.263568223f,
				0.2640047f,
				0.2644414f,
				0.264878362f,
				0.265315533f,
				0.265752971f,
				0.266190618f,
				0.266628534f,
				0.2670667f,
				0.26750505f,
				0.26794365f,
				0.268382519f,
				0.2688216f,
				0.2692609f,
				0.269700468f,
				0.27014026f,
				0.270580262f,
				0.271020532f,
				0.271461f,
				0.271901727f,
				0.272342682f,
				0.272783875f,
				0.273225278f,
				0.273666918f,
				0.2741088f,
				0.274550885f,
				0.2749932f,
				0.275435776f,
				0.275878578f,
				0.2763216f,
				0.27676484f,
				0.2772083f,
				0.277652f,
				0.2780959f,
				0.278540045f,
				0.278984427f,
				0.279429018f,
				0.279873818f,
				0.2803189f,
				0.280764133f,
				0.281209618f,
				0.281655341f,
				0.282101244f,
				0.2825474f,
				0.282993764f,
				0.283440381f,
				0.283887178f,
				0.2843342f,
				0.284781456f,
				0.2852289f,
				0.2856766f,
				0.2861245f,
				0.2865726f,
				0.287020952f,
				0.2874695f,
				0.28791827f,
				0.288367242f,
				0.288816422f,
				0.289265841f,
				0.289715469f,
				0.290165275f,
				0.29061532f,
				0.291065574f,
				0.291516066f,
				0.291966736f,
				0.292417616f,
				0.2928687f,
				0.29332003f,
				0.293771535f,
				0.294223279f,
				0.2946752f,
				0.295127332f,
				0.295579672f,
				0.29603225f,
				0.296485f,
				0.296937972f,
				0.297391146f,
				0.2978445f,
				0.2982981f,
				0.298751861f,
				0.29920584f,
				0.299660027f,
				0.300114423f,
				0.300569028f,
				0.3010238f,
				0.3014788f,
				0.301934f,
				0.302389383f,
				0.302844971f,
				0.303300768f,
				0.303756773f,
				0.304212958f,
				0.304669321f,
				0.305125922f,
				0.305582672f,
				0.306039661f,
				0.306496829f,
				0.306954175f,
				0.30741173f,
				0.3078695f,
				0.308327436f,
				0.308785558f,
				0.3092439f,
				0.3097024f,
				0.3101611f,
				0.31062f,
				0.3110791f,
				0.3115384f,
				0.311997861f,
				0.312457532f,
				0.312917352f,
				0.3133774f,
				0.313837618f,
				0.314298034f,
				0.314758629f,
				0.3152194f,
				0.315680355f,
				0.316141516f,
				0.316602826f,
				0.317064345f,
				0.317526042f,
				0.317987949f,
				0.31845f,
				0.318912238f,
				0.319374681f,
				0.319837272f,
				0.320300072f,
				0.320763022f,
				0.32122618f,
				0.321689516f,
				0.322153f,
				0.3226167f,
				0.32308054f,
				0.323544562f,
				0.3240088f,
				0.324473172f,
				0.324937731f,
				0.325402468f,
				0.325867385f,
				0.32633245f,
				0.3267977f,
				0.327263117f,
				0.327728719f,
				0.3281945f,
				0.328660429f,
				0.329126537f,
				0.329592824f,
				0.3300593f,
				0.3305259f,
				0.330992669f,
				0.331459641f,
				0.331926763f,
				0.332394034f,
				0.3328615f,
				0.3333291f,
				0.3337969f,
				0.334264845f,
				0.33473298f,
				0.335201234f,
				0.3356697f,
				0.336138278f,
				0.336607039f,
				0.337075979f,
				0.337545067f,
				0.3380143f,
				0.338483721f,
				0.3389533f,
				0.339423f,
				0.3398929f,
				0.340362936f,
				0.340833127f,
				0.3413035f,
				0.341774f,
				0.342244655f,
				0.342715472f,
				0.343186468f,
				0.343657583f,
				0.344128877f,
				0.34460032f,
				0.3450719f,
				0.345543653f,
				0.346015543f,
				0.346487582f,
				0.34695977f,
				0.347432137f,
				0.347904623f,
				0.348377258f,
				0.348850042f,
				0.349323f,
				0.3497961f,
				0.350269318f,
				0.3507427f,
				0.351216227f,
				0.351689875f,
				0.3521637f,
				0.352637649f,
				0.353111744f,
				0.353586f,
				0.354060382f,
				0.354534924f,
				0.3550096f,
				0.3554844f,
				0.355959356f,
				0.356434435f,
				0.356909662f,
				0.357385039f,
				0.357860535f,
				0.35833618f,
				0.358811975f,
				0.3592879f,
				0.35976395f,
				0.360240132f,
				0.360716462f,
				0.3611929f,
				0.3616695f,
				0.362146229f,
				0.3626231f,
				0.363100082f,
				0.3635772f,
				0.364054441f,
				0.364531845f,
				0.365009338f,
				0.365486979f,
				0.36596477f,
				0.366442651f,
				0.36692068f,
				0.367398858f,
				0.367877126f,
				0.368355542f,
				0.368834078f,
				0.369312733f,
				0.3697915f,
				0.370270431f,
				0.370749444f,
				0.3712286f,
				0.3717079f,
				0.3721873f,
				0.3726668f,
				0.373146445f,
				0.3736262f,
				0.374106079f,
				0.374586076f,
				0.3750662f,
				0.375546426f,
				0.376026779f,
				0.376507252f,
				0.376987845f,
				0.377468556f,
				0.377949357f,
				0.3784303f,
				0.378911346f,
				0.3793925f,
				0.379873782f,
				0.380355179f,
				0.380836666f,
				0.381318271f,
				0.3818f,
				0.3822818f,
				0.382763773f,
				0.383245826f,
				0.383727968f,
				0.384210229f,
				0.3846926f,
				0.385175079f,
				0.385657668f,
				0.386140376f,
				0.386623174f,
				0.387106061f,
				0.387589067f,
				0.3880722f,
				0.3885554f,
				0.3890387f,
				0.389522135f,
				0.390005648f,
				0.39048928f,
				0.390973f,
				0.3914568f,
				0.391940743f,
				0.392424762f,
				0.392908871f,
				0.39339307f,
				0.3938774f,
				0.3943618f,
				0.3948463f,
				0.3953309f,
				0.3958156f,
				0.396300375f,
				0.396785259f,
				0.397270232f,
				0.3977553f,
				0.398240477f,
				0.398725718f,
				0.399211049f,
				0.399696469f,
				0.400182f,
				0.4006676f,
				0.401153326f,
				0.4016391f,
				0.402124971f,
				0.402610928f,
				0.403096974f,
				0.4035831f,
				0.404069334f,
				0.404555619f,
				0.405042022f,
				0.4055285f,
				0.406015038f,
				0.406501681f,
				0.406988382f,
				0.407475173f,
				0.407962054f,
				0.408449024f,
				0.408936054f,
				0.409423172f,
				0.409910381f,
				0.410397649f,
				0.410885f,
				0.411372423f,
				0.41185993f,
				0.412347525f,
				0.412835151f,
				0.4133229f,
				0.4138107f,
				0.414298564f,
				0.414786518f,
				0.415274531f,
				0.415762633f,
				0.4162508f,
				0.416739017f,
				0.417227328f,
				0.4177157f,
				0.418204159f,
				0.418692648f,
				0.419181228f,
				0.419669867f,
				0.4201586f,
				0.420647383f,
				0.4211362f,
				0.4216251f,
				0.4221141f,
				0.42260313f,
				0.423092216f,
				0.4235814f,
				0.4240706f,
				0.4245599f,
				0.425049245f,
				0.425538629f,
				0.4260281f,
				0.426517636f,
				0.4270072f,
				0.42749685f,
				0.427986532f,
				0.4284763f,
				0.4289661f,
				0.429455966f,
				0.4299459f,
				0.430435866f,
				0.4309259f,
				0.431415975f,
				0.4319061f,
				0.4323963f,
				0.432886541f,
				0.433376849f,
				0.4338672f,
				0.434357554f,
				0.434848f,
				0.4353385f,
				0.435829043f,
				0.436319619f,
				0.436810255f,
				0.43730092f,
				0.437791646f,
				0.43828243f,
				0.438773245f,
				0.4392641f,
				0.439755f,
				0.440245926f,
				0.44073692f,
				0.441227943f,
				0.441719025f,
				0.442210138f,
				0.44270128f,
				0.443192482f,
				0.4436837f,
				0.444174975f,
				0.444666266f,
				0.445157617f,
				0.445649f,
				0.4461404f,
				0.446631849f,
				0.447123349f,
				0.447614878f,
				0.4481064f,
				0.448598f,
				0.449089617f,
				0.4495813f,
				0.450072974f,
				0.450564682f,
				0.451056421f,
				0.4515482f,
				0.452040017f,
				0.452531844f,
				0.4530237f,
				0.4535156f,
				0.4540075f,
				0.454499424f,
				0.4549914f,
				0.4554834f,
				0.4559754f,
				0.45646745f,
				0.456959516f,
				0.4574516f,
				0.4579437f,
				0.458435833f,
				0.458928f,
				0.459420174f,
				0.45991236f,
				0.460404575f,
				0.4608968f,
				0.461389035f,
				0.4618813f,
				0.462373585f,
				0.4628659f,
				0.4633582f,
				0.463850528f,
				0.464342862f,
				0.464835227f,
				0.4653276f,
				0.465819985f,
				0.466312379f,
				0.4668048f,
				0.467297226f,
				0.46778965f,
				0.468282074f,
				0.468774527f,
				0.469267f,
				0.469759464f,
				0.470251948f,
				0.470744431f,
				0.4712369f,
				0.4717294f,
				0.4722219f,
				0.472714424f,
				0.4732069f,
				0.473699421f,
				0.474191964f,
				0.474684477f,
				0.475177f,
				0.4756695f,
				0.476162046f,
				0.476654559f,
				0.477147073f,
				0.477639616f,
				0.478132129f,
				0.478624642f,
				0.479117155f,
				0.479609668f,
				0.480102181f,
				0.480594665f,
				0.481087178f,
				0.481579661f,
				0.482072145f,
				0.482564628f,
				0.4830571f,
				0.483549565f,
				0.484042019f,
				0.484534472f,
				0.485026926f,
				0.48551935f,
				0.486011744f,
				0.486504167f,
				0.486996561f,
				0.487488925f,
				0.4879813f,
				0.488473654f,
				0.488966f,
				0.489458323f,
				0.489950627f,
				0.490442932f,
				0.4909352f,
				0.491427451f,
				0.4919197f,
				0.4924119f,
				0.492904127f,
				0.4933963f,
				0.493888468f,
				0.494380623f,
				0.494872719f,
				0.495364845f,
				0.4958569f,
				0.496348977f,
				0.496840984f,
				0.497333f,
				0.497825f,
				0.498316944f,
				0.498808861f,
				0.499300778f,
				0.499792665f,
				0.5002845f,
				0.50077635f,
				0.5012681f,
				0.5017599f,
				0.5022516f,
				0.5027433f,
				0.503235f,
				0.5037266f,
				0.5042182f,
				0.5047098f,
				0.50520134f,
				0.50569284f,
				0.50618434f,
				0.5066758f,
				0.507167161f,
				0.507658541f,
				0.508149862f,
				0.5086411f,
				0.5091324f,
				0.509623647f,
				0.5101148f,
				0.510605931f,
				0.511097f,
				0.5115881f,
				0.51207906f,
				0.5125701f,
				0.513061f,
				0.5135519f,
				0.514042735f,
				0.5145335f,
				0.515024245f,
				0.51551497f,
				0.516005635f,
				0.516496241f,
				0.5169868f,
				0.517477334f,
				0.517967761f,
				0.5184582f,
				0.518948555f,
				0.5194389f,
				0.5199292f,
				0.520419359f,
				0.520909548f,
				0.5213997f,
				0.521889746f,
				0.5223797f,
				0.5228697f,
				0.5233596f,
				0.5238494f,
				0.5243392f,
				0.5248289f,
				0.5253186f,
				0.5258082f,
				0.5262978f,
				0.5267873f,
				0.5272767f,
				0.5277661f,
				0.5282554f,
				0.5287447f,
				0.5292339f,
				0.529723f,
				0.530212045f,
				0.5307011f,
				0.531190038f,
				0.5316789f,
				0.532167733f,
				0.532656431f,
				0.5331451f,
				0.533633769f,
				0.5341223f,
				0.534610748f,
				0.5350992f,
				0.535587549f,
				0.53607583f,
				0.536564f,
				0.537052155f,
				0.5375402f,
				0.5380282f,
				0.5385161f,
				0.5390039f,
				0.5394917f,
				0.5399794f,
				0.540467f,
				0.54095453f,
				0.541442037f,
				0.5419294f,
				0.542416751f,
				0.54290396f,
				0.5433911f,
				0.5438782f,
				0.5443652f,
				0.544852138f,
				0.545339f,
				0.5458257f,
				0.5463124f,
				0.546799f,
				0.5472855f,
				0.547771931f,
				0.5482583f,
				0.548744559f,
				0.5492307f,
				0.54971683f,
				0.5502028f,
				0.550688744f,
				0.5511746f,
				0.5516603f,
				0.552145958f,
				0.5526315f,
				0.553117f,
				0.5536024f,
				0.554087639f,
				0.5545729f,
				0.555058f,
				0.555543f,
				0.5560279f,
				0.5565128f,
				0.5569975f,
				0.5574821f,
				0.5579667f,
				0.5584511f,
				0.558935463f,
				0.559419751f,
				0.5599039f,
				0.560387969f,
				0.560871959f,
				0.5613558f,
				0.5618396f,
				0.5623232f,
				0.5628068f,
				0.563290238f,
				0.563773632f,
				0.5642569f,
				0.564740062f,
				0.5652231f,
				0.565706f,
				0.5661889f,
				0.5666716f,
				0.5671543f,
				0.5676368f,
				0.5681192f,
				0.568601549f,
				0.56908375f,
				0.569565833f,
				0.5700478f,
				0.5705297f,
				0.5710114f,
				0.5714931f,
				0.571974635f,
				0.572456062f,
				0.5729374f,
				0.5734186f,
				0.5738997f,
				0.5743807f,
				0.5748615f,
				0.5753423f,
				0.575822949f,
				0.5763034f,
				0.576783836f,
				0.57726413f,
				0.5777443f,
				0.578224361f,
				0.5787043f,
				0.579184055f,
				0.579663754f,
				0.580143332f,
				0.5806228f,
				0.5811021f,
				0.5815813f,
				0.5820604f,
				0.5825393f,
				0.5830182f,
				0.583496869f,
				0.583975434f,
				0.5844539f,
				0.584932268f,
				0.5854104f,
				0.5858885f,
				0.5863665f,
				0.586844265f,
				0.587322f,
				0.587799549f,
				0.588277f,
				0.588754237f,
				0.589231431f,
				0.589708447f,
				0.5901854f,
				0.5906621f,
				0.5911388f,
				0.5916153f,
				0.5920917f,
				0.5925679f,
				0.593044f,
				0.59352f,
				0.5939958f,
				0.5944715f,
				0.59494704f,
				0.595422447f,
				0.595897734f,
				0.596372843f,
				0.5968479f,
				0.5973227f,
				0.597797453f,
				0.598272f,
				0.5987465f,
				0.599220753f,
				0.5996949f,
				0.6001689f,
				0.600642741f,
				0.6011165f,
				0.601590037f,
				0.6020635f,
				0.602536738f,
				0.6030099f,
				0.603482842f,
				0.6039557f,
				0.6044284f,
				0.604900956f,
				0.6053733f,
				0.6058456f,
				0.606317639f,
				0.6067896f,
				0.60726136f,
				0.607733f,
				0.6082045f,
				0.608675838f,
				0.609147f,
				0.609618f,
				0.6100889f,
				0.6105596f,
				0.611030161f,
				0.611500561f,
				0.611970842f,
				0.6124409f,
				0.6129108f,
				0.6133806f,
				0.613850236f,
				0.6143197f,
				0.614788949f,
				0.6152581f,
				0.615727067f,
				0.616195858f,
				0.6166645f,
				0.617133f,
				0.617601335f,
				0.61806947f,
				0.6185375f,
				0.6190053f,
				0.619473f,
				0.61994046f,
				0.6204078f,
				0.620875f,
				0.621342f,
				0.6218088f,
				0.6222755f,
				0.622742f,
				0.6232083f,
				0.623674452f,
				0.624140441f,
				0.624606252f,
				0.6250719f,
				0.6255374f,
				0.626002669f,
				0.6264678f,
				0.62693274f,
				0.627397537f,
				0.627862155f,
				0.6283266f,
				0.628790855f,
				0.629254937f,
				0.62971884f,
				0.630182564f,
				0.6306461f,
				0.6311095f,
				0.631572664f,
				0.6320357f,
				0.6324985f,
				0.632961154f,
				0.6334236f,
				0.6338859f,
				0.634348035f,
				0.63481f,
				0.6352717f,
				0.6357333f,
				0.636194646f,
				0.636655867f,
				0.6371169f,
				0.6375777f,
				0.638038337f,
				0.6384988f,
				0.6389591f,
				0.639419138f,
				0.639879048f,
				0.6403388f,
				0.6407983f,
				0.641257644f,
				0.6417168f,
				0.642175734f,
				0.642634451f,
				0.64309305f,
				0.6435514f,
				0.6440096f,
				0.6444676f,
				0.6449254f,
				0.645383f,
				0.6458404f,
				0.646297634f,
				0.6467547f,
				0.6472115f,
				0.6476681f,
				0.6481246f,
				0.648580849f,
				0.6490369f,
				0.649492741f,
				0.649948359f,
				0.6504038f,
				0.650859058f,
				0.651314139f,
				0.651769f,
				0.652223647f,
				0.6526781f,
				0.6531323f,
				0.6535864f,
				0.6540402f,
				0.654493868f,
				0.6549473f,
				0.6554005f,
				0.6558535f,
				0.6563064f,
				0.656758964f,
				0.657211363f,
				0.6576636f,
				0.658115566f,
				0.658567369f,
				0.659019f,
				0.6594703f,
				0.6599215f,
				0.660372436f,
				0.6608232f,
				0.6612737f,
				0.6617241f,
				0.662174165f,
				0.662624061f,
				0.6630738f,
				0.663523257f,
				0.6639725f,
				0.664421558f,
				0.6648704f,
				0.665319f,
				0.665767431f,
				0.666215658f,
				0.6666636f,
				0.6671114f,
				0.6675589f,
				0.668006241f,
				0.668453336f,
				0.668900251f,
				0.6693469f,
				0.669793367f,
				0.6702396f,
				0.670685649f,
				0.671131432f,
				0.671577f,
				0.672022343f,
				0.67246747f,
				0.672912359f,
				0.673357069f,
				0.673801541f,
				0.6742458f,
				0.67468977f,
				0.6751336f,
				0.6755771f,
				0.676020443f,
				0.6764636f,
				0.676906466f,
				0.67734915f,
				0.677791536f,
				0.678233743f,
				0.6786757f,
				0.6791175f,
				0.679559f,
				0.6800003f,
				0.6804413f,
				0.680882156f,
				0.681322753f,
				0.6817631f,
				0.6822033f,
				0.6826432f,
				0.6830828f,
				0.6835223f,
				0.6839615f,
				0.684400439f,
				0.6848392f,
				0.6852777f,
				0.685716f,
				0.686154f,
				0.6865918f,
				0.687029362f,
				0.6874667f,
				0.687903762f,
				0.6883406f,
				0.6887772f,
				0.6892136f,
				0.6896497f,
				0.6900856f,
				0.69052124f,
				0.690956652f,
				0.6913918f,
				0.6918267f,
				0.6922614f,
				0.692695856f,
				0.69313f,
				0.693564f,
				0.693997741f,
				0.6944312f,
				0.6948644f,
				0.69529736f,
				0.69573015f,
				0.6961626f,
				0.696594834f,
				0.697026849f,
				0.6974586f,
				0.6978901f,
				0.698321342f,
				0.698752344f,
				0.6991831f,
				0.699613631f,
				0.700043857f,
				0.7004739f,
				0.700903654f,
				0.701333165f,
				0.701762438f,
				0.7021914f,
				0.702620149f,
				0.7030487f,
				0.7034769f,
				0.7039049f,
				0.70433265f,
				0.704760134f,
				0.7051874f,
				0.7056144f,
				0.7060411f,
				0.706467569f,
				0.706893742f,
				0.707319736f,
				0.707745433f,
				0.7081709f,
				0.708596051f,
				0.709021f,
				0.709445655f,
				0.70987004f,
				0.7102942f,
				0.7107181f,
				0.7111417f,
				0.7115651f,
				0.7119882f,
				0.712411046f,
				0.712833643f,
				0.713255942f,
				0.713678f,
				0.7140998f,
				0.714521348f,
				0.714942634f,
				0.7153636f,
				0.7157844f,
				0.7162049f,
				0.7166251f,
				0.717045f,
				0.7174647f,
				0.7178841f,
				0.718303263f,
				0.718722165f,
				0.719140768f,
				0.719559133f,
				0.7199772f,
				0.720395f,
				0.720812559f,
				0.721229851f,
				0.721646845f,
				0.722063541f,
				0.72248f,
				0.7228962f,
				0.72331214f,
				0.723727763f,
				0.724143147f,
				0.724558234f,
				0.7249731f,
				0.725387633f,
				0.725801945f,
				0.726215959f,
				0.7266297f,
				0.727043152f,
				0.727456331f,
				0.7278692f,
				0.728281856f,
				0.7286942f,
				0.7291063f,
				0.7295181f,
				0.7299296f,
				0.7303409f,
				0.7307519f,
				0.731162548f,
				0.731573f,
				0.7319831f,
				0.732392967f,
				0.73280257f,
				0.7332118f,
				0.7336209f,
				0.7340296f,
				0.734438062f,
				0.734846234f,
				0.7352541f,
				0.735661745f,
				0.736069f,
				0.736476064f,
				0.736882865f,
				0.7372893f,
				0.7376955f,
				0.7381014f,
				0.738507032f,
				0.7389124f,
				0.7393174f,
				0.7397222f,
				0.740126669f,
				0.740530849f,
				0.7409348f,
				0.741338432f,
				0.7417417f,
				0.742144763f,
				0.7425475f,
				0.74295f,
				0.7433522f,
				0.7437541f,
				0.744155645f,
				0.744556963f,
				0.744958f,
				0.7453587f,
				0.7457592f,
				0.7461593f,
				0.746559143f,
				0.746958733f,
				0.747358f,
				0.747756958f,
				0.748155653f,
				0.748554051f,
				0.74895215f,
				0.749349952f,
				0.749747455f,
				0.75014466f,
				0.7505416f,
				0.750938237f,
				0.751334548f,
				0.751730561f,
				0.752126336f,
				0.752521753f,
				0.752916932f,
				0.753311753f,
				0.7537063f,
				0.754100561f,
				0.7544945f,
				0.7548882f,
				0.7552815f,
				0.7556746f,
				0.756067336f,
				0.7564598f,
				0.756852f,
				0.7572438f,
				0.757635355f,
				0.7580266f,
				0.758417547f,
				0.7588082f,
				0.759198546f,
				0.7595886f,
				0.759978354f,
				0.7603678f,
				0.760756969f,
				0.7611458f,
				0.761534333f,
				0.761922538f,
				0.762310445f,
				0.7626981f,
				0.7630854f,
				0.763472438f,
				0.7638591f,
				0.7642455f,
				0.7646316f,
				0.7650174f,
				0.765402853f,
				0.765788f,
				0.7661729f,
				0.766557455f,
				0.7669417f,
				0.76732564f,
				0.7677093f,
				0.768092632f,
				0.768475652f,
				0.7688583f,
				0.769240737f,
				0.7696228f,
				0.7700046f,
				0.77038604f,
				0.7707672f,
				0.771148f,
				0.7715286f,
				0.7719088f,
				0.7722887f,
				0.7726683f,
				0.773047566f,
				0.773426533f,
				0.7738052f,
				0.7741836f,
				0.7745616f,
				0.7749393f,
				0.7753167f,
				0.7756938f,
				0.776070535f,
				0.776447f,
				0.776823163f,
				0.777199f,
				0.7775745f,
				0.7779497f,
				0.778324544f,
				0.7786991f,
				0.779073358f,
				0.7794473f,
				0.7798209f,
				0.7801942f,
				0.780567169f,
				0.7809398f,
				0.781312168f,
				0.78168416f,
				0.7820559f,
				0.782427251f,
				0.78279835f,
				0.7831691f,
				0.7835395f,
				0.7839096f,
				0.7842794f,
				0.784648836f,
				0.785017967f,
				0.7853868f,
				0.7857553f,
				0.786123455f,
				0.786491334f,
				0.786858857f,
				0.7872261f,
				0.787592947f,
				0.7879595f,
				0.7883257f,
				0.78869164f,
				0.789057255f,
				0.7894225f,
				0.7897875f,
				0.7901521f,
				0.7905164f,
				0.7908803f,
				0.791244f,
				0.7916073f,
				0.7919703f,
				0.792332947f,
				0.7926953f,
				0.7930573f,
				0.793419f,
				0.7937804f,
				0.7941414f,
				0.794502139f,
				0.7948625f,
				0.7952226f,
				0.7955823f,
				0.795941651f,
				0.7963007f,
				0.79665947f,
				0.7970179f,
				0.797376f,
				0.7977337f,
				0.7980911f,
				0.7984482f,
				0.798805f,
				0.799161434f,
				0.7995175f,
				0.7998733f,
				0.8002287f,
				0.800583839f,
				0.8009386f,
				0.8012931f,
				0.8016472f,
				0.802001f,
				0.802354455f,
				0.802707553f,
				0.803060353f,
				0.8034128f,
				0.8037649f,
				0.804116666f,
				0.804468155f,
				0.8048193f,
				0.805170059f,
				0.8055205f,
				0.8058706f,
				0.8062204f,
				0.8065699f,
				0.806919f,
				0.8072677f,
				0.8076162f,
				0.8079643f,
				0.808312058f,
				0.8086595f,
				0.809006631f,
				0.8093534f,
				0.809699833f,
				0.8100459f,
				0.810391665f,
				0.8107371f,
				0.8110822f,
				0.8114269f,
				0.811771333f,
				0.8121154f,
				0.8124591f,
				0.8128025f,
				0.8131456f,
				0.8134883f,
				0.8138307f,
				0.8141727f,
				0.8145144f,
				0.814855754f,
				0.8151968f,
				0.815537453f,
				0.8158778f,
				0.81621784f,
				0.816557467f,
				0.8168968f,
				0.817235768f,
				0.817574441f,
				0.817912757f,
				0.8182507f,
				0.8185883f,
				0.8189256f,
				0.8192625f,
				0.819599152f,
				0.8199354f,
				0.8202713f,
				0.8206069f,
				0.8209421f,
				0.821276963f,
				0.8216115f,
				0.8219457f,
				0.8222796f,
				0.8226131f,
				0.82294625f,
				0.8232791f,
				0.823611557f,
				0.823943734f,
				0.824275553f,
				0.824606955f,
				0.8249381f,
				0.825268865f,
				0.825599253f,
				0.825929344f,
				0.8262591f,
				0.826588452f,
				0.8269175f,
				0.8272462f,
				0.827574551f,
				0.827902555f,
				0.828230262f,
				0.828557551f,
				0.828884542f,
				0.8292111f,
				0.829537451f,
				0.829863369f,
				0.83018893f,
				0.8305142f,
				0.8308391f,
				0.8311636f,
				0.831487834f,
				0.831811666f,
				0.832135141f,
				0.8324583f,
				0.832781136f,
				0.8331036f,
				0.8334257f,
				0.833747447f,
				0.8340689f,
				0.83439f,
				0.834710658f,
				0.835031033f,
				0.8353511f,
				0.835670769f,
				0.8359901f,
				0.8363091f,
				0.8366277f,
				0.836946f,
				0.837263942f,
				0.8375815f,
				0.837898731f,
				0.8382156f,
				0.83853215f,
				0.838848352f,
				0.839164138f,
				0.8394796f,
				0.8397948f,
				0.8401096f,
				0.840424f,
				0.840738058f,
				0.8410518f,
				0.8413652f,
				0.841678262f,
				0.8419909f,
				0.8423032f,
				0.842615247f,
				0.84292686f,
				0.8432381f,
				0.8435491f,
				0.8438596f,
				0.844169855f,
				0.84447974f,
				0.844789267f,
				0.845098436f,
				0.845407248f,
				0.8457157f,
				0.8460238f,
				0.846331537f,
				0.846639f,
				0.846946f,
				0.8472527f,
				0.8475591f,
				0.847865045f,
				0.8481707f,
				0.848476f,
				0.84878093f,
				0.8490855f,
				0.849389732f,
				0.849693656f,
				0.849997163f,
				0.8503003f,
				0.850603163f,
				0.8509056f,
				0.851207733f,
				0.8515095f,
				0.8518109f,
				0.852111936f,
				0.852412641f,
				0.852713f,
				0.853013f,
				0.8533126f,
				0.8536119f,
				0.8539108f,
				0.854209363f,
				0.8545076f,
				0.85480547f,
				0.855102956f,
				0.855400145f,
				0.8556969f,
				0.8559934f,
				0.856289446f,
				0.8565852f,
				0.8568806f,
				0.8571756f,
				0.8574703f,
				0.8577646f,
				0.8580586f,
				0.8583522f,
				0.8586454f,
				0.8589383f,
				0.8592308f,
				0.859523f,
				0.8598149f,
				0.860106349f,
				0.860397458f,
				0.8606882f,
				0.8609786f,
				0.86126864f,
				0.8615584f,
				0.8618477f,
				0.862136662f,
				0.8624253f,
				0.8627136f,
				0.8630015f,
				0.863289058f,
				0.8635763f,
				0.8638631f,
				0.86414963f,
				0.864435732f,
				0.864721537f,
				0.8650069f,
				0.865292f,
				0.865576744f,
				0.865861058f,
				0.8661451f,
				0.866428733f,
				0.866712034f,
				0.8669949f,
				0.8672775f,
				0.867559731f,
				0.8678416f,
				0.8681231f,
				0.868404269f,
				0.868685f,
				0.868965447f,
				0.8692455f,
				0.869525254f,
				0.8698046f,
				0.87008363f,
				0.8703623f,
				0.8706406f,
				0.8709185f,
				0.8711961f,
				0.8714733f,
				0.8717502f,
				0.8720267f,
				0.87230283f,
				0.8725786f,
				0.872854054f,
				0.8731292f,
				0.8734039f,
				0.873678267f,
				0.87395227f,
				0.8742259f,
				0.8744992f,
				0.8747722f,
				0.875044763f,
				0.875317f,
				0.875588834f,
				0.875860333f,
				0.876131535f,
				0.8764023f,
				0.876672745f,
				0.8769429f,
				0.8772126f,
				0.877481937f,
				0.877750933f,
				0.878019631f,
				0.8782879f,
				0.8785559f,
				0.878823459f,
				0.879090667f,
				0.8793576f,
				0.879624069f,
				0.879890263f,
				0.88015604f,
				0.88042146f,
				0.8806866f,
				0.8809513f,
				0.8812157f,
				0.8814797f,
				0.8817434f,
				0.882006645f,
				0.8822696f,
				0.882532239f,
				0.88279444f,
				0.883056343f,
				0.8833178f,
				0.883579f,
				0.883839846f,
				0.884100258f,
				0.8843604f,
				0.88462013f,
				0.8848795f,
				0.8851385f,
				0.8853972f,
				0.8856555f,
				0.8859135f,
				0.8861711f,
				0.8864283f,
				0.8866852f,
				0.886941731f,
				0.8871979f,
				0.887453735f,
				0.8877092f,
				0.8879643f,
				0.888219059f,
				0.888473451f,
				0.8887275f,
				0.888981164f,
				0.889234543f,
				0.8894875f,
				0.8897401f,
				0.889992356f,
				0.8902443f,
				0.890495837f,
				0.890747f,
				0.8909979f,
				0.891248345f,
				0.891498446f,
				0.89174825f,
				0.8919977f,
				0.8922467f,
				0.892495453f,
				0.892743766f,
				0.8929918f,
				0.893239439f,
				0.893486738f,
				0.8937336f,
				0.8939802f,
				0.894226432f,
				0.8944723f,
				0.8947178f,
				0.894962966f,
				0.895207763f,
				0.8954522f,
				0.895696342f,
				0.895940065f,
				0.896183431f,
				0.8964265f,
				0.896669149f,
				0.8969115f,
				0.897153437f,
				0.8973951f,
				0.8976363f,
				0.8978772f,
				0.8981178f,
				0.898358f,
				0.898597836f,
				0.8988373f,
				0.899076462f,
				0.899315238f,
				0.899553657f,
				0.8997918f,
				0.9000295f,
				0.9002668f,
				0.9005039f,
				0.9007405f,
				0.900976837f,
				0.9012128f,
				0.9014484f,
				0.9016837f,
				0.9019186f,
				0.902153134f,
				0.9023873f,
				0.90262115f,
				0.9028547f,
				0.9030878f,
				0.9033206f,
				0.903553f,
				0.9037851f,
				0.904016852f,
				0.904248238f,
				0.904479265f,
				0.904709935f,
				0.904940248f,
				0.905170262f,
				0.905399859f,
				0.905629158f,
				0.90585804f,
				0.9060866f,
				0.90631485f,
				0.9065427f,
				0.9067702f,
				0.9069974f,
				0.907224238f,
				0.9074507f,
				0.9076768f,
				0.9079026f,
				0.908128f,
				0.9083531f,
				0.9085778f,
				0.908802152f,
				0.909026146f,
				0.909249842f,
				0.9094732f,
				0.909696162f,
				0.9099188f,
				0.910141051f,
				0.910362959f,
				0.9105845f,
				0.910805762f,
				0.911026657f,
				0.9112472f,
				0.9114674f,
				0.9116872f,
				0.91190666f,
				0.9121258f,
				0.9123446f,
				0.912563f,
				0.9127811f,
				0.912998855f,
				0.9132163f,
				0.9134333f,
				0.913650036f,
				0.9138664f,
				0.9140824f,
				0.914298058f,
				0.9145134f,
				0.914728343f,
				0.914943f,
				0.915157259f,
				0.9153712f,
				0.9155848f,
				0.915798f,
				0.9160109f,
				0.916223466f,
				0.916435659f,
				0.916647553f,
				0.916859031f,
				0.9170702f,
				0.917281032f,
				0.917491555f,
				0.917701662f,
				0.91791147f,
				0.9181209f,
				0.91833f,
				0.918538749f,
				0.9187472f,
				0.918955266f,
				0.919163f,
				0.919370353f,
				0.9195774f,
				0.9197841f,
				0.9199905f,
				0.9201965f,
				0.920402169f,
				0.920607448f,
				0.9208125f,
				0.9210171f,
				0.9212214f,
				0.921425343f,
				0.921628952f,
				0.921832263f,
				0.9220352f,
				0.922237754f,
				0.922440052f,
				0.922641933f,
				0.9228435f,
				0.923044741f,
				0.9232456f,
				0.9234462f,
				0.9236464f,
				0.923846245f,
				0.9240458f,
				0.924245f,
				0.924443841f,
				0.9246423f,
				0.9248405f,
				0.925038338f,
				0.9252358f,
				0.925433f,
				0.9256298f,
				0.925826252f,
				0.9260224f,
				0.9262182f,
				0.926413655f,
				0.9266088f,
				0.9268036f,
				0.926998f,
				0.927192152f,
				0.9273859f,
				0.927579343f,
				0.927772462f,
				0.9279652f,
				0.9281576f,
				0.928349733f,
				0.9285415f,
				0.928732932f,
				0.928924f,
				0.929114759f,
				0.9293052f,
				0.9294953f,
				0.929685f,
				0.9298744f,
				0.9300635f,
				0.930252254f,
				0.9304406f,
				0.9306287f,
				0.9308165f,
				0.931003869f,
				0.9311909f,
				0.931377649f,
				0.9315641f,
				0.9317501f,
				0.9319359f,
				0.9321213f,
				0.932306349f,
				0.9324911f,
				0.93267554f,
				0.9328596f,
				0.933043361f,
				0.933226764f,
				0.93340987f,
				0.9335926f,
				0.933775067f,
				0.93395716f,
				0.9341389f,
				0.934320331f,
				0.934501469f,
				0.9346822f,
				0.9348627f,
				0.9350428f,
				0.935222566f,
				0.935402036f,
				0.935581148f,
				0.935759962f,
				0.9359384f,
				0.9361166f,
				0.9362944f,
				0.9364719f,
				0.936649f,
				0.9368259f,
				0.937002361f,
				0.937178552f,
				0.9373544f,
				0.9375299f,
				0.937705159f,
				0.93788f,
				0.938054562f,
				0.9382288f,
				0.938402653f,
				0.9385763f,
				0.9387495f,
				0.9389224f,
				0.939095f,
				0.9392673f,
				0.939439237f,
				0.9396109f,
				0.9397822f,
				0.939953148f,
				0.9401238f,
				0.940294147f,
				0.940464139f,
				0.940633833f,
				0.94080323f,
				0.940972269f,
				0.94114095f,
				0.9413094f,
				0.9414775f,
				0.9416452f,
				0.941812634f,
				0.941979766f,
				0.94214654f,
				0.942313f,
				0.9424792f,
				0.942645f,
				0.942810535f,
				0.9429757f,
				0.9431406f,
				0.943305135f,
				0.9434694f,
				0.9436333f,
				0.943796933f,
				0.9439602f,
				0.944123149f,
				0.9442858f,
				0.9444482f,
				0.9446102f,
				0.9447719f,
				0.944933236f,
				0.9450943f,
				0.945255041f,
				0.9454155f,
				0.9455756f,
				0.9457354f,
				0.9458949f,
				0.9460541f,
				0.946212947f,
				0.9463715f,
				0.9465297f,
				0.946687639f,
				0.946845233f,
				0.94700253f,
				0.947159469f,
				0.9473161f,
				0.9474725f,
				0.9476285f,
				0.947784245f,
				0.947939634f,
				0.9480948f,
				0.9482495f,
				0.948404f,
				0.9485582f,
				0.948712051f,
				0.9488656f,
				0.949018836f,
				0.9491717f,
				0.949324369f,
				0.949476659f,
				0.949628651f,
				0.949780345f,
				0.949931741f,
				0.9500828f,
				0.9502336f,
				0.950384f,
				0.950534165f,
				0.950684f,
				0.950833559f,
				0.950982749f,
				0.9511317f,
				0.9512803f,
				0.9514286f,
				0.9515766f,
				0.9517243f,
				0.9518717f,
				0.952018738f,
				0.952165544f,
				0.952312f,
				0.9524582f,
				0.952604055f,
				0.9527496f,
				0.952894866f,
				0.9530398f,
				0.9531845f,
				0.953328848f,
				0.953472853f,
				0.9536166f,
				0.9537601f,
				0.9539032f,
				0.9540461f,
				0.9541886f,
				0.9543308f,
				0.9544728f,
				0.9546144f,
				0.9547557f,
				0.954896748f,
				0.955037534f,
				0.955177963f,
				0.9553181f,
				0.9554579f,
				0.95559746f,
				0.9557367f,
				0.955875635f,
				0.956014335f,
				0.9561527f,
				0.9562907f,
				0.956428468f,
				0.9565659f,
				0.9567031f,
				0.95684f,
				0.956976533f,
				0.957112849f,
				0.9572488f,
				0.957384467f,
				0.9575199f,
				0.957655f,
				0.9577898f,
				0.9579243f,
				0.958058536f,
				0.958192468f,
				0.9583261f,
				0.958459437f,
				0.9585925f,
				0.9587252f,
				0.9588577f,
				0.958989859f,
				0.959121764f,
				0.9592533f,
				0.9593846f,
				0.959515631f,
				0.959646344f,
				0.9597768f,
				0.959906936f,
				0.9600368f,
				0.960166335f,
				0.9602956f,
				0.9604246f,
				0.9605533f,
				0.960681736f,
				0.9608098f,
				0.9609377f,
				0.961065233f,
				0.9611925f,
				0.961319447f,
				0.961446166f,
				0.9615726f,
				0.9616987f,
				0.961824536f,
				0.961950064f,
				0.962075353f,
				0.962200344f,
				0.962325037f,
				0.962449431f,
				0.9625736f,
				0.9626974f,
				0.962821f,
				0.962944269f,
				0.9630673f,
				0.96318996f,
				0.963312447f,
				0.9634346f,
				0.963556468f,
				0.963678062f,
				0.963799357f,
				0.9639204f,
				0.9640412f,
				0.964161634f,
				0.964281857f,
				0.9644018f,
				0.9645214f,
				0.964640737f,
				0.9647598f,
				0.9648787f,
				0.9649972f,
				0.9651154f,
				0.9652334f,
				0.9653511f,
				0.9654685f,
				0.9655857f,
				0.965702534f,
				0.9658192f,
				0.965935469f,
				0.9660515f,
				0.966167331f,
				0.9662828f,
				0.966398f,
				0.966513f,
				0.966627657f,
				0.9667421f,
				0.966856241f,
				0.9669701f,
				0.9670837f,
				0.967197f,
				0.9673101f,
				0.967422843f,
				0.9675353f,
				0.967647552f,
				0.96775955f,
				0.967871249f,
				0.96798265f,
				0.9680938f,
				0.968204737f,
				0.968315363f,
				0.9684257f,
				0.9685358f,
				0.968645632f,
				0.9687552f,
				0.9688645f,
				0.9689735f,
				0.969082236f,
				0.9691908f,
				0.969298959f,
				0.969406962f,
				0.9695146f,
				0.9696221f,
				0.969729245f,
				0.9698361f,
				0.9699428f,
				0.970049143f,
				0.9701553f,
				0.970261157f,
				0.9703667f,
				0.970472038f,
				0.9705771f,
				0.9706819f,
				0.970786452f,
				0.97089076f,
				0.9709948f,
				0.971098542f,
				0.9712021f,
				0.9713053f,
				0.9714083f,
				0.971511f,
				0.971613467f,
				0.9717157f,
				0.9718177f,
				0.971919358f,
				0.9720208f,
				0.972121954f,
				0.9722229f,
				0.9723236f,
				0.972424f,
				0.972524166f,
				0.972624063f,
				0.9727237f,
				0.972823143f,
				0.972922266f,
				0.97302115f,
				0.9731198f,
				0.973218143f,
				0.9733163f,
				0.9734142f,
				0.9735118f,
				0.9736092f,
				0.9737063f,
				0.973803163f,
				0.9738998f,
				0.973996162f,
				0.9740923f,
				0.974188149f,
				0.9742838f,
				0.9743792f,
				0.9744743f,
				0.974569142f,
				0.9746638f,
				0.9747582f,
				0.9748523f,
				0.9749462f,
				0.97503984f,
				0.97513324f,
				0.9752264f,
				0.975319266f,
				0.975411952f,
				0.975504339f,
				0.975596547f,
				0.975688457f,
				0.9757801f,
				0.975871563f,
				0.975962758f,
				0.9760537f,
				0.976144433f,
				0.976234853f,
				0.9763251f,
				0.9764151f,
				0.9765048f,
				0.9765943f,
				0.976683557f,
				0.9767726f,
				0.976861358f,
				0.97694993f,
				0.9770382f,
				0.9771263f,
				0.9772141f,
				0.977301657f,
				0.977389038f,
				0.9774761f,
				0.977563f,
				0.9776496f,
				0.977736056f,
				0.9778222f,
				0.977908134f,
				0.977993846f,
				0.978079259f,
				0.9781645f,
				0.9782495f,
				0.978334248f,
				0.978418767f,
				0.978503048f,
				0.9785871f,
				0.9786709f,
				0.9787545f,
				0.978837848f,
				0.978921f,
				0.979003847f,
				0.9790865f,
				0.979168952f,
				0.979251146f,
				0.9793331f,
				0.9794149f,
				0.97949636f,
				0.979577661f,
				0.9796587f,
				0.979739547f,
				0.979820132f,
				0.9799005f,
				0.979980648f,
				0.9800606f,
				0.980140269f,
				0.9802197f,
				0.980298936f,
				0.980378f,
				0.980456769f,
				0.9805353f,
				0.980613649f,
				0.980691731f,
				0.980769634f,
				0.9808473f,
				0.9809248f,
				0.981002f,
				0.981079f,
				0.981155753f,
				0.9812323f,
				0.981308639f,
				0.981384754f,
				0.981460631f,
				0.9815363f,
				0.9816118f,
				0.981687f,
				0.981762f,
				0.9818368f,
				0.981911361f,
				0.981985748f,
				0.9820599f,
				0.9821338f,
				0.9822075f,
				0.98228097f,
				0.9823543f,
				0.9824273f,
				0.982500136f,
				0.9825728f,
				0.9826452f,
				0.9827174f,
				0.9827894f,
				0.982861161f,
				0.9829327f,
				0.983004034f,
				0.983075142f,
				0.9831461f,
				0.983216763f,
				0.9832873f,
				0.983357549f,
				0.9834276f,
				0.983497441f,
				0.9835671f,
				0.983636558f,
				0.983705759f,
				0.9837748f,
				0.9838436f,
				0.9839122f,
				0.9839806f,
				0.9840488f,
				0.984116733f,
				0.9841845f,
				0.9842521f,
				0.984319448f,
				0.9843866f,
				0.984453559f,
				0.984520257f,
				0.984586835f,
				0.9846531f,
				0.9847193f,
				0.9847852f,
				0.9848509f,
				0.9849164f,
				0.9849817f,
				0.9850468f,
				0.9851117f,
				0.985176444f,
				0.985240936f,
				0.98530525f,
				0.9853693f,
				0.9854332f,
				0.985496938f,
				0.9855604f,
				0.9856237f,
				0.985686839f,
				0.9857497f,
				0.9858124f,
				0.985874951f,
				0.985937238f,
				0.985999346f,
				0.9860613f,
				0.986122966f,
				0.9861845f,
				0.9862458f,
				0.986306965f,
				0.9863679f,
				0.9864286f,
				0.9864892f,
				0.9865495f,
				0.9866097f,
				0.98666966f,
				0.986729443f,
				0.986789f,
				0.986848354f,
				0.986907542f,
				0.98696655f,
				0.9870254f,
				0.987084f,
				0.987142444f,
				0.9872007f,
				0.987258732f,
				0.987316549f,
				0.987374246f,
				0.9874317f,
				0.987489f,
				0.987546146f,
				0.987603f,
				0.987659752f,
				0.9877163f,
				0.987772644f,
				0.987828851f,
				0.9878848f,
				0.9879406f,
				0.9879962f,
				0.988051653f,
				0.9881069f,
				0.9881619f,
				0.9882168f,
				0.988271534f,
				0.988326f,
				0.9883804f,
				0.9884345f,
				0.988488436f,
				0.988542259f,
				0.988595843f,
				0.988649249f,
				0.9887025f,
				0.9887555f,
				0.9888084f,
				0.9888611f,
				0.988913655f,
				0.988966f,
				0.989018142f,
				0.9890701f,
				0.9891219f,
				0.989173532f,
				0.989225f,
				0.9892763f,
				0.9893274f,
				0.9893783f,
				0.989429f,
				0.9894796f,
				0.989529967f,
				0.9895802f,
				0.9896302f,
				0.9896801f,
				0.9897298f,
				0.989779353f,
				0.9898287f,
				0.9898779f,
				0.9899269f,
				0.9899757f,
				0.9900244f,
				0.990072846f,
				0.9901212f,
				0.990169346f,
				0.9902173f,
				0.990265131f,
				0.9903128f,
				0.99036026f,
				0.9904076f,
				0.9904547f,
				0.9905017f,
				0.9905485f,
				0.9905951f,
				0.9906416f,
				0.9906879f,
				0.990734041f,
				0.99078f,
				0.9908258f,
				0.9908714f,
				0.9909169f,
				0.9909622f,
				0.9910073f,
				0.9910523f,
				0.991097152f,
				0.9911418f,
				0.9911863f,
				0.9912306f,
				0.991274834f,
				0.9913188f,
				0.991362631f,
				0.9914063f,
				0.9914499f,
				0.9914932f,
				0.991536438f,
				0.9915795f,
				0.9916224f,
				0.9916651f,
				0.9917077f,
				0.991750062f,
				0.9917923f,
				0.9918344f,
				0.991876364f,
				0.991918147f,
				0.991959751f,
				0.992001235f,
				0.992042542f,
				0.9920837f,
				0.992124736f,
				0.992165565f,
				0.9922063f,
				0.9922468f,
				0.9922872f,
				0.992327452f,
				0.9923675f,
				0.992407441f,
				0.992447257f,
				0.992486835f,
				0.992526352f,
				0.992565632f,
				0.992604852f,
				0.992643833f,
				0.9926827f,
				0.992721438f,
				0.99276f,
				0.992798448f,
				0.9928367f,
				0.992874861f,
				0.9929128f,
				0.9929507f,
				0.992988348f,
				0.9930259f,
				0.9930633f,
				0.9931005f,
				0.9931376f,
				0.993174553f,
				0.9932114f,
				0.993248045f,
				0.9932846f,
				0.993320942f,
				0.9933572f,
				0.993393242f,
				0.9934292f,
				0.993465f,
				0.99350065f,
				0.9935362f,
				0.9935716f,
				0.9936068f,
				0.9936419f,
				0.993676841f,
				0.99371165f,
				0.99374634f,
				0.993780851f,
				0.993815243f,
				0.9938495f,
				0.9938836f,
				0.9939176f,
				0.99395144f,
				0.9939852f,
				0.994018734f,
				0.9940521f,
				0.994085431f,
				0.9941186f,
				0.9941516f,
				0.9941845f,
				0.9942172f,
				0.9942498f,
				0.9942823f,
				0.9943147f,
				0.994346857f,
				0.994379f,
				0.994410932f,
				0.9944427f,
				0.9944744f,
				0.994505942f,
				0.994537354f,
				0.994568646f,
				0.9945998f,
				0.9946308f,
				0.9946617f,
				0.994692445f,
				0.9947231f,
				0.9947536f,
				0.994783938f,
				0.9948142f,
				0.9948443f,
				0.9948743f,
				0.99490416f,
				0.9949339f,
				0.9949635f,
				0.994993f,
				0.995022357f,
				0.995051563f,
				0.99508065f,
				0.9951096f,
				0.995138466f,
				0.9951672f,
				0.9951958f,
				0.9952243f,
				0.9952526f,
				0.995280862f,
				0.995308936f,
				0.99533695f,
				0.9953648f,
				0.995392561f,
				0.995420158f,
				0.995447636f,
				0.995475f,
				0.9955023f,
				0.9955294f,
				0.9955564f,
				0.9955833f,
				0.995610058f,
				0.9956367f,
				0.9956633f,
				0.9956897f,
				0.995716f,
				0.995742142f,
				0.9957682f,
				0.9957942f,
				0.99582f,
				0.9958457f,
				0.9958713f,
				0.995896757f,
				0.995922148f,
				0.995947361f,
				0.9959725f,
				0.995997548f,
				0.996022463f,
				0.9960472f,
				0.9960719f,
				0.9960965f,
				0.9961209f,
				0.996145248f,
				0.9961695f,
				0.9961936f,
				0.9962176f,
				0.9962415f,
				0.9962653f,
				0.996288955f,
				0.9963125f,
				0.9963359f,
				0.9963593f,
				0.996382535f,
				0.9964056f,
				0.9964286f,
				0.996451557f,
				0.9964743f,
				0.996497035f,
				0.9965196f,
				0.9965421f,
				0.996564448f,
				0.9965867f,
				0.996608853f,
				0.9966309f,
				0.996652842f,
				0.996674657f,
				0.9966964f,
				0.996718049f,
				0.996739566f,
				0.996760964f,
				0.9967823f,
				0.996803463f,
				0.9968246f,
				0.9968456f,
				0.9968665f,
				0.9968873f,
				0.996908f,
				0.9969286f,
				0.9969491f,
				0.996969461f,
				0.9969898f,
				0.99701f,
				0.9970301f,
				0.997050047f,
				0.997069955f,
				0.997089744f,
				0.9971095f,
				0.997129f,
				0.9971486f,
				0.997167945f,
				0.997187257f,
				0.99720645f,
				0.9972256f,
				0.9972446f,
				0.9972635f,
				0.9972823f,
				0.997301042f,
				0.9973197f,
				0.997338235f,
				0.997356653f,
				0.997375f,
				0.99739325f,
				0.99741143f,
				0.9974295f,
				0.9974475f,
				0.9974654f,
				0.997483134f,
				0.997500837f,
				0.9975184f,
				0.997535944f,
				0.9975534f,
				0.9975707f,
				0.997588f,
				0.9976051f,
				0.997622132f,
				0.9976391f,
				0.997656f,
				0.9976728f,
				0.9976895f,
				0.9977061f,
				0.9977226f,
				0.9977391f,
				0.9977554f,
				0.9977717f,
				0.997787833f,
				0.9978039f,
				0.99781996f,
				0.9978359f,
				0.99785167f,
				0.9978674f,
				0.9978831f,
				0.997898638f,
				0.997914135f,
				0.9979295f,
				0.997944832f,
				0.9979601f,
				0.99797523f,
				0.9979903f,
				0.9980053f,
				0.9980202f,
				0.998035f,
				0.998049736f,
				0.9980644f,
				0.998079f,
				0.9980935f,
				0.9981079f,
				0.9981222f,
				0.998136461f,
				0.998150647f,
				0.9981647f,
				0.9981787f,
				0.998192668f,
				0.9982065f,
				0.998220265f,
				0.998234f,
				0.9982476f,
				0.998261154f,
				0.9982746f,
				0.998288f,
				0.9983013f,
				0.998314559f,
				0.9983277f,
				0.9983408f,
				0.9983538f,
				0.9983667f,
				0.9983795f,
				0.998392344f,
				0.998405039f,
				0.9984177f,
				0.9984302f,
				0.9984427f,
				0.9984551f,
				0.998467445f,
				0.998479664f,
				0.9984919f,
				0.998504f,
				0.998516f,
				0.998528f,
				0.998539865f,
				0.9985517f,
				0.998563468f,
				0.998575151f,
				0.9985868f,
				0.998598337f,
				0.9986098f,
				0.998621166f,
				0.99863255f,
				0.9986438f,
				0.998655f,
				0.9986661f,
				0.9986772f,
				0.998688161f,
				0.998699069f,
				0.99871f,
				0.998720765f,
				0.998731434f,
				0.9987421f,
				0.9987527f,
				0.9987632f,
				0.9987737f,
				0.998784065f,
				0.9987944f,
				0.9988046f,
				0.9988149f,
				0.998824954f,
				0.998835f,
				0.998845041f,
				0.998855f,
				0.9988649f,
				0.998874664f,
				0.998884439f,
				0.9988941f,
				0.998903751f,
				0.9989133f,
				0.998922765f,
				0.998932242f,
				0.9989416f,
				0.9989509f,
				0.9989602f,
				0.9989694f,
				0.9989785f,
				0.998987556f,
				0.998996556f,
				0.999005556f,
				0.999014437f,
				0.999023259f,
				0.999032f,
				0.9990407f,
				0.9990494f,
				0.999058f,
				0.999066532f,
				0.999075055f,
				0.999083459f,
				0.9990918f,
				0.999100149f,
				0.9991084f,
				0.9991166f,
				0.9991247f,
				0.9991328f,
				0.999140859f,
				0.999148846f,
				0.9991568f,
				0.999164641f,
				0.999172449f,
				0.9991802f,
				0.9991879f,
				0.9991955f,
				0.999203146f,
				0.999210656f,
				0.999218166f,
				0.9992256f,
				0.999232948f,
				0.9992403f,
				0.9992476f,
				0.9992548f,
				0.999262f,
				0.9992691f,
				0.999276161f,
				0.9992832f,
				0.999290168f,
				0.9992971f,
				0.999303937f,
				0.9993108f,
				0.9993175f,
				0.999324262f,
				0.999330938f,
				0.999337554f,
				0.9993441f,
				0.999350667f,
				0.9993571f,
				0.999363542f,
				0.9993699f,
				0.9993763f,
				0.999382555f,
				0.9993888f,
				0.999394953f,
				0.9994011f,
				0.999407232f,
				0.999413252f,
				0.9994193f,
				0.999425232f,
				0.999431133f,
				0.999437034f,
				0.9994428f,
				0.9994486f,
				0.9994543f,
				0.999460042f,
				0.999465644f,
				0.999471247f,
				0.99947685f,
				0.999482334f,
				0.9994878f,
				0.999493241f,
				0.9994986f,
				0.99950397f,
				0.9995093f,
				0.9995145f,
				0.9995197f,
				0.9995249f,
				0.99953f,
				0.999535143f,
				0.99954015f,
				0.999545157f,
				0.999550164f,
				0.999555051f,
				0.999559939f,
				0.999564767f,
				0.9995696f,
				0.999574363f,
				0.9995791f,
				0.9995838f,
				0.99958843f,
				0.999593f,
				0.9995976f,
				0.999602139f,
				0.9996066f,
				0.9996111f,
				0.9996155f,
				0.9996199f,
				0.999624252f,
				0.999628544f,
				0.9996328f,
				0.999637f,
				0.99964124f,
				0.9996454f,
				0.9996495f,
				0.9996536f,
				0.999657631f,
				0.9996617f,
				0.9996657f,
				0.9996696f,
				0.9996735f,
				0.99967736f,
				0.999681234f,
				0.999685049f,
				0.9996888f,
				0.999692559f,
				0.999696255f,
				0.99969995f,
				0.9997036f,
				0.999707162f,
				0.999710739f,
				0.9997143f,
				0.999717832f,
				0.9997213f,
				0.999724746f,
				0.999728143f,
				0.999731541f,
				0.9997349f,
				0.9997382f,
				0.9997415f,
				0.9997447f,
				0.999747932f,
				0.999751151f,
				0.9997543f,
				0.999757469f,
				0.999760568f,
				0.9997636f,
				0.9997667f,
				0.9997697f,
				0.999772668f,
				0.999775648f,
				0.999778569f,
				0.9997815f,
				0.99978435f,
				0.9997872f,
				0.99979f,
				0.9997928f,
				0.999795556f,
				0.9997983f,
				0.999801f,
				0.999803662f,
				0.999806345f,
				0.999808967f,
				0.99981153f,
				0.9998141f,
				0.999816656f,
				0.99981916f,
				0.999821663f,
				0.9998241f,
				0.99982655f,
				0.999829f,
				0.9998314f,
				0.999833763f,
				0.9998361f,
				0.9998384f,
				0.9998407f,
				0.999842942f,
				0.9998452f,
				0.9998474f,
				0.9998496f,
				0.999851763f,
				0.9998539f,
				0.999856055f,
				0.999858141f,
				0.9998602f,
				0.9998623f,
				0.99986434f,
				0.999866366f,
				0.999868333f,
				0.9998703f,
				0.9998722f,
				0.9998742f,
				0.9998761f,
				0.9998779f,
				0.9998798f,
				0.9998816f,
				0.9998835f,
				0.999885261f,
				0.999887f,
				0.9998888f,
				0.9998905f,
				0.999892235f,
				0.9998939f,
				0.9998956f,
				0.999897242f,
				0.999898851f,
				0.99990046f,
				0.99990207f,
				0.9999036f,
				0.9999052f,
				0.9999067f,
				0.999908268f,
				0.999909759f,
				0.999911249f,
				0.9999127f,
				0.999914169f,
				0.9999156f,
				0.999917f,
				0.999918342f,
				0.9999198f,
				0.9999211f,
				0.999922454f,
				0.999923766f,
				0.9999251f,
				0.9999263f,
				0.99992764f,
				0.9999289f,
				0.999930143f,
				0.999931335f,
				0.9999325f,
				0.9999337f,
				0.9999349f,
				0.9999361f,
				0.999937236f,
				0.999938369f,
				0.999939442f,
				0.9999406f,
				0.999941647f,
				0.9999427f,
				0.9999438f,
				0.9999448f,
				0.9999458f,
				0.999946833f,
				0.999947846f,
				0.9999488f,
				0.9999498f,
				0.999950767f,
				0.999951661f,
				0.9999526f,
				0.9999535f,
				0.9999544f,
				0.9999553f,
				0.9999562f,
				0.999957f,
				0.9999579f,
				0.999958754f,
				0.9999595f,
				0.999960363f,
				0.999961138f,
				0.999962f,
				0.999962747f,
				0.999963462f,
				0.999964237f,
				0.999965f,
				0.9999657f,
				0.999966443f,
				0.999967158f,
				0.9999678f,
				0.9999685f,
				0.9999692f,
				0.99996984f,
				0.9999705f,
				0.999971151f,
				0.999971747f,
				0.9999724f,
				0.999973f,
				0.9999736f,
				0.9999742f,
				0.9999748f,
				0.9999753f,
				0.9999759f,
				0.999976456f,
				0.999977f,
				0.9999775f,
				0.999978065f,
				0.999978542f,
				0.9999791f,
				0.999979556f,
				0.999980032f,
				0.9999805f,
				0.999981f,
				0.999981463f,
				0.9999819f,
				0.999982357f,
				0.9999828f,
				0.9999832f,
				0.9999836f,
				0.999984f,
				0.999984443f,
				0.99998486f,
				0.9999852f,
				0.9999856f,
				0.999986f,
				0.999986351f,
				0.9999867f,
				0.999987066f,
				0.9999874f,
				0.9999877f,
				0.9999881f,
				0.9999884f,
				0.999988735f,
				0.999989033f,
				0.999989331f,
				0.9999896f,
				0.9999899f,
				0.9999902f,
				0.999990463f,
				0.999990761f,
				0.999991f,
				0.9999913f,
				0.999991536f,
				0.9999918f,
				0.999992f,
				0.999992251f,
				0.9999925f,
				0.9999927f,
				0.999992967f,
				0.9999932f,
				0.9999934f,
				0.9999936f,
				0.9999938f,
				0.999994f,
				0.9999942f,
				0.9999944f,
				0.9999946f,
				0.999994755f,
				0.999994934f,
				0.9999951f,
				0.999995232f,
				0.9999954f,
				0.9999956f,
				0.9999957f,
				0.9999959f,
				0.999996f,
				0.9999962f,
				0.9999963f,
				0.9999964f,
				0.999996543f,
				0.9999967f,
				0.999996841f,
				0.99999696f,
				0.9999971f,
				0.9999972f,
				0.999997258f,
				0.9999974f,
				0.9999975f,
				0.9999976f,
				0.9999977f,
				0.9999978f,
				0.999997854f,
				0.999998f,
				0.999998033f,
				0.999998152f,
				0.9999982f,
				0.9999983f,
				0.9999984f,
				0.99999845f,
				0.9999985f,
				0.999998569f,
				0.9999986f,
				0.9999987f,
				0.999998748f,
				0.9999988f,
				0.999998868f,
				0.9999989f,
				0.999999f,
				0.999999046f,
				0.9999991f,
				0.9999991f,
				0.999999166f,
				0.9999992f,
				0.9999993f,
				0.9999993f,
				0.999999344f,
				0.9999994f,
				0.9999994f,
				0.999999464f,
				0.999999464f,
				0.9999995f,
				0.9999995f,
				0.9999996f,
				0.9999996f,
				0.999999642f,
				0.999999642f,
				0.999999642f,
				0.9999997f,
				0.9999997f,
				0.9999997f,
				0.999999762f,
				0.999999762f,
				0.999999762f,
				0.9999998f,
				0.9999998f,
				0.9999998f,
				0.9999998f,
				0.9999999f,
				0.9999999f,
				0.9999999f,
				0.9999999f,
				0.9999999f,
				0.9999999f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				0.99999994f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f,
				1f
			};
			Xiph.vwin = new float[][]
			{
				Xiph.vwin64,
				Xiph.vwin128,
				Xiph.vwin256,
				Xiph.vwin512,
				Xiph.vwin1024,
				Xiph.vwin2048,
				Xiph.vwin4096,
				Xiph.vwin8192
			};
		}
	}
}
