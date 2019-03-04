using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class GetChatIgnoresRequest : ChatRequest<HashSet<string>>, IGetChatIgnoresRequest, IServiceRequest, IAnswerOnComplete<HashSet<string>>
	{
		protected override byte OperationCode => 8;

		public GetChatIgnoresRequest()
			: base("strRobocloudError", "strErrorLoadingIgnoreList", 0)
		{
		}

		public override void Execute()
		{
			if (CacheDTO.chatIgnores != null)
			{
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new HashSet<string>(CacheDTO.chatIgnores));
				}
			}
			else
			{
				base.Execute();
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override HashSet<string> ProcessResponse(OperationResponse response)
		{
			string[] collection = (string[])response.Parameters[15];
			CacheDTO.chatIgnores = new HashSet<string>(collection, StringComparer.InvariantCultureIgnoreCase);
			return new HashSet<string>(CacheDTO.chatIgnores, StringComparer.InvariantCultureIgnoreCase);
		}
	}
}
