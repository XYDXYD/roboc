using Authentication;
using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ChangeClanMemberRankRequest : SocialRequest<ClanMember[]>, IChangeClanMemberRankRequest, IServiceRequest<ChangeClanMemberRankDependency>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
		private ChangeClanMemberRankDependency _dependency;

		protected override byte OperationCode => 38;

		public ChangeClanMemberRankRequest()
			: base("strChangeClanMemberRankErrorTitle", "strChangeClanMemberRankErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(1, _dependency.UserName);
			dictionary.Add(38, _dependency.ClanMemberRank);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		protected override ClanMember[] ProcessResponse(OperationResponse response)
		{
			if (CacheDTO.MyClanMembers.ContainsKey(_dependency.UserName))
			{
				CacheDTO.MyClanMembers[_dependency.UserName].ClanMemberRank = _dependency.ClanMemberRank;
			}
			if (_dependency.ClanMemberRank == ClanMemberRank.Leader)
			{
				CacheDTO.MyClanMembers[User.Username].ClanMemberRank = ClanMemberRank.Officer;
			}
			return CacheDTO.MyClanMembers.Values.ToArray();
		}

		public void Inject(ChangeClanMemberRankDependency dependency)
		{
			_dependency = dependency;
		}
	}
}
