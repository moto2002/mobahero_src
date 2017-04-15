using System;
using UnityEngine;

namespace MobaHeros
{
	public static class ResourceHandleExtension
	{
		public static bool IsValid<T>(this ResourceHandleWrapper<T> wrapper) where T : Component
		{
			return wrapper != null && wrapper.Component;
		}
	}
}
