using UnityEngine;

namespace Mothership
{
	internal class GhostCubeVisibilityChecker : IGhostCubeVisibilityChecker
	{
		public virtual bool CheckVisibility(Vector3 gridLocation, Quaternion ghostOrientation, CubeTypeID cubeType)
		{
			return true;
		}
	}
}
