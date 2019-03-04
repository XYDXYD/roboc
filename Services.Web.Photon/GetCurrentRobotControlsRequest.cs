using Services.Mothership;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;

namespace Services.Web.Photon
{
	internal sealed class GetCurrentRobotControlsRequest : IGetCurrentRobotControlsRequest, IServiceRequest, IAnswerOnComplete<GetRobotControlsResult>
	{
		private IServiceAnswer<GetRobotControlsResult> _answer;

		void IServiceRequest.Execute()
		{
			IServiceAnswer<GetRobotControlsResult> answer = _answer;
			GarageSlotData value;
			if (answer != null && CacheDTO.garageSlots.TryGetValue(CacheDTO.currentGarageSlot[0], out value) && value != null)
			{
				answer.succeed(new GetRobotControlsResult(value.controlSetting, CacheDTO.currentGarageSlot[0]));
				return;
			}
			throw new Exception("Answer is null or wrong type");
		}

		public IServiceRequest SetAnswer(IServiceAnswer<GetRobotControlsResult> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
