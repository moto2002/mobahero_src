using System;
using System.Reflection;

namespace Pathfinding.Serialization.JsonFx
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class JsonSpecifiedPropertyAttribute : Attribute
	{
		private string specifiedProperty;

		public string SpecifiedProperty
		{
			get
			{
				return this.specifiedProperty;
			}
			set
			{
				this.specifiedProperty = value;
			}
		}

		public JsonSpecifiedPropertyAttribute(string propertyName)
		{
			this.specifiedProperty = propertyName;
		}

		public static string GetJsonSpecifiedProperty(MemberInfo memberInfo)
		{
			if (memberInfo == null || !Attribute.IsDefined(memberInfo, typeof(JsonSpecifiedPropertyAttribute)))
			{
				return null;
			}
			JsonSpecifiedPropertyAttribute jsonSpecifiedPropertyAttribute = (JsonSpecifiedPropertyAttribute)Attribute.GetCustomAttribute(memberInfo, typeof(JsonSpecifiedPropertyAttribute));
			return jsonSpecifiedPropertyAttribute.SpecifiedProperty;
		}
	}
}
