using UnityEngine;

public class GIMaterialAnimator : MonoBehaviour
{
	public GameObject interior;

	public bool test;

	public GIMaterialAnimator()
		: this()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (test)
		{
			RendererExtensions.UpdateGIMaterials(interior.GetComponent<Renderer>());
		}
		test = false;
	}
}
