using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class RegisterCapturePointDependency : NetworkDependency
{
	public CapturePointId id;

	public Vector3 position;

	public Quaternion rotation;

	public RegisterCapturePointDependency(byte[] data)
		: base(data)
	{
	}

	public RegisterCapturePointDependency(CapturePointId _id, Vector3 _position, Quaternion _rotation)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		position = _position;
		rotation = _rotation;
		id = _id;
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
				binaryWriter.Write((byte)id);
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
				id = (CapturePointId)binaryReader.ReadByte();
			}
		}
	}
}
