using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class ShieldModuleEventDependency : NetworkDependency
{
	public Vector3 shieldPosition;

	public Quaternion shieldRotation;

	public int shooterId;

	public ShieldModuleEventDependency()
	{
	}

	public ShieldModuleEventDependency(byte[] data)
		: base(data)
	{
	}

	public ShieldModuleEventDependency Inject(Vector3 shieldWorldPosition, Quaternion shieldWorldRotation, int shooterId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		shieldPosition = shieldWorldPosition;
		shieldRotation = shieldWorldRotation;
		this.shooterId = shooterId;
		return this;
	}

	public override byte[] Serialise()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				DataCompressor.V3Compressed v3Compressed = DataCompressor.CompressVector3(shieldPosition, DataCompressor.CompressionType.MachinePosition);
				binaryWriter.Write(v3Compressed.x);
				binaryWriter.Write(v3Compressed.y);
				binaryWriter.Write(v3Compressed.z);
				DataCompressor.Q3Compressed q3Compressed = DataCompressor.CompressQuaternion(shieldRotation, DataCompressor.CompressionType.MachineRotation);
				binaryWriter.Write(q3Compressed.x);
				binaryWriter.Write(q3Compressed.y);
				binaryWriter.Write(q3Compressed.z);
				binaryWriter.Write((byte)shooterId);
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
				shieldPosition = DataCompressor.DecompressVector3(data2, DataCompressor.CompressionType.MachinePosition);
				DataCompressor.Q3Compressed data3 = default(DataCompressor.Q3Compressed);
				data3.x = binaryReader.ReadInt16();
				data3.y = binaryReader.ReadInt16();
				data3.z = binaryReader.ReadInt16();
				shieldRotation = DataCompressor.DecompressQuaternion(data3, DataCompressor.CompressionType.MachineRotation);
				shooterId = binaryReader.ReadByte();
			}
		}
	}
}
