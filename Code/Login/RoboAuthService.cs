using RoboCraft.MiniJSON;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Utility;

namespace Login
{
	public static class RoboAuthService
	{
		public class DisplayNameNotValidException : Exception
		{
			public enum NotValidReason
			{
				Invalid,
				AlreadyUsed
			}

			private readonly NotValidReason _notValidReason;

			public NotValidReason DisplayNameNotValidReason => _notValidReason;

			public DisplayNameNotValidException(NotValidReason reason_)
			{
				_notValidReason = reason_;
			}
		}

		public const string ERROR_ACCOUNT_UNAUTHORISED = "strErrorUnconfirmed";

		public const string ERROR_PASSWORD_INVALIDATED = "strPasswordInvalidated";

		private const string HTTP_STATUS_HEADER = "STATUS";

		private const string ROBOAUTH_EMAIL_AUTHENTICATE = "authenticate/email/game";

		private const string ROBOAUTH_DISPLAY_NAME_AUTHENTICATE = "authenticate/displayname/game";

		private const string ROBOAUTH_DISPLAY_NAME_AVAILABLE = "account/displayname/available";

		private const string ROBOAUTH_CHANGE_DISPLAY_NAME = "/account/displayname/flag";

		private const string ROBOAUTH_LEGACY_NAME_AUTHENTICATE = "authenticate/robocraft/game";

		private const string ERROR_LOGIN = "Login Flow Error";

		private const string KEY_ERROR_CODE = "ErrorCode";

		private const string KEY_UNCONFIRMED_USER_TOKEN = "Data";

		public static IEnumerator AuthWithIdEnumerator(string identifier, string password, Dictionary<string, object> dataToReturn)
		{
			Console.Log("RoboAuth: Authenticate with identifier: " + identifier);
			string authService = "authenticate/robocraft/game";
			if (identifier.Contains("@"))
			{
				authService = "authenticate/email/game";
			}
			else if (identifier.Contains("#"))
			{
				authService = "authenticate/displayname/game";
			}
			if (ClientConfigData.TryGetValue("authUrl", out string authUrl))
			{
				Dictionary<string, object> requestData = new Dictionary<string, object>
				{
					{
						"DisplayName",
						identifier
					},
					{
						"EmailAddress",
						identifier
					},
					{
						"Password",
						password
					},
					{
						"MacAddress",
						GetMacAddress()
					},
					{
						"Target",
						"Robocraft"
					}
				};
				byte[] postData = Encoding.ASCII.GetBytes(Json.Serialize((object)requestData));
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{
						"Content-Type",
						"application/json"
					}
				};
				string url = authUrl + authService;
				WWW www = new WWW(url, postData, headers);
				WWWEnumerator request = new WWWEnumerator(www, -1f);
				yield return request;
				WWW current = request.get_Current();
				Dictionary<string, string> responseHeaders = current.get_responseHeaders();
				if (responseHeaders.ContainsKey("STATUS"))
				{
					HttpStatusCode statusCodeFromHeader = GetStatusCodeFromHeader(responseHeaders["STATUS"]);
					Dictionary<string, object> dictionary = null;
					byte[] bytes = current.get_bytes();
					if (bytes.Length > 0)
					{
						dictionary = (Dictionary<string, object>)Json.Deserialize(Encoding.UTF8.GetString(bytes));
						if (dictionary == null)
						{
							Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
							dictionary2.Add("statusCode", statusCodeFromHeader.ToString());
							Dictionary<string, string> errorData = dictionary2;
							LoginError("Roboauth null response", dataToReturn, errorData);
						}
					}
					if (current.get_error() != null || statusCodeFromHeader != HttpStatusCode.OK)
					{
						Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
						dictionary2.Add("statusCode", statusCodeFromHeader.ToString());
						Dictionary<string, string> dictionary3 = dictionary2;
						if (dictionary == null || !dictionary.ContainsKey("ErrorCode"))
						{
							LoginError("RoboAuth authentication failed - no data", dataToReturn, dictionary3);
							yield break;
						}
						int num = Convert.ToInt32(dictionary["ErrorCode"]);
						dictionary3.Add("ErrorCode", num.ToString());
						switch (num)
						{
						case 302:
						{
							object value = dictionary["Data"];
							dataToReturn.Add("Data", value);
							LoginError("RoboAuth authentication failed - account unconfirmed", dataToReturn, dictionary3);
							break;
						}
						case 301:
							LoginError("RoboAuth authentication failed - account blocked", dataToReturn, dictionary3);
							break;
						case 203:
							LoginError("RoboAuth authentication failed - password invalidated", dataToReturn, dictionary3);
							break;
						case 204:
							LoginError("RoboAuth authentication failed - bad username or password", dataToReturn, dictionary3);
							break;
						case 202:
							LoginError("RoboAuth authentication failed - account blocked", dataToReturn, dictionary3);
							break;
						default:
							LoginError("RoboAuth authentication failed - unknown error code", dataToReturn, dictionary3);
							break;
						}
					}
					else
					{
						foreach (KeyValuePair<string, object> item in dictionary)
						{
							dataToReturn.Add(item.Key, item.Value);
						}
					}
				}
				else
				{
					LoginError("RoboAuth authentication failed missing status header", dataToReturn);
				}
			}
			else
			{
				LoginError("No RoboAuth URL", dataToReturn);
			}
		}

		private static void LoginError(string errorDetails, IDictionary<string, object> dataToReturn, Dictionary<string, string> errorData = null)
		{
			RemoteLogger.Error("Login Flow Error", errorDetails, null, errorData);
			dataToReturn.Add("ErrorMsg", errorDetails);
			if (errorData != null)
			{
				dataToReturn.Add("ErrorCode", errorData["ErrorCode"]);
			}
		}

		public static IEnumerator WaitForWebService(string url, Dictionary<string, object> requestData, Action<WWW> onComplete, string authToken = null)
		{
			if (ClientConfigData.TryGetValue("authUrl", out string authUrl))
			{
				WWWEnumerator request;
				if (requestData != null)
				{
					byte[] bytes = Encoding.ASCII.GetBytes(Json.Serialize((object)requestData));
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					dictionary.Add("Content-Type", "application/json");
					Dictionary<string, string> dictionary2 = dictionary;
					if (!string.IsNullOrEmpty(authToken))
					{
						dictionary2["Authorization"] = $"Robocraft {authToken}";
					}
					request = new WWWEnumerator(new WWW(authUrl + url, bytes, dictionary2), -1f);
				}
				else
				{
					request = new WWWEnumerator(new WWW(authUrl + url), -1f);
				}
				yield return request;
				onComplete(request.get_Current());
			}
			else
			{
				RemoteLogger.Error("Login Flow Error", "No RoboAuth URL", null);
			}
		}

		private static HttpStatusCode GetStatusCodeFromHeader(string status)
		{
			Match match = Regex.Match(status, " (\\d+) ");
			if (match.Success)
			{
				return (HttpStatusCode)Convert.ToInt32(match.Groups[0].Value);
			}
			return HttpStatusCode.Unused;
		}

		private static void ParseAuthResponse(Action<Dictionary<string, object>> onSuccess, Action<Exception> onError, WWW request, HttpStatusCode statusCode, Dictionary<string, object> data, Exception error)
		{
			if (request.get_error() != null || statusCode != HttpStatusCode.OK)
			{
				if (data == null || !data.ContainsKey("ErrorCode"))
				{
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - no data " + statusCode, null);
					onError(error);
					return;
				}
				int num = Convert.ToInt32(data["ErrorCode"]);
				switch (num)
				{
				case 302:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - account unconfirmed " + statusCode, null);
					onError(new Exception("strErrorUnconfirmed"));
					break;
				case 301:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - account blocked " + statusCode, null);
					onError(new Exception(StringTableBase<StringTable>.Instance.GetString("strAccountSuspendedInfo")));
					break;
				case 203:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - password invalidated " + statusCode, null);
					onError(new Exception("strPasswordInvalidated"));
					break;
				case 204:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - bad username or password " + statusCode, null);
					onError(new Exception(StringTableBase<StringTable>.Instance.GetString("strErrorIncorrectLogin")));
					break;
				case 202:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - account blocked " + statusCode, null);
					onError(new Exception(StringTableBase<StringTable>.Instance.GetString("strAccountSuspendedInfo")));
					break;
				case 210:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed -display name already in use", null);
					onError(new DisplayNameNotValidException(DisplayNameNotValidException.NotValidReason.AlreadyUsed));
					break;
				case 122:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - tried to submit an invalid displayname", null);
					onError(new DisplayNameNotValidException(DisplayNameNotValidException.NotValidReason.Invalid));
					break;
				case 303:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed with error code 303 that is only expected on steam/launcher", null);
					onError(error);
					break;
				default:
					RemoteLogger.Error("Login Flow Error", "RoboAuth authentication failed - unknown error code " + num, null);
					onError(error);
					break;
				}
			}
			else
			{
				onSuccess(data);
			}
		}

		private static string GetMacAddress()
		{
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					return networkInterface.GetPhysicalAddress().ToString();
				}
			}
			return string.Empty;
		}
	}
}
