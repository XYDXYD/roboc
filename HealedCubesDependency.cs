using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using System.IO;

internal class HealedCubesDependency : NetworkDependency
{
	public int healedMachine;

	public List<HitCubeInfo> healedCubes;

	public TargetType typePerformingHealing;

	public TargetType targetType;

	public HealedCubesDependency(byte[] data)
		: base(data)
	{
	}

	public HealedCubesDependency(int healedMachine, List<HitCubeInfo> healedCubes, TargetType typePerformingHealing, TargetType targetType)
	{
		this.healedMachine = healedMachine;
		this.healedCubes = healedCubes;
		this.typePerformingHealing = typePerformingHealing;
		this.targetType = targetType;
	}

	public HealedCubesDependency()
	{
	}

	public void SetVariables(int healedMachine, List<HitCubeInfo> healedCubes, TargetType typePerformingHealing, TargetType targetType)
	{
		this.healedMachine = healedMachine;
		this.healedCubes = healedCubes;
		this.typePerformingHealing = typePerformingHealing;
		this.targetType = targetType;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((short)healedMachine);
				binaryWriter.Write((byte)typePerformingHealing);
				binaryWriter.Write((byte)targetType);
				ushort num = (ushort)healedCubes.Count;
				binaryWriter.Write(num);
				for (int i = 0; i < num; i++)
				{
					HitCubeInfo hitCubeInfo = healedCubes[i];
					binaryWriter.Write(hitCubeInfo.gridLoc.x);
					binaryWriter.Write(hitCubeInfo.gridLoc.y);
					binaryWriter.Write(hitCubeInfo.gridLoc.z);
					binaryWriter.Write(hitCubeInfo.damage);
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
				typePerformingHealing = (TargetType)binaryReader.ReadByte();
				targetType = (TargetType)binaryReader.ReadByte();
				int num = binaryReader.ReadUInt16();
				for (int i = 0; i < num; i++)
				{
					HitCubeInfo item = default(HitCubeInfo);
					item.gridLoc.x = binaryReader.ReadByte();
					item.gridLoc.y = binaryReader.ReadByte();
					item.gridLoc.z = binaryReader.ReadByte();
					item.damage = binaryReader.ReadInt32();
					healedCubes.Add(item);
				}
			}
		}
	}
}
