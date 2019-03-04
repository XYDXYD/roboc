using Svelto.ServiceLayer;

internal interface IApplyPromoCodeRequest : IServiceRequest<string>, IAnswerOnComplete<ApplyPromoCodeResponse>, IServiceRequest
{
}
