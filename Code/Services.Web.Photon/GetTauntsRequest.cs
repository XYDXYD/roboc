using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class GetTauntsRequest : WebServicesCachedRequest<TauntsDeserialisedData>, IGetTauntsRequest, IServiceRequest, IAnswerOnComplete<TauntsDeserialisedData>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 164;

		public bool isDone
		{
			get;
			private set;
		}

		public GetTauntsRequest()
			: base("strRobocloudError", "strUnableLoadTauntsData", 3)
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

		protected override TauntsDeserialisedData ProcessResponse(OperationResponse response)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)response.Parameters[195];
			Dictionary<string, object> inputDictionary = (Dictionary<string, object>)dictionary["taunts"];
			TauntsDeserialisedData result = new TauntsDeserialisedData(inputDictionary);
			isDone = true;
			return result;
		}

		void IGetTauntsRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
