using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class SearchClansRequest : SocialRequest<ClanInfo[]>, ISearchClansRequest, IServiceRequest<SearchClanDependency>, IAnswerOnComplete<ClanInfo[]>, IServiceRequest
	{
		private SearchClanDependency _dependency;

		protected override byte OperationCode => 32;

		public SearchClansRequest()
			: base("strSearchClansErrorTitle", "strSearchClansErrorBody", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Expected O, but got Unknown
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			dictionary.Add(39, _dependency.SearchString);
			dictionary.Add(40, _dependency.DaysSinceActive);
			dictionary.Add(41, _dependency.StartRange);
			dictionary.Add(43, _dependency.EndRange);
			dictionary.Add(34, _dependency.ClanTypes);
			Dictionary<byte, object> parameters = dictionary;
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = parameters;
			return val;
		}

		public void Inject(SearchClanDependency dependency)
		{
			_dependency = dependency;
		}

		protected override ClanInfo[] ProcessResponse(OperationResponse response)
		{
			Hashtable[] array = (Hashtable[])response.Parameters[42];
			ClanInfo[] array2 = new ClanInfo[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Hashtable clanInfoRaw = array[i];
				array2[i] = ClanInfo.FromHashtable(clanInfoRaw);
			}
			return array2;
		}
	}
}
