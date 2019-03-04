using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Simulation
{
	internal sealed class ColliderNode : IClusteredColliderNode
	{
		private Bounds _bounds;

		private float _boundsVolume;

		private double _distance;

		private MachineBounds _machineBounds;

		private int _checkNumber;

		public Bounds bounds => _bounds;

		public List<DualCubeEdge> edges
		{
			get;
			private set;
		}

		public double error
		{
			get;
			private set;
		}

		public bool isEnabled
		{
			get;
			set;
		}

		public bool isLeaf => false;

		public bool isSingularity => false;

		public IClusteredColliderNode left
		{
			get;
			private set;
		}

		public float mergedVolume
		{
			get;
			private set;
		}

		public ColliderNode parent
		{
			get;
			set;
		}

		public IClusteredColliderNode right
		{
			get;
			private set;
		}

		public ColliderNode(DualCubeEdge edge, Action<DualCubeEdge> pushEdge, Action<DualCubeEdge> popEdge)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			isEnabled = true;
			edges = new List<DualCubeEdge>();
			_machineBounds = edge.machineBounds;
			_bounds = edge.mergedBounds;
			error = edge.error;
			IClusteredColliderNode a = edge.A;
			IClusteredColliderNode b = edge.B;
			left = a;
			right = b;
			mergedVolume = a.mergedVolume + b.mergedVolume;
			_boundsVolume = edge.boundsVolume;
			_distance = edge.distanceCentersSQR;
			a.parent = this;
			b.parent = this;
			UpdateCubeEdges(edge, popEdge, a);
			UpdateCubeEdges(edge, popEdge, b);
			RemoveDuplicateEdges();
			List<DualCubeEdge>.Enumerator enumerator = edges.GetEnumerator();
			while (enumerator.MoveNext())
			{
				pushEdge(enumerator.Current);
			}
			a.ClearEdges();
			b.ClearEdges();
		}

		public void ClearEdges()
		{
			edges = null;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		public void ComputeErrorAndVolume()
		{
			if (left.isEnabled || right.isEnabled)
			{
				mergedVolume = 0f;
				if (left.isEnabled)
				{
					mergedVolume += left.mergedVolume;
				}
				if (right.isEnabled)
				{
					mergedVolume += right.mergedVolume;
				}
				if (left.isSingularity || right.isSingularity)
				{
					error = 3.4028234663852886E+38;
				}
				else
				{
					error = DualCubeEdge.EvaluateError(_machineBounds, mergedVolume, _boundsVolume, _distance);
				}
			}
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		public void EnableChild(IClusteredColliderNode child)
		{
			ColliderNode colliderNode = this;
			child.isEnabled = true;
			do
			{
				colliderNode.isEnabled = true;
				colliderNode = colliderNode.parent;
			}
			while (colliderNode != null);
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		public ColliderNode DisableChild(IClusteredColliderNode child)
		{
			ColliderNode colliderNode = this;
			child.isEnabled = false;
			while (colliderNode != null && colliderNode.isEnabled && !colliderNode.left.isEnabled && !colliderNode.right.isEnabled)
			{
				colliderNode.isEnabled = false;
				colliderNode = colliderNode.parent;
			}
			return colliderNode;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		public void RecomputeError(int numberToCheck)
		{
			ColliderNode colliderNode = this;
			while (colliderNode != null && colliderNode.isEnabled && colliderNode._checkNumber != numberToCheck)
			{
				colliderNode.ComputeErrorAndVolume();
				colliderNode._checkNumber = numberToCheck;
				colliderNode = colliderNode.parent;
			}
		}

		private void RemoveDuplicateEdges()
		{
			for (int i = 0; i < edges.Count; i++)
			{
				DualCubeEdge dualCubeEdge = edges[i];
				for (int j = i + 1; j < edges.Count; j++)
				{
					DualCubeEdge dualCubeEdge2 = edges[j];
					if (dualCubeEdge.ID != dualCubeEdge2.ID && ((dualCubeEdge2.A == dualCubeEdge.A && dualCubeEdge2.B == dualCubeEdge.B) || (dualCubeEdge2.A == dualCubeEdge.B && dualCubeEdge2.B == dualCubeEdge.A)))
					{
						edges.UnorderredListRemoveAt(j--);
						if (!dualCubeEdge2.B.edges.Remove(dualCubeEdge2))
						{
							throw new Exception("duplicated edge not found among the neighbour edges");
						}
					}
				}
			}
		}

		private void UpdateCubeEdges(DualCubeEdge justClusteredEdge, Action<DualCubeEdge> popEdgeFromPriorityList, IClusteredColliderNode nodeToCheck)
		{
			List<DualCubeEdge> edges = nodeToCheck.edges;
			for (int i = 0; i < edges.Count; i++)
			{
				DualCubeEdge dualCubeEdge = edges[i];
				if (dualCubeEdge != justClusteredEdge)
				{
					popEdgeFromPriorityList(dualCubeEdge);
					if (dualCubeEdge.A == nodeToCheck)
					{
						dualCubeEdge.ChangeChildren(this, dualCubeEdge.B);
					}
					else
					{
						dualCubeEdge.ChangeChildren(this, dualCubeEdge.A);
					}
					this.edges.Add(dualCubeEdge);
				}
			}
		}
	}
}
