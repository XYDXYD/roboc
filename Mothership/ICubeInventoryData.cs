namespace Mothership
{
	internal interface ICubeInventoryData
	{
		bool HasLoadedAllData
		{
			get;
		}

		int GetRobitsCostToUnlockType(CubeTypeID cubeTypeId);

		int GetGCCostToUnlockType(CubeTypeID cubeTypeId);
	}
}
