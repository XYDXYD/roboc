using UnityEngine;

namespace Mothership.GarageSkins
{
	public class GarageBaySkinImplementor : MonoBehaviour
	{
		[SerializeField]
		private SkyDomeController SkyDome;

		[SerializeField]
		private GameObject[] GameObjectsToDisableInBuildMode;

		[SerializeField]
		private GameObject[] GameObjectsToDisableInGarageMode;

		[SerializeField]
		private GameObject[] GameObjectsToDisableIfMegabot;

		public GarageBaySkinImplementor()
			: this()
		{
		}

		private void ToggleGameObjects(WorldSwitchMode mode)
		{
			for (int i = 0; i < GameObjectsToDisableInBuildMode.Length; i++)
			{
				GameObjectsToDisableInBuildMode[i].SetActive(mode != WorldSwitchMode.BuildMode);
			}
			for (int j = 0; j < GameObjectsToDisableInGarageMode.Length; j++)
			{
				GameObjectsToDisableInGarageMode[j].SetActive(mode != WorldSwitchMode.GarageMode);
			}
			if (mode == WorldSwitchMode.GarageMode)
			{
				EnableSkyDome();
			}
		}

		private void ToggleMegabotGameObjects(bool isMegabot)
		{
			for (int i = 0; i < GameObjectsToDisableIfMegabot.Length; i++)
			{
				GameObjectsToDisableIfMegabot[i].SetActive(!isMegabot);
			}
		}

		private void OnEnable()
		{
			EnableSkyDome();
		}

		private void EnableSkyDome()
		{
			SkyDome.get_gameObject().SetActive(true);
			SkyDome.set_enabled(true);
		}

		private void OnDisable()
		{
			SkyDome.get_gameObject().SetActive(false);
			SkyDome.set_enabled(false);
		}

		private void OnDestroy()
		{
			Object.Destroy(SkyDome.get_gameObject());
		}
	}
}
