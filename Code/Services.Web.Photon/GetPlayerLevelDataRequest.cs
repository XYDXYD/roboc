using ExitGames.Client.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class GetPlayerLevelDataRequest : WebServicesCachedRequest<IDictionary<uint, uint>>, ILoadPlayerLevelDataRequest, IServiceRequest, IAnswerOnComplete<IDictionary<uint, uint>>
	{
		protected override byte OperationCode => 3;

		public GetPlayerLevelDataRequest()
			: base("strRobocloudError", "strUnableLoadPlayerLevelCloud", 3)
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

		protected override IDictionary<uint, uint> ProcessResponse(OperationResponse response)
		{
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, Hashtable> dictionary = (Dictionary<string, Hashtable>)response.Parameters[1];
			SortedDictionary<uint, uint> sortedDictionary = new SortedDictionary<uint, uint>();
			using (Dictionary<string, Hashtable>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					sortedDictionary.Add(Convert.ToUInt32(enumerator.Current.Key), Convert.ToUInt32(enumerator.Current.Value.get_Item((object)"LevelPointsRequired")));
				}
			}
			Dictionary<uint, uint> dictionary2 = new Dictionary<uint, uint>();
			using (SortedDictionary<uint, uint>.Enumerator enumerator2 = sortedDictionary.GetEnumerator())
			{
				uint num = 0u;
				while (enumerator2.MoveNext())
				{
					uint key = enumerator2.Current.Key;
					dictionary2.Add(key, enumerator2.Current.Value);
					if (key - num > 1)
					{
						for (uint num2 = num + 1; num2 < key; num2++)
						{
							uint num3 = dictionary2[num];
							uint num4 = dictionary2[key];
							float num5 = ((float)(double)num2 - (float)(double)num) / (float)(double)(key - num);
							uint value = (uint)Math.Round((float)(double)num3 + (float)(double)(num4 - num3) * num5, MidpointRounding.ToEven);
							dictionary2.Add(num2, value);
						}
					}
					num = key;
				}
			}
			return (IDictionary<uint, uint>)(object)new ReadOnlyDictionary<uint, uint>(dictionary2);
		}
	}
}
