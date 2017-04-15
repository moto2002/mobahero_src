using System;

namespace CsSdkIOS
{
	public interface ICsHttpListener
	{
		void onFailure(string message);

		void onSuccess(string jsonObject);
	}
}
