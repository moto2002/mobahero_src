using SevenZip.CommandLineParser;
using SevenZip.Compression.LZMA;
using System;
using System.Collections;
using System.IO;

namespace SevenZip
{
	internal class LzmaAlone
	{
		private enum Key
		{
			Help1,
			Help2,
			Mode,
			Dictionary,
			FastBytes,
			LitContext,
			LitPos,
			PosBits,
			MatchFinder,
			EOS,
			StdIn,
			StdOut,
			Train
		}

		private static void PrintHelp()
		{
			Console.WriteLine("\nUsage:  LZMA <e|d> [<switches>...] inputFile outputFile\n  e: encode file\n  d: decode file\n  b: Benchmark\n<Switches>\n  -d{N}:  set dictionary - [0, 29], default: 23 (8MB)\n  -fb{N}: set number of fast bytes - [5, 273], default: 128\n  -lc{N}: set number of literal context bits - [0, 8], default: 3\n  -lp{N}: set number of literal pos bits - [0, 4], default: 0\n  -pb{N}: set number of pos bits - [0, 4], default: 2\n  -mf{MF_ID}: set Match Finder: [bt2, bt4], default: bt4\n  -eos:   write End Of Stream marker\n");
		}

		private static bool GetNumber(string s, out int v)
		{
			v = 0;
			for (int i = 0; i < s.Length; i++)
			{
				char c = s[i];
				if (c < '0' || c > '9')
				{
					return false;
				}
				v *= 10;
				v += (int)(c - '0');
			}
			return true;
		}

		private static int IncorrectCommand()
		{
			throw new Exception("Command line error");
		}

		private static int Main2(string[] args)
		{
			Console.WriteLine("\nLZMA# 4.61  2008-11-23\n");
			if (args.Length == 0)
			{
				LzmaAlone.PrintHelp();
				return 0;
			}
			SwitchForm[] array = new SwitchForm[13];
			int numSwitches = 0;
			array[numSwitches++] = new SwitchForm("?", SwitchType.Simple, false);
			array[numSwitches++] = new SwitchForm("H", SwitchType.Simple, false);
			array[numSwitches++] = new SwitchForm("A", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("D", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("FB", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("LC", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("LP", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("PB", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("MF", SwitchType.UnLimitedPostString, false, 1);
			array[numSwitches++] = new SwitchForm("EOS", SwitchType.Simple, false);
			array[numSwitches++] = new SwitchForm("SI", SwitchType.Simple, false);
			array[numSwitches++] = new SwitchForm("SO", SwitchType.Simple, false);
			array[numSwitches++] = new SwitchForm("T", SwitchType.UnLimitedPostString, false, 1);
			Parser parser = new Parser(numSwitches);
			try
			{
				parser.ParseStrings(array, args);
			}
			catch
			{
				int result = LzmaAlone.IncorrectCommand();
				return result;
			}
			if (parser[0].ThereIs || parser[1].ThereIs)
			{
				LzmaAlone.PrintHelp();
				return 0;
			}
			ArrayList nonSwitchStrings = parser.NonSwitchStrings;
			int num = 0;
			if (num >= nonSwitchStrings.Count)
			{
				return LzmaAlone.IncorrectCommand();
			}
			string text = (string)nonSwitchStrings[num++];
			text = text.ToLower();
			bool flag = false;
			int num2 = 2097152;
			if (parser[3].ThereIs)
			{
				int num3;
				if (!LzmaAlone.GetNumber((string)parser[3].PostStrings[0], out num3))
				{
					LzmaAlone.IncorrectCommand();
				}
				num2 = 1 << num3;
				flag = true;
			}
			string text2 = "bt4";
			if (parser[8].ThereIs)
			{
				text2 = (string)parser[8].PostStrings[0];
			}
			text2 = text2.ToLower();
			if (text == "b")
			{
				int numIterations = 10;
				if (num < nonSwitchStrings.Count && !LzmaAlone.GetNumber((string)nonSwitchStrings[num++], out numIterations))
				{
					numIterations = 10;
				}
				return LzmaBench.LzmaBenchmark(numIterations, (uint)num2);
			}
			string text3 = string.Empty;
			if (parser[12].ThereIs)
			{
				text3 = (string)parser[12].PostStrings[0];
			}
			bool flag2 = false;
			if (text == "e")
			{
				flag2 = true;
			}
			else if (text == "d")
			{
				flag2 = false;
			}
			else
			{
				LzmaAlone.IncorrectCommand();
			}
			bool thereIs = parser[10].ThereIs;
			bool thereIs2 = parser[11].ThereIs;
			if (thereIs)
			{
				throw new Exception("Not implemeted");
			}
			if (num >= nonSwitchStrings.Count)
			{
				LzmaAlone.IncorrectCommand();
			}
			string path = (string)nonSwitchStrings[num++];
			Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			if (thereIs2)
			{
				throw new Exception("Not implemeted");
			}
			if (num >= nonSwitchStrings.Count)
			{
				LzmaAlone.IncorrectCommand();
			}
			string path2 = (string)nonSwitchStrings[num++];
			FileStream fileStream = new FileStream(path2, FileMode.Create, FileAccess.Write);
			FileStream fileStream2 = null;
			if (text3.Length != 0)
			{
				fileStream2 = new FileStream(text3, FileMode.Open, FileAccess.Read);
			}
			if (flag2)
			{
				if (!flag)
				{
					num2 = 8388608;
				}
				int num4 = 2;
				int num5 = 3;
				int num6 = 0;
				int num7 = 2;
				int num8 = 128;
				bool flag3 = parser[9].ThereIs || thereIs;
				if (parser[2].ThereIs && !LzmaAlone.GetNumber((string)parser[2].PostStrings[0], out num7))
				{
					LzmaAlone.IncorrectCommand();
				}
				if (parser[4].ThereIs && !LzmaAlone.GetNumber((string)parser[4].PostStrings[0], out num8))
				{
					LzmaAlone.IncorrectCommand();
				}
				if (parser[5].ThereIs && !LzmaAlone.GetNumber((string)parser[5].PostStrings[0], out num5))
				{
					LzmaAlone.IncorrectCommand();
				}
				if (parser[6].ThereIs && !LzmaAlone.GetNumber((string)parser[6].PostStrings[0], out num6))
				{
					LzmaAlone.IncorrectCommand();
				}
				if (parser[7].ThereIs && !LzmaAlone.GetNumber((string)parser[7].PostStrings[0], out num4))
				{
					LzmaAlone.IncorrectCommand();
				}
				CoderPropID[] propIDs = new CoderPropID[]
				{
					CoderPropID.DictionarySize,
					CoderPropID.PosStateBits,
					CoderPropID.LitContextBits,
					CoderPropID.LitPosBits,
					CoderPropID.Algorithm,
					CoderPropID.NumFastBytes,
					CoderPropID.MatchFinder,
					CoderPropID.EndMarker
				};
				object[] properties = new object[]
				{
					num2,
					num4,
					num5,
					num6,
					num7,
					num8,
					text2,
					flag3
				};
				Encoder encoder = new Encoder();
				encoder.SetCoderProperties(propIDs, properties);
				encoder.WriteCoderProperties(fileStream);
				long num9;
				if (flag3 || thereIs)
				{
					num9 = -1L;
				}
				else
				{
					num9 = stream.Length;
				}
				for (int i = 0; i < 8; i++)
				{
					fileStream.WriteByte((byte)(num9 >> 8 * i));
				}
				if (fileStream2 != null)
				{
					CDoubleStream cDoubleStream = new CDoubleStream();
					cDoubleStream.s1 = fileStream2;
					cDoubleStream.s2 = stream;
					cDoubleStream.fileIndex = 0;
					stream = cDoubleStream;
					long length = fileStream2.Length;
					cDoubleStream.skipSize = 0L;
					if (length > (long)num2)
					{
						cDoubleStream.skipSize = length - (long)num2;
					}
					fileStream2.Seek(cDoubleStream.skipSize, SeekOrigin.Begin);
					encoder.SetTrainSize((uint)(length - cDoubleStream.skipSize));
				}
				encoder.Code(stream, fileStream, -1L, -1L, null);
			}
			else
			{
				if (!(text == "d"))
				{
					throw new Exception("Command Error");
				}
				byte[] array2 = new byte[5];
				if (stream.Read(array2, 0, 5) != 5)
				{
					throw new Exception("input .lzma is too short");
				}
				Decoder decoder = new Decoder();
				decoder.SetDecoderProperties(array2);
				if (fileStream2 != null && !decoder.Train(fileStream2))
				{
					throw new Exception("can't train");
				}
				long num10 = 0L;
				for (int j = 0; j < 8; j++)
				{
					int num11 = stream.ReadByte();
					if (num11 < 0)
					{
						throw new Exception("Can't Read 1");
					}
					num10 |= (long)((byte)num11) << 8 * j;
				}
				long inSize = stream.Length - stream.Position;
				decoder.Code(stream, fileStream, inSize, num10, null);
			}
			return 0;
		}

		[STAThread]
		private static int Main(string[] args)
		{
			int result;
			try
			{
				result = LzmaAlone.Main2(args);
			}
			catch (Exception arg)
			{
				Console.WriteLine("{0} Caught exception #1.", arg);
				result = 1;
			}
			return result;
		}
	}
}
