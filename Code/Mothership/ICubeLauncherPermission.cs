using System;

namespace Mothership
{
	internal interface ICubeLauncherPermission
	{
		event Action AttemptPlaceCubeOverLimit;

		event Action AttemptPlaceWeaponsOverLimit;

		bool CheckAndReportCanPlaceCube(GhostCube cube);

		bool CheckNonPlacementRestrictions(CubeTypeID selectedCubeID);

		bool FinalPlacementCheck(GhostCube ghostCube);
	}
}
