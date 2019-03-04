using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class IgnoreUserRequest : ChatRequest<IgnoreUserResponse>, IIgnoreUserRequest, IServiceRequest<string>, IAnswerOnComplete<IgnoreUserResponse>, IServiceRequest
	{
		private string _userName;

		protected override byte OperationCode => 9;

		public IgnoreUserRequest()
			: base("strRobocloudError", "strErrorIgnoringUser", 0)
		{
		}

		public void Inject(string userName)
		{
			_userName = userName;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(7, _userName);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}

		protected override IgnoreUserResponse ProcessResponse(OperationResponse response)
		{
			string item = (string)response.Parameters[22];
			CacheDTO.chatIgnores.Add(item);
			IgnoreUserResponse result = default(IgnoreUserResponse);
			result.Successful = true;
			result.Message = null;
			result.UpdatedIgnoreList = new HashSet<string>(CacheDTO.chatIgnores, StringComparer.InvariantCultureIgnoreCase);
			return result;
		}
	}
}
