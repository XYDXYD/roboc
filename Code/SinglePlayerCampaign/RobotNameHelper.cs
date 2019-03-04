using Simulation.SinglePlayerCampaign.DataTypes;
using System;

namespace SinglePlayerCampaign
{
	internal static class RobotNameHelper
	{
		private const string BOT_SUFFIX = "_Bot_";

		public static string GetName(WaveRobot robot, int spawnEventIndex, int robotIndex)
		{
			string @string = StringTableBase<StringTable>.Instance.GetString(robot.robotName);
			return string.Format("{0}{1}{2}_{3}", @string, "_Bot_", spawnEventIndex, robotIndex);
		}

		public static void ValidatePlayerName(string name)
		{
			if (name.Contains("_Bot_"))
			{
				throw new Exception("Invalid player name.");
			}
		}
	}
}
