using System;

namespace CsSdk
{
	public interface ICsLocalRecListener
	{
		void onFailure(string msg);

		void onRecordStart();

		void onRecordFinish();
	}
}
