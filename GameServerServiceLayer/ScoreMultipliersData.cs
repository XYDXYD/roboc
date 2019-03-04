using System.Collections.Generic;
using System.IO;

namespace GameServerServiceLayer
{
	internal class ScoreMultipliersData
	{
		public ScoreMultipliers scoreMultipliers
		{
			get;
			private set;
		}

		public ScoreMultipliersData(byte[] data)
		{
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					float maxCPU = binaryReader.ReadSingle();
					Dictionary<InGameStatId, float> dictionary = new Dictionary<InGameStatId, float>();
					Dictionary<InGameStatId, float> dictionary2 = new Dictionary<InGameStatId, float>();
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						InGameStatId key = (InGameStatId)binaryReader.ReadUInt32();
						float value = binaryReader.ReadSingle();
						float value2 = binaryReader.ReadSingle();
						dictionary[key] = value;
						dictionary2[key] = value2;
					}
					float completedBattleBaseMultiplier = binaryReader.ReadSingle();
					float completedBattleBonusMultiplier = binaryReader.ReadSingle();
					float deltaScalar = binaryReader.ReadSingle();
					uint defeatScore = binaryReader.ReadUInt32();
					uint victoryScore = binaryReader.ReadUInt32();
					float maxScoreRatio = binaryReader.ReadSingle();
					scoreMultipliers = new ScoreMultipliers(maxCPU, dictionary, dictionary2, victoryScore, defeatScore, deltaScalar, completedBattleBaseMultiplier, completedBattleBonusMultiplier, maxScoreRatio);
				}
			}
		}
	}
}
