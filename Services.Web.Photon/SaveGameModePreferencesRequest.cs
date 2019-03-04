using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class SaveGameModePreferencesRequest : WebServicesRequest, ISaveGameModePreferencesRequest, IServiceRequest<GameModePreferences>, IAnswerOnComplete, IServiceRequest
	{
		private GameModePreferences _dependency;

		protected override byte OperationCode => 93;

		public SaveGameModePreferencesRequest()
			: base("strRobocloudError", "strUnableToSaveGameModePreferencesError", 1)
		{
		}

		public void Inject(GameModePreferences dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					215,
					_dependency.Serialize()
				}
			};
			return val;
		}
	}
}
