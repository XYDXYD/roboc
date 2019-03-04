using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class GetPlatoonPendingInviteRequest : SocialRequest<PlatoonInvite>, IGetPlatoonPendingInviteRequest, IServiceRequest, IAnswerOnComplete<PlatoonInvite>
	{
		private bool _forceRefresh;

		protected override byte OperationCode => 19;

		public GetPlatoonPendingInviteRequest()
			: base("strRobocloudError", "strGetPlatInvitesError", 0)
		{
		}

		public void ForceRefresh()
		{
			_forceRefresh = true;
		}

		public override void Execute()
		{
			if (_forceRefresh)
			{
				base.Execute();
			}
			else if (CacheDTO.platoonInvite != null)
			{
				base.answer.succeed(CacheDTO.platoonInvite.Clone());
			}
			else
			{
				base.answer.succeed(null);
			}
		}

		protected override PlatoonInvite ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			if (parameters.ContainsKey(19))
			{
				string inviterName = (string)parameters[19];
				string displayName = (string)parameters[75];
				bool useCustomAvatar = (bool)parameters[13];
				int avatarId = (int)parameters[14];
				CacheDTO.platoonInvite = new PlatoonInvite(inviterName, displayName, new AvatarInfo(useCustomAvatar, avatarId));
				return CacheDTO.platoonInvite.Clone();
			}
			return null;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void OnFailed(Exception e)
		{
			Console.LogError("Error getting platoon invite data, defaulting to no invite. Exception: " + e);
			CacheDTO.platoonInvite = null;
			base.answer.succeed(null);
		}
	}
}
