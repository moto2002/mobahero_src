using Mono.Cecil;
using System;

namespace CLRSharp
{
	public class Field_Common_CLRSharp : IField
	{
		public Type_Common_CLRSharp _DeclaringType;

		public FieldDefinition field;

		public ICLRType FieldType
		{
			get;
			private set;
		}

		public ICLRType DeclaringType
		{
			get
			{
				return this._DeclaringType;
			}
		}

		public bool isStatic
		{
			get
			{
				return this.field.IsStatic;
			}
		}

		public Field_Common_CLRSharp(Type_Common_CLRSharp type, FieldDefinition field)
		{
			this.field = field;
			this.FieldType = type.env.GetType(field.FieldType.FullName);
			this._DeclaringType = type;
		}

		public void Set(object _this, object value)
		{
			bool flag = _this == null;
			CLRSharp_Instance cLRSharp_Instance;
			if (flag)
			{
				cLRSharp_Instance = this._DeclaringType.staticInstance;
			}
			else
			{
				cLRSharp_Instance = (_this as CLRSharp_Instance);
			}
			cLRSharp_Instance.Fields[this.field.Name] = value;
		}

		public object Get(object _this)
		{
			bool flag = _this == null;
			CLRSharp_Instance cLRSharp_Instance;
			if (flag)
			{
				cLRSharp_Instance = this._DeclaringType.staticInstance;
			}
			else
			{
				cLRSharp_Instance = (_this as CLRSharp_Instance);
			}
			object result = null;
			cLRSharp_Instance.Fields.TryGetValue(this.field.Name, out result);
			return result;
		}
	}
}
