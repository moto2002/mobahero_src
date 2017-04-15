using System;
using System.Reflection;

internal class InfoValue
{
	private static BindingFlags bFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	public object obj;

	public string name;

	public string showName;

	private Type objType;

	private FieldInfo fieldInfo;

	private PropertyInfo propertyInfo;

	public InfoValue(object o, string n)
	{
		this.obj = o;
		this.name = n;
		this.showName = n;
		this.objType = this.obj.GetType();
		this.fieldInfo = this.objType.GetField(n, InfoValue.bFlags);
		this.propertyInfo = this.objType.GetProperty(n, InfoValue.bFlags);
		if (this.fieldInfo == null && this.propertyInfo == null)
		{
			throw new MissingMethodException(string.Concat(new object[]
			{
				"Property or field '",
				n,
				"' not found on ",
				this.obj
			}));
		}
	}

	public object Get()
	{
		if (this.propertyInfo != null)
		{
			return this.propertyInfo.GetValue(this.obj, null);
		}
		return this.fieldInfo.GetValue(this.obj);
	}

	public void Set(object value)
	{
		if (this.propertyInfo != null)
		{
			this.propertyInfo.SetValue(this.obj, value, null);
		}
		else
		{
			this.fieldInfo.SetValue(this.obj, value);
		}
	}

	public override bool Equals(object _obj)
	{
		if (_obj is InfoValue)
		{
			InfoValue infoValue = (InfoValue)_obj;
			return this.obj == infoValue.obj && this.name == infoValue.name;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}
}
