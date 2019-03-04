using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface ISetClanInfosRequest : IServiceRequest<ReadOnlyDictionary<string, ClanInfo>>, IServiceRequest
	{
	}
}
