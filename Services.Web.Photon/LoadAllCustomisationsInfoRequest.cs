using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadAllCustomisationsInfoRequest : WebServicesCachedRequest<AllCustomisationsResponse>, ILoadAllCustomisationInfoRequest, IServiceRequest, IAnswerOnComplete<AllCustomisationsResponse>, ITask, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		protected override byte OperationCode => 216;

		public LoadAllCustomisationsInfoRequest()
			: base("strRobocloudError", "strGetPlayerAvailableCustomisations", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override AllCustomisationsResponse ProcessResponse(OperationResponse response)
		{
			Hashtable[] array = (Hashtable[])response.Parameters[228];
			Hashtable[] array2 = (Hashtable[])response.Parameters[229];
			Hashtable[] array3 = (Hashtable[])response.Parameters[230];
			List<CustomisationsEntry> list = new List<CustomisationsEntry>();
			Hashtable[] array4 = array;
			foreach (Hashtable table in array4)
			{
				list.Add(CustomisationsEntry.DeserialiseFromHashtable(table));
			}
			List<CustomisationsEntry> list2 = new List<CustomisationsEntry>();
			Hashtable[] array5 = array2;
			foreach (Hashtable table2 in array5)
			{
				list2.Add(CustomisationsEntry.DeserialiseFromHashtable(table2));
			}
			List<CustomisationsEntry> list3 = new List<CustomisationsEntry>();
			Hashtable[] array6 = array3;
			foreach (Hashtable table3 in array6)
			{
				list3.Add(CustomisationsEntry.DeserialiseFromHashtable(table3));
			}
			string[] ownedBaySkinCustomisations_ = (string[])response.Parameters[231];
			string[] ownedSpawnFXCustomisations_ = (string[])response.Parameters[232];
			string[] ownedDeathFXCustomisations_ = (string[])response.Parameters[233];
			AllCustomisationsResponse result = new AllCustomisationsResponse(ownedBaySkinCustomisations_, ownedSpawnFXCustomisations_, ownedDeathFXCustomisations_, list, list2, list3);
			isDone = true;
			return result;
		}

		void ILoadAllCustomisationInfoRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
