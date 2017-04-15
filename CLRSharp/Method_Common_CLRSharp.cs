using Mono.Cecil;
using System;

namespace CLRSharp
{
	public class Method_Common_CLRSharp : IMethod_Sharp, IMethod
	{
		private Type_Common_CLRSharp _DeclaringType;

		public MethodDefinition method_CLRSharp;

		private CodeBody _body = null;

		public string Name
		{
			get
			{
				return this.method_CLRSharp.Name;
			}
		}

		public bool isStatic
		{
			get
			{
				return this.method_CLRSharp.IsStatic;
			}
		}

		public ICLRType DeclaringType
		{
			get
			{
				return this._DeclaringType;
			}
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

		public CodeBody body
		{
			get
			{
				bool flag = this._body == null;
				CodeBody result;
				if (flag)
				{
					bool flag2 = !this.method_CLRSharp.HasBody;
					if (flag2)
					{
						result = null;
						return result;
					}
					this._body = new CodeBody(this._DeclaringType.env, this.method_CLRSharp);
				}
				result = this._body;
				return result;
			}
		}

		public Method_Common_CLRSharp(Type_Common_CLRSharp type, MethodDefinition method)
		{
			bool flag = method == null;
			if (flag)
			{
				throw new Exception("not allow null method.");
			}
			this._DeclaringType = type;
			this.method_CLRSharp = method;
			this.ReturnType = type.env.GetType(method.ReturnType.FullName);
			this.ParamList = new MethodParamList(type.env, method);
		}

		public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual)
		{
			bool flag = context == null;
			if (flag)
			{
				context = ThreadContext.activeContext;
			}
			bool flag2 = context == null;
			if (flag2)
			{
				throw new Exception("这个线程上没有CLRSharp:ThreadContext");
			}
			bool flag3 = bVisual && this.method_CLRSharp.IsVirtual;
			object result;
			if (flag3)
			{
				CLRSharp_Instance cLRSharp_Instance = _this as CLRSharp_Instance;
				bool flag4 = cLRSharp_Instance.type != this.DeclaringType;
				if (flag4)
				{
					IMethod vMethod = cLRSharp_Instance.type.GetVMethod(this);
					bool flag5 = vMethod != this;
					if (flag5)
					{
						result = vMethod.Invoke(context, _this, _params);
						return result;
					}
				}
			}
			bool flag6 = this.method_CLRSharp.Name == ".ctor";
			if (flag6)
			{
				CLRSharp_Instance cLRSharp_Instance2 = _this as CLRSharp_Instance;
				bool flag7 = cLRSharp_Instance2 == null;
				if (flag7)
				{
					cLRSharp_Instance2 = new CLRSharp_Instance(this._DeclaringType);
				}
				context.ExecuteFunc(this, cLRSharp_Instance2, _params);
				result = cLRSharp_Instance2;
			}
			else
			{
				object obj = context.ExecuteFunc(this, _this, _params);
				bool flag8 = obj is CLRSharp_Instance && this.ReturnType is ICLRType_System;
				if (flag8)
				{
					ICrossBind crossBind = context.environment.GetCrossBind((this.ReturnType as ICLRType_System).TypeForSystem);
					bool flag9 = crossBind != null;
					if (flag9)
					{
						obj = crossBind.CreateBind(obj as CLRSharp_Instance);
					}
				}
				result = obj;
			}
			return result;
		}

		public object Invoke(ThreadContext context, object _this, object[] _params)
		{
			return this.Invoke(context, _this, _params, true);
		}

		public object Invoke(ThreadContext context, object _this, object[] _params, bool bVisual, bool autoLogDump)
		{
			object result;
			try
			{
				result = this.Invoke(context, _this, _params, bVisual);
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
				context.environment.logger.Log_Error(context.Dump());
				throw ex;
			}
			return result;
		}
	}
}
