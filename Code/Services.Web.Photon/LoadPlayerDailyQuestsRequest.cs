using ExitGames.Client.Photon;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace Services.Web.Photon
{
	internal sealed class LoadPlayerDailyQuestsRequest : WebServicesRequest<PlayerDailyQuestsData>, ILoadPlayerDailyQuestsRequest, IServiceRequest, IAnswerOnComplete<PlayerDailyQuestsData>
	{
		protected override byte OperationCode => 13;

		public LoadPlayerDailyQuestsRequest()
			: base("strRobocloudError", "strErrorDailyQuestsRequest", 0)
		{
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override PlayerDailyQuestsData ProcessResponse(OperationResponse response)
		{
			byte[] data = (byte[])response.get_Item((byte)155);
			return Deserialize(data);
		}

		private PlayerDailyQuestsData Deserialize(byte[] data)
		{
			PlayerDailyQuestsData playerDailyQuestsData = new PlayerDailyQuestsData();
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					playerDailyQuestsData.canRemoveQuest = binaryReader.ReadBoolean();
					playerDailyQuestsData.playerQuests = DeserialiseQuests(binaryReader);
					playerDailyQuestsData.completedQuests = DeserialiseQuests(binaryReader);
					return playerDailyQuestsData;
				}
			}
		}

		private List<Quest> DeserialiseQuests(BinaryReader br)
		{
			List<Quest> list = new List<Quest>();
			short num = br.ReadInt16();
			for (int i = 0; i < num; i++)
			{
				Quest quest = new Quest();
				quest.questID = br.ReadString();
				quest.questNameStrKey = br.ReadString();
				quest.questDescStrKey = br.ReadString();
				quest.xp = br.ReadInt32();
				quest.premiumXP = br.ReadInt32();
				quest.robits = br.ReadInt32();
				quest.premiumRobits = br.ReadInt32();
				quest.progressCount = br.ReadInt32();
				quest.targetCount = br.ReadInt32();
				quest.seen = br.ReadBoolean();
				list.Add(quest);
			}
			return list;
		}
	}
}
