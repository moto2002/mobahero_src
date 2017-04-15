using System;

namespace Assets.Scripts.Model
{
	internal static class ModelTools
	{
		internal static CommonModelNotifyParam GetNotifyData(this IModel model)
		{
			CommonModelNotifyParam result = null;
			if (model != null)
			{
				result = new CommonModelNotifyParam(model);
			}
			return result;
		}
	}
}
