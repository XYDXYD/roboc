using BehaviorDesigner.Runtime;
using System;

public class MainSimulationContextHolderLoader : UnityContext
{
	public MainSimulationContextHolderLoader()
		: this()
	{
	}

	private void Awake()
	{
		this.OnAwake();
	}

	protected override void OnAwake()
	{
		MainSimulationDynamicUICreator component = this.GetComponent<MainSimulationDynamicUICreator>();
		if (component != null)
		{
			component.BuildAllDynamicPrefabs();
		}
		if (WorldSwitching.GetGameModeType() == GameModeType.TestMode)
		{
			this.get_gameObject().AddComponent<SimulationContextTestMode>();
		}
		else if (WorldSwitching.GetGameModeType() == GameModeType.PraticeMode)
		{
			this.get_gameObject().AddComponent<SimulationContextTeamDeathMatchAI>();
			this.get_gameObject().AddComponent<BehaviorManager>();
			BehaviorManager.instance.set_UpdateInterval(2);
		}
		else if (WorldSwitching.GetGameModeType() == GameModeType.TeamDeathmatch)
		{
			this.get_gameObject().AddComponent<SimulationContextTeamDeathMatch>();
			this.get_gameObject().AddComponent<BehaviorManager>();
			BehaviorManager.instance.set_UpdateInterval(2);
		}
		else if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
		{
			this.get_gameObject().AddComponent<SimulationContextNormalMode>();
			this.get_gameObject().AddComponent<BehaviorManager>();
			BehaviorManager.instance.set_UpdateInterval(2);
		}
		else if (WorldSwitching.GetGameModeType() == GameModeType.Pit)
		{
			this.get_gameObject().AddComponent<SimulationContextPit>();
		}
		else if (WorldSwitching.GetGameModeType() == GameModeType.SuddenDeath)
		{
			this.get_gameObject().AddComponent<SimulationContextSuddenDeath>();
			this.get_gameObject().AddComponent<BehaviorManager>();
			BehaviorManager.instance.set_UpdateInterval(2);
		}
		else
		{
			if (WorldSwitching.GetGameModeType() != GameModeType.Campaign)
			{
				throw new Exception("Game mode not found in MainSimulationContextHolderLoader");
			}
			this.get_gameObject().AddComponent<SimulationContextCampaign>();
			this.get_gameObject().AddComponent<BehaviorManager>();
			BehaviorManager.instance.set_UpdateInterval(2);
		}
		this.set_enabled(false);
	}
}
