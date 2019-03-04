using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal sealed class ExistingMachineDefinitionDependency : NetworkDependency
{
	public Dictionary<uint, int> damagedCubes = new Dictionary<uint, int>();

	public int owner;

	public int teamId;

	public Vector3 position;

	public Quaternion rotation;

	public string playerName;

	public int killCount;

	public bool isAlive;

	public ExistingMachineDefinitionDependency(byte[] data)
		: base(data)
	{
	}

	public ExistingMachineDefinitionDependency(Dictionary<uint, int> _damagedCubes, int _owner, int _teamId, Vector3 _position, Quaternion _rotation, string _playerName, int _killCount, bool isAlive)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		damagedCubes = _damagedCubes;
		owner = _owner;
		teamId = _teamId;
		playerName = _playerName;
		killCount = _killCount;
		position = _position;
		rotation = _rotation;
		this.isAlive = isAlive;
	}

	public override byte[] Serialise()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)owner);
				binaryWriter.Write((byte)teamId);
				binaryWriter.Write(playerName);
				binaryWriter.Write(killCount);
				binaryWriter.Write(isAlive);
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(position, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(rotation, DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				owner = binaryReader.ReadByte();
				teamId = binaryReader.ReadByte();
				playerName = binaryReader.ReadString();
				killCount = binaryReader.ReadInt32();
				isAlive = binaryReader.ReadBoolean();
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
			}
		}
	}
}
