using Svelto.ServiceLayer;
using System;
using System.IO;

namespace Services.Local
{
	internal class LoadRobotHashFromDiskCache : ILoadRobotHashFromDiskCache, IServiceRequest<LoadRobotHashFromDiskCacheDependency>, IAnswerOnComplete<uint>, IServiceRequest
	{
		private LoadRobotHashFromDiskCacheDependency _inputParamaters;

		private IServiceAnswer<uint> _answer;

		private ServiceBehaviour _serviceBehaviour;

		public LoadRobotHashFromDiskCache()
		{
			_serviceBehaviour = new ServiceBehaviour("strGenericError", "strGenericErrorQuit");
		}

		public void Inject(LoadRobotHashFromDiskCacheDependency dependency)
		{
			_inputParamaters = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<uint> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (CacheDTO.slotVersionInfoCache.Count == 0)
			{
				string garageThumbnailVersionFilePath = TextureCacheHelper.GetGarageThumbnailVersionFilePath();
				if (File.Exists(garageThumbnailVersionFilePath))
				{
					try
					{
						string[] array = File.ReadAllLines(garageThumbnailVersionFilePath);
						for (int i = 0; i < array.Length; i++)
						{
							string[] array2 = array[i].Split(':');
							string[] array3 = array2[0].Split('_');
							UniqueSlotIdentifier key = new UniqueSlotIdentifier(Convert.ToUInt32(array3[0]), Convert.ToUInt32(array3[1]));
							if (uint.TryParse(array2[1], out uint result))
							{
								CacheDTO.slotVersionInfoCache[key] = result;
							}
						}
					}
					catch (Exception ex)
					{
						RemoteLogger.Error("Corruption in Robot thumbnail Hash file", ex.Message, ex.StackTrace);
						File.Delete(garageThumbnailVersionFilePath);
					}
				}
			}
			if (_answer != null && _answer.succeed != null)
			{
				if (CacheDTO.slotVersionInfoCache.TryGetValue(_inputParamaters.uniqueSlotIdentifier, out uint value))
				{
					_answer.succeed(value);
				}
				else
				{
					_answer.succeed(0u);
				}
			}
			else if (_answer != null && _answer.failed != null)
			{
				_answer.failed(_serviceBehaviour);
			}
		}
	}
}
