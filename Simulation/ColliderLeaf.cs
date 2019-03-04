using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class ColliderLeaf : IClusteredColliderNode
	{
		private Bounds _bounds;

		private bool _initialized;

		public bool currentShown
		{
			get;
			set;
		}

		public Bounds bounds => _bounds;

		public List<DualCubeEdge> edges
		{
			get;
			private set;
		}

		public double error => 1.0;

		public bool isEnabled
		{
			get;
			set;
		}

		public bool isLeaf => true;

		public bool isSingularity
		{
			get;
			private set;
		}

		public IClusteredColliderNode left => null;

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

		public IClusteredColliderNode right => null;

		public ColliderLeaf(CubeColliderInfo[] collidersInfo)
		{
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			isEnabled = true;
			currentShown = false;
			isSingularity = false;
			edges = new List<DualCubeEdge>();
			foreach (CubeColliderInfo cubeColliderInfo in collidersInfo)
			{
				if (cubeColliderInfo.isSingularity)
				{
					isSingularity = true;
				}
				if (!cubeColliderInfo.isTrigger && !cubeColliderInfo.doNotCluster && !cubeColliderInfo.canNotBeHit)
				{
					if (!_initialized)
					{
						_initialized = true;
						_bounds = new Bounds(cubeColliderInfo.bounds.get_center(), cubeColliderInfo.bounds.get_size());
					}
					else
					{
						_bounds.Encapsulate(cubeColliderInfo.bounds);
					}
				}
			}
			if (!_initialized)
			{
				isSingularity = true;
			}
			Vector3 size = _bounds.get_size();
			mergedVolume = size.x * size.y * size.z;
		}

		public void ClearEdges()
		{
			edges = null;
		}

		public void ComputeErrorAndVolume()
		{
		}

		public ColliderNode DisableChild(IClusteredColliderNode child)
		{
			throw new Exception("Leaves don't have childs");
		}

		public void EnableChild(IClusteredColliderNode child)
		{
			throw new Exception("Leaves don't have childs");
		}

		public void AddEdge(DualCubeEdge edge)
		{
			edges.Add(edge);
		}
	}
}
