using System;
using System.Collections;
using System.IO;

namespace LitJson
{
	public class JsonData : IEnumerable, ICollection, IList, IDictionary, IComparable, IJsonWrapper
	{
		private IList inst_array;

		private bool inst_boolean;

		private double inst_double;

		private int inst_int;

		private long inst_long;

		private IDictionary inst_object;

		private string inst_string;

		private string json;

		private JsonType type;

		private IList object_list;

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
				IList list = new ArrayList();
				foreach (JsonKeyValuePair jsonKeyValuePair in this.object_list)
				{
					list.Add(jsonKeyValuePair.Key);
				}
				return list;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList list = new ArrayList();
				foreach (JsonKeyValuePair jsonKeyValuePair in this.object_list)
				{
					list.Add(jsonKeyValuePair.Value);
				}
				return list;
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

		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name] as JsonData;
			}
			set
			{
				this.EnsureDictionary();
				JsonKeyValuePair value2 = new JsonKeyValuePair(prop_name, value);
				if (this.inst_object.Contains(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if ((this.object_list[i] as JsonKeyValuePair).Key == prop_name)
						{
							this.object_list[i] = value2;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(value2);
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
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index] as JsonData;
				}
				return (this.object_list[index] as JsonKeyValuePair).Value;
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
					JsonKeyValuePair jsonKeyValuePair = this.object_list[index] as JsonKeyValuePair;
					JsonKeyValuePair value2 = new JsonKeyValuePair(jsonKeyValuePair.Key, value);
					this.object_list[index] = value2;
					this.inst_object[jsonKeyValuePair.Key] = value;
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
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		void IDictionary.Add(object key, object value)
		{
			JsonData jsonData = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, jsonData);
			JsonKeyValuePair value2 = new JsonKeyValuePair((string)key, jsonData);
			this.object_list.Add(value2);
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
			return this.GetOrderedEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if ((this.object_list[i] as JsonKeyValuePair).Key == (string)key)
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

		public bool Contains(object key)
		{
			return this.inst_object.Contains(key);
		}

		private IDictionaryEnumerator GetOrderedEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Hashtable();
			this.object_list = new ArrayList();
			return this.inst_object;
		}

		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new ArrayList();
			return this.inst_array;
		}

		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object current in obj)
				{
					JsonData.WriteJson((JsonData)current, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (DictionaryEntry dictionaryEntry in ((IDictionary)obj))
				{
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
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
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		public int CompareTo(object obj)
		{
			JsonData jsonData = obj as JsonData;
			if (jsonData == null)
			{
				return -1;
			}
			if (jsonData.type != this.type)
			{
				return -1;
			}
			switch (this.type)
			{
			case JsonType.None:
				return 0;
			case JsonType.Object:
				return (!this.inst_object.Equals(jsonData.inst_object)) ? -1 : 0;
			case JsonType.Array:
				return (!this.inst_array.Equals(jsonData.inst_array)) ? -1 : 0;
			case JsonType.String:
				return this.inst_string.CompareTo(jsonData.inst_string);
			case JsonType.Int:
				return this.inst_int.CompareTo(jsonData.inst_int);
			case JsonType.Long:
				return this.inst_long.CompareTo(jsonData.inst_long);
			case JsonType.Double:
				return this.inst_double.CompareTo(jsonData.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.CompareTo(jsonData.inst_boolean);
			default:
				return -1;
			}
		}

		public JsonType GetJsonType()
		{
			return this.type;
		}

		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Hashtable();
				this.object_list = new ArrayList();
				break;
			case JsonType.Array:
				this.inst_array = new ArrayList();
				break;
			case JsonType.String:
				this.inst_string = string.Empty;
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

		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
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
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
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
	}
}
