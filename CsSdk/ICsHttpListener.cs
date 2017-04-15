using System;

namespace CsSdk
{
	public interface ICsHttpListener
	{
		void onStart();

		void onFailure(int code, string message);

		void onSuccess(string jsonObject);
	}
}
