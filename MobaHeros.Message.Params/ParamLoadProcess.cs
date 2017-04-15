using System;

namespace MobaHeros.Message.Params
{
	public class ParamLoadProcess
	{
		public readonly int userId;

		public readonly byte process;

		public ParamLoadProcess(int userId, byte process)
		{
			this.userId = userId;
			this.process = process;
		}
	}
}
