using ExitGames.Client.Photon;
using Services.Mothership;
using Services.Web.Internal;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal class GetGarageSlotsRequest : WebServicesRequest<LoadGarageDataRequestResponse>, ILoadGarageDataRequest, ITask, IServiceRequest, IAnswerOnComplete<LoadGarageDataRequestResponse>, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public virtual bool includeTutorialBots => false;

		protected override byte OperationCode => 40;

		public GetGarageSlotsRequest()
			: base("strRobocloudError", "strUnableLoadGarage", 3)
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

		protected override LoadGarageDataRequestResponse ProcessResponse(OperationResponse operationResponse)
		{
			LoadGarageDataRequestResponse loadGarageDataRequestResponse = new LoadGarageDataRequestResponse();
			FasterList<GarageSlotDependency> val = new FasterList<GarageSlotDependency>();
			FasterList<int> val2 = new FasterList<int>();
			Dictionary<int, Hashtable> dictionary = (Dictionary<int, Hashtable>)operationResponse.Parameters[44];
			int num = Convert.ToInt32(operationResponse.Parameters[43]);
			CacheDTO.currentGarageSlot = new uint[1]
			{
				(uint)num
			};
			loadGarageDataRequestResponse.currentGarageSlot = (uint)num;
			CacheDTO.garageSlotOrder.FastClear();
			object[] array = (object[])operationResponse.Parameters[58];
			for (int i = 0; i < array.Length; i++)
			{
				val2.Add(Convert.ToInt32(array[i]));
			}
			loadGarageDataRequestResponse.garageSlotOrder = val2;
			CacheDTO.garageSlotOrder.AddRange(val2);
			CacheDTO.garageSlots.Clear();
			foreach (KeyValuePair<int, Hashtable> item in dictionary)
			{
				Hashtable value = item.Value;
				int key = item.Key;
				GarageSlotDependency garageSlotDependency = new GarageSlotDependency((uint)key);
				garageSlotDependency.name = (string)value.get_Item((object)"name");
				garageSlotDependency.numberCubes = Convert.ToUInt32(value.get_Item((object)"numberCubes"));
				garageSlotDependency.crfId = (uint)Convert.ToInt32(value.get_Item((object)"crfId"));
				garageSlotDependency.canBeRated = (garageSlotDependency.crfId != 0 && !Convert.ToBoolean(value.get_Item((object)"wasRated")));
				int[] array2 = (int[])value.get_Item((object)"movementCategories");
				garageSlotDependency.movementCategories = new FasterList<ItemCategory>();
				foreach (int num2 in array2)
				{
					int itemCategory = 0;
					ItemDescriptorKey.GetItemCategoryFromKey(num2, out itemCategory);
					if (itemCategory == 0)
					{
						Console.LogError("Invalid Movement! " + num2);
					}
					garageSlotDependency.movementCategories.Add((ItemCategory)itemCategory);
				}
				uint keyPart = (uint)Convert.ToInt64(value.get_Item((object)"uniqueId1"));
				uint keyPart2 = (uint)Convert.ToInt64(value.get_Item((object)"uniqueId2"));
				garageSlotDependency.uniqueSlotId = new UniqueSlotIdentifier(keyPart, keyPart2);
				garageSlotDependency.remoteThumbnailVersionNumber = Convert.ToUInt32(value.get_Item((object)"thumbnailVersion"));
				garageSlotDependency.totalRobotCPU = Convert.ToUInt32(value.get_Item((object)"totalRobotCPU"));
				garageSlotDependency.totalCosmeticCPU = Convert.ToUInt32(value.get_Item((object)"totalCosmeticCPU"));
				garageSlotDependency.totalRobotRanking = Convert.ToUInt32(value.get_Item((object)"totalRobotRanking"));
				garageSlotDependency.tutorialRobot = false;
				if (((Dictionary<object, object>)value).ContainsKey((object)"tutorialRobot"))
				{
					garageSlotDependency.tutorialRobot = Convert.ToBoolean(value.get_Item((object)"tutorialRobot"));
				}
				garageSlotDependency.starterRobotIndex = -1;
				if (((Dictionary<object, object>)value).ContainsKey((object)"starterRobotIndex"))
				{
					garageSlotDependency.starterRobotIndex = Convert.ToInt32(value.get_Item((object)"starterRobotIndex"));
				}
				int controlType = Convert.ToInt32(value.get_Item((object)"controlType"));
				bool[] controlOptions = (bool[])value.get_Item((object)"controlOptions");
				garageSlotDependency.controlSetting = new ControlSettings(controlType, controlOptions);
				int num3 = garageSlotDependency.masteryLevel = Convert.ToInt32(value.get_Item((object)"masteryLevel"));
				string baySkinID = garageSlotDependency.baySkinID = Convert.ToString(value.get_Item((object)"baySkinId"));
				WeaponOrderMothership weaponOrder = garageSlotDependency.weaponOrder = new WeaponOrderMothership((int[])value.get_Item((object)"weaponOrder"));
				val.Add(garageSlotDependency);
				GarageSlotData garageSlotData = new GarageSlotData();
				if (CacheDTO.garageSlots.ContainsKey(garageSlotDependency.garageSlot))
				{
					garageSlotData = CacheDTO.garageSlots[garageSlotDependency.garageSlot];
				}
				garageSlotData.name = garageSlotDependency.name;
				garageSlotData.movementCategories = garageSlotDependency.movementCategories;
				garageSlotData.uniqueId = garageSlotDependency.uniqueSlotId;
				garageSlotData.controlSetting = garageSlotDependency.controlSetting;
				garageSlotData.crfId = garageSlotDependency.crfId;
				garageSlotData.totalRobotCPU = garageSlotDependency.totalRobotCPU;
				garageSlotData.totalCosmeticCPU = garageSlotDependency.totalCosmeticCPU;
				garageSlotData.isReadOnlyRobot = false;
				garageSlotData.masteryLevel = garageSlotDependency.masteryLevel;
				garageSlotData.starterRobotIndex = garageSlotDependency.starterRobotIndex;
				garageSlotData.baySkinID = baySkinID;
				garageSlotData.weaponOrder = weaponOrder;
				garageSlotData.tutorialRobot = garageSlotDependency.tutorialRobot;
				CacheDTO.garageSlots[garageSlotDependency.garageSlot] = garageSlotData;
			}
			loadGarageDataRequestResponse.garageSlots = val;
			isDone = true;
			return loadGarageDataRequestResponse;
		}

		public static string ConvertToHexString(uint val)
		{
			return Convert.ToString(val, 16).PadLeft(8, '0');
		}
	}
}
