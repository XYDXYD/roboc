using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IRetrieveExpectedPlayersListRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, PlayerDataDependency>>
	{
	}
}
