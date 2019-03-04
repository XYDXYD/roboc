using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogClanLeftRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
