using RCNetwork.Events;
using System;
using System.IO;

internal class HealingAssistBonusRequestDependency : NetworkDependency
{
	public int healedPlayerId
	{
		get;
		private set;
	}

	public int healingPlayerId
	{
		get;
		private set;
	}

	public HealingAssistBonusRequestDependency(int _healingPlayerId, int _healedPlayerId)
	{
		healingPlayerId = _healingPlayerId;
		healedPlayerId = _healedPlayerId;
	}

	public HealingAssistBonusRequestDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(healingPlayerId));
				binaryWriter.Write(Convert.ToByte(healedPlayerId));
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
				healingPlayerId = Convert.ToInt32(binaryReader.ReadByte());
				healedPlayerId = Convert.ToInt32(binaryReader.ReadByte());
			}
		}
	}
}
