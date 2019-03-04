using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal interface IClusteredColliderNode
	{
		Bounds bounds
		{
			get;
		}

		List<DualCubeEdge> edges
		{
			get;
		}

		double error
		{
			get;
		}

		bool isEnabled
		{
			get;
			set;
		}

		bool isLeaf
		{
			get;
		}

		bool isSingularity
		{
			get;
		}

		IClusteredColliderNode left
		{
			get;
		}

		float mergedVolume
		{
			get;
		}

		ColliderNode parent
		{
			get;
			set;
		}

		IClusteredColliderNode right
		{
			get;
		}

		void ClearEdges();
	}
}
