using System;

namespace Com.Game.Module
{
	public enum PriceTagDataCode
	{
		ErrorCode,
		Coin,
		Diamond,
		Cap = 9,
		RMB,
		IsCoinDiscount,
		IsDiamondDiscount,
		IsCapDiscount = 19,
		IsRMBDiscount,
		IsCoinCoupon,
		IsDiamondCoupon,
		IsCapCoupon = 29,
		IsRMBCoupon,
		PaymentWays = 100
	}
}
