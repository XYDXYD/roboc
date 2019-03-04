using RoboCraft.MiniJSON;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Utility;

internal static class ClientConfigData
{
	private static Dictionary<string, object> _cachedResults;

	private static string _defaultUrl = "http://file.robocraft.qq.com/json/data.json";

	private static readonly Dictionary<string, object> _defaults = new Dictionary<string, object>
	{
		{
			"PhotonSocialServer",
			"121.51.214.16:4532"
		},
		{
			"PhotonServicesServer",
			"121.51.214.21:4536"
		},
		{
			"PhotonChatServer",
			"121.51.214.125:4530"
		},
		{
			"PhotonSinglePlayerServer",
			"121.51.214.19:4534"
		},
		{
			"PhotonLobbyServer",
			"121.51.214.14:4540"
		},
		{
			"GameplayServerServiceAddress",
			"121.51.214.126:4538"
		},
		{
			"enterBattleLogGenerationTimeout",
			"60"
		},
		{
			"UnreliableMessages",
			"true"
		},
		{
			"RailGameId",
			"2000065"
		},
		{
			"TGPShopUrl",
			"http://172.16.3.93/store/"
		}
	};

	public static bool TryGetValue<T>(string key, out T value)
	{
		Dictionary<string, object> dictionary = _cachedResults;
		if (dictionary == null)
		{
			RemoteLogger.Error("ClientConfigData not preloaded - using defaults", null, Environment.StackTrace);
			dictionary = _defaults;
		}
		if (dictionary.TryGetValue(key, out object value2))
		{
			if (value2 is T)
			{
				value = (T)value2;
				return true;
			}
			throw new Exception("Retrieved type does not match requested type '" + key + "'");
		}
		if (_defaults.TryGetValue(key, out value2))
		{
			RemoteLogger.Error("Data acquired from S3 does not contain key, using default value '", key, null);
			value = (T)value2;
			return true;
		}
		Console.LogWarning("Couldn't find a value for key '" + key + "'");
		value = default(T);
		return false;
	}

	public static IEnumerator Load(Action onComplete = null, bool saveOnDisk = false)
	{
		string url = _defaultUrl;
		Exception error = null;
		if (_cachedResults == null)
		{
			Console.Log("No cached client configuration data was found.");
			try
			{
				if (ServEnv.Exists() && ServEnv.TryGetValue("S3URL", out string value))
				{
					url = value;
				}
			}
			catch (Exception ex)
			{
				error = ex;
			}
			if (error == null)
			{
				Console.Log("Downloading client configuration data.");
				UnityWebRequest www = UnityWebRequest.Get(url);
				string data;
				try
				{
					yield return (object)new UnityWebRequestEnumerator(www, -1);
					if (www.get_error() != null || www.get_responseCode() != 200)
					{
						string str = (www.get_error() == null) ? www.get_responseCode().ToString() : www.get_error();
						error = new Exception("Data download failed: Error downloading client config data " + str);
						RemoteLogger.Error(error);
						throw new Exception(StringTableBase<StringTable>.Instance.GetString("strUnableToLoadStaticData"));
					}
					data = www.get_downloadHandler().get_text();
				}
				finally
				{
					base._003C_003E__Finally0();
				}
				try
				{
					if (error == null)
					{
						_cachedResults = (Json.Deserialize(data) as Dictionary<string, object>);
					}
					else
					{
						Console.LogWarning("Setting client configuration data to the default values.");
						_cachedResults = _defaults;
					}
				}
				catch (Exception e)
				{
					Console.LogWarning("Setting client configuration data to the default values.");
					_cachedResults = _defaults;
					RemoteLogger.Error(e);
				}
				onComplete?.Invoke();
			}
		}
		else
		{
			Console.Log("Cached client configuration data found.");
			onComplete?.Invoke();
		}
	}
}
