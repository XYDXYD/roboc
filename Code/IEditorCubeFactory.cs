using UnityEngine;

internal interface IEditorCubeFactory : ICubeFactory
{
	GameObject BuildDummyCube(CubeTypeID ID, bool onGrid);
}
