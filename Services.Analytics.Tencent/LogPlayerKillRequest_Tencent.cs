using ExitGames.Client.Photon;
using LobbyServiceLayer;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Analytics.Tencent
{
	internal class LogPlayerKillRequest_Tencent : WebServicesRequest, ILogPlayerKillRequest, IServiceRequest<LogPlayerKillDependency>, IAnswerOnComplete, IServiceRequest
	{
		private LogPlayerKillDependency _dependency;

		protected override byte OperationCode => 210;

		public LogPlayerKillRequest_Tencent()
			: base("strRobocloudError", "strTencentLogPlayerKillGameError", 0)
		{
		}

		public void Inject(LogPlayerKillDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Expected O, but got Unknown
			LogPlayerKillDependency dependency = _dependency;
			GameModeKey value = CacheDTO.GameMode.Value;
			dependency.gameModeType = value.type.ToString();
			LogPlayerKillDependency dependency2 = _dependency;
			GameModeKey value2 = CacheDTO.GameMode.Value;
			dependency2.isRanked = value2.IsRanked;
			LogPlayerKillDependency dependency3 = _dependency;
			GameModeKey value3 = CacheDTO.GameMode.Value;
			dependency3.isBrawl = value3.IsBrawl;
			LogPlayerKillDependency dependency4 = _dependency;
			GameModeKey value4 = CacheDTO.GameMode.Value;
			dependency4.isCustomGame = value4.IsCustomGame;
			_dependency.gameGUID = CacheDTO.ReconnectGameGUID.ToString();
			_dependency.mapName = CacheDTO.MapName.ToString();
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[225] = _dependency.ToDictionary();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}
	}
}
