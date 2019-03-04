using Svelto.ServiceLayer;

namespace Services.Local
{
	internal class SaveUserOptionsRequest : ISaveUserOptionsRequest, IServiceRequest<UserOptionsData>, IServiceRequest
	{
		private UserOptionsData _dependency;

		private IServiceAnswer<UserOptionsData> _answer;

		public SaveUserOptionsRequest()
		{
			_dependency = null;
		}

		public void Inject(UserOptionsData dependency)
		{
			_dependency = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<UserOptionsData> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(_dependency);
			}
		}
	}
}
