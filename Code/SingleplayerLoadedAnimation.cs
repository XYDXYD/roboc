using Simulation;
using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

internal sealed class SingleplayerLoadedAnimation : MonoBehaviour, IInitialize
{
	public AnimationClip clip;

	[Inject]
	internal IInitialSimulationGUIFlow initialSimulationGUIFlow
	{
		private get;
		set;
	}

	[Inject]
	internal GameStartDispatcher gameStartDispatcher
	{
		private get;
		set;
	}

	public SingleplayerLoadedAnimation()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		if (!WorldSwitching.IsMultiplayer())
		{
			Register();
		}
	}

	private void OnDestroy()
	{
		if (!WorldSwitching.IsMultiplayer())
		{
			Unregister();
		}
	}

	private void Register()
	{
		initialSimulationGUIFlow.OnGuiFlowComplete += HandleOnGuiFlowComplete;
	}

	private void Unregister()
	{
		initialSimulationGUIFlow.OnGuiFlowComplete -= HandleOnGuiFlowComplete;
	}

	private void HandleOnGuiFlowComplete()
	{
		PlayAnimation();
	}

	private void PlayAnimation()
	{
		if (this != null)
		{
			Animation component = this.GetComponent<Animation>();
			if (component != null && clip != null)
			{
				component.Play(clip.get_name());
				TaskRunner.get_Instance().Run(CheckAnimation(component));
			}
			else
			{
				gameStartDispatcher.OnGameStart();
			}
		}
	}

	private IEnumerator CheckAnimation(Animation anim)
	{
		while (anim.get_isPlaying())
		{
			yield return null;
		}
		gameStartDispatcher.OnGameStart();
	}
}
