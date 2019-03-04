using Svelto.IoC;
using UnityEngine;

internal sealed class VisibleOnScreenData : MonoBehaviour, IInitialize
{
	public OptimizationsOnVisibilitySettings kinematicInfo;

	[Inject]
	public VisibleOnScreenManager visiblityManager
	{
		private get;
		set;
	}

	public VisibleOnScreenData()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		visiblityManager.toggleOptimizationsSettings = kinematicInfo;
	}
}
