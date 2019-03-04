using Authentication;
using Battle;
using ExitGames.Client.Photon;
using LobbyServiceLayer;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerEnteredGameRequest_Tencent : WebServicesRequest, ILogPlayerEnteredGameRequest, IServiceRequest<EnterBattleDependency>, IAnswerOnComplete, IServiceRequest
	{
		private EnterBattleDependency _dependency;

		protected override byte OperationCode => 209;

		public LogPlayerEnteredGameRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerEnterGameError", 0)
		{
		}

		public void Inject(EnterBattleDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Expected O, but got Unknown
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			BattleParametersData battleParameters = _dependency.BattleParameters;
			GameModeKey gameModeKey = battleParameters.GameModeKey;
			PlayerDataDependency playerDataDependency = _dependency.Players.get_Item(User.Username);
			dictionary["GameModeType"] = gameModeKey.type.ToString();
			dictionary["IsRanked"] = gameModeKey.IsRanked;
			dictionary["IsBrawl"] = gameModeKey.IsBrawl;
			dictionary["IsCustomGame"] = gameModeKey.IsCustomGame;
			dictionary["ReconnectGameGUID"] = battleParameters.GameGuid;
			dictionary["MapName"] = battleParameters.MapName;
			dictionary["TeamId"] = playerDataDependency.TeamId;
			dictionary["RobotUniqueID"] = playerDataDependency.RobotUniqueId;
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[225] = dictionary;
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
