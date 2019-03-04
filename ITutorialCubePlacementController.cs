using UnityEngine;

internal interface ITutorialCubePlacementController
{
	void AddCubeLocationsToPlace(Vector3 position, Quaternion orientation, CubeTypeID cubeTypeID, bool anyOrientationPermitted);

	bool CheckIfPlacementIsAllowed(Vector3 position, Quaternion orientation, CubeTypeID cubeTypeID);

	InvalidPlacementType GetPlacementNotAllowedReason(Vector3 gridPosition, Quaternion orientation, CubeTypeID cubeTypeID);

	bool CheckIfAllCubeLocationsHaveBeenFilled();

	void ResetList();
}
