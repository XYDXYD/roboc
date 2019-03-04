using UnityEngine;

namespace Mothership.OpsRoom
{
	internal class TechTreeCTAImplementor : MonoBehaviour, ITechTreeCTAComponent
	{
		[SerializeField]
		private GameObject _gameObject;

		[SerializeField]
		private UILabel _label;

		public GameObject gameObject => _gameObject;

		public UILabel label => _label;

		public TechTreeCTAImplementor()
			: this()
		{
		}
	}
}
