using Authentication;
using Login;
using RoboCraft.MiniJSON;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Services.Users
{
	internal class UserService
	{
		public const string ERROR_MSG = "User Service Error";

		private const string HTTP_STATUS_HEADER = "STATUS";

		private const string URL = "account/email";

		public IEnumerator UpdateEmailAddress(string emailAddress)
		{
			StringHolder stringHolder = new StringHolder("None");
			yield return WaitForWSReq(emailAddress, stringHolder);
			UserServiceErrorType userServiceErrorType = (UserServiceErrorType)Enum.Parse(typeof(UserServiceErrorType), stringHolder.str);
			yield return userServiceErrorType;
		}

		private static IEnumerator<WWWEnumerator> WaitForWSReq(string emailAddress, StringHolder stringHolder)
		{
			Dictionary<string, object> requestData = new Dictionary<string, object>
			{
				["PublicId"] = User.PublicId,
				["EmailAddress"] = emailAddress,
				["Password"] = "bogus"
			};
			if (ClientConfigData.TryGetValue("authUrl", out string baseUrl))
			{
				string fullUrl = baseUrl + "account/email";
				string requestDataSerialized = Json.Serialize((object)requestData);
				byte[] requestDataBytes = Encoding.ASCII.GetBytes(requestDataSerialized);
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{
						"Content-Type",
						"application/json"
					},
					{
						"Authorization",
						$"Robocraft {User.AuthToken}"
					}
				};
				WWW request = new WWW(fullUrl, requestDataBytes, headers);
				yield return new WWWEnumerator(request, -1f);
				Dictionary<string, string> responseHeaders = request.get_responseHeaders();
				if (responseHeaders.ContainsKey("STATUS"))
				{
					HttpStatusCode statusCodeFromHeader = GetStatusCodeFromHeader(responseHeaders["STATUS"]);
					Dictionary<string, object> dictionary = null;
					if (request.get_bytes().Length > 0)
					{
						dictionary = (Dictionary<string, object>)Json.Deserialize(Encoding.UTF8.GetString(request.get_bytes()));
						if (dictionary == null)
						{
							Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
							dictionary2.Add("statusCode", statusCodeFromHeader.ToString());
							Dictionary<string, string> data = dictionary2;
							HandleUserServiceError("User service null response" + statusCodeFromHeader, stringHolder, data);
						}
					}
					if (request.get_error() != null || statusCodeFromHeader != HttpStatusCode.OK)
					{
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						dictionary2.Add("statusCode", statusCodeFromHeader.ToString());
						Dictionary<string, string> dictionary3 = dictionary2;
						if (dictionary == null || !dictionary.ContainsKey("ErrorCode"))
						{
							HandleUserServiceError("RoboAuth user request failed - no data", stringHolder, dictionary3);
							yield break;
						}
						int num = Convert.ToInt32(dictionary["ErrorCode"]);
						dictionary3.Add("errorCode", num.ToString());
						string details;
						UserServiceErrorType userServiceError;
						switch (num)
						{
						case 140:
							details = "RoboAuth user request failed - invalid email";
							userServiceError = UserServiceErrorType.EmailAddressNotValid;
							break;
						case 201:
							details = "RoboAuth user request failed - email in use";
							userServiceError = UserServiceErrorType.DuplicateEmailAddress;
							break;
						default:
							details = "RoboAuth user request failed - unknown error code";
							userServiceError = UserServiceErrorType.GenericError;
							break;
						}
						stringHolder.str = userServiceError.ToString();
						HandleUserServiceError(details, stringHolder, dictionary3, userServiceError);
					}
					else
					{
						User.EmailAddress = (string)requestData["EmailAddress"];
					}
				}
				else
				{
					HandleUserServiceError("Email update failure", stringHolder, responseHeaders);
				}
			}
			else
			{
				HandleUserServiceError("No RoboAuth URL", stringHolder);
			}
		}

		private static void HandleUserServiceError(string details, StringHolder stringHolder, Dictionary<string, string> data = null, UserServiceErrorType userServiceError = UserServiceErrorType.GenericError)
		{
			RemoteLogger.Error("User Service Error", details, string.Empty, data);
			stringHolder.str = userServiceError.ToString();
		}

		private static HttpStatusCode GetStatusCodeFromHeader(string status)
		{
			Match match = Regex.Match(status, " (\\d+) ");
			if (!match.Success)
			{
				return HttpStatusCode.Unused;
			}
			return (HttpStatusCode)Convert.ToInt32(match.Groups[0].Value);
		}
	}
}
