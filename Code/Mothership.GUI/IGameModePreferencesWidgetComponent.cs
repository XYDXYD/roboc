using Svelto.ECS;
using System.Collections.Generic;

namespace Mothership.GUI
{
	internal interface IGameModePreferencesWidgetComponent
	{
		List<GameModeType> availableGameModeTypes
		{
			get;
			set;
		}

		DispatchOnChange<GameModePreferences> preferences
		{
			get;
			set;
		}
	}
}
