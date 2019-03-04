using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface ICheckPendingSanctionRequest : IServiceRequest, IAnswerOnComplete<bool>
	{
	}
}
