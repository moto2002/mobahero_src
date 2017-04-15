using System;

namespace CLRSharp
{
	public class RefFunc
	{
		public IMethod _method;

		public object _this;

		public RefFunc(IMethod _method, object _this)
		{
			this._method = _method;
			this._this = _this;
		}
	}
}
