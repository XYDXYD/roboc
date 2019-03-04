using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace ChatServiceLayer.Photon
{
	internal class AddOrUpdateSanctionRequest : ChatRequest, IAddOrUpdateSanctionRequest, IServiceRequest<AddOrUpdateSanctionDependency>, IAnswerOnComplete, IServiceRequest
	{
		private Sanction _sanction;

		private bool _adding;

		private int _duration;

		private string _username;

		protected override byte OperationCode => 7;

		public AddOrUpdateSanctionRequest()
			: base("strRobocloudError", "strErrorIssuingSanction", 0)
		{
		}

		public void Inject(AddOrUpdateSanctionDependency dependency)
		{
			_sanction = dependency.Sanction;
			_adding = !dependency.Remove;
			_duration = dependency.Duration;
			_username = dependency.UserName;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(9, _sanction.SanctionType);
			dictionary.Add(10, _adding);
			dictionary.Add(11, _duration);
			dictionary.Add(2, _sanction.Reason);
			dictionary.Add(7, _username);
			Dictionary<byte, object> dictionary2 = val.Parameters = dictionary;
			return val;
		}
	}
}
