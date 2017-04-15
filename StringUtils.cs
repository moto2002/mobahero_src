using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public class StringUtils
{
	private static StringBuilder mCacheString = new StringBuilder();

	public static bool isEmpty(string param)
	{
		return param == null || param.Trim().Length <= 0;
	}

	public static bool isEquals(string param1, string param2)
	{
		return param1 != null && param2 != null && param1.Equals(param2);
	}

	public static string formatCurrency(int total)
	{
		string result = total.ToString();
		if (total >= 100000000)
		{
			int num = total / 10000;
			result = string.Format("{0}亿", num);
		}
		else if (total >= 10000)
		{
			int num2 = total / 10000;
			result = string.Format("{0}万", num2);
		}
		return result;
	}

	public static bool IsNumber(string s, int precision, int scale)
	{
		if (precision == 0 && scale == 0)
		{
			return false;
		}
		string text = "(^\\d{1," + precision + "}";
		if (scale > 0)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"\\.\\d{0,",
				scale,
				"}$)|",
				text
			});
		}
		text += "$)";
		return Regex.IsMatch(s, text);
	}

	public static string GetNoSuffixString(string str)
	{
		return str.Remove(str.LastIndexOf("."));
	}

	public static string GetRemoveStrString(List<string> str, string cha = ":")
	{
		string text = null;
		for (int i = 0; i < str.Count; i++)
		{
			for (int j = 0; j < str[i].Split(new char[]
			{
				':'
			}).Length; j++)
			{
				text += str[i].Split(new char[]
				{
					':'
				})[j];
			}
		}
		return text;
	}

	public static string[] SplitVoString(string str, string delimiter = ",")
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			str = str.TrimStart(new char[]
			{
				'['
			});
			str = str.TrimEnd(new char[]
			{
				']'
			});
			str = str.Replace(delimiter, ":");
			return str.Split(new char[]
			{
				':'
			});
		}
		return null;
	}

	public static string GetValueString(string str)
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			str = str.TrimStart(new char[]
			{
				'['
			});
			str = str.TrimEnd(new char[]
			{
				']'
			});
			return str;
		}
		return null;
	}

	public static void GetAttrRange(string str, out int attrMin, out int attrMax)
	{
		str = StringUtils.GetValueString(str);
		string[] array = StringUtils.SplitVoString(str, "],[");
		attrMin = 0;
		attrMax = 0;
		string[] array2 = array[0].Split(new char[]
		{
			','
		});
		int num = int.Parse(array2[0].TrimStart(new char[]
		{
			'['
		}));
		int num2 = int.Parse(array2[1].TrimEnd(new char[]
		{
			']'
		}));
		attrMin = num;
		attrMax = num2;
		for (int i = 1; i < array.Length; i++)
		{
			array2 = array[i].Split(new char[]
			{
				','
			});
			num = int.Parse(array2[0].TrimStart(new char[]
			{
				'['
			}));
			num2 = int.Parse(array2[1].TrimEnd(new char[]
			{
				']'
			}));
			attrMin = ((attrMin >= num) ? num : attrMin);
			attrMax = ((attrMax <= num2) ? num2 : attrMax);
		}
	}

	public static bool IsValidConfigParam(string param)
	{
		return param != null && param.Trim().Length > 0 && "0" != param;
	}

	public static string[] GetStringVo(string str)
	{
		if (StringUtils.CheckValid(str))
		{
			string[] array = StringUtils.SplitVoString(str, "],");
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].TrimStart(new char[]
				{
					'['
				});
				array[i] = array[i].TrimEnd(new char[]
				{
					']'
				});
			}
			return array;
		}
		return null;
	}

	public static string[] GetWaveString(string str)
	{
		if (StringUtils.CheckValid(str))
		{
			string[] array = StringUtils.SplitVoString(str, "},");
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].TrimStart(new char[]
				{
					'{'
				});
				array[i] = array[i].TrimEnd(new char[]
				{
					'}'
				});
			}
			return array;
		}
		return null;
	}

	public static string[] GetStringSubVo(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.TrimStart(new char[]
			{
				'['
			});
			str = str.TrimEnd(new char[]
			{
				']'
			});
			str = str.Replace(" ", string.Empty);
			return str.Split(new char[]
			{
				separator
			});
		}
		return null;
	}

	public static string[] GetStringSubVo2(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.TrimStart(new char[]
			{
				'{'
			});
			str = str.TrimEnd(new char[]
			{
				'}'
			});
			str = str.Replace(" ", string.Empty);
			return str.Split(new char[]
			{
				separator
			});
		}
		return null;
	}

	public static string[] GetMonsterList(string str)
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace("[", string.Empty);
			str = str.Replace("]", string.Empty);
			str = Regex.Replace(str, "\\{[0-9]+,", string.Empty);
			str = Regex.Replace(str, "[0-9]+\\},", string.Empty);
			str = Regex.Replace(str, "[0-9]+\\}", string.Empty);
			str = str.Substring(0, str.Length - 2);
			str = str.Replace(" ", string.Empty);
			return str.Split(new char[]
			{
				','
			});
		}
		return null;
	}

	public static int[] GetArrayStringToInt(string str)
	{
		if (!StringUtils.CheckValid(str))
		{
			return null;
		}
		string text = str.Substring(1, str.Length - 2);
		text = text.Replace("[", string.Empty);
		text = text.Replace("]", string.Empty);
		string[] array = text.Split(new char[]
		{
			','
		});
		if (array.Count<string>() > 0)
		{
			int[] array2 = new int[array.Count<string>()];
			for (int i = 0; i < array.Count<string>(); i++)
			{
				array2[i] = int.Parse(array[i]);
			}
			return array2;
		}
		return new int[0];
	}

	public static int[] GetSampleArrayStringToInt(string str)
	{
		if (!StringUtils.CheckValid(str))
		{
			return null;
		}
		string text = str.Replace("[", string.Empty);
		text = text.Replace("]", string.Empty);
		string[] array = text.Split(new char[]
		{
			','
		});
		if (array.Count<string>() > 0)
		{
			int[] array2 = new int[array.Count<string>()];
			for (int i = 0; i < array.Count<string>(); i++)
			{
				array2[i] = int.Parse(array[i]);
			}
			return array2;
		}
		return new int[0];
	}

	public static int GetCharLength(string str)
	{
		int num = 0;
		for (int i = 0; i < str.Length; i++)
		{
			if (char.ConvertToUtf32(str, i) >= Convert.ToInt32("4e00", 16) && char.ConvertToUtf32(str, i) <= Convert.ToInt32("9fff", 16))
			{
				num += 2;
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	public static int[] GetStringValueListToInt(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.TrimStart(new char[]
			{
				'{'
			});
			str = str.TrimEnd(new char[]
			{
				'}'
			});
			str = str.Replace(" ", string.Empty);
			string[] array = str.Split(new char[]
			{
				separator
			});
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = ((array[i] == null) ? 0 : int.Parse(array[i]));
			}
			return array2;
		}
		return null;
	}

	public static int[] GetStringToInt(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			string[] array = str.Split(new char[]
			{
				separator
			});
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = ((array[i] == null) ? 0 : int.Parse(array[i]));
			}
			return array2;
		}
		return null;
	}

	public static float[] GetStringToFloat(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			string[] array = str.Split(new char[]
			{
				separator
			});
			float[] array2 = new float[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = ((array[i] == null) ? 0f : float.Parse(array[i]));
			}
			return array2;
		}
		return null;
	}

	public static string[] GetStringValue(string str, char separator = ',')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			return str.Split(new char[]
			{
				separator
			});
		}
		return null;
	}

	public static string[] GetStringValues(string str, int index, char separator1 = ',', char separator2 = '|')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			string[] array = str.Split(new char[]
			{
				separator1
			});
			List<string> list = new List<string>();
			if (array != null && array[0] != string.Empty)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						separator2
					});
					if (array2 != null && index < array2.Length)
					{
						list.Add(array2[index]);
					}
				}
			}
			return list.ToArray();
		}
		return null;
	}

	public static int[] GetStringValuesToInt(string str, int index, char separator1 = ',', char separator2 = '|')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			string[] array = str.Split(new char[]
			{
				separator1
			});
			List<int> list = new List<int>();
			if (array != null && array[0] != string.Empty)
			{
				for (int i = 0; i < array.Length; i++)
				{
					int[] stringToInt = StringUtils.GetStringToInt(array[i], separator2);
					if (stringToInt != null && index < stringToInt.Length)
					{
						list.Add(stringToInt[index]);
					}
				}
			}
			return list.ToArray();
		}
		return null;
	}

	public static float[] GetStringValuesToFloat(string str, int index, char separator1 = ',', char separator2 = '|')
	{
		if (StringUtils.CheckValid(str))
		{
			str = str.Replace(" ", string.Empty);
			string[] array = str.Split(new char[]
			{
				separator1
			});
			List<float> list = new List<float>();
			if (array != null && array[0] != string.Empty)
			{
				for (int i = 0; i < array.Length; i++)
				{
					float[] stringToFloat = StringUtils.GetStringToFloat(array[i], separator2);
					if (stringToFloat != null && index < stringToFloat.Length)
					{
						list.Add(stringToFloat[index]);
					}
				}
			}
			return list.ToArray();
		}
		return null;
	}

	public static string FormatTimeInMinutes(int seconds, bool positive = true)
	{
		if (seconds < 0 && positive)
		{
			seconds = 0;
		}
		int num = seconds / 3600;
		seconds %= 3600;
		if (num == 0)
		{
			return string.Format("{0:D2}:{1:D2}", seconds / 60, seconds % 60);
		}
		return string.Format("{2}:{0:D2}:{1:D2}", seconds / 60, seconds % 60, num);
	}

	public static string FormatNumberPlus(int plus)
	{
		if (plus <= 0)
		{
			return plus.ToString();
		}
		return "+" + plus;
	}

	public static string FormatNumber(int number)
	{
		return string.Format("{0:N0}", number);
	}

	public static string DumpObject(object o)
	{
		if (o == null)
		{
			return null;
		}
		Type type = o.GetType();
		string result;
		try
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
			result = fields.Aggregate(string.Empty, (string current, FieldInfo fd) => string.Concat(new object[]
			{
				current,
				fd.Name,
				": ",
				fd.GetValue(o),
				"; "
			})) + properties.Aggregate(string.Empty, delegate(string current, PropertyInfo fd)
			{
				MethodInfo getMethod = fd.GetGetMethod();
				if (getMethod == null)
				{
					return current;
				}
				return string.Concat(new object[]
				{
					current,
					fd.Name,
					": ",
					getMethod.Invoke(o, new object[0]),
					"; "
				});
			});
		}
		catch (Exception)
		{
			result = type.ToString();
		}
		return result;
	}

	protected static bool serializeObject(Hashtable anObject, StringBuilder builder)
	{
		builder.Append("{");
		IDictionaryEnumerator enumerator = anObject.GetEnumerator();
		bool flag = true;
		while (enumerator.MoveNext())
		{
			string aString = enumerator.Key.ToString();
			object value = enumerator.Value;
			if (!flag)
			{
				builder.Append(", ");
			}
			StringUtils.serializeString(aString, builder);
			builder.Append(":");
			if (!StringUtils.serializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool serializeDictionary(Dictionary<string, string> dict, StringBuilder builder)
	{
		builder.Append("{");
		bool flag = true;
		foreach (KeyValuePair<string, string> current in dict)
		{
			if (!flag)
			{
				builder.Append(", ");
			}
			StringUtils.serializeString(current.Key, builder);
			builder.Append(":");
			StringUtils.serializeString(current.Value, builder);
			flag = false;
		}
		builder.Append("}");
		return true;
	}

	protected static bool serializeArray(ArrayList anArray, StringBuilder builder)
	{
		builder.Append("[");
		bool flag = true;
		for (int i = 0; i < anArray.Count; i++)
		{
			object value = anArray[i];
			if (!flag)
			{
				builder.Append(", ");
			}
			if (!StringUtils.serializeValue(value, builder))
			{
				return false;
			}
			flag = false;
		}
		builder.Append("]");
		return true;
	}

	protected static bool serializeValue(object value, StringBuilder builder)
	{
		if (value == null)
		{
			builder.Append("null");
		}
		else if (value is string)
		{
			StringUtils.serializeString((string)value, builder);
		}
		else if (value is char)
		{
			StringUtils.serializeString(Convert.ToString((char)value), builder);
		}
		else if (value is decimal)
		{
			StringUtils.serializeString(Convert.ToString((decimal)value), builder);
		}
		else if (value is bool)
		{
			builder.Append(value.ToString());
		}
		else if (value.GetType().IsPrimitive)
		{
			StringUtils.serializeNumber(Convert.ToDouble(value), builder);
		}
		else if (value is byte[])
		{
			builder.Append(value.ToString());
		}
		else if (value.GetType().IsArray)
		{
			StringUtils.serializeArray(new ArrayList((ICollection)value), builder);
		}
		else if (value is Hashtable)
		{
			StringUtils.serializeObject((Hashtable)value, builder);
		}
		else if (value is Dictionary<string, string>)
		{
			StringUtils.serializeDictionary((Dictionary<string, string>)value, builder);
		}
		else if (value is ArrayList)
		{
			StringUtils.serializeArray((ArrayList)value, builder);
		}
		else if (value is IEnumerable)
		{
			IEnumerable enumerable = (IEnumerable)value;
			IEnumerator enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StringUtils.serializeValue(enumerator.Current, builder);
				builder.Append(", ");
			}
		}
		else
		{
			StringUtils.serializeRawObj(value, builder);
		}
		return true;
	}

	protected static void serializeString(string aString, StringBuilder builder)
	{
		builder.Append(aString);
	}

	protected static void serializeNumber(double number, StringBuilder builder)
	{
		builder.Append(Convert.ToString(number));
	}

	private static void serializeRawObj(object o, StringBuilder sb)
	{
		Type type = o.GetType();
		sb.Append("{");
		FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
		FieldInfo[] array = fields;
		for (int i = 0; i < array.Length; i++)
		{
			FieldInfo fieldInfo = array[i];
			sb.Append(fieldInfo.Name);
			sb.Append(": ");
			StringUtils.serializeValue(fieldInfo.GetValue(o), sb);
			sb.Append(", ");
		}
		sb.Append("}");
	}

	public static string GetTailNoEnterStr(string str)
	{
		if (str == null || str.Split(new char[]
		{
			'\n'
		}).Length <= 1)
		{
			return str;
		}
		List<string> list = str.Split(new char[]
		{
			'\n'
		}).ToList<string>();
		string text = null;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == string.Empty || list[i] == null || list[i] == " ")
			{
				list.Remove(list[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			text = text + list[j] + ((j != list.Count - 1) ? "\n" : string.Empty);
		}
		return text;
	}

	public static string ReturnMinute(int number)
	{
		return string.Format("{0}{1}{2}", (number % 300 / 60).ToString(), ":", (number % 300 % 60 >= 10) ? (number % 300 % 60).ToString() : ("0" + (number % 300 % 60).ToString()));
	}

	public static bool CheckValid(string str)
	{
		return str != null && !str.Equals("[]") && !str.Equals(string.Empty) && !str.Equals("Null") && !str.Equals("null") && !str.Equals("0");
	}

	public static bool CheckValid(string[] str)
	{
		return str != null && str.Length != 0;
	}

	public static string Join(string sep, params object[] args)
	{
		if (args == null)
		{
			return null;
		}
		return args.Aggregate(string.Empty, (string s, object o) => s + sep + o);
	}

	public static StringBuilder GetStringBuilder()
	{
		StringUtils.mCacheString.Length = 0;
		return StringUtils.mCacheString;
	}

	public static string FromUtf8(string utf8Str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(utf8Str);
		byte[] bytes2 = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, bytes);
		return Encoding.Unicode.GetString(bytes2);
	}

	public static string DumpStr(string str)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < str.Length; i++)
		{
			stringBuilder.Append((int)str[i]);
			stringBuilder.Append(",");
		}
		return stringBuilder.ToString();
	}

	public static string GetLuaStr(string str)
	{
		byte[] array = new byte[str.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (byte)str[i];
		}
		return Encoding.UTF8.GetString(array);
	}
}
