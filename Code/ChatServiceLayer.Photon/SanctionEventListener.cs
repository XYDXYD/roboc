using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Globalization;

namespace ChatServiceLayer.Photon
{
	internal class SanctionEventListener : ChatEventListener<Sanction>, ISanctionEventListener, IServiceEventListener<Sanction>, IServiceEventListenerBase
	{
		public override ChatEventCode ChatEventCode => ChatEventCode.Sanction;

		protected override void ParseData(EventData eventData)
		{
			string reason = (string)eventData.Parameters[2];
			SanctionType type = (SanctionType)eventData.Parameters[9];
			DateTimeOffset issued = DateTimeOffset.Parse((string)eventData.Parameters[8], CultureInfo.InvariantCulture);
			int? duration = null;
			if (eventData.Parameters.ContainsKey(11))
			{
				duration = (int)eventData.Parameters[11];
			}
			Sanction data = new Sanction(type, reason, issued, duration);
			Invoke(data);
		}
	}
}
