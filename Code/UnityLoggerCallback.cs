using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public static class UnityLoggerCallback
{
	private static Action<string, string, LogType, Thread> _onNormaLog;

	private static object _locker = new object();

	[CompilerGenerated]
	private static LogCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static LogCallback _003C_003Ef__mg_0024cache1;

	public unsafe static void Init()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (_003C_003Ef__mg_0024cache0 == null)
		{
			_003C_003Ef__mg_0024cache0 = new LogCallback((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
		}
		Application.remove_logMessageReceivedThreaded(_003C_003Ef__mg_0024cache0);
		if (_003C_003Ef__mg_0024cache1 == null)
		{
			_003C_003Ef__mg_0024cache1 = new LogCallback((object)null, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
		}
		Application.add_logMessageReceivedThreaded(_003C_003Ef__mg_0024cache1);
	}

	public static void Reset()
	{
		lock (_locker)
		{
			_onNormaLog = null;
		}
	}

	public static void AddLogger(Action<string, string, LogType, Thread> logger)
	{
		lock (_locker)
		{
			_onNormaLog = (Action<string, string, LogType, Thread>)Delegate.Combine(_onNormaLog, logger);
		}
	}

	public static void RemoveLogger(Action<string, string, LogType, Thread> logger)
	{
		lock (_locker)
		{
			_onNormaLog = (Action<string, string, LogType, Thread>)Delegate.Remove(_onNormaLog, logger);
		}
	}

	private static void OnLogThreaded(string logString, string stackTrace, LogType type)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (_onNormaLog != null)
		{
			if (MainThreadCaller.get_isCurrentThreadMain())
			{
				_onNormaLog(logString, stackTrace, type, Thread.CurrentThread);
			}
			else
			{
				MainThreadCaller.get_Instance().PushAction((Action)delegate
				{
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					_onNormaLog(logString, stackTrace, type, Thread.CurrentThread);
				});
			}
		}
	}
}
