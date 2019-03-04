using Svelto.ServiceLayer;

namespace Services.TechTree
{
	internal interface IGetTechPointsAwardRequest : IServiceRequest, IAnswerOnComplete<int>
	{
	}
}
