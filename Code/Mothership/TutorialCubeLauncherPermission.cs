using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal sealed class TutorialCubeLauncherPermission : CubeLauncherPermission
	{
		[Inject]
		internal ITutorialCubePlacementController tutorialCubePlacementController
		{
			private get;
			set;
		}

		public TutorialCubeLauncherPermission(InvalidPlacementObservable invalidPlacementObservable)
			: base(invalidPlacementObservable)
		{
		}

		public override bool FinalPlacementCheck(GhostCube ghostCube)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			CubeTypeID currentCube = ghostCube.currentCube;
			Vector3 position = ghostCube.finalGridPos.ToVector3();
			if (tutorialCubePlacementController.CheckIfPlacementIsAllowed(position, ghostCube.GetFinalCubeRotation(), currentCube))
			{
				return true;
			}
			return false;
		}

		public override bool CheckAndReportCanPlaceCube(GhostCube ghostCube)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			CubeTypeID currentCube = ghostCube.currentCube;
			Vector3 val = ghostCube.finalGridPos.ToVector3();
			if (tutorialCubePlacementController.CheckIfPlacementIsAllowed(val, ghostCube.GetFinalCubeRotation(), currentCube))
			{
				return base.CheckAndReportCanPlaceCube(ghostCube);
			}
			InvalidPlacementType placementNotAllowedReason = tutorialCubePlacementController.GetPlacementNotAllowedReason(val, ghostCube.GetFinalCubeRotation(), currentCube);
			_invalidPlacementObservable.Dispatch(ref placementNotAllowedReason);
			return false;
		}
	}
}
