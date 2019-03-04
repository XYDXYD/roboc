using Svelto.IoC;
using UnityEngine;

internal sealed class GroundHeightMarker : MonoBehaviour
{
	[Inject]
	public GroundHeight groundHeight
	{
		private get;
		set;
	}

	public GroundHeightMarker()
		: this()
	{
	}

	private void Start()
	{
		groundHeight.SetGroundHeight(this);
	}
}
