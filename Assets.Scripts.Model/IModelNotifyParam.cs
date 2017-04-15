using System;

namespace Assets.Scripts.Model
{
	public interface IModelNotifyParam
	{
		int ErrorCode
		{
			get;
		}

		object Data
		{
			get;
		}

		EModelType ModelType
		{
			get;
		}
	}
}
