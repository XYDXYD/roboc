using Achievements;
using Utility;

namespace Simulation.Achievements
{
	public class NullAchievementManager : IAchievementManager
	{
		public void ActivatedModule(ItemCategory moduleCategory, int count = 1)
		{
			Console.Log("Achievement manager: ActivatedModule - disregard");
		}

		public void CapturedPoint()
		{
			Console.Log("Achievement manager: CapturedPoint - disregard");
		}

		public void CompletedBattle(ItemCategory itemCategory)
		{
			Console.Log("Achievement manager: CompletedBattle - disregard");
		}

		public void CompletedBattleWithFullParty()
		{
			Console.Log("Achievement manager: CompletedBattleWithFullParty - disregard");
		}

		public void CompletedHealFrom20To100()
		{
			Console.Log("Achievement manager:CompletedHealFrom20To100  - disregard");
		}

		public void CompletedKillWithTeslaAfterDecloaked()
		{
			Console.Log("Achievement manager: CompletedKillWithTeslaAfterDecloaked - disregard");
		}

		public void CompletedRankedBattle()
		{
			Console.Log("Achievement manager: CompletedRankedBattle - disregard");
		}

		public void CompletedTutorial()
		{
			Console.Log("Achievement manager:CompletedTutorial  - disregard");
		}

		public void EarnRobitsFromCRF(int count)
		{
			Console.Log("Achievement manager: EarnRobitsFromCRF - disregard");
		}

		public void MadeAKill(ItemCategory itemCategory)
		{
			Console.Log("Achievement manager: MadeAKill - disregard");
		}

		public void ReachedRank(uint rank)
		{
			Console.Log("Achievement manager:ReachedRank - disregard");
		}

		public void ReachedMastery10OnCRFBot()
		{
			Console.Log("Achievement manager:ReachedMastery10OnCRFBot  - disregard");
		}

		public void EarnFirstCampaign()
		{
			Console.Log("Achievement manager:EarnFirstCampaign  - disregard");
		}
	}
}
