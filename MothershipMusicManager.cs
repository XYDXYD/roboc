using Fabric;
using Mothership;
using Svelto.Context;
using Svelto.IoC;
using System.Collections;
using UnityEngine;

internal sealed class MothershipMusicManager : MonoBehaviour, IWaitForFrameworkDestruction, IInitialize
{
	public string MusicFabricName;

	public string LobbyMusicFabricName;

	public string MenuMusicFabricName;

	[Inject]
	internal BattleCountdownScreenController lobbyScreen
	{
		private get;
		set;
	}

	[Inject]
	internal IDispatchWorldSwitching worldSwitch
	{
		private get;
		set;
	}

	public MothershipMusicManager()
		: this()
	{
	}

	private void Start()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		lobbyScreen.OnBattleCountdownShown += HandleOnBattleCountdownShown;
		lobbyScreen.OnBattleCountdownHidden += HandleOnBattleCountdownHidden;
		worldSwitch.OnWorldJustSwitched += HandleOnWorldJustSwitched;
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		worldSwitch.OnWorldJustSwitched -= HandleOnWorldJustSwitched;
	}

	private IEnumerator OnWorldSwitching()
	{
		if (worldSwitch.SwitchingFrom == WorldSwitchMode.BuildMode)
		{
			StopMusic(MusicFabricName);
		}
		else
		{
			StopMusic(MenuMusicFabricName);
		}
		yield break;
	}

	private void HandleOnWorldJustSwitched(WorldSwitchMode currentMode)
	{
		worldSwitch.OnWorldIsSwitching.Add(OnWorldSwitching());
		if (currentMode == WorldSwitchMode.BuildMode)
		{
			StartMusic(MusicFabricName);
		}
		else
		{
			StartMusic(MenuMusicFabricName);
		}
	}

	private void HandleOnBattleCountdownHidden()
	{
		EventManager.get_Instance().PostEvent(LobbyMusicFabricName, 1, (object)null, null);
		EventManager.get_Instance().PostEvent(MenuMusicFabricName, 0, (object)null, null);
	}

	private void HandleOnBattleCountdownShown()
	{
		EventManager.get_Instance().PostEvent(MenuMusicFabricName, 1, (object)null, null);
		EventManager.get_Instance().PostEvent(LobbyMusicFabricName, 0, (object)null, null);
	}

	public void OnDestroy()
	{
		lobbyScreen.OnBattleCountdownShown -= HandleOnBattleCountdownShown;
		lobbyScreen.OnBattleCountdownHidden -= HandleOnBattleCountdownHidden;
	}

	public void StartMusic(string musicName)
	{
		EventManager.get_Instance().PostEvent(musicName, 0, (object)null, null);
	}

	public void StopMusic(string musicName)
	{
		EventManager.get_Instance().PostEvent(musicName, 1, (object)null, null);
	}
}
