using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;

namespace Services.Web.Photon
{
	internal class GetAvatarInfoRequest : WebServicesRequest<AvatarInfo>, IGetAvatarInfoRequest, IServiceRequest, IAnswerOnComplete<AvatarInfo>, ITask, IAbstractTask
	{
		protected override byte OperationCode => 110;

		public bool isDone
		{
			get;
			private set;
		}

		public GetAvatarInfoRequest()
			: base("strRobocloudError", "strErrorGettingAvatarInfo", 0)
		{
		}

		protected override AvatarInfo ProcessResponse(OperationResponse response)
		{
			isDone = true;
			bool flag = (bool)response.Parameters[130];
			int avatarId = (!flag) ? ((int)response.Parameters[129]) : 0;
			CacheDTO.localPlayerAvatarInfo = new AvatarInfo(flag, avatarId);
			return CacheDTO.localPlayerAvatarInfo;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			return new OperationRequest();
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}

		public override void Execute()
		{
			if (CacheDTO.localPlayerAvatarInfo != null)
			{
				isDone = true;
				IServiceAnswer<AvatarInfo> answer = base.answer;
				if (answer == null)
				{
					throw new Exception("Answer is null or wrong type");
				}
				answer.succeed(CacheDTO.localPlayerAvatarInfo);
			}
			else
			{
				base.Execute();
			}
		}
	}
}
