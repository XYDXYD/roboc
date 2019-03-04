namespace Achievements
{
	public interface IAchievementManager
	{
		void CompletedTutorial();

		void CompletedBattleWithFullParty();

		void CompletedHealFrom20To100();

		void CompletedKillWithTeslaAfterDecloaked();

		void CapturedPoint();

		void ActivatedModule(ItemCategory moduleCategory, int count = 1);

		void MadeAKill(ItemCategory itemCategory);

		void CompletedBattle(ItemCategory itemCategory);

		void ReachedRank(uint rank);

		void ReachedMastery10OnCRFBot();

		void EarnRobitsFromCRF(int count);

		void EarnFirstCampaign();
	}
}
