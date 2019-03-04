using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Services.Web.Photon
{
	internal class LoadCustomGamesAllowedMapsRequest : WebServicesCachedRequest<CustomGamesAllowedMapsData>, ILoadCustomGamesAllowedMapsRequest, IServiceRequest, IAnswerOnComplete<CustomGamesAllowedMapsData>
	{
		protected override byte OperationCode => 146;

		public LoadCustomGamesAllowedMapsRequest()
			: base("strRobocloudError", "strCustomGameRequestErrorLoadMaps", 3)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			return val;
		}

		protected override CustomGamesAllowedMapsData ProcessResponse(OperationResponse response)
		{
			object obj = response.Parameters[170];
			object obj2 = response.Parameters[178];
			Dictionary<string, object[]> dictionary = (Dictionary<string, object[]>)obj;
			Dictionary<string, string> mapNameStrings_ = (Dictionary<string, string>)obj2;
			Dictionary<GameModeType, List<string>> dictionary2 = new Dictionary<GameModeType, List<string>>();
			IEnumerator enumerator = Enum.GetValues(typeof(GameModeType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GameModeType key = (GameModeType)enumerator.Current;
					dictionary2[key] = new List<string>();
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (KeyValuePair<string, object[]> item in dictionary)
			{
				object obj3 = null;
				try
				{
					obj3 = Enum.Parse(typeof(GameModeType), item.Key);
				}
				catch
				{
					Console.LogError("Error in LoadCustomGamesAllowedMapsRequest: unexpected Game mode type. perhaps the enums don't match or the json doesn't match the enum.");
				}
				if (obj3 != null)
				{
					List<string> list = new List<string>(item.Value.Length);
					IEnumerator enumerator3 = item.Value.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						list.Add(enumerator3.Current.ToString());
					}
					dictionary2[(GameModeType)obj3] = list;
				}
			}
			return new CustomGamesAllowedMapsData(dictionary2, mapNameStrings_);
		}
	}
}
