using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetBuildingXPSettingsRequest : WebServicesCachedRequest<BuildingXPSettingsDependency>, IGetBuildingXPSettingsRequest, IServiceRequest, IAnswerOnComplete<BuildingXPSettingsDependency>
	{
		protected override byte OperationCode => 199;

		public GetBuildingXPSettingsRequest()
			: base("strRobocloudError", "strLoadBuildingXPSettingsError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override BuildingXPSettingsDependency ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[79];
			Hashtable val = dictionary["BuildXPSettings"];
			float buildModePeriodUserEarnXP = Convert.ToSingle(val.get_Item((object)"buildModePeriodUserEarnXP"));
			float buildModePeriodUserInactivity = Convert.ToSingle(val.get_Item((object)"buildModePeriodUserInactivity"));
			return new BuildingXPSettingsDependency(buildModePeriodUserEarnXP, buildModePeriodUserInactivity);
		}
	}
}
