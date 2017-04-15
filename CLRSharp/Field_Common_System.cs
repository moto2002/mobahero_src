using System;
using System.Reflection;

namespace CLRSharp
{
	internal class Field_Common_System : IField
	{
		public FieldInfo info;

		public ICLRType FieldType
		{
			get;
			private set;
		}

		public ICLRType DeclaringType
		{
			get;
			private set;
		}

		public bool isStatic
		{
			get
			{
				return this.info.IsStatic;
			}
		}

		public Field_Common_System(ICLRSharp_Environment env, FieldInfo field)
		{
			this.info = field;
			this.FieldType = env.GetType(field.FieldType);
			this.DeclaringType = env.GetType(field.DeclaringType);
		}

		public void Set(object _this, object value)
		{
			this.info.SetValue(_this, value);
		}

		public object Get(object _this)
		{
			return this.info.GetValue(_this);
		}
	}
}
