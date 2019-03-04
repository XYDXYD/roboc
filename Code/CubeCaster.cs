internal sealed class CubeCaster : ICubeCaster
{
	private bool _builderCanPlaceCubeAtGridPos;

	private bool _adjacentToSuitableCube;

	public bool canPlace => _builderCanPlaceCubeAtGridPos && !ghostIntersectsCubes && !ghostIntersectsGhost && !cubeIntersectionsUnreliable && !changingCube;

	public bool isAdjacentSuitableCube => _adjacentToSuitableCube;

	public bool ghostIntersectsFloor
	{
		get;
		set;
	}

	public bool outSideTheGrid
	{
		get;
		private set;
	}

	public bool axisAlligned
	{
		get;
		private set;
	}

	public Int3 displacement
	{
		get;
		private set;
	}

	public bool changingCube
	{
		get;
		set;
	}

	public bool ghostIntersectsCubes
	{
		get;
		set;
	}

	public bool ghostIntersectsGhost
	{
		get;
		set;
	}

	public bool cubeIntersectionsUnreliable
	{
		get;
		set;
	}

	public bool requiresMirrorMode
	{
		get;
		set;
	}

	public void UpdateCaster(bool pBuilderCanPlaceCubeAtGridPos, bool pAxisAlligned, bool pOutSideTheGrid, bool adjacentToSuitableCube)
	{
		_builderCanPlaceCubeAtGridPos = pBuilderCanPlaceCubeAtGridPos;
		axisAlligned = pAxisAlligned;
		outSideTheGrid = pOutSideTheGrid;
		_adjacentToSuitableCube = adjacentToSuitableCube;
	}

	public void SetGhostIntersects(bool cubes, bool floor, bool ghost)
	{
		ghostIntersectsCubes = cubes;
		ghostIntersectsFloor = floor;
		ghostIntersectsGhost = ghost;
	}

	public void SetRequiredDisplacement(Int3 disp)
	{
		displacement = disp;
	}
}
