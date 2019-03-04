using System.Collections.Generic;
using UnityEngine;

namespace Services.Web.Photon
{
	internal class UploadRobotDependency
	{
		public int slotId
		{
			get;
			set;
		}

		public string name
		{
			get;
			set;
		}

		public string description
		{
			get;
			set;
		}

		public Texture2D thumbnail
		{
			get;
			set;
		}

		public PlayerRobotStats robotStats
		{
			get;
			set;
		}

		public UploadRobotDependency(int _slotId, string _name, string _description, Texture2D _thumbnail, PlayerRobotStats robotStats_)
		{
			slotId = _slotId;
			name = _name;
			description = _description;
			thumbnail = _thumbnail;
			robotStats = robotStats_;
		}

		public Dictionary<string, object> ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["SlotId"] = slotId;
			dictionary["Name"] = name;
			dictionary["Description"] = description;
			dictionary["Thumbnail"] = ImageConversion.EncodeToPNG(thumbnail);
			dictionary["RobotCPU"] = robotStats.totalCPU;
			dictionary["DamageBoost"] = robotStats.damageBoost;
			dictionary["BaseHealth"] = robotStats.baseHealth;
			dictionary["HealthBoost"] = robotStats.healthBoost;
			dictionary["BaseSpeed"] = robotStats.baseSpeed;
			dictionary["SpeedBoost"] = robotStats.speedBoost;
			dictionary["Mass"] = robotStats.mass;
			dictionary["CosmeticCPU"] = robotStats.cosmeticCPU;
			dictionary["RobotRanking"] = robotStats.ranking;
			return dictionary;
		}
	}
}
