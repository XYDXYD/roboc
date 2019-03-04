using UnityEngine;

namespace Utilities
{
	internal class SwitchGameObjects : MonoBehaviour
	{
		public GameObject[] objectsToSwitch;

		public SwitchGameObjects()
			: this()
		{
		}

		private void OnEnable()
		{
			for (int i = 0; i < objectsToSwitch.Length; i++)
			{
				if (objectsToSwitch[i] != null)
				{
					objectsToSwitch[i].SetActive(false);
				}
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < objectsToSwitch.Length; i++)
			{
				if (objectsToSwitch[i] != null)
				{
					objectsToSwitch[i].SetActive(true);
				}
			}
		}
	}
}
