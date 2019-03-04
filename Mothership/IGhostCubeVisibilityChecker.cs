using UnityEngine;

namespace Mothership
{
	internal interface IGhostCubeVisibilityChecker
	{
		bool CheckVisibility(Vector3 gridLocation, Quaternion ghostOrientation, CubeTypeID cubeType);
	}
}
