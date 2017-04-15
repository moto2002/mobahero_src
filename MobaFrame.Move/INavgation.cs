using System;
using UnityEngine;

namespace MobaFrame.Move
{
	public interface INavgation
	{
		void InitPath();

		void SearchPath(Transform t, float stop_distance);

		void StopPath();

		void CheckStop(float delata);

		void DestroyPath();
	}
}
