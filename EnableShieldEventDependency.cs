using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class EnableShieldEventDependency : NetworkDependency
{
	public int owner;

	public int id;

	public Quaternion rotation;

	public EnableShieldEventDependency(int owner, int id, Quaternion rotation)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.owner = owner;
		this.id = id;
		this.rotation = rotation;
	}

	public EnableShieldEventDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)owner);
				binaryWriter.Write((byte)id);
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
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				owner = binaryReader.ReadByte();
				id = binaryReader.ReadByte();
				DataCompressor.Q3Compressed data2 = default(DataCompressor.Q3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				rotation = DataCompressor.DecompressQuaternion(data2, DataCompressor.CompressionType.MachineRotation);
			}
		}
	}
}
