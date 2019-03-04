using Svelto.ServiceLayer;

internal interface IGetAccountRightsRequest : IServiceRequest, IAnswerOnComplete<AccountRights>
{
}
