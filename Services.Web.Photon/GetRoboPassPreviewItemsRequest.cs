using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class GetRoboPassPreviewItemsRequest : WebServicesCachedRequest<IList<RoboPassPreviewItemDisplayData>>, IGetRoboPassPreviewItemsRequest, IServiceRequest, IAnswerOnComplete<IList<RoboPassPreviewItemDisplayData>>
	{
		protected override byte OperationCode => 167;

		public GetRoboPassPreviewItemsRequest()
			: base("strRobocloudError", "strUnableLoadRoboPassPreviewItems", 3)
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

		protected override IList<RoboPassPreviewItemDisplayData> ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[1];
			List<RoboPassPreviewItemDisplayData> list = new List<RoboPassPreviewItemDisplayData>();
			foreach (KeyValuePair<string, Hashtable> item2 in dictionary)
			{
				int num = Convert.ToInt32(item2.Key);
				Hashtable value = item2.Value;
				if (num < dictionary.Keys.Count && num >= 0 && ((Dictionary<object, object>)value).ContainsKey((object)"ItemSprite") && ((Dictionary<object, object>)value).ContainsKey((object)"NameString") && ((Dictionary<object, object>)value).ContainsKey((object)"CategoryString"))
				{
					RoboPassPreviewItemDisplayData item = default(RoboPassPreviewItemDisplayData);
					item.SpriteName = (string)value.get_Item((object)"ItemSprite");
					item.SpriteFullSize = (bool)value.get_Item((object)"SpriteFullSize");
					item.Name = (string)value.get_Item((object)"NameString");
					item.Category = (string)value.get_Item((object)"CategoryString");
					list.Add(item);
				}
				else
				{
					Console.LogWarning("Incorrect formatting of preview item data: either the name, sprite, or category fields are missing or the index is set wrongly");
				}
			}
			return list;
		}

		void IGetRoboPassPreviewItemsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
