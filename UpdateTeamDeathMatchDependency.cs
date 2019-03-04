using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

internal sealed class UpdateTeamDeathMatchDependency : NetworkDependency
{
	public IDictionary<int, int> teamScores
	{
		get;
		private set;
	}

	public bool hasTimeExpired
	{
		get;
		private set;
	}

	public UpdateTeamDeathMatchDependency(IDictionary<int, int> teamScores, bool hasTeamExpired)
	{
		this.teamScores = teamScores;
		hasTimeExpired = hasTeamExpired;
	}

	public UpdateTeamDeathMatchDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(teamScores.Count);
				IEnumerator<KeyValuePair<int, int>> enumerator = teamScores.GetEnumerator();
				while (enumerator.MoveNext())
				{
					binaryWriter.Write(enumerator.Current.Key);
					binaryWriter.Write(enumerator.Current.Value);
				}
				binaryWriter.Write(hasTimeExpired);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		teamScores = new Dictionary<int, int>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int key = binaryReader.ReadInt32();
					int value = binaryReader.ReadInt32();
					teamScores[key] = value;
				}
				hasTimeExpired = binaryReader.ReadBoolean();
			}
		}
	}
}
