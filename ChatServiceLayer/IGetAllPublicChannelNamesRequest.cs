using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IGetAllPublicChannelNamesRequest : IServiceRequest, IAnswerOnComplete<string[]>
	{
	}
}
