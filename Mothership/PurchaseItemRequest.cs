using RoboCraft.MiniJSON;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class PurchaseItemRequest
	{
		private string _errorDetails;

		private readonly RealMoneyStoreItemBundle _itemToBuy;

		public Exception error
		{
			get;
			private set;
		}

		public string url
		{
			get;
			private set;
		}

		public string newPrice
		{
			get;
			private set;
		}

		public PurchaseItemRequest(RealMoneyStoreItemBundle itemToBuy)
		{
			_itemToBuy = itemToBuy;
		}

		public IEnumerator Execute()
		{
			Dictionary<string, object> postData = new Dictionary<string, object>
			{
				["Steam"] = false,
				["SteamId"] = string.Empty,
				["Language"] = StringTableBase<StringTable>.Instance.GetString("strLanguageCode"),
				["ItemSku"] = _itemToBuy.ItemSku,
				["ItemPrice"] = _itemToBuy.priceForCheck,
				["ItemCurrency"] = _itemToBuy.currencyCode
			};
			if (ServEnv.Exists() && ServEnv.TryGetValue("Country", out string countryCode))
			{
				postData["Country"] = countryCode;
			}
			WWW request = RoboPayHelper.GetWebService("robopay/token", postData);
			yield return (object)new WWWEnumerator(request, -1f);
			try
			{
				ParseResponse(request);
			}
			catch (Exception ex)
			{
				Exception ex3 = error = ex;
				RemoteLogger.Error(ex3.Message, _errorDetails, ex3.StackTrace);
			}
		}

		private void ParseResponse(WWW request)
		{
			string text = _errorDetails = Encoding.UTF8.GetString(request.get_bytes());
			Console.Log(_errorDetails);
			Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
			HttpStatusCode httpStatusCode = (HttpStatusCode)((!dictionary.ContainsKey("statusCode")) ? ((int)RoboPayHelper.GetStatusCodeFromHeader(request.get_responseHeaders()["STATUS"])) : Convert.ToInt32(dictionary["statusCode"]));
			if (httpStatusCode != HttpStatusCode.OK && httpStatusCode != HttpStatusCode.Forbidden && httpStatusCode != HttpStatusCode.NotFound)
			{
				throw new Exception("GetStoreItems error: http status error.");
			}
			Dictionary<string, object> dictionary2 = dictionary["response"] as Dictionary<string, object>;
			if (dictionary2.ContainsKey("data"))
			{
				newPrice = dictionary2["data"].ToString();
			}
			else
			{
				url = dictionary2["url"].ToString();
			}
		}
	}
}
