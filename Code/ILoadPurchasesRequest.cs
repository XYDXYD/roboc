using Svelto.DataStructures;
using Svelto.ServiceLayer;

internal interface ILoadPurchasesRequest : IServiceRequest, IAnswerOnComplete<FasterList<PurchaseRequestData>>
{
}
