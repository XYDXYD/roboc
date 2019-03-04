using UnityEngine;

namespace Simulation.Sight
{
	internal class SpotterStructureImplementor : MonoBehaviour, ISpotterComponent, IOwnerTeamComponent
	{
		[Tooltip("Position from which the structure can spot enemies")]
		[SerializeField]
		private Transform _spotPoint;

		[SerializeField]
		private float _innerSpotRange;

		public float innerSpotRange => _innerSpotRange;

		public Vector3 spotPositionWorld => _spotPoint.get_position();

		public float spotRange => 216f;

		public int ownerTeamId
		{
			get;
			set;
		}

		public SpotterStructureImplementor()
			: this()
		{
		}

		private void Start()
		{
		}
	}
}
