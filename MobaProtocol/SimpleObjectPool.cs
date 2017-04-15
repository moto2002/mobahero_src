using System;
using System.Collections.Generic;

namespace MobaProtocol
{
	public class SimpleObjectPool<T> where T : class, new()
	{
		private Stack<T> _objectStack;

		private Action<T> _resetAction;

		private Action<T> _onetimeInitAction;

		private int _getCount = 0;

		private int _releaseCount = 0;

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
			T result;
			if (this._objectStack.Count > 0)
			{
				T t = this._objectStack.Pop();
				if (this._resetAction != null)
				{
					this._resetAction(t);
				}
				result = t;
			}
			else
			{
				T t = Activator.CreateInstance<T>();
				if (this._onetimeInitAction != null)
				{
					this._onetimeInitAction(t);
				}
				result = t;
			}
			return result;
		}

		public void Release(T obj)
		{
			this._objectStack.Push(obj);
		}

		public void Dump()
		{
			Console.WriteLine(string.Format("=====>Stats: {0} {1}/{2}", this._name, this._getCount, this._releaseCount));
		}
	}
}
