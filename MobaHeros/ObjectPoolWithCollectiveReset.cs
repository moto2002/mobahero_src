using System;
using System.Collections.Generic;

namespace MobaHeros
{
	public class ObjectPoolWithCollectiveReset<T> where T : class, new()
	{
		private List<T> m_objectList;

		private int m_nextAvailableIndex;

		private Action<T> m_resetAction;

		private Action<T> m_onetimeInitAction;

		public ObjectPoolWithCollectiveReset(int initialBufferSize, Action<T> resetAction = null, Action<T> onetimeInitAction = null)
		{
			this.m_objectList = new List<T>(initialBufferSize);
			this.m_resetAction = resetAction;
			this.m_onetimeInitAction = onetimeInitAction;
		}

		public T Get()
		{
			if (this.m_nextAvailableIndex < this.m_objectList.Count)
			{
				T t = this.m_objectList[this.m_nextAvailableIndex];
				this.m_nextAvailableIndex++;
				if (this.m_resetAction != null)
				{
					this.m_resetAction(t);
				}
				return t;
			}
			T t2 = Activator.CreateInstance<T>();
			this.m_objectList.Add(t2);
			this.m_nextAvailableIndex++;
			if (this.m_onetimeInitAction != null)
			{
				this.m_onetimeInitAction(t2);
			}
			return t2;
		}

		public void ReleaseAll()
		{
			this.m_nextAvailableIndex = 0;
		}
	}
}
