using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Assets.Scripts.Model
{
	public class JSONNode
	{
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual string Value
		{
			get
			{
				return string.Empty;
			}
			set
			{
			}
		}

		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		public virtual IEnumerable<JSONNode> Childs
		{
			get
			{
				JSONNode.<>c__Iterator190 <>c__Iterator = new JSONNode.<>c__Iterator190();
				JSONNode.<>c__Iterator190 expr_07 = <>c__Iterator;
				expr_07.$PC = -2;
				return expr_07;
			}
		}

		public IEnumerable<JSONNode> DeepChilds
		{
			get
			{
				JSONNode.<>c__Iterator191 <>c__Iterator = new JSONNode.<>c__Iterator191();
				<>c__Iterator.<>f__this = this;
				JSONNode.<>c__Iterator191 expr_0E = <>c__Iterator;
				expr_0E.$PC = -2;
				return expr_0E;
			}
		}

		public virtual int AsInt
		{
			get
			{
				int result = 0;
				if (int.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual float AsFloat
		{
			get
			{
				float result = 0f;
				if (float.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0f;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, out result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(this.Value, out result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = ((!value) ? "false" : "true");
			}
		}

		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		public virtual JSONClass AsObject
		{
			get
			{
				return this as JSONClass;
			}
		}

		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		public virtual void Add(JSONNode aItem)
		{
			this.Add(string.Empty, aItem);
		}

		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		public override string ToString()
		{
			return "JSONNode";
		}

		public virtual string ToString(string aPrefix)
		{
			return "JSONNode";
		}

		public override bool Equals(object obj)
		{
			return object.ReferenceEquals(this, obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal static string Escape(string aText)
		{
			string text = string.Empty;
			for (int i = 0; i < aText.Length; i++)
			{
				char c = aText[i];
				char c2 = c;
				switch (c2)
				{
				case '\b':
					text += "\\b";
					goto IL_DB;
				case '\t':
					text += "\\t";
					goto IL_DB;
				case '\n':
					text += "\\n";
					goto IL_DB;
				case '\v':
					IL_3B:
					if (c2 == '"')
					{
						text += "\\\"";
						goto IL_DB;
					}
					if (c2 != '\\')
					{
						text += c;
						goto IL_DB;
					}
					text += "\\\\";
					goto IL_DB;
				case '\f':
					text += "\\f";
					goto IL_DB;
				case '\r':
					text += "\\r";
					goto IL_DB;
				}
				goto IL_3B;
				IL_DB:;
			}
			return text;
		}

		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jSONNode = null;
			int i = 0;
			string text = string.Empty;
			string text2 = string.Empty;
			bool flag = false;
			while (i < aJSON.Length)
			{
				char c = aJSON[i];
				switch (c)
				{
				case '\t':
					goto IL_333;
				case '\n':
				case '\r':
					goto IL_467;
				case '\v':
				case '\f':
					IL_46:
					switch (c)
					{
					case ' ':
						goto IL_333;
					case '!':
						IL_5C:
						switch (c)
						{
						case '[':
							if (flag)
							{
								text += aJSON[i];
								goto IL_467;
							}
							stack.Push(new JSONArray());
							if (jSONNode != null)
							{
								text2 = text2.Trim();
								if (jSONNode is JSONArray)
								{
									jSONNode.Add(stack.Peek());
								}
								else if (text2 != string.Empty)
								{
									jSONNode.Add(text2, stack.Peek());
								}
							}
							text2 = string.Empty;
							text = string.Empty;
							jSONNode = stack.Peek();
							goto IL_467;
						case '\\':
							i++;
							if (flag)
							{
								char c2 = aJSON[i];
								char c3 = c2;
								switch (c3)
								{
								case 'n':
									text += '\n';
									goto IL_44A;
								case 'o':
								case 'p':
								case 'q':
								case 's':
									IL_394:
									if (c3 == 'b')
									{
										text += '\b';
										goto IL_44A;
									}
									if (c3 != 'f')
									{
										text += c2;
										goto IL_44A;
									}
									text += '\f';
									goto IL_44A;
								case 'r':
									text += '\r';
									goto IL_44A;
								case 't':
									text += '\t';
									goto IL_44A;
								case 'u':
								{
									string s = aJSON.Substring(i + 1, 4);
									text += (char)int.Parse(s, NumberStyles.AllowHexSpecifier);
									i += 4;
									goto IL_44A;
								}
								}
								goto IL_394;
							}
							IL_44A:
							goto IL_467;
						case ']':
							break;
						default:
							switch (c)
							{
							case '{':
								if (flag)
								{
									text += aJSON[i];
									goto IL_467;
								}
								stack.Push(new JSONClass());
								if (jSONNode != null)
								{
									text2 = text2.Trim();
									if (jSONNode is JSONArray)
									{
										jSONNode.Add(stack.Peek());
									}
									else if (text2 != string.Empty)
									{
										jSONNode.Add(text2, stack.Peek());
									}
								}
								text2 = string.Empty;
								text = string.Empty;
								jSONNode = stack.Peek();
								goto IL_467;
							case '|':
								IL_88:
								if (c != ',')
								{
									if (c != ':')
									{
										text += aJSON[i];
										goto IL_467;
									}
									if (flag)
									{
										text += aJSON[i];
										goto IL_467;
									}
									text2 = text;
									text = string.Empty;
									goto IL_467;
								}
								else
								{
									if (flag)
									{
										text += aJSON[i];
										goto IL_467;
									}
									if (text != string.Empty)
									{
										if (jSONNode is JSONArray)
										{
											jSONNode.Add(text);
										}
										else if (text2 != string.Empty)
										{
											jSONNode.Add(text2, text);
										}
									}
									text2 = string.Empty;
									text = string.Empty;
									goto IL_467;
								}
								break;
							case '}':
								goto IL_1C5;
							}
							goto IL_88;
						}
						IL_1C5:
						if (flag)
						{
							text += aJSON[i];
							goto IL_467;
						}
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (text != string.Empty)
						{
							text2 = text2.Trim();
							if (jSONNode is JSONArray)
							{
								jSONNode.Add(text);
							}
							else if (text2 != string.Empty)
							{
								jSONNode.Add(text2, text);
							}
						}
						text2 = string.Empty;
						text = string.Empty;
						if (stack.Count > 0)
						{
							jSONNode = stack.Peek();
						}
						goto IL_467;
					case '"':
						flag ^= true;
						goto IL_467;
					}
					goto IL_5C;
				}
				goto IL_46;
				IL_467:
				i++;
				continue;
				IL_333:
				if (flag)
				{
					text += aJSON[i];
				}
				goto IL_467;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jSONNode;
		}

		public virtual void Serialize(BinaryWriter aWriter)
		{
		}

		public void SaveToStream(Stream aData)
		{
			BinaryWriter aWriter = new BinaryWriter(aData);
			this.Serialize(aWriter);
		}

		public void SaveToCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public string SaveToCompressedBase64()
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public void SaveToFile(string aFileName)
		{
			Directory.CreateDirectory(new FileInfo(aFileName).Directory.FullName);
			using (FileStream fileStream = File.OpenWrite(aFileName))
			{
				this.SaveToStream(fileStream);
			}
		}

		public string SaveToBase64()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				this.SaveToStream(memoryStream);
				memoryStream.Position = 0L;
				result = Convert.ToBase64String(memoryStream.ToArray());
			}
			return result;
		}

		public static JSONNode Deserialize(BinaryReader aReader)
		{
			JSONBinaryTag jSONBinaryTag = (JSONBinaryTag)aReader.ReadByte();
			switch (jSONBinaryTag)
			{
			case JSONBinaryTag.Array:
			{
				int num = aReader.ReadInt32();
				JSONArray jSONArray = new JSONArray();
				for (int i = 0; i < num; i++)
				{
					jSONArray.Add(JSONNode.Deserialize(aReader));
				}
				return jSONArray;
			}
			case JSONBinaryTag.Class:
			{
				int num2 = aReader.ReadInt32();
				JSONClass jSONClass = new JSONClass();
				for (int j = 0; j < num2; j++)
				{
					string aKey = aReader.ReadString();
					JSONNode aItem = JSONNode.Deserialize(aReader);
					jSONClass.Add(aKey, aItem);
				}
				return jSONClass;
			}
			case JSONBinaryTag.Value:
				return new JSONData(aReader.ReadString());
			case JSONBinaryTag.IntValue:
				return new JSONData(aReader.ReadInt32());
			case JSONBinaryTag.DoubleValue:
				return new JSONData(aReader.ReadDouble());
			case JSONBinaryTag.BoolValue:
				return new JSONData(aReader.ReadBoolean());
			case JSONBinaryTag.FloatValue:
				return new JSONData(aReader.ReadSingle());
			default:
				throw new Exception("Error deserializing JSON. Unknown tag: " + jSONBinaryTag);
			}
		}

		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedStream(Stream aData)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
		}

		public static JSONNode LoadFromStream(Stream aData)
		{
			JSONNode result;
			using (BinaryReader binaryReader = new BinaryReader(aData))
			{
				result = JSONNode.Deserialize(binaryReader);
			}
			return result;
		}

		public static JSONNode LoadFromFile(string aFileName)
		{
			JSONNode result;
			using (FileStream fileStream = File.OpenRead(aFileName))
			{
				result = JSONNode.LoadFromStream(fileStream);
			}
			return result;
		}

		public static JSONNode LoadFromBase64(string aBase64)
		{
			byte[] buffer = Convert.FromBase64String(aBase64);
			return JSONNode.LoadFromStream(new MemoryStream(buffer)
			{
				Position = 0L
			});
		}

		public static implicit operator JSONNode(string s)
		{
			return new JSONData(s);
		}

		public static implicit operator string(JSONNode d)
		{
			return (!(d == null)) ? d.Value : null;
		}

		public static bool operator ==(JSONNode a, object b)
		{
			return (b == null && a is JSONLazyCreator) || object.ReferenceEquals(a, b);
		}

		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}
	}
}
