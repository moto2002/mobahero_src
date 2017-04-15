using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLRSharp
{
	internal class Method_Common_System : IMethod_System, IMethod
	{
		public bool isStatic
		{
			get
			{
				return this.method_System.IsStatic;
			}
		}

		public string Name
		{
			get
			{
				return this.method_System.Name;
			}
		}

		public ICLRType DeclaringType
		{
			get;
			private set;
		}

		public ICLRType ReturnType
		{
			get;
			private set;
		}

		public MethodParamList ParamList
		{
			get;
			private set;
		}

		public MethodBase method_System
		{
			get;
			private set;
		}

		public Method_Common_System(ICLRType DeclaringType, MethodBase method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new Exception("not allow null method.");
			}
			this.method_System = method;
			this.DeclaringType = DeclaringType;
			bool flag2 = method is MethodInfo;
			if (flag2)
			{
				MethodInfo methodInfo = method as MethodInfo;
				this.ReturnType = DeclaringType.env.GetType(methodInfo.ReturnType);
			}
			this.ParamList = new MethodParamList(DeclaringType.env, method);
		}

		public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
		{
			bool flag = this.Name == "Concat" && this.DeclaringType.TypeForSystem == typeof(string);
			object result;
			if (flag)
			{
				bool flag2 = _params.Length == 1;
				if (flag2)
				{
					bool flag3 = _params[0] == null;
					if (flag3)
					{
						result = "null";
					}
					else
					{
						bool flag4 = _params[0] is string[];
						if (flag4)
						{
							result = string.Concat(_params[0] as string[]);
						}
						else
						{
							bool flag5 = _params[0] is object[];
							if (flag5)
							{
								result = string.Concat(_params[0] as object[]);
							}
							else
							{
								result = _params[0].ToString();
							}
						}
					}
				}
				else
				{
					string text = "null";
					bool flag6 = _params[0] != null;
					if (flag6)
					{
						text = _params[0].ToString();
					}
					for (int i = 1; i < _params.Length; i++)
					{
						bool flag7 = _params[i] != null;
						if (flag7)
						{
							text += _params[i];
						}
						else
						{
							text += "null";
						}
					}
					result = text;
				}
			}
			else
			{
				result = this.Invoke(context, _this, _params);
			}
			return result;
		}

		public object Invoke(ThreadContext context, object _this, object[] _params)
		{
			bool flag = _this is CLRSharp_Instance;
			if (flag)
			{
				CLRSharp_Instance cLRSharp_Instance = _this as CLRSharp_Instance;
				bool hasSysBase = cLRSharp_Instance.type.HasSysBase;
				if (hasSysBase)
				{
					bool flag2 = cLRSharp_Instance.type.ContainBase(this.method_System.DeclaringType);
					bool flag3 = flag2;
					if (flag3)
					{
						ICrossBind crossBind = context.environment.GetCrossBind(this.method_System.DeclaringType);
						bool flag4 = crossBind != null;
						if (flag4)
						{
							_this = crossBind.CreateBind(cLRSharp_Instance);
						}
						else
						{
							_this = (_this as CLRSharp_Instance).system_base;
						}
					}
				}
			}
			bool flag5 = this.method_System is ConstructorInfo;
			object result;
			if (flag5)
			{
				bool flag6 = this.method_System.DeclaringType.IsSubclassOf(typeof(Delegate));
				if (flag6)
				{
					object obj = _params[0];
					RefFunc refFunc = _params[1] as RefFunc;
					ICLRType_Sharp iCLRType_Sharp = refFunc._method.DeclaringType as ICLRType_Sharp;
					bool flag7 = iCLRType_Sharp != null;
					if (flag7)
					{
						CLRSharp_Instance cLRSharp_Instance2 = obj as CLRSharp_Instance;
						bool flag8 = refFunc._method.isStatic && iCLRType_Sharp != null;
						if (flag8)
						{
							cLRSharp_Instance2 = iCLRType_Sharp.staticInstance;
						}
						result = cLRSharp_Instance2.GetDelegate(context, this.method_System.DeclaringType, refFunc._method);
					}
					else
					{
						ICLRType_System iCLRType_System = refFunc._method.DeclaringType as ICLRType_System;
						result = iCLRType_System.CreateDelegate(this.method_System.DeclaringType, obj, refFunc._method as IMethod_System);
					}
				}
				else
				{
					object[] array = null;
					bool flag9 = _params != null && _params.Length != 0;
					if (flag9)
					{
						array = new object[_params.Length];
						ParameterInfo[] parameters = this.method_System.GetParameters();
						for (int i = 0; i < _params.Length; i++)
						{
							bool flag10 = _params[i] == null;
							if (flag10)
							{
								array[i] = null;
							}
							Type type = _params[i].GetType();
							Type parameterType = parameters[i].ParameterType;
							bool flag11 = type == parameterType;
							if (flag11)
							{
								array[i] = _params[i];
							}
							else
							{
								bool flag12 = type.IsSubclassOf(parameterType);
								if (flag12)
								{
									array[i] = _params[i];
								}
								else
								{
									bool isEnum = parameters[i].ParameterType.IsEnum;
									if (isEnum)
									{
										MethodInfo[] methods = parameters[i].ParameterType.GetMethods();
										array[i] = Enum.ToObject(parameters[i].ParameterType, _params[i]);
									}
									else
									{
										bool flag13 = parameterType == typeof(byte);
										if (flag13)
										{
											array[i] = (byte)Convert.ToDecimal(_params[i]);
										}
										else
										{
											array[i] = _params[i];
										}
									}
								}
							}
						}
					}
					object obj2 = (this.method_System as ConstructorInfo).Invoke(array);
					result = obj2;
				}
			}
			else
			{
				Dictionary<int, object> dictionary = new Dictionary<int, object>();
				object[] array2 = null;
				bool flag14 = _params != null && _params.Length != 0;
				if (flag14)
				{
					array2 = new object[_params.Length];
					ParameterInfo[] parameters2 = this.method_System.GetParameters();
					for (int j = 0; j < _params.Length; j++)
					{
						bool flag15 = _params[j] is StackFrame.RefObj;
						if (flag15)
						{
							object obj3 = (_params[j] as StackFrame.RefObj).Get();
							bool flag16 = obj3 is VBox;
							if (flag16)
							{
								obj3 = (obj3 as VBox).BoxDefine();
							}
							dictionary[j] = obj3;
							array2[j] = obj3;
						}
						else
						{
							bool isEnum2 = parameters2[j].ParameterType.IsEnum;
							if (isEnum2)
							{
								MethodInfo[] methods2 = parameters2[j].ParameterType.GetMethods();
								array2[j] = Enum.ToObject(parameters2[j].ParameterType, _params[j]);
							}
							else
							{
								bool flag17 = parameters2[j].ParameterType == typeof(ulong) && _params[j] is long;
								if (flag17)
								{
									array2[j] = (ulong)((long)_params[j]);
								}
								else
								{
									bool flag18 = parameters2[j].ParameterType == typeof(uint) && _params[j] is int;
									if (flag18)
									{
										array2[j] = (uint)((int)_params[j]);
									}
									else
									{
										array2[j] = _params[j];
									}
								}
							}
						}
					}
				}
				object obj4 = this.method_System.Invoke(_this, array2);
				foreach (KeyValuePair<int, object> current in dictionary)
				{
					bool flag19 = current.Value is VBox;
					if (flag19)
					{
						(current.Value as VBox).SetDirect(array2[current.Key]);
					}
					else
					{
						(_params[current.Key] as StackFrame.RefObj).Set(array2[current.Key]);
					}
				}
				result = obj4;
			}
			return result;
		}

		public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
		{
			object result;
			try
			{
				result = this.Invoke(context, _this, _params);
			}
			catch (Exception ex)
			{
				bool flag = context == null;
				if (flag)
				{
					context = ThreadContext.activeContext;
				}
				bool flag2 = context == null;
				if (flag2)
				{
					throw new Exception("当前线程没有创建ThreadContext,无法Dump", ex);
				}
				context.environment.logger.Log_Error("Error InSystemCall:" + this.DeclaringType.FullName + "::" + this.Name);
				throw ex;
			}
			return result;
		}
	}
}
