using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;

namespace ChatServiceLayer.Photon
{
	internal class GetAllPublicChannelNamesRequest : ChatRequest<string[]>, IGetAllPublicChannelNamesRequest, IServiceRequest, IAnswerOnComplete<string[]>
	{
		protected override byte OperationCode => 13;

		public GetAllPublicChannelNamesRequest()
			: base("strGetPublicChannelNamesRequestFailedTitle", "strGetPublicChannelNamesRequestFailedBody", 0)
		{
		}

		public override void Execute()
		{
			if (CacheDTO.PublicChatChannelNames != null)
			{
				base.answer.succeed(CacheDTO.PublicChatChannelNames);
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
			return new OperationRequest();
		}

		protected override string[] ProcessResponse(OperationResponse response)
		{
			CacheDTO.PublicChatChannelNames = (string[])response.Parameters[20];
			return CacheDTO.PublicChatChannelNames;
		}
	}
}
