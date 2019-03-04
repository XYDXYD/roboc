using Simulation.Destruction;
using Svelto.IoC;
using UnityEngine;

internal sealed class DeadCubeEffectData : MonoBehaviour, IInitialize
{
	public GameObject deadCubePrefabPlayer;

	public GameObject debrisPrefabPlayer;

	public GameObject protoniumDestroy;

	public GameObject protoniumDestroy_E;

	public GameObject protoniumDestroy_N;

	[Inject]
	internal VisualDestructionEffects destructionEffects
	{
		private get;
		set;
	}

	public DeadCubeEffectData()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		destructionEffects.SetEffectData(this);
	}
}
