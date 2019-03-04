using UnityEngine;

namespace Mothership.GUI
{
	internal interface IPlayerLevelView
	{
		void InjectController(PlayerLevelController controller);

		void Show();

		void Hide();

		GameObject GetPlayerNameGameObject();

		GameObject GetGameObject();

		GameObject GetProgressGameObject();

		GameObject GetLabelGameObject();
	}
}
