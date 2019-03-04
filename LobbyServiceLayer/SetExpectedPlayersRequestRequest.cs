using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class SetExpectedPlayersRequestRequest : ISetExpectedPlayerRequest, IServiceRequest<ReadOnlyDictionary<string, PlayerDataDependency>>, IServiceRequest
	{
		private ReadOnlyDictionary<string, PlayerDataDependency> _expectedPlayers;

		public void Inject(ReadOnlyDictionary<string, PlayerDataDependency> dependency)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_expectedPlayers = dependency;
		}

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CacheDTO.ExpectedPlayers = _expectedPlayers;
		}
	}
}
