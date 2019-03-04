using RCNetwork.Events;
using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal class DestroyCubeDependency : NetworkDependency, IWeaponFireStateSyncDependency
{
	public ItemDescriptor itemDescriptor = new WeaponDescriptor(ItemCategory.NotAFunctionalItem, ItemSize.NotAWeapon);

	public Vector3 hitEffectOffset;

	public Vector3 hitEffectNormal;

	public int stackCount;

	public int weaponDamage;

	public int shootingMachineId
	{
		get;
		set;
	}

	public int hitMachineId
	{
		get;
		set;
	}

	public TargetType targetType
	{
		get;
		set;
	}

	public List<HitCubeInfo> hitCubeInfo
	{
		get;
		set;
	}

	public float timeStamp
	{
		get;
		set;
	}

	public DestroyCubeDependency()
	{
	}

	public DestroyCubeDependency(byte[] data)
		: base(data)
	{
	}

	public void SetVariables(int _shootingMachine, int _hitMachineId, Vector3 _hitEffectOffset, Vector3 _hitEffectNormal, ItemDescriptor _itemDescriptor, List<HitCubeInfo> _hitCubeInfo, float _timestamp, TargetType _type, int _weaponDamage, int _stackCount = 0)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		shootingMachineId = _shootingMachine;
		hitMachineId = _hitMachineId;
		hitEffectOffset = _hitEffectOffset;
		hitEffectNormal = _hitEffectNormal;
		itemDescriptor = _itemDescriptor;
		stackCount = _stackCount;
		hitCubeInfo = _hitCubeInfo;
		timeStamp = _timestamp;
		targetType = _type;
		weaponDamage = _weaponDamage;
	}

	public void MinorIncreaseToTimeStamp()
	{
		timeStamp += 0.001f;
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
				binaryWriter.Write(weaponDamage);
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.x, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.y, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.z, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.x, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.y, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.z, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write((ushort)this.hitCubeInfo.Count);
				for (int i = 0; i < this.hitCubeInfo.Count; i++)
				{
					HitCubeInfo hitCubeInfo = this.hitCubeInfo[i];
					binaryWriter.Write(hitCubeInfo.gridLoc.x);
					binaryWriter.Write(hitCubeInfo.gridLoc.y);
					binaryWriter.Write(hitCubeInfo.gridLoc.z);
					binaryWriter.Write(hitCubeInfo.destroyed);
					if (!hitCubeInfo.destroyed)
					{
						binaryWriter.Write(hitCubeInfo.damage);
					}
				}
				binaryWriter.Write(timeStamp);
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
				weaponDamage = binaryReader.ReadInt32();
				hitEffectOffset.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectNormal.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				int num = binaryReader.ReadUInt16();
				hitCubeInfo = new List<HitCubeInfo>(num);
				for (int i = 0; i < num; i++)
				{
					HitCubeInfo item = default(HitCubeInfo);
					item.gridLoc.x = binaryReader.ReadByte();
					item.gridLoc.y = binaryReader.ReadByte();
					item.gridLoc.z = binaryReader.ReadByte();
					item.destroyed = binaryReader.ReadBoolean();
					if (!item.destroyed)
					{
						item.damage = binaryReader.ReadInt32();
					}
					hitCubeInfo.Add(item);
				}
				timeStamp = binaryReader.ReadSingle();
			}
		}
	}
}
