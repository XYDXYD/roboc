using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal class HealedAllyCubesDependency : NetworkDependency
{
	public int healedMachine;

	public int shootingMachineId;

	public int shootingPlayerId;

	public List<HitCubeInfo> healedCubes;

	public TargetType typePerformingHealing;

	public float timeStamp;

	public ItemSize itemSize;

	public Vector3 hitEffectOffset;

	public Vector3 hitEffectNormal;

	public HealedAllyCubesDependency(byte[] data)
		: base(data)
	{
	}

	public HealedAllyCubesDependency()
	{
	}

	public void SetVariables(int healedMachine, int shootingMachineId, int shootingPlayerId, List<HitCubeInfo> healedCubes, Vector3 hitEffectOffset, Vector3 hitEffectNormal, ItemSize itemSize, TargetType typePerformingHealing, float timeStamp)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		this.healedMachine = healedMachine;
		this.shootingMachineId = shootingMachineId;
		this.shootingPlayerId = shootingPlayerId;
		this.healedCubes = healedCubes;
		this.hitEffectNormal = hitEffectNormal;
		this.hitEffectOffset = hitEffectOffset;
		this.itemSize = itemSize;
		this.typePerformingHealing = typePerformingHealing;
		this.timeStamp = timeStamp;
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
				binaryWriter.Write((short)healedMachine);
				binaryWriter.Write((short)shootingMachineId);
				binaryWriter.Write((byte)shootingPlayerId);
				binaryWriter.Write((int)itemSize);
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.x, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.y, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectOffset.z, DataCompressor.CompressionType.MachinePosition));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.x, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.y, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(DataCompressor.CompressFloat(hitEffectNormal.z, DataCompressor.CompressionType.NormalRange));
				binaryWriter.Write(timeStamp);
				ushort num = (ushort)healedCubes.Count;
				binaryWriter.Write(num);
				for (int i = 0; i < num; i++)
				{
					HitCubeInfo hitCubeInfo = healedCubes[i];
					binaryWriter.Write(hitCubeInfo.gridLoc.x);
					binaryWriter.Write(hitCubeInfo.gridLoc.y);
					binaryWriter.Write(hitCubeInfo.gridLoc.z);
					binaryWriter.Write(hitCubeInfo.damage);
					binaryWriter.Write(Convert.ToByte((int)typePerformingHealing));
				}
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		healedCubes = new List<HitCubeInfo>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				healedMachine = binaryReader.ReadInt16();
				shootingMachineId = binaryReader.ReadInt16();
				shootingPlayerId = binaryReader.ReadByte();
				itemSize = (ItemSize)binaryReader.ReadInt32();
				hitEffectOffset.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectOffset.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.MachinePosition);
				hitEffectNormal.x = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.y = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				hitEffectNormal.z = DataCompressor.DecompressFloat(binaryReader.ReadInt16(), DataCompressor.CompressionType.NormalRange);
				timeStamp = binaryReader.ReadSingle();
				int num = binaryReader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					HitCubeInfo item = default(HitCubeInfo);
					item.gridLoc.x = binaryReader.ReadByte();
					item.gridLoc.y = binaryReader.ReadByte();
					item.gridLoc.z = binaryReader.ReadByte();
					item.damage = binaryReader.ReadInt32();
					typePerformingHealing = (TargetType)Convert.ToInt32(binaryReader.ReadByte());
					healedCubes.Add(item);
				}
			}
		}
	}
}
