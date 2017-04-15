using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace MobaClientCom
{
	public class JsonData : IJsonWrapper, IList, IOrderedDictionary, IDictionary, ICollection, IEnumerable, IEquatable<JsonData>
	{
		private IList<JsonData> inst_array;

		private bool inst_boolean;

		private double inst_double;

		private int inst_int;

		private long inst_long;

		private IDictionary<string, JsonData> inst_object;

		private string inst_string;

		private string json;

		private JsonType type;

		private IList<KeyValuePair<string, JsonData>> object_list;

		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> current in this.object_list)
				{
					list.Add(current.Key);
				}
				return (ICollection)list;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> current in this.object_list)
				{
					list.Add(current.Value);
				}
				return (ICollection)list;
			}
		}

		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = this.ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData value2 = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = value2;
				KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
				this.object_list[idx] = value3;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData value2 = this.ToJsonData(value);
				this[index] = value2;
			}
		}

		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				JsonData result;
				if (this.type == JsonType.Array)
				{
					result = this.inst_array[index];
				}
				else
				{
					result = this.object_list[index].Value;
				}
				return result;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = value2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		public JsonData()
		{
		}

		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
			}
			else if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
			}
			else if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
			}
			else if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
			}
			else
			{
				if (!(obj is string))
				{
					throw new ArgumentException("Unable to wrap the given object with JsonData");
				}
				this.type = JsonType.String;
				this.inst_string = (string)obj;
			}
		}

		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			this.object_list.Add(item);
			this.json = null;
		}

		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		int IList.Add(object value)
		{
			return this.Add(value);
		}

		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this.ToJsonData(value);
			this[text] = value2;
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			this.object_list.Insert(idx, item);
		}

		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		private ICollection EnsureCollection()
		{
			ICollection result;
			if (this.type == JsonType.Array)
			{
				result = (ICollection)this.inst_array;
			}
			else
			{
				if (this.type != JsonType.Object)
				{
					throw new InvalidOperationException("The JsonData instance has to be initialized first");
				}
				result = (ICollection)this.inst_object;
			}
			return result;
		}

		private IDictionary EnsureDictionary()
		{
			IDictionary result;
			if (this.type == JsonType.Object)
			{
				result = (IDictionary)this.inst_object;
			}
			else
			{
				if (this.type != JsonType.None)
				{
					throw new InvalidOperationException("Instance of JsonData is not a dictionary");
				}
				this.type = JsonType.Object;
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				result = (IDictionary)this.inst_object;
			}
			return result;
		}

		private IList EnsureList()
		{
			IList result;
			if (this.type == JsonType.Array)
			{
				result = (IList)this.inst_array;
			}
			else
			{
				if (this.type != JsonType.None)
				{
					throw new InvalidOperationException("Instance of JsonData is not a list");
				}
				this.type = JsonType.Array;
				this.inst_array = new List<JsonData>();
				result = (IList)this.inst_array;
			}
			return result;
		}

		private JsonData ToJsonData(object obj)
		{
			JsonData result;
			if (obj == null)
			{
				result = null;
			}
			else if (obj is JsonData)
			{
				result = (JsonData)obj;
			}
			else
			{
				result = new JsonData(obj);
			}
			return result;
		}

		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
			}
			else if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
			}
			else if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
			}
			else if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
			}
			else if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
			}
			else if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object current in obj)
				{
					JsonData.WriteJson((JsonData)current, writer);
				}
				writer.WriteArrayEnd();
			}
			else if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (DictionaryEntry dictionaryEntry in obj)
				{
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
			}
		}

		public int Add(object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(value2);
		}

		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
			}
			else if (this.IsArray)
			{
				((IList)this).Clear();
			}
		}

		public bool Equals(JsonData x)
		{
			bool result;
			if (x == null)
			{
				result = false;
			}
			else if (x.type != this.type)
			{
				result = false;
			}
			else
			{
				switch (this.type)
				{
				case JsonType.None:
					result = true;
					break;
				case JsonType.Object:
					result = this.inst_object.Equals(x.inst_object);
					break;
				case JsonType.Array:
					result = this.inst_array.Equals(x.inst_array);
					break;
				case JsonType.String:
					result = this.inst_string.Equals(x.inst_string);
					break;
				case JsonType.Int:
					result = this.inst_int.Equals(x.inst_int);
					break;
				case JsonType.Long:
					result = this.inst_long.Equals(x.inst_long);
					break;
				case JsonType.Double:
					result = this.inst_double.Equals(x.inst_double);
					break;
				case JsonType.Boolean:
					result = this.inst_boolean.Equals(x.inst_boolean);
					break;
				default:
					result = false;
					break;
				}
			}
			return result;
		}

		public JsonType GetJsonType()
		{
			return this.type;
		}

		public void SetJsonType(JsonType type)
		{
			if (this.type != type)
			{
				switch (type)
				{
				case JsonType.Object:
					this.inst_object = new Dictionary<string, JsonData>();
					this.object_list = new List<KeyValuePair<string, JsonData>>();
					break;
				case JsonType.Array:
					this.inst_array = new List<JsonData>();
					break;
				case JsonType.String:
					this.inst_string = null;
					break;
				case JsonType.Int:
					this.inst_int = 0;
					break;
				case JsonType.Long:
					this.inst_long = 0L;
					break;
				case JsonType.Double:
					this.inst_double = 0.0;
					break;
				case JsonType.Boolean:
					this.inst_boolean = false;
					break;
				}
				this.type = type;
			}
		}

		public string ToJson()
		{
			string result;
			if (this.json != null)
			{
				result = this.json;
			}
			else
			{
				StringWriter stringWriter = new StringWriter();
				JsonData.WriteJson(this, new JsonWriter(stringWriter)
				{
					Validate = false
				});
				this.json = stringWriter.ToString();
				result = this.json;
			}
			return result;
		}

		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		public override string ToString()
		{
			string result;
			switch (this.type)
			{
			case JsonType.Object:
				result = "JsonData object";
				break;
			case JsonType.Array:
				result = "JsonData array";
				break;
			case JsonType.String:
				result = this.inst_string;
				break;
			case JsonType.Int:
				result = this.inst_int.ToString();
				break;
			case JsonType.Long:
				result = this.inst_long.ToString();
				break;
			case JsonType.Double:
				result = this.inst_double.ToString();
				break;
			case JsonType.Boolean:
				result = this.inst_boolean.ToString();
				break;
			default:
				result = "Uninitialized JsonData";
				break;
			}
			return result;
		}
	}
}
