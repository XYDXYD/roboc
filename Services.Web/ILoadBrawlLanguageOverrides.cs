using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web
{
	internal interface ILoadBrawlLanguageOverrides : IServiceRequest<string>, IAnswerOnComplete<BrawlOverrideLanguageStrings>, ITask, IServiceRequest, IAbstractTask
	{
		void ClearCache();
	}
}
