using Svelto.ES.Legacy;
using UnityEngine;

internal sealed class UIFadeIn : MonoBehaviour, IRunOnWorldSwitching, IComponent
{
	public bool fadeIn;

	private Transform T;

	public bool FadeIn => fadeIn;

	public int Priority => 0;

	public float Duration => 0f;

	public UIFadeIn()
		: this()
	{
	}

	private void Awake()
	{
		T = this.get_transform();
	}

	public void Execute(WorldSwitchMode currentMode)
	{
		for (int i = 0; i < T.get_childCount(); i++)
		{
			T.GetChild(i).get_gameObject().SetActive(fadeIn);
		}
	}
}
