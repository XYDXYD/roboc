using Services.Mothership;
using Services.Web.Internal;
using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal sealed class GetCurrentGarageRequest : IGetCurrentGarageRequest, IServiceRequest, IAnswerOnComplete<GarageSlotData>
	{
		private IServiceAnswer<GarageSlotData> _answer;

		void IServiceRequest.Execute()
		{
			IServiceAnswer<GarageSlotData> answer = _answer;
			GarageSlotData value = null;
			if (CacheDTO.garageSlots != null && CacheDTO.currentGarageSlot != null)
			{
				CacheDTO.garageSlots.TryGetValue(CacheDTO.currentGarageSlot[0], out value);
			}
			answer.succeed(value);
		}

		public IServiceRequest SetAnswer(IServiceAnswer<GarageSlotData> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
