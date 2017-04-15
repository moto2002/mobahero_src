using MobaProtocol.Data;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	public static class ModelTools_mailList
	{
		public static List<MailData> Get_mailDataList_X(this ModelManager mmng)
		{
			return mmng.GetMailDataList();
		}

		private static List<MailData> GetMailDataList(this ModelManager mmng)
		{
			List<MailData> result = null;
			if (mmng != null && mmng.ValidData(EModelType.Model_mailList))
			{
				MailModelData data = mmng.GetData<MailModelData>(EModelType.Model_mailList);
				if (data != null)
				{
					result = data.mailList;
				}
			}
			return result;
		}

		public static long Get_modifyMailId_X(this ModelManager mmng)
		{
			return mmng.GetModifyMailId();
		}

		private static long GetModifyMailId(this ModelManager mmng)
		{
			long result = 0L;
			if (mmng != null && mmng.ValidData(EModelType.Model_mailList))
			{
				MailModelData data = mmng.GetData<MailModelData>(EModelType.Model_mailList);
				if (data != null)
				{
					result = data.modifyMailId;
				}
			}
			return result;
		}
	}
}
