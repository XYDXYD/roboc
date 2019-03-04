using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IValidateBrawlRobotRequest : IServiceRequest, IAnswerOnComplete<ValidateRobotForBrawlResult>
	{
		void ClearCache();
	}
}
