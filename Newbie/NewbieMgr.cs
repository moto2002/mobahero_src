using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieMgr : MonoBehaviour
	{
		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Start()
		{
		}
	}
}
