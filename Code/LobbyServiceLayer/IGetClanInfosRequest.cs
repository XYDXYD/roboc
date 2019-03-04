using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IGetClanInfosRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, ClanInfo>>
	{
	}
}
