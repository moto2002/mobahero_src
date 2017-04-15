using System;

namespace Assets.Scripts.Model
{
	public interface ITreeTraversCallback
	{
		Func<BEquip_node, int, bool> TraversCallback
		{
			get;
		}

		object Result
		{
			get;
		}
	}
}
