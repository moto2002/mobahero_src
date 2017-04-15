using System;
using System.Collections.Generic;
using UnityEngine;

public class NsRenderManager : MonoBehaviour
{
	public List<Component> m_RenderEventCalls;

	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
	}

	private void OnPreRender()
	{
		if (this.m_RenderEventCalls != null)
		{
			int num = this.m_RenderEventCalls.Count - 1;
			while (0 <= num)
			{
				if (this.m_RenderEventCalls[num] == null)
				{
					this.m_RenderEventCalls.RemoveAt(num);
				}
				else
				{
					this.m_RenderEventCalls[num].SendMessage("OnPreRender", SendMessageOptions.DontRequireReceiver);
				}
				num--;
			}
		}
	}

	private void OnRenderObject()
	{
	}

	private void OnPostRender()
	{
		if (this.m_RenderEventCalls != null)
		{
			foreach (Component current in this.m_RenderEventCalls)
			{
				if (current != null)
				{
					current.SendMessage("OnPostRender", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	public void AddRenderEventCall(Component tarCom)
	{
		if (this.m_RenderEventCalls == null)
		{
			this.m_RenderEventCalls = new List<Component>();
		}
		if (!this.m_RenderEventCalls.Contains(tarCom))
		{
			this.m_RenderEventCalls.Add(tarCom);
		}
	}

	public void RemoveRenderEventCall(Component tarCom)
	{
		if (this.m_RenderEventCalls == null)
		{
			this.m_RenderEventCalls = new List<Component>();
		}
		if (this.m_RenderEventCalls.Contains(tarCom))
		{
			this.m_RenderEventCalls.Remove(tarCom);
		}
	}
}
