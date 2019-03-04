using ExitGames.Client.Photon;

namespace Services.Web.Photon
{
	internal sealed class GetAutoRegenHealthSettingsCustomGame : GetAutoRegenHealthSettings
	{
		protected override OperationRequest BuildOpRequest()
		{
			OperationRequest val = base.BuildOpRequest();
			val.Parameters[191] = true;
			return val;
		}
	}
}
