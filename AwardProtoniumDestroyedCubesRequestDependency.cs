using RCNetwork.Events;
using System;
using System.IO;

internal sealed class AwardProtoniumDestroyedCubesRequestDependency : NetworkDependency
{
	public int destroyedCubesCount
	{
		get;
		private set;
	}

	public int id
	{
		get;
		private set;
	}

	public uint protoniumCubeId
	{
		get;
		private set;
	}

	public AwardProtoniumDestroyedCubesRequestDependency(int data)
	{
		destroyedCubesCount = data;
	}

	public AwardProtoniumDestroyedCubesRequestDependency(byte[] data)
		: base(data)
	{
	}

	public void SetProtoniumCubeId(uint cubeId)
	{
		protoniumCubeId = cubeId;
	}

	public void SetDestroyedCubesCount(int count)
	{
		destroyedCubesCount = count;
	}

	public void SetId(int _id)
	{
		id = _id;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(destroyedCubesCount);
				binaryWriter.Write(Convert.ToByte(id));
				binaryWriter.Write(protoniumCubeId);
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
				destroyedCubesCount = binaryReader.ReadInt32();
				id = Convert.ToInt32(binaryReader.ReadByte());
				protoniumCubeId = binaryReader.ReadUInt32();
			}
		}
	}
}
