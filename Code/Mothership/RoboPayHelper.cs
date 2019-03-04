using Authentication;
using RoboCraft.MiniJSON;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal static class RoboPayHelper
	{
		public static WWW GetWebService(string url, Dictionary<string, object> postData)
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Expected O, but got Unknown
			if (!ClientConfigData.TryGetValue("paymentUrl", out string value))
			{
				throw new Exception("Unable to get payment URL");
			}
			byte[] bytes = Encoding.ASCII.GetBytes(Json.Serialize((object)postData));
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", "application/json");
			dictionary.Add("Authorization", "Robocraft " + User.AuthToken);
			Dictionary<string, string> dictionary2 = dictionary;
			Console.Log(value + url);
			return new WWW(value + url, bytes, dictionary2);
		}

		public static HttpStatusCode GetStatusCodeFromHeader(string status)
		{
			Match match = Regex.Match(status, " (\\d+) ");
			if (match.Success)
			{
				return (HttpStatusCode)Convert.ToInt32(match.Groups[0].Value);
			}
			return HttpStatusCode.Unused;
		}
	}
}
