using System;

[Serializable]
public class Moba_Camera_Settings_Zoom
{
	public bool invertZoom;

	public float defaultZoom = 15f;

	public float minZoom = 10f;

	public float maxZoom = 20f;

	public float zoomRate = 10f;

	public bool constZoomRate;
}
