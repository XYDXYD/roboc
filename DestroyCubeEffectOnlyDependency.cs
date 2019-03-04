using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using System.IO;
using UnityEngine;

internal class DestroyCubeEffectOnlyDependency : NetworkDependency
{
	public Byte3 hitCube;

	public int shootingMachineId;

	public int hitMachineId;

	public TargetType targetType;

	public ItemDescriptor itemDescriptor = new WeaponDescriptor(ItemCategory.NotAFunctionalItem, ItemSize.NotAWeapon);

	public Vector3 hitEffectOffset;

	public Vector3 hitEffectNormal;

	public int stackCount;

	public DestroyCubeEffectOnlyDependency()
	{
	}

	public DestroyCubeEffectOnlyDependency(byte[] data)
		: base(data)
	{
	}

	public void FromDestroyCubeDependency(DestroyCubeDependency destroyCubeDependency)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		HitCubeInfo hitCubeInfo = destroyCubeDependency.hitCubeInfo[0];
		hitCube = hitCubeInfo.gridLoc;
		shootingMachineId = destroyCubeDependency.shootingMachineId;
		hitMachineId = destroyCubeDependency.hitMachineId;
		targetType = destroyCubeDependency.targetType;
		itemDescriptor = destroyCubeDependency.itemDescriptor;
		hitEffectOffset = destroyCubeDependency.hitEffectOffset;
		hitEffectNormal = destroyCubeDependency.hitEffectNormal;
		stackCount = destroyCubeDependency.stackCount;
	}

	public void SetVariables(Byte3 hitCube, int shootingMachineId, int hitMachineId, TargetType targetType, ItemDescriptor itemDescriptor, Vector3 hitEffectOffset, Vector3 hitEffectNormal, int stackCount = 0)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		this.shootingMachineId = shootingMachineId;
		this.hitMachineId = hitMachineId;
		this.hitEffectOffset = hitEffectOffset;
		this.hitEffectNormal = hitEffectNormal;
		this.itemDescriptor = itemDescriptor;
		this.stackCount = stackCount;
		this.hitCube = hitCube;
		this.targetType = targetType;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((short)shootingMachineId);
				binaryWriter.Write((short)hitMachineId);
				binaryWriter.Write((short)itemDescriptor.itemCategory);
				binaryWriter.Write((short)itemDescriptor.itemSize);
				binaryWriter.Write((byte)stackCount);
				binaryWriter.Write((byte)targetType);
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.x, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.y, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.z, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.x, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.y, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.z, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(hitCube.x);
				binaryWriter.Write(hitCube.y);
				binaryWriter.Write(hitCube.z);
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
				shootingMachineId = binaryReader.ReadInt16();
				hitMachineId = binaryReader.ReadInt16();
				itemDescriptor.itemCategory = (ItemCategory)binaryReader.ReadInt16();
				itemDescriptor.itemSize = (ItemSize)binaryReader.ReadInt16();
				stackCount = binaryReader.ReadByte();
				targetType = (TargetType)binaryReader.ReadByte();
				hitEffectOffset.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectNormal.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitCube.x = binaryReader.ReadByte();
				hitCube.y = binaryReader.ReadByte();
				hitCube.z = binaryReader.ReadByte();
			}
		}
	}
}
