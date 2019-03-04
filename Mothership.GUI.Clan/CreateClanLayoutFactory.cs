using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership.GUI.Clan
{
	internal sealed class CreateClanLayoutFactory
	{
		[Inject]
		internal IComponentFactory genericComponentFactory
		{
			get;
			private set;
		}

		public void BuildAll(CreateClanView view)
		{
			BuiltComponentElements builtComponentElements = genericComponentFactory.BuildComponent(GUIComponentType.TextLabel, null, view.errorLabelTemplate);
			builtComponentElements.componentController.SetName("CreateClanErrorLabel");
			builtComponentElements.componentController.Activate();
			builtComponentElements.componentView.ReparentOnly(view.errorLabelTemplate.get_transform().get_parent());
		}
	}
}
