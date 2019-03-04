using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogErrorRequest_Tencent : ILogErrorRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private IServiceAnswer _answer;

		private string _error;

		public void Inject(string error)
		{
			_error = error;
		}

		public void Execute()
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed();
			}
		}

		public IServiceRequest SetAnswer(IServiceAnswer answer)
		{
			_answer = answer;
			return this;
		}
	}
}
