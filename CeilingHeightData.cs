using Svelto.IoC;
using UnityEngine;

internal class CeilingHeightData : MonoBehaviour, IInitialize
{
	public float maxCeilingHeight = 1000f;

	[Inject]
	internal CeilingHeightManager ceilingHeightManager
	{
		private get;
		set;
	}

	public CeilingHeightData()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		ceilingHeightManager.SetMaxCeilingHeight(maxCeilingHeight);
	}
}
