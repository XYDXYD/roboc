using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetBrawlWeaponStatsRequest : WebServicesCachedRequest<IDictionary<int, WeaponStatsData>>, ILoadWeaponStatsRequest, IServiceRequest, IAnswerOnComplete<IDictionary<int, WeaponStatsData>>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 137;

		public bool isDone
		{
			get;
			private set;
		}

		public GetBrawlWeaponStatsRequest()
			: base("strRobocloudError", "strLoadBrawlWeaponStatsError", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
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

		protected override IDictionary<int, WeaponStatsData> ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> data = (Dictionary<string, Hashtable>)response.Parameters[158];
			Dictionary<int, WeaponStatsData> result = LoadWeaponStatsRequest.ParseResponse(data);
			isDone = true;
			return result;
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			throw new Exception("SetParameterOverride for GetBrawlWeaponStatsRequest not impleted");
		}

		void ILoadWeaponStatsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
