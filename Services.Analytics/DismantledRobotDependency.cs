using System.Collections.Generic;

namespace Services.Analytics
{
	internal class DismantledRobotDependency
	{
		public string robotName
		{
			get;
			set;
		}

		public PlayerRobotStats robotStats
		{
			get;
			set;
		}

		public DismantledRobotDependency(string robotName_, PlayerRobotStats robotStats_)
		{
			robotName = robotName_;
			robotStats = robotStats_;
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["Name"] = robotName;
			dictionary["RobotUniqueID"] = robotStats.robotUniqueId;
			dictionary["RobotCPU"] = robotStats.totalCPU;
			dictionary["DamageBoost"] = robotStats.damageBoost;
			dictionary["BaseHealth"] = robotStats.baseHealth;
			dictionary["HealthBoost"] = robotStats.healthBoost;
			dictionary["BaseSpeed"] = robotStats.baseSpeed;
			dictionary["SpeedBoost"] = robotStats.speedBoost;
			dictionary["CosmeticCPU"] = robotStats.cosmeticCPU;
			dictionary["MasteryLevel"] = robotStats.masteryLevel;
			dictionary["Mass"] = robotStats.mass;
			dictionary["RobotRanking"] = robotStats.ranking;
			return dictionary;
		}
	}
}
