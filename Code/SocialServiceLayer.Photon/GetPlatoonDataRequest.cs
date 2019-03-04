using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;
using Utility;

namespace SocialServiceLayer.Photon
{
	internal class GetPlatoonDataRequest : SocialRequest<Platoon>, IGetPlatoonDataRequest, IServiceRequest, IAnswerOnComplete<Platoon>, ITask, IAbstractTask
	{
		private bool _forceRefresh;

		protected override byte OperationCode => 18;

		public bool isDone
		{
			get;
			private set;
		}

		public GetPlatoonDataRequest()
			: base("strRobocloudError", "strGetPlatDataError", 0)
		{
		}

		public void ForceRefresh()
		{
			_forceRefresh = true;
		}

		public override void Execute()
		{
			if (_forceRefresh || CacheDTO.platoon == null)
			{
				base.Execute();
				return;
			}
			isDone = true;
			base.answer.succeed((Platoon)CacheDTO.platoon.Clone());
		}

		protected override Platoon ProcessResponse(OperationResponse response)
		{
			Dictionary<byte, object> parameters = response.Parameters;
			string parameter = parameters.GetParameter<string>(SocialParameterCode.PlatoonId);
			if (parameter != null)
			{
				string parameter2 = parameters.GetParameter<string>(SocialParameterCode.PlatoonLeader);
				PlatoonMember[] parameter3 = parameters.GetParameter<PlatoonMember[]>(SocialParameterCode.UserList);
				Platoon platoon = new Platoon(parameter, parameter2);
				for (int i = 0; i < parameter3.Length; i++)
				{
					platoon.AddMember(parameter3[i]);
				}
				CacheDTO.platoon = platoon;
			}
			else
			{
				CacheDTO.platoon = new Platoon();
			}
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			isDone = true;
			return (Platoon)CacheDTO.platoon.Clone();
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
			isDone = true;
			Console.LogError("Error getting platoon data, defaulting to no platoon. Exception: " + e);
			CacheDTO.platoon = new Platoon();
			base.answer.succeed((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
