using System;

namespace MobaClientCom
{
	internal struct ArrayMetadata
	{
		private Type element_type;

		private bool is_array;

		private bool is_list;

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

		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}
	}
}
