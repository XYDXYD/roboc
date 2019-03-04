using UnityEngine;

public struct Int3
{
	public static Int3 zero = new Int3(0, 0, 0);

	public static Int3 forward = new Int3(0, 0, 1);

	public static Int3 right = new Int3(1, 0, 0);

	public static Int3 up = new Int3(0, 1, 0);

	public static Int3 back = new Int3(0, 0, -1);

	public static Int3 left = new Int3(-1, 0, 0);

	public static Int3 down = new Int3(0, -1, 0);

	public static Int3 one = new Int3(1, 1, 1);

	public int x;

	public int y;

	public int z;

	public int this[int key]
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

	public Int3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Int3(Vector3 vec)
	{
		x = Mathf.RoundToInt(vec.x);
		y = Mathf.RoundToInt(vec.y);
		z = Mathf.RoundToInt(vec.z);
	}

	public Int3(float x, float y, float z)
	{
		this.x = Mathf.RoundToInt(x);
		this.y = Mathf.RoundToInt(y);
		this.z = Mathf.RoundToInt(z);
	}

	public Int3(Byte3 vec)
	{
		x = vec.x;
		y = vec.y;
		z = vec.z;
	}

	public static bool operator ==(Int3 a, Int3 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator ==(Int3 a, Byte3 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator ==(Byte3 a, Int3 b)
	{
		return a.x == b.x && a.y == b.y && a.z == b.z;
	}

	public static bool operator !=(Int3 a, Int3 b)
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}

	public static bool operator !=(Int3 a, Byte3 b)
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}

	public static bool operator !=(Byte3 a, Int3 b)
	{
		return a.x != b.x || a.y != b.y || a.z != b.z;
	}

	public override bool Equals(object o)
	{
		try
		{
			return this == (Int3)o;
		}
		catch
		{
			return false;
		}
	}

	public override int GetHashCode()
	{
		return (x & 0x3FF) | ((y & 0x3FF) << 10) | ((z & 0x3FF) << 20);
	}

	public static Int3 operator *(Int3 c1, int multiplier)
	{
		return new Int3(c1.x * multiplier, c1.y * multiplier, c1.z * multiplier);
	}

	public static Int3 operator /(Int3 c1, int multiplier)
	{
		return new Int3(c1.x / multiplier, c1.y / multiplier, c1.z / multiplier);
	}

	public static Int3 operator +(Int3 c1, Int3 c2)
	{
		return new Int3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
	}

	public static Int3 operator -(Int3 c1, Int3 c2)
	{
		return new Int3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
	}

	public static Int3 operator -(Int3 c1)
	{
		return new Int3(-c1.x, -c1.y, -c1.z);
	}

	public Int3 Substract(Vector3 c2)
	{
		return new Int3(x - Mathf.RoundToInt(c2.x), y - Mathf.RoundToInt(c2.y), z - Mathf.RoundToInt(c2.z));
	}

	public Int3 Add(Vector3 c2)
	{
		return new Int3(x + Mathf.RoundToInt(c2.x), y + Mathf.RoundToInt(c2.y), z + Mathf.RoundToInt(c2.z));
	}

	public Vector3 ToVector3()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)x, (float)y, (float)z);
	}

	public static explicit operator Int3(Byte3 b)
	{
		return new Int3(b);
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}
}
