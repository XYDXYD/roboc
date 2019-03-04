using Battle;
using Svelto.Context;
using System;

namespace Simulation.Battle
{
	internal class BattleParametersSinglePlayer : BattleParameters, IWaitForFrameworkInitialization
	{
		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			BattleParametersData parameters = new BattleParametersData("localhost", 0, "RC_Planet_Earth_01", new GameModeKey(GameModeType.PraticeMode), DateTime.UtcNow, null, null, "<singleplayer_guid>");
			base.OnReceivedParameters(parameters);
		}
	}
}
