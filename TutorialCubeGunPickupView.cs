using UnityEngine;

internal class TutorialCubeGunPickupView : MonoBehaviour
{
	public GameObject displayPanel;

	public TutorialCubeGunPickupView()
		: this()
	{
	}

	public void ShowCounter(bool show)
	{
		displayPanel.SetActive(show);
	}
}
