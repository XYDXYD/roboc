using UnityEngine;

namespace Mothership.GUI
{
	public class PlayerLevelView_Infinity : MonoBehaviour, IPlayerLevelView
	{
		public UILabel playerLevelLabel;

		public UILabel playerNameLabel;

		public UISprite playerLevelFilledSprite;

		public PlayerLevelView_Infinity()
			: this()
		{
		}

		void IPlayerLevelView.InjectController(PlayerLevelController controller)
		{
		}

		public void Show()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localPosition(Vector3.get_zero());
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public GameObject GetGameObject()
		{
			return this.get_gameObject();
		}

		public GameObject GetPlayerNameGameObject()
		{
			return playerNameLabel.get_gameObject();
		}

		public GameObject GetLabelGameObject()
		{
			return playerLevelLabel.get_gameObject();
		}

		public GameObject GetProgressGameObject()
		{
			return playerLevelFilledSprite.get_gameObject();
		}
	}
}
