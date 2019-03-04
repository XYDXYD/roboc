using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer
{
	internal interface IGetChatIgnoresRequest : IServiceRequest, IAnswerOnComplete<HashSet<string>>
	{
	}
}
