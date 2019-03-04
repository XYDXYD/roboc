namespace Simulation
{
	internal interface IHudStyleController
	{
		void SetStyle(HudStyle style);

		void AddHud(IHudElement view);

		void RemoveHud(IHudElement view);
	}
}
