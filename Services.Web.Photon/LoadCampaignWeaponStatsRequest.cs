using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadCampaignWeaponStatsRequest : WebServicesCachedRequest<IDictionary<int, WeaponStatsData>>, ILoadWeaponStatsRequest, IServiceRequest, IAnswerOnComplete<IDictionary<int, WeaponStatsData>>, ITask, IAbstractTask
	{
		private ParameterOverride _parameterOverride;

		protected override byte OperationCode => 47;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadCampaignWeaponStatsRequest()
			: base("strRobocloudError", "strUnableLoadCampaignWeaponStats", 3)
		{
			_serviceBehaviour.SetAlternativeBehaviour(delegate
			{
			}, "strCancel");
		}

		public void SetParameterOverride(ParameterOverride parameterOverride)
		{
			_parameterOverride = parameterOverride;
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
					(byte)_parameterOverride.ParameterCode,
					_parameterOverride.ParameterValue
				}
			};
			return val;
		}

		protected override IDictionary<int, WeaponStatsData> ProcessResponse(OperationResponse response)
		{
			Dictionary<string, Hashtable> data = (Dictionary<string, Hashtable>)response.Parameters[57];
			Dictionary<int, WeaponStatsData> result = LoadWeaponStatsRequest.ParseResponse(data);
			isDone = true;
			return result;
		}

		void ILoadWeaponStatsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
