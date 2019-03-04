using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class WeaponFireEffectDependency : NetworkDependency
{
	public Vector3 launchPosition;

	public Vector3 direction;

	public int shootingMachineId;

	public int shootingPlayerId;

	public Byte3 weaponGridKey;

	public WeaponFireEffectDependency()
	{
	}

	public WeaponFireEffectDependency(byte[] data)
		: base(data)
	{
	}

	public void SetVariables(Vector3 launchPosition_, Vector3 direction_, int shootingMachineId_, int shootingPlayerId_, Byte3 weaponGridKey_)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		launchPosition = launchPosition_;
		direction = direction_;
		shootingMachineId = shootingMachineId_;
		shootingPlayerId = shootingPlayerId_;
		weaponGridKey = weaponGridKey_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(DataCompressor.CompressFloat(launchPosition.x, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(launchPosition.y, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(launchPosition.z, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(direction.x, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(direction.y, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(direction.z, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write((short)shootingMachineId);
				binaryWriter.Write((byte)shootingPlayerId);
				binaryWriter.Write(weaponGridKey.x);
				binaryWriter.Write(weaponGridKey.y);
				binaryWriter.Write(weaponGridKey.z);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				launchPosition.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				launchPosition.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				launchPosition.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				direction.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				direction.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				direction.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				shootingMachineId = binaryReader.ReadInt16();
				shootingPlayerId = binaryReader.ReadByte();
				weaponGridKey.x = binaryReader.ReadByte();
				weaponGridKey.y = binaryReader.ReadByte();
				weaponGridKey.z = binaryReader.ReadByte();
			}
		}
	}
}
