using System;
using UnityEngine;

public interface IConfineCamera
{
	void ConfineCamera(Transform cam);

	void ChangeRange(int nIndex);
}
