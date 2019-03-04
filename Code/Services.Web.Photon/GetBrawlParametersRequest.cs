using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetBrawlParametersRequest : WebServicesCachedRequest<GetBrawlRequestResult>, IGetBrawlParametersRequest, IServiceRequest, IAnswerOnComplete<GetBrawlRequestResult>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 134;

		public bool isDone
		{
			get;
			private set;
		}

		public GetBrawlParametersRequest()
			: base("strRobocloudError", "strErrorBrawlRequestError", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override GetBrawlRequestResult ProcessResponse(OperationResponse response)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			Hashtable data = response.get_Item((byte)152);
			Hashtable data2 = response.get_Item((byte)153);
			BrawlClientParameters brawlClientParameters = BrawlClientParameters.Deserialise(data);
			GetBrawlRequestResult result;
			if (!brawlClientParameters.IsLocked)
			{
				BrawlGameParameters details_ = BrawlGameParameters.Deserialise(data2);
				result = new GetBrawlRequestResult(brawlClientParameters, details_);
			}
			else
			{
				result = new GetBrawlRequestResult(brawlClientParameters, null);
			}
			isDone = true;
			return result;
		}

		void IGetBrawlParametersRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
