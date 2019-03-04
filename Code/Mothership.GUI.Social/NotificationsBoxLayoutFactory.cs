using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership.GUI.Social
{
	internal sealed class NotificationsBoxLayoutFactory
	{
		[Inject]
		internal IComponentFactory genericComponentFactory
		{
			get;
			private set;
		}

		public void Build(NotificationsBoxView view, IDataSource onlinePlayersDataSource, int dataSourceIndex = 0)
		{
			BuiltComponentElements builtComponentElements = genericComponentFactory.BuildComponent(GUIComponentType.TextLabel, onlinePlayersDataSource, view.numberLabelTemplate);
			((GenericTextLabelComponent)builtComponentElements.componentController).SetDataSourceIndex(dataSourceIndex);
			builtComponentElements.componentController.SetName("OnlineNumberLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.numberLabelTemplate.get_transform().get_parent());
		}
	}
}
