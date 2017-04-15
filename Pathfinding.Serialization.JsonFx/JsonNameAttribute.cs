using System;
using System.Reflection;

namespace Pathfinding.Serialization.JsonFx
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
	public class JsonNameAttribute : Attribute
	{
		private string jsonName;

		public string Name
		{
			get
			{
				return this.jsonName;
			}
			set
			{
				this.jsonName = EcmaScriptIdentifier.EnsureValidIdentifier(value, false);
			}
		}

		public JsonNameAttribute()
		{
		}

		public JsonNameAttribute(string jsonName)
		{
			this.jsonName = EcmaScriptIdentifier.EnsureValidIdentifier(jsonName, false);
		}

		public static string GetJsonName(object value)
		{
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			MemberInfo memberInfo;
			if (type.IsEnum)
			{
				string name = Enum.GetName(type, value);
				if (string.IsNullOrEmpty(name))
				{
					return null;
				}
				memberInfo = type.GetField(name);
			}
			else
			{
				memberInfo = (value as MemberInfo);
			}
			if (memberInfo == null)
			{
				throw new ArgumentException();
			}
			if (!Attribute.IsDefined(memberInfo, typeof(JsonNameAttribute)))
			{
				return null;
			}
			JsonNameAttribute jsonNameAttribute = (JsonNameAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(JsonNameAttribute));
			return jsonNameAttribute.Name;
		}
	}
}
