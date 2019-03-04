using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ReclaimSeasonRewardRequest : SocialRequest<ReclaimSeasonRewardsResponse>, IReclaimSeasonRewardsRequest, IServiceRequest<string>, IAnswerOnComplete<ReclaimSeasonRewardsResponse>, ITask, IServiceRequest, IAbstractTask
	{
		private string _userName;

		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 51;

		public ReclaimSeasonRewardRequest()
			: base("strReclaimSeasonRewardsErrorTitle", "strReclaimSeasonRewardsErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, _userName);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(string dependency)
		{
			_userName = dependency;
		}

		protected override ReclaimSeasonRewardsResponse ProcessResponse(OperationResponse response)
		{
			isDone = true;
			if (!response.Parameters.ContainsKey(50))
			{
				throw new Exception("Missing response parameter: updated robits total");
			}
			if (!response.Parameters.ContainsKey(47))
			{
				throw new Exception("Missing response parameter: Robits awarded from the season");
			}
			if (!response.Parameters.ContainsKey(49))
			{
				throw new Exception("Missing response parameter: Number of season from which robits originated");
			}
			int seasonNumberOfReward_ = (int)response.Parameters[49];
			int robitsRewarded_ = (int)response.Parameters[47];
			long newRobitsTotal_ = (long)response.Parameters[50];
			return new ReclaimSeasonRewardsResponse(newRobitsTotal_, robitsRewarded_, seasonNumberOfReward_);
		}

		public override void Execute()
		{
			base.Execute();
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}
	}
}
