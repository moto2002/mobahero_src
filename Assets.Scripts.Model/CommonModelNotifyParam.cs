using System;

namespace Assets.Scripts.Model
{
	public class CommonModelNotifyParam : IModelNotifyParam
	{
		private int errorCode;

		private object data;

		private string debugMessage;

		private EModelType modelType;

		private int typeID;

		private int msgID;

		public int ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public string DebugMessage
		{
			get
			{
				return this.debugMessage;
			}
		}

		public object Data
		{
			get
			{
				return this.data;
			}
		}

		public int TypeID
		{
			get
			{
				return this.typeID;
			}
		}

		public int MsgID
		{
			get
			{
				return this.msgID;
			}
		}

		public EModelType ModelType
		{
			get
			{
				return this.modelType;
			}
		}

		internal CommonModelNotifyParam(IModel model)
		{
			this.errorCode = model.LastError;
			this.data = model.Data;
			this.modelType = model.ModelType;
			this.debugMessage = model.DebugMessage;
			this.typeID = model.LastMsgType;
			this.msgID = model.LastMsgID;
		}

		public T GetData<T>() where T : class
		{
			return this.data as T;
		}
	}
}
