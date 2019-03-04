using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class PollClanExperienceRequest : SocialRequest<PollClanExperienceRequestResponse>, IPollClanExperienceRequest, IServiceRequest<string>, IAnswerOnComplete<PollClanExperienceRequestResponse>, ITask, IServiceRequest, IAbstractTask
	{
		private string _clanName;

		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 57;

		public PollClanExperienceRequest()
			: base("strPollClanExperienceRequestErrorTitle", "strPollClanExperienceRequestErrorBody", 0)
		{
		}

		protected override void OnSuccess()
		{
			base.OnSuccess();
			isDone = true;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(31, _clanName);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		protected override PollClanExperienceRequestResponse ProcessResponse(OperationResponse response)
		{
			if (!response.Parameters.ContainsKey(48))
			{
				throw new Exception("Missing response parameter: season xp's table");
			}
			Dictionary<string, int> dictionary = (Dictionary<string, int>)response.Parameters[48];
			Dictionary<string, ClanMember> dictionary2 = new Dictionary<string, ClanMember>(CacheDTO.MyClanMembers);
			foreach (KeyValuePair<string, ClanMember> item in dictionary2)
			{
				if (dictionary.ContainsKey(item.Value.Name))
				{
					item.Value.SeasonXP = dictionary[item.Value.Name];
				}
			}
			isDone = true;
			return new PollClanExperienceRequestResponse(dictionary);
		}

		public void Inject(string clanName)
		{
			_clanName = clanName;
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
