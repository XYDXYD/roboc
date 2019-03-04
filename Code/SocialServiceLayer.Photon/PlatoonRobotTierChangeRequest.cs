using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonRobotTierChangeRequest : SocialRequest, IPlatoonRobotTierChangeRequest, IServiceRequest<int>, IAnswerOnComplete, ITask, IServiceRequest, IAbstractTask
	{
		private int _newtier;

		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 47;

		public PlatoonRobotTierChangeRequest()
			: base("strPlatoonRobotTierChangeErrorTitle", "strPlatoonRobotTierChangeErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(20, _newtier);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			base.ProcessResponse(response);
			isDone = true;
		}

		public void Inject(int dependency)
		{
			_newtier = dependency;
		}
	}
}
