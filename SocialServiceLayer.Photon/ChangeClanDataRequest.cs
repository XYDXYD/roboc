using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class ChangeClanDataRequest : SocialRequest, IChangeClanDataRequest, IServiceRequest<ChangeClanDataDependency>, IAnswerOnComplete, IServiceRequest
	{
		private ChangeClanDataDependency _dependency;

		protected override byte OperationCode => 42;

		public ChangeClanDataRequest()
			: base("strChangeClanDataErrorTitle", "strChangeClanDataErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			if (_dependency.NewDefaultClanAvatarId.HasValue)
			{
				dictionary.Add(33, _dependency.NewDefaultClanAvatarId.Value);
			}
			if (_dependency.NewDescription != null)
			{
				dictionary.Add(32, _dependency.NewDescription);
			}
			if (_dependency.NewType.HasValue)
			{
				dictionary.Add(34, _dependency.NewType.Value);
			}
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = dictionary;
			return val;
		}

		public void Inject(ChangeClanDataDependency dependency)
		{
			_dependency = dependency;
		}

		public override void OnOpResponse(OperationResponse response)
		{
			if (_dependency.NewDescription != null)
			{
				CacheDTO.MyClanInfo.ClanDescription = _dependency.NewDescription;
			}
			if (_dependency.NewType.HasValue)
			{
				CacheDTO.MyClanInfo.ClanType = _dependency.NewType.Value;
			}
			base.OnOpResponse(response);
		}
	}
}
