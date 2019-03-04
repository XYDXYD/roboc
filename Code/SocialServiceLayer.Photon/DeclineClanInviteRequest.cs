using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class DeclineClanInviteRequest : SocialRequest, IDeclineClanInviteRequest, IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
		private string _clanName;

		protected override byte OperationCode => 37;

		public DeclineClanInviteRequest()
			: base("strDeclineClanInviteErrorTitle", "strDeclineClanInviteErrorBody", 0)
		{
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

		public void Inject(string clanName)
		{
			_clanName = clanName;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			if (CacheDTO.ClanInvites.ContainsKey(_clanName))
			{
				CacheDTO.ClanInvites.Remove(_clanName);
			}
			base.ProcessResponse(response);
		}
	}
}
