using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadBattleArenaSettingsRequest : WebServicesCachedRequest<BattleArenaSettingsDependency>, ILoadBattleArenaSettingsRequest, ITask, IServiceRequest, IAnswerOnComplete<BattleArenaSettingsDependency>, IAbstractTask
	{
		protected override byte OperationCode => 53;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadBattleArenaSettingsRequest()
			: base("strRobocloudError", "strLoadBASettingsError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override BattleArenaSettingsDependency ProcessResponse(OperationResponse response)
		{
			BattleArenaSettingsDependency result = new BattleArenaSettingsDependency(((Dictionary<string, Hashtable>)response.Parameters[1])["BattleArenaSettings"]);
			isDone = true;
			return result;
		}

		void ILoadBattleArenaSettingsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
