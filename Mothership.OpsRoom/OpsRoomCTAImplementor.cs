using UnityEngine;

namespace Mothership.OpsRoom
{
	internal class OpsRoomCTAImplementor : MonoBehaviour, IOpsRoomCTAComponent
	{
		[SerializeField]
		private GameObject _gameObject;

		[SerializeField]
		private UILabel _label;

		public GameObject gameObject => _gameObject;

		public UILabel label => _label;

		public OpsRoomCTAImplementor()
			: this()
		{
		}
	}
}
