using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.TechTree
{
	internal interface IGetTechTreeDataRequest : IServiceRequest, IAnswerOnComplete<Dictionary<string, TechTreeItemData>>
	{
		void ClearCache();
	}
}
