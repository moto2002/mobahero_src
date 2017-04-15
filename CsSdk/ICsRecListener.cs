using System;

namespace CsSdk
{
	public interface ICsRecListener
	{
		void onSuccess();

		void onFailure(string msg);

		void offline();
	}
}
