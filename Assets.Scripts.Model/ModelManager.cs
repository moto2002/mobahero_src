using Assets.Scripts.Server;
using System;
using System.Collections.Generic;
using System.Reflection;
using Utils;

namespace Assets.Scripts.Model
{
	public class ModelManager : IGlobalComServer
	{
		private Dictionary<EModelType, IModel> mDicModel;

		private static ModelManager mInstance;

		public static ModelManager Instance
		{
			get
			{
				return ModelManager.mInstance;
			}
		}

		public void OnAwake()
		{
			ModelManager.mInstance = this;
			this.mDicModel = new Dictionary<EModelType, IModel>(default(EnumEqualityComparer<EModelType>));
			EModelType[] array = (EModelType[])Enum.GetValues(typeof(EModelType));
			EModelType[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				EModelType eModelType = array2[i];
				if (eModelType > EModelType.Model_null && eModelType < EModelType.Model_max)
				{
					this.AddModel(eModelType);
				}
			}
		}

		public void OnStart()
		{
		}

		public void OnUpdate()
		{
		}

		public void OnDestroy()
		{
			this.mDicModel.Clear();
		}

		public void Enable(bool b)
		{
		}

		public void OnRestart()
		{
		}

		public void OnApplicationQuit()
		{
		}

		public void OnApplicationFocus(bool isFocus)
		{
		}

		public void OnApplicationPause(bool isPause)
		{
		}

		private void AddModel(IModel model)
		{
			if (model != null)
			{
				this.mDicModel.Add(model.ModelType, model);
				model.RegisterMsgHandler();
			}
		}

		private void AddModel(EModelType e)
		{
			if (!this.mDicModel.ContainsKey(e))
			{
				IModel model = this.CreateModel(e);
				this.AddModel(model);
			}
		}

		private void RemoveModel(EModelType e)
		{
			if (this.mDicModel.ContainsKey(e))
			{
				this.mDicModel[e].UnRegisterMsgHandler();
				this.mDicModel.Remove(e);
			}
		}

		private IModel CreateModel(EModelType e)
		{
			IModel result = null;
			Type type = Type.GetType("Assets.Scripts.Model." + e.ToString());
			if (type != null)
			{
				object obj = Activator.CreateInstance(type);
				result = (obj as IModel);
			}
			return result;
		}

		public T GetData<T>(EModelType e)
		{
			T result = default(T);
			if (this.mDicModel.ContainsKey(e))
			{
				result = (T)((object)this.mDicModel[e].Data);
			}
			return result;
		}

		public bool ValidData(EModelType e)
		{
			return this.mDicModel.ContainsKey(e) && this.mDicModel[e].Valid;
		}

		public void SetDataInvalid(EModelType e)
		{
			if (this.mDicModel.ContainsKey(e))
			{
				this.mDicModel[e].Valid = false;
			}
		}

		public void AddListener(EModelType e, MobaMessageFunc msgFunc)
		{
			if (this.mDicModel.ContainsKey(e))
			{
				this.mDicModel[e].AddModelListner(msgFunc);
			}
		}

		public void RemoveListener(EModelType e, MobaMessageFunc msgFunc)
		{
			if (this.mDicModel.ContainsKey(e))
			{
				this.mDicModel[e].RemoveModelListner(msgFunc);
			}
		}

		public void InvokeModelMem(EModelType e, string methodName, object[] param)
		{
			if (string.IsNullOrEmpty(methodName) || !this.mDicModel.ContainsKey(e))
			{
				return;
			}
			Type type = this.mDicModel[e].GetType();
			if (type == null)
			{
				return;
			}
			MethodInfo method = type.GetMethod(methodName);
			if (method == null)
			{
				return;
			}
			method.Invoke(this.mDicModel[e], param);
		}
	}
}
