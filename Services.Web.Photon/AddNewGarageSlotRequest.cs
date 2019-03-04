using ExitGames.Client.Photon;
using Services.Mothership;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class AddNewGarageSlotRequest : WebServicesRequest<BuyGarageSlotResponse>, IBuyGarageSlotRequest, IServiceRequest, IAnswerOnComplete<BuyGarageSlotResponse>
	{
		protected override byte OperationCode => 38;

		public AddNewGarageSlotRequest()
			: base("strRobocloudError", "strUnableCompleteSlotPurchase", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override BuyGarageSlotResponse ProcessResponse(OperationResponse operationResponse)
		{
			uint num = (uint)Convert.ToInt32(operationResponse.Parameters[45]);
			BuyGarageSlotResponse buyGarageSlotResponse = new BuyGarageSlotResponse();
			buyGarageSlotResponse.garageIndex = num;
			buyGarageSlotResponse.newGarageSlotName = (string)operationResponse.Parameters[42];
			string value = (string)operationResponse.Parameters[40];
			string value2 = (string)operationResponse.Parameters[41];
			int masteryLevel = Convert.ToInt32(operationResponse.Parameters[18]);
			buyGarageSlotResponse.newGarageSlotUniqueIdentifier = new UniqueSlotIdentifier(Convert.ToUInt32(value), Convert.ToUInt32(value2));
			GarageSlotData garageSlotData = new GarageSlotData();
			garageSlotData.name = buyGarageSlotResponse.newGarageSlotName;
			garageSlotData.machineModel = new MachineModel();
			garageSlotData.uniqueId = buyGarageSlotResponse.newGarageSlotUniqueIdentifier;
			garageSlotData.weaponOrder = new WeaponOrderMothership(new int[0]);
			garageSlotData.movementCategories = new FasterList<ItemCategory>();
			buyGarageSlotResponse.masteryLevel = (garageSlotData.masteryLevel = masteryLevel);
			CacheDTO.garageSlots[num] = garageSlotData;
			return buyGarageSlotResponse;
		}
	}
}
