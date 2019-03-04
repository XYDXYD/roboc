namespace Simulation
{
	internal sealed class GameStateClient
	{
		private bool _gameEnded;

		private GameStateResult _battleEndResult;

		private GameServerErrorCodes _gameServerErrorCode;

		public bool hasGameEnded => _gameEnded;

		public GameStateResult battleEndResult => _battleEndResult;

		public GameServerErrorCodes gameServerErrorCode => _gameServerErrorCode;

		public void ChangeStateToGameEnded(bool gameEnded)
		{
			_gameEnded = gameEnded;
		}

		public void ChangeStateToGameEnded(GameStateResult battleEndResult)
		{
			_gameEnded = true;
			_battleEndResult = battleEndResult;
		}

		public void ChangeStateToGameEnded(GameStateResult battleEndResult, GameEndReason gameEndReason)
		{
			_gameEnded = true;
			if (gameEndReason == GameEndReason.Surrendered)
			{
				switch (battleEndResult)
				{
				case GameStateResult.Won:
					battleEndResult = GameStateResult.OtherTeamSurrendered;
					break;
				case GameStateResult.Lost:
					battleEndResult = GameStateResult.TeamSurrendered;
					break;
				}
			}
			_battleEndResult = battleEndResult;
		}

		public void ChangeStateToGameEnded(GameStateResult battleEndResult, GameServerErrorCodes gameServerErrorCode)
		{
			_gameServerErrorCode = gameServerErrorCode;
			ChangeStateToGameEnded(battleEndResult);
		}
	}
}
