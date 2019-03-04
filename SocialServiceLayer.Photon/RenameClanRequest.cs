using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class RenameClanRequest : SocialRequest, IRenameClanRequest, IServiceRequest<ClanRenameDependency>, IAnswerOnComplete, IServiceRequest
	{
		private string _oldClanName;

		private string _newClanName;

		private string _adminName;

		protected override byte OperationCode => 46;

		public RenameClanRequest()
			: base("strClanRenameErrorTitle", "strClanRenameErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(31, _oldClanName);
			dictionary.Add(44, _newClanName);
			dictionary.Add(1, _adminName);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(ClanRenameDependency dependency)
		{
			_oldClanName = dependency.OldClanName;
			_newClanName = dependency.NewClanName;
			_adminName = dependency.AdminName;
		}
	}
}
