using ExitGames.Client.Photon;
using Services.Mothership;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadPlayerDataRequest : WebServicesRequest<PlayerDataResponse>, ILoadPlayerDataRequest, ITask, IServiceRequest, IAnswerOnComplete<PlayerDataResponse>, IAbstractTask
	{
		protected override byte OperationCode => 61;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadPlayerDataRequest()
			: base("strRobocloudError", "strUnableToLoadPlayerStats", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override PlayerDataResponse ProcessResponse(OperationResponse operationResponse)
		{
			PlayerDataResponse playerDataResponse = new PlayerDataResponse();
			playerDataResponse.garageSlotId = (uint)Convert.ToInt32(operationResponse.Parameters[45]);
			playerDataResponse.name = (string)operationResponse.Parameters[42];
			playerDataResponse.crfId = (uint)Convert.ToInt32(operationResponse.Parameters[35]);
			playerDataResponse.robotCPU = (uint)Convert.ToInt32(operationResponse.Parameters[177]);
			int controlType = Convert.ToInt32(operationResponse.Parameters[59]);
			bool[] controlOptions = (bool[])operationResponse.Parameters[60];
			playerDataResponse.controlSetting = new ControlSettings(controlType, controlOptions);
			int[] weaponOrderList = (int[])operationResponse.Parameters[52];
			playerDataResponse.weaponOrder = new WeaponOrderMothership(weaponOrderList);
			int[] array = (int[])operationResponse.Parameters[56];
			playerDataResponse.movementCategories = new FasterList<ItemCategory>(array.Length);
			foreach (int num in array)
			{
				int itemCategory = 0;
				ItemDescriptorKey.GetItemCategoryFromKey(num, out itemCategory);
				if (itemCategory == 0)
				{
					Console.LogError("Invalid Movement! " + num);
				}
				playerDataResponse.movementCategories.Add((ItemCategory)itemCategory);
			}
			isDone = true;
			return playerDataResponse;
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}
	}
}
