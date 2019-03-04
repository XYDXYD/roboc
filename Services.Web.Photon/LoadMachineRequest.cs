using ExitGames.Client.Photon;
using Services.Mothership;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadMachineRequest : WebServicesRequest<LoadMachineResult>, ILoadMachineRequest, IServiceRequest<uint?>, IAnswerOnComplete<LoadMachineResult>, IServiceRequest
	{
		private uint? _garageSlot;

		protected override byte OperationCode => 43;

		private int garageSlot
		{
			get
			{
				uint? garageSlot = _garageSlot;
				if (garageSlot.HasValue)
				{
					uint? garageSlot2 = _garageSlot;
					return (int)garageSlot2.Value;
				}
				if (CacheDTO.currentGarageSlot != null)
				{
					_garageSlot = CacheDTO.currentGarageSlot[0];
					uint? garageSlot3 = _garageSlot;
					return (int)garageSlot3.Value;
				}
				return -1;
			}
		}

		public LoadMachineRequest()
			: base("strRobocloudError", "strUnableLoadRobot", 3)
		{
		}

		public void ForceClearCache()
		{
			CacheDTO.currentGarageSlot = null;
		}

		public void Inject(uint? garageSlot)
		{
			if (!garageSlot.HasValue && CacheDTO.currentGarageSlot != null)
			{
				_garageSlot = CacheDTO.currentGarageSlot[0];
			}
			else
			{
				_garageSlot = garageSlot;
			}
		}

		public override void Execute()
		{
			if (CacheDTO.currentGarageSlot == null || garageSlot == -1 || !CacheDTO.garageSlots.ContainsKey((uint)garageSlot) || CacheDTO.garageSlots[(uint)garageSlot].machineModel == null)
			{
				base.Execute();
			}
			else
			{
				OnParseSuccess(base.answer);
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[45] = garageSlot;
			val.OperationCode = OperationCode;
			return val;
		}

		protected override LoadMachineResult ProcessResponse(OperationResponse operationResponse)
		{
			uint num = (uint)Convert.ToInt32(operationResponse.Parameters[45]);
			uint? garageSlot = _garageSlot;
			if (!garageSlot.HasValue)
			{
				if (CacheDTO.currentGarageSlot == null)
				{
					CacheDTO.currentGarageSlot = new uint[1];
				}
				CacheDTO.currentGarageSlot[0] = num;
				_garageSlot = num;
			}
			byte[] data = (byte[])operationResponse.Parameters[49];
			int cubesNumber = Convert.ToInt32(operationResponse.Parameters[51]);
			int[] weaponOrderList = (int[])operationResponse.Parameters[52];
			int[] array = (int[])operationResponse.Parameters[56];
			int controlType = Convert.ToInt32(operationResponse.Parameters[59]);
			bool[] controlOptions = (bool[])operationResponse.Parameters[60];
			int masteryLevel = Convert.ToInt32(operationResponse.Parameters[18]);
			MachineModel machineModel = new MachineModel(data);
			if (!CacheDTO.garageSlots.ContainsKey(num))
			{
				CacheDTO.garageSlots.Add(num, new GarageSlotData());
			}
			CacheDTO.garageSlots[num].controlSetting = new ControlSettings(controlType, controlOptions);
			CacheDTO.garageSlots[num].masteryLevel = masteryLevel;
			CacheDTO.garageSlots[num].isReadOnlyRobot = false;
			CacheDTO.garageSlots[num].machineModel = machineModel;
			CacheDTO.garageSlots[num].cubesNumber = (uint)cubesNumber;
			CacheDTO.garageSlots[num].weaponOrder = new WeaponOrderMothership(weaponOrderList);
			CacheDTO.garageSlots[num].movementCategories = new FasterList<ItemCategory>(array.Length);
			foreach (int num2 in array)
			{
				int itemCategory = 0;
				ItemDescriptorKey.GetItemCategoryFromKey(num2, out itemCategory);
				if (itemCategory == 0)
				{
					Console.LogError("Invalid Movement! " + num2);
				}
				CacheDTO.garageSlots[num].movementCategories.Add((ItemCategory)itemCategory);
			}
			return GetLoadMachineResultFromCacheDTO();
		}

		private void OnParseSuccess(IServiceAnswer<LoadMachineResult> answer)
		{
			if (answer != null && answer.succeed != null)
			{
				LoadMachineResult loadMachineResultFromCacheDTO = GetLoadMachineResultFromCacheDTO();
				answer.succeed(loadMachineResultFromCacheDTO);
			}
		}

		private LoadMachineResult GetLoadMachineResultFromCacheDTO()
		{
			LoadMachineResult loadMachineResult = new LoadMachineResult();
			uint? garageSlot = _garageSlot;
			uint key = loadMachineResult.garageSlot = garageSlot.Value;
			GarageSlotData garageSlotData = CacheDTO.garageSlots[key];
			loadMachineResult.model = new MachineModel(garageSlotData.machineModel);
			loadMachineResult.cubesNumber = garageSlotData.cubesNumber;
			loadMachineResult.weaponOrder = garageSlotData.weaponOrder;
			loadMachineResult.movementCategories = garageSlotData.movementCategories;
			loadMachineResult.controlSettings = garageSlotData.controlSetting;
			loadMachineResult.isReadOnlyRobot = false;
			loadMachineResult.MasteryLevel = garageSlotData.masteryLevel;
			return loadMachineResult;
		}
	}
}
