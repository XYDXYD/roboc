using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal class GetEqualizerDependency : NetworkDependency
{
	public Vector3 position;

	public Quaternion rotation;

	public int totalHealth;

	public GetEqualizerDependency(Vector3 pPosition, Quaternion pRotation, int pTotalHealth)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		position = pPosition;
		rotation = pRotation;
		totalHealth = pTotalHealth;
	}

	public GetEqualizerDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(position, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(rotation, DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				binaryWriter.Write(totalHealth);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				position = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				rotation = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				totalHealth = binaryReader.ReadInt32();
			}
		}
	}
}
