using ExitGames.Client.Photon;
using MobaProtocol;
using System;

namespace Assets.Scripts.Model
{
	internal abstract class ModelBase<T> : IModel
	{
		private EModelType modelType;

		private bool bValid;

		private int lastError;

		private string debugMessage;

		private int msgType;

		private int msgID;

		private T data;

		public EModelType ModelType
		{
			get
			{
				return this.modelType;
			}
		}

		public bool Valid
		{
			get
			{
				return this.bValid;
			}
			set
			{
				this.bValid = value;
			}
		}

		public object Data
		{
			get
			{
				return this.data;
			}
			protected set
			{
				this.data = (T)((object)value);
			}
		}

		public int LastMsgType
		{
			get
			{
				return this.msgType;
			}
			protected set
			{
				this.msgType = value;
			}
		}

		public int LastMsgID
		{
			get
			{
				return this.msgID;
			}
			protected set
			{
				this.msgID = value;
			}
		}

		public int LastError
		{
			get
			{
				return this.lastError;
			}
			protected set
			{
				this.lastError = value;
			}
		}

		public string DebugMessage
		{
			get
			{
				return this.debugMessage;
			}
			protected set
			{
				this.debugMessage = value;
			}
		}

		protected ModelBase()
		{
			this.bValid = true;
			this.lastError = 0;
		}

		protected void Init(EModelType mt)
		{
			this.modelType = mt;
		}

		public abstract void RegisterMsgHandler();

		public abstract void UnRegisterMsgHandler();

		public virtual void AddModelListner(MobaMessageFunc msgFunc)
		{
			if (msgFunc != null)
			{
				MobaMessageManager.RegistMessage((ClientMsg)this.modelType, msgFunc);
			}
		}

		public virtual void RemoveModelListner(MobaMessageFunc msgFunc)
		{
			if (msgFunc != null)
			{
				MobaMessageManager.UnRegistMessage((ClientMsg)this.modelType, msgFunc);
			}
		}

		protected abstract void OnGetMsg(MobaMessage msg);

		protected void TriggerListners()
		{
			CommonModelNotifyParam notifyData = this.GetNotifyData();
			MobaMessage message = MobaMessageManager.GetMessage((ClientMsg)this.modelType, notifyData, 0f);
			MobaMessageManager.ExecuteMsg(message);
		}

		protected void Deserialize(byte key, MobaMessage msg)
		{
			this.lastError = 505;
			this.data = default(T);
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null)
				{
					this.lastError = (int)operationResponse.Parameters[1];
					if (this.lastError == 0)
					{
						byte[] buffer = operationResponse.Parameters[key] as byte[];
						this.data = SerializeHelper.Deserialize<T>(buffer);
					}
				}
			}
			this.bValid = (this.lastError == 0 && null != this.data);
		}
	}
}
