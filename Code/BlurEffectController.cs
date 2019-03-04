internal sealed class BlurEffectController
{
	public InterfaceBlurEffect interfaceBlurEffect;

	public void EnableEffect(bool enable)
	{
		if (interfaceBlurEffect != null)
		{
			interfaceBlurEffect.EnableEffects(enable);
		}
	}
}
