using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class TutorialGhostCubeVisibilityChecker : GhostCubeVisibilityChecker
	{
		[Inject]
		internal ITutorialCubePlacementController tutorialCubePlacementController
		{
			private get;
			set;
		}

		public override bool CheckVisibility(Vector3 gridLocation, Quaternion ghostOrientation, CubeTypeID cubeType)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			return base.CheckVisibility(gridLocation, ghostOrientation, cubeType);
		}
	}
}
