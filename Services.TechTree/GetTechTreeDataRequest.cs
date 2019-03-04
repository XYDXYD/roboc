using ExitGames.Client.Photon;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.TechTree
{
	internal class GetTechTreeDataRequest : WebServicesCachedRequest<Dictionary<string, TechTreeItemData>>, IGetTechTreeDataRequest, IServiceRequest, IAnswerOnComplete<Dictionary<string, TechTreeItemData>>
	{
		protected override byte OperationCode => 183;

		public GetTechTreeDataRequest()
			: base("strRobocloudError", "strUnableToGetTechTreeData", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override Dictionary<string, TechTreeItemData> ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[210];
			Dictionary<string, TechTreeItemData> dictionary2 = new Dictionary<string, TechTreeItemData>();
			foreach (KeyValuePair<string, Hashtable> item in dictionary)
			{
				string key = item.Key;
				Hashtable value = item.Value;
				TechTreeItemData value2 = new TechTreeItemData(Convert.ToUInt32((string)value.get_Item((object)"mainCubeId"), 16), Convert.ToInt32(value.get_Item((object)"positionX")), Convert.ToInt32(value.get_Item((object)"positionY")), Convert.ToBoolean(value.get_Item((object)"isUnlocked")), Convert.ToBoolean(value.get_Item((object)"isUnlockable")), (uint)Convert.ToInt32(value.get_Item((object)"tp")), (string[])value.get_Item((object)"neighbours"));
				dictionary2.Add(key, value2);
			}
			return dictionary2;
		}

		void IGetTechTreeDataRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
