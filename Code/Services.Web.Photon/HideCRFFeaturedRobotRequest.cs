using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class HideCRFFeaturedRobotRequest : WebServicesRequest, IHideCRFFeaturedRobotRequest, IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
		private int _itemIndex;

		protected override byte OperationCode => 100;

		public HideCRFFeaturedRobotRequest()
			: base("strGenericError", "strGenericErrorQuit", 0)
		{
		}

		public void Inject(int itemIndex)
		{
			_itemIndex = itemIndex;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[94] = _itemIndex;
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
