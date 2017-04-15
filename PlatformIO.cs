using System;
using UnityEngine;

public class PlatformIO : MonoBehaviour
{
	public static PlatformManager platformManager;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static IOType InfoToType(string str)
	{
		return IOType.PayfirstStrTwios;
	}

	public static string InfoToResult(IOType ioType, float price, int paymentPrice)
	{
		return string.Empty;
	}
}
