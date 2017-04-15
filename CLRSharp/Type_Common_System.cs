using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLRSharp
{
	public class Type_Common_System : ICLRType_System, ICLRType
	{
		public Type TypeForSystem
		{
			get;
			private set;
		}

		public ICLRSharp_Environment env
		{
			get;
			private set;
		}

		public ICLRType[] SubTypes
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return this.TypeForSystem.Name;
			}
		}

		public string FullName
		{
			get
			{
				return this.TypeForSystem.FullName;
			}
		}

		public string FullNameWithAssembly
		{
			get;
			private set;
		}

		public Type_Common_System(ICLRSharp_Environment env, Type type, ICLRType[] subtype)
		{
			this.env = env;
			this.TypeForSystem = type;
			this.FullNameWithAssembly = type.AssemblyQualifiedName;
			this.SubTypes = subtype;
		}

		public virtual IMethod GetMethod(string funcname, MethodParamList types)
		{
			bool flag = funcname == ".ctor";
			IMethod result;
			if (flag)
			{
				ConstructorInfo constructor = this.TypeForSystem.GetConstructor(types.ToArraySystem());
				result = new Method_Common_System(this, constructor);
			}
			else
			{
				MethodInfo method = this.TypeForSystem.GetMethod(funcname, types.ToArraySystem());
				result = new Method_Common_System(this, method);
			}
			return result;
		}

		public virtual IMethod[] GetMethods(string funcname)
		{
			List<IMethod> list = new List<IMethod>();
			bool flag = funcname == ".ctor";
			if (flag)
			{
				ConstructorInfo[] constructors = this.TypeForSystem.GetConstructors();
				ConstructorInfo[] array = constructors;
				for (int i = 0; i < array.Length; i++)
				{
					ConstructorInfo method = array[i];
					list.Add(new Method_Common_System(this, method));
				}
			}
			else
			{
				MethodInfo[] methods = this.TypeForSystem.GetMethods();
				MethodInfo[] array2 = methods;
				for (int j = 0; j < array2.Length; j++)
				{
					MethodInfo methodInfo = array2[j];
					bool flag2 = methodInfo.Name == funcname;
					if (flag2)
					{
						list.Add(new Method_Common_System(this, methodInfo));
					}
				}
			}
			return list.ToArray();
		}

		public virtual IMethod[] GetAllMethods()
		{
			List<IMethod> list = new List<IMethod>();
			MethodInfo[] methods = this.TypeForSystem.GetMethods();
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo method = array[i];
				list.Add(new Method_Common_System(this, method));
			}
			return list.ToArray();
		}

		public object InitObj()
		{
			return Activator.CreateInstance(this.TypeForSystem);
		}

		public virtual IMethod GetMethodT(string funcname, MethodParamList ttypes, MethodParamList types)
		{
			MethodInfo methodInfo = null;
			MethodInfo[] methods = this.TypeForSystem.GetMethods();
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo2 = array[i];
				bool flag = methodInfo2.Name == funcname && methodInfo2.IsGenericMethodDefinition;
				if (flag)
				{
					Type[] genericArguments = methodInfo2.GetGenericArguments();
					ParameterInfo[] parameters = methodInfo2.GetParameters();
					bool flag2 = genericArguments.Length == ttypes.Count && parameters.Length == types.Count;
					if (flag2)
					{
						methodInfo = methodInfo2;
						break;
					}
				}
			}
			return new Method_Common_System(this, methodInfo.MakeGenericMethod(ttypes.ToArraySystem()));
		}

		public virtual IField GetField(string name)
		{
			return new Field_Common_System(this.env, this.TypeForSystem.GetField(name));
		}

		public bool IsInst(object obj)
		{
			return this.TypeForSystem.IsInstanceOfType(obj);
		}

		public ICLRType GetNestType(ICLRSharp_Environment env, string fullname)
		{
			throw new NotImplementedException();
		}

		public Delegate CreateDelegate(Type deletype, object _this, IMethod_System _method)
		{
			return Delegate.CreateDelegate(deletype, _this, _method.method_System as MethodInfo);
		}

		public string[] GetFieldNames()
		{
			FieldInfo[] fields = this.TypeForSystem.GetFields();
			string[] array = new string[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array[i] = fields[i].Name;
			}
			return array;
		}

		public bool IsEnum()
		{
			return this.TypeForSystem.IsEnum;
		}
	}
}
