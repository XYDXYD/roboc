using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal interface IGetSocialSettingsRequest : IServiceRequest, IAnswerOnComplete<Dictionary<string, object>>
	{
	}
}
