using System;
using System.Collections.Generic;

namespace MobaClientCom
{
	internal struct ObjectMetadata
	{
		private Type element_type;

		private bool is_dictionary;

		private IDictionary<string, PropertyMetadata> properties;

		public Type ElementType
		{
			get
			{
				Type typeFromHandle;
				if (this.element_type == null)
				{
					typeFromHandle = typeof(JsonData);
				}
				else
				{
					typeFromHandle = this.element_type;
				}
				return typeFromHandle;
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

		public IDictionary<string, PropertyMetadata> Properties
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
