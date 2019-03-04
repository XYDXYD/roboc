using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class AdjustCustomGameConfigRequest : WebServicesRequest<AdjustCustomGameConfigurationResponse>, IAdjustCustomGameConfigRequest, IServiceRequest<AdjustCustomGameConfigRequestDependancy>, IAnswerOnComplete<AdjustCustomGameConfigurationResponse>, IServiceRequest
	{
		private AdjustCustomGameConfigRequestDependancy _dependancy;

		protected override byte OperationCode => 149;

		public AdjustCustomGameConfigRequest()
			: base("strRobocloudError", "strCustomGameRequestErrorAdjustConfig", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(179, _dependancy.Field);
			dictionary.Add(180, _dependancy.NewValue);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		protected override AdjustCustomGameConfigurationResponse ProcessResponse(OperationResponse response)
		{
			return (AdjustCustomGameConfigurationResponse)response.Parameters[168];
		}

		public void Inject(AdjustCustomGameConfigRequestDependancy dependency)
		{
			_dependancy = dependency;
		}
	}
}
