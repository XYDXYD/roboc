using UnityEngine;

public struct Byte3
{
	public static Byte3 zero = new Byte3(0, 0, 0);

	public static Byte3 one = new Byte3(1, 1, 1);

	public static Byte3 MaxValue = new Byte3(byte.MaxValue, byte.MaxValue, byte.MaxValue);

	public static Byte3 MinValue = new Byte3(0, 0, 0);

	public byte x;

	public byte y;

	public byte z;

	public byte this[int key]
	{
		get
		{
			switch (key)
			{
			case 0:
				return x;
			case 1:
				return y;
			default:
				return z;
			}
		}
		set
		{
			switch (key)
			{
			case 0:
				x = value;
				break;
			case 1:
				y = value;
				break;
			case 2:
				z = value;
				break;
			}
		}
	}

	public Byte3(byte x, byte y, byte z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Byte3(Vector3 vec)
	{
		x = (byte)Mathf.RoundToInt(vec.x);
		y = (byte)Mathf.RoundToInt(vec.y);
		z = (byte)Mathf.RoundToInt(vec.z);
	}

	public Byte3(Int3 vec)
	{
		x = (byte)vec.x;
		y = (byte)vec.y;
		z = (byte)vec.z;
	}

	public static bool operator ==(Byte3 a, Byte3 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(Byte3 a, Byte3 b)
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}

	public override bool Equals(object o)
	{
		try
		{
			return this == (Byte3)o;
		}
		catch
		{
			return false;
		}
	}

	public static uint GridKey(byte x, byte y, byte z)
	{
		return (uint)((x & 0x3FF) | ((y & 0x3FF) << 10) | ((z & 0x3FF) << 20));
	}

	public static Byte3 KeyGrid(uint key)
	{
		return new Byte3((byte)(key & 0x3FF), (byte)((key >> 10) & 0x3FF), (byte)((key >> 20) & 0x3FF));
	}

	public static Byte3 KeyGrid(int key)
	{
		return new Byte3((byte)(key & 0x3FF), (byte)((key >> 10) & 0x3FF), (byte)((key >> 20) & 0x3FF));
	}

	public uint GridKey()
	{
		return GridKey(x, y, z);
	}

	public override int GetHashCode()
	{
		return (int)GridKey(x, y, z);
	}

	public static Byte3 operator *(Byte3 c1, byte multiplier)
	{
		return new Byte3((byte)(c1.x * multiplier), (byte)(c1.y * multiplier), (byte)(c1.z * multiplier));
	}

	public static Byte3 operator /(Byte3 c1, byte multiplier)
	{
		return new Byte3((byte)((int)c1.x / (int)multiplier), (byte)((int)c1.y / (int)multiplier), (byte)((int)c1.z / (int)multiplier));
	}

	public static Byte3 operator +(Byte3 c1, Byte3 c2)
	{
		return new Byte3((byte)(c1.x + c2.x), (byte)(c1.y + c2.y), (byte)(c1.z + c2.z));
	}

	public static Byte3 operator -(Byte3 c1, Byte3 c2)
	{
		return new Byte3((byte)(c1.x - c2.x), (byte)(c1.y - c2.y), (byte)(c1.z - c2.z));
	}

	public static Byte3 operator +(Byte3 c1, Int3 c2)
	{
		return new Byte3((byte)(c1.x + c2.x), (byte)(c1.y + c2.y), (byte)(c1.z + c2.z));
	}

	public static Byte3 operator -(Byte3 c1, Int3 c2)
	{
		return new Byte3((byte)(c1.x - c2.x), (byte)(c1.y - c2.y), (byte)(c1.z - c2.z));
	}

	public Byte3 Substract(Vector3 c2)
	{
		return new Byte3((byte)(x - (byte)Mathf.RoundToInt(c2.x)), (byte)(y - (byte)Mathf.RoundToInt(c2.y)), (byte)(z - (byte)Mathf.RoundToInt(c2.z)));
	}

	public Byte3 Add(Vector3 c2)
	{
		return new Byte3((byte)(x + (byte)Mathf.RoundToInt(c2.x)), (byte)(y + (byte)Mathf.RoundToInt(c2.y)), (byte)(z + (byte)Mathf.RoundToInt(c2.z)));
	}

	public static explicit operator Byte3(Int3 b)
	{
		return new Byte3(b);
	}

	public Vector3 ToVector3()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)(int)x, (float)(int)y, (float)(int)z);
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}
}
