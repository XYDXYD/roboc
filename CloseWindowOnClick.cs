using UnityEngine;

internal class CloseWindowOnClick : MonoBehaviour
{
	public GameObject parent;

	public CloseWindowOnClick()
		: this()
	{
	}

	public void Close()
	{
		parent.SetActive(false);
	}
}
