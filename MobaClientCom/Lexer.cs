using System;
using System.IO;
using System.Text;

namespace MobaClientCom
{
	internal class Lexer
	{
		private delegate bool StateHandler(FsmContext ctx);

		private static int[] fsm_return_table;

		private static Lexer.StateHandler[] fsm_handler_table;

		private bool allow_comments;

		private bool allow_single_quoted_strings;

		private bool end_of_input;

		private FsmContext fsm_context;

		private int input_buffer;

		private int input_char;

		private TextReader reader;

		private int state;

		private StringBuilder string_buffer;

		private string string_value;

		private int token;

		private int unichar;

		public bool AllowComments
		{
			get
			{
				return this.allow_comments;
			}
			set
			{
				this.allow_comments = value;
			}
		}

		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.allow_single_quoted_strings;
			}
			set
			{
				this.allow_single_quoted_strings = value;
			}
		}

		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		public int Token
		{
			get
			{
				return this.token;
			}
		}

		public string StringValue
		{
			get
			{
				return this.string_value;
			}
		}

		static Lexer()
		{
			Lexer.PopulateFsmTables();
		}

		public Lexer(TextReader reader)
		{
			this.allow_comments = true;
			this.allow_single_quoted_strings = true;
			this.input_buffer = 0;
			this.string_buffer = new StringBuilder(128);
			this.state = 1;
			this.end_of_input = false;
			this.reader = reader;
			this.fsm_context = new FsmContext();
			this.fsm_context.L = this;
		}

		private static int HexValue(int digit)
		{
			int result;
			switch (digit)
			{
			case 65:
				break;
			case 66:
				goto IL_4C;
			case 67:
				goto IL_51;
			case 68:
				goto IL_56;
			case 69:
				goto IL_5B;
			case 70:
				goto IL_60;
			default:
				switch (digit)
				{
				case 97:
					break;
				case 98:
					goto IL_4C;
				case 99:
					goto IL_51;
				case 100:
					goto IL_56;
				case 101:
					goto IL_5B;
				case 102:
					goto IL_60;
				default:
					result = digit - 48;
					return result;
				}
				break;
			}
			result = 10;
			return result;
			IL_4C:
			result = 11;
			return result;
			IL_51:
			result = 12;
			return result;
			IL_56:
			result = 13;
			return result;
			IL_5B:
			result = 14;
			return result;
			IL_60:
			result = 15;
			return result;
		}

		private static void PopulateFsmTables()
		{
			Lexer.fsm_handler_table = new Lexer.StateHandler[]
			{
				new Lexer.StateHandler(Lexer.State1),
				new Lexer.StateHandler(Lexer.State2),
				new Lexer.StateHandler(Lexer.State3),
				new Lexer.StateHandler(Lexer.State4),
				new Lexer.StateHandler(Lexer.State5),
				new Lexer.StateHandler(Lexer.State6),
				new Lexer.StateHandler(Lexer.State7),
				new Lexer.StateHandler(Lexer.State8),
				new Lexer.StateHandler(Lexer.State9),
				new Lexer.StateHandler(Lexer.State10),
				new Lexer.StateHandler(Lexer.State11),
				new Lexer.StateHandler(Lexer.State12),
				new Lexer.StateHandler(Lexer.State13),
				new Lexer.StateHandler(Lexer.State14),
				new Lexer.StateHandler(Lexer.State15),
				new Lexer.StateHandler(Lexer.State16),
				new Lexer.StateHandler(Lexer.State17),
				new Lexer.StateHandler(Lexer.State18),
				new Lexer.StateHandler(Lexer.State19),
				new Lexer.StateHandler(Lexer.State20),
				new Lexer.StateHandler(Lexer.State21),
				new Lexer.StateHandler(Lexer.State22),
				new Lexer.StateHandler(Lexer.State23),
				new Lexer.StateHandler(Lexer.State24),
				new Lexer.StateHandler(Lexer.State25),
				new Lexer.StateHandler(Lexer.State26),
				new Lexer.StateHandler(Lexer.State27),
				new Lexer.StateHandler(Lexer.State28)
			};
			Lexer.fsm_return_table = new int[]
			{
				65542,
				0,
				65537,
				65537,
				0,
				65537,
				0,
				65537,
				0,
				0,
				65538,
				0,
				0,
				0,
				65539,
				0,
				0,
				65540,
				65541,
				65542,
				0,
				0,
				65541,
				65542,
				0,
				0,
				0,
				0
			};
		}

		private static char ProcessEscChar(int esc_char)
		{
			char result;
			if (esc_char <= 92)
			{
				if (esc_char <= 39)
				{
					if (esc_char != 34 && esc_char != 39)
					{
						goto IL_73;
					}
				}
				else if (esc_char != 47 && esc_char != 92)
				{
					goto IL_73;
				}
				result = Convert.ToChar(esc_char);
				return result;
			}
			if (esc_char <= 102)
			{
				if (esc_char == 98)
				{
					result = '\b';
					return result;
				}
				if (esc_char == 102)
				{
					result = '\f';
					return result;
				}
			}
			else
			{
				if (esc_char == 110)
				{
					result = '\n';
					return result;
				}
				switch (esc_char)
				{
				case 114:
					result = '\r';
					return result;
				case 116:
					result = '\t';
					return result;
				}
			}
			IL_73:
			result = '?';
			return result;
		}

		private static bool State1(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 32 && (ctx.L.input_char < 9 || ctx.L.input_char > 13))
				{
					if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 3;
						result = true;
					}
					else
					{
						int num = ctx.L.input_char;
						if (num <= 58)
						{
							if (num <= 39)
							{
								if (num == 34)
								{
									ctx.NextState = 19;
									ctx.Return = true;
									result = true;
									return result;
								}
								if (num != 39)
								{
									goto IL_234;
								}
								if (!ctx.L.allow_single_quoted_strings)
								{
									result = false;
									return result;
								}
								ctx.L.input_char = 34;
								ctx.NextState = 23;
								ctx.Return = true;
								result = true;
								return result;
							}
							else
							{
								switch (num)
								{
								case 44:
									break;
								case 45:
									ctx.L.string_buffer.Append((char)ctx.L.input_char);
									ctx.NextState = 2;
									result = true;
									return result;
								case 46:
									goto IL_234;
								case 47:
									if (!ctx.L.allow_comments)
									{
										result = false;
										return result;
									}
									ctx.NextState = 25;
									result = true;
									return result;
								case 48:
									ctx.L.string_buffer.Append((char)ctx.L.input_char);
									ctx.NextState = 4;
									result = true;
									return result;
								default:
									if (num != 58)
									{
										goto IL_234;
									}
									break;
								}
							}
						}
						else if (num <= 102)
						{
							switch (num)
							{
							case 91:
							case 93:
								break;
							case 92:
								goto IL_234;
							default:
								if (num != 102)
								{
									goto IL_234;
								}
								ctx.NextState = 12;
								result = true;
								return result;
							}
						}
						else
						{
							if (num == 110)
							{
								ctx.NextState = 16;
								result = true;
								return result;
							}
							if (num == 116)
							{
								ctx.NextState = 9;
								result = true;
								return result;
							}
							switch (num)
							{
							case 123:
							case 125:
								break;
							case 124:
								goto IL_234;
							default:
								goto IL_234;
							}
						}
						ctx.NextState = 1;
						ctx.Return = true;
						result = true;
						return result;
						IL_234:
						result = false;
					}
					return result;
				}
			}
			result = true;
			return result;
		}

		private static bool State2(FsmContext ctx)
		{
			ctx.L.GetChar();
			bool result;
			if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 3;
				result = true;
			}
			else
			{
				int num = ctx.L.input_char;
				if (num != 48)
				{
					result = false;
				}
				else
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 4;
					result = true;
				}
			}
			return result;
		}

		private static bool State3(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char < 48 || ctx.L.input_char > 57)
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						result = true;
					}
					else
					{
						int num = ctx.L.input_char;
						if (num <= 69)
						{
							switch (num)
							{
							case 44:
								break;
							case 45:
								goto IL_14B;
							case 46:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 5;
								result = true;
								return result;
							default:
								if (num != 69)
								{
									goto IL_14B;
								}
								goto IL_123;
							}
						}
						else if (num != 93)
						{
							if (num == 101)
							{
								goto IL_123;
							}
							if (num != 125)
							{
								goto IL_14B;
							}
						}
						ctx.L.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						result = true;
						return result;
						IL_123:
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 7;
						result = true;
						return result;
						IL_14B:
						result = false;
					}
					return result;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			result = true;
			return result;
		}

		private static bool State4(FsmContext ctx)
		{
			ctx.L.GetChar();
			bool result;
			if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
			{
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			else
			{
				int num = ctx.L.input_char;
				if (num <= 69)
				{
					switch (num)
					{
					case 44:
						break;
					case 45:
						goto IL_108;
					case 46:
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 5;
						result = true;
						return result;
					default:
						if (num != 69)
						{
							goto IL_108;
						}
						goto IL_E0;
					}
				}
				else if (num != 93)
				{
					if (num == 101)
					{
						goto IL_E0;
					}
					if (num != 125)
					{
						goto IL_108;
					}
				}
				ctx.L.UngetChar();
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
				return result;
				IL_E0:
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 7;
				result = true;
				return result;
				IL_108:
				result = false;
			}
			return result;
		}

		private static bool State5(FsmContext ctx)
		{
			ctx.L.GetChar();
			bool result;
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 6;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private static bool State6(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char < 48 || ctx.L.input_char > 57)
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						result = true;
					}
					else
					{
						int num = ctx.L.input_char;
						if (num <= 69)
						{
							if (num != 44)
							{
								if (num != 69)
								{
									goto IL_113;
								}
								goto IL_EB;
							}
						}
						else if (num != 93)
						{
							if (num == 101)
							{
								goto IL_EB;
							}
							if (num != 125)
							{
								goto IL_113;
							}
						}
						ctx.L.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						result = true;
						return result;
						IL_EB:
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 7;
						result = true;
						return result;
						IL_113:
						result = false;
					}
					return result;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			result = true;
			return result;
		}

		private static bool State7(FsmContext ctx)
		{
			ctx.L.GetChar();
			bool result;
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				result = true;
			}
			else
			{
				switch (ctx.L.input_char)
				{
				case 43:
				case 45:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 8;
					result = true;
					return result;
				}
				result = false;
			}
			return result;
		}

		private static bool State8(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char < 48 || ctx.L.input_char > 57)
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						result = true;
					}
					else
					{
						int num = ctx.L.input_char;
						if (num != 44 && num != 93 && num != 125)
						{
							result = false;
						}
						else
						{
							ctx.L.UngetChar();
							ctx.Return = true;
							ctx.NextState = 1;
							result = true;
						}
					}
					return result;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			result = true;
			return result;
		}

		private static bool State9(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 114)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 10;
				result = true;
			}
			return result;
		}

		private static bool State10(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 117)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 11;
				result = true;
			}
			return result;
		}

		private static bool State11(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 101)
			{
				result = false;
			}
			else
			{
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			return result;
		}

		private static bool State12(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 97)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 13;
				result = true;
			}
			return result;
		}

		private static bool State13(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 108)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 14;
				result = true;
			}
			return result;
		}

		private static bool State14(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 115)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 15;
				result = true;
			}
			return result;
		}

		private static bool State15(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 101)
			{
				result = false;
			}
			else
			{
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			return result;
		}

		private static bool State16(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 117)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 17;
				result = true;
			}
			return result;
		}

		private static bool State17(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 108)
			{
				result = false;
			}
			else
			{
				ctx.NextState = 18;
				result = true;
			}
			return result;
		}

		private static bool State18(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 108)
			{
				result = false;
			}
			else
			{
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			return result;
		}

		private static bool State19(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num != 34)
				{
					if (num != 92)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						continue;
					}
					ctx.StateStack = 19;
					ctx.NextState = 21;
					result = true;
				}
				else
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 20;
					result = true;
				}
				return result;
			}
			result = true;
			return result;
		}

		private static bool State20(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 34)
			{
				result = false;
			}
			else
			{
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			return result;
		}

		private static bool State21(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num <= 92)
			{
				if (num <= 39)
				{
					if (num != 34 && num != 39)
					{
						goto IL_A9;
					}
				}
				else if (num != 47 && num != 92)
				{
					goto IL_A9;
				}
			}
			else if (num <= 102)
			{
				if (num != 98 && num != 102)
				{
					goto IL_A9;
				}
			}
			else if (num != 110)
			{
				switch (num)
				{
				case 114:
				case 116:
					break;
				case 115:
					goto IL_A9;
				case 117:
					ctx.NextState = 22;
					result = true;
					return result;
				default:
					goto IL_A9;
				}
			}
			ctx.L.string_buffer.Append(Lexer.ProcessEscChar(ctx.L.input_char));
			ctx.NextState = ctx.StateStack;
			result = true;
			return result;
			IL_A9:
			result = false;
			return result;
		}

		private static bool State22(FsmContext ctx)
		{
			int num = 0;
			int num2 = 4096;
			ctx.L.unichar = 0;
			bool result;
			while (ctx.L.GetChar())
			{
				if ((ctx.L.input_char >= 48 && ctx.L.input_char <= 57) || (ctx.L.input_char >= 65 && ctx.L.input_char <= 70) || (ctx.L.input_char >= 97 && ctx.L.input_char <= 102))
				{
					ctx.L.unichar += Lexer.HexValue(ctx.L.input_char) * num2;
					num++;
					num2 /= 16;
					if (num != 4)
					{
						continue;
					}
					ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
					ctx.NextState = ctx.StateStack;
					result = true;
				}
				else
				{
					result = false;
				}
				return result;
			}
			result = true;
			return result;
		}

		private static bool State23(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num != 39)
				{
					if (num != 92)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						continue;
					}
					ctx.StateStack = 23;
					ctx.NextState = 21;
					result = true;
				}
				else
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 24;
					result = true;
				}
				return result;
			}
			result = true;
			return result;
		}

		private static bool State24(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 39)
			{
				result = false;
			}
			else
			{
				ctx.L.input_char = 34;
				ctx.Return = true;
				ctx.NextState = 1;
				result = true;
			}
			return result;
		}

		private static bool State25(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			bool result;
			if (num != 42)
			{
				if (num != 47)
				{
					result = false;
				}
				else
				{
					ctx.NextState = 26;
					result = true;
				}
			}
			else
			{
				ctx.NextState = 27;
				result = true;
			}
			return result;
		}

		private static bool State26(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 10)
				{
					ctx.NextState = 1;
					result = true;
					return result;
				}
			}
			result = true;
			return result;
		}

		private static bool State27(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					ctx.NextState = 28;
					result = true;
					return result;
				}
			}
			result = true;
			return result;
		}

		private static bool State28(FsmContext ctx)
		{
			bool result;
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 42)
				{
					if (ctx.L.input_char == 47)
					{
						ctx.NextState = 1;
						result = true;
					}
					else
					{
						ctx.NextState = 27;
						result = true;
					}
					return result;
				}
			}
			result = true;
			return result;
		}

		private bool GetChar()
		{
			bool result;
			if ((this.input_char = this.NextChar()) != -1)
			{
				result = true;
			}
			else
			{
				this.end_of_input = true;
				result = false;
			}
			return result;
		}

		private int NextChar()
		{
			int result;
			if (this.input_buffer != 0)
			{
				int num = this.input_buffer;
				this.input_buffer = 0;
				result = num;
			}
			else
			{
				result = this.reader.Read();
			}
			return result;
		}

		public bool NextToken()
		{
			this.fsm_context.Return = false;
			while (true)
			{
				Lexer.StateHandler stateHandler = Lexer.fsm_handler_table[this.state - 1];
				if (!stateHandler(this.fsm_context))
				{
					break;
				}
				if (this.end_of_input)
				{
					goto Block_2;
				}
				if (this.fsm_context.Return)
				{
					goto Block_3;
				}
				this.state = this.fsm_context.NextState;
			}
			throw new JsonException(this.input_char);
			Block_2:
			bool result = false;
			return result;
			Block_3:
			this.string_value = this.string_buffer.ToString();
			this.string_buffer.Remove(0, this.string_buffer.Length);
			this.token = Lexer.fsm_return_table[this.state - 1];
			if (this.token == 65542)
			{
				this.token = this.input_char;
			}
			this.state = this.fsm_context.NextState;
			result = true;
			return result;
		}

		private void UngetChar()
		{
			this.input_buffer = this.input_char;
		}
	}
}
