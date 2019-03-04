using RoboCraft.MiniJSON;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;

namespace Mothership
{
	internal class GetStoreItemsRequest
	{
		private string _errorDetails;

		public Exception error
		{
			get;
			private set;
		}

		public List<RealMoneyStoreItemBundle> result
		{
			get;
			private set;
		}

		public IEnumerator Execute()
		{
			Dictionary<string, object> postData = new Dictionary<string, object>();
			if (ServEnv.Exists() && ServEnv.TryGetValue("Country", out string countryCode))
			{
				postData["Country"] = countryCode;
			}
			WWW request = RoboPayHelper.GetWebService("robopay/store", postData);
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
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(_errorDetails = Encoding.UTF8.GetString(request.get_bytes()));
			HttpStatusCode httpStatusCode = (HttpStatusCode)((!dictionary.ContainsKey("statusCode")) ? ((int)RoboPayHelper.GetStatusCodeFromHeader(request.get_responseHeaders()["STATUS"])) : Convert.ToInt32(dictionary["statusCode"]));
			if (httpStatusCode != HttpStatusCode.OK && httpStatusCode != HttpStatusCode.Forbidden && httpStatusCode != HttpStatusCode.NotFound)
			{
				throw new Exception("GetStoreItems error: http status error.");
			}
			Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["response"];
			List<object> list = (List<object>)dictionary2["data"];
			result = new List<RealMoneyStoreItemBundle>(list.Count);
			foreach (object item3 in list)
			{
				Dictionary<string, object> dictionary3 = (Dictionary<string, object>)item3;
				List<object> list2 = (List<object>)dictionary3["items"];
				List<RealMoneyStoreItem> list3 = new List<RealMoneyStoreItem>(list2.Count);
				foreach (object item4 in list2)
				{
					Dictionary<string, object> dictionary4 = (Dictionary<string, object>)item4;
					string value = dictionary4["itemType"].ToString();
					RealMoneyStoreItem realMoneyStoreItem = default(RealMoneyStoreItem);
					realMoneyStoreItem.StoreItemType = (RealMoneyStoreItemType)Enum.Parse(typeof(RealMoneyStoreItemType), value);
					realMoneyStoreItem.CountOfItems = Convert.ToInt32(dictionary4["amount"]);
					realMoneyStoreItem.Data = (dictionary4["data"] as string);
					RealMoneyStoreItem item = realMoneyStoreItem;
					list3.Add(item);
				}
				RealMoneyStoreItemBundle realMoneyStoreItemBundle = new RealMoneyStoreItemBundle();
				realMoneyStoreItemBundle.ItemSku = (string)dictionary3["itemSku"];
				realMoneyStoreItemBundle.currencyCode = (string)dictionary3["currencyCode"];
				realMoneyStoreItemBundle.currencyString = (string)dictionary3["currencyString"];
				realMoneyStoreItemBundle.oldCurrencyString = (string)dictionary3["oldCurrencyString"];
				realMoneyStoreItemBundle.priceForCheck = Convert.ToSingle(dictionary3["priceForCheck"]);
				realMoneyStoreItemBundle.oldPriceForCheck = Convert.ToSingle(dictionary3["oldPriceForCheck"]);
				realMoneyStoreItemBundle.additionalValue = Convert.ToInt32(dictionary3["additionalValue"]);
				realMoneyStoreItemBundle.mostPopularFlag = Convert.ToBoolean(dictionary3["mostPopular"]);
				realMoneyStoreItemBundle.bestValueFlag = Convert.ToBoolean(dictionary3["bestValue"]);
				realMoneyStoreItemBundle.Items = list3;
				RealMoneyStoreItemBundle item2 = realMoneyStoreItemBundle;
				result.Add(item2);
			}
		}
	}
}
