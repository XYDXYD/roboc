using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Local
{
	internal class SetNewInventoryCubesRequest : ISetNewInventoryCubesRequest, IServiceRequest<HashSet<uint>>, IAnswerOnComplete, IServiceRequest
	{
		private HashSet<uint> _dependency;

		private IServiceAnswer _answer;

		public void Inject(HashSet<uint> dependency)
		{
			_dependency = dependency;
		}

		public void Execute()
		{
			if (_dependency.Count == 0)
			{
				CacheDTO.newInventoryCubes.Clear();
			}
			else
			{
				CacheDTO.newInventoryCubes.UnionWith(_dependency);
			}
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
