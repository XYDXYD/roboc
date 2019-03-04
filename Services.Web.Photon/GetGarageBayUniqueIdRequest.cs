using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web.Photon
{
	internal class GetGarageBayUniqueIdRequest : WebServicesCachedRequest<UniqueSlotIdentifier>, IGetGarageBayUniqueIdRequest, IServiceRequest, IAnswerOnComplete<UniqueSlotIdentifier>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 177;

		public bool isDone
		{
			get;
			private set;
		}

		public GetGarageBayUniqueIdRequest()
			: base("strRobocloudError", "strGetGarageBayUniqueId", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override UniqueSlotIdentifier ProcessResponse(OperationResponse response)
		{
			string fromString = (string)response.Parameters[54];
			UniqueSlotIdentifier result = new UniqueSlotIdentifier(fromString);
			isDone = true;
			return result;
		}

		void IGetGarageBayUniqueIdRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
