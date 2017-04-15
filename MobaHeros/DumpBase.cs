using System;

namespace MobaHeros
{
	public abstract class DumpBase : IDisposable, IDumpable
	{
		protected DumpBase()
		{
			DumpableMgr.Register(this);
		}

		public void Dispose()
		{
			DumpableMgr.Unregister(this);
		}

		public abstract void Dump();
	}
}
