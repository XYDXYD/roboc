using UnityEngine;

namespace Simulation.Sight
{
	internal class AIPlayerSpotterImplementor : MonoBehaviour, ISpotterComponent
	{
		private Vector3 _spotPoint;

		public Vector3 spotPositionWorld => _spotPoint;

		public float spotRange => 216f;

		public float innerSpotRange => 0f;

		public AIPlayerSpotterImplementor()
			: this()
		{
		}
	}
}
