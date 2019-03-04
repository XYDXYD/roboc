using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IPingReconnectableGameRequest : IServiceRequest, IAnswerOnComplete<bool>
	{
	}
}
