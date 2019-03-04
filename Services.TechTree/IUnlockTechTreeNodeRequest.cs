using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.TechTree
{
	internal interface IUnlockTechTreeNodeRequest : IServiceRequest<string>, IAnswerOnComplete<Dictionary<string, TechTreeItemData>>, IServiceRequest
	{
	}
}
