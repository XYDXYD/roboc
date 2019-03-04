using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class SpawnPointDependency : NetworkDependency
{
	public Vector3 position;

	public Quaternion rotation;

	public int owner;

	public Vector3 velocity;

	public Vector3 angularVelocity;

	public SpawnPointDependency(byte[] data)
		: base(data)
	{
	}

	public SpawnPointDependency(Vector3 _position, Quaternion _rotation, int _owner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		position = _position;
		rotation = _rotation;
		owner = _owner;
		velocity = Vector3.get_zero();
		angularVelocity = Vector3.get_zero();
	}

	public SpawnPointDependency(Vector3 _position, Quaternion _rotation, int _owner, Vector3 _velocity, Vector3 _angularVelocity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		position = _position;
		rotation = _rotation;
		owner = _owner;
		velocity = _velocity;
		angularVelocity = _angularVelocity;
	}

	public override byte[] Serialise()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(position, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(rotation, DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				binaryWriter.Write((byte)owner);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				position = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				rotation = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				owner = binaryReader.ReadByte();
			}
		}
	}
}
