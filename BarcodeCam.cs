using System;
using UnityEngine;
using ZXing;
using ZXing.QrCode;

public class BarcodeCam : MonoBehaviour
{
	public Texture2D encoded;

	public string Lastresult;

	private void Start()
	{
		this.encoded = new Texture2D(256, 256);
		this.encoded.name = "BarcodeCam";
		this.Lastresult = "http://www.baidu.com";
	}

	private static Color32[] Encode(string textForEncoding, int width, int height)
	{
		BarcodeWriter barcodeWriter = new BarcodeWriter
		{
			Format = BarcodeFormat.QR_CODE,
			Options = new QrCodeEncodingOptions
			{
				Height = height,
				Width = width
			}
		};
		return barcodeWriter.Write(textForEncoding);
	}

	private void Update()
	{
		string lastresult = this.Lastresult;
		if (lastresult != null)
		{
			Color32[] pixels = BarcodeCam.Encode(lastresult, this.encoded.width, this.encoded.height);
			this.encoded.SetPixels32(pixels);
			this.encoded.Apply();
		}
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(100f, 100f, 256f, 256f), this.encoded);
	}
}
