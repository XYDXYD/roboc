using Svelto.DataStructures;
using Svelto.ServiceLayer;

internal interface ILoadSpecialItemListRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<uint, SpecialItemListData>>
{
}
