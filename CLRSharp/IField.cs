using System;

namespace CLRSharp
{
	public interface IField
	{
		bool isStatic
		{
			get;
		}

		ICLRType DeclaringType
		{
			get;
		}

		ICLRType FieldType
		{
			get;
		}

		void Set(object _this, object value);

		object Get(object _this);
	}
}
