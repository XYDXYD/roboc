using Services.Local;
using Svelto.ServiceLayer;

namespace Mothership
{
	internal sealed class LoadLastMatchResultRequest : ILoadLastMatchResultRequest, IServiceRequest, IAnswerOnComplete<LastMatchResultData>
	{
		private IServiceAnswer<LastMatchResultData> _answer;

		public void Execute()
		{
			LastMatchResultData obj = new LastMatchResultData(CacheDTO.lastMatchResult, CacheDTO.lastMatchResultIsValid);
			CacheDTO.lastMatchResultIsValid = false;
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer<LastMatchResultData> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
