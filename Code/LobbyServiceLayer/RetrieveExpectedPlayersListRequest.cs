using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class RetrieveExpectedPlayersListRequest : IRetrieveExpectedPlayersListRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, PlayerDataDependency>>
	{
		private IServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>> _answer;

		public void Execute()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(CacheDTO.ExpectedPlayers);
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
