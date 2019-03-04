using System;
using UnityEngine;

internal static class DataCompressor
{
	public enum CompressionType
	{
		MachinePosition = 768,
		MachineRotation = 4,
		MachineMaxSizeWorldSpace = 0x20,
		NumberBaseSegments = 4,
		FireMissRange = 0x8000,
		NormalRange = 0xFF
	}

	internal struct V3Compressed
	{
		public short x;

		public short y;

		public short z;
	}

	internal struct Q3Compressed
	{
		public short x;

		public short y;

		public short z;
	}

	public static byte[] SerialiseBool(bool data)
	{
		return new byte[1]
		{
			(byte)(data ? 1 : 0)
		};
	}

	public static bool DeserialiseBool(byte data)
	{
		return data == 1;
	}

	public static short CompressFloat(float data, CompressionType range)
	{
		return (short)Mathf.Round(data * 32767f / (float)range);
	}

	public static float DecompressFloat(short data, CompressionType range)
	{
		return (float)data * (float)range / 32767f;
	}

	public static byte CompressFloatAsByte(float data, CompressionType range)
	{
		return (byte)Mathf.Round(data * 255f / (float)range);
	}

	public static float DecompressFloatAsByte(byte data, CompressionType range)
	{
		return (float)(int)data * (float)range / 255f;
	}

	public static byte[] CompressFloat(float data, int range, int numBytes)
	{
		int num = numBytes << 3;
		int num2 = (1 << num - 1) - 1;
		int num3 = (int)Math.Round(data * (float)num2 / (float)range, 0, MidpointRounding.AwayFromZero);
		byte[] array = new byte[numBytes];
		for (int i = 0; i < numBytes; i++)
		{
			array[i] = (byte)(num3 >> (i << 3));
		}
		return array;
	}

	public static float DecompressFloat(byte[] data, int range)
	{
		int num = data.Length;
		int num2 = num << 3;
		int num3 = (1 << num2 - 1) - 1;
		int num4 = 0;
		for (int i = 0; i < num; i++)
		{
			num4 |= data[i] << (i << 3);
		}
		return (float)num4 * (float)range / (float)num3;
	}

	public static void SerialiseVector3(ref BitStream stream, Vector3 vector3, CompressionType range)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		V3Compressed v3Compressed = CompressVector3(vector3, range);
		stream.Serialize(ref v3Compressed.x);
		stream.Serialize(ref v3Compressed.y);
		stream.Serialize(ref v3Compressed.z);
	}

	public static Vector3 DeserialiseVector3(ref BitStream stream, CompressionType range)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		V3Compressed data = default(V3Compressed);
		stream.Serialize(ref data.x);
		stream.Serialize(ref data.y);
		stream.Serialize(ref data.z);
		return DecompressVector3(data, range);
	}

	public static void SerialiseQuaternion(ref BitStream stream, Quaternion quat, CompressionType range)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Q3Compressed q3Compressed = CompressQuaternion(quat, range);
		stream.Serialize(ref q3Compressed.x);
		stream.Serialize(ref q3Compressed.y);
		stream.Serialize(ref q3Compressed.z);
	}

	public static Quaternion DeserialiseQuaternion(ref BitStream stream, CompressionType range)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Q3Compressed data = default(Q3Compressed);
		stream.Serialize(ref data.x);
		stream.Serialize(ref data.y);
		stream.Serialize(ref data.z);
		return DecompressQuaternion(data, range);
	}

	public static Q3Compressed CompressQuaternion(Quaternion data, CompressionType range)
	{
		Q3Compressed result = default(Q3Compressed);
		result.x = CompressFloat(data.x, range);
		result.y = CompressFloat(data.y, range);
		result.z = CompressFloat(data.z, range);
		if (data.w < 0f)
		{
			result.x *= -1;
			result.y *= -1;
			result.z *= -1;
		}
		return result;
	}

	public static Quaternion DecompressQuaternion(Q3Compressed data, CompressionType range)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Quaternion result = default(Quaternion);
		result.x = DecompressFloat(data.x, range);
		result.y = DecompressFloat(data.y, range);
		result.z = DecompressFloat(data.z, range);
		result.w = result.x * result.x + result.y * result.y + result.z * result.z;
		result.w = Mathf.Clamp01(result.w);
		result.w = 1f - result.w;
		result.w = Mathf.Sqrt(result.w);
		return result;
	}

	public static V3Compressed CompressVector3(Vector3 data, CompressionType range)
	{
		V3Compressed result = default(V3Compressed);
		result.x = CompressFloat(data.x, range);
		result.y = CompressFloat(data.y, range);
		result.z = CompressFloat(data.z, range);
		return result;
	}

	public static Vector3 DecompressVector3(V3Compressed data, CompressionType range)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = default(Vector3);
		result.x = DecompressFloat(data.x, range);
		result.y = DecompressFloat(data.y, range);
		result.z = DecompressFloat(data.z, range);
		return result;
	}
}
