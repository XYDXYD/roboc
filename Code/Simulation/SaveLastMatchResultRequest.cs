using Services.Local;
using Svelto.ServiceLayer;

namespace Simulation
{
	internal sealed class SaveLastMatchResultRequest : ISaveLastMatchResultRequest, IServiceRequest<bool>, IAnswerOnComplete, IServiceRequest
	{
		private bool _dependency;

		private IServiceAnswer _answer;

		public SaveLastMatchResultRequest()
		{
			_dependency = false;
		}

		public void Inject(bool dependency)
		{
			_dependency = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			CacheDTO.lastMatchResult = _dependency;
			CacheDTO.lastMatchResultIsValid = true;
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed();
			}
		}
	}
}
