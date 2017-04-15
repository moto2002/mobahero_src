using CLRSharp;
using Com.Game.Module;
using MobaProtocol.Data;
using Mono.Cecil.Pdb;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ScriptShell
{
	public class ScriptManager
	{
		private static ScriptManager _Instance;

		private ThreadContext mContext;

		private CLRSharp_Environment mRunEnv;

		public static ScriptManager Instance
		{
			get
			{
				if (ScriptManager._Instance == null)
				{
					ScriptManager._Instance = new ScriptManager();
				}
				return ScriptManager._Instance;
			}
		}

		public void Instialize()
		{
			this.InitEnv();
			this.LoadModule("guiscript");
		}

		private void InitEnv()
		{
			this.mRunEnv = new CLRSharp_Environment(new LSharpLogger());
		}

		private void LoadModule(string name)
		{
			string path = name + ".dll";
			string path2 = name + ".pdb";
			byte[] bytes = Resources.Load<TextAsset>(path).bytes;
			byte[] bytes2 = Resources.Load<TextAsset>(path2).bytes;
			MemoryStream dllStream = new MemoryStream(bytes);
			MemoryStream pdbStream = new MemoryStream(bytes2);
			try
			{
				this.mRunEnv.LoadModule(dllStream, pdbStream, new PdbReaderProvider());
			}
			catch (Exception var_6_59)
			{
			}
			this.mRunEnv.GetType(typeof(Dictionary<int, string>));
			this.mRunEnv.GetType(typeof(Dictionary<ShopType, UIToggle>));
			this.mRunEnv.GetType(typeof(Dictionary<int, CLRSharp_Instance>));
			this.mRunEnv.GetType(typeof(Dictionary<int, Action>));
			this.mRunEnv.GetType(typeof(Dictionary<short, Action>));
			this.mRunEnv.GetType(typeof(LinkedList<int>));
			this.mRunEnv.GetType(typeof(int[,]));
			this.mRunEnv.GetType(typeof(List<Vector3>));
			this.mRunEnv.GetType(typeof(List<ShopData>));
			this.mRunEnv.GetType(typeof(List<EquipmentInfoData>));
			this.mRunEnv.GetType(typeof(List<HeroInfoData>));
			this.mRunEnv.GetType(typeof(List<SummSkinData>));
			this.mRunEnv.GetType(typeof(List<int>[]));
			this.mRunEnv.GetType(typeof(List<List<int>>));
			this.mRunEnv.GetType(typeof(List<List<List<int>>>));
			this.mRunEnv.GetType(typeof(Vector3[]));
			this.mRunEnv.GetType(typeof(IEnumerable<int>));
			Delegate_Binder.RegBind(typeof(Action<int>), new Delegate_BindTool<int>());
			Delegate_Binder.RegBind(typeof(Action<bool, ShopData>), new Delegate_BindTool_Ret<bool, ShopData>());
			Delegate_Binder.RegBind(typeof(Action<bool, EquipmentInfoData>), new Delegate_BindTool_Ret<bool, EquipmentInfoData>());
			Delegate_Binder.RegBind(typeof(Action<bool, SummSkinData>), new Delegate_BindTool_Ret<bool, SummSkinData>());
			Delegate_Binder.RegBind(typeof(Action<int, int>), new Delegate_BindTool<int, int>());
			Delegate_Binder.RegBind(typeof(Action<int, int, int>), new Delegate_BindTool<int, int, int>());
			Delegate_Binder.RegBind(typeof(Func<int, int, int>), new Delegate_BindTool_Ret<int, int, int>());
			Delegate_Binder.RegBind(typeof(Action<int, string>), new Delegate_BindTool<int, string>());
			Delegate_Binder.RegBind(typeof(Action<string>), new Delegate_BindTool<string>());
			Delegate_Binder.RegBind(typeof(Action<bool>), new Delegate_BindTool<bool>());
			this.mRunEnv.RegCrossBind(new IViewCrossBind());
			if (ThreadContext.activeContext == null)
			{
				this.mContext = new ThreadContext(this.mRunEnv, 2);
			}
			ICLRType type = this.mRunEnv.GetType("GUIScript.ShellClass");
			IMethod method = type.GetMethod("Initialize", MethodParamList.constEmpty());
			method.Invoke(this.mContext, null, null);
		}

		public void SetShopType(ShopType type)
		{
			ICLRType type2 = this.mRunEnv.GetType("GUIScript.ShopCtrl");
			IMethod method = type2.GetMethod("SetType", MethodParamList.Make(new ICLRType[]
			{
				this.mContext.environment.GetType(typeof(ShopType))
			}));
			method.Invoke(this.mContext, null, new object[]
			{
				type
			});
		}

		public void OpenWindow(WindowID winId)
		{
			ICLRType type = this.mRunEnv.GetType("GUIScript.ShellClass");
			IMethod method = type.GetMethod("OpenWindow", MethodParamList.Make(new ICLRType[]
			{
				this.mContext.environment.GetType(typeof(WindowID))
			}));
			method.Invoke(this.mContext, null, new object[]
			{
				winId
			});
		}
	}
}
