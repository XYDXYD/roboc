using UnityEngine;

internal sealed class MachineCell
{
	private Byte3 _pos;

	public GameObject gameObject
	{
		get;
		set;
	}

	public Byte3 pos => _pos;

	public InstantiatedCube info
	{
		get;
		private set;
	}

	public bool centreCell
	{
		get;
		private set;
	}

	public int x
	{
		get
		{
			Byte3 pos = this.pos;
			return pos.x;
		}
	}

	public int y
	{
		get
		{
			Byte3 pos = this.pos;
			return pos.y;
		}
	}

	public int z
	{
		get
		{
			Byte3 pos = this.pos;
			return pos.z;
		}
	}

	public MachineCell(GameObject go, InstantiatedCube cubeInfo, Byte3 pos, bool centre)
	{
		gameObject = go;
		_pos = pos;
		centreCell = centre;
		info = cubeInfo;
	}

	public bool IsCentreCell()
	{
		return gameObject != null && centreCell;
	}
}
