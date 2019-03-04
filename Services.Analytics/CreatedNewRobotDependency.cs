using System.Collections.Generic;

namespace Services.Analytics
{
	internal class CreatedNewRobotDependency
	{
		public string robotName
		{
			get;
			set;
		}

		public int createType
		{
			get;
			set;
		}

		public int robotCount
		{
			get;
			set;
		}

		public PlayerRobotStats robotStats
		{
			get;
			set;
		}

		public CreatedNewRobotDependency(string robotName_, int createType_, int robotCount_, PlayerRobotStats robotStats_)
		{
			robotName = robotName_;
			createType = createType_;
			robotCount = robotCount_;
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
			dictionary["RobotCreateType"] = createType;
			dictionary["RobotCount"] = robotCount;
			dictionary["RobotRanking"] = robotStats.ranking;
			return dictionary;
		}
	}
}
