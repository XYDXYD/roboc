using Svelto.ServiceLayer;

internal interface IGetLongPlayMultiplierRequest : IServiceRequest, IAnswerOnComplete<float>
{
}
