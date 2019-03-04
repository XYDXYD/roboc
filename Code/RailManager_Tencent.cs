using Authentication;
using rail;
using System;
using UnityEngine;
using Utility;

internal static class RailManager_Tencent
{
	private static RailCallBackHelper callback_helper_ = new RailCallBackHelper();

	private static IRailPlayer railPlayerInstance;

	private static TGPEventManager tgpManagerInstance;

	private static bool is_rail_initialed_ = false;

	public static string RailID => tgpManagerInstance.railId;

	public static string RailSessionID
	{
		get
		{
			if (!tgpManagerInstance.GetSession(out string sessionIDresult))
			{
				throw new Exception("TGP Validate user failed: the user session expired or is invalid");
			}
			return sessionIDresult;
		}
	}

	public static bool SessionReady()
	{
		if (tgpManagerInstance == null)
		{
			return false;
		}
		string sessionIDresult;
		return tgpManagerInstance.GetSession(out sessionIDresult);
	}

	public unsafe static bool RailReady()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		RailGameID val = new RailGameID();
		ClientConfigData.TryGetValue("RailGameId", out string value);
		val.id_ = Convert.ToUInt64(value);
		if (rail_api.RailNeedRestartAppForCheckingEnvironment(val, 1, new string[1]
		{
			string.Empty
		}))
		{
			Console.LogError("CheckingEnvironment failed, please run in TGP");
			return false;
		}
		is_rail_initialed_ = rail_api.RailInitialize();
		if (!is_rail_initialed_)
		{
			Console.LogError("RailInitialize failed");
			return false;
		}
		IRailFactory val2 = rail_api.RailFactory();
		railPlayerInstance = val2.RailPlayer();
		RailID railID = railPlayerInstance.GetRailID();
		tgpManagerInstance = new TGPEventManager(railID);
		GameObject val3 = new GameObject("TGP Behaviour (generated from code)");
		val3.AddComponent<TGPBehaviour>();
		Object.DontDestroyOnLoad(val3);
		callback_helper_.RegisterCallback(13001, new RailEventCallBackHandler((object)tgpManagerInstance, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		string text = "placeholder_string";
		railPlayerInstance.AsyncAcquireSessionTicket(text);
		User.InitializeTencentTGPIdReceived(railID.id_.ToString());
		return true;
	}
}
