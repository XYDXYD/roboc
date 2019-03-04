using DeltaDNA.MiniJSON;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Services.Web.Photon
{
	internal class RobotSanctionData
	{
		private const string DATE_FORMAT = "dd/MM/yyyy H:mm:ss zzz";

		public readonly string RobotUniqueId;

		public readonly string RobotName;

		public readonly string Reason;

		public readonly string Reporter;

		public readonly string Receiver;

		public readonly DateTimeOffset Issued;

		public readonly DateTimeOffset Expires;

		public readonly bool Acknowledged;

		private RobotSanctionData(string robotUniqueId, string robotName, string reason, string reporter, string receiver, DateTimeOffset issued, DateTimeOffset expires, bool acknowledged)
		{
			RobotUniqueId = robotUniqueId;
			RobotName = robotName;
			Reason = (reason ?? string.Empty);
			Reporter = reporter;
			Receiver = receiver;
			Issued = issued;
			Expires = expires;
			Acknowledged = acknowledged;
		}

		internal static RobotSanctionData Deserialise(string source)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			try
			{
				dictionary = (Json.Deserialize(source) as Dictionary<string, object>);
				string robotUniqueId = (string)dictionary["RobotUniqueId"];
				string reason = (string)dictionary["Reason"];
				string robotName = (string)dictionary["RobotName"];
				string reporter = (string)dictionary["Reporter"];
				string receiver = (string)dictionary["Receiver"];
				DateTimeOffset issued = DateTimeOffset.ParseExact((string)dictionary["Issued"], "dd/MM/yyyy H:mm:ss zzz", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.AdjustToUniversal);
				DateTimeOffset expires = DateTimeOffset.ParseExact((string)dictionary["Expires"], "dd/MM/yyyy H:mm:ss zzz", CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.AdjustToUniversal);
				bool acknowledged = (bool)dictionary["Acknowledged"];
				return new RobotSanctionData(robotUniqueId, robotName, reason, reporter, receiver, issued, expires, acknowledged);
			}
			catch (Exception innerException)
			{
				throw new Exception("Exception caught while deserialising sanction ", innerException);
			}
		}

		public string Serialise()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("RobotUniqueId", RobotUniqueId);
			dictionary.Add("RobotName", RobotName);
			dictionary.Add("Reason", Reason);
			dictionary.Add("Reporter", Reporter);
			dictionary.Add("Receiver", Receiver);
			dictionary.Add("Issued", Issued.ToString("dd/MM/yyyy H:mm:ss zzz", CultureInfo.InvariantCulture));
			dictionary.Add("Expires", Expires.ToString("dd/MM/yyyy H:mm:ss zzz", CultureInfo.InvariantCulture));
			dictionary.Add("Acknowledged", Acknowledged);
			Dictionary<string, object> dictionary2 = dictionary;
			return Json.Serialize((object)dictionary2);
		}
	}
}
