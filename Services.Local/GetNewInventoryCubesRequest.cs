using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Local
{
	internal class GetNewInventoryCubesRequest : IGetNewInventoryCubesRequest, IServiceRequest, IAnswerOnComplete<HashSet<uint>>
	{
		private IServiceAnswer<HashSet<uint>> _answer;

		public void Execute()
		{
			if (CacheDTO.newInventoryCubes == null)
			{
				throw new Exception("New Inventory cubes was never initialised");
			}
			_answer.succeed(CacheDTO.newInventoryCubes);
		}

		public IServiceRequest SetAnswer(IServiceAnswer<HashSet<uint>> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
