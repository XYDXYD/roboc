using Achievements;

namespace Simulation.Achievements
{
	internal class Achievement
	{
		public ItemCategory itemCategory;

		public AchievementID achievementID;

		public int count;

		public Achievement(ItemCategory itemCategory_, AchievementID achievementID_, int count_ = 1)
		{
			itemCategory = itemCategory_;
			achievementID = achievementID_;
			count = count_;
		}
	}
}
