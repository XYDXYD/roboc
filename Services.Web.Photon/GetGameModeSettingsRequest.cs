using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetGameModeSettingsRequest : WebServicesRequest<GameModeSettingsDependency>, IGetGameModeSettingsRequest, IServiceRequest, IAnswerOnComplete<GameModeSettingsDependency>
	{
		protected override byte OperationCode => 113;

		public GetGameModeSettingsRequest()
			: base("strRobocloudError", "strLoadGameModeSettingsError", 3)
		{
		}

		public override void Execute()
		{
			if (IsDataCached())
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(GetCachedData());
				}
			}
			else
			{
				base.Execute();
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override GameModeSettingsDependency ProcessResponse(OperationResponse response)
		{
			GameModeSettings gameModeSettings = GetGameModeSettings(response, "BattleArena");
			GameModeSettings gameModeSettings2 = GetGameModeSettings(response, "Elimination");
			GameModeSettings gameModeSettings3 = GetGameModeSettings(response, "ThePit");
			GameModeSettings gameModeSettings4 = GetGameModeSettings(response, "TeamDeathmatch");
			GameModeSettingsDependency gameModeSettingsDependency = CacheDTO.gameModeSettings = new GameModeSettingsDependency(gameModeSettings, gameModeSettings2, gameModeSettings3, gameModeSettings4);
			return GetCachedData();
		}

		private GameModeSettings GetGameModeSettings(OperationResponse response, string gameMode)
		{
			Hashtable val = ((Dictionary<string, Hashtable>)response.Parameters[1])[gameMode];
			float respawnHealDuration_ = Convert.ToSingle(val.get_Item((object)"respawnHealDuration"));
			float respawnFullHealDuration_ = Convert.ToSingle(val.get_Item((object)"respawnFullHealDuration"));
			int value = Convert.ToInt32(val.get_Item((object)"killLimit"));
			int gameTimeMinutes_ = Convert.ToInt32(val.get_Item((object)"gameTimeMinutes"));
			GameModeSettings gameModeSettings = new GameModeSettings(respawnHealDuration_, respawnFullHealDuration_, gameTimeMinutes_);
			gameModeSettings.killLimit = value;
			return gameModeSettings;
		}

		private bool IsDataCached()
		{
			return CacheDTO.gameModeSettings != null;
		}

		private GameModeSettingsDependency GetCachedData()
		{
			return new GameModeSettingsDependency(CacheDTO.gameModeSettings);
		}
	}
}
