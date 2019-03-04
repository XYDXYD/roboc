using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Utility;
using WebServices;

internal sealed class GenericErrorCatch
{
	[CompilerGenerated]
	private static Action<string, string, LogType, Thread> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action _003C_003Ef__mg_0024cache1;

	public static bool IsInitialised
	{
		get;
		set;
	}

	public static void Init()
	{
		Console.onException = delegate(Exception e, string toPrint, string stackTrace)
		{
			if (!IsInitialised)
			{
				Debug.LogException(e);
			}
			else
			{
				OnGenericError(toPrint, stackTrace, 4, null);
			}
		};
		UnityLoggerCallback.AddLogger(OnGenericError);
		IsInitialised = true;
	}

	private static void OnGenericError(string logString, string stackTrace, LogType type, Thread thread)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type == 4 || (int)type == 1)
		{
			if (MainThreadCaller.get_isCurrentThreadMain())
			{
				ShowWindowOnError(logString, stackTrace, type);
			}
			else
			{
				MainThreadCaller.get_Instance().PushAction((Action)delegate
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					ShowWindowOnError(logString, stackTrace, type);
				});
			}
		}
	}

	private static void ShowWindowOnError(string logString, string stackTrace, LogType type)
	{
		if (logString.Contains(QuitDueToMaintenanceException.CODE_ERROR) || logString.Contains(UnmanagedPhotonErrorException.CODE_ERROR) || logString.Contains(DuplicateLoginException.CODE_ERROR) || logString.Contains(PhotonConnectionException.CODE_ERROR))
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(Localization.Get("strError", true), logString, Localization.Get("strQuit", true), (Action)Application.Quit), stackTrace);
		}
		RemoteLogger.Error(logString, string.Empty, stackTrace);
	}
}
