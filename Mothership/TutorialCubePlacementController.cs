using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership
{
	internal sealed class TutorialCubePlacementController : ITutorialCubePlacementController
	{
		internal class TutorialCubePlacementInfo
		{
			public readonly Int3 position;

			public readonly Quaternion orientation;

			public readonly CubeTypeID cubeTypeID;

			public bool anyOrientationPermitted;

			public TutorialCubePlacementInfo(Vector3 position_, Quaternion orientation_, CubeTypeID cubeType_, bool anyOrientationPermitted_)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				position = new Int3(position_);
				orientation = orientation_;
				cubeTypeID = cubeType_;
				anyOrientationPermitted = anyOrientationPermitted_;
			}
		}

		private List<TutorialCubePlacementInfo> tutorialCubePlacements = new List<TutorialCubePlacementInfo>();

		[Inject]
		internal IMachineMap machineMap
		{
			get;
			set;
		}

		public bool CheckIfAllCubeLocationsHaveBeenFilled()
		{
			foreach (TutorialCubePlacementInfo tutorialCubePlacement in tutorialCubePlacements)
			{
				if (!machineMap.IsCellTaken(tutorialCubePlacement.position))
				{
					return false;
				}
			}
			return true;
		}

		public bool CheckIfPlacementIsAllowed(Vector3 gridPosition, Quaternion orientation, CubeTypeID cubeTypeID)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			Int3 b = new Int3(gridPosition);
			foreach (TutorialCubePlacementInfo tutorialCubePlacement in tutorialCubePlacements)
			{
				if (tutorialCubePlacement.cubeTypeID == cubeTypeID && tutorialCubePlacement.position == b)
				{
					if (tutorialCubePlacement.anyOrientationPermitted)
					{
						return true;
					}
					if (orientation == tutorialCubePlacement.orientation)
					{
						return true;
					}
				}
			}
			return false;
		}

		public InvalidPlacementType GetPlacementNotAllowedReason(Vector3 gridPosition, Quaternion orientation, CubeTypeID cubeTypeID)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			Int3 b = new Int3(gridPosition);
			bool flag = false;
			foreach (TutorialCubePlacementInfo tutorialCubePlacement in tutorialCubePlacements)
			{
				if (tutorialCubePlacement.position == b)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return InvalidPlacementType.TutorialWrongGridLocation;
			}
			bool flag2 = true;
			foreach (TutorialCubePlacementInfo tutorialCubePlacement2 in tutorialCubePlacements)
			{
				if (tutorialCubePlacement2.position == b && tutorialCubePlacement2.cubeTypeID == cubeTypeID)
				{
					flag2 = false;
				}
			}
			if (flag2)
			{
				return InvalidPlacementType.TutorialWrongCubeType;
			}
			return InvalidPlacementType.TutorialWrongOrientation;
		}

		public void ResetList()
		{
			tutorialCubePlacements.Clear();
		}

		public void AddCubeLocationsToPlace(Vector3 position, Quaternion orientation, CubeTypeID cubeTypeID, bool anyOrientationPermitted)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			TutorialCubePlacementInfo item = new TutorialCubePlacementInfo(position, orientation, cubeTypeID, anyOrientationPermitted);
			tutorialCubePlacements.Add(item);
		}
	}
}
