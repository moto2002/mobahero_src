using System;

namespace CsSdk
{
	public interface ICsChatListener
	{
		void onNewMsg(string newMsgJson);

		void onPerNum(int count);
	}
}
