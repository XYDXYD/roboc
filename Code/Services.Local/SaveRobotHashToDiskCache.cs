using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Services.Local
{
	internal class SaveRobotHashToDiskCache : ISaveRobotHashToDiskCache, IServiceRequest<SaveRobotHashToDiskCacheDependency>, IAnswerOnComplete<uint>, IServiceRequest
	{
		private SaveRobotHashToDiskCacheDependency _params;

		private IServiceAnswer<uint> _answer;

		private ServiceBehaviour _serviceBehaviour;

		public SaveRobotHashToDiskCache()
		{
			_serviceBehaviour = new ServiceBehaviour("strGenericError", "strGenericErrorQuit");
		}

		public void Inject(SaveRobotHashToDiskCacheDependency dependency)
		{
			_params = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<uint> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			string cacheFolder = TextureCacheHelper.GetCacheFolder();
			if (!Directory.Exists(cacheFolder))
			{
				Directory.CreateDirectory(cacheFolder);
			}
			if (CacheDTO.slotVersionInfoCache.ContainsKey(_params.uniqueIdToIncrement))
			{
				if (_params.versionNumberToSetIfAny == 0)
				{
					uint num = CacheDTO.slotVersionInfoCache[_params.uniqueIdToIncrement];
					num++;
					CacheDTO.slotVersionInfoCache[_params.uniqueIdToIncrement] = num;
				}
				else
				{
					CacheDTO.slotVersionInfoCache[_params.uniqueIdToIncrement] = _params.versionNumberToSetIfAny;
				}
			}
			else
			{
				CacheDTO.slotVersionInfoCache[_params.uniqueIdToIncrement] = _params.versionNumberToSetIfAny;
			}
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<UniqueSlotIdentifier, uint>.Enumerator enumerator = CacheDTO.slotVersionInfoCache.GetEnumerator();
			while (enumerator.MoveNext())
			{
				stringBuilder.AppendFormat("{0}:{1}\r\n", enumerator.Current.Key.ToString(), enumerator.Current.Value);
			}
			string garageThumbnailVersionFilePath = TextureCacheHelper.GetGarageThumbnailVersionFilePath();
			File.WriteAllText(garageThumbnailVersionFilePath, stringBuilder.ToString());
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(CacheDTO.slotVersionInfoCache[_params.uniqueIdToIncrement]);
			}
			else if (_answer != null && _answer.failed != null)
			{
				_answer.failed(_serviceBehaviour);
			}
		}
	}
}
