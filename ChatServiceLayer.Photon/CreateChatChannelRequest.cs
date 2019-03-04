using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class CreateChatChannelRequest : ChatRequest, ICreateChatChannelRequest, IServiceRequest<CreateOrJoinChatChannelDependency>, IAnswerOnComplete, IServiceRequest
	{
		private CreateOrJoinChatChannelDependency _dependency;

		public override bool isEncrypted => true;

		protected override byte OperationCode => 4;

		public CreateChatChannelRequest()
			: base("strCreateChatChannelErrorTitle", "strCreateChatChannelErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[3] = _dependency.ChannelName;
			dictionary[16] = _dependency.Password;
			OperationRequest val = new OperationRequest();
			val.Parameters = dictionary;
			return val;
		}

		public void Inject(CreateOrJoinChatChannelDependency dependency)
		{
			_dependency = dependency;
		}
	}
}
