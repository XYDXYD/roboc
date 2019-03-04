using Simulation;
using UnityEngine;

internal class CubeColliderInfo
{
	public bool isTrigger;

	public bool isSingularity;

	public bool doNotCluster;

	public bool canNotBeHit;

	public IColliderData colliderData;

	public Bounds bounds;
}
