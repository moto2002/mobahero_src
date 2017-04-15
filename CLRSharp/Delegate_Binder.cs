using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLRSharp
{
	public class Delegate_Binder
	{
		private static Dictionary<Type, IDelegate_BindTool> mapBind = new Dictionary<Type, IDelegate_BindTool>();

		public static void RegBind(Type deletype, IDelegate_BindTool bindtool)
		{
			Delegate_Binder.mapBind[deletype] = bindtool;
		}

		public static Delegate MakeDelegate(Type deletype, CLRSharp_Instance _this_inst, IMethod __method)
		{
			IDelegate_BindTool delegate_BindTool = null;
			bool flag = Delegate_Binder.mapBind.TryGetValue(deletype, out delegate_BindTool);
			Delegate result;
			if (flag)
			{
				result = delegate_BindTool.CreateDele(deletype, null, _this_inst, __method);
			}
			else
			{
				MethodInfo method = deletype.GetMethod("Invoke");
				bool isStatic = __method.isStatic;
				if (isStatic)
				{
					_this_inst = null;
				}
				ParameterInfo[] parameters = method.GetParameters();
				bool flag2 = method.ReturnType == typeof(void);
				if (flag2)
				{
					bool flag3 = parameters.Length == 0;
					if (flag3)
					{
						delegate_BindTool = new Delegate_BindTool();
					}
					else
					{
						bool flag4 = parameters.Length == 1;
						if (flag4)
						{
							Type type = typeof(Delegate_BindTool<>).MakeGenericType(new Type[]
							{
								parameters[0].ParameterType
							});
							delegate_BindTool = (type.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
						}
						else
						{
							bool flag5 = parameters.Length == 2;
							if (flag5)
							{
								Type type2 = typeof(Delegate_BindTool<, >).MakeGenericType(new Type[]
								{
									parameters[0].ParameterType,
									parameters[1].ParameterType
								});
								delegate_BindTool = (type2.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
							}
							else
							{
								bool flag6 = parameters.Length == 3;
								if (flag6)
								{
									Type type3 = typeof(Delegate_BindTool<, , >).MakeGenericType(new Type[]
									{
										parameters[0].ParameterType,
										parameters[1].ParameterType,
										parameters[2].ParameterType
									});
									delegate_BindTool = (type3.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
								}
								else
								{
									bool flag7 = parameters.Length == 4;
									if (!flag7)
									{
										throw new Exception("还没有支持这么多参数的委托");
									}
									Type type4 = typeof(Delegate_BindTool<, , , >).MakeGenericType(new Type[]
									{
										parameters[0].ParameterType,
										parameters[1].ParameterType,
										parameters[2].ParameterType,
										parameters[3].ParameterType
									});
									delegate_BindTool = (type4.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
								}
							}
						}
					}
				}
				else
				{
					bool flag8 = parameters.Length == 0;
					if (flag8)
					{
						Type type5 = typeof(Delegate_BindTool_Ret<>).MakeGenericType(new Type[]
						{
							method.ReturnType
						});
						delegate_BindTool = (type5.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
					}
					else
					{
						bool flag9 = parameters.Length == 1;
						if (flag9)
						{
							Type type6 = typeof(Delegate_BindTool_Ret<, >).MakeGenericType(new Type[]
							{
								method.ReturnType,
								parameters[0].ParameterType
							});
							delegate_BindTool = (type6.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
						}
						else
						{
							bool flag10 = parameters.Length == 2;
							if (flag10)
							{
								Type type7 = typeof(Delegate_BindTool_Ret<, , >).MakeGenericType(new Type[]
								{
									method.ReturnType,
									parameters[0].ParameterType,
									parameters[1].ParameterType
								});
								delegate_BindTool = (type7.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
							}
							else
							{
								bool flag11 = parameters.Length == 3;
								if (flag11)
								{
									Type type8 = typeof(Delegate_BindTool_Ret<, , , >).MakeGenericType(new Type[]
									{
										method.ReturnType,
										parameters[0].ParameterType,
										parameters[1].ParameterType,
										parameters[2].ParameterType
									});
									delegate_BindTool = (type8.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
								}
								else
								{
									bool flag12 = parameters.Length == 4;
									if (!flag12)
									{
										throw new Exception("还没有支持这么多参数的委托");
									}
									Type type9 = typeof(Delegate_BindTool_Ret<, , , >).MakeGenericType(new Type[]
									{
										method.ReturnType,
										parameters[0].ParameterType,
										parameters[1].ParameterType,
										parameters[2].ParameterType,
										parameters[3].ParameterType
									});
									delegate_BindTool = (type9.GetConstructor(new Type[0]).Invoke(new object[0]) as IDelegate_BindTool);
								}
							}
						}
					}
				}
				Delegate_Binder.mapBind[deletype] = delegate_BindTool;
				result = delegate_BindTool.CreateDele(deletype, null, _this_inst, __method);
			}
			return result;
		}
	}
}
