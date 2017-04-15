using System;
using UnityEngine;

public class CameraParams : MonoBehaviour
{
	public bool alwaysFocusRole;

	public bool isFree;

	public float cameraFov = 30f;

	public Vector2 rangeX = new Vector2(-3.40282347E+38f, 3.40282347E+38f);

	public Vector2 rangeZ = new Vector2(-3.40282347E+38f, 3.40282347E+38f);
}
