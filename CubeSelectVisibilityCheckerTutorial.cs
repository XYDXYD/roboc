using Svelto.IoC;

internal sealed class CubeSelectVisibilityCheckerTutorial : ICubeSelectVisibilityChecker
{
	[Inject]
	internal ICubeList cubeList
	{
		private get;
		set;
	}

	public bool ShouldCubeBeVisibleInDepot(CubeTypeID type, CubeCategory cubeCategory)
	{
		if (cubeList.CubeTypeDataOf(type).active && cubeList.CubeTypeDataOf(type).cubeData.category == cubeCategory && (cubeList.GetBuildModeVisibility(type) == BuildVisibility.Tutorial || cubeList.GetBuildModeVisibility(type) == BuildVisibility.All))
		{
			return true;
		}
		return false;
	}
}
