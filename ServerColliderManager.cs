using MobaProtocol.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerColliderManager : MonoBehaviour
{
	private Dictionary<string, ServerCollider> colliders;

	private static ServerColliderManager _instance;

	public static ServerColliderManager Instance
	{
		get
		{
			if (ServerColliderManager._instance == null)
			{
				ServerColliderManager._instance = new ServerColliderManager();
			}
			return ServerColliderManager._instance;
		}
	}

	private ServerColliderManager()
	{
		this.colliders = new Dictionary<string, ServerCollider>();
	}

	public void DebugCollider(ColliderInfo item)
	{
		if (item == null)
		{
			return;
		}
		switch (item.code)
		{
		case 1:
			this.CreateCollider(item);
			break;
		case 2:
			this.MoveCollider(item);
			break;
		case 3:
			this.DestroyCollider(item);
			break;
		}
	}

	public void OnEnable()
	{
	}

	private void DestroyCollider(ColliderInfo item)
	{
		this.colliders.Remove(item.name);
	}

	private void MoveCollider(ColliderInfo item)
	{
		ServerCollider serverCollider;
		if (this.colliders.TryGetValue(item.name, out serverCollider))
		{
			serverCollider.position.x = item.x;
			serverCollider.position.z = item.z;
		}
		else
		{
			this.CreateCollider(item);
		}
	}

	private void CreateCollider(ColliderInfo item)
	{
		if (this.colliders.ContainsKey(item.name))
		{
			return;
		}
		ServerCollider serverCollider = new ServerCollider();
		serverCollider.name = item.name;
		serverCollider.position = new Vector3(item.x, 2f, item.z);
		serverCollider.radius = item.radius;
		serverCollider.width = item.width;
		serverCollider.lenght = item.lenght;
		serverCollider.rotation = item.rotation;
		serverCollider.type = item.type;
		this.colliders.Add(item.name, serverCollider);
	}

	public virtual void OnDrawGizmos()
	{
		foreach (KeyValuePair<string, ServerCollider> current in this.colliders)
		{
			if (current.Value.type == 2)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere(current.Value.position, current.Value.radius);
			}
			if (current.Value.type == 1)
			{
				Gizmos.color = Color.cyan;
				Vector3 position = current.Value.position;
				float num = current.Value.lenght / 2f;
				float num2 = current.Value.width / 2f;
				Vector3[] array = new Vector3[]
				{
					new Vector3(position.x - num, 5f, position.z - num2),
					new Vector3(position.x + num, 5f, position.z + num2),
					new Vector3(position.x - num, 5f, position.z + num2),
					new Vector3(position.x + num, 5f, position.z - num2)
				};
				float num3 = current.Value.rotation / 180f * 3.14159274f;
				num3 = 0f - num3;
				for (int i = 0; i < array.Length; i++)
				{
					Vector3 vector = array[i] - position;
					float num4 = vector.x * Mathf.Cos(num3) - vector.z * Mathf.Sin(num3);
					float num5 = vector.x * Mathf.Sin(num3) + vector.z * Mathf.Cos(num3);
					array[i].x = position.x + num4;
					array[i].z = position.z + num5;
				}
				for (int j = 0; j < array.Length; j++)
				{
					Gizmos.DrawLine(position, array[j]);
					for (int k = 0; k < array.Length; k++)
					{
						if (k != j)
						{
							Gizmos.DrawLine(array[k], array[j]);
						}
					}
				}
			}
		}
		foreach (BlockDebugInfo current2 in BlockDebugInfo.InfoMap.Values)
		{
			if (current2.cnt == 1)
			{
				Gizmos.color = Color.green;
			}
			else if (current2.cnt == 2)
			{
				Gizmos.color = Color.yellow;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawCube(current2.pos, BlockDebugInfo.DrawSize);
		}
	}
}
