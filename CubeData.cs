using System;
using UnityEngine;
using Utility;

internal sealed class CubeData
{
	public uint iID;

	public Byte3 gridLocation;

	public byte rotationIndex;

	public byte paletteIndex;

	public bool isDestroyed;

	public uint health;

	public static Quaternion[] sQuatList = (Quaternion[])new Quaternion[24]
	{
		new Quaternion(0f, 0f, 0f, 1f),
		new Quaternion(0f, 0f, 0.7071068f, 0.7071068f),
		new Quaternion(0f, 0f, 1f, 0f),
		new Quaternion(0f, 0f, 0.7071068f, -0.7071068f),
		new Quaternion(0f, 0.7071068f, 0f, 0.7071068f),
		new Quaternion(0.5f, 0.5f, 0.5f, 0.5f),
		new Quaternion(-0.5f, -0.5f, -0.5f, 0.5f),
		new Quaternion(0.5f, -0.5f, 0.5f, -0.5f),
		new Quaternion(0.5f, 0.5f, -0.5f, -0.5f),
		new Quaternion(0.5f, -0.5f, -0.5f, 0.5f),
		new Quaternion(0.5f, -0.5f, 0.5f, 0.5f),
		new Quaternion(-0.5f, -0.5f, 0.5f, -0.5f),
		new Quaternion(0.5f, -0.5f, -0.5f, -0.5f),
		new Quaternion(0.7071068f, 0f, 0.7071068f, 0f),
		new Quaternion(0f, 1f, 0f, 0f),
		new Quaternion(0.7071068f, 0.7071068f, 0f, 0f),
		new Quaternion(1f, 0f, 0f, 0f),
		new Quaternion(0.7071068f, -0.7071068f, 0f, 0f),
		new Quaternion(0f, 0.7071068f, 0f, -0.7071068f),
		new Quaternion(0.7071068f, 0f, -0.7071068f, 0f),
		new Quaternion(0.7071068f, 0f, 0f, 0.7071068f),
		new Quaternion(0f, -0.7071068f, 0.7071068f, 0f),
		new Quaternion(0.7071068f, 0f, 0f, -0.7071068f),
		new Quaternion(0f, -0.7071068f, -0.7071068f, 0f)
	};

	public CubeData()
	{
	}

	public CubeData(CubeData other)
	{
		iID = other.iID;
		gridLocation = other.gridLocation;
		rotationIndex = other.rotationIndex;
		paletteIndex = other.paletteIndex;
		isDestroyed = other.isDestroyed;
		health = other.health;
	}

	public static int QuatToIndex(Quaternion q)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Quaternion[] array = sQuatList;
		foreach (Quaternion val in array)
		{
			float num2 = Quaternion.Angle(val, q);
			if ((double)num2 < 0.1)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public static Quaternion IndexToQuat(int i)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (i < 0 || i >= 24)
		{
			Console.LogException(new Exception("Reading invalid rotation index"));
			return Quaternion.get_identity();
		}
		Quaternion val = default(Quaternion);
		return sQuatList[i];
	}

	public static Quaternion FindClosestQuat(Quaternion q)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Quaternion result = q;
		float num = 50000f;
		Quaternion[] array = sQuatList;
		foreach (Quaternion val in array)
		{
			float num2 = Quaternion.Angle(val, q);
			if (num2 < num)
			{
				num = num2;
				result = val;
			}
		}
		return result;
	}
}
