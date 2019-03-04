using RCNetwork.Events;
using System.IO;

internal sealed class TeamBaseStateDependency : NetworkDependency
{
	public int team;

	public float currentProgress;

	public float maxProgress;

	public TeamBaseStateDependency(byte[] data)
		: base(data)
	{
	}

	public TeamBaseStateDependency(int _team, float _currentProgress, float _maxProgress)
	{
		team = _team;
		currentProgress = _currentProgress;
		maxProgress = _maxProgress;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)team);
				binaryWriter.Write(DataCompressor.CompressFloatAsByte(currentProgress, DataCompressor.CompressionType.MachineRotation));
				binaryWriter.Write(DataCompressor.CompressFloatAsByte(maxProgress, DataCompressor.CompressionType.MachineRotation));
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
				team = binaryReader.ReadByte();
				currentProgress = DataCompressor.DecompressFloatAsByte(binaryReader.ReadByte(), DataCompressor.CompressionType.MachineRotation);
				maxProgress = DataCompressor.DecompressFloatAsByte(binaryReader.ReadByte(), DataCompressor.CompressionType.MachineRotation);
			}
		}
	}
}
