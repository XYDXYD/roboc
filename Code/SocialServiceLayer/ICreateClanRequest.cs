using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface ICreateClanRequest<T> : IServiceRequest<T>, IAnswerOnComplete, IServiceRequest
	{
	}
}
