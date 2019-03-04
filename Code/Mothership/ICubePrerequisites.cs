namespace Mothership
{
	internal interface ICubePrerequisites
	{
		bool CanUseCube(CubeTypeID cubeId, out string errorStringKey);

		bool CanUseCube(CubeTypeID cubeId);
	}
}
