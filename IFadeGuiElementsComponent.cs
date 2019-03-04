using Svelto.ES.Legacy;

internal interface IFadeGuiElementsComponent : IComponent
{
	float GetFadeAwaySpeed();

	float GetCurrentAlpha();

	void SetCurrentAlpha(float alpha);
}
