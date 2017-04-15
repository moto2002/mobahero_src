using CLRSharp;
using Com.Game.Module;
using GUIFramework;
using System;
using UnityEngine;

namespace ScriptShell
{
	public class IViewCrossBind : ICrossBind
	{
		public class Base_IMyType : IView
		{
			private CLRSharp_Instance inst;

			private IMethod _get_transform;

			private IMethod _get_gameobject;

			private IMethod _get_WinResCfg;

			private IMethod _get_uiWindow;

			private IMethod _set_uiWindow;

			private IMethod _Init;

			private IMethod _Destroy;

			private IMethod _HandleAfterOpenView;

			private IMethod _HandleBeforeCloseView;

			private IMethod _RegisterUpdateHandler;

			private IMethod _CancelUpdateHandler;

			private IMethod _RefreshUI;

			private IMethod _DataUpdated;

			private IMethod _Initialize;

			private IMethod _HandleHideEvent;

			private IMethod _HandleDestroyEvent;

			public Transform transform
			{
				get
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					object obj = this._get_transform.Invoke(activeContext, this.inst, null);
					return obj as Transform;
				}
				set
				{
				}
			}

			public GameObject gameObject
			{
				get
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					object obj = this._get_gameobject.Invoke(activeContext, this.inst, null);
					return obj as GameObject;
				}
				set
				{
				}
			}

			public WinResurceCfg WinResCfg
			{
				get
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					object obj = this._get_WinResCfg.Invoke(activeContext, this.inst, null);
					return obj as WinResurceCfg;
				}
				set
				{
				}
			}

			public TUIWindow uiWindow
			{
				get
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					object obj = this._get_uiWindow.Invoke(activeContext, this.inst, null);
					return obj as TUIWindow;
				}
				set
				{
					ThreadContext activeContext = ThreadContext.activeContext;
					this._set_uiWindow.Invoke(activeContext, this.inst, new object[]
					{
						value
					});
				}
			}

			public WindowID WinId
			{
				get;
				set;
			}

			public GameObject AnimationRoot
			{
				get;
				set;
			}

			public string WindowTitle
			{
				get;
				set;
			}

			public bool IsOpened
			{
				get;
				set;
			}

			public Base_IMyType(CLRSharp_Instance inst)
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				this.inst = inst;
				string[] methodNames = this.inst.type.GetMethodNames();
				string[] array = methodNames;
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (text.Contains("get_transform"))
					{
						this._get_transform = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("get_gameObject"))
					{
						this._get_gameobject = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("get_WinResCfg"))
					{
						this._get_WinResCfg = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("get_uiWindow"))
					{
						this._get_uiWindow = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("set_uiWindow"))
					{
						this._set_uiWindow = this.inst.type.GetMethod(text, MethodParamList.Make(new ICLRType[]
						{
							activeContext.environment.GetType(typeof(TUIWindow))
						}));
					}
					if (text.Contains("Init"))
					{
						this._Init = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("Destroy"))
					{
						this._Destroy = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("HandleAfterOpenView"))
					{
						this._HandleAfterOpenView = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("HandleBeforeCloseView"))
					{
						this._HandleBeforeCloseView = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("RegisterUpdateHandler"))
					{
						this._RegisterUpdateHandler = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("CancelUpdateHandler"))
					{
						this._CancelUpdateHandler = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("RefreshUI"))
					{
						this._RefreshUI = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("DataUpdated"))
					{
						this._DataUpdated = this.inst.type.GetMethod(text, MethodParamList.Make(new ICLRType[]
						{
							activeContext.environment.GetType(typeof(object))
						}));
					}
					if (text.Contains("Initialize"))
					{
						this._Initialize = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("HandleHideEvent"))
					{
						this._HandleHideEvent = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
					if (text.Contains("HandleDestroyEvent"))
					{
						this._HandleDestroyEvent = this.inst.type.GetMethod(text, MethodParamList.constEmpty());
					}
				}
			}

			public void Init()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._Init.Invoke(activeContext, this.inst, null);
			}

			public void Destroy()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._Destroy.Invoke(activeContext, this.inst, null);
			}

			public void HandleAfterOpenView()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._HandleAfterOpenView.Invoke(activeContext, this.inst, null);
			}

			public void HandleBeforeCloseView()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._HandleBeforeCloseView.Invoke(activeContext, this.inst, null);
			}

			public void RegisterUpdateHandler()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._RegisterUpdateHandler.Invoke(activeContext, this.inst, null);
			}

			public void CancelUpdateHandler()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._CancelUpdateHandler.Invoke(activeContext, this.inst, null);
			}

			public void RefreshUI()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._RefreshUI.Invoke(activeContext, this.inst, null);
			}

			public void OnRestart()
			{
			}

			public void DataUpdated(object data)
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._DataUpdated.Invoke(activeContext, this.inst, new object[]
				{
					data
				});
			}

			public void Initialize()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._Initialize.Invoke(activeContext, this.inst, null);
			}

			public void HandleHideEvent()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._HandleHideEvent.Invoke(activeContext, this.inst, null);
			}

			public void HandleDestroyEvent()
			{
				ThreadContext activeContext = ThreadContext.activeContext;
				object obj = this._HandleDestroyEvent.Invoke(activeContext, this.inst, null);
			}
		}

		public Type Type
		{
			get
			{
				return typeof(IView);
			}
		}

		public object CreateBind(CLRSharp_Instance inst)
		{
			return new IViewCrossBind.Base_IMyType(inst);
		}
	}
}
