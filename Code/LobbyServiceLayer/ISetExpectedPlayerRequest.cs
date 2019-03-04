using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface ISetExpectedPlayerRequest : IServiceRequest<ReadOnlyDictionary<string, PlayerDataDependency>>, IServiceRequest
	{
	}
}
