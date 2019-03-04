public interface ICubeCaster
{
	bool canPlace
	{
		get;
	}

	bool isAdjacentSuitableCube
	{
		get;
	}

	bool ghostIntersectsFloor
	{
		get;
	}

	bool outSideTheGrid
	{
		get;
	}

	bool axisAlligned
	{
		get;
	}

	Int3 displacement
	{
		get;
	}

	bool changingCube
	{
		get;
		set;
	}

	bool requiresMirrorMode
	{
		get;
		set;
	}

	bool ghostIntersectsCubes
	{
		get;
	}

	bool ghostIntersectsGhost
	{
		get;
	}

	void SetGhostIntersects(bool cubes, bool floor, bool ghost);
}
