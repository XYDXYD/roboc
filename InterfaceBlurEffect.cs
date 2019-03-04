using Kino;
using Svelto.IoC;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

internal sealed class InterfaceBlurEffect : MonoBehaviour
{
	private BlurOptimized blurEffect;

	private ScreenOverlay screenOverlay;

	private Obscurance SSAOEffect;

	private bool isSSAOActive;

	[Inject]
	internal BlurEffectController blurEffectController
	{
		private get;
		set;
	}

	public InterfaceBlurEffect()
		: this()
	{
	}

	private void Start()
	{
		blurEffectController.interfaceBlurEffect = this;
		blurEffect = this.GetComponent<BlurOptimized>();
		screenOverlay = this.GetComponent<ScreenOverlay>();
		SSAOEffect = this.GetComponent<Obscurance>();
		if (SSAOEffect != null)
		{
			isSSAOActive = SSAOEffect.get_enabled();
		}
		EnableEffects(enable: false);
	}

	public void EnableEffects(bool enable)
	{
		if (enable)
		{
			blurEffect.set_enabled(true);
			screenOverlay.set_enabled(true);
			if (isSSAOActive)
			{
				SSAOEffect.set_enabled(false);
			}
		}
		else
		{
			blurEffect.set_enabled(false);
			screenOverlay.set_enabled(false);
			if (isSSAOActive)
			{
				SSAOEffect.set_enabled(true);
			}
		}
	}
}
