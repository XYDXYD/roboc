using Simulation.Hardware.Weapons;
using UnityEngine;

internal interface ICubeFactory
{
	GameObject BuildCube(CubeTypeID ID, Vector3 position, Quaternion rotation, TargetType tagretType);
}
