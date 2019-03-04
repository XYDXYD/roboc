using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal interface IGetRoboPassPreviewItemsRequest : IServiceRequest, IAnswerOnComplete<IList<RoboPassPreviewItemDisplayData>>
	{
		void ClearCache();
	}
}
