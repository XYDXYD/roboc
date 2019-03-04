using UnityEngine;

namespace Mothership.OpsRoom
{
	internal class QuestsCTAImplementor : MonoBehaviour, IQuestsCTAComponent
	{
		[SerializeField]
		private GameObject _gameObject;

		[SerializeField]
		private UILabel _label;

		public GameObject gameObject => _gameObject;

		public UILabel label => _label;

		public QuestsCTAImplementor()
			: this()
		{
		}
	}
}
