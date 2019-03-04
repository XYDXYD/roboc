using Svelto.ServiceLayer;

namespace Services.Local
{
	internal class GetLongPlayMultiplierRequest : IGetLongPlayMultiplierRequest, IServiceRequest, IAnswerOnComplete<float>
	{
		private IServiceAnswer<float> _answer;

		public IServiceRequest SetAnswer(IServiceAnswer<float> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			float obj = 1f;
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(obj);
			}
		}
	}
}
