using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadBattleArenaSettingsCustomGameRequest : LoadBattleArenaSettingsRequest
	{
		protected override OperationRequest BuildOpRequest()
		{
			OperationRequest val = base.BuildOpRequest();
			val.Parameters = new Dictionary<byte, object>
			{
				{
					191,
					true
				}
			};
			return val;
		}
	}
}
