using Svelto.ServiceLayer;

internal interface ILoadUserOptionsRequest : IServiceRequest, IAnswerOnComplete<UserOptionsData>
{
}
