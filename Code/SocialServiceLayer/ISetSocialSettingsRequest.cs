using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface ISetSocialSettingsRequest : IServiceRequest<Dictionary<string, object>>, IAnswerOnComplete, IServiceRequest
	{
	}
}
