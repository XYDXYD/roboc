using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class BrawlButtonFactory
	{
		[Inject]
		internal IComponentFactory componentFactory
		{
			private get;
			set;
		}

		public void BuildAll(BrawlButton view, BrawlDetailsDataSource dataSource)
		{
			BuildLabel(view.title, dataSource, BrawlDetailsFields.TITLE);
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.highlightStateBGImage, makeInstance: false);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(7);
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.normalStateBGImage, makeInstance: false);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(8);
			builtComponentElements = componentFactory.BuildComponent(GUIComponentType.Image, dataSource, view.lockedStateBGImage, makeInstance: false);
			builtComponentElements.componentController.Activate();
			(builtComponentElements.componentController as GenericImageComponent).SetDataSourceIndex(8);
			view.Initialize();
		}

		private void BuildLabel(GameObject go, IDataSource dataSource, BrawlDetailsFields dataField)
		{
			BuiltComponentElements builtComponentElements = componentFactory.BuildComponent(GUIComponentType.TextLabel, dataSource, go, makeInstance: false);
			GenericTextLabelComponent genericTextLabelComponent = (GenericTextLabelComponent)builtComponentElements.componentController;
			genericTextLabelComponent.Activate();
			genericTextLabelComponent.SetDataSourceIndex((int)dataField);
		}
	}
}
