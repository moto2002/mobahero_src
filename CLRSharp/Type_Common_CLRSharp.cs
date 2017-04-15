using Mono.Cecil;
using System;
using System.Collections.Generic;

namespace CLRSharp
{
	public class Type_Common_CLRSharp : ICLRType_Sharp, ICLRType
	{
		public List<ICLRType> _Interfaces = null;

		private CLRSharp_Instance _staticInstance = null;

		private bool _isenum = false;

		public Type TypeForSystem
		{
			get
			{
				bool isenum = this._isenum;
				Type typeFromHandle;
				if (isenum)
				{
					typeFromHandle = typeof(int);
				}
				else
				{
					typeFromHandle = typeof(CLRSharp_Instance);
				}
				return typeFromHandle;
			}
		}

		public TypeDefinition type_CLRSharp
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

		public ICLRType BaseType
		{
			get;
			private set;
		}

		public bool HasSysBase
		{
			get;
			private set;
		}

		public string Name
		{
			get
			{
				return this.type_CLRSharp.Name;
			}
		}

		public string FullName
		{
			get
			{
				return this.type_CLRSharp.FullName;
			}
		}

		public string FullNameWithAssembly
		{
			get
			{
				bool isenum = this._isenum;
				string result;
				if (isenum)
				{
					result = this.env.GetType(typeof(int)).FullNameWithAssembly;
				}
				else
				{
					result = this.type_CLRSharp.FullName;
				}
				return result;
			}
		}

		public CLRSharp_Instance staticInstance
		{
			get
			{
				bool flag = this._staticInstance == null;
				if (flag)
				{
					this._staticInstance = new CLRSharp_Instance(this);
				}
				return this._staticInstance;
			}
		}

		public bool NeedCCtor
		{
			get;
			private set;
		}

		public bool ContainBase(Type t)
		{
			bool flag = this.BaseType != null && this.BaseType.TypeForSystem == t;
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				bool flag2 = this._Interfaces == null;
				if (flag2)
				{
					result = false;
				}
				else
				{
					foreach (ICLRType current in this._Interfaces)
					{
						bool flag3 = current.TypeForSystem == t;
						if (flag3)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
			}
			return result;
		}

		public string[] GetMethodNames()
		{
			string[] array = new string[this.type_CLRSharp.Methods.Count];
			for (int i = 0; i < this.type_CLRSharp.Methods.Count; i++)
			{
				array[i] = this.type_CLRSharp.Methods[i].Name;
			}
			return array;
		}

		public Type_Common_CLRSharp(ICLRSharp_Environment env, TypeDefinition type)
		{
			this.env = env;
			this.type_CLRSharp = type;
			bool isEnum = type.IsEnum;
			if (isEnum)
			{
				this._isenum = true;
			}
			bool flag = this.type_CLRSharp.BaseType != null;
			if (flag)
			{
				this.BaseType = env.GetType(this.type_CLRSharp.BaseType.FullName);
				bool flag2 = this.BaseType is ICLRType_System;
				if (flag2)
				{
					bool flag3 = this.BaseType.TypeForSystem == typeof(Enum) || this.BaseType.TypeForSystem == typeof(object) || this.BaseType.TypeForSystem == typeof(ValueType) || this.BaseType.TypeForSystem == typeof(Enum);
					if (!flag3)
					{
						env.logger.Log_Error("ScriptType:" + this.Name + " Based On a SystemType:" + this.BaseType.Name);
						this.HasSysBase = true;
						throw new Exception("不得继承系统类型，脚本类型系统和脚本类型系统是隔离的");
					}
					this.BaseType = null;
				}
				bool hasInterfaces = this.type_CLRSharp.HasInterfaces;
				if (hasInterfaces)
				{
					this._Interfaces = new List<ICLRType>();
					bool flag4 = true;
					foreach (TypeReference current in this.type_CLRSharp.Interfaces)
					{
						ICLRType type2 = env.GetType(current.FullName);
						bool flag5 = type2 is ICLRType_System;
						if (flag5)
						{
							Type typeForSystem = (type2 as ICLRType_System).TypeForSystem;
							bool flag6 = flag4 & env.GetCrossBind(typeForSystem) == null;
							if (flag6)
							{
								bool isInterface = typeForSystem.IsInterface;
								if (isInterface)
								{
									Type[] interfaces = typeForSystem.GetInterfaces();
									for (int i = 0; i < interfaces.Length; i++)
									{
										Type type3 = interfaces[i];
										bool flag7 = env.GetCrossBind(type3) != null;
										if (flag7)
										{
											flag4 = false;
											break;
										}
									}
								}
								bool flag8 = flag4;
								if (flag8)
								{
									env.logger.Log_Warning("警告:没有CrossBind的情况下直接继承\nScriptType:" + this.Name + " Based On a SystemInterface:" + type2.Name);
								}
							}
							this.HasSysBase = true;
						}
						this._Interfaces.Add(type2);
					}
				}
			}
			foreach (MethodDefinition current2 in this.type_CLRSharp.Methods)
			{
				bool flag9 = current2.Name == ".cctor";
				if (flag9)
				{
					this.NeedCCtor = true;
					break;
				}
			}
		}

		public IMethod GetVMethod(IMethod _base)
		{
			ICLRType_Sharp iCLRType_Sharp = this;
			IMethod result;
			while (iCLRType_Sharp != _base.DeclaringType && iCLRType_Sharp != null)
			{
				IMethod method = iCLRType_Sharp.GetMethod(_base.Name, _base.ParamList);
				bool flag = method != null;
				if (flag)
				{
					result = method;
					return result;
				}
				iCLRType_Sharp = (this.env.GetType(iCLRType_Sharp.type_CLRSharp.BaseType.FullName) as ICLRType_Sharp);
			}
			result = _base;
			return result;
		}

		public void ResetStaticInstace()
		{
			this._staticInstance = null;
			foreach (MethodDefinition current in this.type_CLRSharp.Methods)
			{
				bool flag = current.Name == ".cctor";
				if (flag)
				{
					this.NeedCCtor = true;
					break;
				}
			}
		}

		public IMethod GetMethod(string funcname, MethodParamList types)
		{
			bool hasMethods = this.type_CLRSharp.HasMethods;
			IMethod result;
			if (hasMethods)
			{
				foreach (MethodDefinition current in this.type_CLRSharp.Methods)
				{
					bool flag = current.Name != funcname;
					if (!flag)
					{
						bool flag2 = (types == null) ? (!current.HasParameters) : (current.Parameters.Count == types.Count);
						if (flag2)
						{
							bool flag3 = true;
							for (int i = 0; i < ((types == null) ? 0 : types.Count); i++)
							{
								ICLRType type = this.env.GetType(current.Parameters[i].ParameterType.FullName);
								bool flag4 = type.IsEnum();
								if (flag4)
								{
									bool flag5 = type.TypeForSystem != types[i].TypeForSystem;
									if (flag5)
									{
										flag3 = false;
										break;
									}
								}
								else
								{
									bool flag6 = type != types[i];
									if (flag6)
									{
										flag3 = false;
										break;
									}
								}
							}
							bool flag7 = flag3;
							if (flag7)
							{
								result = new Method_Common_CLRSharp(this, current);
								return result;
							}
						}
					}
				}
			}
			result = null;
			return result;
		}

		public IMethod GetMethodOverloaded(string funcname, MethodParamList types)
		{
			MethodDefinition methodDefinition = null;
			List<int> list = null;
			bool hasMethods = this.type_CLRSharp.HasMethods;
			IMethod result;
			if (hasMethods)
			{
				foreach (MethodDefinition current in this.type_CLRSharp.Methods)
				{
					bool flag = current.Name != funcname;
					if (!flag)
					{
						bool flag2 = (types == null) ? (!current.HasParameters) : (current.Parameters.Count == types.Count);
						if (flag2)
						{
							bool flag3 = true;
							List<int> list2 = new List<int>();
							for (int i = 0; i < ((types == null) ? 0 : types.Count); i++)
							{
								ICLRType type = this.env.GetType(current.Parameters[i].ParameterType.FullName);
								bool flag4 = type.IsEnum();
								if (flag4)
								{
									bool flag5 = type.TypeForSystem != types[i].TypeForSystem;
									if (flag5)
									{
										flag3 = false;
										break;
									}
								}
								else
								{
									bool flag6 = !type.TypeForSystem.IsAssignableFrom(types[i].TypeForSystem);
									if (flag6)
									{
										flag3 = false;
										break;
									}
									list2.Add(this.GetInheritanceDistance(type.TypeForSystem, types[i].TypeForSystem));
								}
							}
							bool flag7 = flag3;
							if (flag7)
							{
								bool flag8 = list == null;
								if (flag8)
								{
									methodDefinition = current;
									list = list2;
								}
								else
								{
									for (int j = 0; j < list2.Count; j++)
									{
										bool flag9 = list2[j] < list[j];
										if (flag9)
										{
											methodDefinition = current;
											list = list2;
										}
									}
								}
							}
						}
					}
				}
				bool flag10 = methodDefinition == null;
				if (flag10)
				{
					result = null;
				}
				else
				{
					result = new Method_Common_CLRSharp(this, methodDefinition);
				}
			}
			else
			{
				result = null;
			}
			return result;
		}

		public int GetInheritanceDistance(Type baseClass, Type subClass)
		{
			bool flag = baseClass == subClass;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				bool flag2 = !baseClass.IsAssignableFrom(subClass);
				if (flag2)
				{
					result = 2147483647;
				}
				else
				{
					int num = 0;
					while ((subClass = subClass.BaseType) != baseClass)
					{
						num++;
					}
					result = num + 1;
				}
			}
			return result;
		}

		public IMethod[] GetMethods(string funcname)
		{
			List<IMethod> list = new List<IMethod>();
			bool hasMethods = this.type_CLRSharp.HasMethods;
			if (hasMethods)
			{
				foreach (MethodDefinition current in this.type_CLRSharp.Methods)
				{
					bool flag = current.Name != funcname;
					if (!flag)
					{
						list.Add(new Method_Common_CLRSharp(this, current));
					}
				}
			}
			return list.ToArray();
		}

		public IMethod[] GetAllMethods()
		{
			List<IMethod> list = new List<IMethod>();
			bool hasMethods = this.type_CLRSharp.HasMethods;
			if (hasMethods)
			{
				foreach (MethodDefinition current in this.type_CLRSharp.Methods)
				{
					list.Add(new Method_Common_CLRSharp(this, current));
				}
			}
			return list.ToArray();
		}

		public object InitObj()
		{
			return new CLRSharp_Instance(this);
		}

		public IMethod GetMethodT(string funcname, MethodParamList ttypes, MethodParamList types)
		{
			return null;
		}

		public IField GetField(string name)
		{
			IField result;
			foreach (FieldDefinition current in this.type_CLRSharp.Fields)
			{
				bool flag = current.Name == name;
				if (flag)
				{
					result = new Field_Common_CLRSharp(this, current);
					return result;
				}
			}
			result = null;
			return result;
		}

		public bool IsInst(object obj)
		{
			bool flag = obj is CLRSharp_Instance;
			bool result;
			if (flag)
			{
				CLRSharp_Instance cLRSharp_Instance = obj as CLRSharp_Instance;
				bool flag2 = cLRSharp_Instance.type == this;
				if (flag2)
				{
					result = true;
					return result;
				}
				bool flag3 = cLRSharp_Instance.type != null;
				if (flag3)
				{
					Type_Common_CLRSharp type_Common_CLRSharp = cLRSharp_Instance.type as Type_Common_CLRSharp;
					bool flag4 = type_Common_CLRSharp._Interfaces.Count > 0;
					if (flag4)
					{
						foreach (ICLRType current in type_Common_CLRSharp._Interfaces)
						{
							bool flag5 = current == this;
							if (flag5)
							{
								result = true;
								return result;
							}
						}
					}
				}
			}
			result = false;
			return result;
		}

		public ICLRType GetNestType(ICLRSharp_Environment env, string fullname)
		{
			ICLRType result;
			foreach (TypeDefinition current in this.type_CLRSharp.NestedTypes)
			{
				bool flag = current.Name == fullname;
				if (flag)
				{
					Type_Common_CLRSharp type_Common_CLRSharp = new Type_Common_CLRSharp(env, current);
					env.RegType(type_Common_CLRSharp);
					result = type_Common_CLRSharp;
					return result;
				}
			}
			result = null;
			return result;
		}

		public void InvokeCCtor(ThreadContext context)
		{
			this.NeedCCtor = false;
			this.GetMethod(".cctor", null).Invoke(context, this.staticInstance, new object[0]);
		}

		public string[] GetFieldNames()
		{
			string[] array = new string[this.type_CLRSharp.Fields.Count];
			for (int i = 0; i < this.type_CLRSharp.Fields.Count; i++)
			{
				array[i] = this.type_CLRSharp.Fields[i].Name;
			}
			return array;
		}

		public bool IsEnum()
		{
			return this._isenum;
		}
	}
}
