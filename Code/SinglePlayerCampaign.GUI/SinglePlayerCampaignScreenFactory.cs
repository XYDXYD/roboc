using Simulation.GUI;
using SinglePlayerCampaign.GUI.Mothership;
using SinglePlayerCampaign.GUI.Mothership.EntityDescriptors;
using SinglePlayerCampaign.GUI.Simulation.EntityDescriptors;
using SinglePlayerCampaign.GUI.Simulation.Implementors;
using Svelto.ECS;
using Svelto.ServiceLayer;
using UnityEngine;

namespace SinglePlayerCampaign.GUI
{
	internal static class SinglePlayerCampaignScreenFactory
	{
		internal static void BuildMothershipUI(IEntityFactory entityFactory, GameObject go, IServiceRequestFactory serviceRequestFactory)
		{
			SinglePlayerCampaignListScreen componentInChildren = go.GetComponentInChildren<SinglePlayerCampaignListScreen>();
			componentInChildren.Initialise();
			int instanceID = go.GetInstanceID();
			go.SetActive(false);
			entityFactory.BuildEntity<SinglePlayerCampaignLayoutEntityDescriptor>(instanceID, new object[1]
			{
				componentInChildren
			});
		}

		internal static void BuildSimulationEntities(IEntityFactory entityFactory, GameObject go, PlayerMachinesContainer playerMachinesContainer)
		{
			RemainingLivesView componentInChildren = go.GetComponentInChildren<RemainingLivesView>();
			int instanceID = componentInChildren.get_gameObject().GetInstanceID();
			object[] array = new object[1]
			{
				componentInChildren
			};
			entityFactory.BuildEntity<RemainingLivesWidgetEntityDescriptor>(instanceID, array);
			CurrentWaveView componentInChildren2 = go.GetComponentInChildren<CurrentWaveView>();
			int instanceID2 = componentInChildren2.get_gameObject().GetInstanceID();
			array = new object[1]
			{
				componentInChildren2
			};
			entityFactory.BuildEntity<CurrentWaveWidgetEntityDescriptor>(instanceID2, array);
			WaveTransitionView componentInChildren3 = go.GetComponentInChildren<WaveTransitionView>();
			int instanceID3 = componentInChildren3.get_gameObject().GetInstanceID();
			array = new object[1]
			{
				componentInChildren3
			};
			entityFactory.BuildEntity<WaveTransitionEntityDescriptor>(instanceID3, array);
			GameOverPlayerDeadAnimation componentInChildren4 = go.GetComponentInChildren<GameOverPlayerDeadAnimation>();
			int instanceID4 = componentInChildren4.get_gameObject().GetInstanceID();
			array = new object[1]
			{
				componentInChildren4
			};
			entityFactory.BuildEntity<GameOverPlayerEntityDescriptor>(instanceID4, array);
			GameOverNoMoreWavesAnimation componentInChildren5 = go.GetComponentInChildren<GameOverNoMoreWavesAnimation>();
			int instanceID5 = componentInChildren5.get_gameObject().GetInstanceID();
			array = new object[1]
			{
				componentInChildren5
			};
			entityFactory.BuildEntity<GameOverNoMoreWavesEntityDescriptor>(instanceID5, array);
			EliminationImplementor componentInChildren6 = go.GetComponentInChildren<EliminationImplementor>();
			SurvivalImplementor componentInChildren7 = go.GetComponentInChildren<SurvivalImplementor>();
			TimedEliminationImplementor componentInChildren8 = go.GetComponentInChildren<TimedEliminationImplementor>();
			int instanceID6 = componentInChildren6.get_transform().get_parent().get_gameObject()
				.GetInstanceID();
			array = new object[3]
			{
				componentInChildren6,
				componentInChildren7,
				componentInChildren8
			};
			entityFactory.BuildEntity<WaveGoalsWidgetEntityDescriptor>(instanceID6, array);
			HealthBarImplementor componentInChildren9 = go.GetComponentInChildren<HealthBarImplementor>();
			int instanceID7 = componentInChildren9.healthBarGameObject.GetInstanceID();
			HealthBarMachineIdImplementor healthBarMachineIdImplementor = new HealthBarMachineIdImplementor(instanceID7);
			array = new object[2]
			{
				componentInChildren9,
				healthBarMachineIdImplementor
			};
			entityFactory.BuildEntity<HealthBarEntityDescriptor>(instanceID7, array);
		}
	}
}
