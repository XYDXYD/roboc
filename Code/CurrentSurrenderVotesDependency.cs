using RCNetwork.Events;
using Svelto.DataStructures;
using System.IO;

internal sealed class CurrentSurrenderVotesDependency : NetworkDependency
{
	public int playersOnTeam
	{
		get;
		private set;
	}

	public FasterList<bool> currentVotes
	{
		get;
		private set;
	}

	public CurrentSurrenderVotesDependency(int numPlayersOnTeam, FasterList<bool> votes)
	{
		playersOnTeam = numPlayersOnTeam;
		currentVotes = votes;
	}

	public CurrentSurrenderVotesDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(playersOnTeam);
				binaryWriter.Write(currentVotes.get_Count());
				for (int i = 0; i < currentVotes.get_Count(); i++)
				{
					binaryWriter.Write(currentVotes.get_Item(i));
				}
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		currentVotes = new FasterList<bool>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				playersOnTeam = binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					currentVotes.Add(binaryReader.ReadBoolean());
				}
			}
		}
	}
}
