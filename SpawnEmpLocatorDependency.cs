using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class SpawnEmpLocatorDependency : NetworkDependency
{
	public Vector3 position;

	public float range;

	public float countdown;

	public float stunDuration;

	public int ownerId;

	public int ownerMachineId;

	public SpawnEmpLocatorDependency()
	{
	}

	public SpawnEmpLocatorDependency(byte[] data)
		: base(data)
	{
	}

	public SpawnEmpLocatorDependency Inject(Vector3 position_, float range_, float countdown_, float stunDuration_, int ownerId_, int ownerMachineId_)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		position = position_;
		range = range_;
		countdown = countdown_;
		stunDuration = stunDuration_;
		ownerId = ownerId_;
		ownerMachineId = ownerMachineId_;
		return this;
	}

	public override byte[] Serialise()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(position, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				binaryWriter.Write(range);
				binaryWriter.Write(countdown);
				binaryWriter.Write(stunDuration);
				binaryWriter.Write((byte)ownerId);
				binaryWriter.Write((short)ownerMachineId);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				DataCompressor.V3Compressed data2 = default(DataCompressor.V3Compressed);
				data2.x = binaryReader.ReadInt16();
				data2.y = binaryReader.ReadInt16();
				data2.z = binaryReader.ReadInt16();
				position = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				range = binaryReader.ReadSingle();
				countdown = binaryReader.ReadSingle();
				stunDuration = binaryReader.ReadSingle();
				ownerId = binaryReader.ReadByte();
				ownerMachineId = binaryReader.ReadInt16();
			}
		}
	}
}
