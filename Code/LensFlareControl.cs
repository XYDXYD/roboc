using UnityEngine;

public class LensFlareControl : MonoBehaviour
{
	public LensFlare lf;

	public LensFlareControl()
		: this()
	{
	}

	private void Update()
	{
		lf.set_brightness(Random.Range(0f, 2f));
	}
}
