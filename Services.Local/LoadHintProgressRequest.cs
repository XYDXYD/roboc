using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace Services.Local
{
	internal class LoadHintProgressRequest : ILoadHintProgressRequest, IServiceRequest, IAnswerOnComplete<List<byte>>
	{
		private const string LOCAL_PATH = "HintProgress.txt";

		private IServiceAnswer<List<byte>> _answer;

		public IServiceRequest SetAnswer(IServiceAnswer<List<byte>> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (CacheDTO.hintProgresses == null)
			{
				string path = "HintProgress.txt";
				if (File.Exists(path))
				{
					using (FileStream fileStream = new FileStream("HintProgress.txt", FileMode.Open, FileAccess.Read))
					{
						CacheDTO.hintProgresses = new byte[(int)fileStream.Length];
						fileStream.Read(CacheDTO.hintProgresses, 0, (int)fileStream.Length);
					}
				}
				else
				{
					CacheDTO.hintProgresses = new byte[0];
				}
			}
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(new List<byte>(CacheDTO.hintProgresses));
			}
		}
	}
}
