using Pathfinding;
using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

[AddComponentMenu("Pathfinding/Pathfinder")]
public class AstarPath : MonoBehaviour
{
	public enum AstarDistribution
	{
		WebsiteDownload,
		AssetStore
	}

	private enum GraphUpdateOrder
	{
		GraphUpdate,
		FloodFill
	}

	private struct GUOSingle
	{
		public AstarPath.GraphUpdateOrder order;

		public IUpdatableGraph graph;

		public GraphUpdateObject obj;
	}

	public struct AstarWorkItem
	{
		public OnVoidDelegate init;

		public Func<bool, bool> update;

		public AstarWorkItem(Func<bool, bool> update)
		{
			this.init = null;
			this.update = update;
		}

		public AstarWorkItem(OnVoidDelegate init, Func<bool, bool> update)
		{
			this.init = init;
			this.update = update;
		}
	}

	public static readonly AstarPath.AstarDistribution Distribution = AstarPath.AstarDistribution.AssetStore;

	public static readonly string Branch = "master_Pro";

	public static readonly bool HasPro = true;

	public AstarData astarData;

	public new static AstarPath active;

	public bool showNavGraphs = true;

	public bool showUnwalkableNodes = true;

	public bool showTagNodes = true;

	public GraphDebugMode debugMode;

	public float debugFloor;

	public float debugRoof = 20000f;

	public bool manualDebugFloorRoof;

	public bool showSearchTree;

	public float unwalkableNodeDebugSize = 0.3f;

	public PathLog logPathResults;

	public float maxNearestNodeDistance = 100f;

	public bool scanOnStartup = true;

	public bool fullGetNearestSearch;

	public bool prioritizeGraphs;

	public float prioritizeGraphsLimit = 1f;

	public AstarColor colorSettings;

	[SerializeField]
	protected string[] tagNames;

	public Heuristic heuristic = Heuristic.Euclidean;

	public float heuristicScale = 1f;

	public ThreadCount threadCount;

	public float maxFrameTime = 1f;

	public int minAreaSize;

	public bool limitGraphUpdates = true;

	public float maxGraphUpdateFreq = 0.2f;

	public static int PathsCompleted = 0;

	public float lastScanTime;

	public Path debugPath;

	public string inGameDebugPath;

	public bool isScanning;

	private bool graphUpdateRoutineRunning;

	private bool isRegisteredForUpdate;

	private bool workItemsQueued;

	private bool queuedWorkItemFloodFill;

	public static OnVoidDelegate OnAwakeSettings;

	public static OnGraphDelegate OnGraphPreScan;

	public static OnGraphDelegate OnGraphPostScan;

	public static OnPathDelegate OnPathPreSearch;

	public static OnPathDelegate OnPathPostSearch;

	public static OnScanDelegate OnPreScan;

	public static OnScanDelegate OnPostScan;

	public static OnScanDelegate OnLatePostScan;

	public static OnScanDelegate OnGraphsUpdated;

	public static OnVoidDelegate On65KOverflow;

	private static OnVoidDelegate OnThreadSafeCallback;

	public OnVoidDelegate OnDrawGizmosCallback;

	[Obsolete]
	public OnVoidDelegate OnGraphsWillBeUpdated;

	[Obsolete]
	public OnVoidDelegate OnGraphsWillBeUpdated2;

	private Queue<GraphUpdateObject> graphUpdateQueue;

	private Stack<GraphNode> floodStack;

	private ThreadControlQueue pathQueue = new ThreadControlQueue(0);

	private static Thread[] threads;

	private Thread graphUpdateThread;

	private static PathThreadInfo[] threadInfos = new PathThreadInfo[0];

	private static IEnumerator threadEnumerator;

	private static LockFreeStack pathReturnStack = new LockFreeStack();

	public EuclideanEmbedding euclideanEmbedding = new EuclideanEmbedding();

	private int nextNodeIndex = 1;

	private Stack<int> nodeIndexPool = new Stack<int>();

	private Path pathReturnPop;

	private Queue<AstarPath.GUOSingle> graphUpdateQueueAsync = new Queue<AstarPath.GUOSingle>();

	private Queue<AstarPath.GUOSingle> graphUpdateQueueRegular = new Queue<AstarPath.GUOSingle>();

	public bool showGraphs;

	public static bool isEditor = true;

	public uint lastUniqueAreaIndex;

	private static readonly object safeUpdateLock = new object();

	private AutoResetEvent graphUpdateAsyncEvent = new AutoResetEvent(false);

	private ManualResetEvent processingGraphUpdatesAsync = new ManualResetEvent(true);

	private float lastGraphUpdate = -9999f;

	private ushort nextFreePathID = 1;

	private Queue<AstarPath.AstarWorkItem> workItems = new Queue<AstarPath.AstarWorkItem>();

	private bool processingWorkItems;

	private float[] mHeightData;

	private Vector3 origin;

	private static int waitForPathDepth = 0;

	public static Version Version
	{
		get
		{
			return new Version(3, 6, 0, 0);
		}
	}

	[Obsolete]
	public Type[] graphTypes
	{
		get
		{
			return this.astarData.graphTypes;
		}
	}

	public NavGraph[] graphs
	{
		get
		{
			if (this.astarData == null)
			{
				this.astarData = new AstarData();
			}
			return this.astarData.graphs;
		}
		set
		{
			if (this.astarData == null)
			{
				this.astarData = new AstarData();
			}
			this.astarData.graphs = value;
		}
	}

	public float maxNearestNodeDistanceSqr
	{
		get
		{
			return this.maxNearestNodeDistance * this.maxNearestNodeDistance;
		}
	}

	public PathHandler debugPathData
	{
		get
		{
			if (this.debugPath == null)
			{
				return null;
			}
			return this.debugPath.pathHandler;
		}
	}

	public static int NumParallelThreads
	{
		get
		{
			return (AstarPath.threadInfos == null) ? 0 : AstarPath.threadInfos.Length;
		}
	}

	public static bool IsUsingMultithreading
	{
		get
		{
			if (AstarPath.threads != null && AstarPath.threads.Length > 0)
			{
				return true;
			}
			if (AstarPath.threads != null && AstarPath.threads.Length == 0 && AstarPath.threadEnumerator != null)
			{
				return false;
			}
			if (Application.isPlaying)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Not 'using threading' and not 'not using threading'... Are you sure pathfinding is set up correctly?\nIf scripts are reloaded in unity editor during play this could happen.\n",
					(AstarPath.threads == null) ? "NULL" : (string.Empty + AstarPath.threads.Length),
					" ",
					AstarPath.threadEnumerator != null
				}));
			}
			return false;
		}
	}

	public bool IsAnyGraphUpdatesQueued
	{
		get
		{
			return this.graphUpdateQueue != null && this.graphUpdateQueue.Count > 0;
		}
	}

	public string[] GetTagNames()
	{
		if (this.tagNames == null || this.tagNames.Length != 32)
		{
			this.tagNames = new string[32];
			for (int i = 0; i < this.tagNames.Length; i++)
			{
				this.tagNames[i] = string.Empty + i;
			}
			this.tagNames[0] = "Basic Ground";
		}
		return this.tagNames;
	}

	public static string[] FindTagNames()
	{
		if (AstarPath.active != null)
		{
			return AstarPath.active.GetTagNames();
		}
		AstarPath astarPath = UnityEngine.Object.FindObjectOfType(typeof(AstarPath)) as AstarPath;
		if (astarPath != null)
		{
			AstarPath.active = astarPath;
			return astarPath.GetTagNames();
		}
		return new string[]
		{
			"There is no AstarPath component in the scene"
		};
	}

	public ushort GetNextPathID()
	{
		if (this.nextFreePathID == 0)
		{
			this.nextFreePathID += 1;
			UnityEngine.Debug.Log("65K cleanup");
			if (AstarPath.On65KOverflow != null)
			{
				OnVoidDelegate on65KOverflow = AstarPath.On65KOverflow;
				AstarPath.On65KOverflow = null;
				on65KOverflow();
			}
		}
		ushort result;
		this.nextFreePathID = (result = this.nextFreePathID) + 1;
		return result;
	}

	private void OnDrawGizmos()
	{
		if (AstarPath.active == null)
		{
			AstarPath.active = this;
		}
		else if (AstarPath.active != this)
		{
			return;
		}
		if (this.graphs == null)
		{
			return;
		}
		if (this.pathQueue != null && this.pathQueue.AllReceiversBlocked && this.workItems.Count > 0)
		{
			return;
		}
		if (this.showNavGraphs && !this.manualDebugFloorRoof)
		{
			this.debugFloor = float.PositiveInfinity;
			this.debugRoof = float.NegativeInfinity;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null && this.graphs[i].drawGizmos)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						if (!AstarPath.active.showSearchTree || this.debugPathData == null || NavGraph.InSearchTree(node, this.debugPath))
						{
							PathNode pathNode = (this.debugPathData == null) ? null : this.debugPathData.GetPathNode(node);
							if (pathNode != null || this.debugMode == GraphDebugMode.Penalty)
							{
								switch (this.debugMode)
								{
								case GraphDebugMode.G:
									this.debugFloor = Mathf.Min(this.debugFloor, pathNode.G);
									this.debugRoof = Mathf.Max(this.debugRoof, pathNode.G);
									break;
								case GraphDebugMode.H:
									this.debugFloor = Mathf.Min(this.debugFloor, pathNode.H);
									this.debugRoof = Mathf.Max(this.debugRoof, pathNode.H);
									break;
								case GraphDebugMode.F:
									this.debugFloor = Mathf.Min(this.debugFloor, pathNode.F);
									this.debugRoof = Mathf.Max(this.debugRoof, pathNode.F);
									break;
								case GraphDebugMode.Penalty:
									this.debugFloor = Mathf.Min(this.debugFloor, node.Penalty);
									this.debugRoof = Mathf.Max(this.debugRoof, node.Penalty);
									break;
								}
							}
						}
						return true;
					});
				}
			}
			if (float.IsInfinity(this.debugFloor))
			{
				this.debugFloor = 0f;
				this.debugRoof = 1f;
			}
			if (this.debugRoof - this.debugFloor < 1f)
			{
				this.debugRoof += 1f;
			}
		}
		for (int j = 0; j < this.graphs.Length; j++)
		{
			if (this.graphs[j] != null && this.graphs[j].drawGizmos)
			{
				this.graphs[j].OnDrawGizmos(this.showNavGraphs);
			}
		}
		if (this.showNavGraphs)
		{
			this.euclideanEmbedding.OnDrawGizmos();
		}
		if (this.showUnwalkableNodes && this.showNavGraphs)
		{
			Gizmos.color = AstarColor.UnwalkableNode;
			GraphNodeDelegateCancelable del = new GraphNodeDelegateCancelable(this.DrawUnwalkableNode);
			for (int k = 0; k < this.graphs.Length; k++)
			{
				if (this.graphs[k] != null && this.graphs[k].drawGizmos)
				{
					this.graphs[k].GetNodes(del);
				}
			}
		}
		if (this.showTagNodes && this.showNavGraphs)
		{
			GraphNodeDelegateCancelable del2 = new GraphNodeDelegateCancelable(this.DrawTagNode);
			for (int l = 0; l < this.graphs.Length; l++)
			{
				if (this.graphs[l] != null && this.graphs[l].drawGizmos)
				{
					this.graphs[l].GetNodes(del2);
				}
			}
		}
		if (this.OnDrawGizmosCallback != null)
		{
			this.OnDrawGizmosCallback();
		}
	}

	private bool DrawUnwalkableNode(GraphNode node)
	{
		if (!node.Walkable)
		{
		}
		return true;
	}

	private bool DrawTagNode(GraphNode node)
	{
		if (node.Tag == 15u)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube((Vector3)node.position, Vector3.one * this.unwalkableNodeDebugSize);
		}
		else if (node.Tag == 10u)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawCube((Vector3)node.position, Vector3.one * this.unwalkableNodeDebugSize);
		}
		else if (node.Tag == 11u)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube((Vector3)node.position, Vector3.one * this.unwalkableNodeDebugSize);
		}
		else if (node.Tag == 12u)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawCube((Vector3)node.position, Vector3.one * this.unwalkableNodeDebugSize);
		}
		else if (node.Tag == 13u)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawCube((Vector3)node.position, Vector3.one * this.unwalkableNodeDebugSize);
		}
		return true;
	}

	private static void AstarLog(string s)
	{
		if (object.ReferenceEquals(AstarPath.active, null))
		{
			UnityEngine.Debug.Log("No AstarPath object was found : " + s);
			return;
		}
	}

	private static void AstarLogError(string s)
	{
		if (AstarPath.active == null)
		{
			UnityEngine.Debug.Log("No AstarPath object was found : " + s);
			return;
		}
		if (AstarPath.active.logPathResults != PathLog.None)
		{
			UnityEngine.Debug.LogError(s);
		}
	}

	private void LogPathResults(Path p)
	{
	}

	private void PerformBlockingActions(bool force = false, bool unblockOnComplete = true)
	{
		if (this.pathQueue.AllReceiversBlocked)
		{
			this.ReturnPaths(false);
			if (AstarPath.OnThreadSafeCallback != null)
			{
				OnVoidDelegate onThreadSafeCallback = AstarPath.OnThreadSafeCallback;
				AstarPath.OnThreadSafeCallback = null;
				onThreadSafeCallback();
			}
			if (this.ProcessWorkItems(force) == 2)
			{
				this.workItemsQueued = false;
				if (unblockOnComplete)
				{
					if (this.euclideanEmbedding.dirty)
					{
						this.euclideanEmbedding.RecalculateCosts();
					}
					this.pathQueue.Unblock();
				}
			}
		}
	}

	public void QueueWorkItemFloodFill()
	{
		if (!this.pathQueue.AllReceiversBlocked)
		{
			throw new Exception("You are calling QueueWorkItemFloodFill from outside a WorkItem. This might cause unexpected behaviour.");
		}
		this.queuedWorkItemFloodFill = true;
	}

	public void EnsureValidFloodFill()
	{
		if (this.queuedWorkItemFloodFill)
		{
			this.FloodFill();
		}
	}

	public void AddWorkItem(AstarPath.AstarWorkItem itm)
	{
		this.workItems.Enqueue(itm);
		if (!this.workItemsQueued)
		{
			this.workItemsQueued = true;
			if (!this.isScanning)
			{
				AstarPath.InterruptPathfinding();
			}
		}
	}

	private int ProcessWorkItems(bool force)
	{
		if (!this.pathQueue.AllReceiversBlocked)
		{
			return 0;
		}
		if (this.processingWorkItems)
		{
			throw new Exception("Processing work items recursively. Please do not wait for other work items to be completed inside work items. If you think this is not caused by any of your scripts, this might be a bug.");
		}
		this.processingWorkItems = true;
		while (this.workItems.Count > 0)
		{
			AstarPath.AstarWorkItem astarWorkItem = this.workItems.Peek();
			if (astarWorkItem.init != null)
			{
				astarWorkItem.init();
				astarWorkItem.init = null;
			}
			bool flag;
			try
			{
				flag = (astarWorkItem.update == null || astarWorkItem.update(force));
			}
			catch
			{
				this.workItems.Dequeue();
				this.processingWorkItems = false;
				throw;
			}
			if (!flag)
			{
				if (force)
				{
					UnityEngine.Debug.LogError("Misbehaving WorkItem. 'force'=true but the work item did not complete.\nIf force=true is passed to a WorkItem it should always return true.");
				}
				this.processingWorkItems = false;
				return 1;
			}
			this.workItems.Dequeue();
		}
		this.EnsureValidFloodFill();
		this.processingWorkItems = false;
		return 2;
	}

	public void QueueGraphUpdates()
	{
		if (!this.isRegisteredForUpdate)
		{
			this.isRegisteredForUpdate = true;
			this.AddWorkItem(new AstarPath.AstarWorkItem
			{
				init = new OnVoidDelegate(this.QueueGraphUpdatesInternal),
				update = new Func<bool, bool>(this.ProcessGraphUpdates)
			});
		}
	}

	[DebuggerHidden]
	private IEnumerator DelayedGraphUpdate()
	{
		AstarPath.<DelayedGraphUpdate>c__Iterator3 <DelayedGraphUpdate>c__Iterator = new AstarPath.<DelayedGraphUpdate>c__Iterator3();
		<DelayedGraphUpdate>c__Iterator.<>f__this = this;
		return <DelayedGraphUpdate>c__Iterator;
	}

	public void UpdateGraphs(Bounds bounds, float t)
	{
		this.UpdateGraphs(new GraphUpdateObject(bounds), t);
	}

	public void UpdateGraphs(GraphUpdateObject ob, float t)
	{
		base.StartCoroutine(this.UpdateGraphsInteral(ob, t));
	}

	[DebuggerHidden]
	private IEnumerator UpdateGraphsInteral(GraphUpdateObject ob, float t)
	{
		AstarPath.<UpdateGraphsInteral>c__Iterator4 <UpdateGraphsInteral>c__Iterator = new AstarPath.<UpdateGraphsInteral>c__Iterator4();
		<UpdateGraphsInteral>c__Iterator.t = t;
		<UpdateGraphsInteral>c__Iterator.ob = ob;
		<UpdateGraphsInteral>c__Iterator.<$>t = t;
		<UpdateGraphsInteral>c__Iterator.<$>ob = ob;
		<UpdateGraphsInteral>c__Iterator.<>f__this = this;
		return <UpdateGraphsInteral>c__Iterator;
	}

	public void UpdateGraphs(Bounds bounds)
	{
		this.UpdateGraphs(new GraphUpdateObject(bounds));
	}

	public void UpdateGraphs(GraphUpdateObject ob)
	{
		if (this.graphUpdateQueue == null)
		{
			this.graphUpdateQueue = new Queue<GraphUpdateObject>();
		}
		this.graphUpdateQueue.Enqueue(ob);
		if (this.limitGraphUpdates && Time.time - this.lastGraphUpdate < this.maxGraphUpdateFreq)
		{
			if (!this.graphUpdateRoutineRunning)
			{
				base.StartCoroutine(this.DelayedGraphUpdate());
			}
		}
		else
		{
			this.QueueGraphUpdates();
		}
	}

	public void FlushGraphUpdates()
	{
		if (this.IsAnyGraphUpdatesQueued)
		{
			this.QueueGraphUpdates();
			this.FlushWorkItems(true, true);
		}
	}

	public void FlushWorkItems(bool unblockOnComplete = true, bool block = false)
	{
		this.BlockUntilPathQueueBlocked();
		this.PerformBlockingActions(block, unblockOnComplete);
	}

	private void QueueGraphUpdatesInternal()
	{
		this.isRegisteredForUpdate = false;
		bool flag = false;
		while (this.graphUpdateQueue.Count > 0)
		{
			GraphUpdateObject graphUpdateObject = this.graphUpdateQueue.Dequeue();
			if (graphUpdateObject.requiresFloodFill)
			{
				flag = true;
			}
			foreach (IUpdatableGraph updatableGraph in this.astarData.GetUpdateableGraphs())
			{
				NavGraph graph = updatableGraph as NavGraph;
				if (graphUpdateObject.nnConstraint == null || graphUpdateObject.nnConstraint.SuitableGraph(AstarPath.active.astarData.GetGraphIndex(graph), graph))
				{
					AstarPath.GUOSingle item = default(AstarPath.GUOSingle);
					item.order = AstarPath.GraphUpdateOrder.GraphUpdate;
					item.obj = graphUpdateObject;
					item.graph = updatableGraph;
					this.graphUpdateQueueRegular.Enqueue(item);
				}
			}
		}
		if (flag)
		{
			AstarPath.GUOSingle item2 = default(AstarPath.GUOSingle);
			item2.order = AstarPath.GraphUpdateOrder.FloodFill;
			this.graphUpdateQueueRegular.Enqueue(item2);
		}
		this.debugPath = null;
		GraphModifier.TriggerEvent(GraphModifier.EventType.PreUpdate);
	}

	private bool ProcessGraphUpdates(bool force)
	{
		if (force)
		{
			this.processingGraphUpdatesAsync.WaitOne();
		}
		else if (!this.processingGraphUpdatesAsync.WaitOne(0))
		{
			return false;
		}
		if (this.graphUpdateQueueAsync.Count != 0)
		{
			throw new Exception("Queue should be empty at this stage");
		}
		while (this.graphUpdateQueueRegular.Count > 0)
		{
			AstarPath.GUOSingle item = this.graphUpdateQueueRegular.Peek();
			GraphUpdateThreading graphUpdateThreading = (item.order != AstarPath.GraphUpdateOrder.FloodFill) ? item.graph.CanUpdateAsync(item.obj) : GraphUpdateThreading.SeparateThread;
			bool flag = force;
			if (!Application.isPlaying || this.graphUpdateThread == null || !this.graphUpdateThread.IsAlive)
			{
				flag = true;
			}
			if (!flag && graphUpdateThreading == GraphUpdateThreading.SeparateAndUnityInit)
			{
				if (this.graphUpdateQueueAsync.Count > 0)
				{
					this.processingGraphUpdatesAsync.Reset();
					this.graphUpdateAsyncEvent.Set();
					return false;
				}
				item.graph.UpdateAreaInit(item.obj);
				this.graphUpdateQueueRegular.Dequeue();
				this.graphUpdateQueueAsync.Enqueue(item);
				this.processingGraphUpdatesAsync.Reset();
				this.graphUpdateAsyncEvent.Set();
				return false;
			}
			else if (!flag && graphUpdateThreading == GraphUpdateThreading.SeparateThread)
			{
				this.graphUpdateQueueRegular.Dequeue();
				this.graphUpdateQueueAsync.Enqueue(item);
			}
			else if (this.graphUpdateQueueAsync.Count > 0)
			{
				if (force)
				{
					throw new Exception("This should not happen");
				}
				this.processingGraphUpdatesAsync.Reset();
				this.graphUpdateAsyncEvent.Set();
				return false;
			}
			else
			{
				this.graphUpdateQueueRegular.Dequeue();
				if (item.order == AstarPath.GraphUpdateOrder.FloodFill)
				{
					this.FloodFill();
				}
				else
				{
					if (graphUpdateThreading == GraphUpdateThreading.SeparateAndUnityInit)
					{
						try
						{
							item.graph.UpdateAreaInit(item.obj);
						}
						catch (Exception arg)
						{
							UnityEngine.Debug.LogError("Error while initializing GraphUpdates\n" + arg);
						}
					}
					try
					{
						item.graph.UpdateArea(item.obj);
					}
					catch (Exception arg2)
					{
						UnityEngine.Debug.LogError("Error while updating graphs\n" + arg2);
					}
				}
			}
		}
		if (this.graphUpdateQueueAsync.Count > 0)
		{
			this.processingGraphUpdatesAsync.Reset();
			this.graphUpdateAsyncEvent.Set();
			return false;
		}
		GraphModifier.TriggerEvent(GraphModifier.EventType.PostUpdate);
		if (AstarPath.OnGraphsUpdated != null)
		{
			AstarPath.OnGraphsUpdated(this);
		}
		return true;
	}

	private void ProcessGraphUpdatesAsync(object _astar)
	{
		AstarPath astarPath = _astar as AstarPath;
		if (object.ReferenceEquals(astarPath, null))
		{
			UnityEngine.Debug.LogError("ProcessGraphUpdatesAsync started with invalid parameter _astar (was no AstarPath object)");
			return;
		}
		while (!astarPath.pathQueue.IsTerminating)
		{
			this.graphUpdateAsyncEvent.WaitOne();
			if (astarPath.pathQueue.IsTerminating)
			{
				this.graphUpdateQueueAsync.Clear();
				this.processingGraphUpdatesAsync.Set();
				return;
			}
			while (this.graphUpdateQueueAsync.Count > 0)
			{
				AstarPath.GUOSingle gUOSingle = this.graphUpdateQueueAsync.Dequeue();
				try
				{
					if (gUOSingle.order == AstarPath.GraphUpdateOrder.GraphUpdate)
					{
						gUOSingle.graph.UpdateArea(gUOSingle.obj);
					}
					else
					{
						if (gUOSingle.order != AstarPath.GraphUpdateOrder.FloodFill)
						{
							throw new NotSupportedException(string.Empty + gUOSingle.order);
						}
						astarPath.FloodFill();
					}
				}
				catch (Exception arg)
				{
					UnityEngine.Debug.LogError("Exception while updating graphs:\n" + arg);
				}
			}
			this.processingGraphUpdatesAsync.Set();
		}
	}

	public void FlushThreadSafeCallbacks()
	{
		if (AstarPath.OnThreadSafeCallback == null)
		{
			return;
		}
		this.BlockUntilPathQueueBlocked();
		this.PerformBlockingActions(false, true);
	}

	[ContextMenu("Log Profiler")]
	public void LogProfiler()
	{
	}

	[ContextMenu("Reset Profiler")]
	public void ResetProfiler()
	{
	}

	public static int CalculateThreadCount(ThreadCount count)
	{
		if (count != ThreadCount.AutomaticLowLoad && count != ThreadCount.AutomaticHighLoad)
		{
			return (int)count;
		}
		int num = Mathf.Max(1, SystemInfo.processorCount);
		int num2 = SystemInfo.systemMemorySize;
		if (num2 <= 0)
		{
			UnityEngine.Debug.LogError("Machine reporting that is has <= 0 bytes of RAM. This is definitely not true, assuming 1 GiB");
			num2 = 1024;
		}
		if (num <= 1)
		{
			return 0;
		}
		if (num2 <= 512)
		{
			return 0;
		}
		if (count == ThreadCount.AutomaticHighLoad)
		{
			if (num2 <= 1024)
			{
				num = Math.Min(num, 2);
			}
		}
		else
		{
			num /= 2;
			num = Mathf.Max(1, num);
			if (num2 <= 1024)
			{
				num = Math.Min(num, 2);
			}
			num = Math.Min(num, 6);
		}
		return num;
	}

	public void Awake()
	{
		AstarPath.active = this;
		this.InitMapHeight();
	}

	public void ScanMapHeight()
	{
		if (this.astarData != null && this.astarData.gridGraph != null)
		{
			GridGraph graph = this.astarData.gridGraph;
			this.mHeightData = new float[graph.nodes.Length];
			this.origin = graph.center;
			this.origin.x = this.origin.x - graph.nodeSize * (float)graph.Width / 2f;
			this.origin.z = this.origin.z - graph.nodeSize * (float)graph.Depth / 2f;
			graph.GetNodes(delegate(GraphNode node)
			{
				Vector3 vector = (Vector3)node.position;
				int num = (int)((vector.x - this.origin.x) / graph.nodeSize);
				int num2 = (int)((vector.z - this.origin.z) / graph.nodeSize);
				this.mHeightData[num2 * graph.width + num] = vector.y;
				return true;
			});
		}
		else
		{
			this.mHeightData = null;
		}
		if (this.astarData != null && this.astarData.gridGraph != null)
		{
			this.astarData.heightMap = this.mHeightData;
			this.astarData.gridNodeSize = this.astarData.gridGraph.nodeSize;
			this.astarData.gridWidth = this.astarData.gridGraph.width;
			this.astarData.gridDepth = this.astarData.gridGraph.Depth;
			this.astarData.gridCenter = this.astarData.gridGraph.center;
		}
	}

	private void InitMapHeight()
	{
		if (this.astarData != null)
		{
			this.mHeightData = this.astarData.heightMap;
			this.origin = this.astarData.gridCenter;
			this.origin.x = this.origin.x - this.astarData.gridNodeSize * (float)this.astarData.gridWidth / 2f;
			this.origin.z = this.origin.z - this.astarData.gridNodeSize * (float)this.astarData.gridDepth / 2f;
		}
		else
		{
			UnityEngine.Debug.LogError("Can't find heightMap");
		}
	}

	public float GetPosHeight(Vector3 pos)
	{
		if (this.mHeightData == null)
		{
			return 0f;
		}
		float num = pos.x - this.origin.x;
		float num2 = pos.z - this.origin.z;
		float gridNodeSize = this.astarData.gridNodeSize;
		int num3 = (int)(num / gridNodeSize - 0.5f);
		int num4 = (int)(num2 / gridNodeSize - 0.5f);
		int gridWidth = this.astarData.gridWidth;
		int gridDepth = this.astarData.gridDepth;
		if (num3 < 0 || num4 < 0 || num3 > gridWidth - 2 || num4 > gridDepth - 2)
		{
			if (num3 < 0)
			{
				num3 = 0;
			}
			else if (num3 > gridWidth - 1)
			{
				num3 = gridWidth - 1;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			else if (num4 > gridDepth - 1)
			{
				num4 = gridDepth - 1;
			}
			return this.mHeightData[num4 * gridWidth + num3];
		}
		float num5 = this.mHeightData[num4 * gridWidth + num3];
		float num6 = this.mHeightData[(num4 + 1) * gridWidth + num3 + 1];
		float num7 = num - ((float)num3 + 0.5f) * gridNodeSize;
		float num8 = num2 - ((float)num4 + 0.5f) * gridNodeSize;
		if (num7 < num8)
		{
			float num9 = this.mHeightData[(num4 + 1) * gridWidth + num3];
			return num5 + ((num9 - num5) * num8 + (num6 - num9) * num7) / gridNodeSize;
		}
		float num10 = this.mHeightData[num4 * gridWidth + num3 + 1];
		return num5 + ((num10 - num5) * num7 + (num6 - num10) * num8) / gridNodeSize;
	}

	public void VerifyIntegrity()
	{
		if (AstarPath.active != this)
		{
			throw new Exception("Singleton pattern broken. Make sure you only have one AstarPath object in the scene");
		}
		if (this.astarData == null)
		{
			throw new NullReferenceException("AstarData is null... Astar not set up correctly?");
		}
	}

	public void SetUpReferences()
	{
		AstarPath.active = this;
		if (this.astarData == null)
		{
			this.astarData = new AstarData();
		}
		if (this.astarData.userConnections == null)
		{
			this.astarData.userConnections = new UserConnection[0];
		}
		if (this.colorSettings == null)
		{
			this.colorSettings = new AstarColor();
		}
		this.colorSettings.OnEnable();
	}

	private void Initialize()
	{
		this.SetUpReferences();
		this.astarData.FindGraphTypes();
		this.astarData.Awake();
		this.astarData.UpdateShortcuts();
		for (int i = 0; i < this.astarData.graphs.Length; i++)
		{
			if (this.astarData.graphs[i] != null)
			{
				this.astarData.graphs[i].Awake();
			}
		}
	}

	public void OnDestroy()
	{
		if (AstarPath.active != this)
		{
			return;
		}
		this.astarData.OnDestroy();
		this.floodStack = null;
		this.graphUpdateQueue = null;
		this.OnDrawGizmosCallback = null;
		AstarPath.OnAwakeSettings = null;
		AstarPath.OnGraphPreScan = null;
		AstarPath.OnGraphPostScan = null;
		AstarPath.OnPathPreSearch = null;
		AstarPath.OnPathPostSearch = null;
		AstarPath.OnPreScan = null;
		AstarPath.OnPostScan = null;
		AstarPath.OnLatePostScan = null;
		AstarPath.On65KOverflow = null;
		AstarPath.OnGraphsUpdated = null;
		AstarPath.OnThreadSafeCallback = null;
		AstarPath.threads = null;
		AstarPath.threadInfos = null;
		AstarPath.PathsCompleted = 0;
		AstarPath.active = null;
	}

	public void FloodFill(GraphNode seed)
	{
		this.FloodFill(seed, this.lastUniqueAreaIndex + 1u);
		this.lastUniqueAreaIndex += 1u;
	}

	public void FloodFill(GraphNode seed, uint area)
	{
		if (area > 131071u)
		{
			UnityEngine.Debug.LogError("Too high area index - The maximum area index is " + 131071u);
			return;
		}
		if (area < 0u)
		{
			UnityEngine.Debug.LogError("Too low area index - The minimum area index is 0");
			return;
		}
		if (this.floodStack == null)
		{
			this.floodStack = new Stack<GraphNode>(1024);
		}
		Stack<GraphNode> stack = this.floodStack;
		stack.Clear();
		stack.Push(seed);
		seed.Area = area;
		while (stack.Count > 0)
		{
			stack.Pop().FloodFill(stack, area);
		}
	}

	[ContextMenu("Flood Fill Graphs")]
	public void FloodFill()
	{
		this.queuedWorkItemFloodFill = false;
		if (this.astarData.graphs == null)
		{
			return;
		}
		uint area = 0u;
		this.lastUniqueAreaIndex = 0u;
		if (this.floodStack == null)
		{
			this.floodStack = new Stack<GraphNode>(1024);
		}
		Stack<GraphNode> stack = this.floodStack;
		for (int i = 0; i < this.graphs.Length; i++)
		{
			NavGraph navGraph = this.graphs[i];
			if (navGraph != null)
			{
				navGraph.GetNodes(delegate(GraphNode node)
				{
					node.Area = 0u;
					return true;
				});
			}
		}
		int smallAreasDetected = 0;
		bool warnAboutAreas = false;
		List<GraphNode> smallAreaList = ListPool<GraphNode>.Claim();
		for (int j = 0; j < this.graphs.Length; j++)
		{
			NavGraph navGraph2 = this.graphs[j];
			if (navGraph2 != null)
			{
				GraphNodeDelegateCancelable del = delegate(GraphNode node)
				{
					if (node.Walkable && node.Area == 0u)
					{
						uint area;
						area += 1u;
						area = area;
						if (area > 131071u)
						{
							if (smallAreaList.Count > 0)
							{
								GraphNode graphNode = smallAreaList[smallAreaList.Count - 1];
								area = graphNode.Area;
								smallAreaList.RemoveAt(smallAreaList.Count - 1);
								stack.Clear();
								stack.Push(graphNode);
								graphNode.Area = 131071u;
								while (stack.Count > 0)
								{
									stack.Pop().FloodFill(stack, 131071u);
								}
								smallAreasDetected++;
							}
							else
							{
								area -= 1u;
								area = area;
								warnAboutAreas = true;
							}
						}
						stack.Clear();
						stack.Push(node);
						int num = 1;
						node.Area = area;
						while (stack.Count > 0)
						{
							num++;
							stack.Pop().FloodFill(stack, area);
						}
						if (num < this.minAreaSize)
						{
							smallAreaList.Add(node);
						}
					}
					return true;
				};
				navGraph2.GetNodes(del);
			}
		}
		this.lastUniqueAreaIndex = area;
		if (warnAboutAreas)
		{
			UnityEngine.Debug.LogError("Too many areas - The maximum number of areas is " + 131071u + ". Try raising the A* Inspector -> Settings -> Min Area Size value. Enable the optimization ASTAR_MORE_AREAS under the Optimizations tab.");
		}
		if (smallAreasDetected > 0)
		{
			AstarPath.AstarLog(string.Concat(new object[]
			{
				smallAreasDetected,
				" small areas were detected (fewer than ",
				this.minAreaSize,
				" nodes),these might have the same IDs as other areas, but it shouldn't affect pathfinding in any significant way (you might get All Nodes Searched as a reason for path failure).\nWhich areas are defined as 'small' is controlled by the 'Min Area Size' variable, it can be changed in the A* inspector-->Settings-->Min Area Size\nThe small areas will use the area id ",
				131071u
			}));
		}
		ListPool<GraphNode>.Release(smallAreaList);
	}

	public int GetNewNodeIndex()
	{
		if (this.nodeIndexPool.Count > 0)
		{
			return this.nodeIndexPool.Pop();
		}
		return this.nextNodeIndex++;
	}

	public void InitializeNode(GraphNode node)
	{
		if (!this.pathQueue.AllReceiversBlocked)
		{
			throw new Exception("Trying to initialize a node when it is not safe to initialize any nodes. Must be done during a graph update");
		}
		if (AstarPath.threadInfos == null)
		{
			AstarPath.threadInfos = new PathThreadInfo[0];
		}
		for (int i = 0; i < AstarPath.threadInfos.Length; i++)
		{
			AstarPath.threadInfos[i].runData.InitializeNode(node);
		}
	}

	public void DestroyNode(GraphNode node)
	{
		if (node.NodeIndex == -1)
		{
			return;
		}
		this.nodeIndexPool.Push(node.NodeIndex);
		if (AstarPath.threadInfos == null)
		{
			AstarPath.threadInfos = new PathThreadInfo[0];
		}
		for (int i = 0; i < AstarPath.threadInfos.Length; i++)
		{
			AstarPath.threadInfos[i].runData.DestroyNode(node);
		}
	}

	public void BlockUntilPathQueueBlocked()
	{
		if (this.pathQueue == null)
		{
			return;
		}
		this.pathQueue.Block();
		while (!this.pathQueue.AllReceiversBlocked)
		{
			if (AstarPath.IsUsingMultithreading)
			{
				Thread.Sleep(1);
			}
			else
			{
				AstarPath.threadEnumerator.MoveNext();
			}
		}
	}

	public void Scan()
	{
		this.ScanLoop(null);
	}

	public void ScanLoop(OnScanStatus statusCallback)
	{
		if (this.graphs == null)
		{
			return;
		}
		this.isScanning = true;
		this.euclideanEmbedding.dirty = false;
		this.VerifyIntegrity();
		this.BlockUntilPathQueueBlocked();
		if (!Application.isPlaying)
		{
			GraphModifier.FindAllModifiers();
			RelevantGraphSurface.FindAllGraphSurfaces();
		}
		RelevantGraphSurface.UpdateAllPositions();
		this.astarData.UpdateShortcuts();
		if (statusCallback != null)
		{
			statusCallback(new Progress(0.05f, "Pre processing graphs"));
		}
		if (AstarPath.OnPreScan != null)
		{
			AstarPath.OnPreScan(this);
		}
		GraphModifier.TriggerEvent(GraphModifier.EventType.PreScan);
		DateTime utcNow = DateTime.UtcNow;
		for (int j = 0; j < this.graphs.Length; j++)
		{
			if (this.graphs[j] != null)
			{
				this.graphs[j].GetNodes(delegate(GraphNode node)
				{
					node.Destroy();
					return true;
				});
			}
		}
		int i;
		for (i = 0; i < this.graphs.Length; i++)
		{
			NavGraph navGraph = this.graphs[i];
			if (navGraph == null)
			{
				if (statusCallback != null)
				{
					statusCallback(new Progress(AstarMath.MapTo(0.05f, 0.7f, ((float)i + 0.5f) / (float)(this.graphs.Length + 1)), string.Concat(new object[]
					{
						"Skipping graph ",
						i + 1,
						" of ",
						this.graphs.Length,
						" because it is null"
					})));
				}
			}
			else
			{
				if (AstarPath.OnGraphPreScan != null)
				{
					if (statusCallback != null)
					{
						statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.7f, (float)i / (float)this.graphs.Length), string.Concat(new object[]
						{
							"Scanning graph ",
							i + 1,
							" of ",
							this.graphs.Length,
							" - Pre processing"
						})));
					}
					AstarPath.OnGraphPreScan(navGraph);
				}
				float minp = AstarMath.MapToRange(0.1f, 0.7f, (float)i / (float)this.graphs.Length);
				float maxp = AstarMath.MapToRange(0.1f, 0.7f, ((float)i + 0.95f) / (float)this.graphs.Length);
				if (statusCallback != null)
				{
					statusCallback(new Progress(minp, string.Concat(new object[]
					{
						"Scanning graph ",
						i + 1,
						" of ",
						this.graphs.Length
					})));
				}
				OnScanStatus statusCallback2 = null;
				if (statusCallback != null)
				{
					statusCallback2 = delegate(Progress p)
					{
						p.progress = AstarMath.MapToRange(minp, maxp, p.progress);
						statusCallback(p);
					};
				}
				navGraph.ScanInternal(statusCallback2);
				navGraph.GetNodes(delegate(GraphNode node)
				{
					node.GraphIndex = (uint)i;
					return true;
				});
				if (AstarPath.OnGraphPostScan != null)
				{
					if (statusCallback != null)
					{
						statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.7f, ((float)i + 0.95f) / (float)this.graphs.Length), string.Concat(new object[]
						{
							"Scanning graph ",
							i + 1,
							" of ",
							this.graphs.Length,
							" - Post processing"
						})));
					}
					AstarPath.OnGraphPostScan(navGraph);
				}
			}
		}
		if (statusCallback != null)
		{
			statusCallback(new Progress(0.8f, "Post processing graphs"));
		}
		if (AstarPath.OnPostScan != null)
		{
			AstarPath.OnPostScan(this);
		}
		GraphModifier.TriggerEvent(GraphModifier.EventType.PostScan);
		this.ApplyLinks();
		try
		{
			this.FlushWorkItems(false, true);
		}
		catch (Exception exception)
		{
			UnityEngine.Debug.LogException(exception);
		}
		this.isScanning = false;
		if (statusCallback != null)
		{
			statusCallback(new Progress(0.9f, "Computing areas"));
		}
		this.FloodFill();
		this.VerifyIntegrity();
		if (statusCallback != null)
		{
			statusCallback(new Progress(0.95f, "Late post processing"));
		}
		if (AstarPath.OnLatePostScan != null)
		{
			AstarPath.OnLatePostScan(this);
		}
		GraphModifier.TriggerEvent(GraphModifier.EventType.LatePostScan);
		this.euclideanEmbedding.dirty = true;
		this.euclideanEmbedding.RecalculatePivots();
		this.PerformBlockingActions(true, true);
		this.lastScanTime = (float)(DateTime.UtcNow - utcNow).TotalSeconds;
		GC.Collect();
		AstarPath.AstarLog("Scanning - Process took " + (this.lastScanTime * 1000f).ToString("0") + " ms to complete");
	}

	public void ApplyLinks()
	{
		if (this.astarData.userConnections != null && this.astarData.userConnections.Length > 0)
		{
			UnityEngine.Debug.LogWarning("<b>Deleting all links now</b>, but saving graph data in backup variable.\nCreating replacement links using the new system, stored under the <i>Links</i> GameObject.");
			GameObject gameObject = new GameObject("Links");
			Dictionary<Int3, GameObject> dictionary = new Dictionary<Int3, GameObject>();
			for (int i = 0; i < this.astarData.userConnections.Length; i++)
			{
				UserConnection userConnection = this.astarData.userConnections[i];
				GameObject gameObject2 = (!dictionary.ContainsKey((Int3)userConnection.p1)) ? new GameObject("Link " + i) : dictionary[(Int3)userConnection.p1];
				GameObject gameObject3 = (!dictionary.ContainsKey((Int3)userConnection.p2)) ? new GameObject("Link " + i) : dictionary[(Int3)userConnection.p2];
				gameObject2.transform.parent = gameObject.transform;
				gameObject3.transform.parent = gameObject.transform;
				dictionary[(Int3)userConnection.p1] = gameObject2;
				dictionary[(Int3)userConnection.p2] = gameObject3;
				gameObject2.transform.position = userConnection.p1;
				gameObject3.transform.position = userConnection.p2;
				NodeLink nodeLink = gameObject2.AddComponent<NodeLink>();
				nodeLink.end = gameObject3.transform;
				nodeLink.deleteConnection = !userConnection.enable;
			}
			this.astarData.userConnections = null;
			this.astarData.data_backup = this.astarData.GetData();
			throw new NotSupportedException("<b>Links have been deprecated</b>. Please use the component <b>NodeLink</b> instead. Create two GameObjects around the points you want to link, then press <b>Cmd+Alt+L</b> ( <b>Ctrl+Alt+L</b> on windows) to link them. See <b>Menubar -> Edit -> Pathfinding</b>.");
		}
	}

	public static void WaitForPath(Path p)
	{
		if (AstarPath.active == null)
		{
			throw new Exception("Pathfinding is not correctly initialized in this scene (yet?). AstarPath.active is null.\nDo not call this function in Awake");
		}
		if (p == null)
		{
			throw new ArgumentNullException("Path must not be null");
		}
		if (AstarPath.active.pathQueue.IsTerminating)
		{
			return;
		}
		if (p.GetState() == PathState.Created)
		{
			throw new Exception("The specified path has not been started yet.");
		}
		AstarPath.waitForPathDepth++;
		if (AstarPath.waitForPathDepth == 5)
		{
			UnityEngine.Debug.LogError("You are calling the WaitForPath function recursively (maybe from a path callback). Please don't do this.");
		}
		if (p.GetState() < PathState.ReturnQueue)
		{
			if (AstarPath.IsUsingMultithreading)
			{
				while (p.GetState() < PathState.ReturnQueue)
				{
					if (AstarPath.active.pathQueue.IsTerminating)
					{
						AstarPath.waitForPathDepth--;
						throw new Exception("Pathfinding Threads seems to have crashed.");
					}
					Thread.Sleep(1);
					AstarPath.active.PerformBlockingActions(false, true);
				}
			}
			else
			{
				while (p.GetState() < PathState.ReturnQueue)
				{
					if (AstarPath.active.pathQueue.IsEmpty && p.GetState() != PathState.Processing)
					{
						AstarPath.waitForPathDepth--;
						throw new Exception("Critical error. Path Queue is empty but the path state is '" + p.GetState() + "'");
					}
					AstarPath.threadEnumerator.MoveNext();
					AstarPath.active.PerformBlockingActions(false, true);
				}
			}
		}
		AstarPath.active.ReturnPaths(false);
		AstarPath.waitForPathDepth--;
	}

	[Obsolete("The threadSafe parameter has been deprecated")]
	public static void RegisterSafeUpdate(OnVoidDelegate callback, bool threadSafe)
	{
		AstarPath.RegisterSafeUpdate(callback);
	}

	public static void RegisterSafeUpdate(OnVoidDelegate callback)
	{
		if (callback == null || !Application.isPlaying)
		{
			return;
		}
		if (AstarPath.active.pathQueue.AllReceiversBlocked)
		{
			AstarPath.active.pathQueue.Lock();
			try
			{
				if (AstarPath.active.pathQueue.AllReceiversBlocked)
				{
					callback();
					return;
				}
			}
			finally
			{
				AstarPath.active.pathQueue.Unlock();
			}
		}
		object obj = AstarPath.safeUpdateLock;
		lock (obj)
		{
			AstarPath.OnThreadSafeCallback = (OnVoidDelegate)Delegate.Combine(AstarPath.OnThreadSafeCallback, callback);
		}
		AstarPath.active.pathQueue.Block();
	}

	private static void InterruptPathfinding()
	{
		AstarPath.active.pathQueue.Block();
	}

	public static void StartPath(Path p, bool pushToFront = false)
	{
		if (object.ReferenceEquals(AstarPath.active, null))
		{
			UnityEngine.Debug.LogError("There is no AstarPath object in the scene");
			return;
		}
		if (p.GetState() != PathState.Created)
		{
			throw new Exception(string.Concat(new object[]
			{
				"The path has an invalid state. Expected ",
				PathState.Created,
				" found ",
				p.GetState(),
				"\nMake sure you are not requesting the same path twice"
			}));
		}
		if (AstarPath.active.pathQueue.IsTerminating)
		{
			p.Error();
			p.LogError("No new paths are accepted");
			return;
		}
		if (AstarPath.active.graphs == null || AstarPath.active.graphs.Length == 0)
		{
			UnityEngine.Debug.LogError("There are no graphs in the scene");
			p.Error();
			p.LogError("There are no graphs in the scene");
			UnityEngine.Debug.LogError(p.errorLog);
			return;
		}
		p.Claim(AstarPath.active);
		p.AdvanceState(PathState.PathQueue);
		if (pushToFront)
		{
			AstarPath.active.pathQueue.PushFront(p);
		}
		else
		{
			AstarPath.active.pathQueue.Push(p);
		}
	}

	public void OnApplicationQuit()
	{
		this.OnDestroy();
	}

	public void ReturnPaths(bool timeSlice)
	{
		Path next = AstarPath.pathReturnStack.PopAll();
		if (this.pathReturnPop == null)
		{
			this.pathReturnPop = next;
		}
		else
		{
			Path next2 = this.pathReturnPop;
			while (next2.next != null)
			{
				next2 = next2.next;
			}
			next2.next = next;
		}
		long num = (!timeSlice) ? 0L : (DateTime.UtcNow.Ticks + 10000L);
		int num2 = 0;
		while (this.pathReturnPop != null)
		{
			Path path = this.pathReturnPop;
			this.pathReturnPop = this.pathReturnPop.next;
			path.next = null;
			path.ReturnPath();
			path.AdvanceState(PathState.Returned);
			path.ReleaseSilent(this);
			num2++;
			if (num2 > 5 && timeSlice)
			{
				num2 = 0;
				if (DateTime.UtcNow.Ticks >= num)
				{
					return;
				}
			}
		}
	}

	private static void CalculatePathsThreaded(object _threadInfo)
	{
		PathThreadInfo pathThreadInfo;
		try
		{
			pathThreadInfo = (PathThreadInfo)_threadInfo;
		}
		catch (Exception ex)
		{
			UnityEngine.Debug.LogError("Arguments to pathfinding threads must be of type ThreadStartInfo\n" + ex);
			throw new ArgumentException("Argument must be of type ThreadStartInfo", ex);
		}
		AstarPath astar = pathThreadInfo.astar;
		try
		{
			PathHandler runData = pathThreadInfo.runData;
			if (runData.nodes == null)
			{
				throw new NullReferenceException("NodeRuns must be assigned to the threadInfo.runData.nodes field before threads are started\nthreadInfo is an argument to the thread functions");
			}
			long num = (long)(astar.maxFrameTime * 10000f);
			long num2 = DateTime.UtcNow.Ticks + num;
			while (true)
			{
				Path path = astar.pathQueue.Pop();
				num = (long)(astar.maxFrameTime * 10000f);
				path.PrepareBase(runData);
				path.AdvanceState(PathState.Processing);
				if (AstarPath.OnPathPreSearch != null)
				{
					AstarPath.OnPathPreSearch(path);
				}
				long ticks = DateTime.UtcNow.Ticks;
				long num3 = 0L;
				path.Prepare();
				if (!path.IsDone())
				{
					astar.debugPath = path;
					path.Initialize();
					while (!path.IsDone())
					{
						path.CalculateStep(num2);
						path.searchIterations++;
						if (path.IsDone())
						{
							break;
						}
						num3 += DateTime.UtcNow.Ticks - ticks;
						Thread.Sleep(0);
						ticks = DateTime.UtcNow.Ticks;
						num2 = ticks + num;
						if (astar.pathQueue.IsTerminating)
						{
							path.Error();
						}
					}
					num3 += DateTime.UtcNow.Ticks - ticks;
					path.duration = (float)num3 * 0.0001f;
				}
				path.Cleanup();
				astar.LogPathResults(path);
				if (path.immediateCallback != null)
				{
					path.immediateCallback(path);
				}
				if (AstarPath.OnPathPostSearch != null)
				{
					AstarPath.OnPathPostSearch(path);
				}
				AstarPath.pathReturnStack.Push(path);
				path.AdvanceState(PathState.ReturnQueue);
				if (DateTime.UtcNow.Ticks > num2)
				{
					Thread.Sleep(1);
					num2 = DateTime.UtcNow.Ticks + num;
				}
			}
		}
		catch (Exception ex2)
		{
			if (ex2 is ThreadAbortException || ex2 is ThreadControlQueue.QueueTerminationException)
			{
				return;
			}
			UnityEngine.Debug.LogException(ex2);
			UnityEngine.Debug.LogError("Unhandled exception during pathfinding. Terminating.");
			astar.pathQueue.TerminateReceivers();
		}
		UnityEngine.Debug.LogError("Error : This part should never be reached.");
		astar.pathQueue.ReceiverTerminated();
	}

	[DebuggerHidden]
	private static IEnumerator CalculatePaths(object _threadInfo)
	{
		AstarPath.<CalculatePaths>c__Iterator5 <CalculatePaths>c__Iterator = new AstarPath.<CalculatePaths>c__Iterator5();
		<CalculatePaths>c__Iterator._threadInfo = _threadInfo;
		<CalculatePaths>c__Iterator.<$>_threadInfo = _threadInfo;
		return <CalculatePaths>c__Iterator;
	}

	public NNInfo GetNearest(Vector3 position)
	{
		return this.GetNearest(position, NNConstraint.None);
	}

	public NNInfo GetNearest(Vector3 position, NNConstraint constraint)
	{
		return this.GetNearest(position, constraint, null);
	}

	public NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
	{
		if (this.graphs == null)
		{
			return default(NNInfo);
		}
		float num = float.PositiveInfinity;
		NNInfo result = default(NNInfo);
		int num2 = -1;
		for (int i = 0; i < this.graphs.Length; i++)
		{
			NavGraph navGraph = this.graphs[i];
			if (navGraph != null)
			{
				if (constraint.SuitableGraph(i, navGraph))
				{
					NNInfo nNInfo;
					if (this.fullGetNearestSearch)
					{
						nNInfo = navGraph.GetNearestForce(position, constraint);
					}
					else
					{
						nNInfo = navGraph.GetNearest(position, constraint);
					}
					if (nNInfo.node != null)
					{
						float magnitude = (nNInfo.clampedPosition - position).magnitude;
						if (this.prioritizeGraphs && magnitude < this.prioritizeGraphsLimit)
						{
							result = nNInfo;
							num2 = i;
							break;
						}
						if (magnitude < num)
						{
							num = magnitude;
							result = nNInfo;
							num2 = i;
						}
					}
				}
			}
		}
		if (num2 == -1)
		{
			return result;
		}
		if (result.constrainedNode != null)
		{
			result.node = result.constrainedNode;
			result.clampedPosition = result.constClampedPosition;
		}
		if (!this.fullGetNearestSearch && result.node != null && !constraint.Suitable(result.node))
		{
			NNInfo nearestForce = this.graphs[num2].GetNearestForce(position, constraint);
			if (nearestForce.node != null)
			{
				result = nearestForce;
			}
		}
		if (!constraint.Suitable(result.node) || (constraint.constrainDistance && (result.clampedPosition - position).sqrMagnitude > this.maxNearestNodeDistanceSqr))
		{
			return default(NNInfo);
		}
		return result;
	}

	public GraphNode GetNearest(Ray ray)
	{
		if (this.graphs == null)
		{
			return null;
		}
		float minDist = float.PositiveInfinity;
		GraphNode nearestNode = null;
		Vector3 lineDirection = ray.direction;
		Vector3 lineOrigin = ray.origin;
		for (int i = 0; i < this.graphs.Length; i++)
		{
			NavGraph navGraph = this.graphs[i];
			navGraph.GetNodes(delegate(GraphNode node)
			{
				Vector3 vector = (Vector3)node.position;
				Vector3 a = lineOrigin + Vector3.Dot(vector - lineOrigin, lineDirection) * lineDirection;
				float num = Mathf.Abs(a.x - vector.x);
				num *= num;
				if (num > minDist)
				{
					return true;
				}
				num = Mathf.Abs(a.z - vector.z);
				num *= num;
				if (num > minDist)
				{
					return true;
				}
				float sqrMagnitude = (a - vector).sqrMagnitude;
				if (sqrMagnitude < minDist)
				{
					minDist = sqrMagnitude;
					nearestNode = node;
				}
				return true;
			});
		}
		return nearestNode;
	}
}
