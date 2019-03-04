using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetChatSettingsRequest : WebServicesRequest<ChatSettingsData>, ILoadChatSettingsRequest, IServiceRequest, IAnswerOnComplete<ChatSettingsData>
	{
		protected override byte OperationCode => 18;

		public GetChatSettingsRequest()
			: base("strRobocloudError", "strUnableLoadChatSettings", 3)
		{
		}

		public override void Execute()
		{
			ChatSettingsData? chatSettings = CacheDTO.chatSettings;
			if (!chatSettings.HasValue)
			{
				base.Execute();
			}
			else if (base.answer != null && base.answer.succeed != null)
			{
				base.answer.succeed(CacheDTO.chatSettings.Value);
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ChatSettingsData ProcessResponse(OperationResponse response)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			Hashtable data = response.Parameters[17];
			ChatSettingsData chatSettingsData = default(ChatSettingsData);
			chatSettingsData.chatEnabled = TryGet(data, "chatEnabled", defval: true);
			ChatSettingsData chatSettingsData2 = chatSettingsData;
			CacheDTO.chatSettings = chatSettingsData2;
			return chatSettingsData2;
		}

		private static T TryGet<T>(Hashtable data, string k, T defval)
		{
			if (((Dictionary<object, object>)data).TryGetValue((object)k, out object value))
			{
				return (T)value;
			}
			return defval;
		}
	}
}
