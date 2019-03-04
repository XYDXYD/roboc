using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using System.IO;

namespace Services.Web.Photon
{
	internal class GetAvailableGameModesRequest : WebServicesCachedRequest<List<GameModeType>>, IGetAvailableGameModesRequest, IServiceRequest<LobbyType>, IAnswerOnComplete<List<GameModeType>>, IServiceRequest
	{
		private LobbyType _dependency;

		protected override byte OperationCode => 116;

		public GetAvailableGameModesRequest()
			: base("strRobocloudError", "strUnableToGetAvailableGameModesError", 1)
		{
		}

		public void Inject(LobbyType dependency)
		{
			_dependency = dependency;
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
					134,
					_dependency
				}
			};
			return val;
		}

		protected override List<GameModeType> ProcessResponse(OperationResponse response)
		{
			Dictionary<LobbyType, List<GameModeType>> dictionary = new Dictionary<LobbyType, List<GameModeType>>();
			byte[] buffer = (byte[])response.Parameters[109];
			using (MemoryStream input = new MemoryStream(buffer))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						LobbyType key = (LobbyType)Enum.Parse(typeof(LobbyType), binaryReader.ReadString());
						int num2 = binaryReader.ReadInt32();
						List<GameModeType> list = new List<GameModeType>();
						for (int j = 0; j < num2; j++)
						{
							GameModeType item = (GameModeType)binaryReader.ReadInt32();
							list.Add(item);
						}
						dictionary[key] = list;
					}
				}
			}
			return dictionary[_dependency];
		}
	}
}
