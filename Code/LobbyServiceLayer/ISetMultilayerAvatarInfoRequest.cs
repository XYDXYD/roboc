using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface ISetMultilayerAvatarInfoRequest : IServiceRequest<ReadOnlyDictionary<string, AvatarInfo>>, IServiceRequest
	{
	}
}
