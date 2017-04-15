using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace Pathfinding.Serialization.JsonFx
{
	public class JsonReader
	{
		internal const char OperatorNegate = '-';

		internal const char OperatorUnaryPlus = '+';

		internal const char OperatorArrayStart = '[';

		internal const char OperatorArrayEnd = ']';

		internal const char OperatorObjectStart = '{';

		internal const char OperatorObjectEnd = '}';

		internal const char OperatorStringDelim = '"';

		internal const char OperatorStringDelimAlt = '\'';

		internal const char OperatorValueDelim = ',';

		internal const char OperatorNameDelim = ':';

		internal const char OperatorCharEscape = '\\';

		private const string CommentStart = "/*";

		private const string CommentEnd = "*/";

		private const string CommentLine = "//";

		private const string LineEndings = "\r\n";

		private const string ErrorUnrecognizedToken = "Illegal JSON sequence.";

		private const string ErrorUnterminatedComment = "Unterminated comment block.";

		private const string ErrorUnterminatedObject = "Unterminated JSON object.";

		private const string ErrorUnterminatedArray = "Unterminated JSON array.";

		private const string ErrorUnterminatedString = "Unterminated JSON string.";

		private const string ErrorIllegalNumber = "Illegal JSON number.";

		private const string ErrorExpectedString = "Expected JSON string.";

		private const string ErrorExpectedObject = "Expected JSON object.";

		private const string ErrorExpectedArray = "Expected JSON array.";

		private const string ErrorExpectedPropertyName = "Expected JSON object property name.";

		private const string ErrorExpectedPropertyNameDelim = "Expected JSON object property name delimiter.";

		private const string ErrorGenericIDictionary = "Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})";

		private const string ErrorGenericIDictionaryKeys = "Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})";

		internal static readonly string LiteralFalse = "false";

		internal static readonly string LiteralTrue = "true";

		internal static readonly string LiteralNull = "null";

		internal static readonly string LiteralUndefined = "undefined";

		internal static readonly string LiteralNotANumber = "NaN";

		internal static readonly string LiteralPositiveInfinity = "Infinity";

		internal static readonly string LiteralNegativeInfinity = "-Infinity";

		internal static readonly string TypeGenericIDictionary = "System.Collections.Generic.IDictionary`2";

		private readonly JsonReaderSettings Settings;

		private readonly string Source;

		private readonly int SourceLength;

		private int index;

		private readonly List<object> previouslyDeserialized;

		private readonly Stack<List<object>> jsArrays;

		public bool EOF
		{
			get
			{
				return this.index >= this.SourceLength - 1;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
		public bool AllowNullValueTypes
		{
			get
			{
				return this.Settings.AllowNullValueTypes;
			}
			set
			{
				this.Settings.AllowNullValueTypes = value;
			}
		}

		[Obsolete("This has been deprecated in favor of JsonReaderSettings object")]
		public string TypeHintName
		{
			get
			{
				return this.Settings.TypeHintName;
			}
			set
			{
				this.Settings.TypeHintName = value;
			}
		}

		public JsonReader(TextReader input) : this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(TextReader input, JsonReaderSettings settings)
		{
			this.Settings = new JsonReaderSettings();
			this.previouslyDeserialized = new List<object>();
			this.jsArrays = new Stack<List<object>>();
			base..ctor();
			this.Settings = settings;
			this.Source = input.ReadToEnd();
			this.SourceLength = this.Source.Length;
		}

		public JsonReader(Stream input) : this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(Stream input, JsonReaderSettings settings)
		{
			this.Settings = new JsonReaderSettings();
			this.previouslyDeserialized = new List<object>();
			this.jsArrays = new Stack<List<object>>();
			base..ctor();
			this.Settings = settings;
			using (StreamReader streamReader = new StreamReader(input, true))
			{
				this.Source = streamReader.ReadToEnd();
			}
			this.SourceLength = this.Source.Length;
		}

		public JsonReader(string input) : this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(string input, JsonReaderSettings settings)
		{
			this.Settings = new JsonReaderSettings();
			this.previouslyDeserialized = new List<object>();
			this.jsArrays = new Stack<List<object>>();
			base..ctor();
			this.Settings = settings;
			this.Source = input;
			this.SourceLength = this.Source.Length;
		}

		public JsonReader(StringBuilder input) : this(input, new JsonReaderSettings())
		{
		}

		public JsonReader(StringBuilder input, JsonReaderSettings settings)
		{
			this.Settings = new JsonReaderSettings();
			this.previouslyDeserialized = new List<object>();
			this.jsArrays = new Stack<List<object>>();
			base..ctor();
			this.Settings = settings;
			this.Source = input.ToString();
			this.SourceLength = this.Source.Length;
		}

		public object Deserialize()
		{
			return this.Deserialize(null);
		}

		public object Deserialize(int start)
		{
			this.index = start;
			return this.Deserialize(null);
		}

		public object Deserialize(Type type)
		{
			return this.Read(type, false);
		}

		public object Deserialize(int start, Type type)
		{
			this.index = start;
			return this.Read(type, false);
		}

		private object Read(Type expectedType, bool typeIsHint)
		{
			if (expectedType == typeof(object))
			{
				expectedType = null;
			}
			JsonToken jsonToken = this.Tokenize();
			if (expectedType != null && !expectedType.IsPrimitive)
			{
				JsonConverter converter = this.Settings.GetConverter(expectedType);
				if (converter != null)
				{
					try
					{
						object obj = this.Read(typeof(Dictionary<string, object>), false);
						Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
						object result;
						if (dictionary == null)
						{
							result = null;
							return result;
						}
						object obj2 = converter.Read(this, expectedType, dictionary);
						result = obj2;
						return result;
					}
					catch (JsonTypeCoercionException arg)
					{
						Console.WriteLine("Could not cast to dictionary for converter processing. Ignoring field.\n" + arg);
					}
					return null;
				}
			}
			switch (jsonToken)
			{
			case JsonToken.Undefined:
				this.index += JsonReader.LiteralUndefined.Length;
				return null;
			case JsonToken.Null:
				this.index += JsonReader.LiteralNull.Length;
				return null;
			case JsonToken.False:
				this.index += JsonReader.LiteralFalse.Length;
				return false;
			case JsonToken.True:
				this.index += JsonReader.LiteralTrue.Length;
				return true;
			case JsonToken.NaN:
				this.index += JsonReader.LiteralNotANumber.Length;
				return double.NaN;
			case JsonToken.PositiveInfinity:
				this.index += JsonReader.LiteralPositiveInfinity.Length;
				return double.PositiveInfinity;
			case JsonToken.NegativeInfinity:
				this.index += JsonReader.LiteralNegativeInfinity.Length;
				return double.NegativeInfinity;
			case JsonToken.Number:
				return this.ReadNumber((!typeIsHint) ? expectedType : null);
			case JsonToken.String:
				return this.ReadString((!typeIsHint) ? expectedType : null);
			case JsonToken.ArrayStart:
				return this.ReadArray((!typeIsHint) ? expectedType : null);
			case JsonToken.ObjectStart:
				return this.ReadObject((!typeIsHint) ? expectedType : null);
			}
			return null;
		}

		public void PopulateObject<T>(ref T obj) where T : class
		{
			object obj2 = obj;
			this.PopulateObject(ref obj2);
			obj = (obj2 as T);
		}

		public void PopulateObject(ref object obj)
		{
			Type type = obj.GetType();
			Dictionary<string, MemberInfo> memberMap = this.Settings.Coercion.GetMemberMap(type);
			Type genericDictionaryType = null;
			if (memberMap == null)
			{
				genericDictionaryType = this.GetGenericDictionaryType(type);
			}
			this.PopulateObject(ref obj, type, memberMap, genericDictionaryType);
		}

		private object ReadObject(Type objectType)
		{
			Type genericDictionaryType = null;
			Dictionary<string, MemberInfo> dictionary = null;
			object obj;
			if (objectType != null)
			{
				obj = this.Settings.Coercion.InstantiateObject(objectType, out dictionary);
				this.previouslyDeserialized.Add(obj);
				if (dictionary == null)
				{
					genericDictionaryType = this.GetGenericDictionaryType(objectType);
				}
			}
			else
			{
				obj = new Dictionary<string, object>();
			}
			object obj2 = obj;
			this.PopulateObject(ref obj, objectType, dictionary, genericDictionaryType);
			if (obj2 != obj)
			{
				this.previouslyDeserialized.RemoveAt(this.previouslyDeserialized.Count - 1);
			}
			return obj;
		}

		private Type GetGenericDictionaryType(Type objectType)
		{
			Type @interface = objectType.GetInterface(JsonReader.TypeGenericIDictionary);
			if (@interface != null)
			{
				Type[] genericArguments = @interface.GetGenericArguments();
				if (genericArguments.Length == 2)
				{
					if (genericArguments[0] != typeof(string))
					{
						throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> need to have string keys to be deserialized. ({0})", objectType), this.index);
					}
					if (genericArguments[1] != typeof(object))
					{
						return genericArguments[1];
					}
				}
			}
			return null;
		}

		private void PopulateObject(ref object result, Type objectType, Dictionary<string, MemberInfo> memberMap, Type genericDictionaryType)
		{
			if (this.Source[this.index] != '{')
			{
				throw new JsonDeserializationException("Expected JSON object.", this.index);
			}
			IDictionary dictionary = result as IDictionary;
			if (dictionary == null && objectType.GetInterface(JsonReader.TypeGenericIDictionary) != null)
			{
				throw new JsonDeserializationException(string.Format("Types which implement Generic IDictionary<TKey, TValue> also need to implement IDictionary to be deserialized. ({0})", objectType), this.index);
			}
			JsonToken jsonToken;
			while (true)
			{
				this.index++;
				if (this.index >= this.SourceLength)
				{
					break;
				}
				jsonToken = this.Tokenize(this.Settings.AllowUnquotedObjectKeys);
				if (jsonToken == JsonToken.ObjectEnd)
				{
					goto Block_5;
				}
				if (jsonToken != JsonToken.String && jsonToken != JsonToken.UnquotedName)
				{
					goto Block_7;
				}
				string text = (jsonToken != JsonToken.String) ? this.ReadUnquotedKey() : ((string)this.ReadString(null));
				MemberInfo memberInfo;
				Type type;
				if (genericDictionaryType == null && memberMap != null)
				{
					type = TypeCoercionUtility.GetMemberInfo(memberMap, text, out memberInfo);
				}
				else
				{
					type = genericDictionaryType;
					memberInfo = null;
				}
				jsonToken = this.Tokenize();
				if (jsonToken != JsonToken.NameDelim)
				{
					goto Block_11;
				}
				this.index++;
				if (this.index >= this.SourceLength)
				{
					goto Block_12;
				}
				if (this.Settings.HandleCyclicReferences && text == "@ref")
				{
					int num = (int)this.Read(typeof(int), false);
					result = this.previouslyDeserialized[num];
					jsonToken = this.Tokenize();
				}
				else
				{
					object obj = this.Read(type, false);
					if (dictionary != null)
					{
						if (objectType == null && this.Settings.IsTypeHintName(text))
						{
							result = this.Settings.Coercion.ProcessTypeHint(dictionary, obj as string, out objectType, out memberMap);
						}
						else
						{
							dictionary[text] = obj;
						}
					}
					else
					{
						this.Settings.Coercion.SetMemberValue(result, type, memberInfo, obj);
					}
					jsonToken = this.Tokenize();
				}
				if (jsonToken != JsonToken.ValueDelim)
				{
					goto IL_234;
				}
			}
			throw new JsonDeserializationException("Unterminated JSON object.", this.index);
			Block_5:
			goto IL_234;
			Block_7:
			throw new JsonDeserializationException("Expected JSON object property name.", this.index);
			Block_11:
			throw new JsonDeserializationException("Expected JSON object property name delimiter.", this.index);
			Block_12:
			throw new JsonDeserializationException("Unterminated JSON object.", this.index);
			IL_234:
			if (jsonToken != JsonToken.ObjectEnd)
			{
				throw new JsonDeserializationException("Unterminated JSON object.", this.index);
			}
			this.index++;
		}

		private IEnumerable ReadArray(Type arrayType)
		{
			if (this.Source[this.index] != '[')
			{
				throw new JsonDeserializationException("Expected JSON array.", this.index);
			}
			bool flag = arrayType != null;
			bool typeIsHint = !flag;
			Type type = null;
			if (flag)
			{
				if (arrayType.HasElementType)
				{
					type = arrayType.GetElementType();
				}
				else if (arrayType.IsGenericType)
				{
					Type[] genericArguments = arrayType.GetGenericArguments();
					if (genericArguments.Length == 1)
					{
						type = genericArguments[0];
					}
				}
			}
			List<object> list = (this.jsArrays.Count <= 0) ? new List<object>() : this.jsArrays.Pop();
			list.Clear();
			JsonToken jsonToken;
			while (true)
			{
				this.index++;
				if (this.index >= this.SourceLength)
				{
					break;
				}
				jsonToken = this.Tokenize();
				if (jsonToken == JsonToken.ArrayEnd)
				{
					goto Block_8;
				}
				object obj = this.Read(type, typeIsHint);
				list.Add(obj);
				if (obj == null)
				{
					if (type != null && type.IsValueType)
					{
						type = null;
					}
					flag = true;
				}
				else if (type != null && !type.IsAssignableFrom(obj.GetType()))
				{
					if (obj.GetType().IsAssignableFrom(type))
					{
						type = obj.GetType();
					}
					else
					{
						type = null;
						flag = true;
					}
				}
				else if (!flag)
				{
					type = obj.GetType();
					flag = true;
				}
				jsonToken = this.Tokenize();
				if (jsonToken != JsonToken.ValueDelim)
				{
					goto IL_17E;
				}
			}
			throw new JsonDeserializationException("Unterminated JSON array.", this.index);
			Block_8:
			IL_17E:
			if (jsonToken != JsonToken.ArrayEnd)
			{
				throw new JsonDeserializationException("Unterminated JSON array.", this.index);
			}
			this.index++;
			this.jsArrays.Push(list);
			if (type != null && type != typeof(object))
			{
				Array array = Array.CreateInstance(type, list.Count);
				for (int i = 0; i < list.Count; i++)
				{
					array.SetValue(list[i], i);
				}
				return array;
			}
			return list.ToArray();
		}

		private string ReadUnquotedKey()
		{
			int num = this.index;
			do
			{
				this.index++;
			}
			while (this.Tokenize(true) == JsonToken.UnquotedName);
			return this.Source.Substring(num, this.index - num);
		}

		private object ReadString(Type expectedType)
		{
			if (this.Source[this.index] != '"' && this.Source[this.index] != '\'')
			{
				throw new JsonDeserializationException("Expected JSON string.", this.index);
			}
			char c = this.Source[this.index];
			this.index++;
			if (this.index >= this.SourceLength)
			{
				throw new JsonDeserializationException("Unterminated JSON string.", this.index);
			}
			int num = this.index;
			StringBuilder stringBuilder = new StringBuilder();
			while (this.Source[this.index] != c)
			{
				if (this.Source[this.index] == '\\')
				{
					stringBuilder.Append(this.Source, num, this.index - num);
					this.index++;
					if (this.index >= this.SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", this.index);
					}
					char c2 = this.Source[this.index];
					switch (c2)
					{
					case 'n':
						stringBuilder.Append('\n');
						goto IL_22D;
					case 'o':
					case 'p':
					case 'q':
					case 's':
						IL_12E:
						if (c2 == '0')
						{
							goto IL_22D;
						}
						if (c2 == 'b')
						{
							stringBuilder.Append('\b');
							goto IL_22D;
						}
						if (c2 != 'f')
						{
							stringBuilder.Append(this.Source[this.index]);
							goto IL_22D;
						}
						stringBuilder.Append('\f');
						goto IL_22D;
					case 'r':
						stringBuilder.Append('\r');
						goto IL_22D;
					case 't':
						stringBuilder.Append('\t');
						goto IL_22D;
					case 'u':
					{
						int utf;
						if (this.index + 4 < this.SourceLength && int.TryParse(this.Source.Substring(this.index + 1, 4), NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo, out utf))
						{
							stringBuilder.Append(char.ConvertFromUtf32(utf));
							this.index += 4;
						}
						else
						{
							stringBuilder.Append(this.Source[this.index]);
						}
						goto IL_22D;
					}
					}
					goto IL_12E;
					IL_22D:
					this.index++;
					if (this.index >= this.SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", this.index);
					}
					num = this.index;
				}
				else
				{
					this.index++;
					if (this.index >= this.SourceLength)
					{
						throw new JsonDeserializationException("Unterminated JSON string.", this.index);
					}
				}
			}
			stringBuilder.Append(this.Source, num, this.index - num);
			this.index++;
			if (expectedType != null && expectedType != typeof(string))
			{
				return this.Settings.Coercion.CoerceType(expectedType, stringBuilder.ToString());
			}
			return stringBuilder.ToString();
		}

		private object ReadNumber(Type expectedType)
		{
			bool flag = false;
			bool flag2 = false;
			int num = this.index;
			int num2 = 0;
			if (this.Source[this.index] == '-')
			{
				this.index++;
				if (this.index >= this.SourceLength || !char.IsDigit(this.Source[this.index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", this.index);
				}
			}
			while (this.index < this.SourceLength && char.IsDigit(this.Source[this.index]))
			{
				this.index++;
			}
			if (this.index < this.SourceLength && this.Source[this.index] == '.')
			{
				flag = true;
				this.index++;
				if (this.index >= this.SourceLength || !char.IsDigit(this.Source[this.index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", this.index);
				}
				while (this.index < this.SourceLength && char.IsDigit(this.Source[this.index]))
				{
					this.index++;
				}
			}
			int num3 = this.index - num - ((!flag) ? 0 : 1);
			if (this.index < this.SourceLength && (this.Source[this.index] == 'e' || this.Source[this.index] == 'E'))
			{
				flag2 = true;
				this.index++;
				if (this.index >= this.SourceLength)
				{
					throw new JsonDeserializationException("Illegal JSON number.", this.index);
				}
				int num4 = this.index;
				if (this.Source[this.index] == '-' || this.Source[this.index] == '+')
				{
					this.index++;
					if (this.index >= this.SourceLength || !char.IsDigit(this.Source[this.index]))
					{
						throw new JsonDeserializationException("Illegal JSON number.", this.index);
					}
				}
				else if (!char.IsDigit(this.Source[this.index]))
				{
					throw new JsonDeserializationException("Illegal JSON number.", this.index);
				}
				while (this.index < this.SourceLength && char.IsDigit(this.Source[this.index]))
				{
					this.index++;
				}
				int.TryParse(this.Source.Substring(num4, this.index - num4), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out num2);
			}
			string s = this.Source.Substring(num, this.index - num);
			if (!flag && !flag2 && num3 < 19)
			{
				decimal num5 = decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
				if (expectedType != null)
				{
					return this.Settings.Coercion.CoerceType(expectedType, num5);
				}
				if (num5 >= -2147483648m && num5 <= 2147483647m)
				{
					return (int)num5;
				}
				if (num5 >= new decimal(-9223372036854775808L) && num5 <= new decimal(9223372036854775807L))
				{
					return (long)num5;
				}
				return num5;
			}
			else
			{
				if (expectedType == typeof(decimal))
				{
					return decimal.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
				}
				double num6 = double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
				if (expectedType != null)
				{
					return this.Settings.Coercion.CoerceType(expectedType, num6);
				}
				return num6;
			}
		}

		public static object Deserialize(string value)
		{
			return JsonReader.Deserialize(value, 0, null);
		}

		public static T Deserialize<T>(string value)
		{
			return (T)((object)JsonReader.Deserialize(value, 0, typeof(T)));
		}

		public static object Deserialize(string value, int start)
		{
			return JsonReader.Deserialize(value, start, null);
		}

		public static T Deserialize<T>(string value, int start)
		{
			return (T)((object)JsonReader.Deserialize(value, start, typeof(T)));
		}

		public static object Deserialize(string value, Type type)
		{
			return JsonReader.Deserialize(value, 0, type);
		}

		public static object Deserialize(string value, int start, Type type)
		{
			return new JsonReader(value).Deserialize(start, type);
		}

		private JsonToken Tokenize()
		{
			return this.Tokenize(false);
		}

		private JsonToken Tokenize(bool allowUnquotedString)
		{
			if (this.index >= this.SourceLength)
			{
				return JsonToken.End;
			}
			while (char.IsWhiteSpace(this.Source[this.index]))
			{
				this.index++;
				if (this.index >= this.SourceLength)
				{
					return JsonToken.End;
				}
			}
			if (this.Source[this.index] == "/*"[0])
			{
				if (this.index + 1 >= this.SourceLength)
				{
					throw new JsonDeserializationException("Illegal JSON sequence. (end of stream while parsing possible comment)", this.index);
				}
				this.index++;
				bool flag = false;
				if (this.Source[this.index] == "/*"[1])
				{
					flag = true;
				}
				else if (this.Source[this.index] != "//"[1])
				{
					throw new JsonDeserializationException("Illegal JSON sequence.", this.index);
				}
				this.index++;
				if (flag)
				{
					int num = this.index - 2;
					if (this.index + 1 >= this.SourceLength)
					{
						throw new JsonDeserializationException("Unterminated comment block.", num);
					}
					while (this.Source[this.index] != "*/"[0] || this.Source[this.index + 1] != "*/"[1])
					{
						this.index++;
						if (this.index + 1 >= this.SourceLength)
						{
							throw new JsonDeserializationException("Unterminated comment block.", num);
						}
					}
					this.index += 2;
					if (this.index >= this.SourceLength)
					{
						return JsonToken.End;
					}
				}
				else
				{
					while ("\r\n".IndexOf(this.Source[this.index]) < 0)
					{
						this.index++;
						if (this.index >= this.SourceLength)
						{
							return JsonToken.End;
						}
					}
				}
				while (char.IsWhiteSpace(this.Source[this.index]))
				{
					this.index++;
					if (this.index >= this.SourceLength)
					{
						return JsonToken.End;
					}
				}
			}
			if (this.Source[this.index] == '+')
			{
				this.index++;
				if (this.index >= this.SourceLength)
				{
					return JsonToken.End;
				}
			}
			char c = this.Source[this.index];
			switch (c)
			{
			case '[':
				return JsonToken.ArrayStart;
			case '\\':
				IL_2C3:
				switch (c)
				{
				case '{':
					return JsonToken.ObjectStart;
				case '|':
				{
					IL_2D8:
					if (c == '"' || c == '\'')
					{
						return JsonToken.String;
					}
					if (c == ',')
					{
						return JsonToken.ValueDelim;
					}
					if (c == ':')
					{
						return JsonToken.NameDelim;
					}
					if (char.IsDigit(this.Source[this.index]) || (this.Source[this.index] == '-' && this.index + 1 < this.SourceLength && char.IsDigit(this.Source[this.index + 1])))
					{
						return JsonToken.Number;
					}
					if (this.MatchLiteral(JsonReader.LiteralFalse))
					{
						return JsonToken.False;
					}
					if (this.MatchLiteral(JsonReader.LiteralTrue))
					{
						return JsonToken.True;
					}
					if (this.MatchLiteral(JsonReader.LiteralNull))
					{
						return JsonToken.Null;
					}
					if (this.MatchLiteral(JsonReader.LiteralNotANumber))
					{
						return JsonToken.NaN;
					}
					if (this.MatchLiteral(JsonReader.LiteralPositiveInfinity))
					{
						return JsonToken.PositiveInfinity;
					}
					if (this.MatchLiteral(JsonReader.LiteralNegativeInfinity))
					{
						return JsonToken.NegativeInfinity;
					}
					if (this.MatchLiteral(JsonReader.LiteralUndefined))
					{
						return JsonToken.Undefined;
					}
					if (allowUnquotedString)
					{
						return JsonToken.UnquotedName;
					}
					string text = this.Source.Substring(Math.Max(0, this.index - 5), Math.Min(this.SourceLength - this.index - 1, 20));
					throw new JsonDeserializationException(string.Concat(new object[]
					{
						"Illegal JSON sequence. (when parsing '",
						this.Source[this.index],
						"' ",
						(int)this.Source[this.index],
						") at index ",
						this.index,
						"\nAround: '",
						text,
						"'"
					}), this.index);
				}
				case '}':
					return JsonToken.ObjectEnd;
				}
				goto IL_2D8;
			case ']':
				return JsonToken.ArrayEnd;
			}
			goto IL_2C3;
		}

		private bool MatchLiteral(string literal)
		{
			int length = literal.Length;
			if (this.index + length > this.SourceLength)
			{
				return false;
			}
			for (int i = 0; i < length; i++)
			{
				if (literal[i] != this.Source[this.index + i])
				{
					return false;
				}
			}
			return true;
		}

		public static T CoerceType<T>(object value, T typeToMatch)
		{
			return (T)((object)new TypeCoercionUtility().CoerceType(typeof(T), value));
		}

		public static T CoerceType<T>(object value)
		{
			return (T)((object)new TypeCoercionUtility().CoerceType(typeof(T), value));
		}

		public static object CoerceType(Type targetType, object value)
		{
			return new TypeCoercionUtility().CoerceType(targetType, value);
		}
	}
}
