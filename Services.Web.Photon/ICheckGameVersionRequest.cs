using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ICheckGameVersionRequest : IServiceRequest, IAnswerOnComplete<CheckGameVersionData>
	{
	}
}
