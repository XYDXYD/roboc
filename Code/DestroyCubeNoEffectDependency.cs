using RCNetwork.Events;
using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using System.IO;

internal class DestroyCubeNoEffectDependency : NetworkDependency, IWeaponFireStateSyncDependency
{
	public List<HitCubeInfo> hitCubeInfo
	{
		get;
		set;
	}

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

	public float timeStamp
	{
		get;
		set;
	}

	public DestroyCubeNoEffectDependency()
	{
	}

	public DestroyCubeNoEffectDependency(byte[] data)
		: base(data)
	{
	}

	public void MinorIncreaseToTimeStamp()
	{
		timeStamp += 0.001f;
	}

	public void SetVariables(int _shootingMachine, int _hitMachineId, List<HitCubeInfo> _hitCubeInfo, float _timestamp, TargetType _type)
	{
		shootingMachineId = _shootingMachine;
		hitMachineId = _hitMachineId;
		hitCubeInfo = _hitCubeInfo;
		timeStamp = _timestamp;
		targetType = _type;
	}

	public void FromDestroyCubeDependency(DestroyCubeDependency destroyCubeDependency)
	{
		shootingMachineId = destroyCubeDependency.shootingMachineId;
		hitMachineId = destroyCubeDependency.hitMachineId;
		targetType = destroyCubeDependency.targetType;
		hitCubeInfo = destroyCubeDependency.hitCubeInfo;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((short)shootingMachineId);
				binaryWriter.Write((short)hitMachineId);
				binaryWriter.Write((byte)targetType);
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
				targetType = (TargetType)binaryReader.ReadByte();
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
			}
		}
	}
}
