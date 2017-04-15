using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLRSharp
{
	public class MethodParamList : List<ICLRType>
	{
		private static MethodParamList _OneParam_Int = null;

		private static MethodParamList _ZeroParam = null;

		private string name = null;

		private Type[] SystemType = null;

		private MethodParamList()
		{
		}

		private MethodParamList(IList<ICLRType> types)
		{
			bool flag = types != null;
			if (flag)
			{
				foreach (ICLRType current in types)
				{
					base.Add(current);
				}
			}
		}

		public static MethodParamList const_OneParam_Int(ICLRSharp_Environment env)
		{
			bool flag = MethodParamList._OneParam_Int == null;
			if (flag)
			{
				MethodParamList._OneParam_Int = new MethodParamList(new ICLRType[]
				{
					env.GetType(typeof(int))
				});
			}
			return MethodParamList._OneParam_Int;
		}

		public static MethodParamList constEmpty()
		{
			bool flag = MethodParamList._ZeroParam == null;
			if (flag)
			{
				MethodParamList._ZeroParam = new MethodParamList(new ICLRType[0]);
			}
			return MethodParamList._ZeroParam;
		}

		public static MethodParamList Make(params ICLRType[] types)
		{
			return new MethodParamList(types);
		}

		public MethodParamList(ICLRSharp_Environment env, MethodReference method)
		{
			bool hasParameters = method.HasParameters;
			if (hasParameters)
			{
				GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
				GenericInstanceMethod genericInstanceMethod = method as GenericInstanceMethod;
				MethodParamList methodParamList = null;
				bool flag = genericInstanceMethod != null;
				if (flag)
				{
					methodParamList = new MethodParamList(env, genericInstanceMethod);
				}
				foreach (ParameterDefinition current in method.Parameters)
				{
					string text = current.ParameterType.FullName;
					bool isGenericParameter = current.ParameterType.IsGenericParameter;
					if (isGenericParameter)
					{
						bool flag2 = current.ParameterType.Name.Contains("!!");
						if (flag2)
						{
							int index = int.Parse(current.ParameterType.Name.Substring(2));
							text = methodParamList[index].FullName;
						}
						else
						{
							bool flag3 = current.ParameterType.Name.Contains("!");
							if (flag3)
							{
								int index2 = int.Parse(current.ParameterType.Name.Substring(1));
								text = genericInstanceType.GenericArguments[index2].FullName;
							}
						}
					}
					bool flag4 = text.Contains("!!");
					if (flag4)
					{
						for (int i = 0; i < methodParamList.Count; i++)
						{
							string oldValue = "!!" + i.ToString();
							text = text.Replace(oldValue, methodParamList[i].FullName);
						}
					}
					bool flag5 = text.Contains("!");
					if (flag5)
					{
						Collection<TypeReference> genericArguments = (method.DeclaringType as GenericInstanceType).GenericArguments;
						for (int j = 0; j < genericArguments.Count; j++)
						{
							string oldValue2 = "!" + j.ToString();
							text = text.Replace(oldValue2, genericArguments[j].FullName);
						}
					}
					ICLRType type = env.GetType(text);
					bool flag6 = type.IsEnum();
					if (flag6)
					{
						type = env.GetType(type.TypeForSystem);
					}
					base.Add(type);
				}
			}
		}

		public MethodParamList(ICLRSharp_Environment env, Collection<VariableDefinition> ps)
		{
			foreach (VariableDefinition current in ps)
			{
				string fullName = current.VariableType.FullName;
				ICLRType type = env.GetType(fullName);
				bool flag = type != null && type.IsEnum();
				if (flag)
				{
					type = env.GetType(type.TypeForSystem);
				}
				base.Add(type);
			}
		}

		private static ICLRType GetTType(ICLRSharp_Environment env, ParameterDefinition param, MethodParamList _methodgen)
		{
			string text = param.ParameterType.FullName;
			for (int i = 0; i < _methodgen.Count; i++)
			{
				string oldValue = "!!" + i.ToString();
				text = text.Replace(oldValue, _methodgen[i].FullName);
			}
			return env.GetType(text);
		}

		public MethodParamList(ICLRSharp_Environment env, GenericInstanceMethod method)
		{
			foreach (TypeReference current in method.GenericArguments)
			{
				string fullName = current.FullName;
				bool isGenericParameter = current.IsGenericParameter;
				if (isGenericParameter)
				{
					GenericInstanceType genericInstanceType = method.DeclaringType as GenericInstanceType;
					bool flag = current.Name[0] == '!';
					if (flag)
					{
						int index = int.Parse(current.Name.Substring(1));
						fullName = genericInstanceType.GenericArguments[index].FullName;
					}
				}
				ICLRType type = env.GetType(fullName);
				bool flag2 = type.IsEnum();
				if (flag2)
				{
					type = env.GetType(type.TypeForSystem);
				}
				base.Add(type);
			}
		}

		public MethodParamList(ICLRSharp_Environment env, MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				base.Add(env.GetType(parameterInfo.ParameterType));
			}
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			bool flag = this.name == null;
			if (flag)
			{
				this.name = "";
				foreach (ICLRType current in this)
				{
					this.name = this.name + current.ToString() + ";";
				}
			}
			return this.name;
		}

		public Type[] ToArraySystem()
		{
			bool flag = this.SystemType == null;
			if (flag)
			{
				this.SystemType = new Type[base.Count];
				for (int i = 0; i < base.Count; i++)
				{
					this.SystemType[i] = base[i].TypeForSystem;
				}
			}
			return this.SystemType;
		}
	}
}
