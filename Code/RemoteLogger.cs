using Authentication;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using RoboCraft.MiniJSON;
using System;
using System.Collections.Generic;
using System.Net;
using Utility;

internal static class RemoteLogger
{
	private static UdpAppender _appender;

	private static PatternLayout _layout;

	private static Dictionary<string, string> _details;

	private static bool _running;

	private static ILog _log;

	public static string username
	{
		get
		{
			return User.Username;
		}
		set
		{
		}
	}

	static RemoteLogger()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		_details = new Dictionary<string, string>();
		_running = false;
		try
		{
			string str = "Unity";
			str += "Windows";
			_log = LogManager.GetLogger(str);
			_appender = new UdpAppender();
			_layout = new PatternLayout("%timestamp %-5level %logger - %message%newline");
			_layout.ActivateOptions();
			_appender.set_Layout(_layout);
			int num = 0;
			string key = "ErrorLogAddress";
			if (!ClientConfigData.TryGetValue(key, out string value))
			{
			}
			if (value.Contains(":"))
			{
				string[] array = value.Split(':');
				value = array[0];
				num = Convert.ToInt32(array[1]);
				_appender.set_RemoteAddress(Dns.GetHostEntry(value).AddressList[0]);
				_appender.set_RemotePort(num);
				_appender.set_Threshold(Level.Warn);
				_appender.ActivateOptions();
				BasicConfigurator.Configure(_appender);
				_running = true;
			}
		}
		catch
		{
			Console.LogError("Failed to set up log4net error log.");
		}
	}

	public static void Error(string message, string details, string stacktrace, Dictionary<string, string> data = null)
	{
		if (data == null)
		{
			_details.Clear();
			data = _details;
		}
		data["user"] = username;
		string text = FromLog(details, data);
		RemoteLog(message, text, stacktrace);
		if (stacktrace != null)
		{
			Console.LogError(FastConcatUtility.FastConcat<string>(FastConcatUtility.FastConcat<string>(message, " "), text), stacktrace);
		}
		else
		{
			Console.LogError(FastConcatUtility.FastConcat<string>(FastConcatUtility.FastConcat<string>(message, " "), text));
		}
	}

	private static void RemoteLog(string message, string detailString, string trace)
	{
		string text = FastConcatUtility.FastConcat(message, "&|&", detailString, "&|&", trace);
		if (_running)
		{
			_log.Error((object)text);
		}
	}

	private static string FromLog(string details, Dictionary<string, string> data)
	{
		string empty = string.Empty;
		empty = "[";
		if (details != null)
		{
			data["details"] = details;
		}
		return FastConcatUtility.FastConcat(empty, Json.Serialize((object)data), "]");
	}

	public static void Error(Exception e)
	{
		Error(e.Message, string.Empty, e.StackTrace);
	}
}
