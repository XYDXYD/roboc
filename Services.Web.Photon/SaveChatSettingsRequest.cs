using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class SaveChatSettingsRequest : WebServicesRequest, ISaveChatSettingsRequest, IServiceRequest<ChatSettingsData>, IAnswerOnComplete, IServiceRequest
	{
		private ChatSettingsData _dependency;

		protected override byte OperationCode => 19;

		public SaveChatSettingsRequest()
			: base("strRobocloudError", "strUnableSaveChatSettings", 0)
		{
		}

		public void Inject(ChatSettingsData dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			dictionary.Add("chatEnabled", _dependency.chatEnabled);
			Dictionary<string, bool> value = dictionary;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					17,
					value
				}
			};
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
			ChatSettingsData? chatSettings = CacheDTO.chatSettings;
			if (!chatSettings.HasValue)
			{
				throw new Exception("Attempted to save changes to chat settings before they had been loaded from the server");
			}
			CacheDTO.chatSettings = _dependency;
		}
	}
}
