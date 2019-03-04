using System.Collections.Generic;

namespace GameServerServiceLayer
{
	internal class ScoreMultipliers
	{
		public float maxCPU
		{
			get;
			private set;
		}

		public Dictionary<InGameStatId, float> baseMultipliers
		{
			get;
			private set;
		}

		public Dictionary<InGameStatId, float> bonusMultipliers
		{
			get;
			private set;
		}

		public uint victoryScore
		{
			get;
			private set;
		}

		public uint defeatScore
		{
			get;
			private set;
		}

		public float deltaScalar
		{
			get;
			private set;
		}

		public float completedBattleBaseMultiplier
		{
			get;
			private set;
		}

		public float completedBattleBonusMultiplier
		{
			get;
			private set;
		}

		public float maxScoreRatio
		{
			get;
			private set;
		}

		public ScoreMultipliers(float _maxCPU, Dictionary<InGameStatId, float> _baseMultipiers, Dictionary<InGameStatId, float> _bonusMultipiers, uint _victoryScore, uint _defeatScore, float _deltaScalar, float _completedBattleBaseMultiplier, float _completedBattleBonusMultiplier, float _maxScoreRatio)
		{
			maxCPU = _maxCPU;
			baseMultipliers = _baseMultipiers;
			bonusMultipliers = _bonusMultipiers;
			victoryScore = _victoryScore;
			defeatScore = _defeatScore;
			deltaScalar = _deltaScalar;
			completedBattleBaseMultiplier = _completedBattleBaseMultiplier;
			completedBattleBonusMultiplier = _completedBattleBonusMultiplier;
			maxScoreRatio = _maxScoreRatio;
		}

		public ScoreMultipliers Copy()
		{
			float maxCPU = this.maxCPU;
			Dictionary<InGameStatId, float> baseMultipiers = new Dictionary<InGameStatId, float>(baseMultipliers);
			Dictionary<InGameStatId, float> bonusMultipiers = new Dictionary<InGameStatId, float>(bonusMultipliers);
			uint victoryScore = this.victoryScore;
			uint defeatScore = this.defeatScore;
			float deltaScalar = this.deltaScalar;
			float completedBattleBaseMultiplier = this.completedBattleBaseMultiplier;
			float completedBattleBonusMultiplier = this.completedBattleBonusMultiplier;
			float maxScoreRatio = this.maxScoreRatio;
			return new ScoreMultipliers(maxCPU, baseMultipiers, bonusMultipiers, victoryScore, defeatScore, deltaScalar, completedBattleBaseMultiplier, completedBattleBonusMultiplier, maxScoreRatio);
		}
	}
}
