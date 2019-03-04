using Svelto.Ticker.Legacy;

namespace Simulation
{
	internal sealed class GUIShortcutTicker_Simulation : ITickable, ITickableBase
	{
		private IGUIInputControllerSim _inputController;

		public GUIShortcutTicker_Simulation(IGUIInputControllerSim inputController)
		{
			_inputController = inputController;
		}

		public void Tick(float delta)
		{
			if (_inputController.GetShortCutMode() != 0 && InputRemapper.Instance.GetButtonDown("Quit"))
			{
				_inputController.HandleQuitPressed();
			}
		}
	}
}
