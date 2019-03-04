using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IGetMultilayerAvatarInfoRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, AvatarInfo>>
	{
	}
}
