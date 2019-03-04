using System.Collections.Generic;
using System.Linq;

namespace Authentication
{
	public static class User
	{
		public static string SessionId
		{
			get;
			set;
		}

		public static string AuthToken
		{
			get;
			set;
		}

		public static string RefreshToken
		{
			get;
			set;
		}

		public static List<string> Flags
		{
			get;
			set;
		}

		public static string EmailAddress
		{
			get;
			set;
		}

		public static bool EmailValidated
		{
			get;
			set;
		}

		public static string UserBuildNo
		{
			get;
			set;
		}

		private static string LegacyName
		{
			get;
			set;
		}

		public static string DisplayName
		{
			get;
			set;
		}

		public static string PublicId
		{
			get;
			set;
		}

		public static string Username => LegacyName;

		public static string TGPID
		{
			get;
			set;
		}

		public static void InitializeTencentTGPIdReceived(string id)
		{
			TGPID = id;
		}

		public static void InitializeTencent(string tencentAuthToken_, string legacyUsername_, string displayname_)
		{
			AuthToken = tencentAuthToken_;
			LegacyName = legacyUsername_;
			DisplayName = displayname_;
		}

		public static void Initialize(string publicId, string legacyName, string displayName, string authToken, string refreshToken, string emailAddress, bool emailValidated, List<string> flags)
		{
			LegacyName = legacyName;
			PublicId = publicId;
			DisplayName = displayName;
			AuthToken = authToken;
			RefreshToken = refreshToken;
			EmailAddress = emailAddress;
			EmailValidated = emailValidated;
			Flags = (from f in flags
			select (f)).ToList();
		}

		public static void Reset()
		{
			PublicId = "client not initialized";
			DisplayName = "client not initialized";
			LegacyName = "client not initialized";
			AuthToken = string.Empty;
			RefreshToken = string.Empty;
			EmailAddress = string.Empty;
			EmailValidated = false;
			Flags = new List<string>();
		}

		public static void SetUserBuildNo(string userBuild)
		{
			UserBuildNo = userBuild;
		}
	}
}
