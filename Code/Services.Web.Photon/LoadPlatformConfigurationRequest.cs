using Authentication;
using ExitGames.Client.Photon;
using Services.Requests.Interfaces;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadPlatformConfigurationRequest : WebServicesCachedRequest<PlatformConfigurationSettings>, ILoadPlatformConfigurationRequest, IServiceRequest, IAnswerOnComplete<PlatformConfigurationSettings>
	{
		protected override byte OperationCode => 165;

		public LoadPlatformConfigurationRequest()
			: base("strGenericError", "strLoadPlatformConfigError", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override PlatformConfigurationSettings ProcessResponse(OperationResponse response)
		{
			try
			{
				object obj = response.Parameters[197];
				Dictionary<object, object> configData = (Dictionary<object, object>)obj;
				return Deserialise(configData);
			}
			catch (Exception ex)
			{
				RemoteLogger.Error("Error loading plaform configuration settings", "user = " + User.Username, ex.ToString());
				throw new Exception(FastConcatUtility.FastConcat<Exception>(FastConcatUtility.FastConcat<string>("Error loading plaform configuration settings", " "), ex));
			}
		}

		private static PlatformConfigurationSettings Deserialise(IDictionary configData)
		{
			bool buyPremiumAvailable_ = Decode.Get<bool>(configData, "BuyPremiumAvailable");
			bool mainShopButtonAvailable_ = Decode.Get<bool>(configData, "MainShopButtonAvailable");
			bool roboPassButtonAvailable_ = Decode.Get<bool>(configData, "RoboPassButtonAvailable");
			bool languageSelectionAvailable_ = Decode.Get<bool>(configData, "LanguageSelectionAvailable");
			string autoJoinPublicChatRoom_ = Decode.Get<string>(configData, "AutoJoinPublicChatRoom");
			bool canCreateChatRooms_ = Decode.Get<bool>(configData, "CanCreateChatRooms");
			bool isCurseVoiceEnabled_ = Decode.Get<bool>(configData, "CurseVoiceEnabled");
			bool isDeltaDNAEnabled_ = Decode.Get<bool>(configData, "DeltaDNAEnabled");
			bool useDecimalSystem_ = Decode.Get<bool>(configData, "UseDecimalSystem");
			string feedbackURL_ = Decode.Get<string>(configData, "FeedbackURL");
			string supportURL_ = Decode.Get<string>(configData, "SupportURL");
			string wikiURL_ = Decode.Get<string>(configData, "WikiURL");
			return new PlatformConfigurationSettings(buyPremiumAvailable_, mainShopButtonAvailable_, roboPassButtonAvailable_, languageSelectionAvailable_, autoJoinPublicChatRoom_, canCreateChatRooms_, isCurseVoiceEnabled_, isDeltaDNAEnabled_, useDecimalSystem_, feedbackURL_, supportURL_, wikiURL_);
		}
	}
}
