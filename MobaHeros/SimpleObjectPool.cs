using Com.Game.Utils;
using System;
using System.Collections.Generic;

namespace MobaHeros
{
	public class SimpleObjectPool<T> : DumpBase where T : class, new()
	{
		private Stack<T> _objectStack;

		private Action<T> _resetAction;

		private Action<T> _onetimeInitAction;

		private int _popCount;

		private int _newCount;

		private int _releaseCount;

		private string _name;

		public SimpleObjectPool(int initialBufferSize, Action<T> resetAction = null, Action<T> onetimeInitAction = null, string name = "")
		{
			this._objectStack = new Stack<T>(initialBufferSize);
			this._resetAction = resetAction;
			this._onetimeInitAction = onetimeInitAction;
			this._name = name;
		}

		public T Get()
		{
			if (this._objectStack.Count > 0)
			{
				this._popCount++;
				T t = this._objectStack.Pop();
				if (this._resetAction != null)
				{
					this._resetAction(t);
				}
				return t;
			}
			this._newCount++;
			T t2 = Activator.CreateInstance<T>();
			if (this._onetimeInitAction != null)
			{
				this._onetimeInitAction(t2);
			}
			return t2;
		}

		public void Release(T obj)
		{
			this._releaseCount++;
			this._objectStack.Push(obj);
		}

		public override void Dump()
		{
			ClientLogger.Info(string.Format("=====>Stats: {0} {1}, {2}, release {3}", new object[]
			{
				this._name,
				this._popCount,
				this._newCount,
				this._releaseCount
			}));
		}
	}
}
