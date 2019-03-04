using Svelto.ServiceLayer;

namespace Services.Local
{
	internal class LoadUserOptionsRequest : ILoadUserOptionsRequest, IServiceRequest, IAnswerOnComplete<UserOptionsData>
	{
		private IServiceAnswer<UserOptionsData> _answer;

		public IServiceRequest SetAnswer(IServiceAnswer<UserOptionsData> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			UserOptionsData obj = new UserOptionsData();
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
			}
		}
	}
}
