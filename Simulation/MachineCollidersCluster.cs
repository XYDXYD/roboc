using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class MachineCollidersCluster
	{
		public const string DO_NOT_CLUSTER = "MachineCollidersCluster.DoNotCluster";

		public const string IS_SINGULARITY = "MachineCollidersCluster.IsSingularity";

		public const string CAN_NOT_HIT = "MachineCollidersCluster.CanNotHit";

		private LeftLeaningKeyedRedBlackTree<DualCubeEdge> _edges;

		private Dictionary<int, ColliderLeaf> _leaves;

		private MachineBounds _machineBounds;

		private Dictionary<long, DualCubeEdge> _leavesEdges;

		private MachineClusterPool _pool;

		public MachineCollidersCluster(MachineClusterPool pool)
		{
			_edges = new LeftLeaningKeyedRedBlackTree<DualCubeEdge>();
			_leaves = new Dictionary<int, ColliderLeaf>();
			_leavesEdges = new Dictionary<long, DualCubeEdge>();
			_pool = pool;
			_machineBounds = new MachineBounds();
		}

		private long BuildKey(int A, int B)
		{
			long num = A;
			long num2 = B;
			if (A < B)
			{
				num = B;
				num2 = A;
			}
			return (num << 32) | num2;
		}

		public void AddLink(int nodeAHashCode, CubeColliderInfo[] nodeAColliders, int nodeBHashCode, CubeColliderInfo[] nodeBColliders)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			long key = BuildKey(nodeAHashCode, nodeBHashCode);
			if (!_leavesEdges.ContainsKey(key))
			{
				if (!_leaves.TryGetValue(nodeAHashCode, out ColliderLeaf value))
				{
					value = new ColliderLeaf(nodeAColliders);
					_machineBounds.bounds.Encapsulate(value.bounds);
					_leaves[nodeAHashCode] = value;
				}
				if (!_leaves.TryGetValue(nodeBHashCode, out ColliderLeaf value2))
				{
					value2 = new ColliderLeaf(nodeBColliders);
					_machineBounds.bounds.Encapsulate(value2.bounds);
					_leaves[nodeBHashCode] = value2;
				}
				DualCubeEdge dualCubeEdge = new DualCubeEdge(value, value2, _machineBounds);
				_leavesEdges[key] = dualCubeEdge;
				_edges.Add(dualCubeEdge);
				value.AddEdge(dualCubeEdge);
				value2.AddEdge(dualCubeEdge);
			}
		}

		private void ClusteringProcess(List<IClusteredColliderNode> roots)
		{
			_leavesEdges = null;
			while (_edges.Count > 0)
			{
				DualCubeEdge minimumKey = _edges.MinimumKey;
				_edges.Remove(minimumKey);
				ColliderNode colliderNode = new ColliderNode(minimumKey, delegate(DualCubeEdge edge)
				{
					_edges.Add(edge);
				}, delegate(DualCubeEdge edge)
				{
					if (!_edges.Remove(edge))
					{
						throw new Exception("Something went wrong during the clustering, an edge could not be found in the edge list");
					}
				});
				if (colliderNode.edges.Count == 0)
				{
					roots.Add(colliderNode);
				}
			}
		}

		public IEnumerator Cluster(MachineGraph machineGraph)
		{
			List<IClusteredColliderNode> list = new List<IClusteredColliderNode>();
			ClusteringProcess(list);
			machineGraph.cluster = new MachineCluster((list.Count != 1) ? null : list[0], _leaves, _pool);
			yield break;
		}
	}
}
