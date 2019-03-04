using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class RemoveFromClanRequest : SocialRequest<ClanMember[]>, IRemoveFromClanRequest, IServiceRequest<string>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 41;

		public RemoveFromClanRequest()
			: base("strRemoveFromClanErrorTitle", "strRemoveFromClanErrorBody", 0)
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

		protected override ClanMember[] ProcessResponse(OperationResponse response)
		{
			if (CacheDTO.MyClanMembers.ContainsKey(_userName))
			{
				CacheDTO.MyClanMembers.Remove(_userName);
			}
			return CacheDTO.MyClanMembers.Values.ToArray();
		}

		public void Inject(string userName)
		{
			_userName = userName;
		}
	}
}
