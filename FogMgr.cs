using System;
using System.Collections.Generic;
using UnityEngine;

public class FogMgr
{
	private class IntVector2
	{
		public int x;

		public int y;

		public IntVector2(int _x, int _y)
		{
			this.x = _x;
			this.y = _y;
		}

		public int sqrMagnitude()
		{
			return this.x * this.x + this.y * this.y;
		}
	}

	public const int FOG_MAXSIZE = 256;

	private static FogMgr m_Instance;

	private byte[,] m_MyFogTable;

	private byte[,] m_MyRealSight;

	private Vector3 m_BasePos;

	private List<FogItem> m_FogItems;

	private float m_fUpdateTick;

	private bool _enablefog = true;

	private Dictionary<int, FogMgr.IntVector2[]> mask = new Dictionary<int, FogMgr.IntVector2[]>();

	public static FogMgr Instance
	{
		get
		{
			if (FogMgr.m_Instance == null)
			{
				FogMgr.m_Instance = new FogMgr();
			}
			return FogMgr.m_Instance;
		}
	}

	public void Init()
	{
		this.m_MyFogTable = new byte[256, 256];
		this.m_MyRealSight = new byte[256, 256];
		this.m_BasePos = new Vector3(-128f, 0f, -128f);
		this.m_FogItems = new List<FogItem>();
	}

	public void Update()
	{
		this.m_fUpdateTick += Time.deltaTime;
		if (this.m_fUpdateTick < 0.5f)
		{
			return;
		}
		this.m_fUpdateTick = 0f;
		if (GlobalSettings.FogMode == 1)
		{
			this.UpdateFog();
		}
		this.UpdateRealSight();
	}

	private void UpdateRealSight()
	{
		this.ResetRealSight();
		if (MapManager.Instance != null)
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			int teamType = player.teamType;
			Dictionary<int, Units> allMapUnits = MapManager.Instance.GetAllMapUnits();
			Dictionary<int, Units>.Enumerator enumerator = allMapUnits.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, Units> current = enumerator.Current;
				Units value = current.Value;
				if (value != null && value.teamType == teamType && value.isLive && value.realSight_range > 0f)
				{
					this.AddRealSightInfo(value.mTransform.position, value.realSight_range);
				}
			}
		}
	}

	private void UpdateFog()
	{
		this.ResetFogTable();
		this.RefreshFogItem();
		if (MapManager.Instance != null)
		{
			Units player = PlayerControlMgr.Instance.GetPlayer();
			Dictionary<int, List<Units>> tagUnits = MapManager.Instance.GetTagUnits((TeamType)player.teamType);
			if (tagUnits == null)
			{
				return;
			}
			foreach (int current in tagUnits.Keys)
			{
				List<Units> list = tagUnits[current];
				if (list != null)
				{
					foreach (Units current2 in list)
					{
						if (current2.fog_range > 0f)
						{
							this.AddFogInfo(current2);
						}
					}
				}
			}
		}
	}

	public void enableFog(bool enb)
	{
		this._enablefog = enb;
	}

	public bool IsInRealSight(Units units)
	{
		if (units == null)
		{
			Debug.LogError(" units is null!");
			return true;
		}
		if (units.mTransform == null)
		{
			Debug.LogError(" units.transform is null!");
			return true;
		}
		Vector3 position = units.mTransform.position;
		Vector2 vector = new Vector2(position.x - this.m_BasePos.x, position.z - this.m_BasePos.z);
		if (vector.x >= 256f || vector.x < 0f || vector.y >= 256f || vector.y < 0f)
		{
			Debug.LogError(vector + " pos is error!");
			return true;
		}
		return this.m_MyRealSight[(int)vector.x, (int)vector.y] != 0;
	}

	public bool IsInFog(Units units)
	{
		if (!this._enablefog)
		{
			return false;
		}
		if (units == null)
		{
			Debug.LogError(" units is null!");
			return true;
		}
		if (units.transform == null)
		{
			Debug.LogError(" units.transform is null!");
			return true;
		}
		Vector3 position = units.transform.position;
		Vector2 vector = new Vector2(position.x - this.m_BasePos.x, position.z - this.m_BasePos.z);
		if (vector.x >= 256f || vector.x < 0f || vector.y >= 256f || vector.y < 0f)
		{
			Debug.LogError(vector + " pos is error!");
			return true;
		}
		return this.m_MyFogTable[(int)vector.x, (int)vector.y] == 0;
	}

	public void AddFogItem(Units units, float fRadius, float fTimes)
	{
		if (units == null || fRadius < 0f || fTimes <= 0f)
		{
			return;
		}
		if (this.m_FogItems == null)
		{
			this.m_FogItems = new List<FogItem>();
		}
		foreach (FogItem current in this.m_FogItems)
		{
			if (current.m_units == units && fRadius - current.m_fRadius < 0.1f && fRadius - current.m_fRadius > -0.1f && fTimes > current.m_fTimes)
			{
				current.m_fTimes = fTimes;
				return;
			}
		}
		FogItem fogItem = new FogItem();
		fogItem.m_units = units;
		fogItem.m_Pos = units.transform.position;
		fogItem.m_fRadius = fRadius;
		fogItem.m_fTimes = fTimes;
		this.m_FogItems.Add(fogItem);
	}

	public void AddFogItem(Vector3 vPos, float fRadius, float fTimes)
	{
		if (false || fRadius < 0f || fTimes <= 0f)
		{
			return;
		}
		if (this.m_FogItems == null)
		{
			this.m_FogItems = new List<FogItem>();
		}
		FogItem fogItem = new FogItem();
		fogItem.m_units = null;
		fogItem.m_Pos = vPos;
		fogItem.m_fRadius = fRadius;
		fogItem.m_fTimes = fTimes;
		this.m_FogItems.Add(fogItem);
	}

	private void RefreshFogItem()
	{
		if (this.m_FogItems == null || this.m_FogItems.Count == 0)
		{
			return;
		}
		for (int i = this.m_FogItems.Count - 1; i >= 0; i--)
		{
			this.m_FogItems[i].m_fTimes -= Time.deltaTime;
			if (this.m_FogItems[i].m_fTimes < 0f)
			{
				this.m_FogItems.RemoveAt(i);
			}
			else if (this.m_FogItems[i].m_units != null)
			{
				this.AddFogInfo(this.m_FogItems[i].m_units.transform.position, this.m_FogItems[i].m_fRadius);
			}
			else
			{
				this.AddFogInfo(this.m_FogItems[i].m_Pos, this.m_FogItems[i].m_fRadius);
			}
		}
	}

	private void AddFogInfo(Vector3 vPos, float fRadius)
	{
		if (false || fRadius <= 0f)
		{
			return;
		}
		Vector2 vector = new Vector2(vPos.x - this.m_BasePos.x, vPos.z - this.m_BasePos.z);
		int radius = (int)fRadius;
		if (vector.x < 0f || vector.x >= 256f || vector.y < 0f || vector.y >= 256f)
		{
			Debug.LogError("unitsPos error!");
			return;
		}
		FogMgr.IntVector2[] vector2Mask = this.GetVector2Mask(radius);
		int num = vector2Mask.Length;
		for (int i = 0; i < num; i++)
		{
			FogMgr.IntVector2 intVector = vector2Mask[i];
			int num2 = (int)(vector.x + (float)intVector.x);
			if (num2 >= 0 && num2 < 256)
			{
				int num3 = (int)(vector.y + (float)intVector.y);
				if (num3 >= 0 && num3 < 256)
				{
					this.m_MyFogTable[num2, num3] = 1;
				}
			}
		}
	}

	private FogMgr.IntVector2[] GetVector2Mask(int radius)
	{
		FogMgr.IntVector2[] result;
		if (this.mask.TryGetValue(radius, out result))
		{
			return result;
		}
		int num = radius * radius;
		List<FogMgr.IntVector2> list = new List<FogMgr.IntVector2>();
		for (int i = -radius; i <= radius; i++)
		{
			for (int j = -radius; j < radius; j++)
			{
				FogMgr.IntVector2 intVector = new FogMgr.IntVector2(i, j);
				if (intVector.sqrMagnitude() <= num)
				{
					list.Add(intVector);
				}
			}
		}
		FogMgr.IntVector2[] array = new FogMgr.IntVector2[list.Count];
		for (int k = 0; k < list.Count; k++)
		{
			array[k] = list[k];
		}
		this.mask.Add(radius, array);
		return array;
	}

	private void AddRealSightInfo(Vector3 vPos, float fRadius)
	{
		if (false || fRadius <= 0f)
		{
			return;
		}
		Vector2 vector = new Vector2(vPos.x - this.m_BasePos.x, vPos.z - this.m_BasePos.z);
		int radius = (int)fRadius;
		if (vector.x < 0f || vector.x >= 256f || vector.y < 0f || vector.y >= 256f)
		{
			Debug.LogError("unitsPos error!");
			return;
		}
		FogMgr.IntVector2[] vector2Mask = this.GetVector2Mask(radius);
		int num = vector2Mask.Length;
		for (int i = 0; i < num; i++)
		{
			FogMgr.IntVector2 intVector = vector2Mask[i];
			int num2 = (int)(vector.x + (float)intVector.x);
			if (num2 >= 0 && num2 < 256)
			{
				int num3 = (int)(vector.y + (float)intVector.y);
				if (num3 >= 0 && num3 < 256)
				{
					this.m_MyRealSight[num2, num3] = 1;
				}
			}
		}
	}

	private void AddFogInfo(Units units)
	{
		if (units == null)
		{
			return;
		}
		this.AddFogInfo(units.transform.position, units.fog_range);
	}

	private void ResetFogTable()
	{
		if (this.m_MyFogTable == null)
		{
			return;
		}
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				this.m_MyFogTable[i, j] = 0;
			}
		}
	}

	private void ResetRealSight()
	{
		if (this.m_MyRealSight == null)
		{
			return;
		}
		Array.Clear(this.m_MyRealSight, 0, 65536);
	}
}
