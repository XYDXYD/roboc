using UnityEngine;

namespace Simulation.Sight
{
	internal class LocalPlayerSpotterImplementor : ISpotterComponent, IFrustumComponent
	{
		public Vector3 spotPositionWorld => attachedCamera.get_transform().get_position();

		public float spotRange => 216f;

		public Camera attachedCamera => Camera.get_main();

		public float innerSpotRange => 0f;
	}
}
