using ExitGames.Client.Photon;
using Simulation;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace Services.Web.Photon
{
	internal class UpdatePlayerDailyQuestProgressRequest : WebServicesRequest, IUpdatePlayerDailyQuestProgressRequest, IServiceRequest<LocalPlayerDailyQuestProgress>, IAnswerOnComplete, IServiceRequest
	{
		private LocalPlayerDailyQuestProgress _progress;

		protected override byte OperationCode => 12;

		public UpdatePlayerDailyQuestProgressRequest()
			: base("strRobocloudError", "strErrorUpdatePlayerDailyQuestProgressRequest", 0)
		{
		}

		public void Inject(LocalPlayerDailyQuestProgress progress)
		{
			_progress = progress;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.Parameters[21] = SerialiseDailyQuestProgress();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override void ProcessResponse(OperationResponse response)
		{
		}

		private byte[] SerialiseDailyQuestProgress()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(_progress.gameMode.ToString());
					binaryWriter.Write(_progress.isRanked);
					binaryWriter.Write(_progress.isBrawl);
					binaryWriter.Write(_progress.isCustomGame);
					binaryWriter.Write(_progress.gameEnded);
					binaryWriter.Write(_progress.gameWon);
					binaryWriter.Write(_progress.playerRobotUniqueId);
					Dictionary<ItemCategory, int> killCountWithWeapon = _progress.killCountWithWeapon;
					binaryWriter.Write(killCountWithWeapon.Keys.Count);
					foreach (KeyValuePair<ItemCategory, int> item in killCountWithWeapon)
					{
						binaryWriter.Write((int)item.Key);
						binaryWriter.Write(item.Value);
					}
					Dictionary<int, int> playerHealStartHealthPercent = _progress.playerHealStartHealthPercent;
					binaryWriter.Write(playerHealStartHealthPercent.Keys.Count);
					foreach (KeyValuePair<int, int> item2 in playerHealStartHealthPercent)
					{
						binaryWriter.Write(item2.Key);
						binaryWriter.Write(item2.Value);
					}
					FasterList<string> partyPlayerNames = _progress.partyPlayerNames;
					binaryWriter.Write(partyPlayerNames.get_Count());
					for (int i = 0; i < partyPlayerNames.get_Count(); i++)
					{
						binaryWriter.Write(partyPlayerNames.get_Item(i));
					}
				}
				return memoryStream.ToArray();
			}
		}
	}
}
