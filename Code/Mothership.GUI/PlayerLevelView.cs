using System;
using UnityEngine;

namespace Mothership.GUI
{
	public class PlayerLevelView : MonoBehaviour, IPlayerLevelView
	{
		public UILabel playerLevelLabel;

		public UISlider playerLevelSlider;

		public PlayerLevelView()
			: this()
		{
		}

		void IPlayerLevelView.InjectController(PlayerLevelController controller)
		{
		}

		public void Show()
		{
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

		public GameObject GetLabelGameObject()
		{
			return playerLevelLabel.get_gameObject();
		}

		public GameObject GetPlayerNameGameObject()
		{
			throw new Exception("The original player level view does not display player name");
		}

		public GameObject GetProgressGameObject()
		{
			return playerLevelSlider.get_gameObject();
		}
	}
}
