using System.Collections.Generic;

namespace SinglePlayerCampaign.GUI.Mothership.DataTypes
{
	public class GameModeVersionParams
	{
		public readonly Dictionary<string, bool> IsLocked;

		public readonly int VersionNumber;

		public GameModeVersionParams(Dictionary<string, bool> isLocked, int versionNumber)
		{
			IsLocked = isLocked;
			VersionNumber = versionNumber;
		}
	}
}
