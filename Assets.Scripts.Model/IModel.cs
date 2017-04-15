using System;

namespace Assets.Scripts.Model
{
	internal interface IModel
	{
		EModelType ModelType
		{
			get;
		}

		bool Valid
		{
			get;
			set;
		}

		int LastMsgType
		{
			get;
		}

		int LastMsgID
		{
			get;
		}

		int LastError
		{
			get;
		}

		string DebugMessage
		{
			get;
		}

		object Data
		{
			get;
		}

		void RegisterMsgHandler();

		void UnRegisterMsgHandler();

		void AddModelListner(MobaMessageFunc msgFunc);

		void RemoveModelListner(MobaMessageFunc msgFunc);
	}
}
