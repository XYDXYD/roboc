using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal sealed class LoadFirstBrawlVictoryPendingRequest : WebServicesCachedRequest<bool>, ILoadFirstBrawlVictoryPendingRequest, IServiceRequest<int>, IAnswerOnComplete<bool>, ITask, IServiceRequest, IAbstractTask
	{
		private int _desiredBrawlNumber;

		protected override byte OperationCode => 141;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadFirstBrawlVictoryPendingRequest()
			: base("strRobocloudError", "strErrorBrawlRequestError", 0)
		{
		}

		public void Inject(int dependancy)
		{
			_desiredBrawlNumber = dependancy;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(164, _desiredBrawlNumber);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			bool flag = false;
			try
			{
				flag = (bool)response.get_Item((byte)163);
			}
			catch (Exception ex)
			{
				Console.Log("Error in LoadFirstBrawlVictoryPendingRequestResult - unexpected server error: " + ex.Message);
				isDone = true;
				return false;
			}
			isDone = true;
			return flag;
		}

		void ILoadFirstBrawlVictoryPendingRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
