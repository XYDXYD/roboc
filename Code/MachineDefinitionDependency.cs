using RCNetwork.Events;
using System;
using System.IO;
using UnityEngine;

internal sealed class MachineDefinitionDependency : NetworkDependency
{
	public int owner;

	public int teamId;

	public Vector3 spawnPosition;

	public Quaternion spawnRotation;

	public int killCount;

	public Vector3 centreOfMass;

	public MachineDefinitionDependency(byte[] data)
		: base(data)
	{
	}

	public MachineDefinitionDependency(int _owner, int _teamId, Vector3 _spawnPosition, Quaternion _spawnRotation, int _killCount, Vector3 _centreOfMass)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		owner = _owner;
		teamId = _teamId;
		killCount = _killCount;
		spawnPosition = _spawnPosition;
		spawnRotation = _spawnRotation;
		centreOfMass = _centreOfMass;
	}

	public override byte[] Serialise()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			memoryStream.WriteByte((byte)owner);
			memoryStream.WriteByte((byte)teamId);
			memoryStream.Write(BitConverter.GetBytes(killCount), 0, 4);
			DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(spawnPosition, DataCompressor.CompressionType.MachinePosition);
			DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(spawnRotation, DataCompressor.CompressionType.MachineRotation);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed.x), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed.y), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed.z), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(q3Compressed.x), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(q3Compressed.y), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(q3Compressed.z), 0, 2);
			DataCompressor.V3Compressed v3Compressed2 = DataCompressor.CompressVector3(centreOfMass, DataCompressor.CompressionType.MachinePosition);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed2.x), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed2.y), 0, 2);
			memoryStream.Write(BitConverter.GetBytes(v3Compressed2.z), 0, 2);
			return memoryStream.ToArray();
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				owner = binaryReader.ReadByte();
				teamId = binaryReader.ReadByte();
				killCount = binaryReader.ReadInt32();
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				spawnPosition = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				spawnRotation = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				DataCompressor.V3Compressed v3Compressed = default(DataCompressor.V3Compressed);
				v3Compressed.x = binaryReader.ReadInt16();
				v3Compressed.y = binaryReader.ReadInt16();
				v3Compressed.z = binaryReader.ReadInt16();
				DataCompressor.V3Compressed data4 = v3Compressed;
				centreOfMass = DataCompressor.DecompressVector3(data4, DataCompressor.CompressionType.MachinePosition);
			}
		}
	}
}
