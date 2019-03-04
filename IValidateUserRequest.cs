using Svelto.ServiceLayer;

internal interface IValidateUserRequest : IServiceRequest, IAnswerOnComplete<ValidateUserRequestData>
{
}
