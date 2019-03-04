using Authentication;
using ExitGames.Client.Photon;
using SinglePlayerServiceLayer.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;
using System.IO;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal class SinglePlayerLoadTdmAiRobotsRequest : SinglePlayerRequestTask<Dictionary<string, PlayerDataDependency>>, ISinglePlayerLoadTdmAiRobotsRequest, IServiceRequest<SinglePlayerLoadTDMAIParameters>, IAnswerOnComplete<Dictionary<string, PlayerDataDependency>>, ITask, IServiceRequest, IAbstractTask
	{
		private SinglePlayerLoadTDMAIParameters _dependancy;

		protected override byte OperationCode => 1;

		public SinglePlayerLoadTdmAiRobotsRequest()
			: base("strRobocloudError", "strSinglePlayerStartError", 0)
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

		protected override Dictionary<string, PlayerDataDependency> ProcessResponse(OperationResponse response)
		{
			byte[] data = (byte[])response.Parameters[8];
			List<PlayerDataDependency> list = Deserialize(data);
			Dictionary<string, PlayerDataDependency> dictionary = new Dictionary<string, PlayerDataDependency>();
			for (int i = 0; i < list.Count; i++)
			{
				dictionary.Add(list[i].PlayerName, list[i]);
			}
			return dictionary;
		}

		private List<PlayerDataDependency> Deserialize(byte[] data)
		{
			List<PlayerDataDependency> list = new List<PlayerDataDependency>();
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						string text = binaryReader.ReadString();
						string displayName = binaryReader.ReadString();
						int masteryLevel = binaryReader.ReadInt32();
						int tier = binaryReader.ReadInt32();
						string robotName = binaryReader.ReadString();
						int count = binaryReader.ReadInt32();
						byte[] cubeMap = binaryReader.ReadBytes(count);
						binaryReader.ReadInt32();
						int team = binaryReader.ReadInt32();
						bool hasPremium = binaryReader.ReadBoolean();
						string robotUniqueId = binaryReader.ReadString();
						int cpu = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						int[] array = new int[num2];
						for (int j = 0; j < num2; j++)
						{
							array[j] = binaryReader.ReadInt32();
						}
						int count2 = binaryReader.ReadInt32();
						byte[] colourMap = binaryReader.ReadBytes(count2);
						bool aiPlayer = binaryReader.ReadBoolean();
						string spawnEffect = binaryReader.ReadString();
						string deathEffect = binaryReader.ReadString();
						AvatarInfo clanAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
						string text2;
						AvatarInfo avatarInfo;
						if (text == User.Username)
						{
							text2 = _dependancy.localUserClan;
							if (text2 != null)
							{
								clanAvatarInfo = _dependancy.UsersClanAvatarInfo;
							}
							avatarInfo = _dependancy.UsersPlayerAvatarInfo;
						}
						else
						{
							text2 = null;
							avatarInfo = new AvatarInfo(useCustomAvatar: false, AvatarUtils.ChooseAvatarIdForAi(text));
						}
						PlayerDataDependency item = new PlayerDataDependency(text, displayName, robotName, cubeMap, (uint)team, hasPremium, robotUniqueId, cpu, masteryLevel, tier, avatarInfo, text2, clanAvatarInfo, aiPlayer, spawnEffect, deathEffect, array, colourMap);
						list.Add(item);
					}
					return list;
				}
			}
		}

		public void Inject(SinglePlayerLoadTDMAIParameters dependency)
		{
			_dependancy = dependency;
		}
	}
}
