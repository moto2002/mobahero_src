using System;
using System.Collections.Generic;

namespace Pathfinding.Serialization.JsonFx
{
	public class JsonReaderSettings
	{
		internal readonly TypeCoercionUtility Coercion = new TypeCoercionUtility();

		private bool allowUnquotedObjectKeys;

		private string typeHintName;

		protected List<JsonConverter> converters = new List<JsonConverter>();

		public bool HandleCyclicReferences
		{
			get;
			set;
		}

		public bool AllowNullValueTypes
		{
			get
			{
				return this.Coercion.AllowNullValueTypes;
			}
			set
			{
				this.Coercion.AllowNullValueTypes = value;
			}
		}

		public bool AllowUnquotedObjectKeys
		{
			get
			{
				return this.allowUnquotedObjectKeys;
			}
			set
			{
				this.allowUnquotedObjectKeys = value;
			}
		}

		public string TypeHintName
		{
			get
			{
				return this.typeHintName;
			}
			set
			{
				this.typeHintName = value;
			}
		}

		internal bool IsTypeHintName(string name)
		{
			return !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(this.typeHintName) && StringComparer.Ordinal.Equals(this.typeHintName, name);
		}

		public virtual JsonConverter GetConverter(Type type)
		{
			for (int i = 0; i < this.converters.Count; i++)
			{
				if (this.converters[i].CanConvert(type))
				{
					return this.converters[i];
				}
			}
			return null;
		}

		public virtual void AddTypeConverter(JsonConverter converter)
		{
			this.converters.Add(converter);
		}
	}
}
