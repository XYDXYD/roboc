using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class SetSocialSettingsRequest : SocialRequest, ISetSocialSettingsRequest, IServiceRequest<Dictionary<string, object>>, IAnswerOnComplete, IServiceRequest
	{
		private Dictionary<string, object> _settings;

		protected override byte OperationCode => 25;

		public SetSocialSettingsRequest()
			: base("strRobocloudError", "strSaveSocialSetError", 0)
		{
		}

		public void Inject(Dictionary<string, object> settings)
		{
			_settings = settings;
		}

		public override void Execute()
		{
			if (CacheDTO.socialSettings == null)
			{
				CacheDTO.socialSettings = new Dictionary<string, object>();
			}
			CacheDTO.socialSettings = _settings;
			base.Execute();
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary[30] = _settings;
			val.Parameters = dictionary;
			val.OperationCode = OperationCode;
			return val;
		}
	}
}
