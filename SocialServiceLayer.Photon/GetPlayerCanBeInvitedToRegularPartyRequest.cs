using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetPlayerCanBeInvitedToRegularPartyRequest : SocialRequest<GetPlayerCanBeInvitedToRegularPartyResponseCode>, IGetPlayerCanBeInvitedToRegularPartyRequest, ITask, IServiceRequest<string>, IAnswerOnComplete<GetPlayerCanBeInvitedToRegularPartyResponseCode>, IServiceRequest, IAbstractTask
	{
		private string _dependancy;

		protected override byte OperationCode => 59;

		public bool isDone
		{
			get;
			private set;
		}

		public GetPlayerCanBeInvitedToRegularPartyRequest()
			: base("strRobocloudError", "strCustomGameCheckCanBeInvitedToRegularPartyError", 0)
		{
		}

		protected override GetPlayerCanBeInvitedToRegularPartyResponseCode ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			GetPlayerCanBeInvitedToRegularPartyResponseCode result = (GetPlayerCanBeInvitedToRegularPartyResponseCode)parameters[66];
			isDone = true;
			return result;
		}

		protected override void OnFailed(Exception exception)
		{
			isDone = true;
			base.OnFailed(exception);
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>
			{
				{
					65,
					_dependancy
				}
			};
			return val;
		}

		public void Inject(string dependency)
		{
			_dependancy = dependency;
		}
	}
}
