using Svelto.ServiceLayer;
using System.IO;

namespace Services.Local
{
	internal class SaveHintProgressRequest : ISaveHintProgressRequest, IServiceRequest<byte[]>, IAnswerOnComplete<byte[]>, IServiceRequest
	{
		private const string LOCAL_PATH = "HintProgress.txt";

		private byte[] _dependency;

		private IServiceAnswer<byte[]> _answer;

		public SaveHintProgressRequest()
		{
			_dependency = null;
		}

		public void Inject(byte[] dependency)
		{
			_dependency = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<byte[]> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			using (FileStream fileStream = new FileStream("HintProgress.txt", FileMode.Create, FileAccess.Write))
			{
				fileStream.Write(_dependency, 0, _dependency.Length);
			}
			CacheDTO.hintProgresses = _dependency;
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(_dependency);
			}
		}
	}
}
