namespace Mothership
{
	internal interface ICubesData
	{
		bool isReady
		{
			get;
		}

		ForgeItemAppearance GetForgeItemTypeAppearance(CubeTypeID cubeTypeId);

		bool GetIsCosmetic(CubeTypeID cubeTypeId);

		bool IsLeagueBadge(CubeTypeID cubetypeID);
	}
}
