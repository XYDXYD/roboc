using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetDamageBoostRequest : WebServicesCachedRequest<DamageBoostDeserialisedData>, IGetDamageBoostRequest, IServiceRequest, IAnswerOnComplete<DamageBoostDeserialisedData>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 163;

		public bool isDone
		{
			get;
			private set;
		}

		public GetDamageBoostRequest()
			: base("strRobocloudError", "strUnableLoadDamageBoostData", 3)
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

		protected override DamageBoostDeserialisedData ProcessResponse(OperationResponse response)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Parameters[192];
			Dictionary<string, object> damageBoostValues = (Dictionary<string, object>)dictionary["damageBoost"];
			DamageBoostDeserialisedData result = new DamageBoostDeserialisedData(damageBoostValues);
			isDone = true;
			return result;
		}

		void IGetDamageBoostRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
