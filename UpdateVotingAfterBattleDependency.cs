using RCNetwork.Events;
using Simulation.GUI;
using System;
using System.IO;

internal sealed class UpdateVotingAfterBattleDependency : NetworkDependency
{
	public string playerName;

	public int amount;

	public VoteType voteType;

	public UpdateVotingAfterBattleDependency(string playerName, int amount, VoteType voteType)
	{
		SetFields(playerName, amount, voteType);
	}

	public UpdateVotingAfterBattleDependency(byte[] data)
		: base(data)
	{
	}

	public void SetFields(string playerName, int amount, VoteType voteType)
	{
		this.playerName = playerName;
		this.amount = amount;
		this.voteType = voteType;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(playerName);
				binaryWriter.Write(amount);
				binaryWriter.Write(Convert.ToByte((uint)voteType));
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
				playerName = binaryReader.ReadString();
				amount = binaryReader.ReadInt32();
				voteType = (VoteType)Convert.ToUInt32(binaryReader.ReadByte());
			}
		}
	}
}
