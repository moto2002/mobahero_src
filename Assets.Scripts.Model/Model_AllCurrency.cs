using ExitGames.Client.Photon;
using MobaProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
	internal class Model_AllCurrency : ModelBase<CurrencyData>
	{
		private List<MobaChatCode> listCode = new List<MobaChatCode>();

		private object[] mgs;

		private CurrencyData data;

		public Model_AllCurrency()
		{
			base.Init(EModelType.Model_AllCurrency);
			this.data = new CurrencyData();
			base.Data = this.data;
		}

		public override void RegisterMsgHandler()
		{
			this.mgs = new object[]
			{
				MobaGameCode.GetCurrencyCount
			};
			MobaMessageManagerTools.RegistMsg(this, this.mgs, true, "OnMsg_");
		}

		public override void UnRegisterMsgHandler()
		{
			MobaMessageManagerTools.RegistMsg(this, this.mgs, false, "OnMsg_");
		}

		protected override void OnGetMsg(MobaMessage msg)
		{
			throw new NotImplementedException();
		}

		private void OnMsg_GetCurrencyCount(MobaMessage msg)
		{
			if (msg != null)
			{
				OperationResponse operationResponse = msg.Param as OperationResponse;
				if (operationResponse != null && (int)operationResponse.Parameters[1] == 0)
				{
					int num = (int)operationResponse.Parameters[10];
					int num2 = num;
					switch (num2)
					{
					case 0:
						if (operationResponse.Parameters.ContainsKey(62))
						{
							long diamond = (long)operationResponse.Parameters[62];
							this.data.Diamond = diamond;
						}
						if (operationResponse.Parameters.ContainsKey(61))
						{
							long gold = (long)operationResponse.Parameters[61];
							this.data.Gold = gold;
						}
						if (operationResponse.Parameters.ContainsKey(101))
						{
							int cap = (int)operationResponse.Parameters[101];
							this.data.Cap = cap;
						}
						if (operationResponse.Parameters.ContainsKey(11))
						{
							int trumpet = (int)operationResponse.Parameters[11];
							this.data.Trumpet = trumpet;
						}
						break;
					case 1:
					{
						long gold2 = (long)operationResponse.Parameters[101];
						this.data.Gold = gold2;
						break;
					}
					case 2:
					{
						long diamond2 = (long)operationResponse.Parameters[101];
						this.data.Diamond = diamond2;
						break;
					}
					default:
						switch (num2)
						{
						case 9:
						{
							int cap2 = (int)operationResponse.Parameters[101];
							this.data.Cap = cap2;
							break;
						}
						case 11:
						{
							int trumpet2 = (int)operationResponse.Parameters[101];
							this.data.Trumpet = trumpet2;
							break;
						}
						}
						break;
					}
					base.Data = this.data;
				}
			}
		}
	}
}
