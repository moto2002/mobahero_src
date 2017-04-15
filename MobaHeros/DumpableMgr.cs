using System;
using System.Collections.Generic;

namespace MobaHeros
{
	public class DumpableMgr
	{
		private static readonly List<IDumpable> _list = new List<IDumpable>();

		public static void Register(IDumpable dumper)
		{
			if (dumper != null)
			{
				DumpableMgr._list.Add(dumper);
			}
		}

		public static void Unregister(IDumpable dumper)
		{
			DumpableMgr._list.RemoveAll((IDumpable x) => x == dumper);
		}

		public static void DumpAll()
		{
			DumpableMgr._list.ForEach(delegate(IDumpable x)
			{
				x.Dump();
			});
		}
	}
}
