using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public class ServerNoticeModelData
	{
		public Queue<NotificationData> gmMsgQueue = new Queue<NotificationData>();

		public Queue<string> otherMsgQueue = new Queue<string>();
	}
}
