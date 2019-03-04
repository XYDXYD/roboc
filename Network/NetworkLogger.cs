using LiteNetLib;
using System;
using Utility;

namespace Network
{
	internal class NetworkLogger : INetLogger
	{
		private const string Prefix = "NetworkLogger: ";

		public void WriteNet(Severity severity, ConsoleColor color, string text)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			StaticLog(severity, color, text);
		}

		public void WriteException(Exception e)
		{
			Console.LogException(e);
		}

		private static void StaticLog(Severity severity, ConsoleColor color, string text, bool colourise = true)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Invalid comparison between Unknown and I4
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Invalid comparison between Unknown and I4
			text = ((!colourise) ? ("NetworkLogger: " + text) : ("<color=blue>NetworkLogger: </color>" + text));
			if ((int)severity != 4)
			{
				if ((int)severity == 3)
				{
					Console.LogWarning(text);
				}
				else
				{
					Console.Log(text);
				}
			}
			else
			{
				Console.LogError(text);
			}
		}

		public static void Attach()
		{
			NetDebug.Logger = new NetworkLogger();
			Log("NetworkLogger.Attach");
		}

		public static void Detach()
		{
			Log("NetworkLogger.Detach");
			NetDebug.Logger = null;
		}

		public static void Log(string text)
		{
			StaticLog(2, ConsoleColor.White, text);
		}

		public static void LogWarn(string text)
		{
			StaticLog(3, ConsoleColor.White, text);
		}

		public static void LogError(string text)
		{
			StaticLog(4, ConsoleColor.White, text);
		}
	}
}
