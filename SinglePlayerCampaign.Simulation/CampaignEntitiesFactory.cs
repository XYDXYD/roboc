using Simulation.SinglePlayerCampaign.DataTypes;
using Simulation.SinglePlayerCampaign.EntityDescriptors;
using Simulation.SinglePlayerCampaign.Implementors;
using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.EntityDescriptors;
using SinglePlayerCampaign.Simulation.Implementors;
using SinglePlayerServiceLayer;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;

namespace SinglePlayerCampaign.Simulation
{
	internal static class CampaignEntitiesFactory
	{
		public const int FIRST_WAVE_INDEX = 0;

		public static void BuildEntities(ISinglePlayerRequestFactory singlePlayerRequestFactory, IEntityFactory entityFactory)
		{
			TaskRunner.get_Instance().Run(BuildEntitiesAsTask(singlePlayerRequestFactory, entityFactory));
		}

		private static IEnumerator BuildEntitiesAsTask(ISinglePlayerRequestFactory singlePlayerRequestFactory, IEntityFactory entityFactory)
		{
			IGetCampaignWavesDataRequest getCampaignWavesDataReq = singlePlayerRequestFactory.Create<IGetCampaignWavesDataRequest>();
			getCampaignWavesDataReq.Inject(new GetCampaignWavesDependency(WorldSwitching.GetCampaignID(), WorldSwitching.GetCampaignDifficulty()));
			TaskService<CampaignWavesDifficultyData> getCampaignWavesDataTS = new TaskService<CampaignWavesDifficultyData>(getCampaignWavesDataReq);
			yield return new HandleTaskServiceWithError(getCampaignWavesDataTS, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			CampaignWavesDifficultyData campaignWavesDifficultyData = getCampaignWavesDataTS.result;
			WavesDataImplementor wavesDataImplementor = new WavesDataImplementor
			{
				wavesData = campaignWavesDifficultyData
			};
			entityFactory.BuildEntity<WavesDataEntityDescriptor>(204, new object[1]
			{
				wavesDataImplementor
			});
			CampaignDifficultySetting campaignDifficulty = campaignWavesDifficultyData.CampaignDifficulty;
			PlayerSetting playerDifficultySetting = campaignDifficulty.PlayerDifficultySetting;
			int totalLives = playerDifficultySetting.TotalLives;
			RemainingLivesImplementor remainingLivesImplementor = new RemainingLivesImplementor(totalLives, 208);
			CampaignDefeatImplementor campaignDefeatImplementor = new CampaignDefeatImplementor(208);
			int wavesNumber = campaignWavesDifficultyData.WavesData.get_Count();
			CounterImplementor currentWaveTrackerImplementor = new CounterImplementor(208, 0, wavesNumber);
			WaveReadyToSpawnImplementor waveReadyToSpawnImplementor = new WaveReadyToSpawnImplementor(208);
			TransitionAnimationTriggerImplementor waveSpawnCompletedImplementor = new TransitionAnimationTriggerImplementor(208);
			CampaignVictoryImplementor campaignVictoryImplementor = new CampaignVictoryImplementor(208);
			entityFactory.BuildEntity<CampaignEntityDescriptor>(208, new object[6]
			{
				remainingLivesImplementor,
				campaignDefeatImplementor,
				currentWaveTrackerImplementor,
				waveReadyToSpawnImplementor,
				waveSpawnCompletedImplementor,
				campaignVictoryImplementor
			});
			WaveData firstWaveData = campaignWavesDifficultyData.WavesData.get_Item(0);
			BuildCampaignWaveEntity(entityFactory, firstWaveData);
		}

		public static void BuildCampaignWaveEntity(IEntityFactory entityFactory, WaveData waveData)
		{
			WaveDataImplementor waveDataImplementor = new WaveDataImplementor(waveData);
			KillCountImplementor killCountImplementor = new KillCountImplementor(207);
			TimeImplementor timeImplementor = new TimeImplementor(207);
			WaveVictoryImplementor waveVictoryImplementor = new WaveVictoryImplementor(waveData.MinimumTime, waveData.KillTargetAmount, 207);
			WaveDefeatImplementor waveDefeatImplementor = new WaveDefeatImplementor(waveData.MaximumTime, 207);
			int num = waveData.WaveRobots.Length;
			SpawnEvent[] array = new SpawnEvent[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new SpawnEvent
				{
					timeOfNextSpawn = waveData.WaveRobots[i].timeToSpawn,
					robotsKilled = new DispatchOnSet<int>(207)
				};
			}
			SpawnDataImplementor spawnDataImplementor = new SpawnDataImplementor(array);
			SpawnRequestImplementor spawnRequestImplementor = new SpawnRequestImplementor(207);
			CounterImplementor counterImplementor = new CounterImplementor(207, 0, -1);
			entityFactory.BuildEntity<CampaignWaveEntityDescriptor>(207, new object[8]
			{
				waveDataImplementor,
				killCountImplementor,
				timeImplementor,
				waveVictoryImplementor,
				waveDefeatImplementor,
				spawnDataImplementor,
				spawnRequestImplementor,
				counterImplementor
			});
		}
	}
}
