using System;
using System.Collections.Generic;

[Serializable]
internal struct CubeTypeID : IEquatable<CubeTypeID>, IEqualityComparer<CubeTypeID>
{
	public static uint StandardCubeID = 227205318u;

	public uint ID;

	public CubeTypeID(uint id)
	{
		ID = id;
	}

	public static explicit operator uint(CubeTypeID id)
	{
		return id.ID;
	}

	public static implicit operator CubeTypeID(uint id)
	{
		return new CubeTypeID(id);
	}

	public static bool operator !=(CubeTypeID a, CubeTypeID b)
	{
		return a.ID != b.ID;
	}

	public static bool operator !=(CubeTypeID a, uint b)
	{
		return a.ID != b;
	}

	public static bool operator ==(CubeTypeID a, CubeTypeID b)
	{
		return a.ID == b.ID;
	}

	public static bool operator ==(CubeTypeID a, uint b)
	{
		return a.ID == b;
	}

	public override bool Equals(object mys)
	{
		if (!(mys is CubeTypeID))
		{
			return false;
		}
		CubeTypeID cubeTypeID = (CubeTypeID)mys;
		return cubeTypeID.ID == ID;
	}

	public bool Equals(CubeTypeID mys)
	{
		return mys.ID == ID;
	}

	public bool Equals(CubeTypeID x, CubeTypeID y)
	{
		return x.ID == y.ID;
	}

	public override int GetHashCode()
	{
		return (int)ID;
	}

	public int GetHashCode(CubeTypeID obj)
	{
		return (int)ID;
	}

	public override string ToString()
	{
		return ID.ToString();
	}

	public string ToHexString()
	{
		return ID.ToString("x");
	}
}
