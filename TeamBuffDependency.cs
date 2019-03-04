using RCNetwork.Events;
using System.IO;

internal sealed class TeamBuffDependency : NetworkDependency
{
	private const int NUM_TEAMS = 2;

	public float[] teamBuffs = new float[2];

	public bool isNeutralised;

	public bool isStartOfGame;

	public int disconnectedTeamID;

	public TeamBuffDependency(byte[] data)
		: base(data)
	{
	}

	public TeamBuffDependency()
	{
	}

	public void SetValues(float team0Buff_, float team1Buff_, bool isNeutralised_, bool isStartOfGame_, int disconnectedTeamID_)
	{
		teamBuffs[0] = team0Buff_;
		teamBuffs[1] = team1Buff_;
		isNeutralised = isNeutralised_;
		isStartOfGame = isStartOfGame_;
		disconnectedTeamID = disconnectedTeamID_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				for (int i = 0; i < 2; i++)
				{
					binaryWriter.Write(teamBuffs[i]);
				}
				binaryWriter.Write(isNeutralised);
				binaryWriter.Write(isStartOfGame);
				binaryWriter.Write((byte)disconnectedTeamID);
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
				for (int i = 0; i < 2; i++)
				{
					teamBuffs[i] = binaryReader.ReadSingle();
				}
				isNeutralised = binaryReader.ReadBoolean();
				isStartOfGame = binaryReader.ReadBoolean();
				disconnectedTeamID = binaryReader.ReadByte();
			}
		}
	}
}
