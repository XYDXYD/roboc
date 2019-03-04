using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadMovementStatsRequest : WebServicesCachedRequest<MovementStats>, ILoadMovementStatsRequest, ITask, IServiceRequest, IAnswerOnComplete<MovementStats>, IAbstractTask
	{
		protected override byte OperationCode => 62;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadMovementStatsRequest()
			: base("strRobocloudError", "strLoadMovementStatsError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override MovementStats ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[1];
			Hashtable val = dictionary["Global"];
			float lerpValue = 1f;
			if (((Dictionary<object, object>)val).ContainsKey((object)"lerpValue"))
			{
				lerpValue = (float)Convert.ToDouble(val.get_Item((object)"lerpValue"));
			}
			Dictionary<int, MovementStatsData> dictionary2 = new Dictionary<int, MovementStatsData>();
			Dictionary<ItemCategory, IMovementCategoryData> dictionary3 = new Dictionary<ItemCategory, IMovementCategoryData>();
			Hashtable val2 = dictionary["Movements"];
			foreach (DictionaryEntry item in val2)
			{
				ItemCategory itemCategory = (ItemCategory)Enum.Parse(typeof(ItemCategory), (string)item.Key);
				Dictionary<string, object> dictionary4 = (Dictionary<string, object>)item.Value;
				float horizontalTopSpeed = 0f;
				if (dictionary4.ContainsKey("horizontalTopSpeed"))
				{
					horizontalTopSpeed = (float)Convert.ToDouble(dictionary4["horizontalTopSpeed"]);
				}
				float verticalTopSpeed = 0f;
				if (dictionary4.ContainsKey("verticalTopSpeed"))
				{
					verticalTopSpeed = (float)Convert.ToDouble(dictionary4["verticalTopSpeed"]);
				}
				int minRequiredItems = 1;
				if (dictionary4.ContainsKey("minRequiredItems"))
				{
					minRequiredItems = Convert.ToInt32(dictionary4["minRequiredItems"]);
				}
				float minRequiredItemsModifier = 1f;
				if (dictionary4.ContainsKey("minItemsModifier"))
				{
					minRequiredItemsModifier = (float)Convert.ToDouble(dictionary4["minItemsModifier"]);
				}
				foreach (KeyValuePair<string, object> item2 in dictionary4)
				{
					if (Enum.IsDefined(typeof(ItemSize), item2.Key))
					{
						ItemSize itemSize = (ItemSize)Enum.Parse(typeof(ItemSize), item2.Key);
						Dictionary<string, object> dictionary5 = (Dictionary<string, object>)item2.Value;
						float speedBoost = 0f;
						if (dictionary5.ContainsKey("speedBoost"))
						{
							speedBoost = (float)Convert.ToDouble(dictionary5["speedBoost"]);
						}
						float maxCarryMass = 0f;
						if (dictionary5.ContainsKey("maxCarryMass"))
						{
							maxCarryMass = (float)Convert.ToDouble(dictionary5["maxCarryMass"]);
						}
						if (dictionary5.ContainsKey("horizontalTopSpeed"))
						{
							horizontalTopSpeed = (float)Convert.ToDouble(dictionary5["horizontalTopSpeed"]);
						}
						if (dictionary5.ContainsKey("verticalTopSpeed"))
						{
							verticalTopSpeed = (float)Convert.ToDouble(dictionary5["verticalTopSpeed"]);
						}
						MovementStatsData value = new MovementStatsData(horizontalTopSpeed, verticalTopSpeed, speedBoost, maxCarryMass, minRequiredItems, minRequiredItemsModifier);
						dictionary2.Add(ItemDescriptorKey.GenerateKey(itemCategory, itemSize), value);
					}
				}
				IMovementCategoryData movementCategoryData = ParseCategoryData(itemCategory, dictionary4);
				if (movementCategoryData != null)
				{
					dictionary3.Add(itemCategory, movementCategoryData);
				}
			}
			isDone = true;
			return new MovementStats(lerpValue, dictionary2, dictionary3);
		}

		private IMovementCategoryData ParseCategoryData(ItemCategory itemCategory, Dictionary<string, object> movementStat)
		{
			if (itemCategory == ItemCategory.Hover)
			{
				return new HoverData(movementStat);
			}
			return null;
		}

		void ILoadMovementStatsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
