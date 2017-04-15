using System;
using System.Collections.Generic;

namespace MobaClient
{
	public class UdpPackage
	{
		public byte code;

		public int channelID;

		public long seqNo;

		public long sentTime;

		public object msg;

		public bool bAck = false;

		private static List<UdpPackage> pkgPool = new List<UdpPackage>();

		public static UdpPackage AllocPkg(byte _code, int _channelId, long _seqNo, object _msg)
		{
			UdpPackage udpPackage;
			if (UdpPackage.pkgPool.Count > 0)
			{
				udpPackage = UdpPackage.pkgPool[0];
				UdpPackage.pkgPool.RemoveAt(0);
			}
			else
			{
				udpPackage = new UdpPackage();
			}
			udpPackage.channelID = _channelId;
			udpPackage.seqNo = _seqNo;
			udpPackage.msg = _msg;
			udpPackage.bAck = false;
			return udpPackage;
		}

		public static void FreePkg(UdpPackage pkg)
		{
			UdpPackage.pkgPool.Add(pkg);
		}
	}
}
