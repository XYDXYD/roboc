using ExitGames.Client.Photon;
using Services.Web.Photon;
using Simulation.SinglePlayerCampaign.DataTypes;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System.Collections.Generic;
using System.IO;

namespace SinglePlayerServiceLayer.Requests.Photon
{
	internal class GetCampaignWavesDataRequest : WebServicesCachedRequest<CampaignWavesDifficultyData>, IGetCampaignWavesDataRequest, IServiceRequest<GetCampaignWavesDependency>, IAnswerOnComplete<CampaignWavesDifficultyData>, IServiceRequest
	{
		private GetCampaignWavesDependency _dependency;

		protected override byte OperationCode => 64;

		public GetCampaignWavesDataRequest()
			: base("strRobocloudError", "strSinglePlayerStartError", 0)
		{
		}

		public void Inject(GetCampaignWavesDependency dependency)
		{
			_dependency = dependency;
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.OperationCode = OperationCode;
			val.Parameters = new Dictionary<byte, object>();
			OperationRequest val2 = val;
			val2.Parameters[22] = _dependency.CampaignID;
			val2.Parameters[23] = _dependency.CampaignDifficulty;
			return val2;
		}

		protected override CampaignWavesDifficultyData ProcessResponse(OperationResponse response)
		{
			byte[] bytes = (byte[])response.Parameters[75];
			return DeserialiseCampaignWavesParameters(bytes);
		}

		private static CampaignWavesDifficultyData DeserialiseCampaignWavesParameters(byte[] bytes)
		{
			FasterList<WaveData> val = new FasterList<WaveData>();
			CampaignDifficultySetting? campaignDifficultySetting;
			using (MemoryStream input = new MemoryStream(bytes))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					int count = binaryReader.ReadInt32();
					byte[] data = binaryReader.ReadBytes(count);
					campaignDifficultySetting = new CampaignDifficultySetting(data);
					int num = binaryReader.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						int playerSpawnLocation_ = binaryReader.ReadInt32();
						int num2 = binaryReader.ReadInt32();
						FasterList<WaveRobot> val2 = new FasterList<WaveRobot>();
						for (int j = 0; j < num2; j++)
						{
							string robotName_ = binaryReader.ReadString();
							int count2 = binaryReader.ReadInt32();
							byte[] serializedRobotData_ = binaryReader.ReadBytes(count2);
							int count3 = binaryReader.ReadInt32();
							byte[] serializedRobotDataColour_ = binaryReader.ReadBytes(count3);
							int timeToSpawn_ = binaryReader.ReadInt32();
							int killsToSpawn_ = binaryReader.ReadInt32();
							int timeToDespawn_ = binaryReader.ReadInt32();
							int killsToDespawn_ = binaryReader.ReadInt32();
							int initialRobotAmount_ = binaryReader.ReadInt32();
							int periodicRobotAmount_ = binaryReader.ReadInt32();
							int spawnInterval_ = binaryReader.ReadInt32();
							int minRobotAmount_ = binaryReader.ReadInt32();
							int maxRobotAmount_ = binaryReader.ReadInt32();
							bool isBoss_ = binaryReader.ReadBoolean();
							bool isKillRequirement_ = binaryReader.ReadBoolean();
							WaveRobot waveRobot = new WaveRobot(robotName_, serializedRobotData_, serializedRobotDataColour_, timeToSpawn_, killsToSpawn_, timeToDespawn_, killsToDespawn_, initialRobotAmount_, periodicRobotAmount_, spawnInterval_, minRobotAmount_, maxRobotAmount_, isBoss_, isKillRequirement_);
							val2.Add(waveRobot);
						}
						int killTarget_ = binaryReader.ReadInt32();
						int timeMin_ = binaryReader.ReadInt32();
						int timeMax_ = binaryReader.ReadInt32();
						WaveData waveData = new WaveData(playerSpawnLocation_, val2.ToArray(), killTarget_, timeMin_, timeMax_);
						val.Add(waveData);
					}
				}
			}
			return new CampaignWavesDifficultyData(campaignDifficultySetting.Value, val);
		}

		void IGetCampaignWavesDataRequest.ClearCache()
		{
			ClearCache();
		}
	}
}
