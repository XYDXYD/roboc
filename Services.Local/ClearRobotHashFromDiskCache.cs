using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace Services.Local
{
	internal class ClearRobotHashFromDiskCache : IClearRobotHashFromDiskCache, IServiceRequest, IAnswerOnComplete
	{
		private IServiceAnswer _answer;

		private ServiceBehaviour _serviceBehaviour;

		public ClearRobotHashFromDiskCache()
		{
			_serviceBehaviour = new ServiceBehaviour("strGenericError", "strGenericErrorQuit");
		}

		public IServiceRequest SetAnswer(IServiceAnswer answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			CacheDTO.slotVersionInfoCache = new Dictionary<UniqueSlotIdentifier, uint>();
			if (CacheDTO.slotVersionInfoCache.Count == 0)
			{
				string garageThumbnailVersionFilePath = TextureCacheHelper.GetGarageThumbnailVersionFilePath();
				if (File.Exists(garageThumbnailVersionFilePath))
				{
					File.Delete(garageThumbnailVersionFilePath);
				}
			}
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed();
			}
			else if (_answer != null && _answer.failed != null)
			{
				_answer.failed(_serviceBehaviour);
			}
		}
	}
}
