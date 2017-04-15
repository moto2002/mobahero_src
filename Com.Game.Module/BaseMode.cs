using System;

namespace Com.Game.Module
{
	public class BaseMode<T> : Singleton<T> where T : new()
	{
		private DataUpateHandler handlList;

		private DataUpateHandlerWithParam handlListWithParam;

		public RefreshDelegate updateView;

		public event DataUpateHandler dataUpdated
		{
			add
			{
				this.handlList = (DataUpateHandler)Delegate.Remove(this.handlList, value);
				this.handlList = (DataUpateHandler)Delegate.Combine(this.handlList, value);
			}
			remove
			{
				this.handlList = (DataUpateHandler)Delegate.Remove(this.handlList, value);
			}
		}

		public event DataUpateHandlerWithParam dataUpdatedWithParam
		{
			add
			{
				this.handlListWithParam = (DataUpateHandlerWithParam)Delegate.Remove(this.handlListWithParam, value);
				this.handlListWithParam = (DataUpateHandlerWithParam)Delegate.Combine(this.handlListWithParam, value);
			}
			remove
			{
				this.handlListWithParam = (DataUpateHandlerWithParam)Delegate.Remove(this.handlListWithParam, value);
			}
		}

		public virtual bool ShowTips
		{
			get
			{
				return false;
			}
		}

		public void DataUpdate(int code = 0)
		{
			DataUpateHandler dataUpateHandler = this.handlList;
			if (dataUpateHandler != null)
			{
				Delegate[] invocationList = dataUpateHandler.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					DataUpateHandler dataUpateHandler2 = (DataUpateHandler)invocationList[i];
					dataUpateHandler2(this, code);
				}
			}
		}

		public void DataUpdateWithParam(int code = 0, object param = null)
		{
			DataUpateHandlerWithParam dataUpateHandlerWithParam = this.handlListWithParam;
			if (dataUpateHandlerWithParam != null)
			{
				Delegate[] invocationList = dataUpateHandlerWithParam.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					DataUpateHandlerWithParam dataUpateHandlerWithParam2 = (DataUpateHandlerWithParam)invocationList[i];
					dataUpateHandlerWithParam2(this, code, param);
				}
			}
		}
	}
}
