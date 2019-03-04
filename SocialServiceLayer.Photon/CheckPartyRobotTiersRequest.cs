using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class CheckPartyRobotTiersRequest : SocialRequest<bool>, ICheckPartyRobotTiersRequest, ITask, IServiceRequest, IAnswerOnComplete<bool>, IAbstractTask
	{
		protected override byte OperationCode => 20;

		public bool isDone
		{
			get;
			private set;
		}

		public CheckPartyRobotTiersRequest()
			: base("strRobocloudError", "strCheckRobotTiersError", 0)
		{
		}

		public override void Execute()
		{
			base.Execute();
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

		protected override bool ProcessResponse(OperationResponse response)
		{
			isDone = true;
			return (bool)response.Parameters[20];
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}
	}
}
