using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal class LoadBrawlLanguageOverridesRequest : WebServicesCachedRequest<BrawlOverrideLanguageStrings>, ILoadBrawlLanguageOverrides, IServiceRequest<string>, IAnswerOnComplete<BrawlOverrideLanguageStrings>, ITask, IServiceRequest, IAbstractTask
	{
		private string _languageChoice;

		protected override byte OperationCode => 140;

		public bool isDone
		{
			get;
			private set;
		}

		public LoadBrawlLanguageOverridesRequest()
			: base("strRobocloudError", "strUnableLoadBrawlLanguageOverrides", 3)
		{
		}

		public void Inject(string languageChoice)
		{
			_languageChoice = languageChoice;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[161] = _languageChoice;
			return val;
		}

		protected override BrawlOverrideLanguageStrings ProcessResponse(OperationResponse response)
		{
			object obj = response.Parameters[162];
			Dictionary<string, string> inputLanguageStrings = (Dictionary<string, string>)obj;
			isDone = true;
			return new BrawlOverrideLanguageStrings(inputLanguageStrings);
		}

		void ILoadBrawlLanguageOverrides.ClearCache()
		{
			ClearCache();
		}
	}
}
