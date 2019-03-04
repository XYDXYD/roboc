using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface IGetDefaultWeaponOrderSubcategoriesRequest : IServiceRequest, IAnswerOnComplete<List<int>>
{
}
