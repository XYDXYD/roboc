using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class GetTeamBaseDependency : NetworkDependency
{
	private const int NUM_TEAMS = 2;

	public Vector3[] positions = (Vector3[])new Vector3[2];

	public Quaternion[] rotations = (Quaternion[])new Quaternion[2];

	public int protoniumCubeHealth;

	public GetTeamBaseDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(positions[0], DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(rotations[0], DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				v3Compressed = DataCompressor.CompressVector3(positions[1], DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				q3Compressed = DataCompressor.CompressQuaternion(rotations[1], DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				binaryWriter.Write(protoniumCubeHealth);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				positions[0] = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				rotations[0] = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				data2 = default(DataCompressor.V3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				positions[1] = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				data3 = default(DataCompressor.Q3Compressed);
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				rotations[1] = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				protoniumCubeHealth = binaryReader.ReadInt32();
			}
		}
	}
}
