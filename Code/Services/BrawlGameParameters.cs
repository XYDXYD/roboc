using ExitGames.Client.Photon;
using System;

namespace Services
{
	public class BrawlGameParameters
	{
		public string BrawlDetailsTopImageName
		{
			get;
			private set;
		}

		public string BrawlDetailsImageOffName
		{
			get;
			private set;
		}

		public string BrawlDetailsImageName
		{
			get;
			private set;
		}

		public float XpMultiplierForFirstPlay
		{
			get;
			private set;
		}

		public ConfigurableGameModeParameters commonParameters
		{
			get;
			private set;
		}

		public static BrawlGameParameters Deserialise(Hashtable data)
		{
			BrawlGameParameters brawlGameParameters = new BrawlGameParameters();
			brawlGameParameters.BrawlDetailsImageOffName = (string)data.get_Item((object)"DetailsImageOffName");
			brawlGameParameters.BrawlDetailsTopImageName = (string)data.get_Item((object)"DetailsTopImageName");
			brawlGameParameters.BrawlDetailsImageName = (string)data.get_Item((object)"DetailsImageName");
			brawlGameParameters.XpMultiplierForFirstPlay = (float)data.get_Item((object)"XPMultiplierRewardForFirstPlay");
			BrawlGameParameters brawlGameParameters2 = brawlGameParameters;
			ConfigurableGameModeParameters configurableGameModeParameters = new ConfigurableGameModeParameters();
			configurableGameModeParameters.GameMode = DeserializeGameMode((string)data.get_Item((object)"GameMode"));
			configurableGameModeParameters.ExcludedCubesList = (string[])data.get_Item((object)"ExcludeTheseCubes");
			configurableGameModeParameters.ExcludedCubeTypesList = (string[])data.get_Item((object)"ExcludeTheseCubeTypes");
			configurableGameModeParameters.MinCPU = (int)data.get_Item((object)"MinCPU");
			configurableGameModeParameters.MaxCPU = (int)data.get_Item((object)"MaxCPU");
			configurableGameModeParameters.NameString = (string)data.get_Item((object)"NameString");
			configurableGameModeParameters.NameDesc = (string)data.get_Item((object)"NameDesc");
			configurableGameModeParameters.RuleStrings = (string[])data.get_Item((object)"Rules");
			configurableGameModeParameters.RuleParameters = (string[][])data.get_Item((object)"RuleParameters");
			ConfigurableGameModeParameters configurableGameModeParameters3 = brawlGameParameters2.commonParameters = configurableGameModeParameters;
			return brawlGameParameters2;
		}

		private static GameModeType DeserializeGameMode(string input)
		{
			return (GameModeType)Enum.Parse(typeof(GameModeType), input, ignoreCase: false);
		}
	}
}
