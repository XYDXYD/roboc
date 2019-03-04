using UnityEngine;

internal sealed class CubeTypeSelectorButton : MonoBehaviour
{
	public GameObject AnimationContainer;

	private string OutroAnimation = "InventoryOutro";

	public CubeTypeSelectorButton()
		: this()
	{
	}

	private void OnClick()
	{
		AnimationContainer.GetComponent<Animation>().Play(OutroAnimation);
	}
}
