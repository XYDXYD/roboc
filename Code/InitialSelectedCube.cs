using Svelto.IoC;
using UnityEngine;

public sealed class InitialSelectedCube : MonoBehaviour, IInitialize
{
	public int StartingCubeID = (int)CubeTypeID.StandardCubeID;

	private static bool _initialised;

	[Inject]
	internal ICubeHolder selectedCube
	{
		private get;
		set;
	}

	public InitialSelectedCube()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		if (!_initialised)
		{
			selectedCube.selectedCubeID = new CubeTypeID((uint)StartingCubeID);
			selectedCube.currentPaletteId = 0;
			_initialised = true;
		}
		else
		{
			CubeTypeID selectedCubeID = selectedCube.selectedCubeID;
			selectedCube.selectedCubeID = CubeTypeID.StandardCubeID;
			selectedCube.selectedCubeID = selectedCubeID;
		}
	}
}
