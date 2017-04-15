using System;
using System.Collections;

namespace LitJson
{
	internal struct ObjectMetadata
	{
		private Type element_type;

		private bool is_dictionary;

		private IDictionary properties;

		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}
	}
}
