using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal interface IGetAvailableGameModesRequest : IServiceRequest<LobbyType>, IAnswerOnComplete<List<GameModeType>>, IServiceRequest
	{
	}
}
