using rail;
using Svelto.ServiceLayer;

namespace Services.Local
{
	internal class GetLongPlayMultiplierRequest_Tencent : IGetLongPlayMultiplierRequest, IServiceRequest, IAnswerOnComplete<float>
	{
		private IServiceAnswer<float> _answer;

		public IServiceRequest SetAnswer(IServiceAnswer<float> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			float num = 1f;
			IRailFactory val = rail_api.RailFactory();
			IRailPlayer val2 = val.RailPlayer();
			num = val2.GetRateOfGameRevenue();
			if (_answer != null && _answer.succeed != null)
			{
				_answer.succeed(num);
			}
		}
	}
}
