using Services.Mothership;
using Services.Web.Internal;
using Svelto.ServiceLayer;

namespace Services.Web
{
	internal sealed class ClearMachineRequest : IClearMachineRequest, IServiceRequest<uint>, IServiceRequest
	{
		private uint _garageSlot;

		public void Inject(uint dependency)
		{
			_garageSlot = dependency;
		}

		public void Execute()
		{
			GarageSlotData garageSlotData = CacheDTO.garageSlots[_garageSlot];
			garageSlotData.machineModel = new MachineModel();
		}
	}
}
