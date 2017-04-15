using System;

namespace LitJson
{
	public class JsonKeyValuePair
	{
		private JsonData data;

		private string key;

		public JsonData Value
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		public JsonKeyValuePair(string key, JsonData data)
		{
			this.key = key;
			this.data = data;
		}
	}
}
