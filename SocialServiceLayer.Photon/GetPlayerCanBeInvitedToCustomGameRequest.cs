using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class GetPlayerCanBeInvitedToCustomGameRequest : SocialRequest<bool>, IGetPlayerCanBeInvitedToCustomGameRequest, ITask, IServiceRequest<string>, IAnswerOnComplete<bool>, IServiceRequest, IAbstractTask
	{
		private string _dependancy;

		protected override byte OperationCode => 58;

		public bool isDone
		{
			get;
			private set;
		}

		public GetPlayerCanBeInvitedToCustomGameRequest()
			: base("strRobocloudError", "strGetPlatoonCustomGameCanBeInvitedError", 0)
		{
		}

		protected override bool ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			bool result = (bool)parameters[66];
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
