using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockDebugInfo
{
	public static Dictionary<int, BlockDebugInfo> InfoMap = new Dictionary<int, BlockDebugInfo>();

	public static Vector3 DrawSize = new Vector3(0.5f, 0.5f, 0.5f);

	public static bool inited = false;

	public int cnt;

	public Vector3 pos;

	public static void initPos(BlockDebugInfo info, int index)
	{
		if (!BlockDebugInfo.inited)
		{
			return;
		}
		int num = index >> 16;
		int num2 = index - (num << 16);
		GridGraph gridGraph = AstarPath.active.astarData.gridGraph;
		if (gridGraph != null && info != null)
		{
			Vector3 center = gridGraph.center;
			center.x -= gridGraph.nodeSize * (float)gridGraph.Width / 2f;
			center.z -= gridGraph.nodeSize * (float)gridGraph.Depth / 2f;
			Vector3 origin = center;
			origin.x += ((float)num2 + 0.5f) * gridGraph.nodeSize;
			origin.z += ((float)num + 0.5f) * gridGraph.nodeSize;
			origin.y += 100f;
			LayerMask layerMask = Layer.GroundMask;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, 100f, layerMask.value))
			{
				origin.y = raycastHit.point.y + 0.25f;
			}
			else
			{
				origin.y = center.y;
			}
			info.pos = origin;
		}
	}

	public static void add(int x, int y)
	{
		if (!BlockDebugInfo.inited)
		{
			GridGraph gridGraph = AstarPath.active.astarData.gridGraph;
			if (gridGraph != null)
			{
				BlockDebugInfo.inited = true;
				foreach (KeyValuePair<int, BlockDebugInfo> current in BlockDebugInfo.InfoMap)
				{
					BlockDebugInfo.initPos(current.Value, current.Key);
				}
			}
		}
		int num = (y << 16) + x;
		BlockDebugInfo blockDebugInfo = null;
		if (!BlockDebugInfo.InfoMap.TryGetValue(num, out blockDebugInfo))
		{
			blockDebugInfo = new BlockDebugInfo
			{
				cnt = 1,
				pos = Vector3.zero
			};
			BlockDebugInfo.initPos(blockDebugInfo, num);
			BlockDebugInfo.InfoMap.Add(num, blockDebugInfo);
		}
		else
		{
			blockDebugInfo.cnt++;
		}
	}

	public static void remove(int x, int y)
	{
		int key = (y << 16) + x;
		BlockDebugInfo blockDebugInfo = null;
		if (BlockDebugInfo.InfoMap.TryGetValue(key, out blockDebugInfo) && --blockDebugInfo.cnt == 0)
		{
			BlockDebugInfo.InfoMap.Remove(key);
		}
	}
}
