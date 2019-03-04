using Battle;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class GetBattleParametersRequest : IGetBattleParametersRequest, IServiceRequest, IAnswerOnComplete<BattleParametersData>
	{
		private IServiceAnswer<BattleParametersData> _answer;

		public void Execute()
		{
			GameModeKey value = CacheDTO.GameMode.Value;
			if (value.type != GameModeType.PraticeMode)
			{
				GameModeKey value2 = CacheDTO.GameMode.Value;
				if (value2.type == GameModeType.Campaign)
				{
				}
			}
			string gameHostIP = CacheDTO.GameHostIP;
			int hostPort = CacheDTO.GameHostPort.HasValue ? CacheDTO.GameHostPort.Value : 0;
			string mapName = CacheDTO.MapName;
			GameModeKey? gameMode = CacheDTO.GameMode;
			BattleParametersData obj = new BattleParametersData(gameHostIP, hostPort, mapName, gameMode.Value, CacheDTO.ReceiveEnterBattleTime, CacheDTO.NetworkConfigs, null, CacheDTO.ReconnectGameGUID);
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<BattleParametersData> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
