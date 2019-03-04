using Svelto.DataStructures;
using System.Collections.Generic;

internal sealed class MachineDTO
{
	private FasterList<CubeData> _cubes;

	private Dictionary<Byte3, CubeData> _cubesDictionary;

	public int Count => _cubes.get_Count();

	public FasterReadOnlyList<CubeData> cubes => _cubes.AsReadOnly();

	public CubeData this[int index]
	{
		get
		{
			return _cubes.get_Item(index);
		}
	}

	public CubeData this[Byte3 gridLoc]
	{
		get
		{
			return _cubesDictionary[gridLoc];
		}
	}

	public MachineDTO()
	{
		_cubes = new FasterList<CubeData>();
		_cubesDictionary = new Dictionary<Byte3, CubeData>();
	}

	public bool Add(CubeData c)
	{
		if (_cubesDictionary.ContainsKey(c.gridLocation))
		{
			return false;
		}
		_cubes.Add(c);
		_cubesDictionary.Add(c.gridLocation, c);
		return true;
	}

	public void Remove(CubeData c)
	{
		_cubes.Remove(c);
		_cubesDictionary.Remove(c.gridLocation);
	}

	public void RemoveAt(int index)
	{
		_cubesDictionary.Remove(_cubes.get_Item(index).gridLocation);
		_cubes.RemoveAt(index);
	}

	public bool Contains(Byte3 gridLoc)
	{
		return _cubesDictionary.ContainsKey(gridLoc);
	}
}
