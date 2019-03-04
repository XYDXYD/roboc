using Svelto.ServiceLayer;

namespace Services.Analytics.Tencent
{
	internal class LogLoadingRequest_Tencent : ILogLoadingRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _operationName;

		private IServiceAnswer _answer;

		public void Inject(string operationName)
		{
			_operationName = operationName;
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
