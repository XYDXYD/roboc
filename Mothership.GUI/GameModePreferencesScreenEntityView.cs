using Game.ECS.GUI.Components;
using Svelto.ECS;

namespace Mothership.GUI
{
	internal sealed class GameModePreferencesScreenEntityView : EntityView
	{
		public IShowComponent showComponent;

		public IDialogChoiceComponent dialogChoiceComponent;

		public IGameModePreferencesWidgetComponent preferencesComponent;

		public IFormValidationComponent formValidationComponent;

		public GameModePreferencesScreenEntityView()
			: this()
		{
		}
	}
}
